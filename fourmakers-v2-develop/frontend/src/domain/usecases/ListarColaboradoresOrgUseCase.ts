import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { ColaboradoresCchResponse, ListarColaboradoresOrgParams } from '@domain/entities/ColaboradorCch'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarColaboradoresOrgUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, params: ListarColaboradoresOrgParams): Promise<ColaboradoresCchResponse> {
    return this.repository.listarColaboradoresOrg(token, params)
  }
}

