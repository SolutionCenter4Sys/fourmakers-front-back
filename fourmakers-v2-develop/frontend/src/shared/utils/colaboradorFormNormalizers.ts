import { formatarDataParaYYYYMMDD } from '@shared/utils/formatUtils'

export function normalizeNullableString(value: unknown): string {
  const raw = String(value ?? '').trim()
  if (!raw) return ''
  const lower = raw.toLowerCase()
  if (lower === 'null' || lower === 'undefined') return ''
  return raw
}

export function normalizeBoolean(value: unknown, fallback = false): boolean {
  if (typeof value === 'boolean') return value
  if (typeof value === 'number') return value === 1
  if (typeof value === 'string') {
    const normalized = value.trim().toLowerCase()
    if (normalized === 'true' || normalized === '1' || normalized === 'sim') return true
    if (normalized === 'false' || normalized === '0' || normalized === 'não' || normalized === 'nao') return false
  }
  return fallback
}

export function toDateInputValue(value: unknown): string {
  const normalized = normalizeNullableString(value)
  return normalized ? formatarDataParaYYYYMMDD(normalized) : ''
}
