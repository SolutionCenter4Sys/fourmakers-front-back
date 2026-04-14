import { inject, injectable } from 'tsyringe'

import type { GestoresExternosResponse } from '@domain/entities/MapaRelacionamento'
import type { ListarGestoresParams, MapaRelacionamentoRepository } from '@domain/repositories/MapaRelacionamentoRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarGestoresExternosUseCase {
  constructor(
    @inject(DiTokens.mapaRelacionamentoRepository)
    private readonly repository: MapaRelacionamentoRepository,
  ) {}

  async execute(token: string, params: ListarGestoresParams): Promise<GestoresExternosResponse> {
    return this.repository.listarGestoresExternos(token, params)
  }
}
