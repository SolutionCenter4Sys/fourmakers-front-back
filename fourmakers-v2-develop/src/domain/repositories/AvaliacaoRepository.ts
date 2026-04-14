import type { InserirAvaliacaoSatisfacaoPayload, InserirAvaliacaoSatisfacaoResponse } from '@domain/entities/AvaliacaoSatisfacao'

export interface AvaliacaoRepository {
  inserirAvaliacao(token: string, payload: InserirAvaliacaoSatisfacaoPayload): Promise<InserirAvaliacaoSatisfacaoResponse>
}

