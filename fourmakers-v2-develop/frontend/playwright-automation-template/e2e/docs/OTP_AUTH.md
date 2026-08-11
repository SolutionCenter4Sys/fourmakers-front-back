# OTP Auth — automação Playwright (HML/QA)

Contorno do **rate limit** de `EnviaTokenAcessoEmail`: a suite **não** dispara OTP em cada teste.

## Fluxo correto

1. `POST /api/Acesso/EnviaTokenAcessoEmail` `{ email, orgId }` — **só quando necessário**
2. `GET /api/Acesso/ObtemCodigoAcessoEmailQA` com `OTP_SYSTEM_TOKEN` — sem abrir e-mail
3. `POST /api/Acesso/ValidaTokenAcessoEmail` — JWT
4. Injeta JWT em `localStorage` e grava `storageState`

## Contorno A — Reuso de sessão (preferido, UI/E2E)

- Setup: `tests/_setup/demo-auth.setup.ts`
- Arquivo: `evidencias/.auth/demo-usuario.json`
- Project `chromium` usa `storageState` (depende de `setup`)
- **Default:** `E2E_REUSE_SESSION=true` — se o arquivo existir, JWT não expirou e idade &lt; max, **pula OTP**

| Env | Default | Efeito |
|-----|---------|--------|
| `E2E_REUSE_SESSION` | `true` | Reusa storageState entre runs |
| `E2E_REUSE_SESSION_MAX_AGE_MIN` | `30` | Idade máxima do arquivo (min) |
| `E2E_FORCE_AUTH` / `BFF_FORCE_REFRESH` | off | Força novo OTP no setup |

## Contorno B — Cache JWT (API)

```bash
npm run auth:mint          # reusa cache se válido
npm run auth:mint -- --force
```

- Cache: `e2e/.bff-token-cache.json` (gitignored)
- Ou `BFF_TOKEN=<jwt>` no ambiente

## Contorno C — Credenciais

| Env | Descrição |
|-----|-----------|
| `OTP_API_BASE_URL` | Backend HML (ex. `https://spw.app.foursys.com/backoffice-rf-hom`) |
| `OTP_EMAIL` | Usuário QA dedicado (não e-mail pessoal compartilhado) |
| `OTP_ORG_ID` | Org do login (Showcase = `5`) |
| `OTP_SYSTEM_TOKEN` | Bearer QA — **só** `.env` / CI secrets |

Fallbacks locais alinhados a `DEMO/setup/demo-credenciais.json` (pré-setup sincroniza). Em CI, **obrigatório** secret `OTP_SYSTEM_TOKEN`.

## Contorno D — Rate limit já estourou

1. **Não** ficar reenviando OTP
2. Esperar janela do backend
3. Usar storageState/cache válido (`E2E_REUSE_SESSION`) ou usuário HML próprio
4. Rodar de novo sem `--force`

## Anti-padrões (proibidos)

- OTP em `beforeEach` / por teste
- Workers paralelos no mesmo `OTP_EMAIL`
- Loop de “reenviar código” após 429 / “Limite de tentativas”
- Ler OTP da inbox

## Arquivos

| Path | Papel |
|------|--------|
| `e2e/support/auth/solution-center-otp.ts` | Cliente OTP (env + rate-limit fail-fast) |
| `e2e/support/auth/session-reuse.ts` | Decisão de reuso JWT/storageState |
| `tests/_setup/demo-auth.setup.ts` | Setup Playwright (1 OTP/run ou 0 se reuso) |
| `e2e/scripts/mint-bff-token.mjs` | Mint/cache JWT |
| `playwright.config.ts` | projects `setup` + `chromium` + `storageState` |
