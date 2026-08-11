/**
 * Helpers de reuso de sessão E2E — evita EnviaTokenAcessoEmail quando JWT/storageState ainda vale.
 *
 * Regra: OTP só quando sessão inválida/expirada (ou force). Idade do arquivo NÃO invalida JWT válido.
 */
import * as fs from 'node:fs'
import * as path from 'node:path'

export const DEFAULT_AUTH_STATE = path.resolve(
  __dirname,
  '../../../evidencias/.auth/demo-usuario.json',
)

export type JwtPayload = {
  exp?: number
  nbf?: number
  email?: string
  OrgId?: string
  [key: string]: unknown
}

export function isReuseSessionEnabled(): boolean {
  const raw = (process.env.E2E_REUSE_SESSION ?? 'true').trim().toLowerCase()
  return raw === '1' || raw === 'true' || raw === 'yes'
}

export function isForceAuthRefresh(): boolean {
  const flags = [process.env.E2E_FORCE_AUTH, process.env.BFF_FORCE_REFRESH]
  return flags.some((v) => {
    const raw = (v ?? '').trim().toLowerCase()
    return raw === '1' || raw === 'true' || raw === 'yes'
  })
}

/** Idade máxima do arquivo — só informativa quando JWT ainda é válido. */
export function reuseMaxAgeMs(): number {
  const minutes = Number(process.env.E2E_REUSE_SESSION_MAX_AGE_MIN || 30)
  if (!Number.isFinite(minutes) || minutes <= 0) return 30 * 60_000
  return minutes * 60_000
}

/** Decodifica payload JWT sem verificar assinatura (só lê exp). */
export function decodeJwtPayload(token: string): JwtPayload | null {
  try {
    const parts = token.split('.')
    if (parts.length < 2) return null
    const b64 = parts[1].replace(/-/g, '+').replace(/_/g, '/')
    const padded = b64 + '='.repeat((4 - (b64.length % 4)) % 4)
    const json = Buffer.from(padded, 'base64').toString('utf8')
    return JSON.parse(json) as JwtPayload
  } catch {
    return null
  }
}

export function isJwtUsable(token: string, skewSeconds = 120): boolean {
  const payload = decodeJwtPayload(token)
  if (!payload?.exp) return false
  const now = Math.floor(Date.now() / 1000)
  return payload.exp > now + skewSeconds
}

export function extractAuthTokenFromStorageState(filePath: string): string | null {
  if (!fs.existsSync(filePath)) return null
  try {
    const state = JSON.parse(fs.readFileSync(filePath, 'utf8')) as {
      origins?: Array<{ localStorage?: Array<{ name: string; value: string }> }>
    }
    for (const origin of state.origins || []) {
      for (const item of origin.localStorage || []) {
        if (['authToken', 'token', '@app:token'].includes(item.name) && item.value) {
          return item.value
        }
      }
    }
  } catch {
    return null
  }
  return null
}

export type SessionReuseDecision = {
  reuse: boolean
  reason: string
  token?: string
  exp?: number
  ageMs?: number
}

/**
 * Decide se o setup pode pular OTP e reusar o storageState em disco.
 * Prioridade: JWT válido → reusa. Não dispara EnviaToken só porque o arquivo é “velho”.
 */
export function decideSessionReuse(authFile = DEFAULT_AUTH_STATE): SessionReuseDecision {
  if (isForceAuthRefresh()) {
    return { reuse: false, reason: 'E2E_FORCE_AUTH / BFF_FORCE_REFRESH ativo' }
  }
  if (!isReuseSessionEnabled()) {
    return { reuse: false, reason: 'E2E_REUSE_SESSION desligado' }
  }
  if (!fs.existsSync(authFile)) {
    return { reuse: false, reason: 'storageState ausente' }
  }

  const ageMs = Date.now() - fs.statSync(authFile).mtimeMs
  const token = extractAuthTokenFromStorageState(authFile)
  if (!token) {
    return { reuse: false, reason: 'storageState sem authToken', ageMs }
  }
  if (!isJwtUsable(token)) {
    const payload = decodeJwtPayload(token)
    return {
      reuse: false,
      reason: 'JWT expirado ou próximo do exp',
      token,
      exp: payload?.exp,
      ageMs,
    }
  }

  const payload = decodeJwtPayload(token)
  const maxAge = reuseMaxAgeMs()
  const ageNote =
    ageMs > maxAge
      ? ` (arquivo ${Math.round(ageMs / 60_000)} min > max ${Math.round(maxAge / 60_000)} — JWT ainda válido, OTP não disparado)`
      : ''

  return {
    reuse: true,
    reason: `storageState válido (JWT OK)${ageNote}`,
    token,
    exp: payload?.exp,
    ageMs,
  }
}
