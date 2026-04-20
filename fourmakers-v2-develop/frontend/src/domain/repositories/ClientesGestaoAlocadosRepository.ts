import type { ClientesGestaoAlocadosResponse } from '@domain/entities/ClienteGestaoAlocados'

export interface ClientesGestaoAlocadosRepository {
  listarClientes(
    token: string,
    busca: string,
  ): Promise<ClientesGestaoAlocadosResponse>
}
