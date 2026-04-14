import { inject, injectable } from 'tsyringe'

import type { ReembolsosRepository } from '@domain/repositories/ReembolsosRepository'
import type { ReembolsoMock, ReembolsoStatMock } from '@data/mocks/reembolsosMock'
import type { ListarSolicitacoesApiResponse } from '@domain/entities/SolicitacaoReembolso'
import type { ListarSolicitacoesParams } from '@data/api/ReembolsosApi'

import { ReembolsosApi } from '@data/api/ReembolsosApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ReembolsosRepositoryImpl implements ReembolsosRepository {
  constructor(
    @inject(DiTokens.reembolsosApi)
    private readonly api: ReembolsosApi,
  ) {}

  async getReembolsos(): Promise<ReembolsoMock[]> {
    return this.api.getReembolsos()
  }

  async getReembolsosStats(): Promise<ReembolsoStatMock[]> {
    return this.api.getReembolsosStats()
  }

  async listarSolicitacoesPorColab(
    token: string,
    params?: ListarSolicitacoesParams
  ): Promise<ListarSolicitacoesApiResponse> {
    return this.api.listarSolicitacoesPorColab(token, params)
  }
}

