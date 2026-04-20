import { inject, injectable } from 'tsyringe'

import type { AcaoResponse, InserirComentarioAcaoPayload } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirComentarioAcaoUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, payload: InserirComentarioAcaoPayload): Promise<AcaoResponse> {
    return this.repository.inserirComentarioAcao(token, payload)
  }
}
