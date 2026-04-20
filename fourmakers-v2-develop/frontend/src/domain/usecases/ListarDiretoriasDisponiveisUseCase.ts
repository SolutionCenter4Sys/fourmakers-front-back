import { inject, injectable } from 'tsyringe'

import type { IntegracaoBancariaRepository } from '@domain/repositories/IntegracaoBancariaRepository'
import type { ListarDiretoriasDisponiveisResponse } from '@domain/entities/Diretoria'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarDiretoriasDisponiveisUseCase {
  constructor(
    @inject(DiTokens.integracaoBancariaRepository)
    private readonly repository: IntegracaoBancariaRepository,
  ) {}

  async execute(token: string): Promise<ListarDiretoriasDisponiveisResponse> {
    return this.repository.listarDiretoriasDisponiveis(token)
  }
}

