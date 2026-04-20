import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarProjetosVisaoGerenteDeProjetoUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    params: {
      codProjeto?: string
      mes: number
      ano: number
      cpfColaborador?: string
      codStatusGrupo?: string
      cpfGerenteAdm?: string
    }
  ) {
    return this.repository.listarProjetosVisaoGerenteDeProjeto(token, params)
  }
}

