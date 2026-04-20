import { inject, injectable } from 'tsyringe'

import type { ReembolsosRepository } from '@domain/repositories/ReembolsosRepository'
import type { ReembolsoMock, ReembolsoStatMock } from '@data/mocks/reembolsosMock'
import type { ListarSolicitacoesApiResponse } from '@domain/entities/SolicitacaoReembolso'
import type { ListarSolicitacoesParams } from '@data/api/ReembolsosApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetReembolsosUseCase {
  constructor(
    @inject(DiTokens.reembolsosRepository)
    private readonly repository: ReembolsosRepository,
  ) {}

  async execute(): Promise<ReembolsoMock[]> {
    return this.repository.getReembolsos()
  }

  async executeStats(): Promise<ReembolsoStatMock[]> {
    return this.repository.getReembolsosStats()
  }

  async listarSolicitacoesPorColab(
    token: string,
    params?: ListarSolicitacoesParams
  ): Promise<ListarSolicitacoesApiResponse> {
    return this.repository.listarSolicitacoesPorColab(token, params)
  }
}

