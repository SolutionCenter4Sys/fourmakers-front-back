/**
 * Utilitários de timezone para Agendas Comerciais.
 *
 * REGRA DE NEGÓCIO (agendas):
 * - dataAgendada: representa SOMENTE A DATA (dia de calendário), sem hora.
 *   Ao receber: exibir como esse dia (parseDataAgendaParaExibicao).
 *   Ao enviar: enviar o dia de calendário local (formatarDataAgendadaParaPayload / formatarDataLocalParaFiltro).
 * - dataInicio e dataFim: representam DATA E HORA.
 *   Ao receber: interpretar como UTC e converter para local (parseDataHoraAgendaParaExibicao).
 *   Ao enviar: converter horário local para UTC (formatarDataHoraAgendaParaPayload).
 *
 * Não normalizar dataAgendada como datetime (não aplicar conversão UTC de hora).
 */

const REGEX_YYYY_MM_DD = /^\d{4}-\d{2}-\d{2}$/
const REGEX_TEM_TIMEZONE = /[Zz+-]\d{2}:?\d{2}$/
/** Data com hora zerada (ex.: 2026-03-15T00:00:00 ou 2026-03-15T00:00:00Z) = tratar como só data (dia de calendário). */
const REGEX_DATA_HORA_ZERADA = /^(\d{4}-\d{2}-\d{2})T00:00:00(\.0+)?Z?$/i

/**
 * Retorna o timezone do browser (ex.: "America/Sao_Paulo").
 */
export function obterTimezoneUsuario(): string {
  return Intl.DateTimeFormat().resolvedOptions().timeZone
}

/**
 * Garante que a string seja interpretada como UTC (acrescenta Z se for naive).
 */
function garantirUtcParaParse(trimmed: string): string {
  if (REGEX_TEM_TIMEZONE.test(trimmed) || trimmed.endsWith('Z') || trimmed.endsWith('z')) {
    return trimmed
  }
  return `${trimmed.replace(/Z$/i, '')}Z`
}

/**
 * Converte string de DATA APENAS (ex.: dataAgendada) recebida da API para Date para exibição.
 * Usar para campos que representam somente o dia (sem hora).
 * - YYYY-MM-DD: interpreta como dia de calendário no timezone do usuário (ex.: "2026-03-12" → 12 de março local).
 * - Se vier com T/espaço: interpreta como UTC e converte para exibição local.
 *
 * @param dateString String de data da API (formato YYYY-MM-DD ou ISO com/sem Z)
 * @returns Date para uso com format() do date-fns (exibição no fuso do browser) ou null se inválida
 */
/**
 * Extrai a parte yyyy-MM-dd e retorna Date como dia de calendário local, ou null se inválido.
 */
function parseApenasDiaLocal(parteData: string): Date | null {
  if (!REGEX_YYYY_MM_DD.test(parteData)) return null
  const [ano, mes, dia] = parteData.split('-').map(Number)
  if (isNaN(ano) || isNaN(mes) || isNaN(dia) || mes < 1 || mes > 12 || dia < 1 || dia > 31) return null
  const d = new Date(ano, mes - 1, dia)
  if (d.getFullYear() !== ano || d.getMonth() !== mes - 1 || d.getDate() !== dia) return null
  return d
}

export function parseDataAgendaParaExibicao(
  dateString: string | null | undefined
): Date | null {
  if (!dateString || dateString.trim() === '') return null
  const trimmed = dateString.trim()

  if (REGEX_YYYY_MM_DD.test(trimmed)) {
    return parseApenasDiaLocal(trimmed)
  }

  const matchHoraZerada = trimmed.match(REGEX_DATA_HORA_ZERADA)
  if (matchHoraZerada) {
    return parseApenasDiaLocal(matchHoraZerada[1])
  }

  const comUtc = garantirUtcParaParse(trimmed)
  const d = new Date(comUtc)
  return Number.isNaN(d.getTime()) ? null : d
}

/**
 * Converte string de DATA E HORA (dataInicio, dataFim, dataCriacao, etc.) da API (UTC) para Date no timezone do usuário.
 * Usar apenas para campos que representam horário; não usar para dataAgendada.
 *
 * @param dateString String de data/hora da API (formato ISO com/sem Z)
 * @returns Date para uso com format() do date-fns (exibição no fuso do browser) ou null se inválida
 */
