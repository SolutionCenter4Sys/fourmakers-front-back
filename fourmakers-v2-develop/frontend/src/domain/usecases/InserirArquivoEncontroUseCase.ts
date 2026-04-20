import { inject, injectable } from 'tsyringe'

import type {
  ArquivoEncontroDto,
  InserirArquivoEncontroParams,
} from '@domain/entities/AgendaGestor'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirArquivoEncontroUseCase {
  constructor(
    @inject(DiTokens.agendaGestorRepository)
    private readonly repository: AgendaGestorRepository,
  ) {}

  async execute(
    token: string,
    params: InserirArquivoEncontroParams,
  ): Promise<ArquivoEncontroDto> {
    return this.repository.inserirArquivoEncontro(token, params)
  }
}
