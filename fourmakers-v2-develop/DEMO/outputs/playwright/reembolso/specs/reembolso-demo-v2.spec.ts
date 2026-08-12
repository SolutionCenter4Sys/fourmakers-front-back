/**
 * Playwright — Demo Reembolso v2
 *
 * Artefatos consumidos:
 *   📄 DEMO/outputs/gherkinflow/reembolso/REEMBOLSO-BDD-v2.md
 *   📄 DEMO/outputs/dataforge/reembolso/reembolso.data.v2.js
 *   📄 DEMO/inputs/ui-elements/reembolso-ui.json → ReembolsoPage.ts
 *
 * Auth: storageState (project setup) — sem loginFourMakers no beforeEach.
 * Executar: node scripts/run-versioned-demo.cjs --headed
 */

import { test, expect } from '@playwright/test'
import { ReembolsoPage } from '../../../../../frontend/playwright-automation-template/support/pages/ReembolsoPage'
// @ts-expect-error — massa DataForge JS sem tipagem
import {
  dadosR01,
  dadosR04,
  dadosI08,
  dadosI13,
} from '../../../dataforge/reembolso/reembolso.data.v2.js'

test.describe.configure({ retries: 0 })

const MESES = [
  'janeiro', 'fevereiro', 'março', 'abril', 'maio', 'junho',
  'julho', 'agosto', 'setembro', 'outubro', 'novembro', 'dezembro',
]

function parseBrDate(data: string): { dia: string; mes: string } {
  const [dia, mesNum] = data.split('/')
  return { dia: String(Number(dia)), mes: MESES[Number(mesNum) - 1] }
}

test.beforeAll(async () => {
  console.log('\n╔══════════════════════════════════════════════════════╗')
  console.log('║  🎯 AUTOMAÇÃO INICIADA — DEMO REEMBOLSO v2           ║')
  console.log('╚══════════════════════════════════════════════════════╝')
})

test.beforeEach(async ({}, testInfo) => {
  console.log(`\n🧪 TESTANDO: ${testInfo.title}`)
})

test.afterEach(async ({}, testInfo) => {
  console.log(`🏁 ${testInfo.title.split(' ')[0]} CONCLUÍDO`)
})

test.afterAll(async () => {
  console.log('\n╚══ ✅ DEMO CONCLUÍDA ══╝\n')
})

// ── Ativos (demo enxuta) ─────────────────────────────────────────────

