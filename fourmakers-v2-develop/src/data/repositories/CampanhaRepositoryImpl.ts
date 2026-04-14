import { inject, injectable } from 'tsyringe'

import type {
  ColetaPerfilColaboradorCampanhaRequest,
  ColetaPerfilColaboradorCampanhaResponse,
} from '@domain/entities/PerfilColaboradorCampanha'
import type { CampanhaRepository } from '@domain/repositories/CampanhaRepository'

import { CampanhaApi } from '@data/api/CampanhaApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CampanhaRepositoryImpl implements CampanhaRepository {
  constructor(
    @inject(DiTokens.campanhaApi)
    private readonly api: CampanhaApi,
  ) {}

  async coletaPerfilColaboradorCampanha(
    token: string,
    payload: ColetaPerfilColaboradorCampanhaRequest
  ): Promise<ColetaPerfilColaboradorCampanhaResponse> {
    return this.api.coletaPerfilColaboradorCampanha(token, payload)
  }
}

