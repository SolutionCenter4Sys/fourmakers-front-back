import { inject, injectable } from 'tsyringe'

import type { DashboardNovosCandidatosPorOrigemRepository } from '@domain/repositories/DashboardNovosCandidatosPorOrigemRepository'
import type {
  DashboardNovosCandidatosPorOrigemResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'

import { DiTokens } from '@core/di/tokens'
import { DashboardNovosCandidatosPorOrigemApi } from '@data/api/DashboardNovosCandidatosPorOrigemApi'

@injectable()
export class DashboardNovosCandidatosPorOrigemRepositoryImpl
  implements DashboardNovosCandidatosPorOrigemRepository {
  constructor(
    @inject(DiTokens.dashboardNovosCandidatosPorOrigemApi)
    private readonly api: DashboardNovosCandidatosPorOrigemApi,
  ) {}

  async obterNovosCandidatosPorOrigem(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardNovosCandidatosPorOrigemResponse> {
    return this.api.obterNovosCandidatosPorOrigem(token, filtros)
  }
}
