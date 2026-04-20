import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { InserirPautaSugeridaColaboradorPayload, InserirPautaSugeridaResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirPautaSugeridaColaboradorUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository,
  ) {}

  async execute(token: string, payload: InserirPautaSugeridaColaboradorPayload): Promise<InserirPautaSugeridaResponse> {
    return this.repository.inserirPautaSugeridaColaborador(token, payload)
  }
}
