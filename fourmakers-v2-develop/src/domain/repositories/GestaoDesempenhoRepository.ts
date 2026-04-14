import type { ObterDashboardGestorResponse, ObterMeusColaboradoresResponse, ObterDashboardColaboradorAvaliadoResponse, InserirPautaSugeridaPayload, InserirPautaSugeridaResponse, InserirPautaSugeridaColaboradorPayload, InserirFeedbackPayload, InserirFeedbackResponse, InserirOneOnOnePayload, InserirOneOnOneResponse, ObterDashboardColaboradorResponse, ObterDashboardRHResponse, InserirParametrizacaoPayload, InserirParametrizacaoResponse, ObterParametrizacaoResponse, InserirVisualizacaoFeedbackResponse, ObterListaColaboradoresRHResponse } from '@domain/entities/GestaoDesempenhoGestor'

export interface GestaoDesempenhoRepository {
  obterDashboardGestor(token: string): Promise<ObterDashboardGestorResponse>
  obterMeusColaboradores(token: string): Promise<ObterMeusColaboradoresResponse>
  obterDashboardColaboradorAvaliado(token: string, codigoInternoColaborador: string): Promise<ObterDashboardColaboradorAvaliadoResponse>
  obterDashboardColaborador(token: string): Promise<ObterDashboardColaboradorResponse>
  obterDashboardRH(token: string): Promise<ObterDashboardRHResponse>
  inserirPautaSugerida(token: string, payload: InserirPautaSugeridaPayload): Promise<InserirPautaSugeridaResponse>
  inserirPautaSugeridaColaborador(token: string, payload: InserirPautaSugeridaColaboradorPayload): Promise<InserirPautaSugeridaResponse>
  inserirFeedback(token: string, payload: InserirFeedbackPayload): Promise<InserirFeedbackResponse>
  inserirOneOnOne(token: string, payload: InserirOneOnOnePayload): Promise<InserirOneOnOneResponse>
  inserirParametrizacao(token: string, payload: InserirParametrizacaoPayload): Promise<InserirParametrizacaoResponse>
  obterParametrizacao(token: string): Promise<ObterParametrizacaoResponse>
  inserirVisualizacaoFeedback(token: string, feedbackId: string): Promise<InserirVisualizacaoFeedbackResponse>
  obterListaColaboradoresRH(token: string, params?: { semFeedback?: boolean; semOneOnOne?: boolean }): Promise<ObterListaColaboradoresRHResponse>
}
