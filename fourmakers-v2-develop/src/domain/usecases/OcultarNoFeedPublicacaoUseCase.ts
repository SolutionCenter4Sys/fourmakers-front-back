import { inject, injectable } from 'tsyringe';
import type { ArquivarPublicacaoResponse, OcultarNoFeedPayload } from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';
import { DiTokens } from '@core/di/tokens';

@injectable()
export class OcultarNoFeedPublicacaoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    payload: OcultarNoFeedPayload,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.repository.ocultarNoFeed(token, payload);
  }
}
