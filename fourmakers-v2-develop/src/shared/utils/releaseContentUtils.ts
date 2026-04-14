import { flutterflowRouteMap } from '@shared/utils/flutterflowRouteMap';

const STORAGE_PREFIX = 'release_visto_';

/**
 * Obtém o código do recurso no menu a partir do pathname da rota.
 * Ex.: "/agendas-comerciais" -> "agendas_comerciais"
 */
export function getMenuCodeByPath(pathname: string): string | null {
  const pathNormalized = pathname.replace(/^\//, '').trim() || '';
  if (!pathNormalized) return null;

  for (const [menuCode, routeInfo] of Object.entries(flutterflowRouteMap)) {
    if (routeInfo.path === pathNormalized) return menuCode;
    // Suporta rotas que começam com o path (ex.: /agendas-comerciais/123)
    if (pathNormalized.startsWith(routeInfo.path + '/')) return menuCode;
  }
  return null;
}

/**
 * Chave de localStorage para marcar que o usuário já viu o release desta tela (por menuCode).
 */
export function getReleaseStorageKey(menuCode: string): string {
  return `${STORAGE_PREFIX}${menuCode}`;
}

/**
 * Verifica se o release desta tela já foi marcado como visto.
 */
export function wasReleaseSeen(menuCode: string): boolean {
  try {
    return localStorage.getItem(getReleaseStorageKey(menuCode)) === '1';
  } catch {
    return false;
  }
}

/**
 * Marca o release desta tela como visto (para não exibir novamente).
 */
export function markReleaseAsSeen(menuCode: string): void {
  try {
    localStorage.setItem(getReleaseStorageKey(menuCode), '1');
  } catch {
    // ignore
  }
}
