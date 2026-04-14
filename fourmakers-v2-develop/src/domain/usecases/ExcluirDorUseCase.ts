import { inject, injectable } from 'tsyringe'

import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ExcluirDorUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string, id: string): Promise<void> {
    if (!id) {
      throw new Error('O campo id é obrigatório para exclusão')
    }

    return this.repository.excluirDor(token, id)
  }
}
