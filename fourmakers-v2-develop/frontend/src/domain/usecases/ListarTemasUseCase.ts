import { inject, injectable } from 'tsyringe'

import type { TemaResponse } from '@domain/entities/VcxIniciativas'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarTemasUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string): Promise<TemaResponse[]> {
    return this.repository.listarTemas(token)
  }
}
