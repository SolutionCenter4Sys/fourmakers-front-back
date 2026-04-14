import { XANO_BASE_URL } from '@shared/constants';
import { getXanoHeaders } from '@shared/utils/xanoUtils';
import { createHttpClient } from './httpClient';

/**
 * API antiga Xano (em uso até a nova API subir):
 * - GET /pdi/metricas/?... → resumo (pdis_criados, pdis_finalizados, etc.)
 * - GET /pdi/relatorio/?... → lista de PDIs do relatório
 * - GET /pdi/{id} → detalhe do PDI (modal)
 */
const xanoClient = createHttpClient({ baseURL: XANO_BASE_URL });

/** Resposta do endpoint Xano /pdi/metricas/ */
export interface PdiMetricasResponse {
  pdis_criados: number;
  pdis_finalizados: number;
  gestores_envolvidos: number;
  colaboradores_envolvidos: number;
}

/** Parâmetros para métricas e relatório (API antiga Xano) */
export interface PdiMetricasRelatorioParams {
  data_inicio?: string;
  data_fim?: string;
  unidade?: string;
  gestor?: string;
  people_partner?: string;
  todos?: boolean;
  em_andamento?: boolean;
  nao_iniciado?: boolean;
  finalizados?: boolean;
  org_id: number;
}

/** Item do relatório Xano /pdi/relatorio/ */
export interface PdiRelatorioMetricasItem {
  id: number;
  manager: string;
  created_at: number;
  status: string;
  collaborator: string;
  collaborator_id: number;
  goal: string;
  deadline: string;
  finished_at: string;
  percentage: number;
  pdi_number?: number;
  step?: number;
  uuid_colab?: string;
  uuid_manager?: string;
  email_colab?: string;
  email_gestor?: string;
  business_unit?: string;
  org_id?: number;
  [key: string]: unknown;
}

/** Detalhe PDI Xano GET /pdi/{id} (para o modal) */
export interface PdiDetalheMetricasResponse {
  id: number;
  manager: string;
  collaborator: string;
  status: string;
  goal: string;
  deadline: string;
  finished_at?: string;
  percentage: number;
  skills?: Array<{ name?: string; nome?: string }>;
  action_plans?: Array<{
    id?: number;
    title?: string;
    description?: string;
    deadline?: string;
    completed_at?: string;
    status?: string;
  }>;
  [key: string]: unknown;
}

export class PdiMetricasApi {
  private buildQuery(params: PdiMetricasRelatorioParams): string {
    const q = new URLSearchParams();
    q.set('data_inicio', params.data_inicio ?? '');
    q.set('data_fim', params.data_fim ?? '');
    q.set('unidade', params.unidade ?? '');
    q.set('gestor', params.gestor ?? '');
    q.set('people_partner', params.people_partner ?? '');
    q.set('todos', String(params.todos ?? false));
    q.set('em_andamento', String(params.em_andamento ?? false));
    q.set('nao_iniciado', String(params.nao_iniciado ?? false));
    q.set('finalizados', String(params.finalizados ?? false));
    q.set('org_id', String(params.org_id));
    return q.toString();
  }

  async getMetricas(params: PdiMetricasRelatorioParams): Promise<PdiMetricasResponse> {
    const query = this.buildQuery(params);
    const headers = getXanoHeaders() as Record<string, string>;
    const data = await xanoClient.get<PdiMetricasResponse>(`/pdi/metricas/?${query}`, {
      headers,
    });
    return (
      data ?? {
        pdis_criados: 0,
        pdis_finalizados: 0,
        gestores_envolvidos: 0,
        colaboradores_envolvidos: 0,
      }
    );
  }

  async getRelatorio(params: PdiMetricasRelatorioParams): Promise<PdiRelatorioMetricasItem[]> {
    const query = this.buildQuery(params);
    const headers = getXanoHeaders() as Record<string, string>;
    const data = await xanoClient.get<PdiRelatorioMetricasItem[]>(`/pdi/relatorio/?${query}`, {
      headers,
    });
    return Array.isArray(data) ? data : [];
  }

  async getPdiDetalhe(pdiId: number): Promise<PdiDetalheMetricasResponse> {
    const headers = getXanoHeaders() as Record<string, string>;
    return xanoClient.get<PdiDetalheMetricasResponse>(`/pdi/${pdiId}`, { headers });
  }
}
