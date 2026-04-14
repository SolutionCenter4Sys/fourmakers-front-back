import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComentarioPublicacaoResponse } from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class ExcluirComentarioPublicacaoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    publicacaoId: string,
    comentarioId: string,
  ): Promise<ComentarioPublicacaoResponse> {
    return this.repository.excluirComentario(token, publicacaoId, comentarioId);
  }
}
