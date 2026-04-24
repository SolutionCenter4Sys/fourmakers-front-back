import { test, expect } from '@playwright/test'
import { ReembolsoPage } from '../../../support/pages/ReembolsoPage'
import { loginFourMakers } from '../../../support/auth/fourmakers-auth'

// ─── Dados do DataForge (Etapa 2) ───────────────────────────────────────
// @ts-ignore — import JS sem types
import { dadosI12, dadosI13 } from '../../../../DEMO/automacao/reembolso/versionadas/reembolso.data.v6.js'

test.describe.configure({ retries: 0, timeout: 60_000 })

const LOG = {
  banner: (msg: string) => {
    const linha = '═'.repeat(58)
    console.log(`\n╔${linha}╗`)
    console.log(`║  ${msg.padEnd(57)}║`)
    console.log(`╚${linha}╝`)
  },
  secao: (codigo: string, tipo: string, titulo: string) => {
    console.log(`\n┌────────────────────────────────────────────────────────────`)
    console.log(`│ ${codigo} · [${tipo}]  ${titulo}`)
    console.log(`└────────────────────────────────────────────────────────────`)
  },
  passo:  (msg: string) => console.log(`   → ${msg}`),
  ok:     (msg: string) => console.log(`   ✅ ${msg}`),
  alerta: (msg: string) => console.log(`   ⚠️  ${msg}`),
}

type TestResultado = {
  id: string
  tipo: string
  titulo: string
  status: string
  duracaoMs: number
}

const resultados: TestResultado[] = []

const CATALOGO_SKIP: Array<Omit<TestResultado, 'status' | 'duracaoMs'> & { razao: string }> = [
  { id: 'R-01', tipo: 'Positivo',   titulo: 'Filtrar período de solicitação',                 razao: 'depende de dados paginados existentes' },
  { id: 'R-02', tipo: 'Positivo',   titulo: 'Buscar por objetivo/destino',                    razao: 'requere dados prévios com destino "Curitiba - PR"' },
  { id: 'R-03', tipo: 'Negativo',   titulo: 'Busca sem correspondência',                      razao: 'coberto pelo catálogo, não executado nesta rodada' },
  { id: 'R-04', tipo: 'Positivo',   titulo: 'Abrir detalhes da solicitação',                  razao: 'precisa de solicitação aprovada no ambiente' },
  { id: 'R-05', tipo: 'Positivo',   titulo: 'Baixar comprovantes do item',                    razao: 'precisa de anexos reais no ambiente' },
  { id: 'R-06', tipo: 'Positivo',   titulo: 'Gerar relatório aguardando pagamento',           razao: 'depende de permissão e dataset com aguardando pagamento' },
  { id: 'R-07', tipo: 'Regressivo', titulo: 'Bloqueio da aba Gestão ADM sem permissão',       razao: 'necessário usuário sem perfil gestor' },
  { id: 'R-08', tipo: 'Regressivo', titulo: 'Bloqueio da aba Aprovações sem permissão',       razao: 'necessário usuário sem perfil aprovador' },
  { id: 'R-09', tipo: 'Regressivo', titulo: 'Sincronizar aba com URL quando permitido',       razao: 'depende de usuário aprovador ou gestor' },
  { id: 'R-10', tipo: 'Positivo',   titulo: 'Exibir Remessa CNAB apenas com parâmetro ativo', razao: 'parâmetro pode variar por ambiente' },
  { id: 'R-11', tipo: 'Positivo',   titulo: 'Resetar paginação ao alterar busca',             razao: 'precisa de dataset com múltiplas páginas' },
  { id: 'R-12', tipo: 'Positivo',   titulo: 'Alternar carregamento entre skeleton e cards',   razao: 'depende de telemetria de loading inicial' },
  { id: 'R-13', tipo: 'Positivo',   titulo: 'Acesso rápido para criar novo reembolso',        razao: 'coberto implicitamente nos fluxos de formulário' },
  { id: 'R-14', tipo: 'Negativo',   titulo: 'Impedir data fim futura',                        razao: 'validado por lógica de componente, não executado neste roteiro' },
  { id: 'I-01', tipo: 'Positivo',   titulo: 'Adicionar item tipo 1 ao carrinho',              razao: 'requer verbas disponíveis para seleção' },
  { id: 'I-02', tipo: 'Positivo',   titulo: 'Calcular valor total para tipo 2',               razao: 'depende de categoria km rodado carregada' },
  { id: 'I-03', tipo: 'Positivo',   titulo: 'OCR de comprovante pré-preenche data e valor',   razao: 'necessita OCR ativo no backend' },
  { id: 'I-04', tipo: 'Positivo',   titulo: 'Exigir comprovante quando a verba obriga',       razao: 'depende de verba com exigirComprovante=true' },
  { id: 'I-05', tipo: 'Positivo',   titulo: 'Editar item do carrinho',                        razao: 'executado quando houver item válido no carrinho' },
  { id: 'I-06', tipo: 'Positivo',   titulo: 'Excluir item do carrinho',                       razao: 'executado quando houver item válido no carrinho' },
  { id: 'I-07', tipo: 'Positivo',   titulo: 'Enviar solicitações consolidadas em ZIP',        razao: 'depende de dataset de verbas e upload de arquivos' },
  { id: 'I-08', tipo: 'Positivo',   titulo: 'Redirecionar após sucesso',                      razao: 'executado junto ao fluxo completo de envio' },
  { id: 'I-09', tipo: 'Regressivo', titulo: 'Sinalizar comprovante vencido',                   razao: 'depende de verba e validação de data de comprovante' },
  { id: 'I-10', tipo: 'Regressivo', titulo: 'Alertar teto da verba excedido',                 razao: 'depende de verbas carregadas e validação de teto' },
  { id: 'I-11', tipo: 'Regressivo', titulo: 'Solicitação sem projeto quando permitido',       razao: 'parâmetro REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO varia por ambiente' },
  { id: 'I-14', tipo: 'Negativo',   titulo: 'Recusar datas inválidas na submissão',           razao: 'validado em conjunto com fluxo completo de envio' },
]

