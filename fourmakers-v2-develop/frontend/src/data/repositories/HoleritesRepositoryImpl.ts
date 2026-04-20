import { inject, injectable } from 'tsyringe';
import type { HoleritesRepository } from '@domain/repositories/HoleritesRepository';
import type { 
  ListarHoleritesColaboradorPorAnoParams,
  ListarHoleritesColaboradorPorAnoResponse,
  AssinarHoleritePorLoteIdParams,
  AssinarHoleritePorLoteIdResponse
} from '@domain/entities/Holerite';
import { HoleritesApi } from '@data/api/HoleritesApi';
import { DiTokens } from '@core/di/tokens';

@injectable()
export class HoleritesRepositoryImpl implements HoleritesRepository {
  constructor(
    @inject(DiTokens.holeritesApi)
    private readonly api: HoleritesApi,
  ) {}

  async listarHoleritesColaboradorPorAno(
    token: string,
    params: ListarHoleritesColaboradorPorAnoParams
  ): Promise<ListarHoleritesColaboradorPorAnoResponse> {
    return this.api.listarHoleritesColaboradorPorAno(token, params);
  }

  async assinarHoleritePorLoteId(
    token: string,
    params: AssinarHoleritePorLoteIdParams
  ): Promise<AssinarHoleritePorLoteIdResponse> {
    return this.api.assinarHoleritePorLoteId(token, params);
  }
}

