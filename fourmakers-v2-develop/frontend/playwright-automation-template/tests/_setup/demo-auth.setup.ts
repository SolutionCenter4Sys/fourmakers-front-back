import { test as setup } from '@playwright/test'
import * as fs from 'node:fs'
import * as path from 'node:path'
import { SolutionCenterAuth } from '../../e2e/support/auth/solution-center-otp'
import { decideSessionReuse } from '../../e2e/support/auth/session-reuse'

/**
 * Setup de autenticação da demo — no máximo 1 OTP por execução.
 * Com E2E_REUSE_SESSION=true (default), reusa storageState se JWT ainda válido.
 * Specs herdam a sessão via `storageState` — nunca logam no beforeEach.
 */
const APP_URL = process.env.PLAYWRIGHT_BASE_URL || 'http://localhost:8080'
const ORG_ID = Number(process.env.OTP_ORG_ID || process.env.DEMO_ORG_ID || 5)
const authFile = path.resolve(__dirname, '../../evidencias/.auth/demo-usuario.json')

setup('autenticar demo (OTP único ou reuso)', async ({ page, request }) => {
  const decision = decideSessionReuse(authFile)

  if (decision.reuse) {
    const expLabel = decision.exp
      ? new Date(decision.exp * 1000).toLocaleString('pt-BR')
      : 'n/a'
    console.log('\n♻️  [setup] Reusando storageState — SEM EnviaTokenAcessoEmail')
    console.log(`   motivo: ${decision.reason}`)
    console.log(`   arquivo: ${path.relative(process.cwd(), authFile)}`)
    console.log(`   JWT exp: ${expLabel}\n`)
    return
  }

  console.log('\n🔐 [setup] Login OTP necessário — reaproveitado por todos os testes desta execução...')
  console.log(`   motivo: ${decision.reason}`)

  const auth = new SolutionCenterAuth(request)
  let jwt: string
  try {
    jwt = await auth.autenticarViaOtp()
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error)
    if (/limite de tentativas|rate limit|429/i.test(message)) {
      throw new Error(
        `[setup] Rate limit OTP — NÃO reenviar.\n` +
          `Aguarde a janela do backend ou reutilize sessão:\n` +
          `  E2E_REUSE_SESSION=true (default) com evidencias/.auth/demo-usuario.json válido\n` +
          `  ou npm run auth:mint (cache JWT)\n` +
          `Detalhe: ${message}`,
      )
    }
    throw error
  }
  console.log('   ✅ JWT obtido via OTP')

  await page.goto(APP_URL, { waitUntil: 'commit', timeout: 60_000 })
  await page.waitForLoadState('domcontentloaded', { timeout: 60_000 }).catch(() => {})
  await page.evaluate(
    ({ token, orgId }) => {
      localStorage.setItem('authToken', token)
      localStorage.setItem('token', token)
      localStorage.setItem('@app:token', token)
      localStorage.setItem('lastOrgId', String(orgId))
    },
    { token: jwt, orgId: ORG_ID },
  )

  fs.mkdirSync(path.dirname(authFile), { recursive: true })
  await page.context().storageState({ path: authFile })
  console.log(`   ✅ [setup] storageState salvo: ${path.relative(process.cwd(), authFile)}\n`)
})
