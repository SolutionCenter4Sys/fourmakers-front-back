import { inject, injectable } from 'tsyringe'

import type { AuthRepository } from '@domain/repositories/AuthRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class LogoutUseCase {
  constructor(
    @inject(DiTokens.authRepository)
    private readonly repository: AuthRepository,
  ) {}

  async execute(token: string, cpf: string): Promise<void> {
    return this.repository.logout(token, cpf)
  }
}

