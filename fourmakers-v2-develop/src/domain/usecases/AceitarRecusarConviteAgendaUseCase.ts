import { inject, injectable } from 'tsyringe'

import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AceitarRecusarConviteAgendaUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(
    token: string,
    params: {
      decisaoStatus: number // 1 = aceitar, 0 = recusar
      agendaId: string
      codigoColaborador: string // cpf
    },
  ): Promise<boolean> {
    return this.repository.aceitarRecusarConviteAgenda(token, params)
  }
}
