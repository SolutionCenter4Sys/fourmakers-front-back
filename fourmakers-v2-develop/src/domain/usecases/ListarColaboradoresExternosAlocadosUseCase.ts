import { inject, injectable } from 'tsyringe'

import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarColaboradoresExternosAlocadosUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(
    token: string,
    codigoCliente: string,
    cursor: number,
    limit: number,
    nome?: string,
  ): Promise<Array<{ CodigoInternoColaborador: string; NomeColaborador: string }>> {
    return this.repository.listarColaboradoresExternosPorCliente(
      token,
      codigoCliente,
      cursor,
      limit,
      nome,
    )
  }
}
