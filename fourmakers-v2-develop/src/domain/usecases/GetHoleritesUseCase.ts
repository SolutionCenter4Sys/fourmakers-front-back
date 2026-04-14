import { inject, injectable } from 'tsyringe';
import type { HoleritesRepository } from '@domain/repositories/HoleritesRepository';
import type { 
  ListarHoleritesColaboradorPorAnoParams,
  ListarHoleritesColaboradorPorAnoResponse,
  AssinarHoleritePorLoteIdParams,
  AssinarHoleritePorLoteIdResponse
} from '@domain/entities/Holerite';
import { DiTokens } from '@core/di/tokens';

@injectable()
export class GetHoleritesUseCase {
  constructor(
    @inject(DiTokens.holeritesRepository)
    private readonly repository: HoleritesRepository,
  ) {}

  async executeListarHoleritesColaboradorPorAno(
    token: string,
    params: ListarHoleritesColaboradorPorAnoParams
  ): Promise<ListarHoleritesColaboradorPorAnoResponse> {
    return this.repository.listarHoleritesColaboradorPorAno(token, params);
  }

  async executeAssinarHoleritePorLoteId(
    token: string,
    params: AssinarHoleritePorLoteIdParams
  ): Promise<AssinarHoleritePorLoteIdResponse> {
    return this.repository.assinarHoleritePorLoteId(token, params);
  }
}

