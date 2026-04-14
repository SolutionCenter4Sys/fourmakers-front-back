import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class EnviarEmailNotificacaoAprovadoresUseCase {
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
      codProjeto?: string
      codColaboradorExternoAprovador?: string
    }
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    return this.repository.enviaEmailNotificacaoAprovadoresStatusPendentes(token, params)
  }
}