export function parseDataHoraAgendaParaExibicao(
  dateString: string | null | undefined
): Date | null {
  if (!dateString || dateString.trim() === '') return null
  const trimmed = dateString.trim()

  if (REGEX_YYYY_MM_DD.test(trimmed) && !trimmed.includes('T') && !trimmed.includes(' ')) {
    const comoUtcMeiaNoite = `${trimmed}T00:00:00Z`
    const d = new Date(comoUtcMeiaNoite)
    return Number.isNaN(d.getTime()) ? null : d
  }

  const comUtc = garantirUtcParaParse(trimmed)
  const d = new Date(comUtc)
  return Number.isNaN(d.getTime()) ? null : d
}

/**
 * Converte Date (horário local) para string em UTC no formato datetime da API.
 * Usar APENAS para dataInicio e dataFim (campos que têm hora). Não usar para dataAgendada.
 *
 * @param data Date em horário local do usuário (ou null)
 * @returns "yyyy-MM-dd'T'HH:mm:ss" em UTC (com Z) ou string vazia se data for null
 */
export function formatarDataHoraAgendaParaPayload(data: Date | null): string {
  if (!data) return ''
  const iso = data.toISOString()
  return iso.slice(0, 19) + 'Z'
}

/**
 * Formata Date como data de calendário local (yyyy-MM-dd), sem hora.
 * Usar para: dataAgendada no payload (criar/atualizar agenda) e parâmetros de filtro (período).
 * dataAgendada representa somente a data; não aplicar conversão de hora/timezone.
 *
 * @param data Date em horário local do usuário (ou null)
 * @returns String "yyyy-MM-dd" no fuso local ou string vazia se data for null
 */
export function formatarDataLocalParaFiltro(data: Date | null): string {
  if (!data) return ''
  const ano = data.getFullYear()
  const mes = String(data.getMonth() + 1).padStart(2, '0')
  const dia = String(data.getDate()).padStart(2, '0')
  return `${ano}-${mes}-${dia}`
}

/** Alias para uso explícito no payload de agenda: campo dataAgendada (somente data). */
export const formatarDataAgendadaParaPayload = formatarDataLocalParaFiltro

/**
 * Retorna o dia de calendário (data apenas) de uma agenda a partir de dataAgendada.
 * Usar para agrupamento na timeline e exibição da data no card.
 * A data exata da agenda deve sempre usar dataAgendada (date) + dataInicio (time).
 *
 * @param dataAgendada String dataAgendada da API (YYYY-MM-DD ou com T00:00:00)
 * @returns Date à meia-noite no fuso local para esse dia, ou null
 */
export function obterDiaCalendarioAgenda(
  dataAgendada: string | null | undefined
): Date | null {
  return parseDataAgendaParaExibicao(dataAgendada)
}

/**
 * Retorna a data/hora exata de uma agenda correlacionando dataAgendada (dia) e dataInicio (hora).
 * Regra: dia vem de dataAgendada (calendário local), hora vem de dataInicio (UTC convertido para local).
 * Usar para ordenação na timeline e sempre que precisar do "momento" exato da agenda.
 *
 * @param dataAgendada String dataAgendada da API (dia do evento)
 * @param dataInicio String dataInicio da API (horário em UTC)
 * @returns Date no fuso local: dia de dataAgendada + horário de dataInicio; ou null se dataAgendada inválida
 */
export function obterDataExataAgenda(
  dataAgendada: string | null | undefined,
  dataInicio: string | null | undefined
): Date | null {
  const dia = parseDataAgendaParaExibicao(dataAgendada)
  if (!dia) return null
  const horaLocal = dataInicio ? parseDataHoraAgendaParaExibicao(dataInicio) : null
  if (!horaLocal) {
    return dia
  }
  return new Date(
    dia.getFullYear(),
    dia.getMonth(),
    dia.getDate(),
    horaLocal.getHours(),
    horaLocal.getMinutes(),
    horaLocal.getSeconds(),
    horaLocal.getMilliseconds()
  )
}
