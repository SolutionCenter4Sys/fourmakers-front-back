import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';
import type { ConfirmacoesLeituraResult } from '@domain/entities/comunicacao';

@injectable()
export class ObterConfirmacoesLeituraUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    publicacaoId: string,
  ): Promise<ConfirmacoesLeituraResult> {
    return this.repository.obterConfirmacoesLeitura(token, publicacaoId);
  }
}
