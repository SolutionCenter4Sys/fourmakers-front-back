/**
 * Utilitários usados apenas pela feature permissionamento.
 * Evita dependência de hooks/utils de outras features (ex.: gestaoVagas).
 */

/**
 * Formata valor de data para exibição em pt-BR.
 */
export const formatDatePtBr = (dateValue?: string, fallback = 'Não informada'): string => {
  if (!dateValue) return fallback;
  try {
    const date = new Date(dateValue);
    if (Number.isNaN(date.getTime())) return dateValue;
    return date.toLocaleDateString('pt-BR');
  } catch {
    return dateValue;
  }
};
