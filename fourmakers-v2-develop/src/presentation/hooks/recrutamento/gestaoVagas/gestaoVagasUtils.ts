export type UnknownRecord = Record<string, unknown>;

export const safeText = (value: unknown): string => {
  if (value === null || value === undefined) return '';
  if (typeof value === 'string') return value;
  if (typeof value === 'number' || typeof value === 'boolean') return String(value);
  try {
    return JSON.stringify(value);
  } catch {
    return String(value);
  }
};

export const toCount = (value: unknown): number => {
  if (typeof value === 'number') return Number.isFinite(value) ? value : 0;
  if (typeof value === 'string') {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
  }
  if (Array.isArray(value)) return value.length;
  if (value && typeof value === 'object') {
    const values = Object.values(value as Record<string, unknown>);
    const numericValues = values
      .map((v) => (typeof v === 'number' ? v : typeof v === 'string' ? Number(v) : NaN))
      .filter((n) => Number.isFinite(n));
    if (numericValues.length > 0) return numericValues.reduce((sum, n) => sum + n, 0);
  }
  return 0;
};

export const normalizeCountMap = (value: unknown): Record<string, number> => {
  if (!value) return {};
  if (typeof value === 'object' && !Array.isArray(value)) {
    const record = value as Record<string, unknown>;
    return Object.fromEntries(Object.entries(record).map(([k, v]) => [k, toCount(v)]));
  }
  return {};
};

export const getStatusIdFromVaga = (vaga: unknown): string => {
  if (!vaga || typeof vaga !== 'object') return '';
  const record = vaga as UnknownRecord;
  const candidates = [
    record.statusId,
    record.statusVagaCod,
    record.statusVagaCodigo,
    record.statusVagaId,
    record.statusVaga,
    record.codigoStatus,
    record.codigoStatusVaga,
    record.statusCodigo,
    record.status,
    record.idStatus,
    record.codStatus,
  ];
  for (const c of candidates) {
    const txt = safeText(c);
    if (txt) return txt;
  }
  return '';
};

export const firstNonEmpty = (record: UnknownRecord, keys: string[]): string => {
  for (const key of keys) {
    const value = safeText(record[key]);
    if (value) return value;
  }
  return '';
};

/** Retorna a descrição do item cujo id coincide (para listas TipoVaga, TipoContratacao, Unidade). */
export function getDescricaoById<T extends { id: string | number; descricao: string }>(
  list: T[],
  id: string | number | null | undefined
): string {
  if (id == null || !Array.isArray(list)) return '';
  const found = list.find((item) => item.id == id || String(item.id) === String(id));
  return found?.descricao ?? '';
}

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

