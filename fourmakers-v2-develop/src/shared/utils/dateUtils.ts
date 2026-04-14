/**
 * Converte string ISO da API em Date.
 * Quando a API envia data sem timezone (ex: "2026-02-25T16:32:51"), interpreta como UTC
 * para que o tempo relativo seja calculado corretamente no fuso do usuário.
 */
export function parseApiDate(isoString: string | null | undefined): Date | null {
  if (isoString == null || isoString === '') return null;
  const trimmed = isoString.trim();
  if (!trimmed) return null;
  // Se já tem indicador de timezone (Z ou + ou - no final), deixa o Date interpretar
  if (/[Zz+-]\d{2}:?\d{2}$/.test(trimmed) || trimmed.endsWith('Z') || trimmed.endsWith('z')) {
    const d = new Date(trimmed);
    return Number.isNaN(d.getTime()) ? null : d;
  }
  // Sem timezone: API geralmente envia em UTC
  const d = new Date(`${trimmed.replace(/Z$/i, '')}Z`);
  return Number.isNaN(d.getTime()) ? null : d;
}