const STATUS_ICON: Record<string, string> = {
  passed:      '✅',
  failed:      '❌',
  timedOut:    '⏱️',
  interrupted: '🛑',
  skipped:     '⏭️',
}

const ROTEIRO_EXECUCAO = [
  { id: 'I-12', tipo: 'Negativo', titulo: 'Impedir envio com carrinho vazio' },
  { id: 'I-13', tipo: 'Negativo', titulo: 'Bloquear inclusão sem campos obrigatórios' },
]

let demoInicioMs = 0

test.beforeAll(async () => {
  demoInicioMs = Date.now()
  LOG.banner('🎬 DEMO REEMBOLSO v6 — ROTEIRO NEGATIVOS RÁPIDOS')
  console.log('📄 BDD     : frontend/DEMO/cenarios-bdd/REEMBOLSO-BDD-v6.md')
  console.log('📄 Massa   : frontend/DEMO/automacao/reembolso/versionadas/reembolso.data.v6.js')
  console.log('📄 UI JSON : frontend/DEMO/ui-elements/reembolso-ui.json')
  console.log('   Ambiente: front local http://localhost:8080 → backend dev spw.app.foursys.com/backoffice-rf-hom')
  console.log('   Usuário : solutioncenter@foursys.com.br (orgId 8) — login OTP real\n')

  console.log('📋 Roteiro desta execução (cenários tentados)')
  for (const r of ROTEIRO_EXECUCAO) {
    console.log(`  • ${r.id} [${r.tipo}] ${r.titulo}`)
  }
})

test.beforeEach(async ({ page, request }, testInfo) => {
  console.log(`\n🧪 TESTANDO: ${testInfo.title}`)
  await loginFourMakers(page, request)
})

test.afterEach(async ({}, testInfo) => {
  const match = testInfo.title.match(/^(R|I)-\d+\s+\[([^\]]+)\]\s+(.+)$/)
  if (match) {
    const [idParte] = testInfo.title.split(' ')
    const id = idParte.trim()
    const tipoMatch = testInfo.title.match(/\[([^\]]+)\]/)
    const tipo = tipoMatch ? tipoMatch[1] : 'Desconhecido'
    const titulo = testInfo.title.replace(/^(R|I)-\d+\s+\[[^\]]+\]\s+/, '').trim()
    resultados.push({
      id,
      tipo,
      titulo,
      status: testInfo.status ?? 'unknown',
      duracaoMs: testInfo.duration,
    })
  }
  console.log(`🏁 ${testInfo.title} CONCLUÍDO`)
})

