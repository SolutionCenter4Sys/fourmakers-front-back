import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository';
import type { GetMapaAlocacaoResumoParams, GetMapaAlocacaoResumoResponse } from '@domain/entities/MapaAlocacao';

@injectable()
export class GetMapaAlocacaoResumoUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository
  ) {}

  async execute(
    token: string,
    params: GetMapaAlocacaoResumoParams
  ): Promise<GetMapaAlocacaoResumoResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    return this.repository.getMapaAlocacaoResumo(token, params);
  }
}
