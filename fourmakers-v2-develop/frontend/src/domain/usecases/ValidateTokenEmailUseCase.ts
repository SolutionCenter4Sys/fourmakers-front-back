import { inject, injectable } from 'tsyringe'

import type {
  AuthRepository,
  ValidateTokenEmailResult,
} from '@domain/repositories/AuthRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ValidateTokenEmailUseCase {
  constructor(
    @inject(DiTokens.authRepository)
    private readonly repository: AuthRepository,
  ) {}

  async execute(
    email: string,
    token: string,
    orgId: number | null,
  ): Promise<ValidateTokenEmailResult> {
    return this.repository.validateTokenEmail(email, token, orgId)
  }
}

