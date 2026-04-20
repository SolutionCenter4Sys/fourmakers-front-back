import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class FecharAlterarPeriodoUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    dataFim: string
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    return this.repository.fecharAlterarPeriodo(token, dataFim)
  }
}

