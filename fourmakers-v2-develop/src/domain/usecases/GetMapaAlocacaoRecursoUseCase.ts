import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository';
import type { GetMapaAlocacaoRecursoParams, GetMapaAlocacaoRecursoResponse } from '@domain/entities/MapaAlocacao';

@injectable()
export class GetMapaAlocacaoRecursoUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository
  ) {}

  async execute(
    token: string,
    params: GetMapaAlocacaoRecursoParams
  ): Promise<GetMapaAlocacaoRecursoResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    return this.repository.getMapaAlocacaoRecurso(token, params);
  }
}
