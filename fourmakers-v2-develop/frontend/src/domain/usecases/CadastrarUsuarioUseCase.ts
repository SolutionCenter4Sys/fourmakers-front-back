import { inject, injectable } from 'tsyringe'

import type {
  AuthRepository,
  InsereUsuarioFourmakerPayload,
  InsereUsuarioFourmakerResponse,
} from '@domain/repositories/AuthRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class CadastrarUsuarioUseCase {
  constructor(
    @inject(DiTokens.authRepository)
    private readonly repository: AuthRepository,
  ) {}

  async execute(payload: InsereUsuarioFourmakerPayload): Promise<InsereUsuarioFourmakerResponse> {
    return this.repository.insereUsuarioFourmaker(payload)
  }
}
