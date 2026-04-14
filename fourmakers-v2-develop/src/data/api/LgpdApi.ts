import { XANO_BASE_URL } from '@shared/constants'
import { getXanoHeaders } from '@shared/utils/xanoUtils'
import { createHttpClient } from './httpClient'

export interface LgpdPayload {
  cpf_pessoa: string
  is_colab: boolean
  nome_pessoa: string
  unidade: string
  contrato: string
}

const xanoClient = createHttpClient({ baseURL: XANO_BASE_URL })

export class LgpdApi {
  async registrarLgpd(payload: LgpdPayload): Promise<void> {
    const headers = getXanoHeaders() as Record<string, string>
    await xanoClient.post<unknown>('/lgpd', payload, { headers })
  }
}
