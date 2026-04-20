import { inject, injectable } from 'tsyringe'
import type { ParametrosRepository } from '@domain/repositories/ParametrosRepository'
import type {
  ParametroConfiguracaoPayload,
  ParametroConfiguracaoInsertResponse,
} from '@domain/entities/ParametroConfiguracao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirParametroConfiguracaoUseCase {
  constructor(
    @inject(DiTokens.parametrosRepository)
    private readonly repository: ParametrosRepository,
  ) {}

  async execute(
    token: string,
    payload: ParametroConfiguracaoPayload
  ): Promise<ParametroConfiguracaoInsertResponse> {
    return this.repository.inserirParametroConfiguracao(token, payload)
  }
}
