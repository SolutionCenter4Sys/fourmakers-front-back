import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { RelatorioColaboradoresResult } from '@domain/repositories/ColaboradoresRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GerarRelatorioColaboradoresUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string): Promise<RelatorioColaboradoresResult> {
    return this.repository.gerarRelatorioColaboradores(token)
  }
}

