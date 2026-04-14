import type { ProjetoMock } from '../mocks/projetosMock'
import { projetosMock } from '../mocks/projetosMock'
import { httpClient } from './httpClient'

export interface Cliente {
  codigoCliente: string
  nomeCliente: string
  labelCodigoCliente: string
  ativo: boolean
}

export type ListarClientesResponse = Cliente[]

export interface Projeto {
  codigoProjeto: string
  nomeProjeto: string
  label: string
}

export interface ListarProjetosClienteResponse {
  retorno: Projeto[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface ListarClientesParams {
  codigoClienteFiltro?: string
  codigoGerenteProjeto?: string
}

export interface StatusProjeto {
  codigoStatusProjeto: number
  nomeStatusProjeto: string
}

export interface ListarStatusProjetoResponse {
  retorno: StatusProjeto[]
  sucesso?: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export class ProjetosApi {
  async getProjetos(): Promise<ProjetoMock[]> {
    // Simula delay de API
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...projetosMock]
  }

  async listarClientesOrg(
    token: string,
    params?: ListarClientesParams
  ): Promise<ListarClientesResponse> {
    const queryParams = new URLSearchParams()
    
    if (params?.codigoClienteFiltro) {
      queryParams.append('codigoClienteFiltro', params.codigoClienteFiltro)
    } else {
      queryParams.append('codigoClienteFiltro', '')
    }
    
    if (params?.codigoGerenteProjeto) {
      queryParams.append('codigoGerenteProjeto', params.codigoGerenteProjeto)
    } else {
      queryParams.append('codigoGerenteProjeto', '')
    }

    const data = await httpClient.get<ListarClientesResponse>(
      `/api/Projeto/Cliente/ListarClientesOrg?${queryParams.toString()}`,
      { token }
    )
    
    // O retorno é um array direto, não um objeto com retorno
    return Array.isArray(data) ? data : data
  }

  async listarProjetosDoCliente(
    token: string,
    codigoCliente: string
  ): Promise<ListarProjetosClienteResponse> {
    return httpClient.get<ListarProjetosClienteResponse>(
      `/api/Projeto/ProjetoOrg/ListarProjetosDoCliente?codigoCliente=${codigoCliente}`,
      { token }
    )
  }

  async listarStatusProjeto(
    token: string
  ): Promise<ListarStatusProjetoResponse> {
    return httpClient.get<ListarStatusProjetoResponse>(
      '/api/Projeto/StatusProjeto/ListarStatusProjeto',
      { token }
    )
  }
}

