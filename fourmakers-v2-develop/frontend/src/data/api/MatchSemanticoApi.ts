import { injectable } from 'tsyringe';
import { createHttpClient } from './httpClient';
import type {
  BuscarMelhoresCandidatosPayload,
  BuscarMelhoresCandidatosResponse,
  RegistrarFeedbackPayload,
} from '@domain/repositories/MatchSemanticoRepository';

const HUB_BASE = (import.meta.env.VITE_API_FOURMAKERS_IA_URL ?? import.meta.env.VITE_API_FOURMAKERS_URL ?? 'https://fourmakershub-api.dev.fourmakers.io').replace(/\/api\/?$/, '');
const hubClient = createHttpClient({ baseURL: `${HUB_BASE}/api` });

@injectable()
export class MatchSemanticoApi {
  /**
   * POST /api/Labs/MatchSemantico/BuscarMelhoresCandidatos
   * Retorno a ser mapeado no futuro para MatchCardItem[].
   */
  async buscarMelhoresCandidatos(
    token: string,
    payload: BuscarMelhoresCandidatosPayload
  ): Promise<BuscarMelhoresCandidatosResponse> {
    const data = await hubClient.post<BuscarMelhoresCandidatosResponse>(
      '/Labs/MatchSemantico/BuscarMelhoresCandidatos',
      payload,
      { token }
    );
    return data;
  }

  /**
   * POST /api/Labs/MatchSemantico/RegistrarFeedback
   * Registra preferência do usuário (Motor Atual vs Motor Beta) para o par de buscas atual.
   */
  async registrarFeedback(token: string, payload: RegistrarFeedbackPayload): Promise<void> {
    await hubClient.post('/Labs/MatchSemantico/RegistrarFeedback', payload, { token });
  }
}
