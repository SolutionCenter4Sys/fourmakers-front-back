/**
 * Interfaces para integração com Microsoft Graph API (criação de reuniões Teams).
 * Usado pelo CriarReuniaoTeamsUseCase e implementado por GraphApiRepositoryImpl.
 */

export interface ParticipanteTeams {
  email: string
  nome?: string
}

export interface CriarEventoTeamsParams {
  titulo: string
  dataInicioIso: string // ISO 8601
  dataFimIso: string // ISO 8601
  participantes: ParticipanteTeams[]
}

export interface ResultadoEventoTeams {
  id: string
  joinUrl: string
}

export interface GraphApiRepository {
  criarEventoTeams(
    params: CriarEventoTeamsParams,
    fallbackToken?: string | null,
  ): Promise<ResultadoEventoTeams>
}
