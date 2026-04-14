import { inject, injectable } from 'tsyringe'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { 
  ListarNomesGestoresParams,
  ListarNomesGestoresResponse
} from '@domain/entities/MapaAlocacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarNomesGestoresUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository,
  ) {}

  async execute(
    token: string,
    params?: ListarNomesGestoresParams
  ): Promise<ListarNomesGestoresResponse> {
    return this.repository.listarNomesGestores(token, params)
  }
}

