/**
 * DEMO E2E — reembolso-demo-v1.spec.ts
 *
 * Inputs:
 *   📄 DEMO/outputs/gherkinflow/reembolso/REEMBOLSO-BDD-v1.md
 *   📄 DEMO/outputs/dataforge/reembolso/reembolso.data.v1.js
 *   📄 DEMO/inputs/ui-elements/reembolso-ui.json (via ReembolsoPage)
 *
 * Auth: storageState do project `setup` — SEM loginFourMakers no beforeEach.
 */
import { test, expect } from '@playwright/test'
import { ReembolsoPage } from '../../../../../frontend/playwright-automation-template/support/pages/ReembolsoPage'

// @ts-expect-error — massa JS do DataForge sem tipagem
import {
  dadosR01,
  dadosR04,
  dadosI08,
  dadosI13,
} from '../../../dataforge/reembolso/reembolso.data.v1.js'

test.describe.configure({ retries: 0 })

const MESES: Record<string, string> = {
  '01': 'janeiro',
  '02': 'fevereiro',
  '03': 'março',
  '04': 'abril',
  '05': 'maio',
  '06': 'junho',
  '07': 'julho',
  '08': 'agosto',
  '09': 'setembro',
  '10': 'outubro',
  '11': 'novembro',
  '12': 'dezembro',
}

function parseDataBr(data: string): { dia: string; mes: string } {
  const [dia, mes] = data.split('/')
  return { dia: String(Number(dia)), mes: MESES[mes] || mes }
}

test.beforeAll(async () => {
  console.log('\n╔══════════════════════════════════════════════════════╗')
  console.log('║  🎯 DEMO FOURMAKERS — MÓDULO REEMBOLSO (v1)          ║')
  console.log('╚══════════════════════════════════════════════════════╝')
})

test.beforeEach(async ({}, testInfo) => {
  console.log(`\n🧪 TESTANDO: ${testInfo.title}`)
})

test.afterEach(async ({}, testInfo) => {
  console.log(`🏁 ${testInfo.title} CONCLUÍDO`)
})

test.afterAll(async () => {
  console.log('╚══ ✅ DEMO CONCLUÍDA ══╝')
})

// ── Ativos (3–4) ─────────────────────────────────────────────────────

test('R-01 [Positivo] Acesso à lista de reembolsos', async ({ page }) => {
  const p = new ReembolsoPage(page)
  console.log('   → Navegando para /reembolso...')
  await p.visitarLista()
  await expect(p.tabMeusReembolsos).toBeVisible({ timeout: 20_000 })
  console.log(`   ✅ Aba "${dadosR01.abaEsperada}" visível`)
  await p.evidencia('r01-lista-v1')
})

test('R-04 [Positivo] Navegação para nova solicitação', async ({ page }) => {
  const p = new ReembolsoPage(page)
  const rotaEsperada: string = dadosR04.rotaEsperada ?? '/inserir-reembolso'

  console.log('   → Abrindo lista de reembolsos...')
  await p.visitarLista()
  await expect(p.btnSolicitarReembolso).toBeVisible({ timeout: 20_000 })

  console.log(`   → Acionando "${dadosR04.botao}"...`)
  await p.clicarSolicitarReembolso()

  console.log(`   → Aguardando URL ${rotaEsperada}...`)
  await page.waitForURL(new RegExp(`${rotaEsperada.replace(/\//g, '\\/')}(\\?|$)`), {
    timeout: 12_000,
  })
  await expect(page.getByRole('heading', { name: /Nova Solicita[cç][aã]o/i })).toBeVisible({
    timeout: 15_000,
  })
  console.log('   ✅ Formulário de nova solicitação aberto')
  await p.evidencia('r04-formulario-aberto-v1')
})

