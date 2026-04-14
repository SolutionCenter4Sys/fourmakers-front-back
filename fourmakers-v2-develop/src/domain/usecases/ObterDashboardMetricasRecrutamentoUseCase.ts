import { inject, injectable } from 'tsyringe'

import type { DashboardMetricasRecrutamentoRepository } from '@domain/repositories/DashboardMetricasRecrutamentoRepository'
import type {
  DashboardMetricasRecrutamentoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterDashboardMetricasRecrutamentoUseCase {
  constructor(
    @inject(DiTokens.dashboardMetricasRecrutamentoRepository)
    private readonly repository: DashboardMetricasRecrutamentoRepository,
  ) {}

  async execute(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasRecrutamentoResponse> {
    return this.repository.obterDashboardMetricas(token, filtros)
  }
}
