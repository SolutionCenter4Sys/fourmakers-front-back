import type {
  DashboardMetricasVagasEmFocoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { httpClient } from './httpClient'
import { buildCandidaturaQueryParams } from './candidaturaFiltrosUtils'

const BASE_CANDIDATURA = '/api/Candidatura'

export class DashboardMetricasVagasEmFocoApi {
  private readonly url = `${BASE_CANDIDATURA}/dashboardMetricasVagasEmFoco`

  async obterVagasEmFoco(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasVagasEmFocoResponse> {
    const queryString = buildCandidaturaQueryParams(filtros)
    const fullUrl = queryString ? `${this.url}?${queryString}` : this.url
    return httpClient.get<DashboardMetricasVagasEmFocoResponse>(fullUrl, { token })
  }
}
