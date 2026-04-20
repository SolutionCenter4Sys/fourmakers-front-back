import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  InserirParceiroPayload,
  ParceiroResponse,
} from '@domain/entities/Parceiro'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirParceiroUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(
    token: string,
    payload: InserirParceiroPayload
  ): Promise<ParceiroResponse> {
    return this.repository.inserirParceiro(token, payload)
  }
}
