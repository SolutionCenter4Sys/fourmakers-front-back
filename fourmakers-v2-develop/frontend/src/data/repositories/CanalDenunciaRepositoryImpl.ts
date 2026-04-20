import { inject, injectable } from 'tsyringe'

import type { CanalDenunciaRepository } from '@domain/repositories/CanalDenunciaRepository'
import type { EnviarDenunciaPayload, EnviarDenunciaResponse } from '@data/api/CanalDenunciaApi'

import { CanalDenunciaApi } from '@data/api/CanalDenunciaApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CanalDenunciaRepositoryImpl implements CanalDenunciaRepository {
  constructor(
    @inject(DiTokens.canalDenunciaApi)
    private readonly api: CanalDenunciaApi,
  ) {}

  async enviarDenuncia(
    token: string,
    payload: EnviarDenunciaPayload
  ): Promise<EnviarDenunciaResponse> {
    return this.api.enviarDenuncia(token, payload)
  }
}

