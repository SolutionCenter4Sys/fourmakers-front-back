import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';
import { API_BASE_URL } from '@shared/constants';
import type { CandidaturaDetails, CandidaturaEditPayload } from '@domain/entities/CandidaturaDetails';
import type { InserirComentarioCandidaturaPayload } from '@domain/entities/GestaoVagasCandidatos';
import { isCepComplete, normalizeCep } from '@shared/utils/calculations';

/** Parâmetros para InserirArquivo (comentário da candidatura – ex.: anexo à movimentação de status). */
export interface InserirArquivoCandidaturaParams {
  idCandidatura: string;
  idComentario: string;
  file: File;
  /** Nome lógico do arquivo (ex.: "arquivoMovimentacaoStatus"). Default: "arquivoMovimentacaoStatus". */
  nomeArquivo?: string;
  /** Tipo/categoria do arquivo (ex.: "3"). Default: "3". */
  tipoArquivo?: string;
}

/** Resposta da API InserirArquivo. */
export interface InserirArquivoCandidaturaResponse {
  retorno?: unknown;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Resposta da API de comentário da jornada. */
export interface InserirComentarioCandidaturaResponse {
  retorno?: {
    id?: string;
    texto?: string;
    dataCriacao?: string;
    dataAlteracao?: string | null;
    codigoInternoColaborador?: string;
    codigoInternoColaboradorNome?: string | null;
    candidaturaId?: string;
    arquivos?: unknown[] | null;
  };
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface CandidaturaApiResponse {
  retorno?: CandidaturaDetails;
}

/** Resposta da API ao editar candidatura (salvar dados). */
export interface CandidaturaEditApiResponse {
  retorno?: unknown;
  sucesso?: boolean;
  mensagem?: string;
  erros?: string[] | null;
}

/** Retorno da API ObterCandidaturaPorId (estrutura real da API para template de contratação). */
export interface ObterCandidaturaPorIdRetorno {
  id?: string | null;
  codColaborador?: string | null;
  idVaga?: string | null;
  orgId?: number | null;
  ativa?: boolean | null;
  tituloVaga?: string | null;
  /** Data da candidatura (ISO). Mapeado para dataAbertura na UI. */
  candidatura?: string | null;
  statusId?: string | null;
  statusDescricao?: string | null;
  ultimaAlteracao?: string | null;
  pretencaoSalarial?: number | string | null;
  modeloTrabalhoId?: string | null;
  disponibilidadeEntrevistaId?: string | null;
  quantidadeDiasPresencial?: number | null;
  colaboradorResponsavel?: string | null;
  /** Observações internas, se retornadas pela API. */
  observacoesInternas?: string | null;
  /** Campos opcionais se a API passar formato alternativo. */
  codigoVaga?: number | string | null;
  requisicao?: string | null;
  tipoContratacao?: string | null;
  tipoVaga?: string | null;
  nomeCliente?: string | null;
  requisitante?: string | null;
  dataAbertura?: string | null;
  descricaoBreveVaga?: string | null;
  status?: string | null;
  [key: string]: unknown;
}

export interface ObterCandidaturaPorIdResponse {
  retorno?: ObterCandidaturaPorIdRetorno;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Payload para AtualizarCandidatura (PUT) – modelo de trabalho, pretensão salarial e dias presenciais (ex.: ao mover para status 3). */
export interface AtualizarCandidaturaPayload {
  idCandidatura: string;
  /** Valor numérico com ponto decimal, ex.: "3400.00" */
  pretencaoSalarial: string;
  modeloTrabalhoId: string;
  /** Quantidade de dias presenciais (1–4 quando modelo é híbrido; 0 quando não). */
  quantidadeDiasPresencial: number;
}

/** Resposta da API AtualizarCandidatura. */
export interface AtualizarCandidaturaResponse {
  retorno?: unknown;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Item retornado por ListarMotivosReprovacao. */
export interface MotivoReprovacaoItem {
  id: string;
  descricao: string;
  ativo: boolean;
  dataCriacao?: string;
  dataAlteracao?: string;
}

/** Resposta da API ListarMotivosReprovacao. */
export interface ListarMotivosReprovacaoResponse {
  retorno?: MotivoReprovacaoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Payload para ReprovarCandidatura (POST) – status 10. */
export interface ReprovarCandidaturaPayload {
  idCandidatura: string;
  comentario: string;
  idMotivoReprovacao: string;
}

/** Resposta da API ReprovarCandidatura. */
export interface ReprovarCandidaturaResponse {
  retorno?: boolean;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Item retornado por ListarMotivosDeclinio. */
export interface MotivoDeclinioItem {
  id: string;
  descricao: string;
  ativo: boolean;
  dataCriacao?: string;
  dataAlteracao?: string;
}

/** Resposta da API ListarMotivosDeclinio. */
export interface ListarMotivosDeclinioResponse {
  retorno?: MotivoDeclinioItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Payload para DeclinarCandidato (POST) – status 12. Comentário opcional. */
export interface DeclinarCandidaturaPayload {
  idCandidatura: string;
  idMotivoDeclinio: string;
  comentario: string;
}

/** Resposta da API DeclinarCandidato. */
export interface DeclinarCandidaturaResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Item retornado pela API ListarMeusTalentos. */
export interface MeuTalentoItem {
  nomeColaborador: string;
  codigoVaga: string;
  nomeVaga: string;
  nomeCliente: string;
  nomeGestor: string;
  statusVaga: string;
  statusMovimentacao: string;
  ultimaAlteracao: string;
  codigoColaboradorUltimaMovimentacao: string | null;
  recrutadorUltimaMovimentacao: string;
  codColaborador: string;
  criadoPor: string;
}

/** Resposta da API ListarMeusTalentos. */
export interface ListarMeusTalentosResponse {
  retorno?: MeuTalentoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Item retornado por BuscarCandidaturasColaborador (detalhe por colaborador). */
export interface CandidaturaColaboradorItemApi {
  codigo: number;
  titulo: string;
  dataCriacao: string | null;
  dataUltimaAlteracao: string | null;
  statusCandidaturaId: string;
  statusCandidaturaDescricao: string;
  candidaturaId: string;
  pretensaoSalarial: string | null;
  modeloTrabalhoId: string | null;
  disponibilidadeEntrevistaId: string | null;
  quantidadeDiasPresencial: number | null;
  nomeCliente: string;
  codigoCliente: string;
  codigoGestor: string;
  nomeGestor: string;
  idVaga: string;
  statusVaga: string;
  dataCandidatura: string | null;
  codigoInternoColaborador: string | null;
}

/** Resposta da API BuscarCandidaturasColaborador. */
export interface BuscarCandidaturasColaboradorResponse {
  retorno?: CandidaturaColaboradorItemApi[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Resposta da API InserirComentarioCandidatura (ComentariosCandidatura). */
export interface InserirComentarioCandidaturaResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Converte payload de domínio (snake_case em endereco) para formato da API (camelCase). */
function toApiPayload(payload: CandidaturaEditPayload): Record<string, unknown> {
  const colaborador = payload.colaborador as unknown as Record<string, unknown>;
  const endereco = colaborador?.endereco as Record<string, unknown> | null | undefined;
  const enderecoApi =
    endereco && typeof endereco === 'object'
      ? {
          id: endereco.id,
          cep: typeof endereco.cep === 'string' && endereco.cep.trim() && isCepComplete(endereco.cep) ? normalizeCep(endereco.cep) : '',
          endereco: endereco.endereco,
          complemento: endereco.complemento,
          numero: endereco.numero,
          bairro: endereco.bairro,
          cidade: endereco.cidade,
          estado: endereco.estado,
          comQuemMora: endereco.com_quem_mora ?? endereco.comQuemMora,
          internacionalLinhaUm: endereco.internacional_linha_um ?? endereco.internacionalLinhaUm,
          internacionalLinhaDois: endereco.internacional_linha_dois ?? endereco.internacionalLinhaDois,
        }
      : endereco;
  return {
    ...payload,
    colaborador: {
      ...colaborador,
      endereco: enderecoApi,
    },
  } as Record<string, unknown>;
}

@injectable()
export class CandidaturaApi {
  async getCandidaturaDetails(
    token: string,
    candidateId: string,
    candidaturaId?: string,
  ): Promise<CandidaturaApiResponse> {
    const params = new URLSearchParams({ codigoInternoColaborador: candidateId });
    if (candidaturaId?.trim()) {
      params.set('idCandidatura', candidaturaId.trim());
    }
    const url = `/api/Candidatura/ObterDadosParaEditarCandidaturaColaboradorComDadosDemograficos?${params.toString()}`;
    return httpClient.get<CandidaturaApiResponse>(url, { token });
  }

  async editCandidaturaDetails(
    token: string,
    payload: CandidaturaEditPayload,
  ): Promise<CandidaturaEditApiResponse> {
    const body = toApiPayload(payload);
    const response = await httpClient.post<CandidaturaEditApiResponse>(
      '/api/Candidatura/EditarCandidaturaColaboradorComDadosDemograficos',
      body,
      { token },
    );
    return response ?? { sucesso: true };
  }

  async listarMeusTalentos(token: string): Promise<ListarMeusTalentosResponse> {
    return httpClient.get<ListarMeusTalentosResponse>('/api/Candidatura/ListarMeusTalentos', {
      token,
    });
  }

  /**
   * Busca candidaturas do colaborador (Candidaturas FMU – detalhe ao expandir linha).
   * GET /api/Candidatura/BuscarCandidaturasColaborador?codColaborador=...
   */
  async buscarCandidaturasColaborador(
    token: string,
    codColaborador: string
  ): Promise<BuscarCandidaturasColaboradorResponse> {
    const params = new URLSearchParams({ codColaborador });
    return httpClient.get<BuscarCandidaturasColaboradorResponse>(
      `/api/Candidatura/BuscarCandidaturasColaborador?${params.toString()}`,
      { token }
    );
  }

  /**
   * Atualiza dados da candidatura (modelo de trabalho, pretensão salarial, dias presenciais).
   * Deve ser chamado antes de MudarStatusCandidatura quando o status destino for 3.
   * PUT /api/Candidatura/AtualizarCandidatura
   */
  async atualizarCandidatura(
    token: string,
    payload: AtualizarCandidaturaPayload
  ): Promise<AtualizarCandidaturaResponse> {
    return httpClient.put<AtualizarCandidaturaResponse>('/api/Candidatura/AtualizarCandidatura', payload, {
      token,
    });
  }

  /**
   * Lista motivos de reprovação para uso ao mover candidato para status 10.
   * GET /api/Candidatura/ListarMotivosReprovacao
   */
  async listarMotivosReprovacao(token: string): Promise<MotivoReprovacaoItem[]> {
    const res = await httpClient.get<ListarMotivosReprovacaoResponse>(
      '/api/Candidatura/ListarMotivosReprovacao',
      { token }
    );
    const list = res?.retorno ?? [];
    return Array.isArray(list) ? list.filter((m) => m.ativo !== false) : [];
  }

  /**
   * Reprova candidatura (mover para status 10). Chamar em vez de MudarStatusCandidatura quando destino for 10.
   * POST /api/Candidatura/ReprovarCandidatura
   */
  async reprovarCandidatura(
    token: string,
    payload: ReprovarCandidaturaPayload
  ): Promise<ReprovarCandidaturaResponse> {
    return httpClient.post<ReprovarCandidaturaResponse>('/api/Candidatura/ReprovarCandidatura', payload, {
      token,
    });
  }

  /**
   * Lista motivos de declínio para uso ao mover candidato para status 12.
   * GET /api/Candidatura/ListarMotivosDeclinio
   */
  async listarMotivosDeclinio(token: string): Promise<MotivoDeclinioItem[]> {
    const res = await httpClient.get<ListarMotivosDeclinioResponse>(
      '/api/Candidatura/ListarMotivosDeclinio',
      { token }
    );
    const list = res?.retorno ?? [];
    return Array.isArray(list) ? list.filter((m) => m.ativo !== false) : [];
  }

  /**
   * Atualiza o recrutador responsável da candidatura.
   * PUT /api/Candidatura/AtualizarRecrutadorResponsavel?idCandidatura=...&codigoRecrutadorResponsavel=...
   */
  async atualizarRecrutadorResponsavel(
    token: string,
    idCandidatura: string,
    codigoRecrutadorResponsavel: string
  ): Promise<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    const params = new URLSearchParams({
      idCandidatura,
      codigoRecrutadorResponsavel,
    });
    return httpClient.put<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }>(
      `/api/Candidatura/AtualizarRecrutadorResponsavel?${params.toString()}`,
      {},
      { token }
    );
  }

