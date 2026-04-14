import { inject, injectable } from 'tsyringe'

import type { DashboardMetricasVagasPerdidasMotivoRepository } from '@domain/repositories/DashboardMetricasVagasPerdidasMotivoRepository'
import type {
  DashboardMetricasVagasPerdidasMotivoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterDashboardMetricasVagasPerdidasMotivoUseCase {
  constructor(
    @inject(DiTokens.dashboardMetricasVagasPerdidasMotivoRepository)
    private readonly repository: DashboardMetricasVagasPerdidasMotivoRepository,
  ) {}

  async execute(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasVagasPerdidasMotivoResponse> {
    return this.repository.obterVagasPerdidasMotivo(token, filtros)
  }
}
