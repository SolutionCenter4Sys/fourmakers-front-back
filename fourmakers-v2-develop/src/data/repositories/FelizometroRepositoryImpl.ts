import { inject, injectable } from 'tsyringe'
import type { FelizometroRepository } from '@domain/repositories/FelizometroRepository'
import type { FelizometroPayload, FelizometroResponse } from '@domain/entities/FelizometroSentimento'
import { FelizometroApi } from '@data/api/FelizometroApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class FelizometroRepositoryImpl implements FelizometroRepository {
  constructor(
    @inject(DiTokens.felizometroApi)
    private readonly api: FelizometroApi,
  ) {}

  async enviarSentimento(payload: FelizometroPayload): Promise<FelizometroResponse> {
    return await this.api.enviarSentimento(payload)
  }
}

