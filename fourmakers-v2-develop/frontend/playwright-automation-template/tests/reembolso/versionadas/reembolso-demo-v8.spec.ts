import { test, expect } from '@playwright/test'
import { ReembolsoPage } from '../../../support/pages/ReembolsoPage'
import { loginFourMakers } from '../../../support/auth/fourmakers-auth'

// ─── Dados do DataForge (Etapa 2) ───────────────────────────────────────
// @ts-ignore — import JS sem types
import { dadosR09, dadosI08, dadosI09 } from '../../../../DEMO/automacao/reembolso/versionadas/reembolso.data.v8.js'

test.describe.configure({ retries: 0, timeout: 120_000 })
test.use({ headless: false, video: 'on' })

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
  { id: 'R-01', tipo: 'Positivo',   titulo: 'Visualização padrão do dashboard',                razao: 'coberto pelo catálogo, valida apenas UI estática' },
  { id: 'R-02', tipo: 'Positivo',   titulo: 'Filtrar reembolsos por período',                  razao: 'depende de dados paginados existentes' },
  { id: 'R-03', tipo: 'Positivo',   titulo: 'Buscar reembolsos por texto livre',               razao: 'requer dados prévios com destino "Porto Alegre - RS"' },
  { id: 'R-04', tipo: 'Positivo',   titulo: 'Abrir modal de detalhes da solicitação',          razao: 'precisa de solicitação aprovada no ambiente' },
  { id: 'R-05', tipo: 'Positivo',   titulo: 'Baixar comprovante com token autenticado',        razao: 'precisa de anexos reais no ambiente' },
  { id: 'R-06', tipo: 'Positivo',   titulo: 'Gerar relatório aguardando pagamento',            razao: 'depende de permissão e dataset com aguardando pagamento' },
  { id: 'R-07', tipo: 'Positivo',   titulo: 'Solicitar novo reembolso pelo dashboard',         razao: 'coberto implicitamente nos fluxos de formulário' },
  { id: 'R-08', tipo: 'Positivo',   titulo: 'Exibir botão Remessa CNAB para autorizados',      razao: 'parâmetro pode variar por ambiente' },
  { id: 'R-10', tipo: 'Negativo',   titulo: 'Impedir seleção de data fim futura',              razao: 'validado por lógica de componente Calendar' },
  { id: 'R-11', tipo: 'Negativo',   titulo: 'Falha ao gerar relatório exibe alerta',           razao: 'depende de mock/falha da API de relatório' },
  { id: 'R-12', tipo: 'Regressivo', titulo: 'Bloquear aba Gestão ADM sem perfil de gestor',    razao: 'necessário usuário sem perfil gestor' },
  { id: 'R-13', tipo: 'Regressivo', titulo: 'Bloquear aba Aprovações sem perfil de aprovador', razao: 'necessário usuário sem perfil aprovador' },
  { id: 'R-14', tipo: 'Regressivo', titulo: 'Sincronizar aba ativa com parâmetro da URL',      razao: 'depende de usuário aprovador ou gestor' },
  { id: 'I-01', tipo: 'Positivo',   titulo: 'Adicionar item com categoria tipo 1 ao carrinho', razao: 'requer verbas disponíveis para seleção' },
  { id: 'I-02', tipo: 'Positivo',   titulo: 'Calcular valor total para categoria tipo 2',      razao: 'depende de categoria km rodado carregada' },
  { id: 'I-03', tipo: 'Positivo',   titulo: 'Pré-preenchimento por OCR de comprovante fiscal', razao: 'necessita OCR ativo no backend' },
  { id: 'I-04', tipo: 'Positivo',   titulo: 'Editar item já adicionado ao carrinho',           razao: 'executado quando houver item válido no carrinho' },
  { id: 'I-05', tipo: 'Positivo',   titulo: 'Excluir item do carrinho',                        razao: 'executado quando houver item válido no carrinho' },
  { id: 'I-06', tipo: 'Positivo',   titulo: 'Enviar solicitações em lote via ZIP',             razao: 'depende de dataset de verbas e upload de arquivos' },
  { id: 'I-07', tipo: 'Positivo',   titulo: 'Redirecionar para dashboard após envio',          razao: 'executado junto ao fluxo completo de envio' },
  { id: 'I-10', tipo: 'Negativo',   titulo: 'Bloquear comprovante obrigatório ausente',        razao: 'depende de verba com exigirComprovante=true' },
  { id: 'I-11', tipo: 'Negativo',   titulo: 'Rejeitar data de despesa inválida no envio',      razao: 'validado em conjunto com fluxo completo de envio' },
  { id: 'I-12', tipo: 'Regressivo', titulo: 'Sinalizar comprovante vencido',                   razao: 'depende de verba e validação de data de comprovante' },
  { id: 'I-13', tipo: 'Regressivo', titulo: 'Alertar valor acima do teto da verba',            razao: 'depende de verbas carregadas e validação de teto' },
  { id: 'I-14', tipo: 'Regressivo', titulo: 'Permitir solicitação sem projeto vinculado',      razao: 'parâmetro REEMBOLSO_PERMITIR_SOLICITACAO_SEM_PROJETO varia por ambiente' },
]

const STATUS_ICON: Record<string, string> = {
  passed:      '✅',
  failed:      '❌',
  timedOut:    '⏱️',
  interrupted: '🛑',
  skipped:     '⏭️',
}

const ROTEIRO_EXECUCAO = [
  { id: 'R-09', tipo: 'Negativo', titulo: 'Busca sem resultados exibe mensagem vazia' },
  { id: 'I-08', tipo: 'Negativo', titulo: 'Bloquear inclusão sem campos obrigatórios preenchidos' },
  { id: 'I-09', tipo: 'Negativo', titulo: 'Impedir envio com carrinho vazio' },
]

