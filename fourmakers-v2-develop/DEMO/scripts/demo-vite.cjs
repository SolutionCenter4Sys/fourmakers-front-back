/**
 * Sobe / derruba / inspeciona o front Vite (porta 8080) usado pela Demo Reembolso.
 * CLI: node DEMO/scripts/demo-vite.cjs start | stop | status
 *
 * O processo é iniciado desacoplado (detached) para continuar vivo depois que o
 * pré-setup termina — o apresentador não precisa de um segundo terminal.
 */
const fs = require('node:fs');
const net = require('node:net');
const path = require('node:path');
const { spawn, spawnSync } = require('node:child_process');

const demoRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(demoRoot, '..');
const setupDir = path.join(demoRoot, 'setup');
const stateFile = path.join(setupDir, '.demo-vite.json');
const logFile = path.join(setupDir, 'vite-dev.log');

const DEFAULT_PORT = 8080;
const DEFAULT_TIMEOUT_MS = 120_000;
const POLL_INTERVAL_MS = 1_500;

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function isTcpPortOpen(port, host = 'localhost', timeoutMs = 1500) {
  return new Promise((resolve) => {
    const socket = new net.Socket();
    const done = (open) => {
      socket.destroy();
      resolve(open);
    };
    socket.setTimeout(timeoutMs);
    socket.once('connect', () => done(true));
    socket.once('timeout', () => done(false));
    socket.once('error', () => done(false));
    socket.connect(port, host);
  });
}

async function isViteResponding(port) {
  if (!(await isTcpPortOpen(port))) return false;
  try {
    const response = await fetch(`http://localhost:${port}/login`, {
      signal: AbortSignal.timeout(8_000),
    });
    return response.ok;
  } catch {
    return false;
  }
}

function readState() {
  if (!fs.existsSync(stateFile)) return null;
  try {
    return JSON.parse(fs.readFileSync(stateFile, 'utf8'));
  } catch {
    return null;
  }
}

function writeState(payload) {
  fs.mkdirSync(setupDir, { recursive: true });
  fs.writeFileSync(stateFile, `${JSON.stringify(payload, null, 2)}\n`, 'utf8');
  return payload;
}

function clearState() {
  if (fs.existsSync(stateFile)) fs.rmSync(stateFile);
}

function spawnVite(port) {
  fs.mkdirSync(setupDir, { recursive: true });
  const out = fs.openSync(logFile, 'a');
  fs.writeSync(
    out,
    `\n===== npm run dev (demo pre-setup) — ${new Date().toISOString()} =====\n`,
  );

  const child = spawn('npm', ['run', 'dev'], {
    cwd: repoRoot,
    detached: true,
    shell: process.platform === 'win32',
    stdio: ['ignore', out, out],
    env: { ...process.env, PORT: String(port) },
  });
  child.unref();
  return child;
}

/**
 * Garante o Vite respondendo em /login. Retorna:
 * { status: 'ja-rodando' | 'iniciado' | 'timeout' | 'erro', port, pid?, log, detail }
 */
async function ensureViteUp(options = {}) {
  const {
    port = DEFAULT_PORT,
    timeoutMs = DEFAULT_TIMEOUT_MS,
    onProgress = () => {},
  } = options;

  if (await isViteResponding(port)) {
    const previous = readState();
    return {
      status: 'ja-rodando',
      port,
      pid: previous?.pid,
      log: logFile,
      detail: `Front ja respondia em http://localhost:${port}/login.`,
    };
  }

  let child;
  try {
    child = spawnVite(port);
  } catch (error) {
    return { status: 'erro', port, log: logFile, detail: error.message };
  }

  const deadline = Date.now() + timeoutMs;
  while (Date.now() < deadline) {
    await sleep(POLL_INTERVAL_MS);
    if (await isViteResponding(port)) {
      const state = writeState({
        pid: child.pid,
        port,
        log: logFile,
        iniciadoEm: new Date().toISOString(),
        iniciadoPelaDemo: true,
      });
      return {
        status: 'iniciado',
        port,
        pid: state.pid,
        log: logFile,
        detail: `Vite subiu em http://localhost:${port} (pid ${state.pid}).`,
      };
    }
    onProgress(Math.round((timeoutMs - (deadline - Date.now())) / 1000));
  }

  return {
    status: 'timeout',
    port,
    pid: child.pid,
    log: logFile,
    detail: `Nao respondeu em ${Math.round(timeoutMs / 1000)}s — veja ${path.relative(repoRoot, logFile)}.`,
  };
}

function killTree(pid) {
  if (process.platform === 'win32') {
    const result = spawnSync('taskkill', ['/PID', String(pid), '/T', '/F'], {
      encoding: 'utf8',
    });
    return result.status === 0;
  }
  try {
    process.kill(-pid);
    return true;
  } catch {
    try {
      process.kill(pid);
      return true;
    } catch {
      return false;
    }
  }
}

function stopVite() {
  const state = readState();
  if (!state?.pid) {
    return { stopped: false, detail: 'Nenhum Vite registrado pela demo.' };
  }
  const stopped = killTree(state.pid);
  clearState();
  return {
    stopped,
    pid: state.pid,
    detail: stopped
      ? `Vite (pid ${state.pid}) encerrado.`
      : `Nao foi possivel encerrar pid ${state.pid} (talvez ja tenha saido).`,
  };
}

async function statusVite(port = DEFAULT_PORT) {
  const respondendo = await isViteResponding(port);
  return { port, respondendo, registro: readState(), log: logFile };
}

if (require.main === module) {
  const [command = 'status'] = process.argv.slice(2);

  (async () => {
    switch (command) {
      case 'start': {
        const result = await ensureViteUp();
        console.log(`[demo-vite] ${result.status}: ${result.detail}`);
        process.exit(result.status === 'timeout' || result.status === 'erro' ? 1 : 0);
        break;
      }
      case 'stop': {
        const result = stopVite();
        console.log(`[demo-vite] ${result.detail}`);
        process.exit(result.stopped ? 0 : 1);
        break;
      }
      case 'status': {
        const result = await statusVite();
        console.log(JSON.stringify(result, null, 2));
        process.exit(result.respondendo ? 0 : 1);
        break;
      }
      default:
        console.error(`Comando desconhecido: ${command}. Use: start | stop | status`);
        process.exit(1);
    }
  })().catch((error) => {
    console.error(`[demo-vite] ${error.message}`);
    process.exit(1);
  });
}

module.exports = {
  ensureViteUp,
  stopVite,
  statusVite,
  isViteResponding,
  DEFAULT_PORT,
  paths: { stateFile, logFile, repoRoot },
};
