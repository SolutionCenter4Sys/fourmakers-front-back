const fs = require('node:fs');
const path = require('node:path');
const { spawn } = require('node:child_process');

const args = process.argv.slice(2);
const isHeaded = args.includes('--headed');
const isDryRun = args.includes('--dry-run');

const templateRoot = process.cwd();
const frontendRoot = path.resolve(templateRoot, '..');

const bddDir = path.join(frontendRoot, 'DEMO', 'cenarios-bdd');
const dataDir = path.join(frontendRoot, 'DEMO', 'automacao', 'reembolso', 'versionadas');
const uiElementsFile = path.join(frontendRoot, 'DEMO', 'ui-elements', 'reembolso-ui.json');
const specDir = path.join(templateRoot, 'tests', 'reembolso', 'versionadas');
const relatorioDir = path.join(templateRoot, 'evidencias', 'relatorios');

function listVersions(dir, pattern) {
  if (!fs.existsSync(dir)) return [];
  return fs
    .readdirSync(dir, { withFileTypes: true })
    .filter((entry) => entry.isFile())
    .map((entry) => entry.name)
    .map((name) => {
      const match = pattern.exec(name);
      return match ? Number(match[1]) : null;
    })
    .filter((version) => Number.isInteger(version))
    .sort((a, b) => b - a);
}

function fail(message) {
  console.error(`\n[demo-versionada] ${message}`);
  process.exit(1);
}

function normalizePath(filePath) {
  return filePath.replace(/\\/g, '/');
}

function uniq(values) {
  return [...new Set(values)];
}

function extractBddScenarioIds(content) {
  const ids = [];
  const pattern = /^\|\s*([RI]-\d+)\s*\|/gm;
  let match = pattern.exec(content);
  while (match) {
    ids.push(match[1]);
    match = pattern.exec(content);
  }
  return uniq(ids);
}

function extractDataScenarioIds(content) {
  const ids = [];
  const pattern = /^export const dados([RI])(\d{2})\s*=/gm;
  let match = pattern.exec(content);
  while (match) {
    ids.push(`${match[1]}-${match[2]}`);
    match = pattern.exec(content);
  }
  return uniq(ids);
}

