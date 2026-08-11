/**
 * Carrega manifesto de módulo da demo E2E e resolve caminhos/versionamento.
 * CLI: node DEMO/scripts/demo-modulo.cjs [moduloId] | list | sources
 */
const fs = require('node:fs');
const path = require('node:path');

const demoRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(demoRoot, '..');
const modulosDir = path.join(demoRoot, 'modulos');

function loadManifest(moduleId = 'reembolso') {
  const manifestPath = path.join(modulosDir, `${moduleId}.manifest.json`);
  if (!fs.existsSync(manifestPath)) {
    throw new Error(`Manifesto nao encontrado: DEMO/modulos/${moduleId}.manifest.json`);
  }
  const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf8'));
  return { manifest, manifestPath };
}

function listManifests() {
  if (!fs.existsSync(modulosDir)) return [];
  return fs
    .readdirSync(modulosDir)
    .filter((name) => name.endsWith('.manifest.json'))
    .map((name) => name.replace('.manifest.json', ''));
}

function getDefaultModuleId() {
  for (const id of listManifests()) {
    const { manifest } = loadManifest(id);
    if (manifest.padrao) return id;
  }
  return 'reembolso';
}

function resolveRepoPath(relativePath) {
  return path.join(repoRoot, relativePath.replace(/\//g, path.sep));
}

function collectSourceFiles(manifest, { prioridadeMinima = 'baixa' } = {}) {
  const ordem = { alta: 3, media: 2, baixa: 1 };
  const min = ordem[prioridadeMinima] ?? 1;
  const files = [];

  for (const tela of manifest.telas || []) {
    if ((ordem[tela.prioridade] ?? 1) < min) continue;
    for (const rel of tela.arquivos || []) {
      files.push({ grupo: `tela:${tela.id}`, rel, abs: resolveRepoPath(rel), tela: tela.nome });
    }
  }

  for (const rel of manifest.apis || []) {
    files.push({ grupo: 'api', rel, abs: resolveRepoPath(rel) });
  }

  for (const rel of manifest.dominio || []) {
    files.push({ grupo: 'dominio', rel, abs: resolveRepoPath(rel) });
  }

  const seen = new Set();
  return files.filter((entry) => {
    if (seen.has(entry.rel)) return false;
    seen.add(entry.rel);
    return true;
  });
}

function validateSources(manifest, options) {
  const files = collectSourceFiles(manifest, options);
  const missing = files.filter((f) => !fs.existsSync(f.abs));
  return { files, missing, ok: missing.length === 0 };
}

function outputPath(manifest, kind, version) {
  const cfg = manifest.outputs?.[kind];
  if (!cfg) throw new Error(`Output desconhecido: ${kind}`);
  const rel = path.join(cfg.dir, cfg.arquivo.replace('{N}', String(version))).replace(/\\/g, '/');
  return { relative: rel, absolute: resolveRepoPath(rel) };
}

function getNextVersion(manifest) {
  const max = manifest.versionamento?.maxVersoes ?? 10;

  function maxInDir(dirRel, patternStr) {
    const dir = resolveRepoPath(dirRel);
    if (!fs.existsSync(dir)) return 0;
    const pattern = new RegExp(patternStr);
    let maxV = 0;
    for (const entry of fs.readdirSync(dir)) {
      const m = entry.match(pattern);
      if (m) maxV = Math.max(maxV, Number(m[1], 10));
    }
    return maxV;
  }

  const vBdd = maxInDir(manifest.outputs.bdd.dir, manifest.versionamento.bddPattern);
  const vData = maxInDir(manifest.outputs.data.dir, manifest.versionamento.dataPattern);
  const vSpec = maxInDir(manifest.outputs.spec.dir, manifest.versionamento.specPattern);
  const currentMax = Math.max(vBdd, vData, vSpec);

  if (currentMax >= max) {
    return { version: 1, pruned: true, previousMax: currentMax };
  }
  return { version: currentMax + 1, pruned: false, previousMax: currentMax };
}

function printSummary(manifest) {
  const { files, missing } = validateSources(manifest);
  const lines = [
    `\n📦 Modulo: ${manifest.nome} (${manifest.id})`,
    `   Telas: ${(manifest.telas || []).map((t) => t.nome).join(' · ')}`,
    `   Arquivos-fonte: ${files.length} (${missing.length} ausente(s))`,
  ];

  if (missing.length) {
    lines.push('   ⚠️  Ausentes:');
    for (const m of missing) lines.push(`      - ${m.rel}`);
  }

  lines.push('\n   Fontes por tela (GherkinFlow):');
  for (const tela of manifest.telas || []) {
    lines.push(`   · ${tela.nome} [${tela.prefixoCenario}-xx]:`);
    for (const rel of tela.arquivos || []) {
      const mark = fs.existsSync(resolveRepoPath(rel)) ? '✅' : '❌';
      lines.push(`     ${mark} ${rel}`);
    }
  }

  lines.push('\n   APIs (DataForge):');
  for (const rel of manifest.apis || []) {
    const mark = fs.existsSync(resolveRepoPath(rel)) ? '✅' : '❌';
    lines.push(`     ${mark} ${rel}`);
  }

  return lines.join('\n');
}

const command = process.argv[2] || 'summary';
const moduleId = process.argv[3] || getDefaultModuleId();

try {
  switch (command) {
    case 'list': {
      console.log(JSON.stringify({ modulos: listManifests(), padrao: getDefaultModuleId() }, null, 2));
      break;
    }
    case 'sources': {
      const { manifest } = loadManifest(moduleId);
      const { files, missing } = validateSources(manifest);
      console.log(JSON.stringify({ moduleId, files, missing: missing.map((m) => m.rel) }, null, 2));
      break;
    }
    case 'next-version': {
      const { manifest } = loadManifest(moduleId);
      console.log(JSON.stringify(getNextVersion(manifest), null, 2));
      break;
    }
    case 'summary':
    default: {
      const { manifest } = loadManifest(moduleId);
      console.log(printSummary(manifest));
      break;
    }
  }
} catch (error) {
  console.error(`[demo-modulo] ${error.message}`);
  process.exit(1);
}

module.exports = {
  demoRoot,
  repoRoot,
  loadManifest,
  listManifests,
  getDefaultModuleId,
  collectSourceFiles,
  validateSources,
  outputPath,
  getNextVersion,
  printSummary,
  resolveRepoPath,
};
