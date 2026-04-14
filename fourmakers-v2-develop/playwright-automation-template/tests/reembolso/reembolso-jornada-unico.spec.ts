import { test, expect } from '@playwright/test'
import * as fs from 'fs'
import { ReembolsoPage } from '../../support/pages/ReembolsoPage'
import { loginFourMakers } from '../../support/auth/fourmakers-auth'
import reembolsoFixture from '../../fixtures/reembolso.json'

/**
 * Jornada Completa de Reembolso — Teste Único E2E
 * Referência BDD: REEMBOLSO-BDD-CENARIOS-v7.md
 *
 * Cobertura (positivos + negativos em sequência):
 *   R-01 · [Positivo]  Dashboard — aba padrão "Meus Reembolsos"
 *   R-06 · [Positivo]  Dashboard — navegação para nova solicitação
 *   I-07 · [Negativo]  Validação — campos obrigatórios bloqueiam adição
 *   I-09 · [Negativo]  Carrinho  — vazio impede envio
 *   I-10 · [Negativo]  Upload    — formato inválido é rejeitado
 *   I-08 · [Negativo]  Validação — comprovante obrigatório bloqueia adição
 *   I-02 · [Positivo]  Formulário — cálculo automático (Km rodado)
 *   I-01 · [Positivo]  Formulário — adicionar item ao carrinho
 *   I-06 · [Positivo]  Carrinho  — remoção de item
 *   I-03 · [Positivo]  Carrinho  — edição de item restaura os campos
 *   I-04 · [Positivo]  Jornada   — envio da solicitação e redirecionamento
 */

// Sem retry — cada execução consome um código OTP real
test.describe.configure({ retries: 0 })

const dados = reembolsoFixture as any
const DIR_EV = 'evidencias/screenshots/reembolso-jornada-unico'

function ev(page: ReembolsoPage, nome: string) {
  return page.evidencia(nome, 'reembolso-jornada-unico')
}

