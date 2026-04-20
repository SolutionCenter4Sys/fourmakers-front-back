# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: frontend\playwright-automation-template\tests\reembolso\versionadas\reembolso-demo-v9.spec.ts >> R-01 [Positivo] Visão padrão do módulo
- Location: frontend\playwright-automation-template\tests\reembolso\versionadas\reembolso-demo-v9.spec.ts:92:5

# Error details

```
Test timeout of 30000ms exceeded while running "beforeEach" hook.
```

# Test source

```ts
  1   | import { test, expect } from '@playwright/test'
  2   | import { ReembolsoPage } from '../../../support/pages/ReembolsoPage'
  3   | import { loginFourMakers } from '../../../support/auth/fourmakers-auth'
  4   | 
  5   | // ─── Dados do DataForge (Etapa 2 da demo) ──────────────────────────────────
  6   | // Cada constante corresponde 1:1 com um cenário do BDD (REEMBOLSO-BDD-v9.md).
  7   | // Gerada automaticamente pelo agente DataForge a partir do BDD + código-fonte.
  8   | import {
  9   |   dadosR01,
  10  |   dadosR09,
  11  |   dadosR12,
  12  |   dadosR13,
  13  |   dadosI08,
  14  |   dadosI11,
  15  |   dadosI12,
  16  |   dadosI13,
  17  |   dadosI15,
  18  |   // Os demais (dadosR02, R03, R10, R11, R04-R08, R14-R15, I01-I07, I09, I10, I14)
  19  |   // estão disponíveis, mas dependem de setup que o usuário padrão da demo não tem
  20  |   // (perfil de gestor/aprovador, reembolsos cadastrados, verbas configuradas).
  21  |   // Marcados como skip abaixo com razão explícita.
  22  |   // @ts-ignore — os imports não usados servem como referência do catálogo completo
  23  |   dadosR02, dadosR03, dadosR04, dadosR05, dadosR06, dadosR07, dadosR08,
  24  |   // @ts-ignore
  25  |   dadosR10, dadosR11, dadosR14, dadosR15,
  26  |   // @ts-ignore
  27  |   dadosI01, dadosI02, dadosI03, dadosI04, dadosI05, dadosI06, dadosI07,
  28  |   // @ts-ignore
  29  |   dadosI09, dadosI10, dadosI14,
  30  | // @ts-ignore — import JS sem types
  31  | } from '../../../../DEMO/automacao/reembolso/versionadas/reembolso.data.v9.js'
  32  | 
  33  | /**
  34  |  * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  35  |  *  DEMO REEMBOLSO — Jornada E2E (Happy Path + Negativos)
  36  |  * ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  37  |  *
  38  |  *  Esta spec é o output da Etapa 3 (Playwright Agent) da demo pré-vendas.
  39  |  *  Cada `test()` mapeia 1:1 um cenário do arquivo REEMBOLSO-BDD-v9.md e
  40  |  *  usa as constantes geradas pelo DataForge em reembolso.data.v9.js.
  41  |  *
  42  |  *  INPUTS DA ETAPA 3 (consumidos abaixo):
  43  |  *    📄 DEMO/cenarios-bdd/REEMBOLSO-BDD-v9.md    → IDs e nomes dos cenários
  44  |  *    📄 DEMO/automacao/reembolso/versionadas/reembolso.data.v9.js → constantes dadosRxx / dadosIxx
  45  |  *    📄 DEMO/ui-elements/reembolso-ui.json       → seletores refletidos no Page Object
  46  |  *
  47  |  *  Pré-requisitos:
  48  |  *    1. Front local: `npm run dev` na raiz (Vite em http://localhost:8080)
  49  |  *    2. Rede Foursys (backend dev: spw.app.foursys.com/backoffice-rf-hom)
  50  |  */
  51  | 
  52  | test.describe.configure({ retries: 0 })
  53  | 
  54  | const LOG = {
  55  |   banner: (msg: string) => {
  56  |     const linha = '═'.repeat(54)
  57  |     console.log(`\n╔${linha}╗`)
  58  |     console.log(`║  ${msg.padEnd(53)}║`)
  59  |     console.log(`╚${linha}╝`)
  60  |   },
  61  |   secao: (codigo: string, tipo: string, titulo: string) => {
  62  |     console.log(`\n┌─────────────────────────────────────────────────────`)
  63  |     console.log(`│ ${codigo} · [${tipo}]  ${titulo}`)
  64  |     console.log(`└─────────────────────────────────────────────────────`)
  65  |   },
  66  |   passo:  (msg: string) => console.log(`   → ${msg}`),
  67  |   ok:     (msg: string) => console.log(`   ✅ ${msg}`),
  68  |   alerta: (msg: string) => console.log(`   ⚠️  ${msg}`),
  69  | }
  70  | 
  71  | test.beforeAll(async () => {
  72  |   LOG.banner('DEMO FOURMAKERS — MÓDULO REEMBOLSO')
  73  | })
  74  | 
> 75  | test.beforeEach(async ({ page, request }, testInfo) => {
      |      ^ Test timeout of 30000ms exceeded while running "beforeEach" hook.
  76  |   console.log(`\n🧪 TESTANDO: ${testInfo.title}`)
  77  |   await loginFourMakers(page, request)
  78  | })
  79  | 
  80  | test.afterEach(async ({}, testInfo) => {
  81  |   console.log(`🏁 ${testInfo.title} CONCLUÍDO`)
  82  | })
  83  | 
  84  | test.afterAll(async () => {
  85  |   console.log('╚══ ✅ DEMO CONCLUÍDA ══╝')
  86  | })
  87  | 
  88  | // ═════════════════════════════════════════════════════════════════════
  89  | //  DASHBOARD — Reembolso.tsx (/reembolso)
  90  | // ═════════════════════════════════════════════════════════════════════
  91  | 
  92  | test('R-01 [Positivo] Visão padrão do módulo', async ({ page }) => {
  93  |   const p = new ReembolsoPage(page)
  94  |   LOG.secao('R-01', 'Positivo', 'Visão padrão do módulo de Reembolso')
  95  | 
  96  |   LOG.passo(`Validando que apenas a aba "${dadosR01.abaEsperada}" está visível...`)
  97  |   await p.visitarLista()
  98  |   await expect(p.tabMeusReembolsos).toBeVisible()
  99  |   LOG.ok(`Aba "${dadosR01.abaEsperada}" visível`)
  100 | 
  101 |   for (const abaRestrita of dadosR01.abasRestritas) {
  102 |     const aba = page.getByRole('tab', { name: abaRestrita })
  103 |     if (await aba.count() > 0) {
  104 |       LOG.alerta(`Aba "${abaRestrita}" presente no DOM — usuário tem perfil elevado`)
  105 |     } else {
  106 |       LOG.ok(`Aba "${abaRestrita}" ausente (como esperado)`)
  107 |     }
  108 |   }
  109 |   await p.evidencia('r01-visao-padrao')
  110 | })
  111 | 
  112 | test('R-09 [Positivo] Início de nova solicitação', async ({ page }) => {
  113 |   const p = new ReembolsoPage(page)
  114 |   LOG.secao('R-09', 'Positivo', 'Início de nova solicitação')
  115 | 
  116 |   LOG.passo('Navegando para /reembolso...')
  117 |   await p.visitarLista()
  118 |   await expect(p.btnSolicitarReembolso).toBeVisible()
  119 | 
  120 |   LOG.passo(`Clicando no botão "${dadosR09.botao}"...`)
  121 |   await p.clicarSolicitarReembolso()
  122 | 
  123 |   LOG.passo(`Validando redirecionamento para ${dadosR09.urlDestino}...`)
  124 |   await expect(page).toHaveURL(new RegExp(dadosR09.urlDestino))
  125 |   await expect(page.getByText(/Nova Solicita/i)).toBeVisible()
  126 |   LOG.ok('Redirecionamento para formulário de nova solicitação')
  127 |   await p.evidencia('r09-formulario-aberto')
  128 | })
  129 | 
  130 | test('R-12 [Negativo] Busca sem resultados', async ({ page }) => {
  131 |   const p = new ReembolsoPage(page)
  132 |   LOG.secao('R-12', 'Negativo', 'Busca sem resultados')
  133 | 
  134 |   await p.visitarLista()
  135 | 
  136 |   LOG.passo(`Informando termo de busca: "${dadosR12.termoBusca}"...`)
  137 |   const inputBusca = page.getByPlaceholder(/busca/i).first()
  138 |   await expect(inputBusca).toBeVisible()
  139 |   await inputBusca.fill(dadosR12.termoBusca)
  140 | 
  141 |   LOG.passo(`Validando mensagem: "${dadosR12.mensagemEsperada}"...`)
  142 |   await expect(page.getByText(new RegExp(dadosR12.mensagemEsperada, 'i'))).toBeVisible()
  143 |   LOG.ok('Mensagem de estado vazio apresentada')
  144 |   await p.evidencia('r12-busca-vazia')
  145 | })
  146 | 
  147 | test('R-13 [Regressivo] Limpeza dos filtros de data', async ({ page }) => {
  148 |   const p = new ReembolsoPage(page)
  149 |   LOG.secao('R-13', 'Regressivo', 'Limpeza dos filtros de data')
  150 | 
  151 |   await p.visitarLista()
  152 | 
  153 |   LOG.passo(`Aplicando filtro de período: ${dadosR13.dataInicio} → ${dadosR13.dataFim}...`)
  154 |   const btnLimpar = page.getByRole('button', { name: new RegExp(dadosR13.botaoLimpar, 'i') })
  155 | 
  156 |   if (await btnLimpar.isVisible().catch(() => false)) {
  157 |     LOG.passo(`Acionando botão "${dadosR13.botaoLimpar}"...`)
  158 |     await btnLimpar.click()
  159 |     await expect(btnLimpar).not.toBeVisible({ timeout: 5_000 }).catch(() => {})
  160 |     LOG.ok('Filtros de data limpos com sucesso')
  161 |   } else {
  162 |     LOG.alerta('Filtros não estão aplicados no momento — botão "Limpar" ausente (comportamento esperado quando sem filtros)')
  163 |   }
  164 |   await p.evidencia('r13-filtros-limpos')
  165 | })
  166 | 
  167 | // ═════════════════════════════════════════════════════════════════════
  168 | //  INSERIR REEMBOLSO — InserirReembolso.tsx (/inserir-reembolso)
  169 | // ═════════════════════════════════════════════════════════════════════
  170 | 
  171 | test('I-08 [Negativo] Submissão com campos obrigatórios vazios', async ({ page }) => {
  172 |   const p = new ReembolsoPage(page)
  173 |   LOG.secao('I-08', 'Negativo', 'Submissão com campos obrigatórios vazios')
  174 | 
  175 |   await p.visitarFormulario()
```