import { inject, injectable } from 'tsyringe'

import type { VcxAgendasPorColaboradorClienteResult } from '@domain/entities/VcxAgenda'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

export interface ListarAgendasPorColaboradorClienteParams {
  codigoColaborador: string
  codigoCliente: string
  limit?: number
  cursor?: number
}

@injectable()
export class ListarAgendasPorColaboradorClienteUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(
    token: string,
    params: ListarAgendasPorColaboradorClienteParams,
  ): Promise<VcxAgendasPorColaboradorClienteResult> {
    return this.repository.listarAgendasPorColaboradorCliente(
      token,
      params.codigoColaborador,
      params.codigoCliente,
      params.limit,
      params.cursor,
    )
  }
}
