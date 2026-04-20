import { inject, injectable } from 'tsyringe'

import type {
  CriarEventoTeamsParams,
  GraphApiRepository,
  ResultadoEventoTeams,
} from '@domain/repositories/GraphApiRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarReuniaoTeamsUseCase {
  constructor(
    @inject(DiTokens.graphApiRepository)
    private readonly repository: GraphApiRepository,
  ) {}

  async execute(
    params: CriarEventoTeamsParams,
    fallbackToken?: string | null,
  ): Promise<ResultadoEventoTeams> {
    return this.repository.criarEventoTeams(params, fallbackToken)
  }
}
