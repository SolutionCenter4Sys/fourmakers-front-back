/**
 * Pré-setup da Demo Reembolso — executar ANTES da apresentação ao cliente.
 * Ativado no Cursor com: demo preparar reembolso
 *
 * Inclui:
 *  - Validações de instalação (Node/npm/git/disco/porta/estrutura) — máquina a máquina
 *  - Credenciais oficiais (DEMO/setup/demo-credenciais.json) → sync auth
 *  - frontend/.env.local (proxy + Flutterflow)
 *  - npm install (raiz + frontend + Playwright) + Chromium
 *  - Validação pós-install (node_modules + Chromium)
 *  - Limpeza de artefatos versionados
 *  - Subida do front Vite (:8080) em background
 *  - Verificação de ambiente (backend, Vite/proxy)
 *  - Smoke OTP com revalidação automática quando houver rate limit
 */
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const demoBug = require('./demo-reembolso-bug.cjs');
const { runEnvChecks } = require('./demo-env-check.cjs');
const demoVite = require('./demo-vite.cjs');
const {
  runPreInstallChecks,
  runPostInstallChecks,
  assertNoFailures,
} = require('./demo-install-check.cjs');
const {
  loadCredenciais,
  syncAuthFiles,
  ensureEnvLocal,
} = require('./demo-credenciais.cjs');

const { repoRoot, frontendRoot } = demoBug.paths;
const playwrightRoot = path.join(frontendRoot, 'playwright-automation-template');

const args = process.argv.slice(2);
const SEM_VITE = args.includes('--sem-vite');
const SEM_SMOKE = args.includes('--sem-smoke');
const VITE_TIMEOUT_MS = Number(process.env.DEMO_VITE_TIMEOUT_MS || 120_000);
const SMOKE_TENTATIVAS = Math.max(1, Number(process.env.DEMO_SMOKE_TENTATIVAS || 3));
const SMOKE_ESPERA_MS = Math.max(5_000, Number(process.env.DEMO_SMOKE_ESPERA_MS || 90_000));

function log(step, message) {
  console.log(`\n[${step}] ${message}`);
}

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function run(cmd, args, cwd, label) {
  log('EXEC', `${label}: ${cmd} ${args.join(' ')}`);
  const result = spawnSync(cmd, args, {
    cwd,
    stdio: 'inherit',
    shell: process.platform === 'win32',
    env: process.env,
  });
  if (result.status !== 0) {
    throw new Error(`Falha em "${label}" (exit ${result.status ?? 1})`);
  }
}

function installDependencies() {
  run('npm', ['install'], repoRoot, 'Dependencias na raiz');
  run('npm', ['install'], frontendRoot, 'Dependencias frontend (Vite/React)');
  run('npm', ['install'], playwrightRoot, 'Dependencias Playwright');
  run(
    'npm',
    ['exec', 'playwright', 'install', 'chromium'],
    playwrightRoot,
    'Browsers Playwright (Chromium)',
  );
}

function runSmokeAttempt() {
  const result = spawnSync('npm', ['run', 'test:solution-center:smoke'], {
    cwd: playwrightRoot,
    encoding: 'utf8',
    shell: process.platform === 'win32',
    env: process.env,
  });
  const output = `${result.stdout || ''}${result.stderr || ''}`;
  if (output.trim()) console.log(output);
  return {
    ok: result.status === 0,
    rateLimit: /limite de tentativas|too many requests|rate limit/i.test(output),
  };
}

/**
 * Roda o smoke OTP e, em caso de rate limit, aguarda e revalida sozinho —
 * o pré-setup só termina com um veredito final da autenticação.
 */
async function runSmokeOtpComRevalidacao() {
  for (let tentativa = 1; tentativa <= SMOKE_TENTATIVAS; tentativa += 1) {
    log(
      'INFO',
      `Validando autenticacao OTP (smoke Solution Center) — tentativa ${tentativa}/${SMOKE_TENTATIVAS}...`,
    );
    const { ok, rateLimit } = runSmokeAttempt();

    if (ok) {
      log('OK', `Smoke OTP passou (tentativa ${tentativa})`);
      return { status: 'ok', tentativas: tentativa };
    }

    if (!rateLimit) {
      log('AVISO', 'Smoke OTP falhou por motivo diferente de rate limit — veja o log acima.');
      return { status: 'falhou', tentativas: tentativa, motivo: 'erro' };
    }

    if (tentativa === SMOKE_TENTATIVAS) {
      log(
        'AVISO',
        `Rate limit OTP persistiu em ${SMOKE_TENTATIVAS} tentativas. Revalide antes da demo: npm --prefix frontend/playwright-automation-template run test:solution-center:smoke`,
      );
      return { status: 'falhou', tentativas: tentativa, motivo: 'rate-limit' };
    }

    log(
      'INFO',
      `Rate limit OTP detectado — aguardando ${Math.round(SMOKE_ESPERA_MS / 1000)}s e revalidando...`,
    );
    await sleep(SMOKE_ESPERA_MS);
  }

  return { status: 'falhou', tentativas: SMOKE_TENTATIVAS, motivo: 'rate-limit' };
}

