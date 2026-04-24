const { spawnSync } = require('node:child_process');
const http = require('node:http');

const SERVER_URL = 'http://localhost:8080';

function isServerUp() {
  return new Promise((resolve) => {
    const req = http.get(SERVER_URL, { timeout: 2000 }, (res) => {
      res.resume();
      resolve(res.statusCode && res.statusCode < 500);
    });
    req.on('error', () => resolve(false));
    req.on('timeout', () => {
      req.destroy();
      resolve(false);
    });
  });
}

async function main() {
  const up = await isServerUp();

  if (up) {
    console.log(`[demo-headed-smart] ${SERVER_URL} já está respondendo. Pulando start do Vite...`);
    const run = spawnSync(
      'npm',
      ['--prefix', 'playwright-automation-template', 'run', 'demo:only:headed'],
      { stdio: 'inherit', env: process.env },
    );
    process.exit(run.status ?? 1);
  }

  console.log(`[demo-headed-smart] ${SERVER_URL} não respondeu. Iniciando start-server-and-test...`);
  const start = spawnSync(
    'npx',
    [
      'start-server-and-test',
      'dev',
      SERVER_URL,
      'npm --prefix playwright-automation-template run demo:only:headed',
    ],
    { stdio: 'inherit', env: process.env, shell: true },
  );
  process.exit(start.status ?? 1);
}

main().catch((err) => {
  console.error('[demo-headed-smart] Falha inesperada:', err);
  process.exit(1);
});
