import { inject, injectable } from 'tsyringe'

import type { AgendaResponse, AtualizarAgendaPayload } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarAgendaUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, payload: AtualizarAgendaPayload): Promise<AgendaResponse> {
    return this.repository.atualizarAgenda(token, payload)
  }
}
