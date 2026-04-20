import { httpClient } from './httpClient';
import type {
  ArquivarPublicacaoResponse,
  AtualizarComentarioPublicacaoPayload,
  ComentarioPublicacaoResponse,
  ComunicacaoFeedApiResponse,
  ComunicacaoFeedParams,
  ConfirmarLeituraObrigatoriaPayload,
  ConfirmarLeituraObrigatoriaResponse,
  CriarPublicacaoResponse,
  InserirComentarioPublicacaoPayload,
  InserirComentarioPublicacaoResponse,
  InserirInteracaoComentarioPayload,
  InserirInteracaoComentarioResponse,
  ExcluirInteracaoComentarioPayload,
  ExcluirInteracaoComentarioResponse,
  InserirInteracaoPublicacaoPayload,
  InteracaoPublicacaoResponse,
  ObterConfirmacoesLeituraResponse,
  ObterPublicacaoResponse,
  OcultarNoFeedPayload,
} from '@domain/entities/comunicacao';

/**
 * API de Publicação do módulo Marketing/Comunicação.
 * Utiliza httpClient conforme ARCHITECTURE.md.
 */
export class ComunicacaoPublicacaoApi {
  /**
   * Obtém uma publicação pelo ID.
   * GET api/Marketing/Comunicacao/Publicacao/{publicacaoId}
   */
  async getPublicacaoById(
    token: string,
    publicacaoId: string,
  ): Promise<ObterPublicacaoResponse> {
    return httpClient.get<ObterPublicacaoResponse>(
      `/api/Marketing/Comunicacao/Publicacao/${encodeURIComponent(publicacaoId)}`,
      { token },
    );
  }

  /**
   * Lista o feed de publicações.
   * POST api/Marketing/Comunicacao/Publicacao/ObterListaPublicacaoGeral
   */
  async getFeed(
    token: string,
    params: ComunicacaoFeedParams = {},
  ): Promise<ComunicacaoFeedApiResponse> {
    const payload: Record<string, unknown> = {
      somenteLeituraObrigatoria: params.somenteLeituraObrigatoria ?? false,
      tipo: params.tipo ?? '',
      labels: params.labels ?? [],
      comunidadeId: params.comunidadeId ?? '',
    };
    if (params.pagina != null) payload.pagina = params.pagina;
    if (params.quantidadePorPagina != null) payload.quantidadePorPagina = params.quantidadePorPagina;
    if (params.somenteNaoOcultoNoFeed === true) payload.somenteNaoOcultoNoFeed = true;
    return httpClient.post<ComunicacaoFeedApiResponse>(
      '/api/Marketing/Comunicacao/Publicacao/ObterListaPublicacaoGeral',
      payload,
      { token, keepEmptyStrings: true },
    );
  }

  /**
   * Obtém confirmações de leitura de uma publicação.
   * GET api/Marketing/Comunicacao/Publicacao/{publicacaoId}/ConfirmacoesLeitura
   */
  async getConfirmacoesLeitura(
    token: string,
    publicacaoId: string,
  ): Promise<ObterConfirmacoesLeituraResponse> {
    const url = `/api/Marketing/Comunicacao/Publicacao/${encodeURIComponent(publicacaoId)}/ConfirmacoesLeitura`;
    return httpClient.get<ObterConfirmacoesLeituraResponse>(url, { token });
  }

  /**
   * Confirma leitura obrigatória de uma publicação.
   * POST api/Marketing/Comunicacao/Publicacao/ConfirmarLeituraObrigatoria
   */
  async confirmarLeituraObrigatoria(
    token: string,
    payload: ConfirmarLeituraObrigatoriaPayload,
  ): Promise<ConfirmarLeituraObrigatoriaResponse> {
    return httpClient.post<ConfirmarLeituraObrigatoriaResponse>(
      '/api/Marketing/Comunicacao/Publicacao/ConfirmarLeituraObrigatoria',
      payload,
      { token },
    );
  }

  /**
   * Oculta ou exibe publicação no feed.
   * PATCH api/Marketing/Comunicacao/Publicacao/OcultarNoFeed
   */
  async ocultarNoFeed(
    token: string,
    payload: OcultarNoFeedPayload,
  ): Promise<ArquivarPublicacaoResponse> {
    return httpClient.patch<ArquivarPublicacaoResponse>(
      '/api/Marketing/Comunicacao/Publicacao/OcultarNoFeed',
      payload,
      { token },
    );
  }

