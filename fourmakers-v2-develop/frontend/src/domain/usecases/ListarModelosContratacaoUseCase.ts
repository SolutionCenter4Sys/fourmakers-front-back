import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { ModelosContratacaoResponse } from '@domain/entities/ModeloContratacao'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarModelosContratacaoUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string): Promise<ModelosContratacaoResponse> {
    return this.repository.listarModelosContratacao(token)
  }
}

