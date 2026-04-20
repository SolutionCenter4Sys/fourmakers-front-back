import { inject, injectable } from 'tsyringe'

import type {
  AuthRepository,
  ValidateTokenEmailResult,
} from '@domain/repositories/AuthRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetAccessTokenUseCase {
  constructor(
    @inject(DiTokens.authRepository)
    private readonly repository: AuthRepository,
  ) {}

  async execute(code: string, orgId: number): Promise<ValidateTokenEmailResult> {
    return this.repository.getAccessToken(code, orgId)
  }
}

