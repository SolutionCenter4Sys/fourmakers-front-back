import { inject, injectable } from 'tsyringe'

import type { AgendaResponse, CriarAgendaPayload } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarAgendaUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, payload: CriarAgendaPayload): Promise<AgendaResponse> {
    return this.repository.criarAgenda(token, payload)
  }
}
