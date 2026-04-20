import { inject, injectable } from 'tsyringe'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { 
  EditarAlocacaoPayload,
  EditarAlocacaoResponse
} from '@domain/entities/MapaAlocacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class EditarAlocacaoUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository,
  ) {}

  async execute(
    token: string,
    payload: EditarAlocacaoPayload
  ): Promise<EditarAlocacaoResponse> {
    return this.repository.editarAlocacao(token, payload)
  }
}

