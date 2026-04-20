import { inject, injectable } from 'tsyringe'
import type {
  EncontrosBigNumbersRepository,
  ObterBigNumbersParams,
} from '@domain/repositories/EncontrosBigNumbersRepository'
import type { AgendaRealizadaDetalhe } from '@shared/types/dashboardComercialTypes'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterAgendaRealizadaDetalheUseCase {
  constructor(
    @inject(DiTokens.encontrosBigNumbersRepository)
    private readonly repository: EncontrosBigNumbersRepository,
  ) {}

  async execute(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<AgendaRealizadaDetalhe[]> {
    return this.repository.agendaRealizadaDetalhe(token, params)
  }
}
