/**
 * Verificações de ambiente da Demo Reembolso (pré-setup).
 * CLI: node DEMO/scripts/demo-env-check.cjs
 */
const fs = require('node:fs');
const net = require('node:net');
const path = require('node:path');

const { loadCredenciais } = require('./demo-credenciais.cjs');

const demoRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(demoRoot, '..');
const frontendRoot = path.join(repoRoot, 'frontend');
const envLocalPath = path.join(frontendRoot, '.env.local');

const CRED = (() => {
  try {
    return loadCredenciais();
  } catch {
    return {
      backend: 'https://spw.app.foursys.com/backoffice-rf-hom',
      email: 'usuario_qa@foursys.com.br',
      orgId: 5,
      systemTokenRaw: '5YB40IzcB7x83WsFYK0qCioG6i3lFhP3qlYbCirzmc995KtB8B',
      orgNome: 'Showcase',
    };
  }
})();

const DEFAULT_HOM_URL = CRED.backend;
const DEMO_EMAIL = CRED.email;
const DEMO_SYSTEM_TOKEN_RAW = CRED.systemTokenRaw;
const DEMO_ORG_ID = Number(CRED.orgId);
const DEMO_ORG_NOME = CRED.orgNome || 'Showcase';
const VITE_DEV_PORT = 8080;
const REQUEST_TIMEOUT_MS = 20_000;

const FIREBASE_VARS = [
  'VITE_FIREBASE_API_KEY',
  'VITE_FIREBASE_AUTH_DOMAIN',
  'VITE_FIREBASE_PROJECT_ID',
  'VITE_FIREBASE_STORAGE_BUCKET',
  'VITE_FIREBASE_MESSAGING_SENDER_ID',
  'VITE_FIREBASE_APP_ID',
];

function parseEnvFile(filePath) {
  if (!fs.existsSync(filePath)) return {};
  const vars = {};
  for (const line of fs.readFileSync(filePath, 'utf8').split(/\r?\n/)) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith('#')) continue;
    const eq = trimmed.indexOf('=');
    if (eq === -1) continue;
    const key = trimmed.slice(0, eq).trim();
    let value = trimmed.slice(eq + 1).trim();
    if (
      (value.startsWith('"') && value.endsWith('"')) ||
      (value.startsWith("'") && value.endsWith("'"))
    ) {
      value = value.slice(1, -1);
    }
    vars[key] = value;
  }
  return vars;
}

function isTcpPortOpen(port, host = 'localhost', timeoutMs = 1500) {
  return new Promise((resolve) => {
    const socket = new net.Socket();
    const onDone = (open) => {
      socket.destroy();
      resolve(open);
    };
    socket.setTimeout(timeoutMs);
    socket.once('connect', () => onDone(true));
    socket.once('timeout', () => onDone(false));
    socket.once('error', () => onDone(false));
    socket.connect(port, host);
  });
}

async function fetchWithTimeout(url, options = {}, timeoutMs = REQUEST_TIMEOUT_MS) {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), timeoutMs);
  try {
    return await fetch(url, { ...options, signal: controller.signal });
  } finally {
    clearTimeout(timer);
  }
}

function pushCheck(checks, id, label, status, detail) {
  checks.push({ id, label, status, detail });
}

function printChecks(checks) {
  for (const item of checks) {
    const icon =
      item.status === 'ok' ? '✅' : item.status === 'aviso' ? '⚠️' : '❌';
    console.log(`  ${icon} ${item.label}`);
    if (item.detail) console.log(`     ${item.detail}`);
  }
}

