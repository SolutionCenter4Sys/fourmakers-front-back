import { inject, injectable } from 'tsyringe'
import type { ProjetosRepository } from '@domain/repositories/ProjetosRepository'
import type { ListarStatusProjetoResponse } from '@data/api/ProjetosApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarStatusProjetoUseCase {
  constructor(
    @inject(DiTokens.projetosRepository)
    private readonly repository: ProjetosRepository,
  ) {}

  async execute(
    token: string
  ): Promise<ListarStatusProjetoResponse> {
    return this.repository.listarStatusProjeto(token)
  }
}

