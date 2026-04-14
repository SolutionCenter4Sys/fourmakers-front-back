import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { ObterDashboardGestorResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterDashboardGestorUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository,
  ) {}

  async execute(token: string): Promise<ObterDashboardGestorResponse> {
    return this.repository.obterDashboardGestor(token)
  }
}
