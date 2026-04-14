import { inject, injectable } from 'tsyringe'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { 
  RemoverAlocacoesEmLotePayload,
  RemoverAlocacoesEmLoteResponse
} from '@domain/entities/MapaAlocacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class RemoverAlocacoesEmLoteUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository,
  ) {}

  async execute(
    token: string,
    payload: RemoverAlocacoesEmLotePayload
  ): Promise<RemoverAlocacoesEmLoteResponse> {
    return this.repository.removerAlocacoesEmLote(token, payload)
  }
}

