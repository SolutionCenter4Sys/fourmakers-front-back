import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoAnalyticsApi } from '@data/api/ComunicacaoAnalyticsApi';
import type {
  AnalyticsResumoPayload,
  AnalyticsResumoRetorno,
} from '@domain/entities/comunicacao';
import type { ComunicacaoAnalyticsRepository } from '@domain/repositories/ComunicacaoAnalyticsRepository';

@injectable()
export class ComunicacaoAnalyticsRepositoryImpl
  implements ComunicacaoAnalyticsRepository
{
  constructor(
    @inject(DiTokens.comunicacaoAnalyticsApi)
    private readonly api: ComunicacaoAnalyticsApi,
  ) {}

  async obterResumo(
    token: string,
    payload: AnalyticsResumoPayload,
  ): Promise<AnalyticsResumoRetorno | null> {
    const response = await this.api.postResumo(token, payload);
    if (!response.sucesso || !response.retorno) {
      return null;
    }
    return response.retorno;
  }
}
