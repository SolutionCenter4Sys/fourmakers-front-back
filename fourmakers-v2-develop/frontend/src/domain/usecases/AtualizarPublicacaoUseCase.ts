import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CriarPublicacaoResponse } from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class AtualizarPublicacaoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    publicacaoId: string,
    formData: FormData,
  ): Promise<CriarPublicacaoResponse> {
    return this.repository.atualizarPublicacao(token, publicacaoId, formData);
  }
}
