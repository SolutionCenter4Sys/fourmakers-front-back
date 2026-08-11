/**
 * Status dos artefatos versionados da demo (E2E e por agente).
 * CLI: node DEMO/scripts/demo-status.cjs
 */
const {
  getLatestBdd,
  getLatestData,
  getLatestSpec,
  getLatestSeed,
  PATHS,
  listVersions,
} = require('./demo-version.cjs');

function line(label, info) {
  if (!info) return `  ${label}: (nenhum)`;
  return `  ${label}: v${info.version} → ${info.relative}`;
}

console.log('\n📦 DEMO REEMBOLSO — Status dos artefatos\n');
console.log(line('BDD (GherkinFlow)', getLatestBdd()));
console.log(line('Data (DataForge)', getLatestData()));
console.log(line('Spec (Playwright)', getLatestSpec()));
console.log(line('Seed SQL (DataForge showcase)', getLatestSeed()));

const bddV = listVersions(PATHS.bddDir, PATHS.bddPattern);
const dataV = listVersions(PATHS.dataDir, PATHS.dataPattern);
const specV = listVersions(PATHS.specDir, PATHS.specPattern);
const seedV = listVersions(PATHS.seedDir, PATHS.seedPattern);

const latestBdd = getLatestBdd();
const latestData = getLatestData();
const aligned =
  latestBdd &&
  latestData &&
  latestBdd.version === latestData.version &&
  getLatestSpec()?.version === latestBdd.version;

console.log('\nVersoes por pasta:');
console.log(`  BDD:  [${bddV.join(', ') || '—'}]`);
console.log(`  Data: [${dataV.join(', ') || '—'}]`);
console.log(`  Spec: [${specV.join(', ') || '—'}]`);
console.log(`  Seed: [${seedV.join(', ') || '—'}]`);

console.log('\nComandos showcase (Cursor Chat):');
console.log('  demo gherkinflow  → só GherkinFlow');
console.log('  demo dataforge    → só DataForge (precisa BDD)');
console.log('  demo playwright   → só Playwright (precisa BDD + data + ui.json)');
console.log('  demo reembolso    → jornada E2E completa (3 agentes)');

if (aligned) {
  console.log(`\n✅ Trio alinhado em v${latestBdd.version} — pronto para demo playwright ou E2E.`);
} else if (latestBdd && !latestData) {
  console.log(
    `\n⚠️  BDD v${latestBdd.version} sem mock JS — para Playwright/E2E rode demo reembolso (Etapa 2). Showcase demo dataforge gera apenas seed SQL.`,
  );
} else if (latestBdd && latestData && latestBdd.version !== latestData.version) {
  console.log(
    `\n⚠️  BDD v${latestBdd.version} e data v${latestData.version} desalinhados — alinhe antes do Playwright.`,
  );
} else if (!latestBdd) {
  console.log('\n⚠️  Sem BDD — comece com: demo gherkinflow');
}
console.log('');
