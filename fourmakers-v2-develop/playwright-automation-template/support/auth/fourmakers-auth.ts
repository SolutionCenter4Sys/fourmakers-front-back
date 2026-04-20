import { Page, APIRequestContext } from '@playwright/test'

/**
 * FourMakers — Login via OTP (backoffice-rf-hom / dev + front local)
 *
 * ⚠️  CREDENCIAIS FIXAS — não altere os valores abaixo sem autorização do time QA.
 *
 * Como funciona:
 *   - O deploy backend `spw.app.foursys.com/backoffice-rf-hom` é onde o Gustavo
 *     foi provisionado. Esse deploy tem o módulo de Reembolso via
 *     `/api/Financeiro/Reembolso/*` (compartilhado com a FourMakers).
 *   - O front-end é servido localmente pelo Vite (`npm run dev` → http://localhost:8080)
 *     configurado com `VITE_API_PROXY_TARGET` apontando pro backend acima
 *     (ver .env.local na raiz do repo). Isso evita CORS: o browser chama
 *     `/api/*` relativo, o Vite intercepta e repassa pro backend.
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
 * Pré-requisito:
 *   - Front local rodando: `npm run dev` na raiz do repo (porta 8080)
 *   - `.env.local` com VITE_API_PROXY_TARGET=https://spw.app.foursys.com/backoffice-rf-hom
 */

// ─── Credenciais — NÃO EDITAR ────────────────────────────────────────────────
const FM = {
  apiBase: 'https://spw.app.foursys.com/backoffice-rf-hom',
  email:   'gustavo.queiroz@foursys.com.br',
  orgId:   8,
  // Token Base64Url("{orgId}|{segredo}") — usado no Step 2 e Step 3
  tokenBase64: 'OHw1WUI0MEl6Y0I3eDgzV3NGWUswcUNpb0c2aTNsRmhQM3FsWWJDaXJ6bWM5OTVLdEI4Qg==',
  // Segredo RAW — exigido como Bearer do Step 1 neste deploy
  segredoRaw:  '5YB40IzcB7x83WsFYK0qCioG6i3lFhP3qlYbCirzmc995KtB8B',
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
      headers: { Authorization: `Bearer ${FM.tokenBase64}` },
      timeout: 30_000,
    },
  )

  const body   = await res.json().catch(() => ({}))
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
 *
 * @param page    Instância da página Playwright
 * @param request Contexto de requisições Playwright
 * @param appUrl  URL da aplicação onde o JWT será injetado
 *                (default: http://localhost:8080 — front local com proxy pro spw)
 */
export async function loginFourMakers(
  page: Page,
  request: APIRequestContext,
  appUrl = 'http://localhost:8080',
): Promise<void> {
  console.log('\n──────────────────────────────────────────────')
  console.log('🔐 AUTENTICAÇÃO: Login FourMakers via OTP')
  console.log(`   usuário: ${FM.email}`)
  console.log(`   orgId:   ${FM.orgId}`)
  console.log(`   apiBase: ${FM.apiBase}`)
  console.log(`   appUrl:  ${appUrl}`)
  console.log('──────────────────────────────────────────────')

  // ── Step 1: solicitar OTP ───────────────────────────────────────────────────
  console.log('   → [Step 1] Enviando OTP para o e-mail cadastrado...')
  const res1 = await request.post(
    `${FM.apiBase}/api/Acesso/EnviaTokenAcessoEmail`,
    {
      data: { email: FM.email, orgId: FM.orgId },
      headers: {
        'Content-Type': 'application/json',
        // Este deploy exige Bearer com o segredo RAW no Step 1
        Authorization:  `Bearer ${FM.segredoRaw}`,
      },
      timeout: 30_000,
    },
  )
  const body1 = await res1.json().catch(() => ({} as Record<string, unknown>))
  if (!res1.ok() || !body1.sucesso) {
    throw new Error(
      `[Step 1] falhou — status=${res1.status()} body=${JSON.stringify(body1)}`,
    )
  }
  if (body1.tipoAcesso === 'SSO') {
    throw new Error('[Step 1] organização está em SSO — fluxo OTP não disponível')
  }
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
      data: { email: FM.email, token: codigo, orgId: FM.orgId },
      headers: {
        'Content-Type': 'application/json',
        Authorization:  `Bearer ${FM.tokenBase64}`,
      },
      timeout: 30_000,
    },
  )
  const body3 = await res3.json().catch(() => ({} as Record<string, unknown>))
  if (!res3.ok() || !body3.sucesso) {
    throw new Error(
      `[Step 3] falhou — status=${res3.status()} body=${JSON.stringify(body3)}`,
    )
  }
  const jwt = body3.token as string | undefined
  if (!jwt) {
    throw new Error('[Step 3] JWT ausente na resposta de sucesso')
  }
  console.log('   ✅ JWT obtido com sucesso')

  // ── Injetar JWT na aplicação ────────────────────────────────────────────────
  // 1. Carrega a origem para tornar o localStorage acessível
  // 2. Injeta o JWT + lastOrgId (o front lê ambos no bootstrap)
  // 3. reload() (não goto) preserva o localStorage; domcontentloaded evita timeout em SPAs lentas
  // 4. Aguarda URL sair de /login (redirecionamento pós-auth da SPA)
  console.log(`   → Injetando JWT no localStorage de ${appUrl}...`)
  await page.goto(appUrl, { waitUntil: 'domcontentloaded', timeout: 30_000 })
  await page.evaluate(
    ({ token, orgId }) => {
      localStorage.setItem('authToken', token)
      localStorage.setItem('lastOrgId', String(orgId))
    },
    { token: jwt, orgId: FM.orgId },
  )
  await page.reload({ waitUntil: 'domcontentloaded', timeout: 30_000 })
  await page.waitForURL(/(?!.*\/login)/, { timeout: 25_000 }).catch(() => {
    // não falha se demorar — alguns testes validam URL depois
  })

  console.log('   ✅ Sessão iniciada com sucesso')
  console.log('──────────────────────────────────────────────\n')
}
