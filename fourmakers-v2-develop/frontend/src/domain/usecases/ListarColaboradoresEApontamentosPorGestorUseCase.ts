import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { ListarColaboradoresEApontamentosPorGestorResponse } from '@data/api/TimesheetComponentesApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarColaboradoresEApontamentosPorGestorUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    params: {
      nomeColaborador?: string
      codigoGerente?: string
      codigoStatus?: string
      mesVigencia: number
      anoVigencia: number
      cursor?: number
      limite?: number
      codProjeto?: string
      codColaboradorExternoAprovador?: string
    }
  ): Promise<ListarColaboradoresEApontamentosPorGestorResponse> {
    return this.repository.listarColaboradoresEApontamentosPorGestor(token, params)
  }
}

