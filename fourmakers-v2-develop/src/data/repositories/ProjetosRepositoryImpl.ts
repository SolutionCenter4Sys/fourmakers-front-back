import { inject, injectable } from 'tsyringe'

import type { ProjetosRepository } from '@domain/repositories/ProjetosRepository'
import type { ProjetoMock } from '@data/mocks/projetosMock'
import type { ListarStatusProjetoResponse } from '@data/api/ProjetosApi'

import { ProjetosApi } from '@data/api/ProjetosApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ProjetosRepositoryImpl implements ProjetosRepository {
  constructor(
    @inject(DiTokens.projetosApi)
    private readonly api: ProjetosApi,
  ) {}

  async getProjetos(): Promise<ProjetoMock[]> {
    return this.api.getProjetos()
  }

  async listarStatusProjeto(token: string): Promise<ListarStatusProjetoResponse> {
    return this.api.listarStatusProjeto(token)
  }
}

