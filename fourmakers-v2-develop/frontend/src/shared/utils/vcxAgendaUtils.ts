import type { VcxAgenda } from '@domain/entities/VcxAgenda'
import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'

/** Mapeamento tipoInteracao numérico (VCX) para rótulo do modal de detalhes */
const TIPO_INTERACAO_LABEL: Record<number, string> = {
  1: 'Reunião',
  2: 'Ligação',
  3: 'Chat',
  4: 'Email',
  5: 'Presencial',
}

/**
 * Formata data ISO de forma crua (sem conversão de fuso).
 * Extrai dia, mês e ano diretamente da string, preservando o valor do endpoint.
 */
export function formatarDataCrua(dataStr: string | null): string {
  if (!dataStr || dataStr.trim() === '') return '—'
  const match = dataStr.trim().match(/(\d{4})-(\d{2})-(\d{2})/)
  if (!match) return '—'
  const [, ano, mes, dia] = match
  return `${dia}/${mes}/${ano}`
}

/**
 * Extrai horário HH:mm de forma crua (sem conversão de fuso).
 * Usa os valores exatamente como vêm do endpoint.
 */
export function formatarHorarioCruo(valor: string | null): string {
  if (!valor || valor.trim() === '') return '—'
  const match = valor.trim().match(/[T\s](\d{2}):(\d{2})/)
  if (!match) return '—'
  const [, h, m] = match
  return `${h}:${m}`
}

/**
 * Formata intervalo de horários de forma crua (início – fim).
 */
export function formatarIntervaloHorarioCruo(inicio: string | null, fim: string | null): string {
  const hi = formatarHorarioCruo(inicio)
  if (hi === '—') return '—'
  if (!fim || fim.trim() === '') return hi
  const hf = formatarHorarioCruo(fim)
  if (hf === '—') return hi
  return `${hi} – ${hf}`
}

/**
 * Retorna o rótulo do tipo de interação (1–5) ou undefined se inválido/null.
 * Usado no card da aba Agenda do VCX e em outros pontos que exibem o tipo.
 */
export function obterLabelTipoInteracao(tipo: number | null): string | undefined {
  if (tipo == null || tipo < 1 || tipo > 5) return undefined
  return TIPO_INTERACAO_LABEL[tipo]
}

/**
 * Retorna a data de exibição da agenda (dataInicio ou dataAgendada) como timestamp ou null
 */
function parsearDataExibicao(agenda: VcxAgenda): number | null {
  const valor = agenda.dataInicio ?? agenda.dataAgendada
  if (valor == null || valor === '') return null
  try {
    const date = new Date(valor)
    return Number.isNaN(date.getTime()) ? null : date.getTime()
  } catch {
    return null
  }
}

/**
 * Mescla agendas novas e antigas e ordena por data de exibição (mais recente primeiro).
 * Itens sem data válida ficam no final.
 */
export function mesclarEOrdenarAgendas(
  antigas: VcxAgenda[],
  novas: VcxAgenda[],
): VcxAgenda[] {
  const todas = [...novas, ...antigas]
  todas.sort((a, b) => {
    const da = parsearDataExibicao(a)
    const db = parsearDataExibicao(b)
    if (da == null && db == null) return 0
    if (da == null) return 1
    if (db == null) return -1
    return db - da
  })
  return todas
}

/**
 * Encontra a agenda pai na lista (onde agenda.id === agendaPaiId)
 */
export function encontrarAgendaPai(
  agenda: VcxAgenda,
  todas: VcxAgenda[],
): VcxAgenda | null {
  const paiId = agenda.agendaPaiId
  if (paiId == null) return null
  return todas.find((a) => a.id === paiId) ?? null
}

/**
 * Conta quantas agendas têm agendaPaiId === agendaId
 */
export function contarAgendasFilhas(
  agendaId: number | null,
  todas: VcxAgenda[],
): number {
  if (agendaId == null) return 0
  return todas.filter((a) => a.agendaPaiId === agendaId).length
}

/**
 * Converte VcxAgenda para ItemAgendaGestor para exibição no modal Ver Mais (Agendas Comerciais).
 * Usado ao navegar da aba Agenda do VCX para a tela Agendas Comerciais com o modal aberto.
 */
export function vcxAgendaParaItemAgendaGestor(agenda: VcxAgenda): ItemAgendaGestor {
  const tipoInteracaoStr =
    agenda.tipoInteracao != null && TIPO_INTERACAO_LABEL[agenda.tipoInteracao]
      ? TIPO_INTERACAO_LABEL[agenda.tipoInteracao]
      : undefined

  return {
    tipo: 'agenda',
    id: String(agenda.id ?? ''),
    agendaId: agenda.id ?? undefined,
    titulo: agenda.titulo?.trim() || 'Sem título',
    responsavel: agenda.nomeCompletoColaboradorCriador?.trim() || '',
    data:
      agenda.dataAgendada?.trim() ||
      agenda.dataInicio?.trim() ||
      agenda.dataCriacao?.trim() ||
      '',
    status: agenda.status?.trim() || 'Sem status',
    descricao: agenda.descricao?.trim() || undefined,
    dataInicio: agenda.dataInicio?.trim() || undefined,
    dataFim: agenda.dataFim?.trim() || undefined,
    localizacao: agenda.localizacao?.trim() || undefined,
    linkReuniao: agenda.linkReuniao?.trim() || undefined,
    tipoInteracao: tipoInteracaoStr,
    participantes: agenda.quantidadeParticipantes ?? undefined,
  }
}
