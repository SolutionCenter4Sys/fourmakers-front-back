import { inject, injectable } from 'tsyringe'
import type { AvaliacaoRepository } from '@domain/repositories/AvaliacaoRepository'
import type { InserirAvaliacaoSatisfacaoPayload, InserirAvaliacaoSatisfacaoResponse } from '@domain/entities/AvaliacaoSatisfacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirAvaliacaoSatisfacaoUseCase {
  constructor(
    @inject(DiTokens.avaliacaoRepository)
    private readonly repository: AvaliacaoRepository,
  ) {}

  async execute(token: string, payload: InserirAvaliacaoSatisfacaoPayload): Promise<InserirAvaliacaoSatisfacaoResponse> {
    return this.repository.inserirAvaliacao(token, payload)
  }
}

