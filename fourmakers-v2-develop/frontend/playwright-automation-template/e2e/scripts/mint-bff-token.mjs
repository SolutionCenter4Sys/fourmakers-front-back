#!/usr/bin/env node
/**
 * mint-bff-token.mjs — gera/reusa JWT via OTP QA (sem ler e-mail).
 *
 * Uso:
 *   node e2e/scripts/mint-bff-token.mjs
 *   node e2e/scripts/mint-bff-token.mjs --force
 *   BFF_FORCE_REFRESH=1 node e2e/scripts/mint-bff-token.mjs
 *
 * Cache: e2e/.bff-token-cache.json (gitignored)
 * Env: OTP_API_BASE_URL, OTP_EMAIL, OTP_ORG_ID, OTP_SYSTEM_TOKEN
 *      BFF_TOKEN (se setado e válido, não chama OTP)
 */
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '../..');
const cacheFile = path.join(root, 'e2e', '.bff-token-cache.json');

/** Carrega .env local sem dotenv. */
function loadEnvFile(filePath) {
  if (!fs.existsSync(filePath)) return;
  for (const line of fs.readFileSync(filePath, 'utf8').split(/\r?\n/)) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith('#')) continue;
    const eq = trimmed.indexOf('=');
    if (eq === -1) continue;
    const key = trimmed.slice(0, eq).trim();
    const value = trimmed.slice(eq + 1).trim();
    if (key && process.env[key] === undefined) process.env[key] = value;
  }
}
loadEnvFile(path.join(root, '.env'));

const force =
  process.argv.includes('--force') ||
  ['1', 'true', 'yes'].includes(String(process.env.BFF_FORCE_REFRESH || '').toLowerCase());

function loadSystemTokenFromCredenciais() {
  const candidates = [
    path.resolve(root, '../../DEMO/setup/demo-credenciais.json'),
    path.resolve(root, '../../../DEMO/setup/demo-credenciais.json'),
    path.resolve(process.cwd(), 'DEMO/setup/demo-credenciais.json'),
  ];
  for (const credPath of candidates) {
    try {
      if (!fs.existsSync(credPath)) continue;
      const cred = JSON.parse(fs.readFileSync(credPath, 'utf8'));
      if (cred.systemTokenBase64) return String(cred.systemTokenBase64);
    } catch {
      /* ignore */
    }
  }
  return '';
}

const cfg = {
  baseUrl: (process.env.OTP_API_BASE_URL || 'https://spw.app.foursys.com/backoffice-rf-hom').replace(
    /\/$/,
    '',
  ),
  email: process.env.OTP_EMAIL || 'usuario_qa@foursys.com.br',
  orgId: Number(process.env.OTP_ORG_ID || 5),
  systemToken: (process.env.OTP_SYSTEM_TOKEN || '').trim() || loadSystemTokenFromCredenciais(),
  pollTimeoutMs: 8_000,
  pollIntervalMs: 500,
};

function decodeJwt(token) {
  try {
    const part = token.split('.')[1];
    const b64 = part.replace(/-/g, '+').replace(/_/g, '/');
    const padded = b64 + '='.repeat((4 - (b64.length % 4)) % 4);
    return JSON.parse(Buffer.from(padded, 'base64').toString('utf8'));
  } catch {
    return null;
  }
}

function isUsable(token) {
  const payload = decodeJwt(token);
  if (!payload?.exp) return false;
  return payload.exp > Math.floor(Date.now() / 1000) + 120;
}

function readCache() {
  if (process.env.BFF_TOKEN && isUsable(process.env.BFF_TOKEN)) {
    return { token: process.env.BFF_TOKEN, source: 'env:BFF_TOKEN' };
  }
  if (!fs.existsSync(cacheFile)) return null;
  try {
    const data = JSON.parse(fs.readFileSync(cacheFile, 'utf8'));
    if (data?.token && isUsable(data.token)) {
      return { token: data.token, source: 'cache', ...data };
    }
  } catch {
    /* ignore */
  }
  return null;
}

