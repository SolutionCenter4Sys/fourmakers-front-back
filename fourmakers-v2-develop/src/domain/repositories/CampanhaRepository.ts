import type {
  ColetaPerfilColaboradorCampanhaRequest,
  ColetaPerfilColaboradorCampanhaResponse,
} from '@domain/entities/PerfilColaboradorCampanha'

export interface CampanhaRepository {
  coletaPerfilColaboradorCampanha(
    token: string,
    payload: ColetaPerfilColaboradorCampanhaRequest
  ): Promise<ColetaPerfilColaboradorCampanhaResponse>
}

