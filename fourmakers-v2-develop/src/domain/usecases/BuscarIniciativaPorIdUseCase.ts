import { inject, injectable } from 'tsyringe'

import type { IniciativaResponse } from '@domain/entities/VcxIniciativas'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarIniciativaPorIdUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string, id: string): Promise<IniciativaResponse> {
    return this.repository.buscarIniciativaPorId(token, id)
  }
}
