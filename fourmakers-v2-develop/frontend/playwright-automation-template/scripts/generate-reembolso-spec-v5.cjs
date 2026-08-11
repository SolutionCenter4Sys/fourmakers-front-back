const fs = require('fs');
const path = require('path');

const templateRoot = path.join(__dirname, '..');
const frontendRoot = path.resolve(templateRoot, '..');
const repoRoot = path.resolve(frontendRoot, '..');
const demoRoot = path.join(repoRoot, 'DEMO');
const outputsRoot = path.join(demoRoot, 'outputs');
const bddFile = path.join(outputsRoot, 'gherkinflow', 'reembolso', 'REEMBOLSO-BDD-v5.md');
const dataFile = path.join(outputsRoot, 'dataforge', 'reembolso', 'reembolso.data.v5.js');
const outFile = path.join(
  outputsRoot,
  'playwright',
  'reembolso',
  'specs',
  'reembolso-demo-v5.spec.ts',
);

function parseBddRows(content) {
  const rows = [];
  const re = /^\|\s*([A-Z]{2}-\d{2})\s*\|\s*\[([^\]]+)\]\s*([^|]+?)\s*\|/gm;
  let match = re.exec(content);
  while (match) {
    rows.push({ id: match[1], tipo: match[2], titulo: match[3].trim() });
    match = re.exec(content);
  }
  return rows;
}

function parseDataExports(content) {
  const exports = [];
  const re = /^export const (dados[A-Z]{2}\d{2})\s*=\s*cenarios\['([A-Z]{2}-\d{2})'\]/gm;
  let match = re.exec(content);
  while (match) {
    exports.push({ constName: match[1], id: match[2] });
    match = re.exec(content);
  }
  return exports;
}

function toConstName(id) {
  const [prefix, num] = id.split('-');
  return `dados${prefix}${num}`;
}

function buildSpec(bddRows, dataExports) {
  const exportNames = dataExports.map((item) => item.constName);
  const importBlock = exportNames.join(',\n  ');

  const tests = bddRows
    .map(({ id, tipo, titulo }) => {
      const constName = toConstName(id);
      const tituloEsc = titulo.replace(/'/g, "\\'");
      const skipReasons = require('../support/reembolso-v5/skip-reasons.ts');
      // Cannot require TS in cjs easily — inline skip check via executables set
      const executaveis = new Set([
        'RD-01', 'RD-03', 'RD-08', 'RD-10', 'RD-11', 'RD-14',
        'AP-08', 'IR-11', 'IR-14', 'IR-15', 'IR-18',
      ]);

      if (executaveis.has(id)) {
        return `test('${id} [${tipo}] ${tituloEsc}', async ({ page, request }) => {
  console.log('\\n🧪 TESTANDO: ${id} — ${tituloEsc}')
  await executarCenarioV5(page, request, ${constName}, LOG, async () => {
    await loginFourMakers(page, request)
  })
  console.log('🏁 ${id} CONCLUÍDO\\n')
})`;
      }

      const motivo = getSkipReasonInline(id, tipo, titulo);
      return `test.skip('${id} [${tipo}] ${tituloEsc} — ${motivo}', () => {})`;
    })
    .join('\n\n');

  return `/**
 * Playwright — Módulo Reembolso v5
 * Gerado automaticamente — não editar manualmente (regenerar via scripts/generate-reembolso-spec-v5.cjs)
 *
 * Artefatos consumidos:
 *   📄 DEMO/outputs/gherkinflow/reembolso/REEMBOLSO-BDD-v5.md
 *   📄 DEMO/outputs/dataforge/reembolso/reembolso.data.v5.js
 *   📄 DEMO/inputs/ui-elements/reembolso-ui.json → ReembolsoPage.ts
 *
 * Executar:
 *   cd fourmakers-v2-develop/frontend/playwright-automation-template
 *   npm run demo
 */

import { test } from '../../../../../frontend/playwright-automation-template/support/playwright-test'
import { loginFourMakers } from '../../../../../frontend/playwright-automation-template/support/auth/fourmakers-auth'
import { LOG } from '../../../../../frontend/playwright-automation-template/support/reembolso-v5/demo-log'
import { executarCenarioV5 } from '../../../../../frontend/playwright-automation-template/support/reembolso-v5/executar-cenario-v5'
// @ts-ignore — massa DataForge v5 (JS sem types)
import {
  ${importBlock},
  dataforgeMeta,
} from '../../../dataforge/reembolso/reembolso.data.v5.js'

test.describe.configure({ retries: 0 })

test.beforeAll(async () => {
  LOG.banner('AUTOMAÇÃO E2E — MÓDULO REEMBOLSO v5')
  console.log(\`   📦 Massa DataForge: \${dataforgeMeta.total} cenários (\${dataforgeMeta.modo})\`)
  console.log('   📄 BDD: REEMBOLSO-BDD-v5.md')
  console.log('   🧭 UI: reembolso-ui.json → ReembolsoPage')
})

test.beforeEach(async ({ page, request }) => {
  await loginFourMakers(page, request)
})

test.afterAll(async () => {
  console.log('╚══ ✅ CATÁLOGO v5 CONCLUÍDO ══╝')
})

${tests}
`;
}

function getSkipReasonInline(id, tipo, titulo) {
  const prefix = id.slice(0, 2);
  if (['GA'].includes(prefix)) return 'requer perfil gestor na aba Gestão ADM';
  if (prefix === 'CN') return 'requer perfil gestor e remessa CNAB habilitada';
  if (prefix === 'AV') return 'requer perfil aprovador em /reembolso/aprovar';
  if (['PR', 'PP', 'PL'].includes(prefix)) return 'requer perfil admin em /reembolso-parametros';
  if (prefix === 'AP' && id !== 'AP-08') return 'requer perfil aprovador na aba Aprovações';
  if (prefix === 'IR') return 'requer verbas e projetos configurados para o usuário OTP';
  if (prefix === 'RD') {
    if (['RD-12', 'RD-13'].includes(id)) return 'requer perfil gestor ou aprovador';
    return 'requer solicitações cadastradas ou volume específico no backend';
  }
  return 'depende de setup não disponível para usuário OTP padrão';
}

function main() {
  if (!fs.existsSync(bddFile)) {
    console.error('BDD não encontrado:', bddFile);
    process.exit(1);
  }
  if (!fs.existsSync(dataFile)) {
    console.error('Massa DataForge não encontrada:', dataFile);
    process.exit(1);
  }

  const bddRows = parseBddRows(fs.readFileSync(bddFile, 'utf8'));
  const dataExports = parseDataExports(fs.readFileSync(dataFile, 'utf8'));

  if (bddRows.length !== 129) {
    console.warn(`[warn] BDD retornou ${bddRows.length} cenários (esperado 129)`);
  }
  if (dataExports.length !== 129) {
    console.warn(`[warn] DataForge retornou ${dataExports.length} exports (esperado 129)`);
  }

  fs.mkdirSync(path.dirname(outFile), { recursive: true });
  fs.writeFileSync(outFile, buildSpec(bddRows, dataExports), 'utf8');

  const executaveis = 11;
  console.log(JSON.stringify({
    outFile,
    bdd: bddRows.length,
    data: dataExports.length,
    executaveis,
    skip: bddRows.length - executaveis,
  }, null, 2));
}

main();