function extractSpecScenarioIds(content) {
  const ids = [];
  const pattern = /test(?:\.skip)?\(\s*['"`]([RI]-\d+)\s+\[[^\]]+\]/g;
  let match = pattern.exec(content);
  while (match) {
    ids.push(match[1]);
    match = pattern.exec(content);
  }
  return uniq(ids);
}

function extractCountsFromText(outputText) {
  const extract = (label) => {
    const matches = [...outputText.matchAll(new RegExp(`(\\d+)\\s+${label}`, 'gi'))];
    if (!matches.length) return 0;
    return Number(matches[matches.length - 1][1]);
  };

  return {
    passed: extract('passed'),
    failed: extract('failed'),
    skipped: extract('skipped'),
  };
}

function extractScenarioIdsFromOutput(outputText) {
  const ids = [];
  const testLinePattern = /🧪\s*TESTANDO:\s*([RI]-\d+)/g;
  let match = testLinePattern.exec(outputText);
  while (match) {
    ids.push(match[1]);
    match = testLinePattern.exec(outputText);
  }

  if (!ids.length) {
    const genericPattern = /\b([RI]-\d{2})\b/g;
    let genericMatch = genericPattern.exec(outputText);
    while (genericMatch) {
      ids.push(genericMatch[1]);
      genericMatch = genericPattern.exec(outputText);
    }
  }

  return uniq(ids);
}

function resolvePlaywrightCli() {
  const candidates = ['@playwright/test/cli', 'playwright/cli'];
  const tentativas = [];
  for (const id of candidates) {
    try {
      return require.resolve(id, { paths: [templateRoot] });
    } catch (error) {
      tentativas.push(`${id}: ${error.message}`);
    }
  }
  const fallback = path.join(templateRoot, 'node_modules', '@playwright', 'test', 'cli.js');
  if (fs.existsSync(fallback)) return fallback;

  fail(
    `Nao foi possivel localizar o CLI do Playwright. Rode "npm install" em ` +
      `"${templateRoot}" antes de executar a demo.\n` +
      `Tentativas:\n  - ${tentativas.join('\n  - ')}\n  - ${fallback} (nao existe)`,
  );
}

function runCommandStream(command, commandArgs, env) {
  return new Promise((resolve, reject) => {
    const child = spawn(command, commandArgs, {
      cwd: templateRoot,
      env,
      stdio: ['ignore', 'pipe', 'pipe'],
    });

    let stdout = '';
    let stderr = '';

    child.stdout.on('data', (chunk) => {
      const text = chunk.toString();
      stdout += text;
      process.stdout.write(text);
    });

    child.stderr.on('data', (chunk) => {
      const text = chunk.toString();
      stderr += text;
      process.stderr.write(text);
    });

    child.on('error', reject);
    child.on('close', (code) => {
      resolve({
        exitCode: code ?? 1,
        output: `${stdout}${stderr}`,
      });
    });
  });
}

function parsePlaywrightJsonReport(report) {
  const tests = [];

  function visitSuite(suite) {
    if (Array.isArray(suite.specs)) {
      for (const spec of suite.specs) {
        for (const test of spec.tests || []) {
          const results = Array.isArray(test.results) ? test.results : [];
          const finalResult = [...results].reverse().find((result) => result.status) || null;
          const statusFromOutcome = test.outcome === 'skipped'
            ? 'skipped'
            : test.outcome === 'unexpected'
              ? 'failed'
              : 'unknown';
          const status = finalResult?.status || statusFromOutcome;
          const errorMessage = finalResult?.error?.message || '';
          const duration = Number(finalResult?.duration || 0);
          tests.push({
            title: spec.title || test.title || 'Sem titulo',
            status,
            duration,
            error: errorMessage,
          });
        }
      }
    }

    for (const nested of suite.suites || []) visitSuite(nested);
  }

  for (const suite of report?.suites || []) visitSuite(suite);
  return tests;
}

function safeReadJson(filePath) {
  if (!fs.existsSync(filePath)) return null;
  try {
    return JSON.parse(fs.readFileSync(filePath, 'utf8'));
  } catch {
    return null;
  }
}

function stripAnsi(text) {
  return text.replace(/\u001b\[[0-9;]*m/g, '');
}

function escapeHtml(text) {
  return text
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#39;');
}

function renderStatusBadge(status) {
  if (status === 'passed') return '<span class="badge ok">passed</span>';
  if (status === 'failed' || status === 'timedOut') return '<span class="badge fail">failed</span>';
  if (status === 'skipped') return '<span class="badge skip">skipped</span>';
  return `<span class="badge">${escapeHtml(status)}</span>`;
}

function buildHtmlReport({
  version,
  bddRelative,
  dataRelative,
  specRelative,
  uiRelative,
  bddScenarioIds,
  dataScenarioIds,
  specScenarioIds,
  testedScenarioIds,
  uiJsonUsed,
  testRows,
  summary,
  logs,
}) {
  const linhasTeste = testRows.length
    ? testRows
        .map((row) => {
          return `<tr><td>${renderStatusBadge(row.status)}</td><td>${escapeHtml(row.title)}</td><td>${row.duration} ms</td></tr>`;
        })
        .join('\n')
    : '<tr><td colspan="3">Nenhum teste identificado no JSON do Playwright.</td></tr>';

  const falhas = testRows.filter((row) => row.error && (row.status === 'failed' || row.status === 'timedOut'));
  const blocoFalhas = falhas.length
    ? falhas
        .map((row) => {
          return `<details><summary>${escapeHtml(row.title)}</summary><pre>${escapeHtml(row.error)}</pre></details>`;
        })
        .join('\n')
    : '<p>Nenhuma falha registrada no relatório JSON.</p>';

  return `<!doctype html>
<html lang="pt-BR">
<head>
  <meta charset="utf-8" />
  <title>Demo Reembolso v${version} - Resultado Final</title>
  <style>
    body { font-family: Arial, sans-serif; margin: 24px; background: #0f172a; color: #e2e8f0; }
    h1, h2 { margin: 0 0 12px; }
    .card { background: #111827; border: 1px solid #334155; border-radius: 12px; padding: 16px; margin-bottom: 16px; }
    .muted { color: #94a3b8; }
    .grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 12px; }
    .metric { background: #0b1220; border: 1px solid #334155; border-radius: 10px; padding: 12px; }
    .metric strong { display: block; font-size: 28px; margin-bottom: 4px; }
    ul { margin: 0; padding-left: 18px; }
    table { width: 100%; border-collapse: collapse; }
    th, td { border: 1px solid #334155; padding: 8px; text-align: left; vertical-align: top; }
    th { background: #1f2937; }
    pre { white-space: pre-wrap; background: #020617; border: 1px solid #334155; border-radius: 8px; padding: 12px; }
    .badge { display: inline-block; border-radius: 10px; padding: 2px 8px; border: 1px solid #64748b; }
    .ok { border-color: #22c55e; color: #22c55e; }
    .fail { border-color: #ef4444; color: #ef4444; }
    .skip { border-color: #f59e0b; color: #f59e0b; }
  </style>
</head>
<body>
  <h1>Demo Reembolso v${version} - Resultado Final</h1>
  <p class="muted">Gerado automaticamente em ${new Date().toLocaleString('pt-BR')}</p>

  <div class="card">
    <h2>Artefatos utilizados</h2>
    <ul>
      <li><strong>BDD:</strong> ${escapeHtml(bddRelative)}</li>
      <li><strong>Massa DataForge:</strong> ${escapeHtml(dataRelative)}</li>
      <li><strong>Spec Playwright:</strong> ${escapeHtml(specRelative)}</li>
      <li><strong>JSON de campos da tela:</strong> ${escapeHtml(uiRelative)} (${uiJsonUsed ? 'usado' : 'nao evidenciado'})</li>
    </ul>
  </div>

  <div class="card">
    <h2>Resumo da execução</h2>
    <div class="grid">
      <div class="metric"><strong>${summary.passed}</strong><span>passed</span></div>
      <div class="metric"><strong>${summary.failed}</strong><span>failed</span></div>
      <div class="metric"><strong>${summary.skipped}</strong><span>skipped</span></div>
      <div class="metric"><strong>${summary.total}</strong><span>total</span></div>
    </div>
  </div>

  <div class="card">
    <h2>Cobertura declarada</h2>
    <ul>
      <li><strong>Cenários do BDD (${bddScenarioIds.length}):</strong> ${escapeHtml(bddScenarioIds.join(', '))}</li>
      <li><strong>Cenários da massa (${dataScenarioIds.length}):</strong> ${escapeHtml(dataScenarioIds.join(', '))}</li>
      <li><strong>Cenários mapeados na spec (${specScenarioIds.length}):</strong> ${escapeHtml(specScenarioIds.join(', '))}</li>
      <li><strong>Cenários testados na execução (${testedScenarioIds.length}):</strong> ${escapeHtml(testedScenarioIds.join(', '))}</li>
    </ul>
  </div>

  <div class="card">
    <h2>Resultados por cenário</h2>
    <table>
      <thead>
        <tr><th>Status</th><th>Cenário</th><th>Duração</th></tr>
      </thead>
      <tbody>
        ${linhasTeste}
      </tbody>
    </table>
  </div>

  <div class="card">
    <h2>Falhas e erros (quando houver)</h2>
    ${blocoFalhas}
  </div>

  <div class="card">
    <h2>Logs completos da execução</h2>
    <pre>${escapeHtml(logs)}</pre>
  </div>
</body>
</html>`;
}

async function main() {
  const bddVersions = listVersions(bddDir, /^REEMBOLSO-BDD-v(\d+)\.md$/);
  const dataVersions = listVersions(dataDir, /^reembolso\.data\.v(\d+)\.js$/);
  const specVersions = listVersions(specDir, /^reembolso-demo-v(\d+)\.spec\.ts$/);

  if (!bddVersions.length) fail(`Nenhum BDD versionado encontrado em "${bddDir}".`);
  if (!dataVersions.length) fail(`Nenhuma massa DataForge versionada encontrada em "${dataDir}".`);
  if (!specVersions.length) fail(`Nenhuma spec versionada encontrada em "${specDir}".`);
  if (!fs.existsSync(uiElementsFile)) {
    fail(`Arquivo JSON da tela nao encontrado em "${uiElementsFile}".`);
  }

  const commonVersions = bddVersions.filter(
    (version) => dataVersions.includes(version) && specVersions.includes(version),
  );

  if (!commonVersions.length) {
    fail(
      `Nao existe versao comum entre BDD, DataForge e spec. ` +
        `BDD=${bddVersions.join(', ')} | DATA=${dataVersions.join(', ')} | SPEC=${specVersions.join(', ')}`,
    );
  }

  const version = commonVersions[0];
  const bddFile = path.join(bddDir, `REEMBOLSO-BDD-v${version}.md`);
  const dataFile = path.join(dataDir, `reembolso.data.v${version}.js`);
  const specFile = path.join(specDir, `reembolso-demo-v${version}.spec.ts`);
  const specRelative = normalizePath(path.relative(templateRoot, specFile));
  const bddRelative = normalizePath(path.relative(frontendRoot, bddFile));
  const dataRelative = normalizePath(path.relative(frontendRoot, dataFile));
  const uiRelative = normalizePath(path.relative(frontendRoot, uiElementsFile));

  const bddContent = fs.readFileSync(bddFile, 'utf8');
  const dataContent = fs.readFileSync(dataFile, 'utf8');
  const specContent = fs.readFileSync(specFile, 'utf8');

  if (!specContent.includes(`REEMBOLSO-BDD-v${version}.md`)) {
    fail(
      `A spec v${version} nao referencia explicitamente o BDD v${version}. ` +
        `Arquivo: ${specRelative}`,
    );
  }
  if (!specContent.includes(`reembolso.data.v${version}.js`)) {
    fail(
      `A spec v${version} nao importa explicitamente a massa DataForge v${version}. ` +
        `Arquivo: ${specRelative}`,
    );
  }

  const bddScenarioIds = extractBddScenarioIds(bddContent);
  const dataScenarioIds = extractDataScenarioIds(dataContent);
  const specScenarioIds = extractSpecScenarioIds(specContent);
  const uiJsonUsed = specContent.includes('ReembolsoPage');

  console.log(`[demo-versionada] Versao selecionada: v${version}`);
  console.log(`[demo-versionada] BDD:  ${bddRelative}`);
  console.log(`[demo-versionada] DATA: ${dataRelative}`);
  console.log(`[demo-versionada] SPEC: ${specRelative}`);
  console.log(`[demo-versionada] UI-JSON: ${uiRelative}`);

  if (isDryRun) {
    console.log('[demo-versionada] Dry-run concluido, sem executar Playwright.');
    return;
  }

  fs.mkdirSync(relatorioDir, { recursive: true });

  const jsonReportFile = path.join(relatorioDir, `demo-reembolso-v${version}.json`);
  const htmlSummaryFile = path.join(relatorioDir, `demo-reembolso-v${version}.html`);
  const playwrightCli = resolvePlaywrightCli();

  const playwrightArgs = [playwrightCli, 'test', specRelative, '--reporter=list,html,json'];
  if (isHeaded) playwrightArgs.push('--headed');

  console.log(`[demo-versionada] Executando: node ${playwrightArgs.join(' ')}`);

  const execution = await runCommandStream(process.execPath, playwrightArgs, {
    ...process.env,
    PLAYWRIGHT_JSON_OUTPUT_NAME: jsonReportFile,
  });

  const playReport = safeReadJson(jsonReportFile);
  const parsedTests = playReport ? parsePlaywrightJsonReport(playReport) : [];
  const fallbackCounts = extractCountsFromText(execution.output);

  const summary = {
    passed: parsedTests.filter((test) => test.status === 'passed').length || fallbackCounts.passed,
    failed:
      parsedTests.filter((test) => test.status === 'failed' || test.status === 'timedOut').length ||
      fallbackCounts.failed,
    skipped: parsedTests.filter((test) => test.status === 'skipped').length || fallbackCounts.skipped,
    total: parsedTests.length || (fallbackCounts.passed + fallbackCounts.failed + fallbackCounts.skipped),
  };

  const testedFromJson = uniq(
    parsedTests
      .map((test) => {
        const match = test.title.match(/([RI]-\d+)/);
        return match ? match[1] : null;
      })
      .filter(Boolean),
  );
  const testedScenarioIds = testedFromJson.length
    ? testedFromJson
    : extractScenarioIdsFromOutput(execution.output);

  const sanitizedLogs = stripAnsi(execution.output);

  const htmlContent = buildHtmlReport({
    version,
    bddRelative,
    dataRelative,
    specRelative,
    uiRelative,
    bddScenarioIds,
    dataScenarioIds,
    specScenarioIds,
    testedScenarioIds,
    uiJsonUsed,
    testRows: parsedTests,
    summary,
    logs: sanitizedLogs,
  });

  fs.writeFileSync(htmlSummaryFile, htmlContent, 'utf8');

  console.log(
    `[demo-versionada] DataForge cobriu ${dataScenarioIds.length} cenarios: ${dataScenarioIds.join(', ')}`,
  );
  console.log(
    `[demo-versionada] Playwright testou ${testedScenarioIds.length} cenarios: ${testedScenarioIds.join(', ') || 'nenhum identificado'}`,
  );
  console.log(
    `[demo-versionada] JSON de campos da tela usado: ${uiJsonUsed ? 'SIM' : 'NAO (apenas presença validada)'} (${uiRelative})`,
  );
  console.log(
    `[demo-versionada] HTML final: ${normalizePath(path.relative(templateRoot, htmlSummaryFile))}`,
  );

  if (execution.exitCode !== 0) {
    process.exit(execution.exitCode);
  }
}

main().catch((error) => {
  console.error('[demo-versionada] Falha inesperada:', error);
  process.exit(1);
});
