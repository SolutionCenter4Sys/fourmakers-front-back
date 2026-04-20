import { inject, injectable } from 'tsyringe'

import type { EncontroAiResponse } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarEncontroAiPorEncontroIdUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, encontroId: number): Promise<EncontroAiResponse | null> {
    return this.repository.buscarEncontroAiPorEncontroId(token, encontroId)
  }
}
