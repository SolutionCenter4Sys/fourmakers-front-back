import { inject, injectable } from 'tsyringe'

import type { DashboardMetricasVagasEmFocoRepository } from '@domain/repositories/DashboardMetricasVagasEmFocoRepository'
import type {
  DashboardMetricasVagasEmFocoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

import { DiTokens } from '@core/di/tokens'
import { DashboardMetricasVagasEmFocoApi } from '@data/api/DashboardMetricasVagasEmFocoApi'

@injectable()
export class DashboardMetricasVagasEmFocoRepositoryImpl
  implements DashboardMetricasVagasEmFocoRepository {
  constructor(
    @inject(DiTokens.dashboardMetricasVagasEmFocoApi)
    private readonly api: DashboardMetricasVagasEmFocoApi,
  ) {}

  async obterVagasEmFoco(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasVagasEmFocoResponse> {
    return this.api.obterVagasEmFoco(token, filtros)
  }
}
