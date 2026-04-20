import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { PayloadEditarApontamento } from '@domain/repositories/TimesheetRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class EditarApontamentoUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    payload: PayloadEditarApontamento
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return this.repository.editarApontamento(token, payload)
  }
}

