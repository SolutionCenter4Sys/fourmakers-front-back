import { Page, APIRequestContext } from '@playwright/test'

/**
 * FourMakers — Login via OTP (Produção)
 * Migrado de: cypress/support/commands/fourmakers-auth.ts
 *
 * ⚠️  CREDENCIAIS FIXAS — não altere os valores abaixo.
 *     Qualquer mudança de email, orgId ou token deve ser
 *     solicitada ao time FourMakers QA.
 *
 * Fluxo (3 etapas obrigatórias e sequenciais):
 *   1. EnviaTokenAcessoEmail    → dispara o OTP para o e-mail cadastrado
 *   2. ObtemCodigoAcessoEmailQA → recupera o código via endpoint QA (com polling)
 *   3. ValidaTokenAcessoEmail   → troca o código pelo JWT de sessão
 *
 * Uso no teste:
 *   test.beforeEach(async ({ page, request }) => {
 *     await loginFourMakers(page, request)
 *   })
 *
 * Uso com storageState (recomendado para suítes grandes):
 *   Ver: playwright.config.ts → projects[].use.storageState
 */

// ─── Credenciais — NÃO EDITAR ────────────────────────────────────────────────
const FM = {
  apiBase: 'https://api.fourmakers.io',
  email:   'solutioncenter@foursys.com.br',
  orgId:   8,
  token:   'Mnx5Y0R3dzNtd3hqaFFuSVVVekVVUHhNdDl2OVowVHNKMnlnclJMOFhWRlB4aGRnT0IxcQ==',
} as const
// ─────────────────────────────────────────────────────────────────────────────

/**
 * Polling para ObtemCodigoAcessoEmailQA.
 * Tenta até 12 vezes com intervalo de 400 ms (~5 s no total).
 */
async function obtemCodigoComPolling(
  request: APIRequestContext,
  tentativas = 12,
): Promise<string> {
  const res = await request.get(
    `${FM.apiBase}/api/Acesso/ObtemCodigoAcessoEmailQA`,
    {
      params:  { email: FM.email, orgId: FM.orgId },
      headers: { Authorization: `Bearer ${FM.token}` },
    },
  )

  const body   = await res.json()
  const codigo: string =
    body.codigo ?? body.codigoAcesso ?? body.token ?? body.code ?? ''

  if (res.ok() && body.sucesso && codigo) {
    return codigo
  }

  if (tentativas <= 0) {
    throw new Error(
      `[FourMakers] ObtemCodigoAcessoEmailQA: código não encontrado após 5 s.\n` +
      `Última resposta: ${JSON.stringify(body)}`,
    )
  }

  await new Promise(resolve => setTimeout(resolve, 400))
  return obtemCodigoComPolling(request, tentativas - 1)
}

/**
 * Realiza login via OTP e injeta o JWT no localStorage da aplicação.
 * @param page       Instância da página Playwright
 * @param request    Contexto de requisições Playwright
 * @param appUrl     URL da aplicação onde o JWT será injetado
 */
export async function loginFourMakers(
  page: Page,
  request: APIRequestContext,
  appUrl = 'https://app.fourmakers.io',
): Promise<void> {
  console.log('\n──────────────────────────────────────────────')
  console.log('🔐 AUTENTICAÇÃO: Login FourMakers via OTP')
  console.log('──────────────────────────────────────────────')

  // ── Step 1: solicitar OTP ───────────────────────────────────────────────────
  console.log('   → [Step 1] Enviando OTP para o e-mail cadastrado...')
  const res1 = await request.post(
    `${FM.apiBase}/api/Acesso/EnviaTokenAcessoEmail`,
    {
      data:    { email: FM.email, orgId: FM.orgId },
      headers: { 'Content-Type': 'application/json' },
    },
  )
  const body1 = await res1.json()
  if (!body1.sucesso)           throw new Error('[Step 1] sucesso deve ser true')
  if (body1.tipoAcesso === 'SSO') throw new Error('[Step 1] organização não pode usar SSO')
  console.log('   ✅ OTP enviado com sucesso')

  // ── Step 2: recuperar código via QA bypass ──────────────────────────────────
  console.log('   → [Step 2] Recuperando código OTP via endpoint QA (polling)...')
  const codigo = await obtemCodigoComPolling(request)
  console.log('   ✅ Código OTP obtido')

  // ── Step 3: trocar código pelo JWT ──────────────────────────────────────────
  console.log('   → [Step 3] Validando OTP e obtendo JWT de sessão...')
  const res3 = await request.post(
    `${FM.apiBase}/api/Acesso/ValidaTokenAcessoEmail`,
    {
      data:    { email: FM.email, token: codigo, orgId: FM.orgId },
      headers: { 'Content-Type': 'application/json' },
    },
  )
  const body3 = await res3.json()
  if (!body3.sucesso) throw new Error('[Step 3] autenticação deve retornar sucesso=true')
  if (!body3.token)   throw new Error('[Step 3] JWT deve estar presente')

  const jwt: string = body3.token
  console.log('   ✅ JWT obtido com sucesso')

  // ── Injetar JWT na aplicação ────────────────────────────────────────────────
  // 1. Carrega a origem para tornar o localStorage acessível
  // 2. Injeta o JWT
  // 3. reload() (não goto) preserva o localStorage; domcontentloaded evita timeout em SPAs lentas
  // 4. Aguarda URL sair de /login (redirecionamento pós-auth da SPA)
  console.log('   → Injetando JWT no localStorage da aplicação...')
  await page.goto(appUrl, { waitUntil: 'domcontentloaded', timeout: 30_000 })
  await page.evaluate((token) => {
    localStorage.setItem('authToken', token)
  }, jwt)
  await page.reload({ waitUntil: 'domcontentloaded', timeout: 30_000 })
  await page.waitForURL(/(?!.*\/login)/, { timeout: 25_000 })

  console.log('   ✅ Sessão iniciada com sucesso')
  console.log('──────────────────────────────────────────────\n')
}
