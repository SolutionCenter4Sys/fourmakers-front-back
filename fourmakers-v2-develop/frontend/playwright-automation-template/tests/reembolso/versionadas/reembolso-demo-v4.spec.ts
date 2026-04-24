import { test, expect } from '@playwright/test'
import { ReembolsoPage } from '../../../support/pages/ReembolsoPage'
import { loginSolutionCenter } from '../../../support/auth/fourmakers-auth'

// ─── Dados do DataForge (Etapa 2) ───────────────────────────────────────
// Cada constante corresponde 1:1 aos cenários do BDD REEMBOLSO-BDD-v4.md.
// @ts-ignore — import JS sem types
import {
  dadosG01, dadosG02, dadosG03, dadosG04, dadosG05,
  dadosG06, dadosG07, dadosG08, dadosG09, dadosG10,
  dadosG11, dadosG12, dadosG13, dadosG14, dadosG15,
  dadosG16, dadosG17,
// @ts-ignore
} from '../../../../DEMO/automacao/reembolso/versionadas/reembolso.data.v4.js'

test.describe.configure({ retries: 0, timeout: 120_000 })

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
  { id: 'G-01', tipo: 'Positivo',   titulo: 'Filtro por período, cliente e status',         razao: 'requer base de gestão com dados e perfil gestor' },
  { id: 'G-02', tipo: 'Positivo',   titulo: 'Carregamento de projetos por cliente',         razao: 'requer lista de clientes/projetos disponíveis' },
  { id: 'G-03', tipo: 'Positivo',   titulo: 'Busca textual de solicitações',               razao: 'requer colaboradores carregados' },
  { id: 'G-04', tipo: 'Positivo',   titulo: 'Visão de indicadores da gestão',              razao: 'requer dados consolidados de gestão' },
  { id: 'G-05', tipo: 'Positivo',   titulo: 'Distribuição de status por colaborador',      razao: 'requer itens com status variados' },
  { id: 'G-06', tipo: 'Positivo',   titulo: 'Consulta detalhada de solicitação',           razao: 'requer solicitação aprovada cadastrada' },
  { id: 'G-07', tipo: 'Positivo',   titulo: 'Visualização de comprovantes anexados',       razao: 'requer documentos anexados' },
  { id: 'G-08', tipo: 'Positivo',   titulo: 'Habilitação do modo Baixa',                   razao: 'requer itens aprovados' },
  { id: 'G-09', tipo: 'Positivo',   titulo: 'Seleção em massa de itens aprovados',         razao: 'requer itens aprovados para baixa' },
  { id: 'G-10', tipo: 'Positivo',   titulo: 'Confirmação de baixas com sucesso',           razao: 'requer API liberando baixa para gestor' },
  { id: 'G-11', tipo: 'Positivo',   titulo: 'Paginação e tamanho da página',               razao: 'requer volume de colaboradores > page size' },
  { id: 'G-13', tipo: 'Negativo',   titulo: 'Solicitação sem documentos anexados',         razao: 'requer item sem anexos' },
  { id: 'G-14', tipo: 'Negativo',   titulo: 'Ausência de itens aprovados para baixa',      razao: 'requer lista sem itens aprovados' },
  { id: 'G-15', tipo: 'Regressivo', titulo: 'Limpeza completa dos filtros',                razao: 'requer filtros aplicados + dataset' },
  { id: 'G-16', tipo: 'Regressivo', titulo: 'Reset ao fechar modal de detalhes',           razao: 'requer modal de detalhes com itens aprovados' },
  { id: 'G-17', tipo: 'Regressivo', titulo: 'Busca reinicia paginação',                    razao: 'requer tabela com múltiplas páginas' },
]

const STATUS_ICON: Record<string, string> = {
  passed:      '✅',
  failed:      '❌',
  timedOut:    '⏱️',
  interrupted: '🛑',
  skipped:     '⏭️',
}

function formatarDuracao(ms: number): string {
  return ms < 1000 ? `${ms}ms` : `${(ms / 1000).toFixed(1)}s`
}

const ROTEIRO_EXECUCAO = [
  { id: 'G-12', tipo: 'Negativo', titulo: 'Busca sem correspondência' },
]

let demoInicioMs = 0

test.beforeAll(async () => {
  demoInicioMs = Date.now()
  LOG.banner('🎬 DEMO FOURMAKERS — GESTÃO ADM REEMBOLSO (v4)')
  console.log('📄 BDD     : frontend/DEMO/cenarios-bdd/REEMBOLSO-BDD-v4.md')
  console.log('📄 Massa   : frontend/DEMO/automacao/reembolso/versionadas/reembolso.data.v4.js')
  console.log('📄 UI JSON : frontend/DEMO/ui-elements/dom-elements-2026-04-08.json')
  console.log('   Ambiente: front local http://localhost:8080 → backend dev spw.app.foursys.com/backoffice-rf-hom')
  console.log('   Usuário : solutioncenter@foursys.com.br (orgId 8) — login OTP real\n')

  console.log('📋 Roteiro desta execução (cenários tentados)')
  for (const r of ROTEIRO_EXECUCAO) {
    console.log(`  • ${r.id} [${r.tipo}] ${r.titulo}`)
  }
})

test.beforeEach(async ({ page, request }, testInfo) => {
  console.log(`\n🧪 TESTANDO: ${testInfo.title}`)
  await loginSolutionCenter(page, request)
})

test.afterEach(async ({}, testInfo) => {
  const match = testInfo.title.match(/^(G-\d+)\s+\[([^\]]+)\]\s+(.+)$/)
  if (match) {
    const [, id, tipo, titulo] = match
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
  console.log(`║  🎬 DEMO GESTÃO ADM v4 — RELATÓRIO FINAL                   ║`)
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

  console.log(`\n╚══ ✅ DEMO FINALIZADA — ${passados}/${executados.length} cenários executados ══╝\n`)
})

// ═════════════════════════════════════════════════════════════════════
//  GESTÃO ADM — Dashboard /reembolso?tab=gestao-adm
// ═════════════════════════════════════════════════════════════════════

test('G-12 [Negativo] Busca sem correspondência', async ({ page }) => {
  const p = new ReembolsoPage(page)
  LOG.secao('G-12', 'Negativo', 'Busca sem correspondência')

  const temGestao = await p.abrirGestaoAdm()
  if (!temGestao) {
    test.skip('Usuário sem perfil gestor: aba Gestão ADM não está disponível')
  }

  LOG.passo(`Aplicando termo de busca inexistente: ${dadosG12.termoBusca}`)
  await expect(p.buscaGestao).toBeVisible()
  await p.buscaGestao.fill(dadosG12.termoBusca)
  await expect(p.mensagemNenhumColaborador).toBeVisible()
  LOG.ok(`Mensagem esperada visível: ${dadosG12.mensagemEsperada}`)
  await p.evidencia('g12-busca-sem-correspondencia', 'gestao-adm')
})

// Cenários restantes catalogados como skip por dependências de ambiente/dados
for (const skip of CATALOGO_SKIP) {
  test.skip(`${skip.id} [${skip.tipo}] ${skip.titulo} — ${skip.razao}`, () => {})
}
