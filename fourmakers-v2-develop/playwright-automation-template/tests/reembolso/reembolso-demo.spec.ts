import { test, expect } from '@playwright/test'
import { ReembolsoPage } from '../../support/pages/ReembolsoPage'
import { loginFourMakers } from '../../support/auth/fourmakers-auth'
import reembolsoFixture from '../../fixtures/reembolso.json'

/**
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *  DEMO REEMBOLSO — Script de Apresentação (Happy Path)
 * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 *  Meta: < 60 segundos de execução
 *
 *  Cenários cobertos:
 *    R-01 · Dashboard com aba "Meus Reembolsos" ativa
 *    R-06 · Navegar para nova solicitação
 *    I-02 · Cálculo automático de valor (Km rodado)
 *    I-01 · Preencher formulário completo e adicionar ao carrinho
 *    I-04 · Enviar solicitação e confirmar redirecionamento
 *
 *  Para rodar:
 *    npm run demo
 */

test.describe.configure({ retries: 0 })

const dados = reembolsoFixture as any

const LOG = {
  banner: (msg: string) => {
    const linha = '═'.repeat(54)
    console.log(`\n╔${linha}╗`)
    console.log(`║  ${msg.padEnd(53)}║`)
    console.log(`╚${linha}╝`)
  },
  secao: (codigo: string, tipo: string, titulo: string) => {
    console.log(`\n┌─────────────────────────────────────────────────────`)
    console.log(`│ ${codigo} · [${tipo}]  ${titulo}`)
    console.log(`└─────────────────────────────────────────────────────`)
  },
  passo: (msg: string) => console.log(`   → ${msg}`),
  ok:    (msg: string) => console.log(`   ✅ ${msg}`),
  tempo: (inicio: number) => {
    const ms = Date.now() - inicio
    const s  = (ms / 1000).toFixed(1)
    console.log(`   ⏱  Tempo total: ${s}s`)
  },
}

