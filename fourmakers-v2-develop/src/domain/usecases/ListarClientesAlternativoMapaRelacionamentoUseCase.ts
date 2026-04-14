import { inject, injectable } from 'tsyringe'

import type { ClientesMapaRelacionamentoResponse } from '@domain/entities/MapaRelacionamento'
import type { MapaRelacionamentoRepository } from '@domain/repositories/MapaRelacionamentoRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarClientesAlternativoMapaRelacionamentoUseCase {
  constructor(
    @inject(DiTokens.mapaRelacionamentoRepository)
    private readonly repository: MapaRelacionamentoRepository,
  ) {}

  async execute(token: string, limite: number): Promise<ClientesMapaRelacionamentoResponse> {
    return this.repository.listarClientesAlternativo(token, limite)
  }
}
