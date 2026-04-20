import type {
  ParametrosConfiguracaoResponse,
  ParametroConfiguracaoPayload,
  ParametroConfiguracaoInsertResponse,
} from '@domain/entities/ParametroConfiguracao'

export interface ParametrosRepository {
  listarParametrosConfiguracao(token: string): Promise<ParametrosConfiguracaoResponse>
  inserirParametroConfiguracao(
    token: string,
    payload: ParametroConfiguracaoPayload
  ): Promise<ParametroConfiguracaoInsertResponse>
  atualizarParametroConfiguracao(
    token: string,
    id: string,
    orgId: number,
    payload: ParametroConfiguracaoPayload
  ): Promise<void>
}
