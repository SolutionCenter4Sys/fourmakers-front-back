import type {
  ColetaPerfilColaboradorCampanhaRequest,
  ColetaPerfilColaboradorCampanhaResponse,
} from '@domain/entities/PerfilColaboradorCampanha'
import { httpClient } from './httpClient'

export class CampanhaApi {
  async coletaPerfilColaboradorCampanha(
    token: string,
    payload: ColetaPerfilColaboradorCampanhaRequest
  ): Promise<ColetaPerfilColaboradorCampanhaResponse> {
    try {
      const data = await httpClient.post<ColetaPerfilColaboradorCampanhaResponse>(
        '/api/Fourmakers/Campanha/ColetaPerfilColaboradorCampanha',
        payload,
        { token }
      )

      // Se a resposta for vazia ou undefined, retornar resposta padrão
      if (!data) {
        return {
          sucesso: true,
          mensagem: '',
          erros: [],
          retorno: '',
        }
      }

      return data
    } catch (fetchError) {
      // Erro de rede (Failed to fetch, etc)
      if (fetchError instanceof Error && fetchError.message.includes('conexão')) {
        throw new Error('Erro de conexão. Verifique sua internet e tente novamente.')
      }
      throw fetchError
    }
  }
}

