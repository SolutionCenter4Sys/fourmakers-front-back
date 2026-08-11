# Demo Reembolso — fluxo atualizado

Esteira E2E de pré-vendas no Cursor (**uma demo só**, ~2–4 min no Playwright):

```
🛠️ Pré-setup → 🐛 Bug manual (opcional) → 🎭 Gherkin → 🧪 DataForge → 🎬 Playwright → 🔴 Self-heal
```

Tudo roda no chat. Você prepara o ambiente, cola o bug (opcional) e digita o comando.

---

## Visão rápida do fluxo

| Fase | Comando | O que acontece | OTP? |
|------|---------|----------------|------|
| **1. Pré-setup** | `demo preparar reembolso` | Deps, Vite `:8080`, smoke | **1×** (smoke) |
| **2. Bug** | Colar `DEMO/bug-self-healing/Reembolso.tsx` | Rota inválida p/ R-04 | — |
| **3. Esteira** | `demo reembolso` | BDD enxuto → massa → 3–4 testes headed | **0–1×** (reuso de sessão) |
| **3b. Self-heal** | após falha + decisão `GO` | Playwright pede GO/NOGO; com GO, Dev corrige → reexecuta R-04 | reusa sessão |

### Volume enxuto (obrigatório)

| Etapa | Volume |
|-------|--------|
| GherkinFlow | **8–12** cenários no total (foco R-01, R-04, I-08, I-13) |
| DataForge | constantes dos cenários executáveis (~4+) |
| Playwright | **3–4 testes** (R-01, R-04, I-08, I-13); resto `test.skip` |

### Auth (sem rate limit)

- Specs **não** logam no `beforeEach`
- Project `setup` grava `evidencias/.auth/demo-usuario.json` (`storageState`)
- `E2E_REUSE_SESSION=true` (default): se JWT válido → **pula** `EnviaTokenAcessoEmail`
- Doc: `frontend/playwright-automation-template/e2e/docs/OTP_AUTH.md`

---

## Passo 1 — Preparar o ambiente (ANTES do cliente)

Na raiz `fourmakers-v2-develop`, no Cursor Chat:

```text
demo preparar reembolso
```

Ou no terminal:

```bash
npm run demo:preparar
```

O script faz sozinho:

1. Valida instalação na máquina (Node/npm/disco/porta/estrutura)
2. Sync credenciais OTP (`usuario_qa@foursys.com.br` / orgId `5`)
3. Cria/atualiza `frontend/.env.local`
4. Instala deps (**raiz + frontend + Playwright**) + Chromium
5. Valida pós-install (vite, @playwright/test, Chromium)
6. Limpa artefatos versionados antigos (BDD / data / specs)
7. Sobe o front Vite em `http://localhost:8080`
8. Valida backend + proxy (**sem** consumir OTP)
9. Roda smoke OTP (retenta se rate limit)

Banner final esperado:

- Smoke OTP: `ok`
- Front: `UP :8080`
- Verificações: `0 falha`

> Se o front cair depois: `npm run demo:front`  
> Status: `npm run demo:front:status` · Parar: `npm run demo:front:parar`

### Auth / rate limit OTP

| Env / comando | Uso |
|---------------|-----|
| `E2E_REUSE_SESSION` | `true` (default) reusa sessão entre runs |
| `E2E_REUSE_SESSION_MAX_AGE_MIN` | idade máx. do storageState (default `30`) |
| `E2E_FORCE_AUTH=1` | força OTP novo no setup |
| `npm run auth:mint` (em `playwright-automation-template`) | cache JWT API |

---

## Passo 2 — Injetar o bug (opcional, self-healing)

Faça **você mesma**, fora do chat, antes da apresentação:

1. Abra `DEMO/bug-self-healing/Reembolso.tsx`
2. `Ctrl+A` → `Ctrl+C`
3. Abra `frontend/src/presentation/pages/Reembolso.tsx`
4. `Ctrl+A` → `Ctrl+V` → salve

Conferir:

```bash
node DEMO/scripts/demo-reembolso-bug.cjs status
```

Deve mostrar bug ativo com rota `/inserir-reembolso-invalido`.

Isso quebra o cenário **R-04** (botão Solicitar Reembolso). Na esteira: Playwright falha → mostra diagnóstico/evidências → pede **GO/NOGO**. Só com **GO** ativa Dev FourBlox, corrige e reexecuta.

Detalhes: `DEMO/bug-self-healing/README.md`

---

## Passo 3 — Rodar a demo ao vivo

No Cursor Chat (Vite já no ar pelo pré-setup):

```text
demo reembolso
```

Sem pausas. Orquestrador ativa cada agente:

| Etapa | Agente | Gera / faz |
|-------|--------|------------|
| 1 | GherkinFlow | `DEMO/outputs/gherkinflow/reembolso/REEMBOLSO-BDD-vN.md` (**8–12** cenários) |
| 2 | DataForge | `DEMO/outputs/dataforge/reembolso/reembolso.data.vN.js` |
| 3 | Playwright | Spec enxuta + Chromium headed (`SLOWMO=150`) + `storageState` |
| 3b | Self-healing | Se falhar, Playwright pede **GO/NOGO**; com GO corrige app e reexecuta **só** o cenário |

