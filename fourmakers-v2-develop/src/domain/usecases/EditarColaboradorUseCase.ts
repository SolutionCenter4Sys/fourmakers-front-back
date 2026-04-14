import { inject, injectable } from 'tsyringe'

import type { ColaboradoresRepository, EditarColaboradorPayload, ColaboradorResponse } from '@domain/repositories/ColaboradoresRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class EditarColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, payload: EditarColaboradorPayload): Promise<ColaboradorResponse> {
    return this.repository.editarColaborador(token, payload)
  }
}





