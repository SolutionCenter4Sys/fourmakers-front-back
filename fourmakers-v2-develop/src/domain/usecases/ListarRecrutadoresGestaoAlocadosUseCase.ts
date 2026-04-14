import { inject, injectable } from 'tsyringe'

import type { RecrutadoresGestaoAlocadosRepository } from '@domain/repositories/RecrutadoresGestaoAlocadosRepository'
import type { RecrutadoresGestaoAlocadosResponse } from '@domain/entities/RecrutadorGestaoAlocados'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarRecrutadoresGestaoAlocadosUseCase {
  constructor(
    @inject(DiTokens.recrutadoresGestaoAlocadosRepository)
    private readonly repository: RecrutadoresGestaoAlocadosRepository,
  ) {}

  async execute(
    token: string,
    busca: string,
  ): Promise<RecrutadoresGestaoAlocadosResponse> {
    return this.repository.listarRecrutadores(token, busca)
  }
}
