import type { ClientesGestaoAlocadosResponse } from '@domain/entities/ClienteGestaoAlocados'
import { httpClient } from './httpClient'

export class ClientesGestaoAlocadosApi {
  private readonly clientesUrl = '/api/GestaoDeAlocados/ListarClienteOrgDaGestaoDeAlocados'

  async listarClientes(
    token: string,
    busca: string,
  ): Promise<ClientesGestaoAlocadosResponse> {
    const queryParams = new URLSearchParams()
    queryParams.set('cursor', '0')
    queryParams.set('limite', '50')
    queryParams.set('semPerfil', 'false')
    queryParams.set('buscaCodigoOuNome', busca)
    queryParams.set('busca', busca)

    return httpClient.get<ClientesGestaoAlocadosResponse>(
      `${this.clientesUrl}?${queryParams.toString()}`,
      { token },
    )
  }
}
