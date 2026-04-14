import { inject, injectable } from 'tsyringe'
import { DiTokens } from '@core/di/tokens'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { ListarPerfilAlocacaoParams, ListarPerfilAlocacaoResponse } from '@domain/entities/MapaAlocacao'

@injectable()
export class ListarPerfilAlocacaoUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository
  ) {}

  async execute(
    token: string,
    params?: ListarPerfilAlocacaoParams
  ): Promise<ListarPerfilAlocacaoResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório')
    }

    return this.repository.listarPerfilAlocacao(token, params)
  }
}
