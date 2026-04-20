import { inject, injectable } from 'tsyringe'

import type { ClientesGestaoAlocadosRepository } from '@domain/repositories/ClientesGestaoAlocadosRepository'
import type { ClientesGestaoAlocadosResponse } from '@domain/entities/ClienteGestaoAlocados'

import { DiTokens } from '@core/di/tokens'
import { ClientesGestaoAlocadosApi } from '@data/api/ClientesGestaoAlocadosApi'

@injectable()
export class ClientesGestaoAlocadosRepositoryImpl
  implements ClientesGestaoAlocadosRepository {
  constructor(
    @inject(DiTokens.clientesGestaoAlocadosApi)
    private readonly api: ClientesGestaoAlocadosApi,
  ) {}

  async listarClientes(
    token: string,
    busca: string,
  ): Promise<ClientesGestaoAlocadosResponse> {
    return this.api.listarClientes(token, busca)
  }
}