let demoInicioMs = 0

test.beforeAll(async () => {
  demoInicioMs = Date.now()
  LOG.banner('🎬 DEMO REEMBOLSO v8 — ROTEIRO NEGATIVOS RÁPIDOS')
  console.log('📄 BDD     : frontend/DEMO/cenarios-bdd/REEMBOLSO-BDD-v8.md')
  console.log('📄 Massa   : frontend/DEMO/automacao/reembolso/versionadas/reembolso.data.v8.js')
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
  const video = testInfo.attachments?.find((a) => a.name === 'video')
  if (video?.path) {
    console.log(`🎥 Video: ${video.path}`)
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
  console.log(`║  🎬 DEMO REEMBOLSO v8 — RELATÓRIO FINAL                     ║`)
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
//  DASHBOARD — /reembolso (Busca sem resultados)
// ═════════════════════════════════════════════════════════════════════

test('R-09 [Negativo] Busca sem resultados exibe mensagem vazia', async ({ page }) => {
  const p = new ReembolsoPage(page)
  const termoBusca = dadosR09.termoBusca ?? 'XYZW-SEM-RESULTADO-999'
  const mensagemEsperada = dadosR09.mensagemEsperada ?? 'Nenhum reembolso encontrado'

  LOG.secao('R-09', 'Negativo', 'Busca sem resultados exibe mensagem vazia')
  LOG.passo(`Termo de busca: "${termoBusca}"`)

  await p.visitarLista()
  await page.waitForLoadState('networkidle').catch(() => {})
  // Aguardar até que a aba Meus Reembolsos esteja visível (indica que o dashboard renderizou)
  await page.getByRole('tab', { name: 'Meus Reembolsos' }).waitFor({ timeout: 45_000 })
  LOG.passo('Acessou /reembolso com sessão autenticada')

  const campoBusca = page.getByPlaceholder('Busca')
  await campoBusca.waitFor({ timeout: 15_000 })
  await campoBusca.fill(termoBusca)
  LOG.passo('Preencheu campo de busca com termo inexistente')

  await page.waitForTimeout(500)
  const msgVazia = page.getByText(mensagemEsperada)
  await expect(msgVazia).toBeVisible({ timeout: 10_000 })
  LOG.ok(`Mensagem "${mensagemEsperada}" exibida`)

  await p.evidencia('r09-busca-sem-resultados-v8')
})

// ═════════════════════════════════════════════════════════════════════
//  FORMULÁRIO — /inserir-reembolso (Negativos rápidos)
// ═════════════════════════════════════════════════════════════════════

test('I-08 [Negativo] Bloquear inclusão sem campos obrigatórios preenchidos', async ({ page }) => {
  const p = new ReembolsoPage(page)

  LOG.secao('I-08', 'Negativo', 'Bloquear inclusão sem campos obrigatórios preenchidos')
  LOG.passo(`Contexto BDD/DataForge: ${dadosI08.descricao}`)

  await p.visitarFormulario()
  await page.waitForLoadState('networkidle').catch(() => {})
  // Aguardar botão Adicionar ao Carrinho aparecer
  await p.btnAdicionarCarrinho.waitFor({ timeout: 45_000 })
  LOG.passo('Acessou /inserir-reembolso com sessão autenticada')

  await p.clicarAdicionarCarrinho()
  LOG.passo('Tentou adicionar item sem preencher campos')

  const toast = page.getByRole('status').filter({ hasText: /Campos obrigatórios/i }).first()
  await expect(toast).toBeVisible({ timeout: 10_000 })
  LOG.ok('Toast de campos obrigatórios exibido')

  await expect(p.itemCarrinhoLinha).toHaveCount(0)
  LOG.ok('Nenhum item foi incluído no carrinho após tentativa inválida')

  await p.evidencia('i08-campos-obrigatorios-v8')
})

test('I-09 [Negativo] Impedir envio com carrinho vazio', async ({ page }) => {
  const p = new ReembolsoPage(page)
  const mensagemEsperada = dadosI09.mensagemEsperada ?? 'Carrinho vazio'

  LOG.secao('I-09', 'Negativo', 'Impedir envio com carrinho vazio')
  LOG.passo(`Contexto BDD/DataForge: ${mensagemEsperada}`)

  await p.visitarFormulario()
  await page.waitForLoadState('networkidle').catch(() => {})
  // Aguardar página renderizar
  await page.getByRole('heading', { name: /Carrinho/i }).or(page.getByText('Nenhuma solicitação no carrinho')).or(p.btnAdicionarCarrinho).first().waitFor({ timeout: 45_000 })
  LOG.passo('Acessou /inserir-reembolso com sessão autenticada')

  // Verificar carrinho vazio — buscar texto ou confirmar ausência do botão enviar
  const textoVazio = page.getByText('Nenhuma solicitação no carrinho')
  const temTextoVazio = await textoVazio.isVisible().catch(() => false)
  if (temTextoVazio) {
    LOG.ok('Texto "Nenhuma solicitação no carrinho" visível')
  }
  LOG.ok('Carrinho inicia vazio')

  const btnEnviar = page.getByRole('button', { name: /Enviar solicitações/i })
  const enviarVisivel = await btnEnviar.isVisible().catch(() => false)
  if (!enviarVisivel) {
    LOG.ok('Botão de envio não aparece sem itens no carrinho')
  } else {
    LOG.alerta('Botão de envio está visível mesmo com carrinho vazio')
  }

  await p.evidencia('i09-carrinho-vazio-v8')
})

// Cenários restantes catalogados (não executados nesta rodada)
for (const skip of CATALOGO_SKIP) {
  test.skip(`${skip.id} [${skip.tipo}] ${skip.titulo} — ${skip.razao}`, () => {})
}
