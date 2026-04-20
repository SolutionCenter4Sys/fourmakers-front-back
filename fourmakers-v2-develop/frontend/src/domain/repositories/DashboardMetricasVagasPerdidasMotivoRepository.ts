import type {
  DashboardMetricasVagasPerdidasMotivoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

export interface DashboardMetricasVagasPerdidasMotivoRepository {
  obterVagasPerdidasMotivo(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasVagasPerdidasMotivoResponse>
}
