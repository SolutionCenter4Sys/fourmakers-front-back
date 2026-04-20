import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { EmpresasRelacionadasResponse, ListarEmpresasRelacionadasParams } from '@domain/entities/EmpresaRelacionada'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarEmpresasRelacionadasUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, params: ListarEmpresasRelacionadasParams): Promise<EmpresasRelacionadasResponse> {
    return this.repository.listarEmpresasRelacionadas(token, params)
  }
}

