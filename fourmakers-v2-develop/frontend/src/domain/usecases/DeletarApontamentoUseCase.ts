import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DeletarApontamentoUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    apontamentoId: string,
    dataColetaDeDados: string
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return this.repository.deletarApontamentoColaborador(token, apontamentoId, dataColetaDeDados)
  }
}

