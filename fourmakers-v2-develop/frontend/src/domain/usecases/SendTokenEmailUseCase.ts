import { inject, injectable } from 'tsyringe'

import type {
  AuthRepository,
  SendTokenEmailResult,
} from '@domain/repositories/AuthRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class SendTokenEmailUseCase {
  constructor(
    @inject(DiTokens.authRepository)
    private readonly repository: AuthRepository,
  ) {}

  async execute(email: string, orgId: number | null): Promise<SendTokenEmailResult> {
    return this.repository.sendTokenEmail(email, orgId)
  }
}

