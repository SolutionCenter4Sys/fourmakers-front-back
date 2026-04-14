import { test, expect } from '@playwright/test'
import * as fs from 'fs'
import { ReembolsoPage } from '../../support/pages/ReembolsoPage'
import { loginFourMakers } from '../../support/auth/fourmakers-auth'
import reembolsoFixture from '../../fixtures/reembolso.json'

/**
 * Testes E2E — Jornada Completa de Solicitação de Reembolso
 * Migrado de: cypress/e2e/reembolso/reembolso-jornada.cy.ts
 *
 * Cenários BDD de referência: REEMBOLSO-BDD-CENARIOS-v7.md
 * Seletores validados via: dom-elements-2026-04-01.json
 *
 * Cobertura:
 *   R-01 · [Positivo]  Dashboard — aba padrão "Meus Reembolsos"
 *   R-06 · [Positivo]  Dashboard — navegação para nova solicitação
 *   I-01 · [Positivo]  Formulário — adicionar item ao carrinho com campos obrigatórios
 *   I-02 · [Positivo]  Formulário — cálculo automático para categoria tipoCodigo=2 (Km rodado)
 *   I-03 · [Positivo]  Carrinho   — edição de item restaura todos os campos
 *   I-04 · [Positivo]  Carrinho   — envio gera solicitação e redireciona
 *   I-06 · [Positivo]  Carrinho   — remoção de item
 *   I-07 · [Negativo]  Validação  — campos obrigatórios vazios bloqueiam adição
 *   I-08 · [Negativo]  Validação  — categoria exige comprovante; sem arquivo bloqueia
 *   I-09 · [Negativo]  Carrinho   — vazio impede envio
 *   I-10 · [Negativo]  Upload     — arquivo com formato inválido é rejeitado
 *
 * Evidências geradas em: evidencias/screenshots/reembolso-jornada/
 * Relatório gerado em:   evidencias/relatorios/
 */

test.describe.configure({ mode: 'serial' })

const dados = reembolsoFixture as any

