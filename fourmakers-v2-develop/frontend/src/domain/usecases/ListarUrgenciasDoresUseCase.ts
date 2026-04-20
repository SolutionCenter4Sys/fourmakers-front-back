import { inject, injectable } from 'tsyringe'

import type { UrgenciaResponse } from '@domain/entities/VcxDores'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarUrgenciasDoresUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string): Promise<UrgenciaResponse[]> {
    return this.repository.listarUrgenciasDores(token)
  }
}
