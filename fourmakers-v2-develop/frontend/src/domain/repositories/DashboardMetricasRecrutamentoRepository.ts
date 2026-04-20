import type {
  DashboardMetricasRecrutamentoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

export interface DashboardMetricasRecrutamentoRepository {
  obterDashboardMetricas(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasRecrutamentoResponse>
}
