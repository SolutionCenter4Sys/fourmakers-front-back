import type {
  AtualizarParametrizacaoNotificacaoCandidatosPayload,
  AtualizarParametrizacaoNotificacaoCandidatosResponse,
  DeletarParametrizacaoNotificacaoCandidatosResponse,
  ObterParametrizacaoNotificacaoCandidatosPorIdResponse,
  ObterParametrizacaoNotificacaoCandidatosResponse,
  SalvarParametrizacaoNotificacaoCandidatosPayload,
  SalvarParametrizacaoNotificacaoCandidatosResponse,
} from '@domain/entities/ParametrizacaoNotificacaoCandidatos'

import { httpClient } from './httpClient'

export class ParametrizacaoNotificacaoCandidatosApi {
  private readonly listarUrl = '/api/Candidatura/CandidatoTemplateEmailListar'
  private readonly listarPorIdUrl = '/api/Candidatura/CandidatoTemplateEmailListarPorId'
  private readonly inserirUrl = '/api/Candidatura/CandidatoTemplateEmailInserir'
  private readonly atualizarUrl = '/api/Candidatura/CandidatoTemplateEmailAtualizar'
  private readonly deletarUrl = '/api/Candidatura/CandidatoTemplateEmailDeletar'

  async obterParametrizacao(
    token: string,
    orgId: number,
  ): Promise<ObterParametrizacaoNotificacaoCandidatosResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('orgId', orgId.toString())

    return httpClient.get<ObterParametrizacaoNotificacaoCandidatosResponse>(
      `${this.listarUrl}?${queryParams.toString()}`,
      { token }
    )
  }

  async obterParametrizacaoPorId(
    token: string,
    orgId: number,
  ): Promise<ObterParametrizacaoNotificacaoCandidatosPorIdResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('orgId', orgId.toString())

    return httpClient.get<ObterParametrizacaoNotificacaoCandidatosPorIdResponse>(
      `${this.listarPorIdUrl}?${queryParams.toString()}`,
      { token }
    )
  }

  async salvarParametrizacao(
    token: string,
    payload: SalvarParametrizacaoNotificacaoCandidatosPayload,
  ): Promise<SalvarParametrizacaoNotificacaoCandidatosResponse> {
    return httpClient.post<SalvarParametrizacaoNotificacaoCandidatosResponse>(
      this.inserirUrl,
      payload,
      { token }
    )
  }

  async atualizarParametrizacao(
    token: string,
    payload: AtualizarParametrizacaoNotificacaoCandidatosPayload,
  ): Promise<AtualizarParametrizacaoNotificacaoCandidatosResponse> {
    return httpClient.put<AtualizarParametrizacaoNotificacaoCandidatosResponse>(
      this.atualizarUrl,
      payload,
      { token }
    )
  }

  async deletarParametrizacao(
    token: string,
    orgId: number,
  ): Promise<DeletarParametrizacaoNotificacaoCandidatosResponse> {
    return httpClient.delete<DeletarParametrizacaoNotificacaoCandidatosResponse>(
      `${this.deletarUrl}/${orgId}`,
      { token }
    )
  }
}
