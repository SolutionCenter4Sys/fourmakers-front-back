import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  AnalyticsResumoPayload,
  AnalyticsResumoRetorno,
} from '@domain/entities/comunicacao';
import type { ComunicacaoAnalyticsRepository } from '@domain/repositories/ComunicacaoAnalyticsRepository';

@injectable()
export class ObterAnalyticsResumoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoAnalyticsRepository)
    private readonly repository: ComunicacaoAnalyticsRepository,
  ) {}

  async execute(
    token: string,
    payload: AnalyticsResumoPayload,
  ): Promise<AnalyticsResumoRetorno | null> {
    return this.repository.obterResumo(token, payload);
  }
}
