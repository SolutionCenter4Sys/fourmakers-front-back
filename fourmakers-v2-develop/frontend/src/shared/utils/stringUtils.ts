/**
 * Normaliza texto para comparação em buscas: minúsculas, trim e sem acentos.
 * Uso: autocomplete, filtros (ex.: Command/combobox).
 */
export function normalizarParaBusca(texto: string | null | undefined): string {
  if (texto == null) return '';
  return String(texto)
    .trim()
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '');
}
