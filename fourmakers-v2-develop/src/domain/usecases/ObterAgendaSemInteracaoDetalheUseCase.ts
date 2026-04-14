import { inject, injectable } from 'tsyringe'
import type {
  EncontrosBigNumbersRepository,
  ObterBigNumbersParams,
} from '@domain/repositories/EncontrosBigNumbersRepository'
import type { AgendaSemInteracaoDetalhe } from '@shared/types/dashboardComercialTypes'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterAgendaSemInteracaoDetalheUseCase {
  constructor(
    @inject(DiTokens.encontrosBigNumbersRepository)
    private readonly repository: EncontrosBigNumbersRepository,
  ) {}

  async execute(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<AgendaSemInteracaoDetalhe[]> {
    return this.repository.agendaSemInteracaoDetalhe(token, params)
  }
}