function writeCache(token) {
  const payload = decodeJwt(token) || {};
  const data = {
    token,
    email: cfg.email,
    orgId: cfg.orgId,
    exp: payload.exp || null,
    mintedAt: new Date().toISOString(),
  };
  fs.mkdirSync(path.dirname(cacheFile), { recursive: true });
  fs.writeFileSync(cacheFile, `${JSON.stringify(data, null, 2)}\n`, 'utf8');
  return data;
}

async function sleep(ms) {
  await new Promise((r) => setTimeout(r, ms));
}

async function mint() {
  const envia = await fetch(`${cfg.baseUrl}/api/Acesso/EnviaTokenAcessoEmail`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: cfg.email, orgId: cfg.orgId }),
  });
  const enviaBody = await envia.json().catch(() => ({}));
  if (
    envia.status === 429 ||
    /limite de tentativas/i.test(String(enviaBody.mensagem || ''))
  ) {
    throw new Error(
      `RATE LIMIT em EnviaTokenAcessoEmail — não reenviar.\n${JSON.stringify(enviaBody)}`,
    );
  }
  if (!envia.ok || !enviaBody.sucesso) {
    throw new Error(`EnviaToken falhou HTTP ${envia.status}: ${JSON.stringify(enviaBody)}`);
  }

  const url =
    `${cfg.baseUrl}/api/Acesso/ObtemCodigoAcessoEmailQA` +
    `?email=${encodeURIComponent(cfg.email)}&orgId=${cfg.orgId}`;
  const deadline = Date.now() + cfg.pollTimeoutMs;
  let codigo = null;
  let last = '';
  while (Date.now() < deadline) {
    const obt = await fetch(url, {
      headers: { Authorization: `Bearer ${cfg.systemToken}` },
    });
    const body = await obt.json().catch(() => ({}));
    if (obt.ok && body.sucesso && body.codigo) {
      codigo = body.codigo;
      break;
    }
    last = JSON.stringify(body);
    await sleep(cfg.pollIntervalMs);
  }
  if (!codigo) throw new Error(`ObtemCodigoQA sem código. Última: ${last}`);

  const val = await fetch(`${cfg.baseUrl}/api/Acesso/ValidaTokenAcessoEmail`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: cfg.email, token: codigo, orgId: cfg.orgId }),
  });
  const valBody = await val.json().catch(() => ({}));
  if (!val.ok || !valBody.sucesso || !valBody.token) {
    throw new Error(`ValidaToken falhou HTTP ${val.status}: ${JSON.stringify(valBody)}`);
  }
  return valBody.token;
}

async function main() {
  if (!force) {
    const cached = readCache();
    if (cached?.token) {
      const payload = decodeJwt(cached.token);
      console.log(`[mint] Reusando JWT (${cached.source}) — SEM EnviaToken`);
      console.log(
        `[mint] exp=${payload?.exp ? new Date(payload.exp * 1000).toISOString() : 'n/a'}`,
      );
      console.log(cached.token);
      process.exit(0);
    }
  } else {
    console.log('[mint] --force / BFF_FORCE_REFRESH: gerando OTP novo...');
  }

  if (!cfg.systemToken) {
    throw new Error(
      'OTP_SYSTEM_TOKEN ausente. Defina no .env / CI ou rode npm run demo:preparar.',
    );
  }

  const token = await mint();
  const saved = writeCache(token);
  console.log(`[mint] JWT novo salvo em ${path.relative(root, cacheFile)}`);
  console.log(
    `[mint] exp=${saved.exp ? new Date(saved.exp * 1000).toISOString() : 'n/a'}`,
  );
  console.log(token);
}

main().catch((err) => {
  console.error('[mint] Falha:', err.message || err);
  process.exit(1);
});
