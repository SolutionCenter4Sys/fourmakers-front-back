import { inject, injectable } from 'tsyringe'

import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ExcluirIniciativaUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string, id: string): Promise<void> {
    return this.repository.excluirIniciativa(token, id)
  }
}
