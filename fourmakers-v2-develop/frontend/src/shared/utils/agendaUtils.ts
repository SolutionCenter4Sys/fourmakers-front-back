import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'
import { parseISO } from 'date-fns'

/**
 * Extrai o código interno do colaborador do objeto de usuário (CPF).
 * @param user Objeto de usuário (pode ser null)
 * @returns Código interno do colaborador (CPF) ou string vazia
 */
export function getCodInternoColaborador(user: ShowmeUserProfile | null | undefined): string {
  return user?.cpf || user?.colaborador?.cpf || ''
}

/**
 * Retorna todos os identificadores possíveis do colaborador para comparação com a API.
 * A API pode devolver criador como CPF ou como código interno (codColaborador); esta função
 * permite comparar com ambos.
 * @param user Objeto de usuário (pode ser null)
 * @returns Array de strings não vazias (cpf, codColaborador) para comparação
 */
export function getIdentificadoresColaborador(
  user: ShowmeUserProfile | null | undefined
): string[] {
  if (!user) return []
  const ids: string[] = []
  const cpf = user?.cpf || user?.colaborador?.cpf
  if (cpf && cpf.trim()) ids.push(cpf.trim().toLowerCase())
  const codColab = user?.colaboradorOrg?.codColaborador
  if (codColab && String(codColab).trim()) {
    const s = String(codColab).trim().toLowerCase()
    if (!ids.includes(s)) ids.push(s)
  }
  return ids
}

/**
 * Calcula a duração de uma agenda em minutos.
 * 
 * @param dataInicio Data de início (pode ser null)
 * @param dataFim Data de fim (pode ser null)
 * @returns Duração em minutos ou null se não houver ambas as datas
 */
export function calcularDuracaoAgenda(
  dataInicio: Date | null,
  dataFim: Date | null
): number | null {
  if (!dataInicio || !dataFim) {
    return null
  }

  return Math.round((dataFim.getTime() - dataInicio.getTime()) / (1000 * 60))
}

/** Label exibido quando a data da agenda/interação não está disponível. */
export const LABEL_DATA_NAO_INFORMADA = 'Data não informada'

const REGEX_YYYY_MM_DD = /^\d{4}-\d{2}-\d{2}$/

/** Extrai a parte YYYY-MM-DD de uma string (inteira ou antes de T/espaço). */
function extrairParteData(trimmed: string): string | null {
  if (REGEX_YYYY_MM_DD.test(trimmed)) return trimmed
  if (trimmed.includes('T') || trimmed.includes(' ')) {
    const dataPart = trimmed.split('T')[0].split(' ')[0]
    return REGEX_YYYY_MM_DD.test(dataPart) ? dataPart : null
  }
  return null
}

/**
 * Cria uma data local e valida (ex.: 31/02 retorna null).
 * @param hora minuto segundo opcionais (default 0)
 */
function criarDataLocalValidada(
  ano: number,
  mes: number,
  dia: number,
  hora = 0,
  minuto = 0,
  segundo = 0
): Date | null {
  if (isNaN(ano) || isNaN(mes) || isNaN(dia) || mes < 1 || mes > 12 || dia < 1 || dia > 31) {
    return null
  }
  const date = new Date(ano, mes - 1, dia, hora, minuto, segundo, 0)
  if (
    date.getFullYear() !== ano ||
    date.getMonth() !== mes - 1 ||
    date.getDate() !== dia
  ) {
    return null
  }
  return date
}

/**
 * Parseia uma data de forma segura, evitando problemas de timezone.
 * Para datas no formato YYYY-MM-DD (sem hora), cria uma data local.
 * Para datas com hora (ISO), extrai apenas a parte da data (ignora hora) para evitar problemas de timezone.
 * 
 * IMPORTANTE: Esta função é para dataAgendada que só precisa da data, sem hora.
 * Para dataInicio e dataFim, use parseAgendaDateTime que preserva a hora.
 * 
 * @param dateString String de data da API (formato YYYY-MM-DD ou ISO)
 * @returns Date object ou null se inválida
 */
export function parseAgendaDate(dateString: string | null | undefined): Date | null {
  if (!dateString || dateString.trim() === '') return null
  const trimmed = dateString.trim()

  const dataPart = extrairParteData(trimmed)
  if (dataPart) {
    const [ano, mes, dia] = dataPart.split('-').map(Number)
    return criarDataLocalValidada(ano, mes, dia)
  }

  if (trimmed.includes('T') || trimmed.includes(' ')) {
    try {
      return parseISO(trimmed)
    } catch {
      return null
    }
  }

  try {
    const date = new Date(trimmed)
    return isNaN(date.getTime()) ? null : date
  } catch {
    return null
  }
}

/**
 * Parseia uma data com hora de forma segura, preservando a hora mas ajustando para o timezone local.
 * Para datas no formato YYYY-MM-DD (sem hora), cria uma data local com horas zeradas.
 * Para datas com hora (ISO), extrai a data e hora e cria uma data local preservando os valores.
 * 
 * IMPORTANTE: Esta função é para dataInicio e dataFim que precisam preservar a hora.
 * Para dataAgendada, use parseAgendaDate que ignora a hora.
 * 
 * @param dateString String de data da API (formato YYYY-MM-DD ou ISO)
 * @returns Date object ou null se inválida
 */
export function parseAgendaDateTime(dateString: string | null | undefined): Date | null {
  if (!dateString || dateString.trim() === '') return null
  const trimmed = dateString.trim()

  const dataPart = extrairParteData(trimmed)
  if (dataPart && !trimmed.includes('T') && !trimmed.includes(' ')) {
    const [ano, mes, dia] = dataPart.split('-').map(Number)
    return criarDataLocalValidada(ano, mes, dia)
  }

  if (trimmed.includes('T') || trimmed.includes(' ')) {
    try {
      const [parteData, horaPart] = trimmed.includes('T') ? trimmed.split('T') : trimmed.split(' ')
      const dataPartISO = REGEX_YYYY_MM_DD.test(parteData) ? parteData : null
      if (!dataPartISO) return parseISO(trimmed)

      const [ano, mes, dia] = dataPartISO.split('-').map(Number)
      let hora = 0, minuto = 0, segundo = 0
      if (horaPart) {
        const horaSemTimezone = horaPart.replace(/[Z+-].*$/, '')
        const partesHora = horaSemTimezone.split(':').map(Number)
        if (partesHora.length >= 1 && !isNaN(partesHora[0])) hora = partesHora[0]
        if (partesHora.length >= 2 && !isNaN(partesHora[1])) minuto = partesHora[1]
        if (partesHora.length >= 3 && !isNaN(partesHora[2])) segundo = Math.floor(partesHora[2])
      }
      return criarDataLocalValidada(ano, mes, dia, hora, minuto, segundo)
    } catch {
      return parseISO(trimmed)
    }
  }

  try {
    const date = new Date(trimmed)
    return isNaN(date.getTime()) ? null : date
  } catch {
    return null
  }
}
