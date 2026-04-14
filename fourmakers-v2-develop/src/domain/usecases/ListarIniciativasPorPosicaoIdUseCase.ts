import { inject, injectable } from 'tsyringe'

import type { IniciativaResponse } from '@domain/entities/VcxIniciativas'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarIniciativasPorPosicaoIdUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string, posicaoId: string): Promise<IniciativaResponse[]> {
    return this.repository.listarIniciativasPorPosicaoId(token, posicaoId)
  }
}