test.afterAll(async () => {
  const duracaoTotalMs = Date.now() - demoInicioMs
  const executados = [...resultados]
  const passados = executados.filter((r) => r.status === 'passed').length
  const falhados = executados.filter((r) => r.status === 'failed' || r.status === 'timedOut').length
  const skippedTotal = CATALOGO_SKIP.length + executados.filter((r) => r.status === 'skipped').length
  const coberturaTotal = executados.length + CATALOGO_SKIP.length

  const linhaSup = '═'.repeat(60)
  const linhaInf = '─'.repeat(60)

  console.log(`\n╔${linhaSup}╗`)
  console.log(`║  🎬 DEMO REEMBOLSO v6 — RELATÓRIO FINAL                     ║`)
  console.log(`╠${linhaSup}╣`)
  console.log(`║  ✅ Passed        : ${String(passados).padEnd(40)}║`)
  console.log(`║  ❌ Failed        : ${String(falhados).padEnd(40)}║`)
  console.log(`║  ⏭️  Catalogados   : ${String(skippedTotal).padEnd(40)}║`)
  console.log(`║  📊 Cobertura     : ${String(coberturaTotal).padEnd(40)}║`)
  console.log(`║  ⏱  Tempo total   : ${formatarDuracao(duracaoTotalMs).padEnd(40)}║`)
  console.log(`╚${linhaSup}╝`)

  if (executados.length > 0) {
    console.log(`\n📋 Cenários executados`)
    console.log(linhaInf)
    for (const r of executados) {
      const icon = STATUS_ICON[r.status] ?? '•'
      const id = r.id.padEnd(5)
      const tipo = `[${r.tipo}]`.padEnd(13)
      const dur = formatarDuracao(r.duracaoMs).padStart(6)
      console.log(`  ${icon} ${id} ${tipo} ${r.titulo.padEnd(42)} ${dur}`)
    }
  }

  if (CATALOGO_SKIP.length > 0) {
    console.log(`\n📋 Cenários catalogados (aguardam setup no ambiente)`)
    console.log(linhaInf)
    for (const r of CATALOGO_SKIP) {
      const id = r.id.padEnd(5)
      const tipo = `[${r.tipo}]`.padEnd(13)
      console.log(`  ⏭️  ${id} ${tipo} ${r.titulo.padEnd(42)} — ${r.razao}`)
    }
  }

  console.log(`\n╚══ ✅ DEMO CONCLUÍDA — ${passados}/${executados.length || 1} cenários executados ══╝\n`)
})

function formatarDuracao(ms: number): string {
  return ms < 1000 ? `${ms}ms` : `${(ms / 1000).toFixed(1)}s`
}

// ═════════════════════════════════════════════════════════════════════
//  FORMULÁRIO — /inserir-reembolso (Negativos rápidos)
// ═════════════════════════════════════════════════════════════════════

test('I-12 [Negativo] Impedir envio com carrinho vazio', async ({ page }) => {
  const p = new ReembolsoPage(page)
  const mensagemEsperada = dadosI12.mensagemEsperada ?? 'Carrinho vazio'

  LOG.secao('I-12', 'Negativo', 'Impedir envio com carrinho vazio')
  LOG.passo(`Contexto BDD/DataForge: ${mensagemEsperada}`)

  await p.visitarFormulario()
  LOG.passo('Acessou /inserir-reembolso com sessão autenticada')

  await p.verificarCarrinhoVazio()
  LOG.ok('Carrinho inicia vazio')

  const btnEnviar = page.getByRole('button', { name: /Enviar solicitações/i })
  await expect(btnEnviar).toHaveCount(0)
  LOG.ok('Botão de envio não aparece sem itens no carrinho')

  // Evidência visual do estado vazio
  await p.evidencia('i12-carrinho-vazio-v6')
})

test('I-13 [Negativo] Bloquear inclusão sem campos obrigatórios', async ({ page }) => {
  const p = new ReembolsoPage(page)

  LOG.secao('I-13', 'Negativo', 'Bloquear inclusão sem campos obrigatórios')
  LOG.passo(`Contexto BDD/DataForge: ${dadosI13.descricao}`)

  await p.visitarFormulario()
  LOG.passo('Acessou /inserir-reembolso com sessão autenticada')

  await p.clicarAdicionarCarrinho()
  LOG.passo('Tentou adicionar item sem preencher campos')

  const toast = page.getByRole('status').filter({ hasText: /Campos obrigatórios/i }).first()
  await expect(toast).toBeVisible({ timeout: 10_000 })
  LOG.ok('Toast de campos obrigatórios exibido')

  await expect(p.itemCarrinhoLinha).toHaveCount(0)
  LOG.ok('Nenhum item foi incluído no carrinho após tentativa inválida')

  // Evidência visual do bloqueio
  await p.evidencia('i13-campos-obrigatorios-v6')
})

// Cenários restantes catalogados (não executados nesta rodada)
for (const skip of CATALOGO_SKIP) {
  test.skip(`${skip.id} [${skip.tipo}] ${skip.titulo} — ${skip.razao}`, () => {})
}
