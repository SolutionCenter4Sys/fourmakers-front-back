import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { ListarApontamentosPorVigenciaResponse } from '@data/api/TimesheetComponentesApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarApontamentosPorVigenciaUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    mes: number,
    ano: number,
    cpfColaborador: string
  ): Promise<ListarApontamentosPorVigenciaResponse> {
    return this.repository.listarApontamentosPorVigencia(token, mes, ano, cpfColaborador)
  }
}