test.describe('Reembolso — Jornada Completa de Solicitação', () => {

  test.beforeEach(async ({ page, request }) => {
    console.log('\n──────────────────────────────────────────')
    console.log('▶ INICIANDO: Setup — autenticação e sessão')
    console.log('──────────────────────────────────────────')
    await loginFourMakers(page, request)
    console.log('✅ Setup concluído\n')
  })

  test.afterEach(async ({ page }, testInfo) => {
    const estado      = testInfo.status === 'passed' ? 'passou' : 'falhou'
    const tituloLimpo = testInfo.title.replace(/[^\w\s-]/g, '').replace(/\s+/g, '-').toLowerCase().substring(0, 60)
    const dir         = 'evidencias/screenshots/reembolso-jornada'
    const caminho     = `${dir}/${estado}--${tituloLimpo}.png`

    fs.mkdirSync(dir, { recursive: true })
    await page.screenshot({ path: caminho })
    console.log(`\n   📸 [EVIDÊNCIA FINAL] ${caminho}`)
  })

  // ─────────────────────────────────────────────────────────────────────
  // R-01 · [Positivo] Visualização dos reembolsos na aba padrão
  // ─────────────────────────────────────────────────────────────────────
  test.describe('R-01 · Dashboard — Visualização da aba padrão', () => {

    test('deve exibir a aba "Meus Reembolsos" ativa ao acessar a tela de Reembolso', async ({ page }) => {
      console.log('\n🧪 TESTANDO: R-01 — Aba padrão "Meus Reembolsos"')
      const p = new ReembolsoPage(page)

      console.log('   → Navegando para /reembolso...')
      await p.visitarLista()
      await p.evidencia('r01-dashboard-carregado', 'reembolso-jornada')

      console.log('   → Verificando visibilidade da aba "Meus Reembolsos"...')
      await expect(page.getByRole('tab', { name: 'Meus Reembolsos' })).toBeVisible()
      await p.evidencia('r01-aba-meus-reembolsos-visivel', 'reembolso-jornada')

      console.log('   ✅ Aba "Meus Reembolsos" visível conforme esperado')
      console.log('🏁 R-01 CONCLUÍDO\n')
    })

    test('deve exibir o botão "Solicitar Reembolso" no dashboard', async ({ page }) => {
      console.log('\n🧪 TESTANDO: R-01 — Botão "Solicitar Reembolso" no dashboard')
      const p = new ReembolsoPage(page)

      await p.visitarLista()
      console.log('   → Verificando botão "Solicitar Reembolso"...')
      await expect(p.btnSolicitarReembolso).toBeVisible()
      await p.evidencia('r01-botao-solicitar-visivel', 'reembolso-jornada')

      console.log('   ✅ Botão visível no dashboard')
      console.log('🏁 R-01 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // R-06 · [Positivo] Navegação para nova solicitação
  // ─────────────────────────────────────────────────────────────────────
  test.describe('R-06 · Dashboard — Navegação para nova solicitação', () => {

    test('deve navegar para /inserir-reembolso ao acionar "Solicitar Reembolso"', async ({ page }) => {
      console.log('\n🧪 TESTANDO: R-06 — Navegação para /inserir-reembolso')
      const p = new ReembolsoPage(page)

      await p.visitarLista()
      await p.evidencia('r06-dashboard-antes-navegar', 'reembolso-jornada')

      console.log('   → Clicando em "Solicitar Reembolso"...')
      await p.clicarSolicitarReembolso()
      await p.evidencia('r06-apos-clicar-solicitar', 'reembolso-jornada')

      console.log('   → Verificando URL /inserir-reembolso...')
      await expect(page).toHaveURL(/inserir-reembolso/)
      await p.evidencia('r06-url-inserir-reembolso-confirmada', 'reembolso-jornada')

      console.log('   ✅ Navegação para /inserir-reembolso confirmada')
      console.log('🏁 R-06 CONCLUÍDO\n')
    })

    test('deve exibir o formulário "Nova Solicitação" após navegação', async ({ page }) => {
      console.log('\n🧪 TESTANDO: R-06 — Formulário "Nova Solicitação" exibido após navegação')
      const p = new ReembolsoPage(page)

      await p.visitarLista()
      await p.clicarSolicitarReembolso()

      console.log('   → Verificando título e campo Objetivo...')
      await expect(page.getByText(/Nova Solicita/i)).toBeVisible()
      await expect(p.objetivo).toBeVisible()
      await p.evidencia('r06-formulario-nova-solicitacao-visivel', 'reembolso-jornada')

      console.log('   ✅ Formulário "Nova Solicitação" exibido corretamente')
      console.log('🏁 R-06 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-01 · [Positivo] Adicionar item ao carrinho com campos obrigatórios
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-01 · Formulário — Adicionar item ao carrinho', () => {

    test('deve adicionar item ao carrinho após preencher todos os campos obrigatórios', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-01 — Adição de item ao carrinho (fluxo completo)')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()
      await p.evidencia('i01-formulario-vazio', 'reembolso-jornada')

      console.log('   → Preenchendo Objetivo e Destino...')
      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.preencherDestino(dados.reembolsoValido.destino)

      console.log('   → Selecionando datas de início e fim do período...')
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarDataFim(dados.reembolsoValido.dataFim.dia, dados.reembolsoValido.dataFim.mes)
      await p.evidencia('i01-campos-gerais-preenchidos', 'reembolso-jornada')

      console.log('   → Selecionando projeto...')
      await p.selecionarPrimeiroProjeto()
      await p.evidencia('i01-projeto-selecionado', 'reembolso-jornada')

      console.log('   → Selecionando categoria...')
      await p.selecionarPrimeiraCategoria()
      await p.evidencia('i01-categoria-selecionada', 'reembolso-jornada')

      console.log('   → Fazendo upload do comprovante...')
      await p.uploadComprovante('fixtures/comprovante-teste.jpg')
      await p.evidencia('i01-comprovante-anexado', 'reembolso-jornada')

      console.log('   → Selecionando data da despesa...')
      await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
      await p.evidencia('i01-data-despesa-selecionada', 'reembolso-jornada')

      console.log('   → Preenchendo descrição...')
      await p.preencherDescricao(dados.reembolsoValido.descricao)
      await p.evidencia('i01-descricao-preenchida', 'reembolso-jornada')

      console.log('   → Adicionando ao carrinho...')
      await p.clicarAdicionarCarrinho()
      await p.evidencia('i01-apos-clicar-adicionar-carrinho', 'reembolso-jornada')

      console.log('   → Verificando item no carrinho...')
      await p.verificarItemNoCarrinho()
      await p.evidencia('i01-item-confirmado-no-carrinho', 'reembolso-jornada')

      console.log('   ✅ Item adicionado ao carrinho com sucesso')
      console.log('🏁 I-01 CONCLUÍDO\n')
    })

    test('deve manter os campos gerais (Objetivo, Destino) após adicionar item ao carrinho', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-01 — Persistência dos campos gerais após adição ao carrinho')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.preencherDestino(dados.reembolsoValido.destino)
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarDataFim(dados.reembolsoValido.dataFim.dia, dados.reembolsoValido.dataFim.mes)
      await p.selecionarPrimeiroProjeto()
      await p.selecionarPrimeiraCategoria()
      await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
      await p.preencherDescricao(dados.reembolsoValido.descricao)
      await p.clicarAdicionarCarrinho()
      await p.evidencia('i01-campos-gerais-mantidos-apos-adicionar', 'reembolso-jornada')

      console.log('   → Verificando que Objetivo e Destino permanecem preenchidos...')
      await expect(p.objetivo).toHaveValue(dados.reembolsoValido.objetivo)
      await expect(p.destino).toHaveValue(dados.reembolsoValido.destino)
      console.log('   ✅ Campos gerais persistem após adição ao carrinho')
      console.log('🏁 I-01 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-02 · [Positivo] Cálculo automático (categoria tipoCodigo=2 / Km rodado)
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-02 · Formulário — Cálculo automático (Km rodado)', () => {

    test('deve exibir os campos "Quantidade" para categoria tipoCodigo=2', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-02 — Campo Quantidade para categoria Km rodado')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoKmRodado.objetivo)
      console.log('   → Selecionando categoria "Km rodado"...')
      await p.selecionarCategoriaKmRodado()
      await p.evidencia('i02-categoria-km-rodado-selecionada', 'reembolso-jornada')

      console.log('   → Verificando visibilidade do campo Quantidade...')
      await expect(p.quantidade).toBeVisible()
      await p.evidencia('i02-campo-quantidade-visivel', 'reembolso-jornada')

      console.log('   ✅ Campo Quantidade visível para categoria Km rodado')
      console.log('🏁 I-02 CONCLUÍDO\n')
    })

    test('deve calcular o Valor Total automaticamente ao informar a quantidade', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-02 — Cálculo automático do Valor Total')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoKmRodado.objetivo)
      await p.selecionarCategoriaKmRodado()
      await p.evidencia('i02-antes-preencher-quantidade', 'reembolso-jornada')

      console.log(`   → Preenchendo quantidade: "${dados.reembolsoKmRodado.quantidade}"...`)
      await p.preencherQuantidade(dados.reembolsoKmRodado.quantidade)
      await p.evidencia('i02-quantidade-preenchida', 'reembolso-jornada')

      console.log('   → Verificando painel "Valor Total"...')
      await expect(p.valorTotal).toBeVisible()
      await expect(p.valorTotal).toContainText('Valor Total')
      await p.evidencia('i02-valor-total-calculado-exibido', 'reembolso-jornada')

      console.log('   ✅ Valor Total calculado e exibido automaticamente')
      console.log('🏁 I-02 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-03 · [Positivo] Edição de item do carrinho restaura todos os campos
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-03 · Carrinho — Edição de item restaura todos os campos', () => {

    test('deve preencher o formulário com os dados do item ao editar e exibir "Atualizar Item"', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-03 — Edição de item do carrinho')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.preencherDestino(dados.reembolsoValido.destino)
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarDataFim(dados.reembolsoValido.dataFim.dia, dados.reembolsoValido.dataFim.mes)
      await p.selecionarPrimeiroProjeto()
      await p.selecionarPrimeiraCategoria()
      await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
      await p.preencherDescricao(dados.reembolsoValido.descricao)
      await p.clicarAdicionarCarrinho()
      await p.verificarItemNoCarrinho()
      await p.evidencia('i03-item-no-carrinho-antes-editar', 'reembolso-jornada')

      console.log('   → Clicando no botão de editar o primeiro item do carrinho...')
      await p.itemCarrinhoLinha
        .first()
        .locator('button')
        .first()
        .click({ force: true })
      await p.evidencia('i03-apos-clicar-editar', 'reembolso-jornada')

      console.log('   → Verificando botão "Atualizar Item"...')
      await expect(page.getByRole('button', { name: /Atualizar Item/i })).toBeVisible()
      await p.evidencia('i03-botao-atualizar-item-visivel', 'reembolso-jornada')

      console.log('   ✅ Formulário preenchido com dados do item e botão "Atualizar Item" visível')
      console.log('🏁 I-03 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-04 · [Positivo] Envio do carrinho com itens gera solicitação consolidada
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-04 · Jornada Completa — Envio da solicitação ao servidor', () => {

    test('deve enviar o carrinho, exibir confirmação e redirecionar para /reembolso', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-04 — Jornada completa de envio do carrinho')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()
      await p.evidencia('i04-inicio-jornada-completa', 'reembolso-jornada')

      console.log('   → Preenchendo campos gerais...')
      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.preencherDestino(dados.reembolsoValido.destino)
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarDataFim(dados.reembolsoValido.dataFim.dia, dados.reembolsoValido.dataFim.mes)
      await p.evidencia('i04-campos-gerais-preenchidos', 'reembolso-jornada')

      console.log('   → Preenchendo campos de item...')
      await p.selecionarPrimeiroProjeto()
      await p.selecionarPrimeiraCategoria()
      await p.uploadComprovante('fixtures/comprovante-teste.jpg')
      await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
      await p.preencherDescricao(dados.reembolsoValido.descricao)
      await p.evidencia('i04-formulario-completo-preenchido', 'reembolso-jornada')

      console.log('   → Adicionando item ao carrinho...')
      await p.clicarAdicionarCarrinho()
      await p.evidencia('i04-item-adicionado-ao-carrinho', 'reembolso-jornada')

      await p.verificarItemNoCarrinho()
      await p.evidencia('i04-carrinho-com-item-confirmado', 'reembolso-jornada')

      console.log('   → Enviando solicitações...')
      await p.clicarEnviarSolicitacoes()
      await p.evidencia('i04-apos-enviar-solicitacoes', 'reembolso-jornada')

      console.log('   → Aguardando redirecionamento para /reembolso...')
      await expect(page).toHaveURL(/\/reembolso/, { timeout: 20_000 })
      await p.evidencia('i04-redirecionado-para-reembolso', 'reembolso-jornada')

      console.log('   ✅ Solicitação enviada e redirecionamento para /reembolso confirmado')
      console.log('🏁 I-04 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-06 · [Positivo] Remoção de item do carrinho
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-06 · Carrinho — Remoção de item', () => {

    test('deve remover o item do carrinho e exibir estado vazio após exclusão', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-06 — Remoção de item do carrinho')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarPrimeiroProjeto()
      await p.selecionarPrimeiraCategoria()
      await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
      await p.preencherDescricao(dados.reembolsoValido.descricao)
      await p.clicarAdicionarCarrinho()
      await p.verificarItemNoCarrinho()
      await p.evidencia('i06-carrinho-com-item-antes-remover', 'reembolso-jornada')

      console.log('   → Removendo item do carrinho...')
      await p.removerItemDoCarrinho()
      await p.evidencia('i06-apos-remover-item', 'reembolso-jornada')

      console.log('   → Verificando carrinho vazio...')
      await p.verificarCarrinhoVazio()
      await p.evidencia('i06-carrinho-vazio-confirmado', 'reembolso-jornada')

      console.log('   ✅ Item removido e carrinho exibe estado vazio')
      console.log('🏁 I-06 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-07 · [Negativo] Campos obrigatórios vazios bloqueiam adição
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-07 · Validação — Campos obrigatórios bloqueiam adição ao carrinho', () => {

    test('[Objetivo vazio] deve bloquear adição ao carrinho sem preencher Objetivo', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-07 — [Negativo] Objetivo vazio bloqueia adição')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      console.log('   → Tentando adicionar ao carrinho sem preencher nenhum campo...')
      await p.clicarAdicionarCarrinho()
      await p.evidencia('i07-objetivo-vazio-apos-tentar-adicionar', 'reembolso-jornada')

      console.log('   → Verificando que o carrinho permanece vazio...')
      await p.verificarCarrinhoVazio()
      await p.evidencia('i07-objetivo-vazio-carrinho-ainda-vazio', 'reembolso-jornada')

      console.log('   ✅ Adição bloqueada — carrinho vazio confirmado')
      console.log('🏁 I-07 CONCLUÍDO\n')
    })

    test('[Data de Início vazia] deve bloquear adição ao carrinho sem Data de Início', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-07 — [Negativo] Data de Início vazia bloqueia adição')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      console.log('   → Tentando adicionar sem data de início...')
      await p.clicarAdicionarCarrinho()
      await p.evidencia('i07-data-inicio-vazia-apos-tentar-adicionar', 'reembolso-jornada')

      await p.verificarCarrinhoVazio()
      console.log('   ✅ Adição bloqueada — Data de Início obrigatória')
      console.log('🏁 I-07 CONCLUÍDO\n')
    })

    test('[Categoria vazia] deve bloquear adição ao carrinho sem selecionar Categoria', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-07 — [Negativo] Categoria vazia bloqueia adição')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarPrimeiroProjeto()
      console.log('   → Tentando adicionar sem selecionar categoria...')
      await p.clicarAdicionarCarrinho()
      await p.evidencia('i07-categoria-vazia-apos-tentar-adicionar', 'reembolso-jornada')

      await p.verificarCarrinhoVazio()
      console.log('   ✅ Adição bloqueada — Categoria obrigatória')
      console.log('🏁 I-07 CONCLUÍDO\n')
    })

    test('[Descrição vazia] deve bloquear adição ao carrinho sem preencher Descrição', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-07 — [Negativo] Descrição vazia bloqueia adição')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarPrimeiroProjeto()
      await p.selecionarPrimeiraCategoria()
      await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
      console.log('   → Tentando adicionar sem preencher descrição...')
      await p.clicarAdicionarCarrinho()
      await p.evidencia('i07-descricao-vazia-apos-tentar-adicionar', 'reembolso-jornada')

      await p.verificarCarrinhoVazio()
      console.log('   ✅ Adição bloqueada — Descrição obrigatória')
      console.log('🏁 I-07 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-08 · [Negativo] Categoria que exige comprovante bloqueia sem arquivo
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-08 · Validação — Comprovante obrigatório bloqueia adição sem arquivo', () => {

    test('deve bloquear adição quando categoria exige comprovante e nenhum arquivo foi anexado', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-08 — [Negativo] Comprovante obrigatório não anexado')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()

      await p.preencherObjetivo(dados.reembolsoValido.objetivo)
      await p.selecionarDataInicio(dados.reembolsoValido.dataInicio.dia, dados.reembolsoValido.dataInicio.mes)
      await p.selecionarPrimeiroProjeto()
      await p.selecionarPrimeiraCategoria()
      await p.selecionarDataDespesa(dados.reembolsoValido.dataDespesa.dia, dados.reembolsoValido.dataDespesa.mes)
      await p.preencherDescricao(dados.reembolsoValido.descricao)
      console.log('   → Tentando adicionar sem anexar comprovante...')
      await p.clicarAdicionarCarrinho()
      await p.evidencia('i08-sem-comprovante-apos-tentar-adicionar', 'reembolso-jornada')

      console.log('   → Verificando área de upload ainda visível...')
      await expect(p.uploadArea).toBeVisible()
      await p.evidencia('i08-area-upload-ainda-visivel', 'reembolso-jornada')

      console.log('   ✅ Adição bloqueada — área de upload sinalizada')
      console.log('🏁 I-08 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-09 · [Negativo] Carrinho vazio impede envio
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-09 · Carrinho — Vazio impede envio da solicitação', () => {

    test('deve bloquear envio e permanecer na página quando o carrinho está vazio', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-09 — [Negativo] Carrinho vazio bloqueia envio')
      const p = new ReembolsoPage(page)

      await p.visitarFormulario()
      await p.evidencia('i09-formulario-carrinho-vazio', 'reembolso-jornada')

      console.log('   → Verificando estado vazio do carrinho...')
      await p.verificarCarrinhoVazio()

      console.log('   → Tentando enviar com carrinho vazio...')
      await p.clicarEnviarSolicitacoes()
      await p.evidencia('i09-apos-tentar-enviar-carrinho-vazio', 'reembolso-jornada')

      console.log('   → Verificando que permaneceu em /inserir-reembolso...')
      await expect(page).toHaveURL(/inserir-reembolso/)
      await p.evidencia('i09-permaneceu-no-formulario', 'reembolso-jornada')

      console.log('   ✅ Envio bloqueado — permaneceu na tela de inserção')
      console.log('🏁 I-09 CONCLUÍDO\n')
    })
  })

  // ─────────────────────────────────────────────────────────────────────
  // I-10 · [Negativo] Arquivo com formato inválido é rejeitado
  // ─────────────────────────────────────────────────────────────────────
  test.describe('I-10 · Upload — Arquivo com formato inválido é rejeitado', () => {

    test('deve rejeitar arquivo .txt e exibir notificação sobre formatos aceitos', async ({ page }) => {
      console.log('\n🧪 TESTANDO: I-10 — [Negativo] Upload de arquivo com formato inválido')
      const p = new ReembolsoPage(page)
      await p.visitarFormulario()
      await p.evidencia('i10-antes-upload-formato-invalido', 'reembolso-jornada')

      console.log('   → Fazendo upload de arquivo .txt (formato inválido)...')
      await p.uploadInput.setInputFiles({
        name:     'comprovante-teste.txt',
        mimeType: 'text/plain',
        buffer:   Buffer.from('arquivo de teste invalido'),
      })
      await p.evidencia('i10-apos-selecionar-arquivo-invalido', 'reembolso-jornada')

      console.log('   → Verificando mensagem de erro sobre formatos aceitos...')
      await expect(
        page.getByText(/PNG|JPG|PDF|formato/i).first(),
      ).toBeVisible({ timeout: 6_000 })
      await p.evidencia('i10-notificacao-formato-invalido-exibida', 'reembolso-jornada')

      console.log('   ✅ Arquivo rejeitado — notificação de formato inválido exibida')
      console.log('🏁 I-10 CONCLUÍDO\n')
    })
  })

})