  /**
   * Declina candidatura (mover para status 12). Comentário opcional.
   * POST /api/Candidatura/DeclinarCandidato
   */
  async declinarCandidato(
    token: string,
    payload: DeclinarCandidaturaPayload
  ): Promise<DeclinarCandidaturaResponse> {
    return httpClient.post<DeclinarCandidaturaResponse>('/api/Candidatura/DeclinarCandidato', payload, {
      token,
    });
  }

  /**
   * Obtém dados da candidatura por id (para exibir detalhes da vaga no template de contratação).
   * GET /api/Candidatura/ObterCandidaturaPorId?idCandidatura=...
   */
  async obterCandidaturaPorId(
    token: string,
    idCandidatura: string
  ): Promise<ObterCandidaturaPorIdResponse> {
    const params = new URLSearchParams({ idCandidatura });
    return httpClient.get<ObterCandidaturaPorIdResponse>(
      `/api/Candidatura/ObterCandidaturaPorId?${params.toString()}`,
      { token }
    );
  }

  /**
   * Insere arquivo em candidatura (ex.: anexo ao comentário de movimentação de status).
   * POST /api/Candidatura/InserirArquivo (multipart/form-data: arquivoParam, token, files).
   */
  async inserirArquivo(
    token: string,
    params: InserirArquivoCandidaturaParams
  ): Promise<InserirArquivoCandidaturaResponse> {
    const formData = new FormData();
    const arquivoParam = {
      idCandidatura: params.idCandidatura,
      idComentario: params.idComentario,
      nomeArquivo: params.nomeArquivo ?? 'arquivoMovimentacaoStatus',
      tipoArquivo: params.tipoArquivo ?? '3',
    };
    formData.append('arquivoParam', JSON.stringify(arquivoParam));
    formData.append('token', token);
    formData.append('files', params.file, params.file.name);

    const base = API_BASE_URL && API_BASE_URL.trim() !== '' ? API_BASE_URL.replace(/\/$/, '') : '';
    const url = base ? `${base}/api/Candidatura/InserirArquivo` : '/api/Candidatura/InserirArquivo';

    const response = await fetch(url, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: formData,
    });

    if (!response.ok) {
      let message = `Falha ao enviar arquivo: ${response.status}`;
      try {
        const data = await response.json();
        if (data?.mensagem) message = data.mensagem;
      } catch {
        // ignore
      }
      throw new Error(message);
    }

    try {
      return (await response.json()) as InserirArquivoCandidaturaResponse;
    } catch {
      return { sucesso: true };
    }
  }

  /**
   * Insere comentário em candidatura (ex.: movimentação de status).
   * POST /api/Social/ComentariosCandidatura
   */
  async inserirComentarioCandidatura(
    token: string,
    payload: InserirComentarioCandidaturaPayload
  ): Promise<InserirComentarioCandidaturaResponse> {
    return httpClient.post<InserirComentarioCandidaturaResponse>(
      '/api/Social/ComentariosCandidatura',
      payload,
      { token }
    );
  }
}
