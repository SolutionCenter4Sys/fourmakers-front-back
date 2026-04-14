import { inject, injectable } from 'tsyringe'

import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DeletarArquivoEncontroUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(token: string, arquivoId: number): Promise<void> {
    return this.repository.deletarArquivoEncontro(token, arquivoId)
  }
}
