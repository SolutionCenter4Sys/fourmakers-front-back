import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  InserirContratoPayload,
  ContratoResponse,
} from '@domain/entities/Contrato'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirContratoUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(
    token: string,
    payload: InserirContratoPayload
  ): Promise<ContratoResponse> {
    return this.repository.inserirContrato(token, payload)
  }
}
