import type { MenuResource } from '@domain/entities/MenuResource'

/** Achata itens do menu (raiz + subMenus) para buscar por código. */
export function flattenMenuItems(items: MenuResource[]): MenuResource[] {
  if (!Array.isArray(items)) return []
  return items.flatMap((item) => [
    item,
    ...flattenMenuItems(Array.isArray(item.subMenus) ? item.subMenus : []),
  ])
}

/** Fallback quando o menu ainda não carregou: formata o código (ex.: agendas_comerciais → Agendas Comerciais). */
export function formatMenuCodeAsName(menuCode: string): string {
  if (!menuCode.trim()) return menuCode
  return menuCode
    .split('_')
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase())
    .join(' ')
}

/**
 * Retorna o nomeMenu do item do menu cujo codigoRecursoMenu ou codigoRecurso
 * corresponde ao menuCode (ex.: agendas_comerciais).
 */
export function getNomeMenuByMenuCode(
  menuItems: MenuResource[],
  menuCode: string
): string {
  if (!menuCode) return ''
  const flat = flattenMenuItems(Array.isArray(menuItems) ? menuItems : [])
  const code = menuCode.toLowerCase()
  const item = flat.find((i) => {
    const c = (i.codigoRecursoMenu ?? i.codigoRecurso ?? '').toLowerCase()
    return c === code
  })
  if (item?.nomeMenu && String(item.nomeMenu).trim()) {
    return String(item.nomeMenu).trim()
  }
  return formatMenuCodeAsName(menuCode)
}
