import { inject, injectable } from 'tsyringe'

import type { CriarEncontroAiPayload, EncontroAiResponse } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarEncontroAiProximosPassosUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, payload: CriarEncontroAiPayload): Promise<EncontroAiResponse> {
    return this.repository.criarEncontroAiProximosPassos(token, payload)
  }
}