test('R-01 [Positivo] Acesso ao dashboard de reembolsos', async ({ page }) => {
  const p = new ReembolsoPage(page)
  console.log('   → Navegando para /reembolso...')
  await p.visitarLista()
  await expect(p.tabMeusReembolsos).toBeVisible({ timeout: 20_000 })
  console.log(`   ✅ Aba "${dadosR01.abaEsperada}" visível`)
  await expect(page).toHaveURL(new RegExp(dadosR01.rotaEsperada.replace(/\//g, '\\/')))
  await p.evidencia('r01-dashboard-v2')
})

test('R-04 [Positivo] Navegação para nova solicitação', async ({ page }) => {
  const p = new ReembolsoPage(page)
  const rotaEsperada: string = dadosR04.rotaEsperada ?? '/inserir-reembolso'

  console.log('   → Abrindo lista de reembolsos...')
  await p.visitarLista()
  await expect(p.btnSolicitarReembolso).toBeVisible({ timeout: 15_000 })

  console.log(`   → Acionando "${dadosR04.botao}"...`)
  await p.clicarSolicitarReembolso()

  console.log(`   → Aguardando URL ${rotaEsperada}...`)
  await page.waitForURL(new RegExp(`${rotaEsperada.replace(/\//g, '\\/')}(\\?|$)`), {
    timeout: 12_000,
  })
  await expect(page.getByRole('heading', { name: /Nova Solicita[cç][aã]o/i })).toBeVisible()
  console.log('   ✅ Formulário de nova solicitação aberto')
  await p.evidencia('r04-formulario-aberto-v2')
})

test('I-08 [Positivo] Inclusão de item no carrinho com comprovante', async ({ page }) => {
  test.setTimeout(90_000)
  const p = new ReembolsoPage(page)
  const ini = parseBrDate(dadosI08.dataInicio)
  const fim = parseBrDate(dadosI08.dataFim)
  const desp = parseBrDate(dadosI08.dataDespesa)

  console.log('   → Abrindo formulário de inserção...')
  await p.visitarFormulario()
  await page.waitForLoadState('domcontentloaded')

  console.log('   → Preenchendo campos gerais + item...')
  await p.preencherCamposGerais({
    objetivo: dadosI08.objetivo,
    destino: dadosI08.destino,
    dataInicio: ini,
    dataFim: fim,
  })
  await p.selecionarPrimeiroProjeto()
  await p.selecionarPrimeiraCategoria()
  await p.uploadComprovante('fixtures/comprovante-teste.jpg')
  await p.selecionarDataDespesa(desp.dia, desp.mes)
  await p.preencherValor(dadosI08.valor)
  await p.preencherDescricao(dadosI08.descricao)

  console.log('   → Adicionando ao carrinho...')
  await p.clicarAdicionarCarrinho()
  await p.verificarItemNoCarrinho()
  console.log(`   ✅ Item no carrinho — valor ${dadosI08.valor} | ${dadosI08.categoria}`)
  await p.evidencia('i08-item-carrinho-v2')
})

test('I-13 [Positivo] Envio da solicitação com itens no carrinho', async ({ page }) => {
  test.setTimeout(120_000)
  const p = new ReembolsoPage(page)
  const ini = parseBrDate(dadosI13.dataInicio)
  const fim = parseBrDate(dadosI13.dataFim)
  const desp = parseBrDate(dadosI13.dataDespesa)

  console.log('   → Preparando solicitação completa...')
  await p.visitarFormulario()
  await page.waitForLoadState('domcontentloaded')
  await p.preencherCamposGerais({
    objetivo: dadosI13.objetivo,
    destino: dadosI13.destino,
    dataInicio: ini,
    dataFim: fim,
  })
  await p.selecionarPrimeiroProjeto()
  await p.selecionarPrimeiraCategoria()
  await p.uploadComprovante('fixtures/comprovante-teste.jpg')
  await p.selecionarDataDespesa(desp.dia, desp.mes)
  await p.preencherValor(dadosI13.valor)
  await p.preencherDescricao(dadosI13.descricao)
  await p.clicarAdicionarCarrinho()
  await p.verificarItemNoCarrinho()

  await page.route('**/api/Financeiro/Reembolso/Solicitacao/Inserir', async (route) => {
    console.log('   → [API] Interceptando Inserir — sucesso controlado para demo...')
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({ sucesso: true, mensagem: 'Solicitação enviada com sucesso' }),
    })
  })

  console.log('   → Enviando solicitações...')
  await p.clicarEnviarSolicitacoes()

  const modalSucesso = page.getByRole('alertdialog')
  await expect(modalSucesso).toBeVisible({ timeout: 20_000 })
  await expect(modalSucesso.getByRole('heading', { name: /Sucesso/i })).toBeVisible()
  console.log('   ✅ Modal de sucesso exibido')
  await modalSucesso.getByRole('button', { name: /OK/i }).click()
  await expect(page).toHaveURL(/\/reembolso/, { timeout: 10_000 })
  console.log('   ✅ Redirecionado para acompanhamento em /reembolso')
  await p.evidencia('i13-envio-sucesso-v2')
})

// ── Catálogo BDD (não executáveis no volume enxuto / dependem de perfil) ──

test.skip('R-02 [Negativo] Aba de gestão restrita sem perfil de gestor — requer colaborador sem permissão de gestão', () => {})
test.skip('I-09 [Negativo] Categoria com comprovante obrigatório sem anexo — coberto indiretamente por I-08 com anexo', () => {})
test.skip('I-14 [Regressivo] Leitura automática de dados do comprovante — depende de OCR estável no HML', () => {})
test.skip('AP-01 [Positivo] Aprovação de solicitações pendentes — requer perfil aprovador + massa pendente', () => {})
test.skip('AP-02 [Negativo] Reprovação sem justificativa — requer perfil aprovador', () => {})
test.skip('RP-01 [Regressivo] Consulta das regras e verbas parametrizadas — requer perfil gestor', () => {})
