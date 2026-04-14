import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  ConfirmarLeituraObrigatoriaPayload,
  ConfirmarLeituraObrigatoriaResponse,
} from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class ConfirmarLeituraObrigatoriaUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    payload: ConfirmarLeituraObrigatoriaPayload,
  ): Promise<ConfirmarLeituraObrigatoriaResponse> {
    return this.repository.confirmarLeituraObrigatoria(token, payload);
  }
}
