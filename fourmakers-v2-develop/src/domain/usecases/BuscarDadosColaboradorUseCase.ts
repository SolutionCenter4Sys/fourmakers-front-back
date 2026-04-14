import { inject, injectable } from 'tsyringe'
import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { BuscarDadosColaboradorResponse } from '@domain/entities/Profile360'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarDadosColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, cpf: string): Promise<BuscarDadosColaboradorResponse> {
    return this.repository.buscarDadosColaborador(token, cpf)
  }
}
