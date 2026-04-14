import { inject, injectable } from 'tsyringe'
import type { AvaliacaoRepository } from '@domain/repositories/AvaliacaoRepository'
import type { InserirAvaliacaoSatisfacaoPayload, InserirAvaliacaoSatisfacaoResponse } from '@domain/entities/AvaliacaoSatisfacao'
import { FourmakersApi } from '@data/api/FourmakersApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AvaliacaoRepositoryImpl implements AvaliacaoRepository {
  constructor(
    @inject(DiTokens.fourmakersApi)
    private readonly api: FourmakersApi,
  ) {}

  async inserirAvaliacao(token: string, payload: InserirAvaliacaoSatisfacaoPayload): Promise<InserirAvaliacaoSatisfacaoResponse> {
    return this.api.inserirAvaliacao(token, payload)
  }
}

