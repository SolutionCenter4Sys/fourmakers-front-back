import { XANO_BASE_URL } from '@shared/constants'
import { getXanoHeaders } from '@shared/utils/xanoUtils'
import type { FelizometroPayload, FelizometroResponse } from '@domain/entities/FelizometroSentimento'
import { createHttpClient } from './httpClient'

const xanoClient = createHttpClient({ baseURL: XANO_BASE_URL })

export class FelizometroApi {
  async enviarSentimento(payload: FelizometroPayload): Promise<FelizometroResponse> {
    const xanoHeaders = getXanoHeaders() as Record<string, string>
    
    return xanoClient.post<FelizometroResponse>(
      '/felizometro',
      payload,
      { headers: xanoHeaders }
    )
  }
}

