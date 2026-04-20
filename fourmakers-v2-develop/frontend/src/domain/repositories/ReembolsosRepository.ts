import type { ReembolsoMock, ReembolsoStatMock } from '@data/mocks/reembolsosMock'
import type { ListarSolicitacoesApiResponse } from '@domain/entities/SolicitacaoReembolso'
import type { ListarSolicitacoesParams } from '@data/api/ReembolsosApi'

export interface ReembolsosRepository {
  getReembolsos(): Promise<ReembolsoMock[]>
  getReembolsosStats(): Promise<ReembolsoStatMock[]>
  listarSolicitacoesPorColab(
    token: string,
    params?: ListarSolicitacoesParams
  ): Promise<ListarSolicitacoesApiResponse>
}

