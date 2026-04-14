import { httpClient } from './httpClient'
import type {
  ListarUnidadesRemessaResponse,
  UnidadeRemessaDTO,
} from '@shared/types/integracaoContabilApi'

/**
 * API para listar unidades (CNPJ) usadas na geração de remessa contábil.
 * A organização é inferida pelo backend a partir do token.
 * Usa a base da API do env (VITE_API_FOURMAKERS_URL) como no restante do projeto.
 */
export class CompetenciaRemessaApi {
  /**
   * Lista unidades por organização (org obtida do token).
   */
  async listarUnidades(token: string): Promise<UnidadeRemessaDTO[]> {
    const response = await httpClient.get<ListarUnidadesRemessaResponse>(
      '/api/Competencia/ListarUnidadesPorOrgId',
      { token }
    )
    if (!response?.sucesso || !Array.isArray(response.ListaUnidadesResult)) {
      return []
    }
    return response.ListaUnidadesResult
  }
}
