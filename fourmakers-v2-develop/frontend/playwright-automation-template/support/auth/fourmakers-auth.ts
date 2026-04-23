import { Page, APIRequestContext } from '@playwright/test'
import { SolutionCenterAuth } from '../../e2e/support/auth/solution-center-otp'

const SC = {
  apiBase: 'https://spw.app.foursys.com/backoffice-rf-hom',
  email: 'solutioncenter@foursys.com.br',
  orgId: 8,
} as const

/**
 * Login oficial da suíte usando o novo fluxo Solution Center OTP.
 * Mantém a injeção de chaves de sessão esperadas pelo front local.
 */
export async function loginSolutionCenter(
  page: Page,
  request: APIRequestContext,
  appUrl = 'http://localhost:8080',
): Promise<void> {
  console.log('\n──────────────────────────────────────────────')
  console.log('🔐 AUTENTICAÇÃO: Login Solution Center via OTP')
  console.log(`   usuário: ${SC.email}`)
  console.log(`   orgId:   ${SC.orgId}`)
  console.log(`   apiBase: ${SC.apiBase}`)
  console.log(`   appUrl:  ${appUrl}`)
  console.log('──────────────────────────────────────────────')

  console.log('   → [Step 1..3] Executando fluxo OTP Solution Center...')
  const auth = new SolutionCenterAuth(request)
  const jwt = await auth.autenticarViaOtp()
  console.log('   ✅ JWT obtido com sucesso')

  console.log(`   → Injetando JWT no localStorage de ${appUrl}...`)
  await page.goto(appUrl, { waitUntil: 'commit', timeout: 60_000 })
  await page.waitForLoadState('domcontentloaded', { timeout: 60_000 }).catch(() => {})
  await page.evaluate(
    ({ token, orgId }) => {
      localStorage.setItem('authToken', token)
      localStorage.setItem('token', token)
      localStorage.setItem('@app:token', token)
      localStorage.setItem('lastOrgId', String(orgId))
    },
    { token: jwt, orgId: SC.orgId },
  )

  try {
    await page.reload({ waitUntil: 'commit', timeout: 60_000 })
    await page.waitForLoadState('domcontentloaded', { timeout: 60_000 }).catch(() => {})
  } catch (error) {
    const mensagem = error instanceof Error ? error.message : String(error)
    if (mensagem.includes('ERR_ABORTED') || mensagem.includes('frame was detached')) {
      console.log('   ⚠️  reload interrompido; aplicando fallback com goto...')
      await page.goto(appUrl, { waitUntil: 'commit', timeout: 60_000 })
      await page.waitForLoadState('domcontentloaded', { timeout: 60_000 }).catch(() => {})
    } else {
      throw error
    }
  }

  await page.waitForURL(/(?!.*\/login)/, { timeout: 25_000 }).catch(() => {})
  console.log('   ✅ Sessão iniciada com sucesso')
  console.log('──────────────────────────────────────────────\n')
}

/**
 * Alias de compatibilidade para specs legadas que ainda importam `loginFourMakers`.
 */
export async function loginFourMakers(
  page: Page,
  request: APIRequestContext,
  appUrl = 'http://localhost:8080',
): Promise<void> {
  await loginSolutionCenter(page, request, appUrl)
}
