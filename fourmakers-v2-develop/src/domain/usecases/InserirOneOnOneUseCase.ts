import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { InserirOneOnOnePayload, InserirOneOnOneResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirOneOnOneUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository,
  ) {}

  async execute(token: string, payload: InserirOneOnOnePayload): Promise<InserirOneOnOneResponse> {
    return this.repository.inserirOneOnOne(token, payload)
  }
}
