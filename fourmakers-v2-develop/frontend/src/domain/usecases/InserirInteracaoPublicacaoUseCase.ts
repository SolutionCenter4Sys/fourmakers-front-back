import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  InserirInteracaoPublicacaoPayload,
  InteracaoPublicacaoResponse,
} from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class InserirInteracaoPublicacaoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    payload: InserirInteracaoPublicacaoPayload,
  ): Promise<InteracaoPublicacaoResponse> {
    return this.repository.inserirInteracaoPublicacao(token, payload);
  }
}
