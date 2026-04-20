import { inject, injectable } from 'tsyringe'
import type { ParametrosRepository } from '@domain/repositories/ParametrosRepository'
import type { ParametrosConfiguracaoResponse } from '@domain/entities/ParametroConfiguracao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarParametrosConfiguracaoUseCase {
  constructor(
    @inject(DiTokens.parametrosRepository)
    private readonly repository: ParametrosRepository,
  ) {}

  async execute(token: string): Promise<ParametrosConfiguracaoResponse> {
    return this.repository.listarParametrosConfiguracao(token)
  }
}

