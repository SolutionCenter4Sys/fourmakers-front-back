import type { EnviarDenunciaPayload, EnviarDenunciaResponse } from '@data/api/CanalDenunciaApi'

export interface CanalDenunciaRepository {
  enviarDenuncia(
    token: string,
    payload: EnviarDenunciaPayload
  ): Promise<EnviarDenunciaResponse>
}

