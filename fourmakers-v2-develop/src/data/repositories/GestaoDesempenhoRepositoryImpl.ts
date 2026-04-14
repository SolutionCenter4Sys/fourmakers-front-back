import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { ObterDashboardGestorResponse, ObterMeusColaboradoresResponse, ObterDashboardColaboradorAvaliadoResponse, InserirPautaSugeridaPayload, InserirPautaSugeridaResponse, InserirPautaSugeridaColaboradorPayload, InserirFeedbackPayload, InserirFeedbackResponse, InserirOneOnOnePayload, InserirOneOnOneResponse, ObterDashboardColaboradorResponse, ObterDashboardRHResponse, InserirParametrizacaoPayload, InserirParametrizacaoResponse, ObterParametrizacaoResponse, InserirVisualizacaoFeedbackResponse, ObterListaColaboradoresRHResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { FourmakersApi } from '@data/api/FourmakersApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class GestaoDesempenhoRepositoryImpl implements GestaoDesempenhoRepository {
  constructor(
    @inject(DiTokens.fourmakersApi)
    private readonly api: FourmakersApi,
  ) {}

  async obterDashboardGestor(token: string): Promise<ObterDashboardGestorResponse> {
    return this.api.obterDashboardGestor(token)
  }

  async obterMeusColaboradores(token: string): Promise<ObterMeusColaboradoresResponse> {
    return this.api.obterMeusColaboradores(token)
  }

  async obterDashboardColaboradorAvaliado(token: string, codigoInternoColaborador: string): Promise<ObterDashboardColaboradorAvaliadoResponse> {
    return this.api.obterDashboardColaboradorAvaliado(token, codigoInternoColaborador)
  }

  async obterDashboardColaborador(token: string): Promise<ObterDashboardColaboradorResponse> {
    return this.api.obterDashboardColaborador(token)
  }

  async obterDashboardRH(token: string): Promise<ObterDashboardRHResponse> {
    return this.api.obterDashboardRH(token)
  }

  async inserirPautaSugerida(token: string, payload: InserirPautaSugeridaPayload): Promise<InserirPautaSugeridaResponse> {
    return this.api.inserirPautaSugerida(token, payload)
  }

  async inserirPautaSugeridaColaborador(token: string, payload: InserirPautaSugeridaColaboradorPayload): Promise<InserirPautaSugeridaResponse> {
    return this.api.inserirPautaSugeridaColaborador(token, payload)
  }

  async inserirFeedback(token: string, payload: InserirFeedbackPayload): Promise<InserirFeedbackResponse> {
    return this.api.inserirFeedback(token, payload)
  }

  async inserirOneOnOne(token: string, payload: InserirOneOnOnePayload): Promise<InserirOneOnOneResponse> {
    return this.api.inserirOneOnOne(token, payload)
  }

  async inserirParametrizacao(token: string, payload: InserirParametrizacaoPayload): Promise<InserirParametrizacaoResponse> {
    return this.api.inserirParametrizacao(token, payload)
  }

  async obterParametrizacao(token: string): Promise<ObterParametrizacaoResponse> {
    return this.api.obterParametrizacao(token)
  }

  async inserirVisualizacaoFeedback(token: string, feedbackId: string): Promise<InserirVisualizacaoFeedbackResponse> {
    return this.api.inserirVisualizacaoFeedback(token, feedbackId)
  }

  async obterListaColaboradoresRH(token: string, params?: { semFeedback?: boolean; semOneOnOne?: boolean }): Promise<ObterListaColaboradoresRHResponse> {
    return this.api.obterListaColaboradoresRH(token, params)
  }
}
