import { httpClient } from "./httpClient";
import type {
  ApiGenericResult,
  PdiResumoDTO,
  PdiCompletoTimeDTO,
  PdiListagemTimeResult,
  PdiActionPlanInputDTO,
  PdiActionPlanDTO,
  PdiConcluirActionPlanResponseDTO,
  PdiEvidenciaUploadResultDTO,
  PdiEvidenciaDTO,
  PdiCriarRequestDTO,
  PdiCriarResponseDTO,
  PdiAtualizarRequestDTO,
  PdiAtualizarResponseDTO,
  PdiAprovarResponseDTO,
  PdiMetricasResultDTO,
} from "@shared/types/pdiApi";

const BASE = "/api/GestaoPessoa/Pdi";

export interface PdiMetricasFiltrosParams {
  data_inicio?: string;
  data_fim?: string;
  unidade?: string;
  gestor?: string;
  todos?: boolean;
  em_andamento?: boolean;
  nao_iniciado?: boolean;
  finalizados?: boolean;
}

export class ColaboradorPdiApi {
  /** GET /api/GestaoPessoa/Pdi/MeusPdis — Listar meus PDIs (colaborador_id = usuário logado) */
  async getMeusPdis(token: string): Promise<PdiResumoDTO[]> {
    const data = await httpClient.get<ApiGenericResult<PdiResumoDTO[]>>(
      `${BASE}/MeusPdis`,
      { token },
    );
    if (!data?.sucesso || !Array.isArray(data.retorno)) {
      throw new Error(data?.mensagem || "Erro ao listar meus PDIs.");
    }
    return data.retorno;
  }

  /**
   * Criar PDI.
   * Colaborador (meu PDI): POST MeusPdis
   * Gestor (criar para colaborador): POST PdisDoTime/colaborador/{colaboradorId}
   */
  async criarPdi(
    token: string,
    payload: PdiCriarRequestDTO,
    colaboradorId?: string,
  ): Promise<PdiCriarResponseDTO> {
    const url = colaboradorId
      ? `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}`
      : `${BASE}/MeusPdis`;
    const data = await httpClient.post<ApiGenericResult<PdiCriarResponseDTO>>(
      url,
      payload,
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao criar PDI.");
    }
    return data.retorno;
  }