  /**
   * Arquivar uma publicação.
   * POST api/Marketing/Comunicacao/Publicacao/{publicacaoId}/Arquivar
   */
  async arquivar(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    const url = `/api/Marketing/Comunicacao/Publicacao/${encodeURIComponent(publicacaoId)}/Arquivar`;
    return httpClient.post<ArquivarPublicacaoResponse>(url, undefined, {
      token,
    });
  }

  /**
   * Excluir uma publicação.
   * DELETE api/Marketing/Comunicacao/Publicacao/{publicacaoId}
   */
  async excluir(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    const url = `/api/Marketing/Comunicacao/Publicacao/${encodeURIComponent(publicacaoId)}`;
    return httpClient.delete<ArquivarPublicacaoResponse>(url, { token });
  }

  /**
   * Publicar agora uma publicação em rascunho.
   * POST api/Marketing/Comunicacao/PublicacaoGerencial/{publicacaoId}/PublicarAgora
   */
  async publicarAgora(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    const url = `/api/Marketing/Comunicacao/PublicacaoGerencial/${encodeURIComponent(publicacaoId)}/PublicarAgora`;
    return httpClient.post<ArquivarPublicacaoResponse>(url, undefined, {
      token,
    });
  }

  /**
   * Aprovar uma publicação (aprovacao_status pendente -> aprovado).
   * POST api/Marketing/Comunicacao/PublicacaoGerencial/{publicacaoId}/Aprovar
   */
  async aprovar(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    const url = `/api/Marketing/Comunicacao/PublicacaoGerencial/${encodeURIComponent(publicacaoId)}/Aprovar`;
    return httpClient.post<ArquivarPublicacaoResponse>(url, undefined, {
      token,
    });
  }

  /**
   * Rejeitar uma publicação (aprovacao_status pendente -> rejeitado).
   * POST api/Marketing/Comunicacao/PublicacaoGerencial/{publicacaoId}/Rejeitar
   * Body: { motivoRejeicao: string }
   */
  async rejeitar(
    token: string,
    publicacaoId: string,
    motivoRejeicao: string,
  ): Promise<ArquivarPublicacaoResponse> {
    const url = `/api/Marketing/Comunicacao/PublicacaoGerencial/${encodeURIComponent(publicacaoId)}/Rejeitar`;
    return httpClient.post<ArquivarPublicacaoResponse>(
      url,
      { motivoRejeicao },
      { token },
    );
  }

  /**
   * Criar uma publicação (multipart/form-data).
   * POST api/Marketing/Comunicacao/Publicacao
   * FormData: tipo, titulo, conteudo, comunidadeId, anexos[i].tipo, anexos[i].nomeArquivo, anexosUpload (files), configuracaoInteracao.*
   */
  async criarPublicacao(
    token: string,
    formData: FormData,
  ): Promise<CriarPublicacaoResponse> {
    return httpClient.post<CriarPublicacaoResponse>(
      '/api/Marketing/Comunicacao/Publicacao',
      formData,
      { token },
    );
  }

  /**
   * Atualizar uma publicação.
   * PUT api/Marketing/Comunicacao/Publicacao (publicacaoId no body)
   */
  async atualizarPublicacao(
    token: string,
    publicacaoId: string,
    formData: FormData,
  ): Promise<CriarPublicacaoResponse> {
    formData.append('publicacaoId', publicacaoId);
    return httpClient.request<CriarPublicacaoResponse>({
      url: '/api/Marketing/Comunicacao/Publicacao',
      method: 'PUT',
      body: formData,
      token,
    });
  }

  /**
   * Excluir um anexo de uma publicação.
   * DELETE api/Marketing/Comunicacao/Publicacao/{publicacaoId}/Anexos/{anexoId}
   */
  async excluirAnexo(
    token: string,
    publicacaoId: string,
    anexoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    const url = `/api/Marketing/Comunicacao/Publicacao/${encodeURIComponent(publicacaoId)}/Anexos/${encodeURIComponent(anexoId)}`;
    return httpClient.delete<ArquivarPublicacaoResponse>(url, { token });
  }

