import { inject, injectable } from 'tsyringe'

import type { DashboardMetricasFunilDeVagasRepository } from '@domain/repositories/DashboardMetricasFunilDeVagasRepository'
import type {
  DashboardMetricasFunilDeVagasResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

import { DiTokens } from '@core/di/tokens'
import { DashboardMetricasFunilDeVagasApi } from '@data/api/DashboardMetricasFunilDeVagasApi'

@injectable()
export class DashboardMetricasFunilDeVagasRepositoryImpl
  implements DashboardMetricasFunilDeVagasRepository {
  constructor(
    @inject(DiTokens.dashboardMetricasFunilDeVagasApi)
    private readonly api: DashboardMetricasFunilDeVagasApi,
  ) {}

  async obterFunilDeVagas(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasFunilDeVagasResponse> {
    return this.api.obterFunilDeVagas(token, filtros)
  }
}
