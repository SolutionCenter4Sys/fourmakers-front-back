import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { ListarVigenciasApontamentosGerenteDeProjetoResponse } from '@data/api/TimesheetComponentesApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarVigenciasApontamentosGerenteDeProjetoUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string
  ): Promise<ListarVigenciasApontamentosGerenteDeProjetoResponse> {
    return this.repository.listarVigenciasApontamentosGerenteDeProjeto(token)
  }
}