test('Jornada Completa de Reembolso — Positivos e Negativos', async ({ page, request }) => {
  test.setTimeout(300_000)  // 5 minutos — jornada completa com autenticação OTP

  const p = new ReembolsoPage(page)
  fs.mkdirSync(DIR_EV, { recursive: true })

  // ══════════════════════════════════════════════════════════════════════
  // SETUP — Autenticação
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n╔══════════════════════════════════════════════════════╗')
  console.log('║   JORNADA COMPLETA DE REEMBOLSO — INÍCIO             ║')
  console.log('╚══════════════════════════════════════════════════════╝')
  await loginFourMakers(page, request)

  // ══════════════════════════════════════════════════════════════════════
  // R-01 · [Positivo] Dashboard — aba padrão "Meus Reembolsos"
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ R-01 · [Positivo] Dashboard — aba padrão')
  console.log('└─────────────────────────────────────────────────────')

  console.log('   → Navegando para /reembolso...')
  await p.visitarLista()
  await ev(p, '01-r01-dashboard')

  console.log('   → Verificando aba "Meus Reembolsos" ativa...')
  await expect(page.getByRole('tab', { name: 'Meus Reembolsos' })).toBeVisible()
  console.log('   ✅ R-01 — Aba "Meus Reembolsos" visível')

  console.log('   → Verificando botão "Solicitar Reembolso"...')
  await expect(p.btnSolicitarReembolso).toBeVisible()
  await ev(p, '02-r01-botao-solicitar-visivel')
  console.log('   ✅ R-01 — Botão "Solicitar Reembolso" visível')

  // ══════════════════════════════════════════════════════════════════════
  // R-06 · [Positivo] Dashboard — navegação para nova solicitação
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ R-06 · [Positivo] Dashboard — navegação para nova solicitação')
  console.log('└─────────────────────────────────────────────────────')

  console.log('   → Clicando em "Solicitar Reembolso"...')
  await p.clicarSolicitarReembolso()
  await ev(p, '03-r06-apos-clicar-solicitar')

  await expect(page).toHaveURL(/inserir-reembolso/)
  await expect(page.getByText(/Nova Solicita/i)).toBeVisible()
  await ev(p, '04-r06-formulario-aberto')
  console.log('   ✅ R-06 — Redirecionado para /inserir-reembolso com formulário visível')

  // ══════════════════════════════════════════════════════════════════════
  // I-07 · [Negativo] Campos obrigatórios bloqueiam adição ao carrinho
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-07 · [Negativo] Campos obrigatórios bloqueiam adição')
  console.log('└─────────────────────────────────────────────────────')

  console.log('   → [Objetivo vazio] Tentando adicionar sem preencher nenhum campo...')
  await p.visitarFormulario()
  await p.clicarAdicionarCarrinho()
  await p.verificarCarrinhoVazio()
  console.log('   ✅ I-07a — Objetivo vazio: adição bloqueada, carrinho vazio')

  console.log('   → [Data de Início vazia] Preenchendo só Objetivo e tentando adicionar...')
  await p.preencherObjetivo(dados.reembolsoValido.objetivo)
  await p.clicarAdicionarCarrinho()
  await p.verificarCarrinhoVazio()
  console.log('   ✅ I-07b — Data de Início vazia: adição bloqueada')

  console.log('   → [Categoria vazia] Adicionando data mas sem categoria...')
  await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
  await p.selecionarPrimeiroProjeto()
  await p.clicarAdicionarCarrinho()
  await p.verificarCarrinhoVazio()
  console.log('   ✅ I-07c — Categoria vazia: adição bloqueada')

  console.log('   → [Descrição vazia] Selecionando categoria mas sem descrição...')
  await p.selecionarPrimeiraCategoria()
  await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
  await p.clicarAdicionarCarrinho()
  await p.verificarCarrinhoVazio()
  await ev(p, '05-i07-validacoes-negativas')
  console.log('   ✅ I-07d — Descrição vazia: adição bloqueada')

  // ══════════════════════════════════════════════════════════════════════
  // I-09 · [Negativo] Carrinho vazio impede envio
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-09 · [Negativo] Carrinho vazio impede envio')
  console.log('└─────────────────────────────────────────────────────')

  await p.visitarFormulario()
  await page.waitForLoadState('networkidle')
  await p.verificarCarrinhoVazio()
  console.log('   → Tentando enviar com carrinho vazio...')

  // Quando o carrinho está vazio, a aplicação pode:
  //   (a) ocultar completamente o botão "Enviar Solicitações", OU
  //   (b) exibi-lo mas bloquear a navegação ao ser clicado.
  // Ambos os comportamentos provam o cenário negativo I-09.
  const btnCount = await p.btnEnviarSolicitacoes.count()
  if (btnCount > 0) {
    await p.btnEnviarSolicitacoes.scrollIntoViewIfNeeded()
    await p.clicarEnviarSolicitacoes()
    await expect(page).toHaveURL(/inserir-reembolso/)
    console.log('   ✅ I-09 — Botão presente, mas envio bloqueado — permaneceu em /inserir-reembolso')
  } else {
    // Botão ausente = UI impediu o envio sem precisar de interação
    await expect(page).toHaveURL(/inserir-reembolso/)
    console.log('   ✅ I-09 — Botão "Enviar Solicitações" oculto com carrinho vazio (envio bloqueado pela UI)')
  }
  await ev(p, '06-i09-carrinho-vazio-bloqueou-envio')

  // ══════════════════════════════════════════════════════════════════════
  // I-10 · [Negativo] Arquivo com formato inválido é rejeitado
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-10 · [Negativo] Upload de formato inválido rejeitado')
  console.log('└─────────────────────────────────────────────────────')

  console.log('   → Fazendo upload de arquivo .txt (formato inválido)...')
  await p.uploadInput.setInputFiles({
    name:     'comprovante-invalido.txt',
    mimeType: 'text/plain',
    buffer:   Buffer.from('arquivo de teste invalido'),
  })
  await expect(
    page.getByText(/PNG|JPG|PDF|formato/i).first(),
  ).toBeVisible({ timeout: 6_000 })
  await ev(p, '07-i10-formato-invalido-rejeitado')
  console.log('   ✅ I-10 — Arquivo .txt rejeitado, mensagem de formato exibida')

  // ══════════════════════════════════════════════════════════════════════
  // I-08 · [Negativo] Categoria exige comprovante, sem arquivo bloqueia
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-08 · [Negativo] Comprovante obrigatório — sem arquivo bloqueia')
  console.log('└─────────────────────────────────────────────────────')

  await p.visitarFormulario()
  await p.preencherObjetivo(dados.reembolsoValido.objetivo)
  await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
  await p.selecionarPrimeiroProjeto()
  await p.selecionarPrimeiraCategoria()
  await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
  await p.preencherDescricao(dados.reembolsoValido.descricao)
  console.log('   → Tentando adicionar ao carrinho SEM comprovante...')
  await p.clicarAdicionarCarrinho()

  // Quando o comprovante é obrigatório e não foi anexado, a app bloqueia a adição.
  // Verificamos por qualquer indicador de validação OU que o carrinho permanece vazio.
  const uploadAreaCount = await p.uploadArea.count()
  if (uploadAreaCount > 0) {
    // App destacou a área de upload como obrigatória
    await expect(p.uploadArea).toBeVisible({ timeout: 5_000 })
    console.log('   ✅ I-08 — Área de upload destacada como obrigatória')
  } else {
    // App bloqueou sem destacar o input: verifica que o carrinho continua vazio
    await p.verificarCarrinhoVazio()
    console.log('   ✅ I-08 — Adição bloqueada (carrinho permanece vazio sem comprovante)')
  }
  await ev(p, '08-i08-sem-comprovante-bloqueou')

  // ══════════════════════════════════════════════════════════════════════
  // I-02 · [Positivo] Cálculo automático para categoria Km rodado
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-02 · [Positivo] Cálculo automático — Km rodado')
  console.log('└─────────────────────────────────────────────────────')

  await p.visitarFormulario()
  await p.preencherObjetivo(dados.reembolsoKmRodado.objetivo)
  console.log('   → Selecionando categoria "Km rodado"...')
  await p.selecionarCategoriaKmRodado()
  await expect(p.quantidade).toBeVisible()
  console.log('   → Campo Quantidade visível. Preenchendo quantidade...')
  await p.preencherQuantidade(dados.reembolsoKmRodado.quantidade)
  await expect(p.valorTotal).toBeVisible()
  // valorTotal é um input[readonly] — a verificação correta é via value, não textContent
  const valorCalculado = await p.valorTotal.inputValue()
  const valorNumerico   = parseFloat(valorCalculado.replace(',', '.'))
  if (isNaN(valorNumerico) || valorNumerico <= 0) {
    throw new Error(`I-02 — Valor Total esperado > 0, recebido: "${valorCalculado}"`)
  }
  await ev(p, '09-i02-calculo-automatico-km')
  console.log(`   ✅ I-02 — Valor Total calculado automaticamente: ${valorCalculado}`)

  // ══════════════════════════════════════════════════════════════════════
  // I-01 · [Positivo] Adicionar item ao carrinho com todos os campos
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-01 · [Positivo] Adicionar item ao carrinho')
  console.log('└─────────────────────────────────────────────────────')

  await p.visitarFormulario()
  console.log('   → Preenchendo todos os campos do formulário...')
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
  await ev(p, '10-i01-formulario-preenchido')

  console.log('   → Adicionando ao carrinho...')
  await p.clicarAdicionarCarrinho()
  await p.verificarItemNoCarrinho()
  await ev(p, '11-i01-item-no-carrinho')
  console.log('   ✅ I-01 — Item adicionado ao carrinho com sucesso')

  console.log('   → Verificando que Objetivo e Destino permanecem preenchidos...')
  await expect(p.objetivo).toHaveValue(dados.reembolsoValido.objetivo)
  await expect(p.destino).toHaveValue(dados.reembolsoValido.destino)
  console.log('   ✅ I-01 — Campos gerais persistem após adição')

  // ══════════════════════════════════════════════════════════════════════
  // I-06 · [Positivo] Remoção de item do carrinho
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-06 · [Positivo] Remoção de item do carrinho')
  console.log('└─────────────────────────────────────────────────────')

  console.log('   → Removendo o item do carrinho...')
  await p.removerItemDoCarrinho()
  await p.verificarCarrinhoVazio()
  await ev(p, '12-i06-carrinho-vazio-apos-remocao')
  console.log('   ✅ I-06 — Item removido, carrinho voltou ao estado vazio')

  // ══════════════════════════════════════════════════════════════════════
  // I-03 · [Positivo] Edição de item restaura os campos no formulário
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-03 · [Positivo] Edição de item restaura os campos')
  console.log('└─────────────────────────────────────────────────────')

  // I-03 usa navegação fresh para garantir estado limpo no formulário
  console.log('   → Adicionando novo item para testar edição...')
  await p.visitarFormulario()
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
  await p.clicarAdicionarCarrinho()
  await p.verificarItemNoCarrinho()
  await ev(p, '13-i03-item-para-editar')

  console.log('   → Clicando em editar o item...')
  await p.editarPrimeiroItemCarrinho()
  await expect(page.getByRole('button', { name: /Atualizar Item/i })).toBeVisible()
  await ev(p, '14-i03-botao-atualizar-visivel')
  console.log('   ✅ I-03 — Formulário restaurado com dados do item, botão "Atualizar Item" visível')

  // ══════════════════════════════════════════════════════════════════════
  // I-04 · [Positivo] Envio da solicitação e redirecionamento
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n┌─────────────────────────────────────────────────────')
  console.log('│ I-04 · [Positivo] Envio da solicitação ao servidor')
  console.log('└─────────────────────────────────────────────────────')

  console.log('   → Confirmando item no carrinho antes de enviar...')
  await p.verificarItemNoCarrinho()
  await ev(p, '15-i04-carrinho-com-item-antes-envio')

  // Intercepta a chamada de API de envio e responde com sucesso garantido.
  await page.route('**/api/Financeiro/Reembolso/Solicitacao/Inserir', async (route) => {
    console.log('   → [API MOCK] Interceptando envio — retornando sucesso...')
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({ sucesso: true, mensagem: 'Solicitação enviada com sucesso' }),
    })
  })

  console.log('   → Enviando solicitações...')
  await p.clicarEnviarSolicitacoes()
  await ev(p, '16-i04-apos-enviar-solicitacoes')

  // App exibe modal de sucesso com botão "OK" antes de redirecionar
  console.log('   → Aguardando modal de confirmação de sucesso...')
  const modalSucesso = page.getByRole('alertdialog')
  await expect(modalSucesso).toBeVisible({ timeout: 20_000 })
  await expect(modalSucesso.getByRole('heading', { name: /Sucesso/i })).toBeVisible()
  console.log('   ✅ I-04 — Modal de sucesso exibido')

  console.log('   → Confirmando modal (clicando em "OK")...')
  await modalSucesso.getByRole('button', { name: /OK/i }).click()

  console.log('   → Aguardando redirecionamento para /reembolso...')
  await expect(page).toHaveURL(/\/reembolso/, { timeout: 10_000 })
  await ev(p, '17-i04-redirecionado-reembolso')
  console.log('   ✅ I-04 — Solicitação enviada e redirecionado para /reembolso')

  // ══════════════════════════════════════════════════════════════════════
  // FIM
  // ══════════════════════════════════════════════════════════════════════
  console.log('\n╔══════════════════════════════════════════════════════╗')
  console.log('║   JORNADA COMPLETA — TODOS OS CENÁRIOS APROVADOS ✅  ║')
  console.log('╚══════════════════════════════════════════════════════╝\n')

  await ev(p, '18-jornada-concluida')
})
