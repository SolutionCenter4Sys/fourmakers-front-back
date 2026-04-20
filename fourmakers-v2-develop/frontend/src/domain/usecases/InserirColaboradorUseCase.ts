import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository, InserirColaboradorPayload, ColaboradorResponse } from '@domain/repositories/ColaboradoresRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, payload: InserirColaboradorPayload): Promise<ColaboradorResponse> {
    return this.repository.inserirColaborador(token, payload)
  }
}





