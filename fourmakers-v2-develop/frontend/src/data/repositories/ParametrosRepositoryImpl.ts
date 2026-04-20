import { inject, injectable } from 'tsyringe'
import type { ParametrosRepository } from '@domain/repositories/ParametrosRepository'
import type {
  ParametrosConfiguracaoResponse,
  ParametroConfiguracaoPayload,
  ParametroConfiguracaoInsertResponse,
} from '@domain/entities/ParametroConfiguracao'
import { ParametrosApi } from '@data/api/ParametrosApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ParametrosRepositoryImpl implements ParametrosRepository {
  constructor(
    @inject(DiTokens.parametrosApi)
    private readonly api: ParametrosApi,
  ) {}

  async listarParametrosConfiguracao(token: string): Promise<ParametrosConfiguracaoResponse> {
    return this.api.listarParametrosConfiguracao(token)
  }

  async inserirParametroConfiguracao(
    token: string,
    payload: ParametroConfiguracaoPayload
  ): Promise<ParametroConfiguracaoInsertResponse> {
    return this.api.inserirParametroConfiguracao(token, payload)
  }

  async atualizarParametroConfiguracao(
    token: string,
    id: string,
    orgId: number,
    payload: ParametroConfiguracaoPayload
  ): Promise<void> {
    await this.api.atualizarParametroConfiguracao(token, id, orgId, payload)
  }
}

