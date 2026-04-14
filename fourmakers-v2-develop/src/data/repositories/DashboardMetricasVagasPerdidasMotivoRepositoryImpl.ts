import { inject, injectable } from 'tsyringe'

import type { DashboardMetricasVagasPerdidasMotivoRepository } from '@domain/repositories/DashboardMetricasVagasPerdidasMotivoRepository'
import type {
  DashboardMetricasVagasPerdidasMotivoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

import { DiTokens } from '@core/di/tokens'
import { DashboardMetricasVagasPerdidasMotivoApi } from '@data/api/DashboardMetricasVagasPerdidasMotivoApi'

@injectable()
export class DashboardMetricasVagasPerdidasMotivoRepositoryImpl
  implements DashboardMetricasVagasPerdidasMotivoRepository {
  constructor(
    @inject(DiTokens.dashboardMetricasVagasPerdidasMotivoApi)
    private readonly api: DashboardMetricasVagasPerdidasMotivoApi,
  ) {}

  async obterVagasPerdidasMotivo(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasVagasPerdidasMotivoResponse> {
    return this.api.obterVagasPerdidasMotivo(token, filtros)
  }
}
