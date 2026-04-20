import { httpClient } from './httpClient'
import type { InserirAvaliacaoSatisfacaoPayload, InserirAvaliacaoSatisfacaoResponse } from '@domain/entities/AvaliacaoSatisfacao'
import type { ObterDashboardGestorResponse, ObterMeusColaboradoresResponse, ObterDashboardColaboradorAvaliadoResponse, InserirPautaSugeridaPayload, InserirPautaSugeridaResponse, InserirPautaSugeridaColaboradorPayload, InserirFeedbackPayload, InserirFeedbackResponse, InserirOneOnOnePayload, InserirOneOnOneResponse, ObterDashboardColaboradorResponse, ObterDashboardRHResponse, InserirParametrizacaoPayload, InserirParametrizacaoResponse, ObterParametrizacaoResponse, InserirVisualizacaoFeedbackResponse, ObterListaColaboradoresRHResponse } from '@domain/entities/GestaoDesempenhoGestor'

export interface InsereUsuarioFourmakerPayload {
  cpf: string
  email: string
  nome_completo: string
  senha: string
}

export interface InsereUsuarioFourmakerResponse {
  sucesso: boolean
  mensagem?: string
}

export class FourmakersApi {
  async inserirAvaliacao(token: string, payload: InserirAvaliacaoSatisfacaoPayload): Promise<InserirAvaliacaoSatisfacaoResponse> {
    return httpClient.post<InserirAvaliacaoSatisfacaoResponse>(
      '/api/Fourmakers/InserirAvaliacao',
      payload,
      { token }
    )
  }

  async insereUsuarioFourmaker(payload: InsereUsuarioFourmakerPayload): Promise<InsereUsuarioFourmakerResponse> {
    return httpClient.post<InsereUsuarioFourmakerResponse>(
      '/api/Usuario/InsereUsuarioFourmaker',
      payload
    )
  }

  async obterDashboardGestor(token: string): Promise<ObterDashboardGestorResponse> {
    return httpClient.get<ObterDashboardGestorResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/Gestor/ObterDashboard',
      { token }
    )
  }

  async obterMeusColaboradores(token: string): Promise<ObterMeusColaboradoresResponse> {
    return httpClient.get<ObterMeusColaboradoresResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/Gestor/MeusColaboradores',
      { token }
    )
  }

  async obterDashboardColaboradorAvaliado(token: string, codigoInternoColaborador: string): Promise<ObterDashboardColaboradorAvaliadoResponse> {
    return httpClient.get<ObterDashboardColaboradorAvaliadoResponse>(
      `/api/GestaoPessoa/GestaoDesempenho/Gestor/ColaboradorAvaliado/ObterDashboard/${codigoInternoColaborador}`,
      { token }
    )
  }

  async inserirPautaSugerida(token: string, payload: InserirPautaSugeridaPayload): Promise<InserirPautaSugeridaResponse> {
    return httpClient.post<InserirPautaSugeridaResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/Gestor/InserirPautaSugerida',
      payload,
      { token }
    )
  }

  async inserirPautaSugeridaColaborador(token: string, payload: InserirPautaSugeridaColaboradorPayload): Promise<InserirPautaSugeridaResponse> {
    return httpClient.post<InserirPautaSugeridaResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/Colaborador/InserirPautaSugerida',
      payload,
      { token }
    )
  }

  async inserirFeedback(token: string, payload: InserirFeedbackPayload): Promise<InserirFeedbackResponse> {
    return httpClient.post<InserirFeedbackResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/Gestor/ColaboradorAvaliado/InserirFeedback',
      payload,
      { token }
    )
  }

  async inserirOneOnOne(token: string, payload: InserirOneOnOnePayload): Promise<InserirOneOnOneResponse> {
    return httpClient.post<InserirOneOnOneResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/Gestor/ColaboradorAvaliado/InserirOneOnOne',
      payload,
      { token }
    )
  }

  async inserirVisualizacaoFeedback(token: string, feedbackId: string): Promise<InserirVisualizacaoFeedbackResponse> {
    return httpClient.post<InserirVisualizacaoFeedbackResponse>(
      `/api/GestaoPessoa/GestaoDesempenho/Colaborador/Feedback/InserirVisualizacao/${feedbackId}`,
      undefined,
      { token }
    )
  }

  async obterDashboardColaborador(token: string): Promise<ObterDashboardColaboradorResponse> {
    return httpClient.get<ObterDashboardColaboradorResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/Colaborador/ObterDashboard',
      { token }
    )
  }

  async obterDashboardRH(token: string): Promise<ObterDashboardRHResponse> {
    return httpClient.get<ObterDashboardRHResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/RH/ObterDashboard',
      { token }
    )
  }

  async inserirParametrizacao(token: string, payload: InserirParametrizacaoPayload): Promise<InserirParametrizacaoResponse> {
    return httpClient.post<InserirParametrizacaoResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/RH/InserirParametrizacao',
      payload,
      { token }
    )
  }

  async obterParametrizacao(token: string): Promise<ObterParametrizacaoResponse> {
    return httpClient.get<ObterParametrizacaoResponse>(
      '/api/GestaoPessoa/GestaoDesempenho/RH/ObterParametrizacao',
      { token }
    )
  }

  async obterListaColaboradoresRH(token: string, params?: { semFeedback?: boolean; semOneOnOne?: boolean }): Promise<ObterListaColaboradoresRHResponse> {
    const queryParams = new URLSearchParams();
    
    if (params?.semFeedback) {
      queryParams.append('semFeedback', 'true');
    }
    
    if (params?.semOneOnOne) {
      queryParams.append('semOneOnOne', 'true');
    }
    
    const queryString = queryParams.toString();
    const url = `/api/GestaoPessoa/GestaoDesempenho/RH/ListaColaboradores${queryString ? `?${queryString}` : ''}`;
    
    return httpClient.get<ObterListaColaboradoresRHResponse>(
      url,
      { token }
    )
  }
}

