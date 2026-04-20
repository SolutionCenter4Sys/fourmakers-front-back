import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { CargosResponse } from '@domain/entities/Cargo'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarCargosUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string): Promise<CargosResponse> {
    return this.repository.listarCargos(token)
  }
}

