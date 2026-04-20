import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  AtualizarParceiroPayload,
  ParceiroResponse,
} from '@domain/entities/Parceiro'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarParceiroUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(
    token: string,
    payload: AtualizarParceiroPayload
  ): Promise<ParceiroResponse> {
    return this.repository.atualizarParceiro(token, payload)
  }
}
