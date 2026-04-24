import { Page, expect } from '@playwright/test'
import * as path from 'path'
import * as fs from 'fs'

/**
 * Page Object — Módulo de Reembolso
 * Migrado de: cypress/support/pages/ReembolsoPage.ts
 *
 * Fluxo: /reembolso → clicar "Solicitar Reembolso" → /inserir-reembolso
 *
 * Elementos mapeados via captura DOM em 01/04/2026 (dom-elements-2026-04-01.json)
 * Cenários BDD de referência: REEMBOLSO-BDD-CENARIOS-v7.md
 */
export class ReembolsoPage {
  readonly page: Page

  readonly urlLista      = '/reembolso'
  readonly urlFormulario = '/inserir-reembolso'
  readonly baseUrl: string
  readonly navTimeoutMs = 20_000

  constructor(page: Page) {
    this.page = page
    this.baseUrl = process.env.PLAYWRIGHT_BASE_URL || 'http://localhost:8080'
    this.page.setDefaultNavigationTimeout(this.navTimeoutMs)
    this.page.setDefaultTimeout(30_000)
  }

  // ── Locators — Dashboard (/reembolso) ─────────────────────────────

  get tabMeusReembolsos() {
    return this.page.getByRole('tab', { name: 'Meus Reembolsos' })
  }

  get tabGestaoAdm() {
    return this.page.getByRole('tab', { name: /Gestão ADM/i })
  }

  get btnSolicitarReembolso() {
    return this.page.getByRole('button', { name: 'Solicitar Reembolso' })
  }

  // ── Locators — Campos gerais (/inserir-reembolso) ─────────────────

  get objetivo() { return this.page.locator('#objetivo') }
  get destino()  { return this.page.locator('#destino') }
  get dataInicio() { return this.page.locator('#dataInicio') }
  get dataFim()    { return this.page.locator('#dataFim') }

  // ── Locators — Campos de item ─────────────────────────────────────

  get comboboxProjeto() { return this.page.locator('button[role="combobox"]').first() }
  get uploadInput()     { return this.page.locator('input[type="file"]') }
  get uploadArea()      { return this.page.locator('label:has(input[type="file"])') }
  // Campo Quantidade: input number editável (min="1", não é readonly)
  get quantidade()      { return this.page.locator('input[type="number"]:not([readonly])') }
  // Campo Valor Total: input number somente-leitura calculado automaticamente
  get valorTotal()      { return this.page.locator('input[type="number"][readonly]') }
  get valor()           { return this.page.locator('input[placeholder="0,00"]') }
  get descricao()       { return this.page.locator('textarea') }

  // ── Locators — Ações ──────────────────────────────────────────────

  get btnAdicionarCarrinho()  { return this.page.getByRole('button', { name: 'Adicionar ao Carrinho' }) }
  get btnEnviarSolicitacoes() { return this.page.getByRole('button', { name: 'Enviar Solicitações' }) }
  get btnLimpar()             { return this.page.getByRole('button', { name: 'Limpar' }) }

  // ── Locators — Carrinho ───────────────────────────────────────────

  get carrinhoVazioTexto() {
    return this.page.getByText('Nenhuma solicitação no carrinho')
  }

  // ── Locators — Gestão ADM (/reembolso?tab=gestao-adm) ───────────────

  get buscaGestao() {
    return this.page.getByPlaceholder('Busca')
  }

  get btnBuscarGestao() {
    return this.page.getByRole('button', { name: /^Buscar$/ })
  }

  get btnLimparGestao() {
    return this.page.getByRole('button', { name: /^Limpar$/ })
  }

  get mensagemNenhumColaborador() {
    return this.page.getByText('Nenhum colaborador encontrado', { exact: false })
  }

  get tituloColaboradores() {
    return this.page.getByRole('heading', { name: /Colaboradores/i })
  }

  // Campos de filtro (botões de calendário com label textual)
  get filtroDataInicioBotao() {
    return this.page.locator('div').filter({ hasText: /^Data Início/ }).locator('button').first()
  }

  get filtroDataFimBotao() {
    return this.page.locator('div').filter({ hasText: /^Data Fim/ }).locator('button').first()
  }

  get selectCliente() {
    return this.page.locator('label:has-text("Cliente")').locator('xpath=..').locator('button[role="combobox"],button')
  }

  get selectProjeto() {
    return this.page.locator('label:has-text("Projeto")').locator('xpath=..').locator('button[role="combobox"],button')
  }

  get selectStatus() {
    return this.page.locator('label:has-text("Status")').locator('xpath=..').locator('button[role="combobox"],button')
  }

