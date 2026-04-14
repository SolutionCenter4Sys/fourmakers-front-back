import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { InserirParametrizacaoPayload, InserirParametrizacaoResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirParametrizacaoUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository,
  ) {}

  async execute(token: string, payload: InserirParametrizacaoPayload): Promise<InserirParametrizacaoResponse> {
    return this.repository.inserirParametrizacao(token, payload)
  }
}
