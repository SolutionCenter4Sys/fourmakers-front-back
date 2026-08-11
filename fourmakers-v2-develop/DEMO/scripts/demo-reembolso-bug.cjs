/**
 * Demo Reembolso — bug intencional + limpeza de versões.
 * CLI: inject | fix | status | clean
 */
const fs = require('node:fs');
const path = require('node:path');

const demoRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(demoRoot, '..');
const frontendRoot = path.join(repoRoot, 'frontend');
const outputsRoot = path.join(demoRoot, 'outputs');
const reembolsoTsx = path.join(frontendRoot, 'src', 'presentation', 'pages', 'Reembolso.tsx');
const stateFile = path.join(__dirname, '..', 'setup', '.demo-preparada.json');

const ROTA_CORRETA = '/inserir-reembolso';
const ROTA_BUG = '/inserir-reembolso-invalido';
const MARKER = 'ROTA_NOVA_SOLICITACAO_REEMBOLSO';

const LINE_CORRETA = `const ${MARKER} = "${ROTA_CORRETA}"; // DEMO_BUG_ATIVO: false`;
const LINE_BUG = `const ${MARKER} = "${ROTA_BUG}"; // DEMO_BUG_ATIVO: true`;

const VERSION_PATTERNS = [
  {
    dir: path.join(outputsRoot, 'gherkinflow', 'reembolso'),
    pattern: /^REEMBOLSO-BDD-v\d+\.md$/,
  },
  {
    dir: path.join(outputsRoot, 'dataforge', 'reembolso'),
    pattern: /^reembolso\.data\.v\d+\.js$/,
  },
  {
    dir: path.join(outputsRoot, 'playwright', 'reembolso', 'specs'),
    pattern: /^reembolso-demo-v\d+\.spec\.ts$/,
  },
  {
    dir: path.join(outputsRoot, 'playwright', 'reembolso', 'relatorios'),
    pattern: /^demo-reembolso-v\d+\.(html|json)$/,
  },
];

function readReembolsoSource() {
  if (!fs.existsSync(reembolsoTsx)) {
    throw new Error(`Arquivo nao encontrado: ${reembolsoTsx}`);
  }
  return fs.readFileSync(reembolsoTsx, 'utf8');
}

function writeReembolsoSource(content) {
  fs.writeFileSync(reembolsoTsx, content, 'utf8');
}

function patchRota(content, targetLine) {
  const regex = new RegExp(
    `const ${MARKER} = "[^"]+"; \\/\\/ DEMO_BUG_ATIVO: (true|false)`,
  );
  if (!regex.test(content)) {
    throw new Error(
      `Marcador ${MARKER} nao encontrado em Reembolso.tsx. ` +
        'Verifique se a constante de demo esta presente.',
    );
  }
  return content.replace(regex, targetLine);
}

function injectBug() {
  const updated = patchRota(readReembolsoSource(), LINE_BUG);
  writeReembolsoSource(updated);
  return { ativo: true, rota: ROTA_BUG };
}

function fixBug() {
  const updated = patchRota(readReembolsoSource(), LINE_CORRETA);
  writeReembolsoSource(updated);
  return { ativo: false, rota: ROTA_CORRETA };
}

function getBugStatus() {
  const content = readReembolsoSource();
  const match = content.match(
    new RegExp(`const ${MARKER} = "([^"]+)"; \\/\\/ DEMO_BUG_ATIVO: (true|false)`),
  );
  if (!match) {
    return { ativo: null, rota: null, markerFound: false };
  }
  return {
    ativo: match[2] === 'true',
    rota: match[1],
    markerFound: true,
  };
}

function cleanVersionedArtifacts() {
  const removed = [];

  for (const { dir, pattern } of VERSION_PATTERNS) {
    if (!fs.existsSync(dir)) continue;
    for (const entry of fs.readdirSync(dir)) {
      if (!pattern.test(entry)) continue;
      const fullPath = path.join(dir, entry);
      fs.unlinkSync(fullPath);
      removed.push(path.relative(repoRoot, fullPath).replace(/\\/g, '/'));
    }
  }

  return removed;
}

function writePreparedState(extra = {}) {
  const payload = {
    preparadaEm: new Date().toISOString(),
    bugAtivo: true,
    rotaBug: ROTA_BUG,
    rotaCorreta: ROTA_CORRETA,
    comandoDemonstracao: 'demo reembolso',
    ...extra,
  };
  fs.writeFileSync(stateFile, `${JSON.stringify(payload, null, 2)}\n`, 'utf8');
  return payload;
}

function readPreparedState() {
  if (!fs.existsSync(stateFile)) return null;
  try {
    return JSON.parse(fs.readFileSync(stateFile, 'utf8'));
  } catch {
    return null;
  }
}

if (require.main === module) {
  const command = process.argv[2] || 'status';

  try {
    switch (command) {
      case 'inject': {
        const result = injectBug();
        console.log(`[demo-bug] Bug injetado: ${result.rota}`);
        break;
      }
      case 'fix': {
        const result = fixBug();
        console.log(`[demo-bug] Bug corrigido: ${result.rota}`);
        break;
      }
      case 'clean': {
        const removed = cleanVersionedArtifacts();
        if (!removed.length) {
          console.log('[demo-bug] Nenhum artefato versionado para remover.');
        } else {
          console.log(`[demo-bug] ${removed.length} artefato(s) removido(s):`);
          for (const file of removed) console.log(`  - ${file}`);
        }
        break;
      }
      case 'status': {
        const status = getBugStatus();
        const prepared = readPreparedState();
        console.log(JSON.stringify({ bug: status, preparada: prepared }, null, 2));
        break;
      }
      case 'mark-prepared': {
        writePreparedState();
        console.log(`[demo-bug] Estado salvo em ${path.relative(repoRoot, stateFile)}`);
        break;
      }
      default:
        console.error(`Comando desconhecido: ${command}. Use: inject | fix | status | clean | mark-prepared`);
        process.exit(1);
    }
  } catch (error) {
    console.error(`[demo-bug] ${error.message}`);
    process.exit(1);
  }
}

module.exports = {
  injectBug,
  fixBug,
  getBugStatus,
  cleanVersionedArtifacts,
  writePreparedState,
  readPreparedState,
  paths: { reembolsoTsx, stateFile, repoRoot, frontendRoot, demoRoot },
};
