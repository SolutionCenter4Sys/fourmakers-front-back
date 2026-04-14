import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { PeriodoFechado } from '@data/api/TimesheetComponentesApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarPeriodoFechadoUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(token: string): Promise<PeriodoFechado> {
    return this.repository.buscarPeriodoFechado(token)
  }
}

