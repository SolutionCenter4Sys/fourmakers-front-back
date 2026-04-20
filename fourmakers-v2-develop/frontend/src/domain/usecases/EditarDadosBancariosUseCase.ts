import { inject, injectable } from 'tsyringe'

import type { DadosBancariosRepository } from '@domain/repositories/DadosBancariosRepository'
import type { DadosBancariosResponse, PayloadDadosBancarios } from '@domain/entities/DadosBancarios'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class EditarDadosBancariosUseCase {
  constructor(
    @inject(DiTokens.dadosBancariosRepository)
    private readonly repository: DadosBancariosRepository,
  ) {}

  async execute(token: string, payload: PayloadDadosBancarios): Promise<DadosBancariosResponse> {
    return this.repository.editarDadosBancarios(token, payload)
  }
}

