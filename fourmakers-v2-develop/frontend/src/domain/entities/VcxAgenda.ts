/**
 * Entidades para a aba Agenda do painel VCX 360°.
 * Representa reuniões/agendas entre colaborador e cliente.
 */

export interface VcxAgenda {
  id: number | null
  graphEventId: string | null
  codColaboradorCriador: string | null
  nomeCompletoColaboradorCriador: string | null
  codigoCliente: string | null
  tipoInteracao: number | null
  dataCriacao: string | null // ISO 8601
  dataAtualizacao: string | null
  dataAgendada: string | null
  dataInicio: string | null
  dataFim: string | null
  status: string | null // ex: "Confirmada", "Em aprovação"
  localizacao: string | null
  linkReuniao: string | null
  quantidadeParticipantes: number | null
  titulo: string | null
  descricao: string | null
  agendaPaiId: number | null
}

export interface VcxAgendasPorColaboradorClienteResult {
  agendasAntigas: VcxAgenda[]
  agendasNovas: VcxAgenda[]
}
