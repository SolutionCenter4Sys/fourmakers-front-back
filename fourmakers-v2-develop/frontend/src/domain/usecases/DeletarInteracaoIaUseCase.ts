import { inject, injectable } from 'tsyringe'

import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DeletarInteracaoIaUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, interacaoId: number): Promise<boolean> {
    return this.repository.deletarInteracaoIa(token, interacaoId)
  }
}