async function subirFront() {
  if (SEM_VITE) {
    log('INFO', 'Front Vite ignorado (--sem-vite).');
    return { status: 'ignorado', detail: 'Flag --sem-vite.' };
  }

  log('INFO', 'Subindo front Vite (:8080) em background...');
  const result = await demoVite.ensureViteUp({ timeoutMs: VITE_TIMEOUT_MS });
  const nivel = result.status === 'iniciado' || result.status === 'ja-rodando' ? 'OK' : 'AVISO';
  log(nivel, `Vite: ${result.detail}`);
  if (nivel === 'OK') {
    log('INFO', `Logs do Vite: ${path.relative(repoRoot, result.log)} · parar: node DEMO/scripts/demo-vite.cjs stop`);
  }
  return result;
}

function printBanner(prepared, cred) {
  const linha = '═'.repeat(58);
  const env = prepared.verificacaoAmbiente || {};
  const resumoEnv = env.summary
    ? `${env.summary.ok} ok / ${env.summary.aviso} aviso / ${env.summary.falha} falha`
    : 'nao executado';
  const smokeDetalhe = prepared.smokeOtpDetalhe;
  const smoke = `${prepared.smokeOtp || 'nao executado'}${
    smokeDetalhe?.tentativas > 1 ? ` (${smokeDetalhe.tentativas} tentativas)` : ''
  }`;
  const front = prepared.front || {};
  const frontLabel =
    front.status === 'iniciado'
      ? `UP :${front.port} (pid ${front.pid})`
      : front.status === 'ja-rodando'
        ? `UP :${front.port} (ja rodava)`
        : front.status === 'ignorado'
          ? 'ignorado (--sem-vite)'
          : `${front.status || 'nao iniciado'} — suba com npm run dev`;

  console.log(`\n╔${linha}╗`);
  console.log('║  ✅ PRÉ-SETUP DA DEMO CONCLUÍDO                         ║');
  console.log('╠' + linha + '╣');
  console.log(`║  Backend: ${(cred.backend || '').slice(0, 45)}`.padEnd(59) + '║');
  console.log(`║  Usuario: ${cred.email}`.padEnd(59) + '║');
  console.log(`║  orgId:   ${cred.orgId} (${cred.orgNome || ''})`.padEnd(59) + '║');
  console.log(`║  Smoke OTP: ${smoke}`.padEnd(59) + '║');
  console.log(`║  Front:   ${frontLabel}`.padEnd(59) + '║');
  const inst = prepared.verificacaoInstalacao || {};
  const resumoInst = inst.summary
    ? `${inst.summary.ok} ok / ${inst.summary.aviso} aviso / ${inst.summary.falha} falha`
    : 'nao executado';

  console.log('╠' + linha + '╣');
  console.log('║  Credenciais sync + .env.local + deps + Chromium         ║');
  console.log('║  Install: frontend + playwright + Chromium validados     ║');
  console.log('║  Artefatos versionados antigos removidos                 ║');
  console.log('║  Self-healing pronto (bug manual pelo apresentador)      ║');
  console.log('╠' + linha + '╣');
  console.log(`║  Verificacoes instalacao: ${resumoInst}`.padEnd(59) + '║');
  console.log(`║  Verificacoes ambiente: ${resumoEnv}`.padEnd(59) + '║');
  if (env.summary && !env.summary.prontoParaDemo) {
    console.log('║  ⚠️  Revise falhas acima antes da demo ao vivo           ║');
  }
  console.log('╠' + linha + '╣');
  console.log('║  Injete o bug manualmente antes da demo (fora da esteira) ║');
  console.log('║  Na demonstracao: demo reembolso (front ja no ar)        ║');
  console.log('║  Falha → Playwright + Dev FourBlox corrigem no chat      ║');
  console.log('╚' + linha + '╝\n');
}