/** Converte data ISO para valor de input type="date" (yyyy-MM-dd). */
export const toInputDate = (iso: string | null | undefined): string => {
  if (!iso) return '';
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return '';
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${y}-${m}-${day}`;
};

/**
 * Converte o valor de `<input type="date">` (YYYY-MM-DD) em ISO preservando o dia civil no fuso local.
 * `new Date("YYYY-MM-DD")` usa meia-noite UTC e, ao exibir com getDate() local, pode cair no dia anterior.
 */
export const dateInputValueToIsoLocal = (value: string): string | null => {
  const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value.trim());
  if (!m) return null;
  const y = parseInt(m[1], 10);
  const mo = parseInt(m[2], 10) - 1;
  const d = parseInt(m[3], 10);
  if (mo < 0 || mo > 11 || d < 1 || d > 31) return null;
  const dt = new Date(y, mo, d, 12, 0, 0, 0);
  if (Number.isNaN(dt.getTime()) || dt.getFullYear() !== y || dt.getMonth() !== mo || dt.getDate() !== d) {
    return null;
  }
  return dt.toISOString();
};

export const formatCurrencyBRL = (value: unknown): string => {
  const n =
    typeof value === 'number'
      ? value
      : typeof value === 'string'
        ? Number(value)
        : NaN;
  const numberValue = Number.isFinite(n) ? n : 0;
  try {
    return numberValue.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  } catch {
    return `R$ ${numberValue.toFixed(2)}`;
  }
};

/**
 * Formata valor de frequência (ex.: "1", "2", "5" da API ListarVagasRecrutamentoEPerfis)
 * para exibição: "1 dia presencial" ou "X dias presenciais".
 */
export function formatFrequenciaPresencial(value: string | number | null | undefined): string {
  if (value == null || value === '') return '—';
  const n = typeof value === 'number' ? value : parseInt(String(value), 10);
  if (!Number.isFinite(n) || n < 1) return '—';
  return n === 1 ? '1 dia presencial' : `${n} dias presenciais`;
}

/**
 * Converte string de input em pt-BR (ex.: "R$ 1.234,56" ou "1234,56") em número.
 * Remove "R$", espaços e pontos (milhar); troca vírgula por ponto.
 */
export function parseBRLCurrencyInput(value: string | null | undefined): number | null {
  if (value == null || typeof value !== 'string') return null;
  const cleaned = value.replace(/\s/g, '').replace(/R\$/gi, '').trim();
  if (cleaned === '') return null;
  const withoutThousands = cleaned.replace(/\./g, '');
  const withDotDecimal = withoutThousands.replace(',', '.');
  const num = parseFloat(withDotDecimal);
  return Number.isFinite(num) ? num : null;
}

/**
 * Formato do backend: "hh:mm:ss.frac" (horas, minutos, segundos; o ponto pode estar nos segundos).
 * Legado: "dias.horas:minutos:segundos" (dias é só dígitos, sem ":").
 * Não confundir: "00:31:00.8504891" é hh:mm:ss (0h 31m 0.85s), não "dias.tempo".
 */
function parseSlaToHms(sla?: string | null): { h: number; m: number; s: number; totalSeconds: number; totalHours: number } {
  if (!sla || typeof sla !== 'string') return { h: 0, m: 0, s: 0, totalSeconds: 0, totalHours: 0 };
  const trimmed = sla.trim();
  if (!trimmed) return { h: 0, m: 0, s: 0, totalSeconds: 0, totalHours: 0 };

  const dotIdx = trimmed.indexOf('.');
  let h = 0;
  let m = 0;
  let s = 0;

  const isLegacyFormat =
    dotIdx >= 0 &&
    !trimmed.slice(0, dotIdx).includes(':') &&
    /^\d+$/.test(trimmed.slice(0, dotIdx)) &&
    (trimmed.slice(dotIdx + 1).match(/:/g)?.length ?? 0) >= 2;

  if (isLegacyFormat) {
    const daysPart = trimmed.slice(0, dotIdx);
    const timePart = trimmed.slice(dotIdx + 1);
    const days = Number(daysPart) || 0;
    h += days * 24;
    const timeParts = timePart.split(':');
    if (timeParts.length >= 1) h += Number(timeParts[0]) || 0;
    if (timeParts.length >= 2) m = Number(timeParts[1]) || 0;
    if (timeParts.length >= 3) s = Number(timeParts[2]) || 0;
  } else {
    const parts = trimmed.split(':');
    if (parts.length >= 1) h = Number(parts[0]) || 0;
    if (parts.length >= 2) m = Number(parts[1]) || 0;
    if (parts.length >= 3) s = Number(parts[2]) || 0;
  }

  const totalSeconds = h * 3600 + m * 60 + s;
  const totalHours = totalSeconds / 3600;
  return { h, m, s, totalSeconds, totalHours };
}

/**
 * Retorna total em horas e label curto para o card: maior unidade não nula em inteiro (Xh, Xm ou Xs).
 * Ex.: "00:00:06.9950914" → "6s"; "00:15:06" → "15m"; "378674:15:06" → "378674h".
 */
export function parseSlaToHours(sla?: string | null): { hours: number; label: string } {
  const { h, m, s, totalHours } = parseSlaToHms(sla);
  if (h >= 1) return { hours: totalHours, label: `${Math.floor(h)}h` };
  if (m >= 1) return { hours: totalHours, label: `${Math.floor(m)}m` };
  return { hours: totalHours, label: `${Math.floor(s)}s` };
}

/**
 * Converte duração em "X dias e Y horas e Z minutos" (dias de 24h, horas 0–23, minutos 0–59).
 */
function slaToReadable(sla?: string | null): string {
  const { totalSeconds } = parseSlaToHms(sla);
  const total = Math.floor(totalSeconds);
  const days = Math.floor(total / 86400);
  const remainderAfterDays = total % 86400;
  const hours = Math.floor(remainderAfterDays / 3600);
  const remainderAfterHours = remainderAfterDays % 3600;
  const minutes = Math.floor(remainderAfterHours / 60);
  return `${days} dias e ${hours} horas e ${minutes} minutos`;
}

/** Monta o tooltip do tempo decorrido (etapa atual, total, SLA em horas). */
export function formatSlaTooltip(
  slaDecorridoDaEtapaAtual?: string | null,
  slaDecorridoTotal?: string | null,
  slaTotalHoras = 2
): string {
  const etapa = slaToReadable(slaDecorridoDaEtapaAtual);
  const total = slaToReadable(slaDecorridoTotal);
  return `Tempo decorrido:\nNesta etapa: ${etapa}.\nTotal: ${total}.\nSLA Total: ${slaTotalHoras} horas.`;
}
