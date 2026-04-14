import { httpClient } from './httpClient';
import type { ComunicacaoProfissionaisApiResponse } from '@domain/entities/comunicacao';

/**
 * API de Profissionais do módulo Marketing/Comunicação.
 * GET api/Marketing/Comunicacao/Profissionais
 * POST .../Profissionais/Favoritar?codigoInternoColaborador=
 * POST .../Profissionais/Desfavoritar?codigoInternoColaborador=
 */
export class ComunicacaoProfissionaisApi {
  async getProfissionais(
    token: string,
  ): Promise<ComunicacaoProfissionaisApiResponse> {
    return httpClient.get<ComunicacaoProfissionaisApiResponse>(
      '/api/Marketing/Comunicacao/Profissionais',
      { token },
    );
  }

  async favoritar(
    token: string,
    codigoInternoColaborador: string,
  ): Promise<{ sucesso?: boolean }> {
    const url = `/api/Marketing/Comunicacao/Profissionais/Favoritar?codigoInternoColaborador=${encodeURIComponent(codigoInternoColaborador)}`;
    return httpClient.post<{ sucesso?: boolean }>(url, {}, { token });
  }

  async desfavoritar(
    token: string,
    codigoInternoColaborador: string,
  ): Promise<{ sucesso?: boolean }> {
    const url = `/api/Marketing/Comunicacao/Profissionais/Desfavoritar?codigoInternoColaborador=${encodeURIComponent(codigoInternoColaborador)}`;
    return httpClient.post<{ sucesso?: boolean }>(url, {}, { token });
  }
}
