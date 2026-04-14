import { inject, injectable } from 'tsyringe'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { 
  ListarProjetosColaboradorMapaParams,
  ListarProjetosColaboradorMapaResponse
} from '@domain/entities/MapaAlocacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarProjetosColaboradorMapaUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository,
  ) {}

  async execute(
    token: string,
    params?: ListarProjetosColaboradorMapaParams
  ): Promise<ListarProjetosColaboradorMapaResponse> {
    return this.repository.listarProjetosColaborador(token, params)
  }
}

