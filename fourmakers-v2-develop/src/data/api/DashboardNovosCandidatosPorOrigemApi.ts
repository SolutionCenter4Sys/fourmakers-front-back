import type {
  DashboardNovosCandidatosPorOrigemResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { httpClient } from './httpClient'
import { buildCandidaturaQueryParams } from './candidaturaFiltrosUtils'

const BASE_CANDIDATURA = '/api/Candidatura'

export class DashboardNovosCandidatosPorOrigemApi {
  private readonly url = `${BASE_CANDIDATURA}/dashboardNovosCandidatosPorOrigem`

  async obterNovosCandidatosPorOrigem(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardNovosCandidatosPorOrigemResponse> {
    const queryString = buildCandidaturaQueryParams(filtros)
    const fullUrl = queryString ? `${this.url}?${queryString}` : this.url
    return httpClient.get<DashboardNovosCandidatosPorOrigemResponse>(fullUrl, { token })
  }
}
