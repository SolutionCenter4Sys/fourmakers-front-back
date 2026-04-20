import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { ObterDashboardColaboradorAvaliadoResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterDashboardColaboradorAvaliadoUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository,
  ) {}

  async execute(token: string, codigoInternoColaborador: string): Promise<ObterDashboardColaboradorAvaliadoResponse> {
    return this.repository.obterDashboardColaboradorAvaliado(token, codigoInternoColaborador)
  }
}
