import { inject, injectable } from 'tsyringe'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { 
  ListarAlocacoesPayload,
  ListarAlocacoesResponse
} from '@domain/entities/MapaAlocacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarAlocacoesColabETbdUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository,
  ) {}

  async execute(
    token: string,
    payload: ListarAlocacoesPayload
  ): Promise<ListarAlocacoesResponse> {
    return this.repository.listarAlocacoesColabETbd(token, payload)
  }
}

