import { inject, injectable } from 'tsyringe'

import type { StatusIniciativaResponse } from '@domain/entities/VcxIniciativas'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarStatusIniciativasUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string): Promise<StatusIniciativaResponse[]> {
    return this.repository.listarStatusIniciativas(token)
  }
}
