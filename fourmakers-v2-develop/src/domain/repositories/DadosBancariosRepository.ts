import type { DadosBancariosResponse, PayloadDadosBancarios } from '@domain/entities/DadosBancarios'

export interface DadosBancariosRepository {
  buscarDadosBancariosPorColaborador(token: string): Promise<DadosBancariosResponse>
  criarDadosBancarios(token: string, payload: PayloadDadosBancarios): Promise<DadosBancariosResponse>
  editarDadosBancarios(token: string, payload: PayloadDadosBancarios): Promise<DadosBancariosResponse>
}

