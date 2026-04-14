import { inject, injectable } from 'tsyringe'

import type { InserirInteracaoCategoriaPayload } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirInteracaoCategoriaUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(
    token: string,
    payload: InserirInteracaoCategoriaPayload,
  ): Promise<{ id?: number }> {
    return this.repository.inserirInteracaoCategoria(token, payload)
  }
}
