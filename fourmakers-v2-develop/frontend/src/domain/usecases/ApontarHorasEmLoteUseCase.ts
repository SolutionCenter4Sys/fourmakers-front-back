import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { PayloadApontarHorasEmLote } from '@domain/repositories/TimesheetRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ApontarHorasEmLoteUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    payload: PayloadApontarHorasEmLote
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return this.repository.apontarHorasEmLote(token, payload)
  }
}

