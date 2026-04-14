import { inject, injectable } from 'tsyringe'

import { DiTokens } from '@core/di/tokens'
import type {
  CriarEventoTeamsParams,
  ResultadoEventoTeams,
} from '@domain/repositories/GraphApiRepository'
import type { GraphApiRepository } from '@domain/repositories/GraphApiRepository'
import type { MicrosoftGraphApi } from '@data/api/MicrosoftGraphApi'

@injectable()
export class GraphApiRepositoryImpl implements GraphApiRepository {
  constructor(
    @inject(DiTokens.microsoftGraphApi)
    private readonly api: MicrosoftGraphApi,
  ) {}

  async criarEventoTeams(
    params: CriarEventoTeamsParams,
    fallbackToken?: string | null,
  ): Promise<ResultadoEventoTeams> {
    return this.api.criarEventoTeams(params, fallbackToken)
  }
}
