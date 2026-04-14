import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { InserirPautaSugeridaPayload, InserirPautaSugeridaResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirPautaSugeridaUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository,
  ) {}

  async execute(token: string, payload: InserirPautaSugeridaPayload): Promise<InserirPautaSugeridaResponse> {
    return this.repository.inserirPautaSugerida(token, payload)
  }
}