Execução Playwright (por baixo):

```text
setup (OTP 0–1×) → R-01 → R-04 → se falhar: GO/NOGO → I-08 → I-13
```

Ao final: banners no chat + HTML em  
`DEMO/outputs/playwright/reembolso/relatorios/demo-reembolso-vN.html`

---

## Passo 4 — Mostrar evidências

1. Trio versionado com o mesmo `vN` (BDD + massa + spec)
2. Screenshots em `DEMO/outputs/playwright/reembolso/screenshots/`
3. Relatório HTML consolidado
4. Se usou bug: falha R-04 → correção Dev FourBlox → reteste verde
5. No log: `♻️ Reusando storageState` (runs seguintes) ou `🔐 Login OTP` (primeira vez)

---

## Comandos rápidos

| Ação | Chat | Terminal |
|------|------|----------|
| Pré-setup | `demo preparar reembolso` | `npm run demo:preparar` |
| Só checks de install | — | `npm run demo:install-check` |
| Demo completa | `demo reembolso` | — |
| Status artefatos | — | `npm run demo:status` |
| Manifesto / fontes | — | `npm run demo:modulo` |
| Front sobe / status / para | — | `npm run demo:front` · `demo:front:status` · `demo:front:parar` |
| Bug status / fix | — | `node DEMO/scripts/demo-reembolso-bug.cjs status` · `fix` |
| Mint JWT (cache) | — | `npm --prefix frontend/playwright-automation-template run auth:mint` |
| Showcase só 1 agente | `demo gherkinflow` · `demo dataforge` · `demo playwright` | — |

---

## Estrutura da pasta DEMO

```
DEMO/
├── README.md                 ← este guia (fluxo atualizado)
├── bug-self-healing/         ← código com bug pra colar (Ctrl+A/V)
├── setup/                    ← credenciais + estado do pré-setup
├── modulos/                  ← manifesto reembolso (fontes pré-selecionadas)
├── scripts/                  ← demo:preparar, vite, env-check, self-healing…
├── inputs/ui-elements/       ← reembolso-ui.json (seletores)
├── inputs/integracao-acesso-qa/  ← docs OTP
├── outputs/
│   ├── gherkinflow/reembolso/ ← BDD versionado da Etapa 1
│   ├── dataforge/reembolso/   ← massa versionada da Etapa 2
│   └── playwright/reembolso/  ← specs, relatórios, screenshots e test-results
└── agentes/                  ← elenco da demo (QA + Dev + orquestrador)
```

Playwright (auth):

```
frontend/playwright-automation-template/
├── tests/_setup/demo-auth.setup.ts   ← OTP 0–1× + storageState
├── e2e/support/auth/session-reuse.ts ← E2E_REUSE_SESSION
├── e2e/docs/OTP_AUTH.md              ← doc rate limit
└── evidencias/.auth/demo-usuario.json
```

Skills: `DEMO/agentes/<agente>/<agente>.md` (orquestrador, gherkinflow, dataforge, playwright, self-healing) + `dev-fourblox/bridge.md`  
Stubs Cursor: `.cursor/skills/<agente>/SKILL.md`

Credenciais oficiais: `DEMO/setup/demo-credenciais.json`

---

## Troubleshooting

| Sintoma | O que fazer |
|---------|-------------|
| Vite fora do ar | `npm run demo:front` — log em `DEMO/setup/vite-dev.log` |
| Smoke OTP rate limit | Aguarde ~2 min; pré-setup retenta 3×. Ou `--sem-smoke` + reuso de sessão depois |
| Rate limit na demo | Confirme `E2E_REUSE_SESSION=true`; use storageState existente; **não** force OTP em loop |
| Dashboard em branco | Confirme `VITE_FLUTTERFLOW_BASE_URL` no `.env.local` |
| CORS / API | `VITE_API_PROXY_TARGET=https://spw.app.foursys.com/backoffice-rf-hom` |
| Usuário não encontrado | Org **5** (Showcase), e-mail `usuario_qa@foursys.com.br` |
| Diagnóstico geral | `node DEMO/scripts/demo-env-check.cjs` |

---

## Regras importantes

- **Não** rode `npm install` na frente do cliente — isso é pré-setup.
- **Não** injete bug pela esteira — cole o arquivo de `bug-self-healing/` você mesma.
- **Não** gere dezenas de cenários BDD — volume enxuto (8–12).
- **Não** chame OTP por teste — só setup / reuso de `storageState`.
- Sem SQL fictício / banco fake — massa entra pelo formulário real no backend hom.
- Framework da demo: **Playwright** (não Cypress).
- Versões: cada run cria um trio novo `vN` (BDD + data + spec). Nunca sobrescreve.
