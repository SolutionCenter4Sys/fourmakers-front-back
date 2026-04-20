import { inject, injectable } from 'tsyringe'

import type { DashboardMetricasRecrutamentoRepository } from '@domain/repositories/DashboardMetricasRecrutamentoRepository'
import type {
  DashboardMetricasRecrutamentoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

import { DiTokens } from '@core/di/tokens'
import { DashboardMetricasRecrutamentoApi } from '@data/api/DashboardMetricasRecrutamentoApi'

@injectable()
export class DashboardMetricasRecrutamentoRepositoryImpl
  implements DashboardMetricasRecrutamentoRepository {
  constructor(
    @inject(DiTokens.dashboardMetricasRecrutamentoApi)
    private readonly api: DashboardMetricasRecrutamentoApi,
  ) {}

  async obterDashboardMetricas(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasRecrutamentoResponse> {
    return this.api.obterDashboardMetricas(token, filtros)
  }
}
