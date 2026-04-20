import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { DiretoriasResponse } from '@domain/entities/Diretoria'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarDiretoriasUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string): Promise<DiretoriasResponse> {
    return this.repository.listarDiretorias(token)
  }
}

