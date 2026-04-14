import type {
  DashboardMetricasRecrutamentoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { httpClient } from './httpClient'
import { buildCandidaturaQueryParamsRecrutamento } from './candidaturaFiltrosUtils'

const BASE_CANDIDATURA = '/api/Candidatura'

export class DashboardMetricasRecrutamentoApi {
  private readonly url = `${BASE_CANDIDATURA}/dashboardMetricasRecrutamento`

  async obterDashboardMetricas(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasRecrutamentoResponse> {
    const queryString = buildCandidaturaQueryParamsRecrutamento(filtros)
    const fullUrl = queryString ? `${this.url}?${queryString}` : this.url
    return httpClient.get<DashboardMetricasRecrutamentoResponse>(fullUrl, { token })
  }
}
