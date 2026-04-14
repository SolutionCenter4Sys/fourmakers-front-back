import { httpClient } from './httpClient';
import type {
  AssistenteIaComunicacaoApiResponse,
  AssistenteIaComunicacaoPayload,
} from '@domain/entities/comunicacao';

/**
 * API de IA do módulo Marketing/Comunicação.
 * POST api/Marketing/Comunicacao/IA/Assistente
 */
export class ComunicacaoIaApi {
  async postAssistente(
    token: string,
    payload: AssistenteIaComunicacaoPayload,
  ): Promise<AssistenteIaComunicacaoApiResponse> {
    return httpClient.post<AssistenteIaComunicacaoApiResponse>(
      '/api/Marketing/Comunicacao/IA/Assistente',
      payload,
      { token },
    );
  }
}
