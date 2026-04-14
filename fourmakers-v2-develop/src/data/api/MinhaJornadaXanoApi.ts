import type { CriarMinhaJornadaPdiPayload } from '@domain/entities/MinhaJornadaGoal'
import { XANO_BASE_URL } from '@shared/constants'
import { getXanoHeaders } from '@shared/utils/xanoUtils'
import { createHttpClient } from './httpClient'

const xanoClient = createHttpClient({ baseURL: XANO_BASE_URL })

export class MinhaJornadaXanoApi {
  async criarPdi(payload: CriarMinhaJornadaPdiPayload): Promise<void> {
    const xanoHeaders = getXanoHeaders() as Record<string, string>
    
    await xanoClient.post<void>(
      '/pdi/novo/criarpdi',
      payload,
      { headers: xanoHeaders }
    )
  }
}



