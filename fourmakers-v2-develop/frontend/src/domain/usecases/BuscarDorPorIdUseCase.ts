import { inject, injectable } from 'tsyringe'

import type { DorResponse } from '@domain/entities/VcxDores'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarDorPorIdUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string, id: string): Promise<DorResponse> {
    return this.repository.buscarDorPorId(token, id)
  }
}
