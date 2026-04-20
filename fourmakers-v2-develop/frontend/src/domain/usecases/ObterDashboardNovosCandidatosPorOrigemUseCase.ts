import { inject, injectable } from 'tsyringe'

import type { DashboardNovosCandidatosPorOrigemRepository } from '@domain/repositories/DashboardNovosCandidatosPorOrigemRepository'
import type {
  DashboardNovosCandidatosPorOrigemResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterDashboardNovosCandidatosPorOrigemUseCase {
  constructor(
    @inject(DiTokens.dashboardNovosCandidatosPorOrigemRepository)
    private readonly repository: DashboardNovosCandidatosPorOrigemRepository,
  ) {}

  async execute(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardNovosCandidatosPorOrigemResponse> {
    return this.repository.obterNovosCandidatosPorOrigem(token, filtros)
  }
}
