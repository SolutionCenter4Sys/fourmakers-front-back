import { inject, injectable } from 'tsyringe'

import type { DorResponse } from '@domain/entities/VcxDores'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarDoresPorPosicaoIdUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string, posicaoId: string): Promise<DorResponse[]> {
    return this.repository.listarDoresPorPosicaoId(token, posicaoId)
  }
}
