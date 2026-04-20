import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  ComunicacaoFeedParams,
  ComunicacaoFeedResult,
} from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class ListarComunicacaoFeedUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    params?: ComunicacaoFeedParams,
  ): Promise<ComunicacaoFeedResult> {
    return this.repository.getFeed(token, params);
  }
}
