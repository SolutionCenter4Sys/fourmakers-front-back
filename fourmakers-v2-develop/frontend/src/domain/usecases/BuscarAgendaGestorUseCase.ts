import { inject, injectable } from 'tsyringe'

import type { AgendaGestorCompleto } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarAgendaGestorUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(
    token: string,
    codInternoColaborador: string,
    dataInicio?: string,
    dataFim?: string,
  ): Promise<AgendaGestorCompleto> {
    return this.repository.buscarAgendaCompleta(token, codInternoColaborador, dataInicio, dataFim)
  }
}
