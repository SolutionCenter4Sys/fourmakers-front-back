# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: fourmakers-v2-develop\frontend\playwright-automation-template\tests\reembolso\versionadas\reembolso-demo-v2.spec.ts >> I-08 [Negativo] Submissão com campos obrigatórios vazios
- Location: fourmakers-v2-develop\frontend\playwright-automation-template\tests\reembolso\versionadas\reembolso-demo-v2.spec.ts:362:5

# Error details

```
Error: page.goto: Protocol error (Page.navigate): Cannot navigate to invalid URL
Call log:
  - navigating to "/inserir-reembolso", waiting until "domcontentloaded"

```

# Test source

```ts
  1   | import { Page, expect } from '@playwright/test'
  2   | import * as path from 'path'
  3   | import * as fs from 'fs'
  4   | 
  5   | /**
  6   |  * Page Object — Módulo de Reembolso
  7   |  * Migrado de: cypress/support/pages/ReembolsoPage.ts
  8   |  *
  9   |  * Fluxo: /reembolso → clicar "Solicitar Reembolso" → /inserir-reembolso
  10  |  *
  11  |  * Elementos mapeados via captura DOM em 01/04/2026 (dom-elements-2026-04-01.json)
  12  |  * Cenários BDD de referência: REEMBOLSO-BDD-CENARIOS-v7.md
  13  |  */
  14  | export class ReembolsoPage {
  15  |   readonly page: Page
  16  | 
  17  |   readonly urlLista      = '/reembolso'
  18  |   readonly urlFormulario = '/inserir-reembolso'
  19  | 
  20  |   constructor(page: Page) {
  21  |     this.page = page
  22  |   }
  23  | 
  24  |   // ── Locators — Dashboard (/reembolso) ─────────────────────────────
  25  | 
  26  |   get tabMeusReembolsos() {
  27  |     return this.page.getByRole('tab', { name: 'Meus Reembolsos' })
  28  |   }
  29  | 
  30  |   get btnSolicitarReembolso() {
  31  |     return this.page.getByRole('button', { name: 'Solicitar Reembolso' })
  32  |   }
  33  | 
  34  |   // ── Locators — Campos gerais (/inserir-reembolso) ─────────────────
  35  | 
  36  |   get objetivo() { return this.page.locator('#objetivo') }
  37  |   get destino()  { return this.page.locator('#destino') }
  38  |   get dataInicio() { return this.page.locator('#dataInicio') }
  39  |   get dataFim()    { return this.page.locator('#dataFim') }
  40  | 
  41  |   // ── Locators — Campos de item ─────────────────────────────────────
  42  | 
  43  |   get comboboxProjeto() { return this.page.locator('button[role="combobox"]').first() }
  44  |   get uploadInput()     { return this.page.locator('input[type="file"]') }
  45  |   get uploadArea()      { return this.page.locator('label:has(input[type="file"])') }
  46  |   // Campo Quantidade: input number editável (min="1", não é readonly)
  47  |   get quantidade()      { return this.page.locator('input[type="number"]:not([readonly])') }
  48  |   // Campo Valor Total: input number somente-leitura calculado automaticamente
  49  |   get valorTotal()      { return this.page.locator('input[type="number"][readonly]') }
  50  |   get valor()           { return this.page.locator('input[placeholder="0,00"]') }
  51  |   get descricao()       { return this.page.locator('textarea') }
  52  | 
  53  |   // ── Locators — Ações ──────────────────────────────────────────────
  54  | 
  55  |   get btnAdicionarCarrinho()  { return this.page.getByRole('button', { name: 'Adicionar ao Carrinho' }) }
  56  |   get btnEnviarSolicitacoes() { return this.page.getByRole('button', { name: 'Enviar Solicitações' }) }
  57  |   get btnLimpar()             { return this.page.getByRole('button', { name: 'Limpar' }) }
  58  | 
  59  |   // ── Locators — Carrinho ───────────────────────────────────────────
  60  | 
  61  |   get carrinhoVazioTexto() {
  62  |     return this.page.getByText('Nenhuma solicitação no carrinho')
  63  |   }
  64  | 
  65  |   get itemCarrinhoLinha() {
  66  |     return this.page.locator('tbody tr, [data-testid*="item-carrinho"], .carrinho-item')
  67  |   }
  68  | 
  69  |   // ── Navegação ──────────────────────────────────────────────────────
  70  | 
  71  |   async visitarLista() {
  72  |     await this.page.goto(this.urlLista, { waitUntil: 'domcontentloaded' })
  73  |   }
  74  | 
  75  |   async visitarFormulario() {
> 76  |     await this.page.goto(this.urlFormulario, { waitUntil: 'domcontentloaded' })
      |                     ^ Error: page.goto: Protocol error (Page.navigate): Cannot navigate to invalid URL
  77  |   }
  78  | 
  79  |   // ── Ações no Dashboard ─────────────────────────────────────────────
  80  | 
  81  |   async clicarSolicitarReembolso() {
  82  |     await this.btnSolicitarReembolso.click()
  83  |   }
  84  | 
  85  |   // ── Campos gerais ──────────────────────────────────────────────────
  86  | 
  87  |   async preencherObjetivo(texto: string) {
  88  |     await this.objetivo.clear()
  89  |     await this.objetivo.fill(texto)
  90  |   }
  91  | 
  92  |   async preencherDestino(texto: string) {
  93  |     await this.destino.clear()
  94  |     await this.destino.fill(texto)
  95  |   }
  96  | 
  97  |   async selecionarDataInicio(dia: string, mes: string) {
  98  |     await this.dataInicio.click()
  99  |     await this._selecionarDiaNoCalendario(dia, mes)
  100 |   }
  101 | 
  102 |   async selecionarDataFim(dia: string, mes: string) {
  103 |     await this.dataFim.click()
  104 |     await this._selecionarDiaNoCalendario(dia, mes)
  105 |   }
  106 | 
  107 |   // ── Campos de item ─────────────────────────────────────────────────
  108 | 
  109 |   async selecionarPrimeiroProjeto() {
  110 |     await this.comboboxProjeto.click()
  111 |     await this.page
  112 |       .locator('[role="option"], [role="listbox"] li, [cmdk-item]')
  113 |       .first()
  114 |       .click({ force: true })
  115 |   }
  116 | 
  117 |   /**
  118 |    * Seleciona a primeira categoria disponível.
  119 |    * Suporta <select> nativo e combobox customizado (cmdk/radix).
  120 |    */
  121 |   async selecionarPrimeiraCategoria() {
  122 |     const selects = this.page.locator('select')
  123 |     const count   = await selects.count()
  124 | 
  125 |     if (count > 0) {
  126 |       const select  = selects.first()
  127 |       const options = select.locator('option')
  128 |       const total   = await options.count()
  129 | 
  130 |       for (let i = 0; i < total; i++) {
  131 |         const val = await options.nth(i).getAttribute('value')
  132 |         if (val && val !== '' && val !== '0') {
  133 |           await select.selectOption(val)
  134 |           return
  135 |         }
  136 |       }
  137 |       await select.selectOption({ index: 1 })
  138 |     } else {
  139 |       await this.page.locator('button[role="combobox"]').nth(1).click()
  140 |       await this.page.locator('[role="option"], [cmdk-item]').first().click({ force: true })
  141 |     }
  142 |   }
  143 | 
  144 |   /**
  145 |    * Seleciona a categoria "Km rodado" (tipoCodigo=2) para testes de cálculo automático.
  146 |    */
  147 |   async selecionarCategoriaKmRodado() {
  148 |     const selects = this.page.locator('select')
  149 |     const count   = await selects.count()
  150 | 
  151 |     if (count > 0) {
  152 |       const select  = selects.first()
  153 |       const options = select.locator('option')
  154 |       const total   = await options.count()
  155 | 
  156 |       for (let i = 0; i < total; i++) {
  157 |         const text = await options.nth(i).textContent()
  158 |         const val  = await options.nth(i).getAttribute('value')
  159 |         if (text?.toLowerCase().includes('km') && val) {
  160 |           await select.selectOption(val)
  161 |           return
  162 |         }
  163 |       }
  164 |       await select.selectOption({ index: 1 })
  165 |     } else {
  166 |       await this.page.locator('button[role="combobox"]').nth(1).click()
  167 |       const kmOpcao = this.page.locator('[role="option"], [cmdk-item]').filter({ hasText: /km/i })
  168 |       if (await kmOpcao.count() > 0) {
  169 |         await kmOpcao.first().click({ force: true })
  170 |       } else {
  171 |         await this.page.locator('[role="option"], [cmdk-item]').first().click({ force: true })
  172 |       }
  173 |     }
  174 |   }
  175 | 
  176 |   async uploadComprovante(caminhoRelativo: string) {
```