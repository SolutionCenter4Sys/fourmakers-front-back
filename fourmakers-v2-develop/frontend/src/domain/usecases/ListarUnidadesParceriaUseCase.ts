import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type { ListarUnidadesResponse } from '@domain/entities/NotaFiscalGestao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarUnidadesParceriaUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(token: string): Promise<ListarUnidadesResponse> {
    return this.repository.listarUnidades(token)
  }
}
