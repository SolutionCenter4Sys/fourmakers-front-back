/**
 * Fonte única de credenciais da Demo Reembolso (HML fixo).
 * Arquivo: DEMO/setup/demo-credenciais.json (gitignored — tem OTP_SYSTEM_TOKEN)
 *
 * Ambiente sempre normalizado para homolog via demo-ambiente.cjs — ignora "dev" legado.
 */
const fs = require('node:fs');
const path = require('node:path');
const { DEMO_AMBIENTE, forceAmbienteHml, describeAmbiente } = require('./demo-ambiente.cjs');

const demoRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(demoRoot, '..');
const frontendRoot = path.join(repoRoot, 'frontend');
const playwrightRoot = path.join(frontendRoot, 'playwright-automation-template');
const credenciaisPath = path.join(demoRoot, 'setup', 'demo-credenciais.json');
const examplePath = path.join(demoRoot, 'setup', 'demo-credenciais.example.json');

function loadCredenciais() {
  if (!fs.existsSync(credenciaisPath)) {
    throw new Error(
      `Credenciais ausentes: ${credenciaisPath}\n` +
        `Copie ${path.relative(repoRoot, examplePath)} → demo-credenciais.json e preencha o token.`,
    );
  }
  const raw = JSON.parse(fs.readFileSync(credenciaisPath, 'utf8'));
  const forced = forceAmbienteHml(raw);

  // Persistência: evita JSON local com "dev" voltar a confundir no próximo run.
  const precisaGravar =
    raw.ambiente !== forced.ambiente ||
    String(raw.backend || '').replace(/\/$/, '') !== forced.backend ||
    (raw.envLocal && raw.envLocal.VITE_API_PROXY_TARGET) !== forced.envLocal.VITE_API_PROXY_TARGET;

  if (precisaGravar) {
    fs.writeFileSync(credenciaisPath, `${JSON.stringify(forced, null, 2)}\n`, 'utf8');
  }

  return forced;
}

function replaceLiteral(content, pattern, replacement) {
  const next = content.replace(pattern, replacement);
  return { content: next, changed: next !== content };
}

/**
 * Grava OTP_* no .env do Playwright (gitignored) — nunca no source TypeScript.
 */
function ensurePlaywrightEnv(cred = loadCredenciais()) {
  const envPath = path.join(playwrightRoot, '.env');
  const backend = String(cred.backend).replace(/\/$/, '');
  const lines = [
    '# Gerado por npm run demo:preparar — NÃO commit',
    `# Ambiente: ${cred.ambiente || 'homolog'} (HML) | ${cred.email} | org ${cred.orgId}`,
    `PLAYWRIGHT_BASE_URL=http://localhost:8080`,
    `PLAYWRIGHT_SLOWMO=150`,
    `E2E_REUSE_SESSION=true`,
    `E2E_REUSE_SESSION_MAX_AGE_MIN=30`,
    `OTP_API_BASE_URL=${backend}`,
    `OTP_EMAIL=${cred.email}`,
    `OTP_ORG_ID=${cred.orgId}`,
    `OTP_SYSTEM_TOKEN=${cred.systemTokenBase64 || ''}`,
  ];
  fs.writeFileSync(envPath, `${lines.join('\n')}\n`, 'utf8');
  return path.relative(repoRoot, envPath).replace(/\\/g, '/');
}

/**
 * Sincroniza e-mail/orgId/backend nos arquivos de auth da demo.
 * OTP_SYSTEM_TOKEN → só .env (gitignored) + demo-credenciais.json.
 */
