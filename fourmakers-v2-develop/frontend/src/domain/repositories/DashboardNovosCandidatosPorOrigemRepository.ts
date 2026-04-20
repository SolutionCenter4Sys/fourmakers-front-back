import type {
  DashboardNovosCandidatosPorOrigemResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

export interface DashboardNovosCandidatosPorOrigemRepository {
  obterNovosCandidatosPorOrigem(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardNovosCandidatosPorOrigemResponse>
}
