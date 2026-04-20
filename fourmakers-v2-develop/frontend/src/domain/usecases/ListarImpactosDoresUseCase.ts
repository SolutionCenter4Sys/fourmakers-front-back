import { inject, injectable } from 'tsyringe'

import type { ImpactoResponse } from '@domain/entities/VcxDores'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarImpactosDoresUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string): Promise<ImpactoResponse[]> {
    return this.repository.listarImpactosDores(token)
  }
}
