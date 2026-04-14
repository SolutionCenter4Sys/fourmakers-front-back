import { inject, injectable } from 'tsyringe'

import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CarregarAgendaDetalheUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, agendaId: number): Promise<ItemAgendaGestor> {
    return this.repository.carregarAgendaDetalhe(token, agendaId)
  }
}
