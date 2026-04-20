import { httpClient } from './httpClient';
import type {
  ComunicacaoGruposApiResponse,
  ColaboradoresDisponiveisApiResponse,
  CriarGrupoPayload,
  CriarGrupoResponse,
  ComunicacaoGrupoDetalheApiResponse,
  PermissoesUsuarioLogadoApiResponse,
} from '@domain/entities/comunicacao';

/**
 * API de Grupos do módulo Marketing/Comunicação.
 * GET api/Marketing/Comunicacao/Grupo
 * POST api/Marketing/Comunicacao/Grupo
 * GET api/Marketing/Comunicacao/Grupo/ColaboradoresDisponiveis
 */
export class ComunicacaoGrupoApi {
  async getGrupos(token: string): Promise<ComunicacaoGruposApiResponse> {
    return httpClient.get<ComunicacaoGruposApiResponse>(
      '/api/Marketing/Comunicacao/Grupo',
      { token },
    );
  }

  /**
   * Permissões do usuário logado no módulo de comunicação.
   * GET api/Marketing/Comunicacao/Grupo/PermissoesUsuarioLogado
   */
  async getPermissoesUsuarioLogado(
    token: string,
  ): Promise<PermissoesUsuarioLogadoApiResponse> {
    return httpClient.get<PermissoesUsuarioLogadoApiResponse>(
      '/api/Marketing/Comunicacao/Grupo/PermissoesUsuarioLogado',
      { token },
    );
  }

  /**
   * Cria um novo grupo.
   * POST api/Marketing/Comunicacao/Grupo
   */
  async criarGrupo(
    token: string,
    payload: CriarGrupoPayload,
  ): Promise<CriarGrupoResponse> {
    return httpClient.post<CriarGrupoResponse>(
      '/api/Marketing/Comunicacao/Grupo',
      payload,
      { token },
    );
  }

  /**
   * Obtém um grupo por id.
   * GET api/Marketing/Comunicacao/Grupo/{id}
   */
  async getGrupoById(
    token: string,
    grupoId: string,
  ): Promise<ComunicacaoGrupoDetalheApiResponse> {
    const url = `/api/Marketing/Comunicacao/Grupo/${encodeURIComponent(grupoId)}`;
    return httpClient.get<ComunicacaoGrupoDetalheApiResponse>(url, { token });
  }

  /**
   * Atualiza um grupo.
   * PUT api/Marketing/Comunicacao/Grupo/{id}
   */
  async atualizarGrupo(
    token: string,
    grupoId: string,
    payload: CriarGrupoPayload,
  ): Promise<CriarGrupoResponse> {
    const url = `/api/Marketing/Comunicacao/Grupo/${encodeURIComponent(grupoId)}`;
    return httpClient.put<CriarGrupoResponse>(url, payload, { token });
  }

  /**
   * Exclui um grupo.
   * DELETE api/Marketing/Comunicacao/Grupo/{id}
   */
  async deletarGrupo(
    token: string,
    grupoId: string,
  ): Promise<CriarGrupoResponse> {
    const url = `/api/Marketing/Comunicacao/Grupo/${encodeURIComponent(grupoId)}`;
    return httpClient.delete<CriarGrupoResponse>(url, { token });
  }

  async getColaboradoresDisponiveis(
    token: string,
    params: { grupoId?: string; filtro?: string } = {},
  ): Promise<ColaboradoresDisponiveisApiResponse> {
    const search = new URLSearchParams();
    if (params.grupoId != null) search.set('grupoId', params.grupoId);
    if (params.filtro != null) search.set('filtro', params.filtro);
    const query = search.toString();
    const url = `/api/Marketing/Comunicacao/Grupo/ColaboradoresDisponiveis${query ? `?${query}` : ''}`;
    return httpClient.get<ColaboradoresDisponiveisApiResponse>(url, { token });
  }
}
