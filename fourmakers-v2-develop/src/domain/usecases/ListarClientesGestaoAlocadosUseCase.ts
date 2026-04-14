import { inject, injectable } from 'tsyringe'

import type { ClientesGestaoAlocadosRepository } from '@domain/repositories/ClientesGestaoAlocadosRepository'
import type { ClientesGestaoAlocadosResponse } from '@domain/entities/ClienteGestaoAlocados'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarClientesGestaoAlocadosUseCase {
  constructor(
    @inject(DiTokens.clientesGestaoAlocadosRepository)
    private readonly repository: ClientesGestaoAlocadosRepository,
  ) {}

  async execute(
    token: string,
    busca: string,
  ): Promise<ClientesGestaoAlocadosResponse> {
    return this.repository.listarClientes(token, busca)
  }
}
