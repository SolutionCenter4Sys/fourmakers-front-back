import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { ListarTemplateSemanaVigenciaResponse } from '@data/api/TimesheetComponentesApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarTemplateSemanaVigenciaUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    mes: number,
    ano: number
  ): Promise<ListarTemplateSemanaVigenciaResponse> {
    return this.repository.listarTemplateSemanaVigencia(token, mes, ano)
  }
}

