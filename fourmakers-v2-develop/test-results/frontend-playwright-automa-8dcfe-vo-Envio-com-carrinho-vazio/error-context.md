# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: frontend\playwright-automation-template\tests\reembolso\versionadas\reembolso-demo-v9.spec.ts >> I-12 [Negativo] Envio com carrinho vazio
- Location: frontend\playwright-automation-template\tests\reembolso\versionadas\reembolso-demo-v9.spec.ts:221:5

# Error details

```
Error: [FourMakers] ObtemCodigoAcessoEmailQA: código não encontrado após 5 s.
Última resposta: {"sucesso":false,"mensagem":"Nenhum código de acesso ativo encontrado para este e-mail","erros":null}
```

# Test source

```ts
  1   | import { Page, APIRequestContext } from '@playwright/test'
  2   | 
  3   | /**
  4   |  * FourMakers — Login via OTP (backoffice-rf-hom / dev + front local)
  5   |  *
  6   |  * ⚠️  CREDENCIAIS FIXAS — não altere os valores abaixo sem autorização do time QA.
  7   |  *
  8   |  * Como funciona:
  9   |  *   - O deploy backend `spw.app.foursys.com/backoffice-rf-hom` é onde o Gustavo
  10  |  *     foi provisionado. Esse deploy tem o módulo de Reembolso via
  11  |  *     `/api/Financeiro/Reembolso/*` (compartilhado com a FourMakers).
  12  |  *   - O front-end é servido localmente pelo Vite (`npm run dev` → http://localhost:8080)
  13  |  *     configurado com `VITE_API_PROXY_TARGET` apontando pro backend acima
  14  |  *     (ver .env.local na raiz do repo). Isso evita CORS: o browser chama
  15  |  *     `/api/*` relativo, o Vite intercepta e repassa pro backend.
  16  |  *
  17  |  * Fluxo (3 etapas obrigatórias e sequenciais):
  18  |  *   1. EnviaTokenAcessoEmail    → dispara o OTP para o e-mail cadastrado
  19  |  *   2. ObtemCodigoAcessoEmailQA → recupera o código via endpoint QA (com polling)
  20  |  *   3. ValidaTokenAcessoEmail   → troca o código pelo JWT de sessão
  21  |  *
  22  |  * Uso no teste:
  23  |  *   test.beforeEach(async ({ page, request }) => {
  24  |  *     await loginFourMakers(page, request)
  25  |  *   })
  26  |  *
  27  |  * Pré-requisito:
  28  |  *   - Front local rodando: `npm run dev` na raiz do repo (porta 8080)
  29  |  *   - `.env.local` com VITE_API_PROXY_TARGET=https://spw.app.foursys.com/backoffice-rf-hom
  30  |  */
  31  | 
  32  | // ─── Credenciais — NÃO EDITAR ────────────────────────────────────────────────
  33  | const FM = {
  34  |   apiBase: 'https://spw.app.foursys.com/backoffice-rf-hom',
  35  |   email:   'gustavo.queiroz@foursys.com.br',
  36  |   orgId:   8,
  37  |   // Token Base64Url("{orgId}|{segredo}") — usado no Step 2 e Step 3
  38  |   tokenBase64: 'OHw1WUI0MEl6Y0I3eDgzV3NGWUswcUNpb0c2aTNsRmhQM3FsWWJDaXJ6bWM5OTVLdEI4Qg==',
  39  |   // Segredo RAW — exigido como Bearer do Step 1 neste deploy
  40  |   segredoRaw:  '5YB40IzcB7x83WsFYK0qCioG6i3lFhP3qlYbCirzmc995KtB8B',
  41  | } as const
  42  | // ─────────────────────────────────────────────────────────────────────────────
  43  | 
  44  | /**
  45  |  * Polling para ObtemCodigoAcessoEmailQA.
  46  |  * Tenta até 12 vezes com intervalo de 400 ms (~5 s no total).
  47  |  */
  48  | async function obtemCodigoComPolling(
  49  |   request: APIRequestContext,
  50  |   tentativas = 12,
  51  | ): Promise<string> {
  52  |   const res = await request.get(
  53  |     `${FM.apiBase}/api/Acesso/ObtemCodigoAcessoEmailQA`,
  54  |     {
  55  |       params:  { email: FM.email, orgId: FM.orgId },
  56  |       headers: { Authorization: `Bearer ${FM.tokenBase64}` },
  57  |       timeout: 30_000,
  58  |     },
  59  |   )
  60  | 
  61  |   const body   = await res.json().catch(() => ({}))
  62  |   const codigo: string =
  63  |     body.codigo ?? body.codigoAcesso ?? body.token ?? body.code ?? ''
  64  | 
  65  |   if (res.ok() && body.sucesso && codigo) {
  66  |     return codigo
  67  |   }
  68  | 
  69  |   if (tentativas <= 0) {
> 70  |     throw new Error(
      |           ^ Error: [FourMakers] ObtemCodigoAcessoEmailQA: código não encontrado após 5 s.
  71  |       `[FourMakers] ObtemCodigoAcessoEmailQA: código não encontrado após 5 s.\n` +
  72  |       `Última resposta: ${JSON.stringify(body)}`,
  73  |     )
  74  |   }
  75  | 
  76  |   await new Promise(resolve => setTimeout(resolve, 400))
  77  |   return obtemCodigoComPolling(request, tentativas - 1)
  78  | }
  79  | 
  80  | /**
  81  |  * Realiza login via OTP e injeta o JWT no localStorage da aplicação.
  82  |  *
  83  |  * @param page    Instância da página Playwright
  84  |  * @param request Contexto de requisições Playwright
  85  |  * @param appUrl  URL da aplicação onde o JWT será injetado
  86  |  *                (default: http://localhost:8080 — front local com proxy pro spw)
  87  |  */
  88  | export async function loginFourMakers(
  89  |   page: Page,
  90  |   request: APIRequestContext,
  91  |   appUrl = 'http://localhost:8080',
  92  | ): Promise<void> {
  93  |   console.log('\n──────────────────────────────────────────────')
  94  |   console.log('🔐 AUTENTICAÇÃO: Login FourMakers via OTP')
  95  |   console.log(`   usuário: ${FM.email}`)
  96  |   console.log(`   orgId:   ${FM.orgId}`)
  97  |   console.log(`   apiBase: ${FM.apiBase}`)
  98  |   console.log(`   appUrl:  ${appUrl}`)
  99  |   console.log('──────────────────────────────────────────────')
  100 | 
  101 |   // ── Step 1: solicitar OTP ───────────────────────────────────────────────────
  102 |   console.log('   → [Step 1] Enviando OTP para o e-mail cadastrado...')
  103 |   const res1 = await request.post(
  104 |     `${FM.apiBase}/api/Acesso/EnviaTokenAcessoEmail`,
  105 |     {
  106 |       data: { email: FM.email, orgId: FM.orgId },
  107 |       headers: {
  108 |         'Content-Type': 'application/json',
  109 |         // Este deploy exige Bearer com o segredo RAW no Step 1
  110 |         Authorization:  `Bearer ${FM.segredoRaw}`,
  111 |       },
  112 |       timeout: 30_000,
  113 |     },
  114 |   )
  115 |   const body1 = await res1.json().catch(() => ({} as Record<string, unknown>))
  116 |   if (!res1.ok() || !body1.sucesso) {
  117 |     throw new Error(
  118 |       `[Step 1] falhou — status=${res1.status()} body=${JSON.stringify(body1)}`,
  119 |     )
  120 |   }
  121 |   if (body1.tipoAcesso === 'SSO') {
  122 |     throw new Error('[Step 1] organização está em SSO — fluxo OTP não disponível')
  123 |   }
  124 |   console.log('   ✅ OTP enviado com sucesso')
  125 | 
  126 |   // ── Step 2: recuperar código via QA bypass ──────────────────────────────────
  127 |   console.log('   → [Step 2] Recuperando código OTP via endpoint QA (polling)...')
  128 |   const codigo = await obtemCodigoComPolling(request)
  129 |   console.log('   ✅ Código OTP obtido')
  130 | 
  131 |   // ── Step 3: trocar código pelo JWT ──────────────────────────────────────────
  132 |   console.log('   → [Step 3] Validando OTP e obtendo JWT de sessão...')
  133 |   const res3 = await request.post(
  134 |     `${FM.apiBase}/api/Acesso/ValidaTokenAcessoEmail`,
  135 |     {
  136 |       data: { email: FM.email, token: codigo, orgId: FM.orgId },
  137 |       headers: {
  138 |         'Content-Type': 'application/json',
  139 |         Authorization:  `Bearer ${FM.tokenBase64}`,
  140 |       },
  141 |       timeout: 30_000,
  142 |     },
  143 |   )
  144 |   const body3 = await res3.json().catch(() => ({} as Record<string, unknown>))
  145 |   if (!res3.ok() || !body3.sucesso) {
  146 |     throw new Error(
  147 |       `[Step 3] falhou — status=${res3.status()} body=${JSON.stringify(body3)}`,
  148 |     )
  149 |   }
  150 |   const jwt = body3.token as string | undefined
  151 |   if (!jwt) {
  152 |     throw new Error('[Step 3] JWT ausente na resposta de sucesso')
  153 |   }
  154 |   console.log('   ✅ JWT obtido com sucesso')
  155 | 
  156 |   // ── Injetar JWT na aplicação ────────────────────────────────────────────────
  157 |   // 1. Carrega a origem para tornar o localStorage acessível
  158 |   // 2. Injeta o JWT + lastOrgId (o front lê ambos no bootstrap)
  159 |   // 3. reload() (não goto) preserva o localStorage; domcontentloaded evita timeout em SPAs lentas
  160 |   // 4. Aguarda URL sair de /login (redirecionamento pós-auth da SPA)
  161 |   console.log(`   → Injetando JWT no localStorage de ${appUrl}...`)
  162 |   await page.goto(appUrl, { waitUntil: 'commit', timeout: 60_000 })
  163 |   await page.waitForLoadState('domcontentloaded', { timeout: 60_000 }).catch(() => {
  164 |     // Algumas páginas do app podem atrasar o DOMContentLoaded no primeiro boot.
  165 |     // O token pode ser injetado após o commit da navegação.
  166 |   })
  167 |   await page.evaluate(
  168 |     ({ token, orgId }) => {
  169 |       localStorage.setItem('authToken', token)
  170 |       localStorage.setItem('lastOrgId', String(orgId))
```