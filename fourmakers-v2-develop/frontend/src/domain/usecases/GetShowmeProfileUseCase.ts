import { inject, injectable } from 'tsyringe'

import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'
import type { AuthRepository } from '@domain/repositories/AuthRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetShowmeProfileUseCase {
  constructor(
    @inject(DiTokens.authRepository)
    private readonly repository: AuthRepository,
  ) {}

  async execute(token: string): Promise<ShowmeUserProfile> {
    return this.repository.fetchShowmeProfile(token)
  }
}

