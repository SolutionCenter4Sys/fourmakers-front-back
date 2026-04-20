import { inject, injectable } from 'tsyringe'

import type { DadosBancariosRepository } from '@domain/repositories/DadosBancariosRepository'
import type { DadosBancariosResponse } from '@domain/entities/DadosBancarios'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetDadosBancariosUseCase {
  constructor(
    @inject(DiTokens.dadosBancariosRepository)
    private readonly repository: DadosBancariosRepository,
  ) {}

  async buscarPorColaborador(token: string): Promise<DadosBancariosResponse> {
    return this.repository.buscarDadosBancariosPorColaborador(token)
  }
}

