import { inject, injectable } from 'tsyringe'

import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarAgendasFilhosUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, agendaPaiId: number): Promise<ItemAgendaGestor[]> {
    return this.repository.buscarAgendasFilhos(token, agendaPaiId)
  }
}
