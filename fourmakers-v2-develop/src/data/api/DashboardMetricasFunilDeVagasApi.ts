import type {
  DashboardMetricasFunilDeVagasResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { httpClient } from './httpClient'
import { buildCandidaturaQueryParamsFunil } from './candidaturaFiltrosUtils'

const BASE_CANDIDATURA = '/api/Candidatura'

export class DashboardMetricasFunilDeVagasApi {
  private readonly url = `${BASE_CANDIDATURA}/dashboardMetricasFunilDeVagas`

  async obterFunilDeVagas(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasFunilDeVagasResponse> {
    const queryString = buildCandidaturaQueryParamsFunil(filtros)
    const fullUrl = queryString ? `${this.url}?${queryString}` : this.url
    return httpClient.get<DashboardMetricasFunilDeVagasResponse>(fullUrl, { token })
  }
}
