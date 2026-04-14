import type {
  ReembolsoStatMock,
  ColaboradorGestaoAdmMock,
  SolicitacaoAprovacaoMock,
} from '../mocks/reembolsoComponentesMock'
import {
  gestaoAdmStatsMock,
  colaboradoresGestaoAdmMock,
  aprovacoesStatsMock,
  solicitacoesAprovacaoMock,
} from '../mocks/reembolsoComponentesMock'

export class ReembolsoComponentesApi {
  async getGestaoAdmStats(): Promise<ReembolsoStatMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...gestaoAdmStatsMock]
  }

  async getColaboradoresGestaoAdm(): Promise<ColaboradorGestaoAdmMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...colaboradoresGestaoAdmMock]
  }

  async getAprovacoesStats(): Promise<ReembolsoStatMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...aprovacoesStatsMock]
  }

  async getSolicitacoesAprovacao(): Promise<SolicitacaoAprovacaoMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...solicitacoesAprovacaoMock]
  }
}

