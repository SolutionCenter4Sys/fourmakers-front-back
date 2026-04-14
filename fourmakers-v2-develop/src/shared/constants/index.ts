// Constantes compartilhadas entre camadas
const CONFIGURED_API_BASE_URL = import.meta.env.VITE_API_FOURMAKERS_URL || ''

function isLocalhost(): boolean {
  if (typeof window === 'undefined') return false
  const hostname = window.location.hostname
  return hostname === 'localhost' || hostname === '127.0.0.1' || hostname === '::1'
}

// Se VITE_API_FOURMAKERS_URL estiver definido, usar sempre (mesmo em dev)
// Caso contrário, em dev/local usamos URLs relativas (ex: `/api/...`) para permitir proxy do Vite
// Em produção, sempre usar CONFIGURED_API_BASE_URL (que deve estar definido)
export const API_BASE_URL = CONFIGURED_API_BASE_URL 
  ? CONFIGURED_API_BASE_URL 
  : (import.meta.env.DEV && isLocalhost() ? '' : CONFIGURED_API_BASE_URL)

// XANO API
export { XANO_BASE_URL, getXanoHeaders, getXanoDataSource } from '../utils/xanoUtils'

// Skill type constants
export { TIPO_SKILL_MAP, SKILL_TYPE_LABELS } from './skillTypes'
