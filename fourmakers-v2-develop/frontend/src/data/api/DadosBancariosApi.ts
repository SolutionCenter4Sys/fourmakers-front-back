import type { DadosBancariosResponse, PayloadDadosBancarios } from '@domain/entities/DadosBancarios'
import { httpClient } from './httpClient'

export class DadosBancariosApi {
  async buscarDadosBancariosPorColaborador(
    token: string
  ): Promise<DadosBancariosResponse> {
    return httpClient.get<DadosBancariosResponse>(
      '/api/Financeiro/DadosBancariosColaborador/BuscarDadosBancariosPorColaborador',
      { token }
    )
  }

  async criarDadosBancarios(
    token: string,
    payload: PayloadDadosBancarios
  ): Promise<DadosBancariosResponse> {
    return httpClient.post<DadosBancariosResponse>(
      '/api/Financeiro/DadosBancariosColaborador/CriarDadosBancarios',
      payload,
      { token }
    )
  }

  async editarDadosBancarios(
    token: string,
    payload: PayloadDadosBancarios
  ): Promise<DadosBancariosResponse> {
    return httpClient.put<DadosBancariosResponse>(
      '/api/Financeiro/DadosBancariosColaborador/EditarDadosBancarios',
      payload,
      { token }
    )
  }
}