  /**
   * Adicionar anexos a uma publicação.
   * POST api/Marketing/Comunicacao/Publicacao/{publicacaoId}/Anexos
   * FormData: anexos[0].tipo, anexos[1].tipo, ..., anexosUpload (files)
   */
  async adicionarAnexos(
    token: string,
    publicacaoId: string,
    formData: FormData,
  ): Promise<ArquivarPublicacaoResponse> {
    const url = `/api/Marketing/Comunicacao/Publicacao/${encodeURIComponent(publicacaoId)}/Anexos`;
    return httpClient.post<ArquivarPublicacaoResponse>(url, formData, { token });
  }

  /**
   * Inserir comentário em uma publicação.
   * POST api/Marketing/Comunicacao/PublicacaoComentario/Comentarios
   */
  async inserirComentario(
    token: string,
    payload: InserirComentarioPublicacaoPayload,
  ): Promise<InserirComentarioPublicacaoResponse> {
    return httpClient.post<InserirComentarioPublicacaoResponse>(
      '/api/Marketing/Comunicacao/PublicacaoComentario/Comentarios',
      payload,
      { token },
    );
  }

  /**
   * Atualizar comentário de uma publicação.
   * PUT api/Marketing/Comunicacao/PublicacaoComentario/Comentarios
   */
  async atualizarComentario(
    token: string,
    payload: AtualizarComentarioPublicacaoPayload,
  ): Promise<ComentarioPublicacaoResponse> {
    return httpClient.put<ComentarioPublicacaoResponse>(
      '/api/Marketing/Comunicacao/PublicacaoComentario/Comentarios',
      payload,
      { token },
    );
  }

  /**
   * Excluir comentário de uma publicação.
   * DELETE api/Marketing/Comunicacao/PublicacaoComentario/{publicacaoId}/Comentarios/{comentarioId}
   */
  async excluirComentario(
    token: string,
    publicacaoId: string,
    comentarioId: string,
  ): Promise<ComentarioPublicacaoResponse> {
    const url = `/api/Marketing/Comunicacao/PublicacaoComentario/${encodeURIComponent(publicacaoId)}/Comentarios/${encodeURIComponent(comentarioId)}`;
    return httpClient.delete<ComentarioPublicacaoResponse>(url, { token });
  }

  /**
   * Inserir interação (emoji) em comentário/publicação.
   * POST api/Marketing/Comunicacao/PublicacaoComentario/Comentarios/Interacao
   */
  async inserirInteracaoComentario(
    token: string,
    payload: InserirInteracaoComentarioPayload,
  ): Promise<InserirInteracaoComentarioResponse> {
    return httpClient.post<InserirInteracaoComentarioResponse>(
      '/api/Marketing/Comunicacao/PublicacaoComentario/Comentarios/Interacao',
      payload,
      { token },
    );
  }

  /**
   * Remover interação (emoji) de comentário/publicação.
   * DELETE api/Marketing/Comunicacao/PublicacaoComentario/Comentarios/Interacao
   * Body: { comentarioId: string }
   */
  async excluirInteracaoComentario(
    token: string,
    payload: ExcluirInteracaoComentarioPayload,
  ): Promise<ExcluirInteracaoComentarioResponse> {
    return httpClient.request<ExcluirInteracaoComentarioResponse>({
      url: '/api/Marketing/Comunicacao/PublicacaoComentario/Comentarios/Interacao',
      method: 'DELETE',
      body: JSON.stringify(payload),
      token,
    });
  }

  /**
   * Inserir interação (emoji) em publicação.
   * POST api/Marketing/Comunicacao/Publicacao/Interacao
   * Body: { publicacaoId: string, emoji?: string }
   */
  async inserirInteracaoPublicacao(
    token: string,
    payload: InserirInteracaoPublicacaoPayload,
  ): Promise<InteracaoPublicacaoResponse> {
    return httpClient.post<InteracaoPublicacaoResponse>(
      '/api/Marketing/Comunicacao/Publicacao/Interacao',
      payload,
      { token },
    );
  }

  /**
   * Remover interação (emoji) de publicação.
   * DELETE api/Marketing/Comunicacao/Publicacao/Interacao
   * Body: { publicacaoId: string }
   */
  async excluirInteracaoPublicacao(
    token: string,
    payload: { publicacaoId: string },
  ): Promise<InteracaoPublicacaoResponse> {
    return httpClient.request<InteracaoPublicacaoResponse>({
      url: '/api/Marketing/Comunicacao/Publicacao/Interacao',
      method: 'DELETE',
      body: JSON.stringify(payload),
      token,
    });
  }
}
