import { inject, injectable } from 'tsyringe'

import type { InteracaoResponse, InserirInteracaoIaPayload } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirInteracaoIaUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, payload: InserirInteracaoIaPayload): Promise<InteracaoResponse> {
    return this.repository.inserirInteracaoIa(token, payload)
  }
}
