import { httpClient } from './httpClient';

export interface ComunicacaoLabelItem {
  id: string;
  nome: string;
}

export interface ComunicacaoLabelListApiResponse {
  retorno: ComunicacaoLabelItem[];
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/**
 * API de Label do módulo Marketing/Comunicação.
 * GET, PUT e DELETE em api/Marketing/Comunicacao/Label.
 */
export class ComunicacaoLabelApi {
  /**
   * Lista todas as labels (tags).
   * GET api/Marketing/Comunicacao/Label
   */
  async listar(token: string): Promise<ComunicacaoLabelItem[]> {
    const response = await httpClient.get<ComunicacaoLabelListApiResponse>(
      '/api/Marketing/Comunicacao/Label',
      { token },
    );
    if (!response.sucesso || !response.retorno) return [];
    return response.retorno;
  }

  /**
   * Atualiza o nome de uma label.
   * PUT api/Marketing/Comunicacao/Label/{labelId}
   */
  async atualizar(
    token: string,
    labelId: string,
    payload: { nome: string },
  ): Promise<void> {
    await httpClient.put(
      `/api/Marketing/Comunicacao/Label/${encodeURIComponent(labelId)}`,
      payload,
      { token },
    );
  }

  /**
   * Exclui uma label (somente se não tiver documentos).
   * DELETE api/Marketing/Comunicacao/Label/{labelId}
   */
  async excluir(token: string, labelId: string): Promise<void> {
    await httpClient.delete(
      `/api/Marketing/Comunicacao/Label/${encodeURIComponent(labelId)}`,
      { token },
    );
  }
}
