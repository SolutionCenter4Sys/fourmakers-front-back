import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarColaboradoresVinculadosGerenteDeProjetoUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    codProjeto: string,
    mes: number,
    ano: number
  ) {
    return this.repository.listarColaboradoresVinculadosGerenteDeProjeto(token, codProjeto, mes, ano)
  }
}