async function checkBackendComOtp(checks, homBase, demoEmail, demoOrgId) {
  try {
    const response = await fetchWithTimeout(`${homBase}/api/Acesso/EnviaTokenAcessoEmail`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${DEMO_SYSTEM_TOKEN_RAW}`,
      },
      body: JSON.stringify({ email: demoEmail, orgId: demoOrgId }),
    });
    let body = {};
    try {
      body = await response.json();
    } catch {
      body = {};
    }

    if (!response.ok) {
      const msg = body.mensagem || `${response.status} ${response.statusText}`;
      const isUserMissing =
        typeof msg === 'string' &&
        msg.toLowerCase().includes('nenhum usu') &&
        msg.toLowerCase().includes('e-mail');
      const isRateLimit =
        typeof msg === 'string' && msg.toLowerCase().includes('limite de tentativas');
      if (isRateLimit) {
        pushCheck(
          checks,
          'hom-api',
          'Backend HML (API Acesso)',
          'ok',
          `API alcancavel; rate limit OTP ativo para "${demoEmail}" (orgId ${demoOrgId}). Aguarde alguns minutos antes do smoke.`,
        );
      } else {
        pushCheck(
          checks,
          'hom-api',
          'Backend HML (API Acesso)',
          isUserMissing ? 'falha' : 'aviso',
          isUserMissing
            ? `${msg} — usuario demo "${demoEmail}" indisponivel no backend (orgId ${demoOrgId}).`
            : `HTTP ${response.status}: ${msg}`,
        );
      }
    } else if (body.sucesso) {
      pushCheck(
        checks,
        'hom-api',
        'Backend HML (API Acesso)',
        'ok',
        `Respondeu; usuario demo "${demoEmail}" encontrado (orgId ${demoOrgId} / ${DEMO_ORG_NOME}).`,
      );
    } else {
      pushCheck(
        checks,
        'hom-api',
        'Backend HML (API Acesso)',
        'aviso',
        body.mensagem || 'API retornou sucesso=false sem mensagem.',
      );
    }
  } catch (error) {
    const detail =
      error.name === 'AbortError'
        ? `Timeout apos ${REQUEST_TIMEOUT_MS}ms — verifique VPN/rede para ${homBase}.`
        : error.message;
    pushCheck(checks, 'hom-api', 'Backend HML (API Acesso)', 'falha', detail);
  }
}

/**
 * Alternativa que NÃO consome a cota de OTP: valida apenas alcance do backend.
 * Usada no pré-setup, onde o smoke OTP é a fonte de verdade da autenticação.
 */
async function checkBackendSemOtp(checks, homBase) {
  try {
    const response = await fetchWithTimeout(`${homBase}/api/Usuario/Showme`, {
      method: 'GET',
      headers: { Authorization: 'Bearer token-diagnostico-demo' },
    });
    pushCheck(
      checks,
      'hom-api',
      'Backend HML (alcance)',
      'ok',
      `Backend respondeu HTTP ${response.status} (sem consumir OTP — smoke valida o login).`,
    );
  } catch (error) {
    const detail =
      error.name === 'AbortError'
        ? `Timeout apos ${REQUEST_TIMEOUT_MS}ms — verifique VPN/rede para ${homBase}.`
        : error.message;
    pushCheck(checks, 'hom-api', 'Backend HML (alcance)', 'falha', detail);
  }
}

async function runEnvChecks(options = {}) {
  const {
    envPath = envLocalPath,
    homUrl = DEFAULT_HOM_URL,
    demoEmail = DEMO_EMAIL,
    demoOrgId = DEMO_ORG_ID,
    vitePort = VITE_DEV_PORT,
    silent = false,
    otpProbe = true,
  } = options;

  const checks = [];
  const env = parseEnvFile(envPath);

  if (!fs.existsSync(envPath)) {
    pushCheck(checks, 'env-local', '.env.local', 'falha', `Arquivo ausente: ${envPath}`);
  } else {
    pushCheck(checks, 'env-local', '.env.local', 'ok', 'Arquivo encontrado em frontend/');
  }

  const proxyTarget = (env.VITE_API_PROXY_TARGET || '').trim();
  if (!proxyTarget) {
    pushCheck(
      checks,
      'env-proxy',
      'VITE_API_PROXY_TARGET',
      'falha',
      'Variavel ausente — proxy do Vite nao aponta para o backend hom.',
    );
  } else if (!/^https?:\/\//.test(proxyTarget)) {
    pushCheck(
      checks,
      'env-proxy',
      'VITE_API_PROXY_TARGET',
      'falha',
      `Valor invalido: ${proxyTarget}`,
    );
  } else {
    pushCheck(
      checks,
      'env-proxy',
      'VITE_API_PROXY_TARGET',
      'ok',
      proxyTarget,
    );
  }

  const apiBase = (env.VITE_API_FOURMAKERS_URL || '').trim();
  if (apiBase) {
    pushCheck(
      checks,
      'env-api-base',
      'VITE_API_FOURMAKERS_URL',
      'aviso',
      'Definido como URL absoluta — em dev local prefira vazio para usar proxy do Vite.',
    );
  } else {
    pushCheck(
      checks,
      'env-api-base',
      'VITE_API_FOURMAKERS_URL',
      'ok',
      'Vazio — front usara /api/* via proxy (recomendado).',
    );
  }

  const flutterflow = (env.VITE_FLUTTERFLOW_BASE_URL || '').trim();
  if (!flutterflow) {
    pushCheck(
      checks,
      'env-flutterflow',
      'VITE_FLUTTERFLOW_BASE_URL',
      'aviso',
      'Ausente — dashboard pode ficar em branco apos login (iframe undefined/homedashboard2).',
    );
  } else {
    pushCheck(checks, 'env-flutterflow', 'VITE_FLUTTERFLOW_BASE_URL', 'ok', flutterflow);
  }

  const missingFirebase = FIREBASE_VARS.filter((key) => !(env[key] || '').trim());
  if (missingFirebase.length) {
    pushCheck(
      checks,
      'env-firebase',
      'Variaveis Firebase',
      'aviso',
      `Ausentes: ${missingFirebase.join(', ')} (analytics/performance limitados).`,
    );
  } else {
    pushCheck(checks, 'env-firebase', 'Variaveis Firebase', 'ok', 'Todas configuradas.');
  }

  const homBase = (proxyTarget || homUrl).replace(/\/$/, '');

  if (!otpProbe) {
    await checkBackendSemOtp(checks, homBase);
  } else {
    await checkBackendComOtp(checks, homBase, demoEmail, demoOrgId);
  }

  const viteOpen = await isTcpPortOpen(vitePort);
  if (viteOpen) {
    try {
      const front = await fetchWithTimeout(`http://localhost:${vitePort}/login`, {
        method: 'GET',
      });
      pushCheck(
        checks,
        'vite-dev',
        `Front Vite (porta ${vitePort})`,
        front.ok ? 'ok' : 'aviso',
        front.ok
          ? 'Servidor local respondendo em /login.'
          : `HTTP ${front.status} em /login.`,
      );
    } catch (error) {
      pushCheck(
        checks,
        'vite-dev',
        `Front Vite (porta ${vitePort})`,
        'aviso',
        `Porta aberta mas /login falhou: ${error.message}`,
      );
    }
  } else {
    pushCheck(
      checks,
      'vite-dev',
      `Front Vite (porta ${vitePort})`,
      'aviso',
      'Nao esta rodando — antes da demo execute: npm run dev (raiz fourmakers-v2-develop).',
    );
  }

  if (viteOpen && proxyTarget) {
    try {
      const proxy = await fetchWithTimeout(
        `http://localhost:${vitePort}/api/Usuario/Showme`,
        {
          method: 'GET',
          headers: { Authorization: 'Bearer token-diagnostico-demo' },
        },
        15_000,
      );
      pushCheck(
        checks,
        'vite-proxy',
        'Proxy Vite → backend (/api/*)',
        proxy.status === 401 ? 'ok' : 'aviso',
        proxy.status === 401
          ? 'Proxy ativo (401 esperado sem JWT valido).'
          : `Resposta inesperada: HTTP ${proxy.status}.`,
      );
    } catch (error) {
      pushCheck(
        checks,
        'vite-proxy',
        'Proxy Vite → backend (/api/*)',
        'falha',
        error.message,
      );
    }
  } else {
    pushCheck(
      checks,
      'vite-proxy',
      'Proxy Vite → backend (/api/*)',
      'aviso',
      'Nao testado — suba o Vite com npm run dev para validar o proxy.',
    );
  }

  const summary = {
    ok: checks.filter((c) => c.status === 'ok').length,
    aviso: checks.filter((c) => c.status === 'aviso').length,
    falha: checks.filter((c) => c.status === 'falha').length,
    prontoParaDemo:
      checks.filter((c) => c.status === 'falha').length === 0 &&
      checks.some((c) => c.id === 'hom-api' && c.status === 'ok'),
  };

  if (!silent) {
    console.log('\n🔍 VERIFICAÇÕES DE AMBIENTE — DEMO REEMBOLSO\n');
    printChecks(checks);
    console.log(
      `\nResumo: ${summary.ok} ok | ${summary.aviso} aviso(s) | ${summary.falha} falha(s)`,
    );
    if (!summary.prontoParaDemo) {
      if (summary.falha > 0) {
        console.log(
          '\n⚠️  Corrija as falhas (rede, .env.local, usuario demo) antes da apresentacao.',
        );
      } else {
        console.log(
          '\nℹ️  Ambiente quase pronto — revise avisos (Vite parado, Firebase, etc.) antes da demo ao vivo.',
        );
      }
    } else if (summary.aviso > 0) {
      console.log('\nℹ️  Avisos nao bloqueiam o pre-setup; revise antes da demo ao vivo.');
    } else {
      console.log('\n✅ Ambiente pronto para a demonstracao.');
    }
    console.log('');
  }

  return { checks, summary };
}

if (require.main === module) {
  runEnvChecks()
    .then(({ summary }) => process.exit(summary.falha > 0 ? 1 : 0))
    .catch((error) => {
      console.error(`[demo-env-check] ${error.message}`);
      process.exit(1);
    });
}

module.exports = {
  runEnvChecks,
  parseEnvFile,
  DEFAULT_HOM_URL,
  DEMO_EMAIL,
  DEMO_ORG_ID,
  VITE_DEV_PORT,
  envLocalPath,
};
