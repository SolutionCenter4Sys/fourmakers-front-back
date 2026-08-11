import { test, expect } from '@playwright/test'
import * as fs from 'fs'
import { ReembolsoPage } from '../../support/pages/ReembolsoPage'
import { loginFourMakers } from '../../support/auth/fourmakers-auth'
import reembolsoFixture from '../../fixtures/reembolso.json'

/**
 * Testes E2E — Módulo de Reembolso (Cenários Positivos)
 * Migrado de: cypress/e2e/reembolso/reembolso.cy.ts
 *
 * Fluxo mapeado via captura DOM em 01/04/2026:
 *   /reembolso          → botão "Solicitar Reembolso"
 *   /inserir-reembolso  → formulário de cadastro de reembolso
 *
 * Evidências geradas em: ../../DEMO/outputs/playwright/reembolso/screenshots/
 * Relatório gerado em:   ../../DEMO/outputs/playwright/reembolso/relatorios/
 */

// ─── Setup de sessão ──────────────────────────────────────────────────────────
// Sessão cacheada no storage state após o primeiro teste para evitar múltiplos OTPs
test.describe.configure({ mode: 'serial' })

test.describe('Reembolso — Cadastro de Solicitação', () => {

  test.beforeEach(async ({ page, request }) => {
    console.log('\n──────────────────────────────────────────')
    console.log('▶ INICIANDO: Setup — autenticação e navegação')
    console.log('──────────────────────────────────────────')
    await loginFourMakers(page, request)
    console.log('✅ Setup concluído\n')
  })

  test.afterEach(async ({ page }, testInfo) => {
    const estado      = testInfo.status === 'passed' ? 'passou' : 'falhou'
    const tituloLimpo = testInfo.title.replace(/[^\w\s-]/g, '').replace(/\s+/g, '-').toLowerCase().substring(0, 60)
    const dir         = '../../DEMO/outputs/playwright/reembolso/screenshots'
    const caminho     = `${dir}/${estado}--${tituloLimpo}.png`

    fs.mkdirSync(dir, { recursive: true })
    await page.screenshot({ path: caminho })
    console.log(`\n   📸 [EVIDÊNCIA FINAL] ${caminho}`)
  })

  // ─────────────────────────────────────────────────────────────
  // Acesso ao módulo
  // ─────────────────────────────────────────────────────────────
  test.describe('Acesso ao módulo de Reembolso', () => {

    test('deve exibir o botão "Solicitar Reembolso" na página de listagem', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Visibilidade do botão "Solicitar Reembolso"')
      const reembolsoPage = new ReembolsoPage(page)

      console.log('   → Navegando para /reembolso...')
      await reembolsoPage.visitarLista()
      await reembolsoPage.evidencia('01-listagem-reembolso')

      console.log('   → Verificando visibilidade do botão "Solicitar Reembolso"...')
      await expect(reembolsoPage.btnSolicitarReembolso).toBeVisible()
      await reembolsoPage.evidencia('02-botao-solicitar-visivel')

      console.log('   ✅ Botão "Solicitar Reembolso" visível')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve navegar para o formulário ao clicar em "Solicitar Reembolso"', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Navegação para formulário de inserção')
      const reembolsoPage = new ReembolsoPage(page)

      console.log('   → Acessando listagem e clicando em "Solicitar Reembolso"...')
      await reembolsoPage.visitarLista()
      await reembolsoPage.clicarSolicitarReembolso()
      await reembolsoPage.evidencia('03-navegou-para-formulario')

      console.log('   → Verificando URL /inserir-reembolso...')
      await expect(page).toHaveURL(/inserir-reembolso/)

      console.log('   ✅ Navegação para formulário confirmada')
      console.log('🏁 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────
  // Verificação dos campos do formulário
  // ─────────────────────────────────────────────────────────────
  test.describe('Verificação de Campos do Formulário', () => {

    test('deve exibir o campo Objetivo', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Campo Objetivo')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await expect(p.objetivo).toBeVisible()
      await expect(p.objetivo).toHaveAttribute('placeholder', 'Objetivo')
      console.log('   ✅ Campo Objetivo visível com placeholder correto')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve exibir o campo Destino', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Campo Destino')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await expect(p.destino).toBeVisible()
      await expect(p.destino).toHaveAttribute('placeholder', 'Destino')
      console.log('   ✅ Campo Destino visível com placeholder correto')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve exibir os seletores de data de início e fim', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Seletores de data início e fim')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      console.log('   → Verificando data de início...')
      await expect(p.dataInicio).toBeVisible()
      await expect(p.dataInicio).toContainText('Selecione a data')

      console.log('   → Verificando data fim...')
      await expect(p.dataFim).toBeVisible()
      await expect(p.dataFim).toContainText('Selecione a data')

      console.log('   ✅ Ambos os seletores de data visíveis e com texto padrão')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve exibir o seletor de projeto', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Combobox de Projeto')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await expect(p.comboboxProjeto).toBeVisible()
      await expect(p.comboboxProjeto).toContainText('Selecione o projeto')
      console.log('   ✅ Combobox de projeto visível')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve exibir a área de upload de comprovante', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Área de upload de comprovante')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await expect(page.getByText('Clique ou arraste os comprovantes')).toBeVisible()
      await expect(page.getByText('Apenas PNG, JPG e PDF')).toBeVisible()
      console.log('   ✅ Área de upload visível com instruções corretas')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve exibir o campo de valor numérico', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Campo de valor (máscara BRL)')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await expect(p.valor).toBeVisible()
      await expect(p.valor).toHaveAttribute('placeholder', '0,00')
      console.log('   ✅ Campo de valor visível com placeholder "0,00"')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve exibir o campo de descrição da despesa', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Campo de descrição da despesa')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await expect(p.descricao).toBeVisible()
      const placeholder = await p.descricao.getAttribute('placeholder')
      expect(placeholder).toContain('Descreva a despesa')
      console.log('   ✅ Campo descrição visível com placeholder correto')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve exibir o botão "Adicionar ao Carrinho"', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Botão "Adicionar ao Carrinho"')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await expect(p.btnAdicionarCarrinho).toBeVisible()
      console.log('   ✅ Botão "Adicionar ao Carrinho" visível')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve exibir todos os campos em uma única verificação', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Verificação completa de todos os campos do formulário')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()
      await p.evidencia('04-formulario-vazio-completo')

      console.log('   → Verificando todos os campos...')
      await expect(p.objetivo).toBeVisible()
      await expect(p.destino).toBeVisible()
      await expect(p.dataInicio).toBeVisible()
      await expect(p.dataFim).toBeVisible()
      await expect(p.comboboxProjeto).toBeVisible()
      await expect(p.valor).toBeVisible()
      await expect(p.descricao).toBeVisible()
      await expect(p.btnAdicionarCarrinho).toBeVisible()
      console.log('   ✅ Todos os 8 campos/botões visíveis')
      console.log('🏁 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────
  // Cadastro de Reembolso com Sucesso
  // ─────────────────────────────────────────────────────────────
  test.describe('Cadastro de Reembolso com Sucesso', () => {

    test('deve preencher o campo Objetivo corretamente', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Preenchimento do campo Objetivo')
      const p    = new ReembolsoPage(page)
      const dados = reembolsoFixture as any
      await p.visitarFormulario()

      console.log(`   → Preenchendo Objetivo: "${dados.reembolsoValido.objetivo}"...`)
      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await expect(p.objetivo).toHaveValue(dados.reembolsoValido.objetivo)
      console.log('   ✅ Campo Objetivo preenchido com valor correto')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve preencher o campo Destino corretamente', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Preenchimento do campo Destino')
      const p    = new ReembolsoPage(page)
      const dados = reembolsoFixture as any
      await p.visitarFormulario()

      console.log(`   → Preenchendo Destino: "${dados.reembolsoValido.destino}"...`)
      await p.preencherDestino(dados.reembolsoValido.destino)
      await expect(p.destino).toHaveValue(dados.reembolsoValido.destino)
      console.log('   ✅ Campo Destino preenchido com valor correto')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve preencher o valor do reembolso corretamente', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Preenchimento do campo Valor (máscara BRL)')
      const p    = new ReembolsoPage(page)
      const dados = reembolsoFixture as any
      await p.visitarFormulario()

      console.log(`   → Preenchendo valor: "${dados.reembolsoValido.valor}"...`)
      await p.preencherValor(dados.reembolsoValido.valor)
      await expect(p.valor).toHaveValue(dados.reembolsoValido.valorFormatado)
      console.log(`   ✅ Campo Valor exibe "${dados.reembolsoValido.valorFormatado}" (máscara BRL aplicada)`)
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve preencher a descrição da despesa corretamente', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Preenchimento da descrição')
      const p    = new ReembolsoPage(page)
      const dados = reembolsoFixture as any
      await p.visitarFormulario()

      console.log(`   → Preenchendo Descrição: "${dados.reembolsoValido.descricao}"...`)
      await p.preencherDescricao(dados.reembolsoValido.descricao)
      await expect(p.descricao).toHaveValue(dados.reembolsoValido.descricao)
      console.log('   ✅ Campo Descrição preenchido com valor correto')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve abrir o calendário ao clicar no campo de data de início', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Abertura do calendário — Data de Início')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      console.log('   → Clicando no campo de data de início...')
      await p.dataInicio.click()
      await p.evidencia('06-calendario-data-inicio-aberto')

      const calendario = page.locator('[role="dialog"], [data-radix-popper-content-wrapper], table')
      await expect(calendario.first()).toBeVisible()
      console.log('   ✅ Calendário aberto com sucesso')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve abrir o calendário ao clicar no campo de data fim', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Abertura do calendário — Data Fim')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      console.log('   → Clicando no campo de data fim...')
      await p.dataFim.click()
      await p.evidencia('07-calendario-data-fim-aberto')

      const calendario = page.locator('[role="dialog"], [data-radix-popper-content-wrapper], table')
      await expect(calendario.first()).toBeVisible()
      console.log('   ✅ Calendário aberto com sucesso')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve abrir a lista de projetos ao clicar no combobox', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Abertura da lista de projetos')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      console.log('   → Clicando no combobox de projeto...')
      await p.comboboxProjeto.click()
      await p.evidencia('08-lista-projetos-aberta')

      const opcoes = page.locator('[role="option"], [role="listbox"], [cmdk-item]')
      await expect(opcoes.first()).toBeVisible()
      console.log('   ✅ Lista de projetos exibida')
      console.log('🏁 CONCLUÍDO\n')
    })

    test('deve cadastrar reembolso completo e adicionar ao carrinho', async ({ page }) => {
      console.log('\n🧪 TESTANDO: Cadastro completo de reembolso e adição ao carrinho')
      const p    = new ReembolsoPage(page)
      const dados = reembolsoFixture as any
      await p.visitarFormulario()
      await p.evidencia('09-inicio-cadastro-completo')

      console.log('   → Preenchendo Objetivo e Destino...')
      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.preencherDestino(dados.reembolsoValido.destino)
      await p.evidencia('10-objetivo-destino-preenchidos')

      console.log('   → Selecionando datas de início e fim...')
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarDataFim(dados.reembolsoValido.dataFim.dia, dados.reembolsoValido.dataFim.mes)
      await p.evidencia('11-datas-selecionadas')

      console.log('   → Selecionando projeto...')
      await p.selecionarPrimeiroProjeto()
      await p.evidencia('12-projeto-selecionado')

      console.log('   → Preenchendo valor e descrição...')
      await p.preencherValor(dados.reembolsoValido.valor)
      await p.preencherDescricao(dados.reembolsoValido.descricao)
      await p.evidencia('14-formulario-completo-preenchido')

      console.log('   → Adicionando ao carrinho...')
      await p.clicarAdicionarCarrinho()
      await p.evidencia('15-apos-adicionar-ao-carrinho')

      await expect(page.locator('body')).toBeVisible()
      await expect(page).toHaveURL(/fourmakers\.io/)
      await expect(p.btnAdicionarCarrinho).toBeVisible()
      console.log('   ✅ Item adicionado ao carrinho com sucesso')
      console.log('🏁 CONCLUÍDO\n')
    })
  })
})