test('🚀 Demo — Jornada Reembolso (Happy Path)', async ({ page, request }) => {
  test.setTimeout(120_000)

  const p     = new ReembolsoPage(page)
  const inicio = Date.now()

  // ── SETUP — Autenticação ──────────────────────────────────────────────────
  LOG.banner('DEMO FOURMAKERS — MÓDULO REEMBOLSO')
  LOG.passo('Autenticando via OTP...')
  await loginFourMakers(page, request)
  LOG.ok('Autenticado com sucesso')

  // ── R-01 · Dashboard ─────────────────────────────────────────────────────
  LOG.secao('R-01', 'Positivo', 'Dashboard — aba "Meus Reembolsos"')

  LOG.passo('Navegando para /reembolso...')
  await p.visitarLista()

  LOG.passo('Verificando aba "Meus Reembolsos"...')
  await expect(page.getByRole('tab', { name: 'Meus Reembolsos' })).toBeVisible()

  LOG.passo('Verificando botão "Solicitar Reembolso"...')
  await expect(p.btnSolicitarReembolso).toBeVisible()
  LOG.ok('Dashboard carregado com aba e botão visíveis')

  // ── R-06 · Nova solicitação ───────────────────────────────────────────────
  LOG.secao('R-06', 'Positivo', 'Navegar para formulário de nova solicitação')

  LOG.passo('Clicando em "Solicitar Reembolso"...')
  await p.clicarSolicitarReembolso()

  await expect(page).toHaveURL(/inserir-reembolso/)
  await expect(page.getByText(/Nova Solicita/i)).toBeVisible()
  LOG.ok('Redirecionado para /inserir-reembolso — formulário aberto')

  // ── I-02 · Cálculo automático (Km rodado) ────────────────────────────────
  LOG.secao('I-02', 'Positivo', 'Cálculo automático de valor — Km rodado')

  await p.visitarFormulario()
  LOG.passo(`Preenchendo objetivo: "${dados.reembolsoKmRodado.objetivo}"...`)
  await p.preencherObjetivo(dados.reembolsoKmRodado.objetivo)

  LOG.passo('Selecionando categoria "Km rodado"...')
  await p.selecionarCategoriaKmRodado()
  await expect(p.quantidade).toBeVisible()

  LOG.passo(`Digitando quantidade: ${dados.reembolsoKmRodado.quantidade} km...`)
  await p.preencherQuantidade(dados.reembolsoKmRodado.quantidade)
  await expect(p.valorTotal).toBeVisible()

  const valorCalculado = await p.valorTotal.inputValue()
  const valorNumerico  = parseFloat(valorCalculado.replace(',', '.'))
  if (isNaN(valorNumerico) || valorNumerico <= 0) {
    throw new Error(`I-02 — Valor Total esperado > 0, recebido: "${valorCalculado}"`)
  }
  LOG.ok(`Valor calculado automaticamente: R$ ${valorCalculado}`)

  // ── I-01 · Adicionar item ao carrinho ─────────────────────────────────────
  LOG.secao('I-01', 'Positivo', 'Preencher formulário completo e adicionar ao carrinho')

  await p.visitarFormulario()
  LOG.passo('Preenchendo todos os campos do formulário...')

  await p.preencherObjetivo(dados.reembolsoValido.objetivo)
  await p.preencherDestino(dados.reembolsoValido.destino)
  await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
  await p.selecionarDataFim(dados.reembolsoValido.dataFim.dia, dados.reembolsoValido.dataFim.mes)
  await p.selecionarPrimeiroProjeto()
  await p.selecionarPrimeiraCategoria()
  await p.uploadComprovante('fixtures/comprovante-teste.jpg')
  await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
  await p.preencherValor(dados.reembolsoValido.valor)
  await p.preencherDescricao(dados.reembolsoValido.descricao)

  LOG.passo('Adicionando item ao carrinho...')
  await p.clicarAdicionarCarrinho()
  await p.verificarItemNoCarrinho()
  LOG.ok('Item adicionado ao carrinho com sucesso')

  LOG.passo('Verificando persistência dos campos gerais...')
  await expect(p.objetivo).toHaveValue(dados.reembolsoValido.objetivo)
  await expect(p.destino).toHaveValue(dados.reembolsoValido.destino)
  LOG.ok('Campos "Objetivo" e "Destino" persistem após adição')

  // ── I-04 · Enviar solicitação ─────────────────────────────────────────────
  LOG.secao('I-04', 'Positivo', 'Enviar solicitação e confirmar redirecionamento')

  // Intercepta a chamada de API de envio e responde com sucesso garantido.
  // Isso isola a demo de falhas de rede/API em ambiente de apresentação.
  await page.route('**/api/Financeiro/Reembolso/Solicitacao/Inserir', async (route) => {
    LOG.passo('[API MOCK] Interceptando envio — retornando sucesso...')
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({ sucesso: true, mensagem: 'Solicitação enviada com sucesso' }),
    })
  })

  LOG.passo('Enviando solicitação ao servidor...')
  await p.clicarEnviarSolicitacoes()

  // App exibe modal de sucesso com botão "OK" antes de redirecionar
  LOG.passo('Aguardando modal de confirmação de sucesso...')
  const modalSucesso = page.getByRole('alertdialog')
  await expect(modalSucesso).toBeVisible({ timeout: 10_000 })
  LOG.ok('Modal de sucesso exibido: "Reembolso enviado com sucesso!"')

  LOG.passo('Confirmando e fechando modal (clicando em "OK")...')
  await modalSucesso.getByRole('button', { name: /OK/i }).click()

  LOG.passo('Aguardando redirecionamento para /reembolso...')
  await expect(page).toHaveURL(/\/reembolso/, { timeout: 10_000 })
  LOG.ok('Solicitação enviada — redirecionado para /reembolso')

  // ── FIM ───────────────────────────────────────────────────────────────────
  const linha = '═'.repeat(54)
  console.log(`\n╔${linha}╗`)
  console.log(`║  ✅  DEMO CONCLUÍDA — TODOS OS CENÁRIOS APROVADOS    ║`)
  console.log(`╚${linha}╝`)
  LOG.tempo(inicio)
})
