import { injectable } from 'tsyringe'
import type { HistoricoCandidaturaResponse } from '@domain/entities/HistoricoCandidatura'
import { httpClient } from './httpClient'

const BASE_CANDIDATURA = '/api/Candidatura'

@injectable()
export class HistoricoCandidaturaApi {
  private readonly url = `${BASE_CANDIDATURA}/ListarLogsCandidaturaAgrupados`

  async obterHistorico(
    token: string,
    codigoInternoColaborador: string,
    busca?: string,
  ): Promise<HistoricoCandidaturaResponse> {
    const params = new URLSearchParams()
    params.set('codColaborador', codigoInternoColaborador)
    params.set('busca', busca?.trim() ?? '')
    const queryString = params.toString()
    const fullUrl = `${this.url}?${queryString}`
    return httpClient.get<HistoricoCandidaturaResponse>(fullUrl, { token })
  }
}
