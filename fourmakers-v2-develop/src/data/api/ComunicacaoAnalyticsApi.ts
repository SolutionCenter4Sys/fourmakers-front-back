import { httpClient } from './httpClient';
import type {
  AnalyticsResumoPayload,
  AnalyticsResumoApiResponse,
} from '@domain/entities/comunicacao';

/**
 * API de Analytics do módulo Marketing/Comunicação.
 * POST api/Marketing/Comunicacao/Analytics/Resumo
 */
export class ComunicacaoAnalyticsApi {
  async postResumo(
    token: string,
    payload: AnalyticsResumoPayload,
  ): Promise<AnalyticsResumoApiResponse> {
    return httpClient.post<AnalyticsResumoApiResponse>(
      '/api/Marketing/Comunicacao/Analytics/Resumo',
      payload,
      { token },
    );
  }
}
