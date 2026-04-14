import { httpClient } from './httpClient';
import type {
  Feedback360AvaliacaoItem,
  Feedback360RelacionamentoItem,
  Feedback360Item,
  Feedback360CriarPayload,
  Feedback360AtualizarPayload,
} from '@domain/entities/Feedback360';

const BASE_PATH = '/api/Social/Feedback360';

/** Envelope genérico de resposta da API (retorno, sucesso, mensagem, erros). */
interface ApiEnvelope<T> {
  retorno?: T | null;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/**
 * API de Feedback 360 — OpenAPI api/Social/Feedback360.
 * Utiliza httpClient conforme ARCHITECTURE.md.
 */
export class Feedback360Api {
  async listarAvaliacoes(token: string): Promise<Feedback360AvaliacaoItem[]> {
    const res = await httpClient.get<ApiEnvelope<Feedback360AvaliacaoItem[]>>(
      `${BASE_PATH}/ListarAvaliacoes`,
      { token },
    );
    return res?.retorno ?? [];
  }

  async listarRelacionamentos(token: string): Promise<Feedback360RelacionamentoItem[]> {
    const res = await httpClient.get<ApiEnvelope<Feedback360RelacionamentoItem[]>>(
      `${BASE_PATH}/ListarRelacionamentos`,
      { token },
    );
    return res?.retorno ?? [];
  }

  async listarEnviados(token: string): Promise<ApiEnvelope<Feedback360Item[]>> {
    return httpClient.get<ApiEnvelope<Feedback360Item[]>>(
      `${BASE_PATH}/ListarEnviados`,
      { token },
    );
  }

  async listarRecebidos(token: string): Promise<ApiEnvelope<Feedback360Item[]>> {
    return httpClient.get<ApiEnvelope<Feedback360Item[]>>(
      `${BASE_PATH}/ListarRecebidos`,
      { token },
    );
  }

  async criar(token: string, payload: Feedback360CriarPayload): Promise<ApiEnvelope<Feedback360Item>> {
    return httpClient.post<ApiEnvelope<Feedback360Item>>(
      `${BASE_PATH}/CriarFeedback`,
      payload,
      { token },
    );
  }

  async atualizar(
    token: string,
    id: string,
    payload: Feedback360AtualizarPayload,
  ): Promise<ApiEnvelope<Feedback360Item>> {
    return httpClient.put<ApiEnvelope<Feedback360Item>>(
      `${BASE_PATH}/AtualizarFeedback?id=${encodeURIComponent(id)}`,
      payload,
      { token },
    );
  }
}
