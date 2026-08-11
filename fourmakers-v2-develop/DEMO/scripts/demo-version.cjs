/**
 * Versionamento compartilhado da demo Reembolso (E2E e por agente).
 */
const fs = require('node:fs');
const path = require('node:path');

const demoRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(demoRoot, '..');
const frontendRoot = path.join(repoRoot, 'frontend');
const outputsRoot = path.join(demoRoot, 'outputs');

const PATHS = {
  bddDir: path.join(outputsRoot, 'gherkinflow', 'reembolso'),
  dataDir: path.join(outputsRoot, 'dataforge', 'reembolso'),
  specDir: path.join(outputsRoot, 'playwright', 'reembolso', 'specs'),
  seedDir: path.join(demoRoot, 'dataforge', 'banco-ficticio', 'versionadas'),
  bddPattern: /^REEMBOLSO-BDD-v(\d+)\.md$/,
  dataPattern: /^reembolso\.data\.v(\d+)\.js$/,
  specPattern: /^reembolso-demo-v(\d+)\.spec\.ts$/,
  seedPattern: /^reembolso-seed\.v(\d+)\.sql$/,
  maxVersions: 10,
};

function listVersions(dir, pattern) {
  if (!fs.existsSync(dir)) return [];
  const versions = [];
  for (const entry of fs.readdirSync(dir)) {
    const match = entry.match(pattern);
    if (match) versions.push(Number(match[1], 10));
  }
  return versions.sort((a, b) => a - b);
}

function getMaxVersion(dir, pattern) {
  const versions = listVersions(dir, pattern);
  return versions.length ? Math.max(...versions) : 0;
}

function getNextVersion(dir, pattern) {
  const max = getMaxVersion(dir, pattern);
  if (max >= PATHS.maxVersions) {
    return { version: 1, pruned: true, previousMax: max };
  }
  return { version: max + 1, pruned: false, previousMax: max };
}

function bddFile(version) {
  return path.join(PATHS.bddDir, `REEMBOLSO-BDD-v${version}.md`);
}

function dataFile(version) {
  return path.join(PATHS.dataDir, `reembolso.data.v${version}.js`);
}

function specFile(version) {
  return path.join(PATHS.specDir, `reembolso-demo-v${version}.spec.ts`);
}

function seedFile(version) {
  return path.join(PATHS.seedDir, `reembolso-seed.v${version}.sql`);
}

function getLatestBdd() {
  const max = getMaxVersion(PATHS.bddDir, PATHS.bddPattern);
  if (!max) return null;
  return {
    version: max,
    path: bddFile(max),
    relative: `DEMO/outputs/gherkinflow/reembolso/REEMBOLSO-BDD-v${max}.md`,
  };
}

function getLatestData() {
  const max = getMaxVersion(PATHS.dataDir, PATHS.dataPattern);
  if (!max) return null;
  return {
    version: max,
    path: dataFile(max),
    relative: `DEMO/outputs/dataforge/reembolso/reembolso.data.v${max}.js`,
  };
}

function getLatestSpec() {
  const max = getMaxVersion(PATHS.specDir, PATHS.specPattern);
  if (!max) return null;
  return {
    version: max,
    path: specFile(max),
    relative: `DEMO/outputs/playwright/reembolso/specs/reembolso-demo-v${max}.spec.ts`,
  };
}

function getLatestSeed() {
  const max = getMaxVersion(PATHS.seedDir, PATHS.seedPattern);
  if (!max) return null;
  return {
    version: max,
    path: seedFile(max),
    relative: `DEMO/dataforge/banco-ficticio/versionadas/reembolso-seed.v${max}.sql`,
  };
}

function artifactExists(type, version) {
  const map = {
    bdd: bddFile,
    data: dataFile,
    spec: specFile,
    seed: seedFile,
  };
  const resolver = map[type];
  if (!resolver) throw new Error(`Tipo de artefato desconhecido: ${type}`);
  return fs.existsSync(resolver(version));
}

module.exports = {
  PATHS,
  demoRoot,
  frontendRoot,
  repoRoot,
  listVersions,
  getMaxVersion,
  getNextVersion,
  bddFile,
  dataFile,
  specFile,
  getLatestBdd,
  getLatestData,
  getLatestSpec,
  getLatestSeed,
  seedFile,
  artifactExists,
};
