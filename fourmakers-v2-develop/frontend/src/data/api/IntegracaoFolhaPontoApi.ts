import type {
  LoteProcessadoMock,
  ColaboradorDetalheMock,
} from '../mocks/integracaoFolhaPontoMock'
import {
  lotesProcessadosMock,
  colaboradoresDetalheMock,
} from '../mocks/integracaoFolhaPontoMock'

export class IntegracaoFolhaPontoApi {
  async getLotes(): Promise<LoteProcessadoMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...lotesProcessadosMock]
  }

  async getColaboradoresPorLote(loteId: number): Promise<ColaboradorDetalheMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return colaboradoresDetalheMock[loteId] || []
  }
}

