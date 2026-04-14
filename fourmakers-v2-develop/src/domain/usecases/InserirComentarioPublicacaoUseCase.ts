import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  InserirComentarioPublicacaoPayload,
  InserirComentarioPublicacaoResponse,
} from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class InserirComentarioPublicacaoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    payload: InserirComentarioPublicacaoPayload,
  ): Promise<InserirComentarioPublicacaoResponse> {
    return this.repository.inserirComentario(token, payload);
  }
}
