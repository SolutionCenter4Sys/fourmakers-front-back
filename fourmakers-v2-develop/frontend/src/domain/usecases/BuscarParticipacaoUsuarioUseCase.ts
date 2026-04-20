import { inject, injectable } from 'tsyringe'

import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import type { ParticipacaoUsuarioLogado } from '@domain/entities/AgendaGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarParticipacaoUsuarioUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, agendaId: number): Promise<ParticipacaoUsuarioLogado | null> {
    return this.repository.buscarParticipacaoUsuario(token, agendaId)
  }
}
