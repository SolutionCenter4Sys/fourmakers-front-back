import { XANO_BASE_URL } from '@shared/constants'
import { getXanoHeaders } from '@shared/utils/xanoUtils'
import { createHttpClient } from './httpClient'
import type { BeneficioXanoListItem, BeneficioXanoDetail } from '@domain/entities/BeneficioXano'

const xanoClient = createHttpClient({ baseURL: XANO_BASE_URL })

export class BeneficioXanoApi {
  /** GET .../beneficio_fourmakers?orgId=&user_id= */
  async listar(orgId: number, userId: string): Promise<BeneficioXanoListItem[]> {
    const params = new URLSearchParams({ orgId: String(orgId), user_id: userId })
    const headers = getXanoHeaders() as Record<string, string>
    const data = await xanoClient.get<BeneficioXanoListItem[]>(
      `/beneficio_fourmakers?${params}`,
      { headers }
    )
    return Array.isArray(data) ? data : []
  }

  /** GET .../beneficio_fourmakers/:id */
  async obterPorId(id: number): Promise<BeneficioXanoDetail | null> {
    const headers = getXanoHeaders() as Record<string, string>
    const data = await xanoClient.get<BeneficioXanoDetail>(
      `/beneficio_fourmakers/${id}`,
      { headers }
    )
    return data ?? null
  }
}
