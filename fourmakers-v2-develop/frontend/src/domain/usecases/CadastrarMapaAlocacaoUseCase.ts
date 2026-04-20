import { inject, injectable } from 'tsyringe'
import { DiTokens } from '@core/di/tokens'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { CadastrarMapaAlocacaoPayload, CadastrarMapaAlocacaoResponse } from '@domain/entities/MapaAlocacao'

@injectable()
export class CadastrarMapaAlocacaoUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository
  ) {}

  async execute(
    token: string,
    payload: CadastrarMapaAlocacaoPayload
  ): Promise<CadastrarMapaAlocacaoResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório')
    }

    return this.repository.cadastrarMapaAlocacao(token, payload)
  }
}
