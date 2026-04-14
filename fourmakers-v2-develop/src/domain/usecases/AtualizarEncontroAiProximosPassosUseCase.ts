import { inject, injectable } from 'tsyringe'

import type { AtualizarEncontroAiPayload, EncontroAiResponse } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarEncontroAiProximosPassosUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, payload: AtualizarEncontroAiPayload): Promise<EncontroAiResponse> {
    return this.repository.atualizarEncontroAiProximosPassos(token, payload)
  }
}
