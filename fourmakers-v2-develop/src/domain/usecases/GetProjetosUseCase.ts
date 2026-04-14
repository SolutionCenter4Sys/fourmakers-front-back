import { inject, injectable } from 'tsyringe'

import type { ProjetosRepository } from '@domain/repositories/ProjetosRepository'
import type { ProjetoMock } from '@data/mocks/projetosMock'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetProjetosUseCase {
  constructor(
    @inject(DiTokens.projetosRepository)
    private readonly repository: ProjetosRepository,
  ) {}

  async execute(): Promise<ProjetoMock[]> {
    return this.repository.getProjetos()
  }
}

