import { inject, injectable } from 'tsyringe'

import type { InteracaoResponse, AtualizarInteracaoIaPayload } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarInteracaoIaUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, payload: AtualizarInteracaoIaPayload): Promise<InteracaoResponse> {
    return this.repository.atualizarInteracaoIa(token, payload)
  }
}