  async abrirGestaoAdm(): Promise<boolean> {
    await this.visitarLista()
    if (await this.tabGestaoAdm.count() === 0) {
      return false
    }
    await this.tabGestaoAdm.click()
    await this.tituloColaboradores.waitFor({ timeout: 15_000 }).catch(() => {})
    return true
  }

  get itemCarrinhoLinha() {
    return this.page.locator('tbody tr, [data-testid*="item-carrinho"], .carrinho-item')
  }

  // ── Navegação ──────────────────────────────────────────────────────

  async visitarLista() {
    await this.page.goto(`${this.baseUrl}${this.urlLista}`, {
      waitUntil: 'domcontentloaded',
      timeout: this.navTimeoutMs,
    })
  }

  async visitarFormulario() {
    await this.page.goto(`${this.baseUrl}${this.urlFormulario}`, {
      waitUntil: 'domcontentloaded',
      timeout: this.navTimeoutMs,
    })
  }

  // ── Ações no Dashboard ─────────────────────────────────────────────

  async clicarSolicitarReembolso() {
    await this.btnSolicitarReembolso.click()
  }

  // ── Campos gerais ──────────────────────────────────────────────────

  async preencherObjetivo(texto: string) {
    await this.objetivo.clear()
    await this.objetivo.fill(texto)
  }

  async preencherDestino(texto: string) {
    await this.destino.clear()
    await this.destino.fill(texto)
  }

  async selecionarDataInicio(dia: string, mes: string) {
    await this.dataInicio.click()
    await this._selecionarDiaNoCalendario(dia, mes)
  }

  async selecionarDataFim(dia: string, mes: string) {
    await this.dataFim.click()
    await this._selecionarDiaNoCalendario(dia, mes)
  }

  // ── Campos de item ─────────────────────────────────────────────────

  async selecionarPrimeiroProjeto() {
    await this.comboboxProjeto.click()
    await this.page
      .locator('[role="option"], [role="listbox"] li, [cmdk-item]')
      .first()
      .click({ force: true })
  }

  /**
   * Seleciona a primeira categoria disponível.
   * Suporta <select> nativo e combobox customizado (cmdk/radix).
   */
  async selecionarPrimeiraCategoria() {
    const selects = this.page.locator('select')
    const count   = await selects.count()

    if (count > 0) {
      const select  = selects.first()
      const options = select.locator('option')
      const total   = await options.count()

      for (let i = 0; i < total; i++) {
        const val = await options.nth(i).getAttribute('value')
        if (val && val !== '' && val !== '0') {
          await select.selectOption(val)
          return
        }
      }
      await select.selectOption({ index: 1 })
    } else {
      await this.page.locator('button[role="combobox"]').nth(1).click()
      await this.page.locator('[role="option"], [cmdk-item]').first().click({ force: true })
    }
  }

  /**
   * Seleciona a categoria "Km rodado" (tipoCodigo=2) para testes de cálculo automático.
   */
  async selecionarCategoriaKmRodado() {
    const selects = this.page.locator('select')
    const count   = await selects.count()

    if (count > 0) {
      const select  = selects.first()
      const options = select.locator('option')
      const total   = await options.count()

      for (let i = 0; i < total; i++) {
        const text = await options.nth(i).textContent()
        const val  = await options.nth(i).getAttribute('value')
        if (text?.toLowerCase().includes('km') && val) {
          await select.selectOption(val)
          return
        }
      }
      await select.selectOption({ index: 1 })
    } else {
      await this.page.locator('button[role="combobox"]').nth(1).click()
      const kmOpcao = this.page.locator('[role="option"], [cmdk-item]').filter({ hasText: /km/i })
      if (await kmOpcao.count() > 0) {
        await kmOpcao.first().click({ force: true })
      } else {
        await this.page.locator('[role="option"], [cmdk-item]').first().click({ force: true })
      }
    }
  }

  async uploadComprovante(caminhoRelativo: string) {
    const caminhoAbsoluto = path.resolve(__dirname, '../../', caminhoRelativo)
    await this.uploadInput.setInputFiles(caminhoAbsoluto)
    // Aguarda o toast de "Aguarde o envio" sumir (upload assíncrono concluído)
    // ou no máximo 10s — evita que clicarAdicionarCarrinho seja bloqueado pela app
    try {
      await this.page.waitForSelector(
        'text=/Aguarde o envio|sending|uploading/i',
        { state: 'detached', timeout: 10_000 }
      )
    } catch {
      // Toast nunca apareceu ou já sumiu — upload foi imediato, seguir em frente
    }
    // Pequena pausa para garantir que a UI registrou o arquivo antes de continuar
    await this.page.waitForTimeout(800)
  }

