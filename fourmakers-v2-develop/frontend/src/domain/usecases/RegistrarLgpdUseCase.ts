import { inject, injectable } from 'tsyringe'
import type { LgpdRepository, LgpdRegistroPayload } from '@domain/repositories/LgpdRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class RegistrarLgpdUseCase {
  constructor(
    @inject(DiTokens.lgpdRepository)
    private readonly repository: LgpdRepository,
  ) {}

  async execute(payload: LgpdRegistroPayload): Promise<void> {
    await this.repository.registrar(payload)
  }
}
