import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  AtualizarComentarioPublicacaoPayload,
  ComentarioPublicacaoResponse,
} from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class AtualizarComentarioPublicacaoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    payload: AtualizarComentarioPublicacaoPayload,
  ): Promise<ComentarioPublicacaoResponse> {
    return this.repository.atualizarComentario(token, payload);
  }
}
