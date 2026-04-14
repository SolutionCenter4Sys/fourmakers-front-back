import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { ListarVigenciasColaboradorResponse } from '@data/api/TimesheetComponentesApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarVigenciasColaboradorUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    cpfColaborador: string
  ): Promise<ListarVigenciasColaboradorResponse> {
    return this.repository.listarVigenciasColaborador(token, cpfColaborador)
  }
}