  /**
   * Atualizar PDI.
   * Colaborador: PUT MeusPdis/{id}
   * Gestor: PUT PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}
   */
  async atualizarPdi(
    token: string,
    id: string,
    payload: PdiAtualizarRequestDTO,
    colaboradorId?: string,
  ): Promise<PdiAtualizarResponseDTO> {
    const url = colaboradorId
      ? `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${id}`
      : `${BASE}/MeusPdis/${id}`;
    const data = await httpClient.put<
      ApiGenericResult<PdiAtualizarResponseDTO>
    >(url, payload, { token });
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao atualizar PDI.");
    }
    return data.retorno;
  }

  /** GET /api/GestaoPessoa/Pdi/PdisDoTime — Listar PDIs do time (paginado) */
  async getPdisTime(
    token: string,
    params?: { pagina?: number; tamanhoPagina?: number },
  ): Promise<PdiListagemTimeResult> {
    const pagina = params?.pagina ?? 1;
    const tamanhoPagina = Math.min(params?.tamanhoPagina ?? 100, 100);
    const query = `pagina=${pagina}&tamanhoPagina=${tamanhoPagina}`;
    const data = await httpClient.get<ApiGenericResult<PdiListagemTimeResult>>(
      `${BASE}/PdisDoTime?${query}`,
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao listar PDIs do time.");
    }
    return data.retorno;
  }

  /** GET /api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId} — Listar PDIs de um colaborador do time */
  async getPdisTimeByColaborador(
    token: string,
    colaboradorId: string,
  ): Promise<PdiResumoDTO[]> {
    const data = await httpClient.get<ApiGenericResult<PdiResumoDTO[]>>(
      `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}`,
      { token },
    );
    if (!data?.sucesso || !Array.isArray(data.retorno)) {
      throw new Error(data?.mensagem || "Erro ao listar PDIs do colaborador.");
    }
    return data.retorno;
  }

  /** GET /api/GestaoPessoa/Pdi/PdisDoTime/{pdiId} — Obter PDI completo do time */
  async getPdiTimeById(
    token: string,
    pdiId: string,
  ): Promise<PdiCompletoTimeDTO> {
    const data = await httpClient.get<ApiGenericResult<PdiCompletoTimeDTO>>(
      `${BASE}/PdisDoTime/${pdiId}`,
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "PDI não encontrado.");
    }
    return data.retorno;
  }

  /**
   * Adicionar plano de ação.
   * Colaborador (meu PDI): POST MeusPdis/{pdiId}/action-plans
   * Gestor (PDI do colaborador): POST PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/action-plans
   */
  async addActionPlan(
    token: string,
    pdiId: string,
    payload: PdiActionPlanInputDTO,
    colaboradorId?: string,
  ): Promise<PdiActionPlanDTO> {
    const url = colaboradorId
      ? `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${pdiId}/ActionPlans`
      : `${BASE}/MeusPdis/${pdiId}/action-plans`;
    const data = await httpClient.post<ApiGenericResult<PdiActionPlanDTO>>(
      url,
      payload,
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao adicionar plano de ação.");
    }
    return data.retorno;
  }

  /**
   * Atualizar plano de ação (planejamento).
   * Sempre usa o endpoint do gestor: PUT PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/ActionPlans/{actionPlanId}.
   * Para "meu PDI", passar o colaboradorId do usuário logado.
   */
  async updateActionPlan(
    token: string,
    pdiId: string,
    actionPlanId: string,
    payload: PdiActionPlanInputDTO,
    colaboradorId: string,
  ): Promise<PdiActionPlanDTO> {
    const url = `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${pdiId}/ActionPlans/${actionPlanId}`;
    const data = await httpClient.put<ApiGenericResult<PdiActionPlanDTO>>(
      url,
      payload,
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao atualizar plano de ação.");
    }
    return data.retorno;
  }

  /**
   * Excluir plano de ação.
   * Sempre usa o endpoint do gestor: DELETE PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/ActionPlans/{actionPlanId}.
   * Para "meu PDI", passar o colaboradorId do usuário logado.
   */
  async deleteActionPlan(
    token: string,
    pdiId: string,
    actionPlanId: string,
    colaboradorId: string,
  ): Promise<void> {
    const url = `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${pdiId}/ActionPlans/${actionPlanId}`;
    const data = await httpClient.delete<ApiGenericResult<unknown>>(url, { token });
    if (!data?.sucesso) {
      throw new Error(data?.mensagem || "Erro ao excluir plano de ação.");
    }
  }

  /**
   * Concluir plano de ação.
   * Colaborador: PATCH MeusPdis/{pdiId}/action-plans/{actionPlanId}/complete
   * Gestor: PATCH PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/action-plans/{actionPlanId}/complete
   */
  async completeActionPlan(
    token: string,
    pdiId: string,
    actionPlanId: string,
    colaboradorId?: string,
  ): Promise<PdiConcluirActionPlanResponseDTO> {
    const url = colaboradorId
      ? `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${pdiId}/ActionPlans/${actionPlanId}/complete`
      : `${BASE}/MeusPdis/${pdiId}/action-plans/${actionPlanId}/complete`;
    const data = await httpClient.patch<
      ApiGenericResult<PdiConcluirActionPlanResponseDTO>
    >(url, {}, { token });
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao concluir plano de ação.");
    }
    return data.retorno;
  }

  /**
   * Upload evidência (multipart).
   * Colaborador: POST MeusPdis/{pdiId}/evidencias
   * Gestor: POST PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/evidencias
   */
  async uploadEvidencia(
    token: string,
    pdiId: string,
    formData: FormData,
    colaboradorId?: string,
  ): Promise<PdiEvidenciaUploadResultDTO> {
    const url = colaboradorId
      ? `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${pdiId}/evidencias`
      : `${BASE}/MeusPdis/${pdiId}/evidencias`;
    const data = await httpClient.request<
      ApiGenericResult<PdiEvidenciaUploadResultDTO>
    >({
      url,
      method: "POST",
      body: formData,
      token,
    });
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao enviar anexo.");
    }
    return data.retorno;
  }

  /** GET MeusPdis/{pdiId}/evidencias ou Gestor: GET PdisDoTime/colaborador/{id}/pdi/{pdiId}/evidencias */
  async getEvidencias(
    token: string,
    pdiId: string,
    colaboradorId?: string,
  ): Promise<PdiEvidenciaDTO[]> {
    const url = colaboradorId
      ? `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${pdiId}/evidencias`
      : `${BASE}/MeusPdis/${pdiId}/evidencias`;
    const data = await httpClient.get<ApiGenericResult<PdiEvidenciaDTO[]>>(
      url,
      { token },
    );
    if (!data?.sucesso || !Array.isArray(data.retorno)) {
      throw new Error(data?.mensagem || "Erro ao listar evidências.");
    }
    return data.retorno;
  }

  /**
   * Retorna o arquivo da evidência para visualização/download.
   * GET MeusPdis/{pdiId}/evidencias/{evidenciaId} ou Gestor: .../PdisDoTime/colaborador/{id}/pdi/{pdiId}/evidencias/{evidenciaId}
   * Response 200: arquivo binário (stream).
   */
  async getEvidenciaFile(
    token: string,
    pdiId: string,
    evidenciaId: string,
    colaboradorId?: string,
  ): Promise<Response> {
    const path = colaboradorId
      ? `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${pdiId}/evidencias/${evidenciaId}`
      : `${BASE}/MeusPdis/${pdiId}/evidencias/${evidenciaId}`;
    return httpClient.getBlob(path, { token });
  }

  /**
   * Gestor: aprovar PDI (criado pelo gestor; status NOT_STARTED → IN_PROGRESS).
   * PATCH /api/GestaoPessoa/Pdi/PdisDoTime/colaborador/{colaboradorId}/pdi/{pdiId}/aprovar
   */
  async aprovarPdi(
    token: string,
    colaboradorId: string,
    pdiId: string,
  ): Promise<PdiAprovarResponseDTO> {
    const url = `${BASE}/PdisDoTime/colaborador/${encodeURIComponent(colaboradorId)}/pdi/${pdiId}/aprovar`;
    const data = await httpClient.patch<ApiGenericResult<PdiAprovarResponseDTO>>(
      url,
      {},
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao aprovar PDI.");
    }
    return data.retorno;
  }

  /** Parâmetros opcionais para filtro de métricas (enviados como query string; backend pode ignorar se não suportar) */
  private static buildMetricasQuery(params?: PdiMetricasFiltrosParams): string {
    if (!params) return "";
    const q = new URLSearchParams();
    if (params.data_inicio) q.set("data_inicio", params.data_inicio);
    if (params.data_fim) q.set("data_fim", params.data_fim);
    if (params.unidade) q.set("unidade", params.unidade);
    if (params.gestor) q.set("gestor", params.gestor);
    if (params.todos !== undefined) q.set("todos", String(params.todos));
    if (params.em_andamento !== undefined) q.set("em_andamento", String(params.em_andamento));
    if (params.nao_iniciado !== undefined) q.set("nao_iniciado", String(params.nao_iniciado));
    if (params.finalizados !== undefined) q.set("finalizados", String(params.finalizados));
    const s = q.toString();
    return s ? `?${s}` : "";
  }

  /** GET /api/GestaoPessoa/Pdi/Metricas — Métricas do colaborador logado */
  async getMetricasColaborador(
    token: string,
    params?: PdiMetricasFiltrosParams
  ): Promise<PdiMetricasResultDTO> {
    const query = ColaboradorPdiApi.buildMetricasQuery(params);
    const data = await httpClient.get<ApiGenericResult<PdiMetricasResultDTO>>(
      `${BASE}/Metricas${query}`,
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao obter métricas de PDI.");
    }
    return data.retorno;
  }

  /** GET /api/GestaoPessoa/Pdi/Metricas/gestor — Métricas do time (gestor + subordinados) */
  async getMetricasGestor(
    token: string,
    params?: PdiMetricasFiltrosParams
  ): Promise<PdiMetricasResultDTO> {
    const query = ColaboradorPdiApi.buildMetricasQuery(params);
    const data = await httpClient.get<ApiGenericResult<PdiMetricasResultDTO>>(
      `${BASE}/Metricas/gestor${query}`,
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao obter métricas do time.");
    }
    return data.retorno;
  }

  /** GET /api/GestaoPessoa/Pdi/Metricas/gestor/colaborador/{colaboradorId} */
  async getMetricasGestorColaborador(
    token: string,
    colaboradorId: string,
  ): Promise<PdiMetricasResultDTO> {
    const data = await httpClient.get<ApiGenericResult<PdiMetricasResultDTO>>(
      `${BASE}/Metricas/gestor/colaborador/${encodeURIComponent(colaboradorId)}`,
      { token },
    );
    if (!data?.sucesso || !data.retorno) {
      throw new Error(data?.mensagem || "Erro ao obter métricas do colaborador.");
    }
    return data.retorno;
  }
}
