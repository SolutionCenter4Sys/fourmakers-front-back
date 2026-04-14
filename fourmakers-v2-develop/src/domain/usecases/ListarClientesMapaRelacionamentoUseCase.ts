import { inject, injectable } from 'tsyringe'

import type { ClientesMapaRelacionamentoResponse } from '@domain/entities/MapaRelacionamento'
import type { MapaRelacionamentoRepository } from '@domain/repositories/MapaRelacionamentoRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarClientesMapaRelacionamentoUseCase {
  constructor(
    @inject(DiTokens.mapaRelacionamentoRepository)
    private readonly repository: MapaRelacionamentoRepository,
  ) {}

  async execute(
    token: string,
    orgId: number,
    limite: number = 50000,
    cursor: number = 0,
    nomeCliente: string = '',
  ): Promise<ClientesMapaRelacionamentoResponse> {
    return this.repository.listarClientes(token, {
      orgId,
      limite,
      cursor,
      nomeCliente,
    })
  }
}
