import { inject, injectable } from 'tsyringe'

import type { DashboardMetricasFunilDeVagasRepository } from '@domain/repositories/DashboardMetricasFunilDeVagasRepository'
import type {
  DashboardMetricasFunilDeVagasResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterDashboardMetricasFunilDeVagasUseCase {
  constructor(
    @inject(DiTokens.dashboardMetricasFunilDeVagasRepository)
    private readonly repository: DashboardMetricasFunilDeVagasRepository,
  ) {}

  async execute(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasFunilDeVagasResponse> {
    return this.repository.obterFunilDeVagas(token, filtros)
  }
}
