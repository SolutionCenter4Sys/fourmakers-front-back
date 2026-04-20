import type {
  DashboardMetricasVagasEmFocoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

export interface DashboardMetricasVagasEmFocoRepository {
  obterVagasEmFoco(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasVagasEmFocoResponse>
}
