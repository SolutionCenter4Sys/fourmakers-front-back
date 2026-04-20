import { httpClient } from './httpClient';
import type {
  ComunicacaoComunidadesApiResponse,
  ComunicacaoComunidadeDetalheApiResponse,
  CriarComunidadeApiResponse,
} from '@domain/entities/comunicacao';

/**
 * API de Comunidades do módulo Marketing/Comunicação.
 * GET api/Marketing/Comunicacao/Comunidade
 * GET api/Marketing/Comunicacao/Comunidade/{id}
 * POST api/Marketing/Comunicacao/Comunidade (multipart/form-data)
 */
export class ComunicacaoComunidadeApi {
  async getComunidades(
    token: string,
  ): Promise<ComunicacaoComunidadesApiResponse> {
    return httpClient.get<ComunicacaoComunidadesApiResponse>(
      '/api/Marketing/Comunicacao/Comunidade',
      { token },
    );
  }

  async getComunidadeById(
    token: string,
    comunidadeId: string,
  ): Promise<ComunicacaoComunidadeDetalheApiResponse> {
    const url = `/api/Marketing/Comunicacao/Comunidade/${encodeURIComponent(comunidadeId)}`;
    return httpClient.get<ComunicacaoComunidadeDetalheApiResponse>(url, {
      token,
    });
  }

  /**
   * Criar comunidade (multipart/form-data).
   * Campos: capaComunidade (file opcional), nome, descricao, tipo (publica|privada),
   * permitePostagemMembro, permiteSair, publicacaoConfiguracaoPolitica, publicacaoPermiteComentario,
   * publicacaoPermiteLikeHabilitado, codigosInternoColaboradoresModeradores, ativo.
   * Se tipo=privada: gruposComunidade (ids), codigosInternosColaboradores (ids).
   */
  async criarComunidade(
    token: string,
    formData: FormData,
  ): Promise<CriarComunidadeApiResponse> {
    return httpClient.post<CriarComunidadeApiResponse>(
      '/api/Marketing/Comunicacao/Comunidade',
      formData,
      { token },
    );
  }

  /**
   * Atualizar comunidade (mesmo endpoint que criar, método PUT).
   * Mesmo body do criar (multipart/form-data), acrescido do campo ativo.
   */
  async atualizarComunidade(
    token: string,
    comunidadeId: string,
    formData: FormData,
  ): Promise<CriarComunidadeApiResponse> {
    return httpClient.request<CriarComunidadeApiResponse>({
      url: `/api/Marketing/Comunicacao/Comunidade/${encodeURIComponent(comunidadeId)}`,
      method: 'PUT',
      body: formData,
      token,
    });
  }

  /**
   * Participar da comunidade.
   * POST api/Marketing/Comunicacao/Comunidade/{comunidadeId}/participar
   */
  async participar(
    token: string,
    comunidadeId: string,
  ): Promise<{ sucesso: boolean; mensagem: string | null }> {
    const url = `/api/Marketing/Comunicacao/Comunidade/${encodeURIComponent(comunidadeId)}/participar`;
    return httpClient.post<{ sucesso: boolean; mensagem: string | null }>(url, undefined, { token });
  }

  /**
   * Sair da comunidade (só disponível quando permiteSair = true).
   * POST api/Marketing/Comunicacao/Comunidade/{comunidadeId}/sair
   */
  async sair(
    token: string,
    comunidadeId: string,
  ): Promise<{ sucesso: boolean; mensagem: string | null }> {
    const url = `/api/Marketing/Comunicacao/Comunidade/${encodeURIComponent(comunidadeId)}/sair`;
    return httpClient.post<{ sucesso: boolean; mensagem: string | null }>(url, undefined, { token });
  }
}
