import { inject, injectable } from 'tsyringe'
import type {
  EncontrosBigNumbersRepository,
  ObterBigNumbersParams,
} from '@domain/repositories/EncontrosBigNumbersRepository'
import type { ClienteImpactadoDetalhe } from '@shared/types/dashboardComercialTypes'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterClientesImpactadosDetalheUseCase {
  constructor(
    @inject(DiTokens.encontrosBigNumbersRepository)
    private readonly repository: EncontrosBigNumbersRepository,
  ) {}

  async execute(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ClienteImpactadoDetalhe[]> {
    return this.repository.clientesImpactadosDetalhe(token, params)
  }
}
