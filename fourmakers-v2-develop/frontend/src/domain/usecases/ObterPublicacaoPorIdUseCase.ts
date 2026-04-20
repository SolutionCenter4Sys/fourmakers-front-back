import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CommunityPost } from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class ObterPublicacaoPorIdUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(token: string, publicacaoId: string): Promise<CommunityPost> {
    return this.repository.obterPublicacaoPorId(token, publicacaoId);
  }
}
