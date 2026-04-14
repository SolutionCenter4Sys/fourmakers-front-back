import { inject, injectable } from 'tsyringe'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { 
  ListarColaboradoresETbdsParams,
  ListarColaboradoresETbdsResponse
} from '@domain/entities/MapaAlocacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarColaboradoresETbdsUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository,
  ) {}

  async execute(
    token: string,
    params?: ListarColaboradoresETbdsParams
  ): Promise<ListarColaboradoresETbdsResponse> {
    return this.repository.listarColaboradoresETbds(token, params)
  }
}