  /**
   * Seleciona a "Data da Despesa" — terceiro date picker do formulário.
   * Identificado pelo contexto do label na página.
   */
  async selecionarDataDespesa(dia: string, mes: string) {
    await this.page
      .locator('div')
      .filter({ hasText: /^Data da Despesa/ })
      .locator('button[type="button"]')
      .first()
      .click()
    await this._selecionarDiaNoCalendario(dia, mes)
  }

  async preencherQuantidade(quantidade: string) {
    await this.quantidade.clear()
    await this.quantidade.fill(quantidade)
  }

  async preencherValor(valor: string) {
    // Máscara BRL processa apenas dígitos
    const apenasDigitos = valor.replace(/\D/g, '')
    await this.valor.clear()
    await this.valor.pressSequentially(apenasDigitos)
  }

  async preencherDescricao(texto: string) {
    await this.descricao.clear()
    await this.descricao.fill(texto)
  }

  // ── Ações no carrinho ──────────────────────────────────────────────

  async clicarAdicionarCarrinho() {
    await this.btnAdicionarCarrinho.click()
  }

  async clicarEnviarSolicitacoes() {
    await this.btnEnviarSolicitacoes.scrollIntoViewIfNeeded()
    await this.btnEnviarSolicitacoes.click()
  }

  async clicarLimpar() {
    await this.btnLimpar.click()
  }

  async editarPrimeiroItemCarrinho() {
    // DOM: <div class="flex items-center gap-1">
    //        <button class="...hover:bg-primarySoft">  ← EDITAR (1º filho)
    //        <button class="...text-destructive...">   ← EXCLUIR (2º filho)
    //      </div>
    // Estratégia: achamos o delete (text-destructive), subimos ao pai (xpath=..)
    // e pegamos o primeiro button do container — que é sempre o de editar.
    await this.page.evaluate(() => window.scrollTo({ top: 0 }))
    const trashBtn = this.page.locator('button[class*="text-destructive"]').first()
    await expect(trashBtn).toBeVisible({ timeout: 10_000 })
    const container = trashBtn.locator('xpath=..')
    const editBtn   = container.locator('button').first()
    await editBtn.scrollIntoViewIfNeeded()
    await editBtn.click({ force: true })
  }

  async removerItemDoCarrinho() {
    // O botão de exclusão do item do carrinho em InserirReembolso.tsx tem
    // className="...text-destructive..." — seletor preciso e direto.
    const btnTrash = this.page.locator('button[class*="text-destructive"]').first()
    if (await btnTrash.count() > 0) {
      await btnTrash.scrollIntoViewIfNeeded()
      await btnTrash.click()
      return
    }
    // Fallback: botão por texto/aria-label de remoção
    const btnPorTexto = this.page.getByRole('button', { name: /remov|delet|exclu|lixo|trash/i })
    if (await btnPorTexto.count() > 0) {
      await btnPorTexto.first().click()
      return
    }
    throw new Error('Não foi possível localizar o botão de remover item do carrinho')
  }

  // ── Assertions do carrinho ─────────────────────────────────────────

  async verificarItemNoCarrinho() {
    await expect(this.carrinhoVazioTexto).not.toBeVisible()
  }

  async verificarCarrinhoVazio() {
    await expect(this.carrinhoVazioTexto).toBeVisible()
  }

  // ── Helper composto ────────────────────────────────────────────────

  async preencherCamposGerais(dados: {
    objetivo  : string
    destino   : string
    dataInicio: { dia: string; mes: string }
    dataFim   : { dia: string; mes: string }
  }) {
    await this.preencherObjetivo(dados.objetivo)
    await this.preencherDestino(dados.destino)
    await this.selecionarDataInicio(dados.dataInicio.dia, dados.dataInicio.mes)
    await this.selecionarDataFim(dados.dataFim.dia, dados.dataFim.mes)
  }

  // ── Screenshot de evidência ────────────────────────────────────────

  async evidencia(nome: string, pasta = 'reembolso') {
    const dir    = `evidencias/screenshots/${pasta}`
    const caminho = `${dir}/${nome}.png`
    fs.mkdirSync(dir, { recursive: true })
    await this.page.screenshot({ path: caminho, fullPage: false })
    console.log(`   📸 [EVIDÊNCIA] ${caminho}`)
  }

  // ── Calendário (privado) ───────────────────────────────────────────

  private async _selecionarDiaNoCalendario(dia: string, mes: string) {
    const calendario = this.page.locator(
      '[role="dialog"], [data-radix-popper-content-wrapper], .rdp, .calendar',
    )
    await expect(calendario.first()).toBeVisible()

    await this.page.getByText(mes, { exact: false }).first().waitFor()

    await this.page
      .locator('[role="gridcell"] button:not([disabled]):not([aria-disabled="true"]), .rdp-day:not([disabled]), td button:not([disabled])')
      .filter({ hasText: new RegExp(`^${dia}$`) })
      .first()
      .click()
  }
}