function syncAuthFiles(cred = loadCredenciais()) {
  const email = cred.email;
  const orgId = Number(cred.orgId);
  const backend = String(cred.backend).replace(/\/$/, '');
  const updated = [];

  const targets = [
    {
      file: path.join(playwrightRoot, 'e2e', 'support', 'auth', 'solution-center-otp.ts'),
      patches: [
        {
          pattern: /appUrl:\s*'[^']*'/,
          replacement: `appUrl: '${backend}'`,
        },
        {
          pattern: /email:\s*'[^']*'/,
          replacement: `email: '${email}'`,
        },
        {
          pattern: /orgId:\s*\d+/,
          replacement: `orgId: ${orgId}`,
        },
      ],
    },
    {
      file: path.join(playwrightRoot, 'support', 'auth', 'fourmakers-auth.ts'),
      patches: [
        {
          pattern: /apiBase:\s*'[^']*'/,
          replacement: `apiBase: '${backend}'`,
        },
        {
          pattern: /email:\s*'[^']*'/,
          replacement: `email: '${email}'`,
        },
        {
          pattern: /orgId:\s*\d+/,
          replacement: `orgId: ${orgId}`,
        },
      ],
    },
    {
      file: path.join(playwrightRoot, 'e2e', 'debug', 'solution-center-smoke.spec.ts'),
      patches: [
        {
          pattern: /expect\(resultado\.usuario\?\.email\)\.toBe\('[^']*'\)/,
          replacement: `expect(resultado.usuario?.email).toBe('${email}')`,
        },
        {
          pattern: /expect\(payload\.Email\)\.toBe\('[^']*'\)/,
          replacement: `expect(payload.Email).toBe('${email}')`,
        },
        {
          pattern: /expect\(payload\.OrgId\)\.toBe\('[^']*'\)/,
          replacement: `expect(payload.OrgId).toBe('${orgId}')`,
        },
        {
          pattern: /https:\/\/[^/]+(?:\/[^'"]*)?\/api\/Acesso\/EnviaTokenAcessoEmail/,
          replacement: `${backend}/api/Acesso/EnviaTokenAcessoEmail`,
        },
        {
          pattern: /email:\s*'[^']*',\s*orgId:\s*\d+/,
          replacement: `email: '${email}', orgId: ${orgId}`,
        },
      ],
    },
    {
      file: path.join(frontendRoot, 'cypress', 'support', 'commands', 'fourmakers-auth.js'),
      patches: [
        {
          pattern: /apiBase:\s*'[^']*'/,
          replacement: `apiBase: '${backend}'`,
        },
        {
          pattern: /email:\s*'[^']*'/,
          replacement: `email: '${email}'`,
        },
        {
          pattern: /orgId:\s*\d+/,
          replacement: `orgId: ${orgId}`,
        },
      ],
    },
  ];

  for (const target of targets) {
    if (!fs.existsSync(target.file)) continue;
    let content = fs.readFileSync(target.file, 'utf8');
    let fileChanged = false;
    for (const patch of target.patches) {
      const result = replaceLiteral(content, patch.pattern, patch.replacement);
      content = result.content;
      fileChanged = fileChanged || result.changed;
    }
    if (fileChanged) {
      fs.writeFileSync(target.file, content, 'utf8');
      updated.push(path.relative(repoRoot, target.file).replace(/\\/g, '/'));
    }
  }

  updated.push(ensurePlaywrightEnv(cred));

  const manifestPath = path.join(demoRoot, 'modulos', 'reembolso.manifest.json');
  if (fs.existsSync(manifestPath)) {
    const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf8'));
    manifest.ambiente = {
      ...(manifest.ambiente || {}),
      nome: cred.ambiente || 'homolog',
      backend,
      frontLocal: cred.frontLocal,
      usuarioDemo: email,
      orgId: String(orgId),
      orgNome: cred.orgNome,
    };
    fs.writeFileSync(manifestPath, `${JSON.stringify(manifest, null, 2)}\n`, 'utf8');
    updated.push('DEMO/modulos/reembolso.manifest.json');
  }

  return { cred, updated };
}

/**
 * Garante frontend/.env.local com as chaves mínimas da demo (cria ou completa).
 */
function ensureEnvLocal(cred = loadCredenciais()) {
  const envPath = path.join(frontendRoot, '.env.local');
  const desired = cred.envLocal || {};
  const created = !fs.existsSync(envPath);

  let existing = {};
  if (!created) {
    for (const line of fs.readFileSync(envPath, 'utf8').split(/\r?\n/)) {
      const trimmed = line.trim();
      if (!trimmed || trimmed.startsWith('#')) continue;
      const eq = trimmed.indexOf('=');
      if (eq === -1) continue;
      existing[trimmed.slice(0, eq).trim()] = trimmed.slice(eq + 1).trim();
    }
  }

  const merged = { ...desired, ...existing };
  for (const [key, value] of Object.entries(desired)) {
    if (!(key in existing) || String(existing[key] || '').trim() === '') {
      merged[key] = value;
    }
  }
  merged.VITE_API_PROXY_TARGET = desired.VITE_API_PROXY_TARGET || cred.backend;
  if (!('VITE_API_FOURMAKERS_URL' in existing)) {
    merged.VITE_API_FOURMAKERS_URL = desired.VITE_API_FOURMAKERS_URL ?? '';
  }

  const lines = [
    '# Gerado/atualizado por npm run demo:preparar (DEMO/setup/demo-credenciais.json)',
    `# Ambiente: ${cred.ambiente || 'homolog'} (HML) | usuario: ${cred.email} | orgId: ${cred.orgId} (${cred.orgNome})`,
    `VITE_API_PROXY_TARGET=${merged.VITE_API_PROXY_TARGET}`,
    `VITE_API_FOURMAKERS_URL=${merged.VITE_API_FOURMAKERS_URL ?? ''}`,
    `VITE_FLUTTERFLOW_BASE_URL=${merged.VITE_FLUTTERFLOW_BASE_URL ?? ''}`,
  ];

  for (const [key, value] of Object.entries(merged)) {
    if (key.startsWith('VITE_API_') || key === 'VITE_FLUTTERFLOW_BASE_URL') continue;
    lines.push(`${key}=${value}`);
  }

  fs.writeFileSync(envPath, `${lines.join('\n')}\n`, 'utf8');
  return {
    path: envPath,
    created,
    patched: !created,
    proxy: merged.VITE_API_PROXY_TARGET,
    flutterflow: merged.VITE_FLUTTERFLOW_BASE_URL || '',
  };
}

module.exports = {
  credenciaisPath,
  loadCredenciais,
  syncAuthFiles,
  ensureEnvLocal,
  ensurePlaywrightEnv,
  DEMO_AMBIENTE,
  forceAmbienteHml,
  describeAmbiente,
  demoRoot,
  repoRoot,
  frontendRoot,
};
