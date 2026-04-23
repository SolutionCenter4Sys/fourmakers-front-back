---
name: playwright-agent
description: "Agente Playwright — executa testes E2E automatizados com login OTP real (QA Master) contra o backend de produção. Use quando o usuário disser: gerar testes, Etapa 3 da demo, automação Playwright, rodar automação."
---

# 🎬 Playwright Agent
> Especialista em automação E2E com Playwright + OTP QA Master contra backend real — Page Objects, evidências e logs ricos no terminal.

---

## O que ele faz

Executa testes E2E com Playwright usando:
- **Login OTP real** via `loginFourMakers(page, request)` — 3 etapas: `EnviaTokenAcessoEmail` → `ObtemCodigoAcessoEmailQA` (polling) → `ValidaTokenAcessoEmail` → JWT no `localStorage`
- **Servico OTP API-only do Solution Center** via `SolutionCenterAuth` para validacao e reaproveitamento de JWT em cenarios de API
- **Front local via Vite** (`npm run dev` na raiz do repo → http://localhost:8080) configurado com proxy pra API de dev
- **Page Object `ReembolsoPage`** — seletores mapeados do DOM real
- **Fixtures JSON** — dados de teste tipados
- **Evidências automáticas** — screenshots por cenário (passou/falhou)
- **Logs detalhados** — emojis + prefixos padronizados no terminal

---

## Como ativar no Cursor Chat

```
demo reembolso
```

**Essa é a frase padronizada única** para disparar a demo completa (GherkinFlow → DataForge → Playwright). Digite exatamente `demo reembolso` no chat e o orquestrador executa as 3 etapas em sequência.

> Para rodar apenas o Playwright sem os outros agentes (útil pra re-executar a automação já gerada), use o terminal: `npm run demo:run` a partir da raiz do repo. Esse comando executa a spec versionada mais recente e valida alinhamento de versão entre BDD + massa + spec.

---

## Estrutura do projeto

```
frontend/playwright-automation-template/
├── playwright.config.ts              ← config (baseURL, reporter, evidências)
├── playwright.solution-center.config.ts ← config isolada do smoke OTP API
├── package.json                      ← scripts npm para rodar os testes
├── e2e/
│   ├── support/auth/solution-center-otp.ts ← servico OTP API-only (3 passos)
│   ├── fixtures/solution-center.ts   ← fixture worker-scoped para reaproveitar JWT
│   ├── debug/solution-center-smoke.spec.ts ← smoke de autenticacao OTP (4 testes)
│   └── exemplo-autenticado.spec.ts   ← exemplo de chamada autenticada com Bearer
├── fixtures/
│   ├── reembolso.json                ← dados de teste (objetivo, datas, valores)
│   └── comprovante-teste.jpg         ← arquivo para upload
├── support/
│   ├── auth/
│   │   └── fourmakers-auth.ts        ← login OTP QA Master (3 steps + JWT)
│   └── pages/
│       └── ReembolsoPage.ts          ← Page Object com locators e ações
├── tests/
│   └── reembolso/
│       ├── reembolso.spec.ts         ← testes unitários por campo/ação
│       ├── reembolso-jornada.spec.ts ← jornada completa (11 cenários BDD)
│       ├── reembolso-demo.spec.ts    ← happy path para apresentação (< 60s)
│       └── reembolso-jornada-unico.spec.ts
└── evidencias/                       ← screenshots e relatórios gerados
```

---

## Inputs que ele consome

| Arquivo | Para que serve |
|---|---|
| `frontend/playwright-automation-template/support/auth/fourmakers-auth.ts` | **Login OTP QA Master** (credenciais fixas, endpoint real) |
| `frontend/playwright-automation-template/e2e/support/auth/solution-center-otp.ts` | **Servico OTP Solution Center** para cenarios API-only com JWT reutilizavel |
| `frontend/playwright-automation-template/e2e/fixtures/solution-center.ts` | **Fixture worker-scoped** para compartilhar JWT por worker |
| `frontend/playwright-automation-template/e2e/debug/solution-center-smoke.spec.ts` | **Smoke OTP** dos 3 passos + validacao do Bearer |
| `frontend/playwright-automation-template/support/pages/ReembolsoPage.ts` | **Page Object** — locators e ações mapeados do DOM |
| `frontend/playwright-automation-template/fixtures/reembolso.json` | **Dados de teste** — objetivo, datas, valores, descrição |
| `frontend/DEMO/ui-elements/reembolso-ui.json` | Referência dos seletores reais capturados da UI |
| `frontend/DEMO/cenarios-bdd/REEMBOLSO-BDD-vN.md` | IDs e estrutura dos cenários BDD |

---

## Fluxo de autenticação OTP QA Master

```
loginFourMakers(page, request, appUrl?)
  │
  ├─ Step 1: POST /api/Acesso/EnviaTokenAcessoEmail
  │          → Dispara OTP para o e-mail cadastrado
  │          (Bearer com o segredo RAW do ambiente)
  │
  ├─ Step 2: GET /api/Acesso/ObtemCodigoAcessoEmailQA
  │          → Polling (12 tentativas × 400ms) para recuperar código
  │          (Bearer Base64Url("{orgId}|{segredo}"))
  │
  ├─ Step 3: POST /api/Acesso/ValidaTokenAcessoEmail
  │          → Troca código pelo JWT de sessão
  │
  └─ Injeção: page.goto(appUrl) → localStorage.setItem('authToken', jwt)
              + localStorage.setItem('lastOrgId', orgId)
              → reload() → waitForURL(!login)
```

> **Backend dev:** `https://spw.app.foursys.com/backoffice-rf-hom`
> **Front local (Vite):** `http://localhost:8080` (default do `npm run demo`)
> **Usuário padrão:** `gustavo.queiroz@foursys.com.br` (orgId 8)
> **Credenciais fixas** — NÃO editar sem autorização do time QA.
> **Ref. completa:** `frontend/DEMO/integracao-acesso-qa/`

---

## Smoke OTP Solution Center (obrigatorio no roteiro da demonstracao)

Antes de apresentar a jornada E2E no navegador, execute o smoke API-only para validar autenticacao:

```bash
cd fourmakers-v2-develop/frontend/playwright-automation-template
npm run test:solution-center:smoke
```

Checklist minimo para seguir com a demo:
- `4 passed` no final da execucao
- payload JWT com `Email=solutioncenter@foursys.com.br`
- payload JWT com `OrgId=8`
- claim `exp` no futuro

Esse smoke cobre os 3 passos oficiais da autenticacao OTP:
1. `POST /api/Acesso/EnviaTokenAcessoEmail`
2. `GET /api/Acesso/ObtemCodigoAcessoEmailQA`
3. `POST /api/Acesso/ValidaTokenAcessoEmail`

---

## Specs disponíveis

### `reembolso-demo.spec.ts` — Happy Path para apresentação

Script otimizado para demo (~11s). Cobre:
- R-01 · Dashboard com aba "Meus Reembolsos" e botão Solicitar
- R-06 · Navegação para /inserir-reembolso
- F-01 · Formulário "Nova Solicitação" com todos os campos estruturais

> **Nota:** os cenários adicionais do `reembolso-jornada.spec.ts` (Km rodado, carrinho, envio) dependem de **verbas configuradas** no backend pro usuário. O Gustavo hoje não tem verbas, por isso o happy path da demo para no formulário aberto. Para rodar a jornada completa, peça ao time pra configurar verbas de reembolso pro usuário da automação.

### `reembolso-jornada.spec.ts` — Jornada completa (11 cenários)

| ID | Tipo | Cenário |
|:---:|:---:|---|
| R-01 | Positivo | Dashboard — aba padrão "Meus Reembolsos" |
| R-06 | Positivo | Navegação para nova solicitação |
| I-01 | Positivo | Adicionar item ao carrinho |
| I-02 | Positivo | Cálculo automático (Km rodado) |
| I-03 | Positivo | Edição de item restaura campos |
| I-04 | Positivo | Envio gera solicitação e redireciona |
| I-06 | Positivo | Remoção de item do carrinho |
| I-07 | Negativo | Campos obrigatórios vazios bloqueiam |
| I-08 | Negativo | Comprovante obrigatório sem arquivo |
| I-09 | Negativo | Carrinho vazio impede envio |
| I-10 | Negativo | Formato de arquivo inválido rejeitado |

### `reembolso.spec.ts` — Testes isolados por campo

Testes individuais para cada campo e ação do formulário — usados para validação granular.

---

## Exemplo de teste com OTP + Page Object

```typescript
test('🚀 Demo — Jornada Reembolso (Happy Path)', async ({ page, request }) => {
  test.setTimeout(120_000)

  const p      = new ReembolsoPage(page)
  const inicio = Date.now()

  LOG.banner('DEMO FOURMAKERS — MÓDULO REEMBOLSO')
  LOG.passo('Autenticando via OTP...')
  await loginFourMakers(page, request)
  LOG.ok('Autenticado com sucesso')

  LOG.secao('R-01', 'Positivo', 'Dashboard — aba "Meus Reembolsos"')
  await p.visitarLista()
  await expect(page.getByRole('tab', { name: 'Meus Reembolsos' })).toBeVisible()
  LOG.ok('Dashboard carregado com aba visível')

  LOG.secao('I-01', 'Positivo', 'Preencher formulário e adicionar ao carrinho')
  await p.visitarFormulario()
  await p.preencherObjetivo(dados.reembolsoValido.objetivo)
  await p.preencherDestino(dados.reembolsoValido.destino)
  await p.selecionarPrimeiroProjeto()
  await p.selecionarPrimeiraCategoria()
  await p.clicarAdicionarCarrinho()
  await p.verificarItemNoCarrinho()
  LOG.ok('Item adicionado ao carrinho com sucesso')
})
```

---

## Prioridade de seletores (ReembolsoPage)

| Prioridade | Seletor | Quando usar |
|:---:|---|---|
| 1 | `page.locator('#id')` | Elemento com id HTML (`#objetivo`, `#destino`) |
| 2 | `page.getByRole('...', { name: '...' })` | Botão/tab com texto (`Solicitar Reembolso`, `Meus Reembolsos`) |
| 3 | `page.locator('button[role="combobox"]')` | Combobox customizado (Radix/cmdk) |
| 4 | `page.locator('input[type="file"]')` | Input de upload |
| 5 | `page.getByText('...')` | Texto visível único na tela |

---

## Para rodar

### Instalação (primeira vez)

```bash
# Raiz do repo — instala o front
cd fourmakers-v2-develop
npm install

# Template Playwright
cd playwright-automation-template
npm install
npx playwright install chromium
```

### Pré-requisitos

**`.env.local`** no `fourmakers-v2-develop/` com:
```
VITE_API_PROXY_TARGET=https://spw.app.foursys.com/backoffice-rf-hom
VITE_API_FOURMAKERS_URL=
```

### Comando único (recomendado)

Na raiz do repo:

```bash
cd fourmakers-v2-develop
npm run demo            # sobe Vite → roda Playwright → desliga Vite (~50s total)
npm run demo:headed     # idem, com navegador visível pra apresentação
```

### Scripts manuais (dentro do template)

```bash
# Opção: rodar separado (precisa ter Vite rodando antes em localhost:8080)
cd fourmakers-v2-develop/frontend/playwright-automation-template

# Demo versionada (usa automaticamente o ultimo vN comum entre BDD + DataForge + spec)
npm run demo

# Com navegador visível
npm run demo:headed

# Smoke OTP do Solution Center (API-only)
npm run test:solution-center:smoke

# Todos os testes
npm test

# Jornada completa (11 cenários)
npm run test:jornada

# Testes isolados por campo
npm run test:reembolso

# Todos os specs de reembolso
npm run test:reembolso:all

# Com UI interativa
npm run test:ui

# Modo headed (navegador visível)
npm run test:headed

# Ver relatório HTML
npm run relatorio
```

### Execução direta (sem npm scripts)

```bash
# Demo headed
npx playwright test tests/reembolso/reembolso-demo.spec.ts --headed

# Jornada completa
npx playwright test tests/reembolso/reembolso-jornada.spec.ts --reporter=list

# Todos com reporter list
npx playwright test --reporter=list
```

---

## Logs no terminal

| Momento | Formato |
|---------|---------|
| Banner | `╔══ DEMO FOURMAKERS — MÓDULO REEMBOLSO ══╗` |
| Seção | `┌─ R-01 · [Positivo] Dashboard ─┐` |
| Auth inicio | `🔐 AUTENTICAÇÃO: Login FourMakers via OTP` |
| Auth step | `   → [Step 1] Enviando OTP para o e-mail cadastrado...` |
| Auth ok | `   ✅ JWT obtido com sucesso` |
| Teste início | `🧪 TESTANDO: R-01 — Aba padrão "Meus Reembolsos"` |
| Passo | `   → Navegando para /reembolso...` |
| Aprovado | `   ✅ Dashboard carregado com aba visível` |
| Evidência | `   📸 [EVIDÊNCIA FINAL] evidencias/screenshots/reembolso-jornada/passou--r01.png` |
| Tempo | `   ⏱  Tempo total: 45.2s` |
| Fim | `╚══ ✅ DEMO CONCLUÍDA — TODOS OS CENÁRIOS APROVADOS ══╝` |

---

## Evidências

Screenshots geradas automaticamente em `evidencias/screenshots/`:
- `{passou|falhou}--{titulo-do-teste}.png` — captura final de cada teste
- `{id}-{nome-acao}.png` — capturas intermediárias durante a jornada

Relatório HTML gerado em `evidencias/relatorios/` (abrir com `npm run relatorio`).

---

## Diferenças do Cypress (migração concluída)

| Cypress | Playwright |
|---------|-----------|
| `cy.session('id', () => cy.loginFourMakers())` | `await loginFourMakers(page, request)` no `beforeEach` |
| `cy.visit('/rota')` | `await page.goto('/rota')` |
| `cy.get('#id').type('valor')` | `await page.locator('#id').fill('valor')` |
| `cy.contains('Texto').click()` | `await page.getByText('Texto').click()` |
| `cy.intercept()` + `cy.wait('@alias')` | `await page.route()` + `await page.waitForResponse()` |
| `reembolso.cy.js` (JS) | `reembolso.spec.ts` (TypeScript) |
| Seletores inline | Page Object `ReembolsoPage` |
| Sem evidências automáticas | Screenshots + relatório HTML |

---

## Arquivo de definição completo

> `frontend/playwright-automation-template/` — projeto independente com `package.json` próprio

