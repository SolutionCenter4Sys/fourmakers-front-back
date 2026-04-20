import { inject, injectable } from 'tsyringe'

import type { CategoriaAssuntoComSubModel } from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarCategoriasAssuntoComSubUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string): Promise<CategoriaAssuntoComSubModel[]> {
    return this.repository.listarCategoriasAssuntoComSub(token)
  }
}
