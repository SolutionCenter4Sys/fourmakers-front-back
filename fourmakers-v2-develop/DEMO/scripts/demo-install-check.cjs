/**
 * Validações de instalação para máquinas da equipe (pré-setup Demo Reembolso).
 * CLI: node DEMO/scripts/demo-install-check.cjs
 *
 * Uso pelo demo-preparar.cjs (antes e depois do npm install).
 */
const fs = require('node:fs');
const os = require('node:os');
const net = require('node:net');
const path = require('node:path');
const { spawnSync } = require('node:child_process');

const demoRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(demoRoot, '..');
const frontendRoot = path.join(repoRoot, 'frontend');
const playwrightRoot = path.join(frontendRoot, 'playwright-automation-template');
const VITE_PORT = 8080;
const MIN_NODE_MAJOR = 18;
const MIN_DISK_MB_WARN = 1024;
const MIN_DISK_MB_FAIL = 500;

function log(step, message) {
  console.log(`\n[${step}] ${message}`);
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

async function isViteResponding(port = VITE_PORT) {
  if (!(await isTcpPortOpen(port))) return false;
  try {
    const response = await fetch(`http://localhost:${port}/login`, {
      signal: AbortSignal.timeout(3000),
    });
    return response.status > 0;
  } catch {
    return false;
  }
}

function bytesToMb(bytes) {
  return Math.round(bytes / (1024 * 1024));
}

function getFreeDiskMb(targetPath) {
  try {
    if (typeof fs.statfsSync === 'function') {
      const st = fs.statfsSync(targetPath);
      return bytesToMb(st.bavail * st.bsize);
    }
  } catch {
    /* fallback abaixo */
  }

  if (process.platform === 'win32') {
    const drive = path.parse(path.resolve(targetPath)).root.replace(/\\$/, '');
    const ps = spawnSync(
      'powershell',
      [
        '-NoProfile',
        '-Command',
        `(Get-PSDrive -Name '${drive.replace(':', '')}').Free`,
      ],
      { encoding: 'utf8' },
    );
    if (ps.status === 0) {
      const free = Number(String(ps.stdout).trim());
      if (Number.isFinite(free)) return bytesToMb(free);
    }
  }
  return null;
}

function checkNode() {
  const version = process.version;
  const major = Number(version.replace('v', '').split('.')[0]);
  if (major < MIN_NODE_MAJOR) {
    return {
      id: 'node',
      status: 'falha',
      detail: `Node.js >= ${MIN_NODE_MAJOR} necessario. Atual: ${version}`,
    };
  }
  return { id: 'node', status: 'ok', detail: `Node.js ${version}` };
}

function checkNpm() {
  const result = spawnSync('npm', ['-v'], { encoding: 'utf8', shell: true });
  if (result.status !== 0) {
    return {
      id: 'npm',
      status: 'falha',
      detail: 'npm nao encontrado. Instale Node.js LTS com npm.',
    };
  }
  return { id: 'npm', status: 'ok', detail: `npm ${result.stdout.trim()}` };
}

function checkGit() {
  const result = spawnSync('git', ['--version'], { encoding: 'utf8', shell: true });
  if (result.status !== 0) {
    return {
      id: 'git',
      status: 'aviso',
      detail: 'git nao encontrado (opcional para clonar/atualizar o repo).',
    };
  }
  return {
    id: 'git',
    status: 'ok',
    detail: String(result.stdout || result.stderr || 'git').trim(),
  };
}

function checkOs() {
  return {
    id: 'os',
    status: 'ok',
    detail: `${os.platform()} ${os.release()} · ${os.arch()} · ${os.cpus()?.[0]?.model || 'cpu'}`,
  };
}

function checkPackageFiles() {
  const required = [
    path.join(repoRoot, 'package.json'),
    path.join(frontendRoot, 'package.json'),
    path.join(playwrightRoot, 'package.json'),
  ];
  const missing = required.filter((p) => !fs.existsSync(p));
  if (missing.length) {
    return {
      id: 'package-json',
      status: 'falha',
      detail: `Arquivos ausentes: ${missing.map((p) => path.relative(repoRoot, p)).join(', ')}`,
    };
  }
  return {
    id: 'package-json',
    status: 'ok',
    detail: 'package.json raiz + frontend + playwright-automation-template',
  };
}

function checkDisk() {
  const freeMb = getFreeDiskMb(repoRoot);
  if (freeMb == null) {
    return {
      id: 'disk',
      status: 'aviso',
      detail: 'Nao foi possivel medir espaco livre em disco.',
    };
  }
  if (freeMb < MIN_DISK_MB_FAIL) {
    return {
      id: 'disk',
      status: 'falha',
      detail: `Espaco livre ${freeMb} MB (< ${MIN_DISK_MB_FAIL} MB). Libere disco antes do install.`,
    };
  }
  if (freeMb < MIN_DISK_MB_WARN) {
    return {
      id: 'disk',
      status: 'aviso',
      detail: `Espaco livre ${freeMb} MB (< ${MIN_DISK_MB_WARN} MB recomendados).`,
    };
  }
  return { id: 'disk', status: 'ok', detail: `Espaco livre ~${freeMb} MB` };
}

async function checkPort8080() {
  const open = await isTcpPortOpen(VITE_PORT);
  if (!open) {
    return {
      id: 'port-8080',
      status: 'ok',
      detail: `Porta ${VITE_PORT} livre (Vite podera subir).`,
    };
  }
  const vite = await isViteResponding(VITE_PORT);
  if (vite) {
    return {
      id: 'port-8080',
      status: 'ok',
      detail: `Porta ${VITE_PORT} ja responde como Vite (/login).`,
    };
  }
  return {
    id: 'port-8080',
    status: 'aviso',
    detail: `Porta ${VITE_PORT} ocupada por outro processo. Pare o conflito ou use npm run demo:front:parar / mate o PID.`,
  };
}

function moduleExists(base, relativeParts) {
  return fs.existsSync(path.join(base, 'node_modules', ...relativeParts));
}

function checkNodeModulesInstalled() {
  const checks = [
    {
      label: 'frontend/node_modules/vite',
      ok: moduleExists(frontendRoot, ['vite']),
    },
    {
      label: 'playwright/@playwright/test',
      ok: moduleExists(playwrightRoot, ['@playwright', 'test']),
    },
  ];
  const missing = checks.filter((c) => !c.ok).map((c) => c.label);
  if (missing.length) {
    return {
      id: 'node-modules',
      status: 'falha',
      detail: `Deps ausentes: ${missing.join(', ')}. Rode npm install (frontend + playwright).`,
    };
  }
  return {
    id: 'node-modules',
    status: 'ok',
    detail: 'node_modules frontend (vite) + playwright (@playwright/test)',
  };
}

function checkChromiumInstalled() {
  // Preferencia: caminho real do executavel Chromium do Playwright
  try {
    const playwrightPkg = path.join(playwrightRoot, 'node_modules', 'playwright');
    if (fs.existsSync(playwrightPkg)) {
      const playwright = require(playwrightPkg);
      const exe = playwright.chromium.executablePath();
      if (exe && fs.existsSync(exe)) {
        return {
          id: 'chromium',
          status: 'ok',
          detail: `Chromium OK (${exe})`,
        };
      }
      return {
        id: 'chromium',
        status: 'falha',
        detail:
          'Playwright instalado, mas Chromium ausente. Rode: npm --prefix frontend/playwright-automation-template run install:browsers',
      };
    }
  } catch (error) {
    return {
      id: 'chromium',
      status: 'falha',
      detail: `Falha ao localizar Chromium: ${error.message}`,
    };
  }

  return {
    id: 'chromium',
    status: 'falha',
    detail:
      'Pacote playwright ausente. Rode npm install em frontend/playwright-automation-template e install:browsers.',
  };
}

function summarize(checks) {
  const summary = {
    ok: checks.filter((c) => c.status === 'ok').length,
    aviso: checks.filter((c) => c.status === 'aviso').length,
    falha: checks.filter((c) => c.status === 'falha').length,
  };
  summary.pronto = summary.falha === 0;
  return summary;
}

function printChecks(titulo, checks) {
  console.log(`\n🔍 ${titulo}\n`);
  for (const c of checks) {
    const icon = c.status === 'ok' ? '✅' : c.status === 'aviso' ? '⚠️' : '❌';
    console.log(`  ${icon} ${c.id}`);
    console.log(`     ${c.detail}`);
  }
  const s = summarize(checks);
  console.log(
    `\nResumo instalacao: ${s.ok} ok | ${s.aviso} aviso(s) | ${s.falha} falha(s)`,
  );
  return s;
}

/**
 * Checks ANTES do npm install (ferramentas + disco + estrutura).
 */
async function runPreInstallChecks({ verbose = true } = {}) {
  const checks = [
    checkOs(),
    checkNode(),
    checkNpm(),
    checkGit(),
    checkPackageFiles(),
    checkDisk(),
    await checkPort8080(),
  ];
  if (verbose) printChecks('VALIDACOES DE INSTALACAO (pre-install)', checks);
  return { checks, summary: summarize(checks) };
}

/**
 * Checks DEPOIS do npm install (node_modules + Chromium).
 */
function runPostInstallChecks({ verbose = true } = {}) {
  const checks = [checkNodeModulesInstalled(), checkChromiumInstalled()];
  if (verbose) printChecks('VALIDACOES POS-INSTALL (deps + Chromium)', checks);
  return { checks, summary: summarize(checks) };
}

async function runAllInstallChecks({ verbose = true } = {}) {
  const pre = await runPreInstallChecks({ verbose });
  const post = {
    checks: [],
    summary: { ok: 0, aviso: 0, falha: 0, pronto: true },
  };
  // so valida node_modules se ja existirem (CLI standalone)
  if (
    fs.existsSync(path.join(frontendRoot, 'node_modules')) ||
    fs.existsSync(path.join(playwrightRoot, 'node_modules'))
  ) {
    Object.assign(post, runPostInstallChecks({ verbose }));
  }
  const checks = [...pre.checks, ...post.checks];
  return { checks, summary: summarize(checks), pre, post };
}

function assertNoFailures(result, fase) {
  if (result.summary.falha > 0) {
    const falhas = result.checks
      .filter((c) => c.status === 'falha')
      .map((c) => `${c.id}: ${c.detail}`)
      .join('\n  - ');
    throw new Error(
      `Pre-setup bloqueado (${fase}) — ${result.summary.falha} falha(s):\n  - ${falhas}`,
    );
  }
}

module.exports = {
  runPreInstallChecks,
  runPostInstallChecks,
  runAllInstallChecks,
  assertNoFailures,
  paths: { repoRoot, frontendRoot, playwrightRoot },
};

if (require.main === module) {
  runAllInstallChecks()
    .then((result) => {
      process.exit(result.summary.falha > 0 ? 1 : 0);
    })
    .catch((error) => {
      console.error(`\n[demo-install-check] ${error.message}\n`);
      process.exit(1);
    });
}
