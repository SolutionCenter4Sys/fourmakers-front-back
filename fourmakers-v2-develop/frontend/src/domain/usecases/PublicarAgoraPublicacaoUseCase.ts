import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ArquivarPublicacaoResponse } from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class PublicarAgoraPublicacaoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.repository.publicarAgora(token, publicacaoId);
  }
}