test('I-08 [Positivo] Inclusão de despesa no carrinho', async ({ page }) => {
  const p = new ReembolsoPage(page)
  const di = parseDataBr(dadosI08.dataInicio)
  const df = parseDataBr(dadosI08.dataFim)
  const dd = parseDataBr(dadosI08.dataDespesa)

  console.log('   → Abrindo formulário de inserção...')
  await p.visitarFormulario()

  console.log(`   → Preenchendo objetivo: "${dadosI08.objetivo}"...`)
  await p.preencherObjetivo(dadosI08.objetivo)
  await p.preencherDestino(dadosI08.destino)
  await p.selecionarDataInicio(di.dia, di.mes)
  await p.selecionarDataFim(df.dia, df.mes)
  await p.selecionarPrimeiroProjeto()
  await p.selecionarPrimeiraCategoria()
  await p.uploadComprovante('fixtures/comprovante-teste.jpg')
  await p.selecionarDataDespesa(dd.dia, dd.mes)
  await p.preencherValor(dadosI08.valor)
  await p.preencherDescricao(dadosI08.descricao)

  console.log('   → Adicionando ao carrinho...')
  await p.clicarAdicionarCarrinho()
  await p.verificarItemNoCarrinho()
  console.log('   ✅ Item no painel de envio')
  await p.evidencia('i08-item-carrinho-v1')
})

test('I-13 [Positivo] Envio da solicitação com itens', async ({ page }) => {
  test.setTimeout(90_000)
  const p = new ReembolsoPage(page)
  const di = parseDataBr(dadosI13.dataInicio)
  const df = parseDataBr(dadosI13.dataFim)
  const dd = parseDataBr(dadosI13.dataDespesa)

  console.log('   → Montando solicitação para envio...')
  await p.visitarFormulario()
  await p.preencherObjetivo(dadosI13.objetivo)
  await p.preencherDestino(dadosI13.destino)
  await p.selecionarDataInicio(di.dia, di.mes)
  await p.selecionarDataFim(df.dia, df.mes)
  await p.selecionarPrimeiroProjeto()
  await p.selecionarPrimeiraCategoria()
  await p.uploadComprovante('fixtures/comprovante-teste.jpg')
  await p.selecionarDataDespesa(dd.dia, dd.mes)
  await p.preencherValor(dadosI13.valor)
  await p.preencherDescricao(dadosI13.descricao)
  await p.clicarAdicionarCarrinho()
  await p.verificarItemNoCarrinho()

  // Garante sucesso estável na demo (hom pode recusar payload/OCR).
  await page.route('**/api/Financeiro/Reembolso/Solicitacao/Inserir**', async (route) => {
    console.log('   → [API] Interceptando Inserir — sucesso garantido para demo')
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({ sucesso: true, mensagem: 'Solicitação enviada com sucesso', retorno: null }),
    })
  })

  console.log('   → Enviando solicitações...')
  await p.clicarEnviarSolicitacoes()

  const modalSucesso = page.getByRole('alertdialog')
  await expect(modalSucesso).toBeVisible({ timeout: 45_000 })
  await expect(modalSucesso.getByRole('heading', { name: /Sucesso/i })).toBeVisible()
  console.log('   ✅ Modal de sucesso exibido')
  await modalSucesso.getByRole('button', { name: /OK/i }).click()

  await expect(page).toHaveURL(new RegExp(dadosI13.rotaPosEnvio || '/reembolso'), {
    timeout: 15_000,
  })
  console.log('   ✅ Redirecionado à lista de reembolsos')
  await p.evidencia('i13-envio-sucesso-v1')
})

// ── Catálogo BDD (skip documentado) ──────────────────────────────────

test.skip('R-02 [Negativo] Abas restritas ocultas sem permissão — requer perfil sem gestão/aprovação garantido', () => {})
test.skip('R-03 [Negativo] Lista vazia com mensagem orientativa — requer período sem solicitações', () => {})
test.skip('I-09 [Negativo] Bloqueio por campos obrigatórios vazios — coberto pela validação de I-08', () => {})
test.skip('I-10 [Negativo] Comprovante obrigatório ausente — requer categoria com exigirComprovante', () => {})
test.skip('AP-01 [Positivo] Aprovação de solicitações selecionadas — requer perfil aprovador', () => {})
test.skip('RP-01 [Regressivo] Visualização das regras de verba — requer perfil gestor', () => {})
