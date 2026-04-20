import type {
  DashboardMetricasVagasPerdidasMotivoResponse,
  FiltrosCandidaturaParams,
} from '@domain/entities/DashboardMetricasRecrutamento'
import { httpClient } from './httpClient'
import { buildCandidaturaQueryParams } from './candidaturaFiltrosUtils'

const BASE_CANDIDATURA = '/api/Candidatura'

export class DashboardMetricasVagasPerdidasMotivoApi {
  private readonly url = `${BASE_CANDIDATURA}/dashboardMetricasVagasPerdidasMotivo`

  async obterVagasPerdidasMotivo(
    token: string,
    filtros: FiltrosCandidaturaParams,
  ): Promise<DashboardMetricasVagasPerdidasMotivoResponse> {
    const queryString = buildCandidaturaQueryParams(filtros)
    const fullUrl = queryString ? `${this.url}?${queryString}` : this.url
    return httpClient.get<DashboardMetricasVagasPerdidasMotivoResponse>(fullUrl, { token })
  }
}