async function main() {
  console.log('\n🛠️  DEMO REEMBOLSO — PRÉ-SETUP (executar antes do cliente)\n');

  const cred = loadCredenciais();
  log(
    'OK',
    `Credenciais: ${cred.email} | orgId ${cred.orgId} (${cred.orgNome}) | ${cred.ambiente}`,
  );

  log('INFO', 'Validando instalacao na maquina (Node/npm/disco/porta/estrutura)...');
  const preInstall = await runPreInstallChecks({ verbose: true });
  assertNoFailures(preInstall, 'pre-install');
  if (preInstall.summary.aviso > 0) {
    log('INFO', `${preInstall.summary.aviso} aviso(s) de instalacao (nao bloqueiam).`);
  }

  log('INFO', 'Sincronizando auth Playwright/Cypress/smoke com demo-credenciais.json...');
  const sync = syncAuthFiles(cred);
  if (sync.updated.length) {
    log('OK', `Arquivos sincronizados: ${sync.updated.join(', ')}`);
  } else {
    log('OK', 'Auth ja estava alinhado com demo-credenciais.json');
  }

  log('INFO', 'Garantindo frontend/.env.local (proxy + Flutterflow)...');
  const envInfo = ensureEnvLocal(cred);
  log(
    'OK',
    envInfo.created
      ? `.env.local criado (${envInfo.proxy})`
      : `.env.local atualizado (proxy=${envInfo.proxy}; flutterflow=${envInfo.flutterflow || 'vazio'})`,
  );

  log('INFO', 'Instalando dependencias (raiz + frontend + Playwright + Chromium)...');
  installDependencies();

  log('INFO', 'Validando deps instaladas (node_modules + Chromium)...');
  const postInstall = runPostInstallChecks({ verbose: true });
  assertNoFailures(postInstall, 'pos-install');

  const verificacaoInstalacao = {
    checks: [...preInstall.checks, ...postInstall.checks],
    summary: {
      ok: preInstall.summary.ok + postInstall.summary.ok,
      aviso: preInstall.summary.aviso + postInstall.summary.aviso,
      falha: preInstall.summary.falha + postInstall.summary.falha,
      pronto: preInstall.summary.pronto && postInstall.summary.pronto,
    },
  };

  log('INFO', 'Limpando versoes antigas da demo (BDD, DataForge, specs, relatorios)...');
  const removed = demoBug.cleanVersionedArtifacts();
  log('OK', removed.length ? `${removed.length} arquivo(s) removido(s)` : 'Nada para limpar');

  const vite = await subirFront();

  log('INFO', 'Verificando ambiente (.env.local, backend, Vite, proxy)...');
  const verificacaoAmbiente = await runEnvChecks({
    homUrl: cred.backend,
    demoEmail: cred.email,
    demoOrgId: cred.orgId,
    otpProbe: false,
  });
  if (verificacaoAmbiente.summary.falha > 0) {
    log(
      'AVISO',
      `${verificacaoAmbiente.summary.falha} falha(s) de ambiente — corrija antes da demo.`,
    );
  } else if (verificacaoAmbiente.summary.aviso > 0) {
    log('INFO', `${verificacaoAmbiente.summary.aviso} aviso(s) de ambiente (nao bloqueiam pre-setup).`);
  } else {
    log('OK', 'Verificacoes de ambiente passaram sem avisos.');
  }

  log('INFO', 'Self-healing: bug NAO e injetado pela demo (apresentador injeta manualmente).');

  const prepared = demoBug.writePreparedState({
    selfHealing: true,
    bugInjetadoPelaDemo: false,
    artefatosRemovidos: removed.length,
    nodeVersion: process.version,
    npmVersion: (() => {
      const r = spawnSync('npm', ['-v'], { encoding: 'utf8', shell: true });
      return r.status === 0 ? r.stdout.trim() : null;
    })(),
    platform: process.platform,
    arch: process.arch,
    verificacaoInstalacao,
    verificacaoAmbiente,
    front: {
      status: vite.status,
      port: vite.port,
      pid: vite.pid,
      log: vite.log,
      detail: vite.detail,
    },
    credenciais: {
      email: cred.email,
      orgId: cred.orgId,
      orgNome: cred.orgNome,
      backend: cred.backend,
      ambiente: cred.ambiente,
    },
    authSync: sync.updated,
    envLocal: {
      created: envInfo.created,
      proxy: envInfo.proxy,
      flutterflow: envInfo.flutterflow,
    },
  });

  if (SEM_SMOKE) {
    log('INFO', 'Smoke OTP ignorado (--sem-smoke).');
    prepared.smokeOtp = 'ignorado';
  } else {
    const smoke = await runSmokeOtpComRevalidacao();
    prepared.smokeOtp = smoke.status;
    prepared.smokeOtpDetalhe = smoke;
  }
  demoBug.writePreparedState(prepared);

  printBanner(prepared, cred);
}

main().catch((error) => {
  console.error(`\n[demo-preparar] ${error.message}\n`);
  process.exit(1);
});
