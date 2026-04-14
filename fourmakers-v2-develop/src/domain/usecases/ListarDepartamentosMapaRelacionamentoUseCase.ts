import { inject, injectable } from 'tsyringe'

import type { DepartamentoResponse } from '@domain/entities/Organograma'
import type { MapaRelacionamentoRepository } from '@domain/repositories/MapaRelacionamentoRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarDepartamentosMapaRelacionamentoUseCase {
  constructor(
    @inject(DiTokens.mapaRelacionamentoRepository)
    private readonly repository: MapaRelacionamentoRepository,
  ) {}

  async execute(token: string, codigoCliente: string): Promise<DepartamentoResponse[]> {
    return this.repository.listarDepartamentos(token, codigoCliente)
  }
}
