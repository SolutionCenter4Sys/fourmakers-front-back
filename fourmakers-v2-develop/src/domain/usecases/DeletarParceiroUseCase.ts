import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  DeletarParceiroParams,
  DeletarParceiroResponse,
} from '@domain/entities/Parceiro'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DeletarParceiroUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(
    token: string,
    params: DeletarParceiroParams
  ): Promise<DeletarParceiroResponse> {
    return this.repository.deletarParceiro(token, params)
  }
}
