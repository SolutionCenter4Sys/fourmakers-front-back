import { inject, injectable } from 'tsyringe'
import type { ParametrosRepository } from '@domain/repositories/ParametrosRepository'
import type { ParametroConfiguracaoPayload } from '@domain/entities/ParametroConfiguracao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarParametroConfiguracaoUseCase {
  constructor(
    @inject(DiTokens.parametrosRepository)
    private readonly repository: ParametrosRepository,
  ) {}

  async execute(
    token: string,
    id: string,
    orgId: number,
    payload: ParametroConfiguracaoPayload
  ): Promise<void> {
    return this.repository.atualizarParametroConfiguracao(token, id, orgId, payload)
  }
}
