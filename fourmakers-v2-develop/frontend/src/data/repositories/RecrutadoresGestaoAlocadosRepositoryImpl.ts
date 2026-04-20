import { inject, injectable } from 'tsyringe'

import type { RecrutadoresGestaoAlocadosRepository } from '@domain/repositories/RecrutadoresGestaoAlocadosRepository'
import type { RecrutadoresGestaoAlocadosResponse } from '@domain/entities/RecrutadorGestaoAlocados'

import { DiTokens } from '@core/di/tokens'
import { RecrutadoresGestaoAlocadosApi } from '@data/api/RecrutadoresGestaoAlocadosApi'

@injectable()
export class RecrutadoresGestaoAlocadosRepositoryImpl
  implements RecrutadoresGestaoAlocadosRepository {
  constructor(
    @inject(DiTokens.recrutadoresGestaoAlocadosApi)
    private readonly api: RecrutadoresGestaoAlocadosApi,
  ) {}

  async listarRecrutadores(
    token: string,
    busca: string,
  ): Promise<RecrutadoresGestaoAlocadosResponse> {
    return this.api.listarRecrutadores(token, busca)
  }
}
