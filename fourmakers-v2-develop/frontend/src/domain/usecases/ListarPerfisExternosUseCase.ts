import { inject, injectable } from 'tsyringe'

import type { ListarPerfisParams, MapaRelacionamentoRepository } from '@domain/repositories/MapaRelacionamentoRepository'
import type { PerfisExternosResponse } from '@domain/entities/MapaRelacionamento'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarPerfisExternosUseCase {
  constructor(
    @inject(DiTokens.mapaRelacionamentoRepository)
    private readonly repository: MapaRelacionamentoRepository,
  ) {}

  async execute(token: string, params: ListarPerfisParams): Promise<PerfisExternosResponse> {
    return this.repository.listarPerfisExternos(token, params)
  }
}
