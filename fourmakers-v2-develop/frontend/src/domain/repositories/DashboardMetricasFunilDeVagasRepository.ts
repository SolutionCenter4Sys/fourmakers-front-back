import type {
  DashboardMetricasFunilDeVagasResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

export interface DashboardMetricasFunilDeVagasRepository {
  obterFunilDeVagas(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasFunilDeVagasResponse>
}
