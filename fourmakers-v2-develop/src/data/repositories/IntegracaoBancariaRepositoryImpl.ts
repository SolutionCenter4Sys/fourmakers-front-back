import { inject, injectable } from 'tsyringe'

import type { IntegracaoBancariaRepository } from '@domain/repositories/IntegracaoBancariaRepository'
import type { ListarDiretoriasDisponiveisResponse } from '@domain/entities/Diretoria'

import { IntegracaoBancariaApi } from '@data/api/IntegracaoBancariaApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class IntegracaoBancariaRepositoryImpl implements IntegracaoBancariaRepository {
  constructor(
    @inject(DiTokens.integracaoBancariaApi)
    private readonly api: IntegracaoBancariaApi,
  ) {}

  async listarDiretoriasDisponiveis(token: string): Promise<ListarDiretoriasDisponiveisResponse> {
    return this.api.listarDiretoriasDisponiveis(token)
  }
}

