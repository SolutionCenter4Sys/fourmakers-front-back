import { inject, injectable } from 'tsyringe'
import type {
  EncontrosBigNumbersRepository,
  ObterBigNumbersParams,
} from '@domain/repositories/EncontrosBigNumbersRepository'
import type { GestoresImpactadosDetalhe } from '@shared/types/dashboardComercialTypes'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterGestoresImpactadosDetalheUseCase {
  constructor(
    @inject(DiTokens.encontrosBigNumbersRepository)
    private readonly repository: EncontrosBigNumbersRepository,
  ) {}

  async execute(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<GestoresImpactadosDetalhe[]> {
    return this.repository.gestoresImpactadosDetalhe(token, params)
  }
}
