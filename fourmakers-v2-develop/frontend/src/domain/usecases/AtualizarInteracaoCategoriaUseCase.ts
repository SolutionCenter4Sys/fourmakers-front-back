import { inject, injectable } from 'tsyringe'

import type { AtualizarInteracaoCategoriaPayload } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarInteracaoCategoriaUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(
    token: string,
    payload: AtualizarInteracaoCategoriaPayload,
  ): Promise<void> {
    return this.repository.atualizarInteracaoCategoria(token, payload)
  }
}
