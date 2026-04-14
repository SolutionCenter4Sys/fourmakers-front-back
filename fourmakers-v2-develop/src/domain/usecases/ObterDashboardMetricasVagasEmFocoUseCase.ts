import { inject, injectable } from 'tsyringe'

import type { DashboardMetricasVagasEmFocoRepository } from '@domain/repositories/DashboardMetricasVagasEmFocoRepository'
import type {
  DashboardMetricasVagasEmFocoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterDashboardMetricasVagasEmFocoUseCase {
  constructor(
    @inject(DiTokens.dashboardMetricasVagasEmFocoRepository)
    private readonly repository: DashboardMetricasVagasEmFocoRepository,
  ) {}

  async execute(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasVagasEmFocoResponse> {
    return this.repository.obterVagasEmFoco(token, filtros)
  }
}
