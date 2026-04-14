import type {
  ParametrosConfiguracaoResponse,
  ParametroConfiguracaoPayload,
  ParametroConfiguracaoInsertResponse,
} from '@domain/entities/ParametroConfiguracao'
import { httpClient } from './httpClient'

export class ParametrosApi {
  async listarParametrosConfiguracao(token: string): Promise<ParametrosConfiguracaoResponse> {
    return httpClient.get<ParametrosConfiguracaoResponse>(
      '/api/Fourmakers/ParametroConfiguracao/ListarParametroConfiguracaoDoUsuarioLogado',
      { token }
    )
  }

  async inserirParametroConfiguracao(
    token: string,
    payload: ParametroConfiguracaoPayload
  ): Promise<ParametroConfiguracaoInsertResponse> {
    return httpClient.post<ParametroConfiguracaoInsertResponse>(
      '/api/Fourmakers/ParametroConfiguracao/InserirParametroConfiguracao',
      payload,
      { token }
    )
  }

  async atualizarParametroConfiguracao(
    token: string,
    id: string,
    orgId: number,
    payload: ParametroConfiguracaoPayload
  ): Promise<ParametrosConfiguracaoResponse> {
    const url = `/api/Fourmakers/ParametroConfiguracao/AtualizarParametroConfiguracao?id=${encodeURIComponent(id)}&orgId=${encodeURIComponent(orgId)}`
    return httpClient.put<ParametrosConfiguracaoResponse>(url, payload, { token })
  }
}

