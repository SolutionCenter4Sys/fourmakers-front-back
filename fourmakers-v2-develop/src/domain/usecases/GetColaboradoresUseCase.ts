import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { ColaboradoresResponse, ColaboradoresParams } from '@domain/entities/Colaborador'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetColaboradoresUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, params: ColaboradoresParams): Promise<ColaboradoresResponse> {
    return this.repository.getColaboradores(token, params)
  }
}

