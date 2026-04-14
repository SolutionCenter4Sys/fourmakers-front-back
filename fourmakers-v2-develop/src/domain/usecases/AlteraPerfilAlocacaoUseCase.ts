import { inject, injectable } from 'tsyringe'
import { DiTokens } from '@core/di/tokens'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { AlteraPerfilAlocacaoPayload, AlteraPerfilAlocacaoResponse } from '@domain/entities/MapaAlocacao'

@injectable()
export class AlteraPerfilAlocacaoUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository
  ) {}

  async execute(
    token: string,
    payload: AlteraPerfilAlocacaoPayload
  ): Promise<AlteraPerfilAlocacaoResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório')
    }

    return this.repository.alteraPerfilAlocacao(token, payload)
  }
}
