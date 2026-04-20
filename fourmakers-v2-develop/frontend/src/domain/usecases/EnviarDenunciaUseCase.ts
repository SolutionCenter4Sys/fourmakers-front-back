import { inject, injectable } from 'tsyringe'

import type { CanalDenunciaRepository } from '@domain/repositories/CanalDenunciaRepository'
import type { EnviarDenunciaPayload, EnviarDenunciaResponse } from '@data/api/CanalDenunciaApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class EnviarDenunciaUseCase {
  constructor(
    @inject(DiTokens.canalDenunciaRepository)
    private readonly repository: CanalDenunciaRepository,
  ) {}

  async execute(
    token: string,
    payload: EnviarDenunciaPayload
  ): Promise<EnviarDenunciaResponse> {
    return this.repository.enviarDenuncia(token, payload)
  }
}

