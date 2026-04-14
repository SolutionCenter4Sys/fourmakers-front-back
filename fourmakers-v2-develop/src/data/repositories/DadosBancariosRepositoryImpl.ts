import { inject, injectable } from 'tsyringe'

import type { DadosBancariosRepository } from '@domain/repositories/DadosBancariosRepository'
import type { DadosBancariosResponse, PayloadDadosBancarios } from '@domain/entities/DadosBancarios'

import { DadosBancariosApi } from '@data/api/DadosBancariosApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DadosBancariosRepositoryImpl implements DadosBancariosRepository {
  constructor(
    @inject(DiTokens.dadosBancariosApi)
    private readonly api: DadosBancariosApi,
  ) {}

  async buscarDadosBancariosPorColaborador(token: string): Promise<DadosBancariosResponse> {
    return this.api.buscarDadosBancariosPorColaborador(token)
  }

  async criarDadosBancarios(token: string, payload: PayloadDadosBancarios): Promise<DadosBancariosResponse> {
    return this.api.criarDadosBancarios(token, payload)
  }

  async editarDadosBancarios(token: string, payload: PayloadDadosBancarios): Promise<DadosBancariosResponse> {
    return this.api.editarDadosBancarios(token, payload)
  }
}

