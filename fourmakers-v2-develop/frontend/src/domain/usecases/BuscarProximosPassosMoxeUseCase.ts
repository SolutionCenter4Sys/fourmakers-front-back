import { inject, injectable } from 'tsyringe'

import type { BuscarProximosPassosMoxeRequest, BuscarProximosPassosMoxeResponse } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarProximosPassosMoxeUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, payload: BuscarProximosPassosMoxeRequest): Promise<BuscarProximosPassosMoxeResponse> {
    return this.repository.buscarProximosPassosIntegracaoMoxe(token, payload)
  }
}
