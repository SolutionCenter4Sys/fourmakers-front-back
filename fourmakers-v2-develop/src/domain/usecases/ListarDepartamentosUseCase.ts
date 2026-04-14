import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { DepartamentosResponse, ListarDepartamentosParams } from '@domain/entities/Departamento'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarDepartamentosUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, params: ListarDepartamentosParams): Promise<DepartamentosResponse> {
    return this.repository.listarDepartamentos(token, params)
  }
}

