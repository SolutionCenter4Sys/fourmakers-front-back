import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  AtualizarContratoPayload,
  ContratoResponse,
} from '@domain/entities/Contrato'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarContratoUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(
    token: string,
    payload: AtualizarContratoPayload
  ): Promise<ContratoResponse> {
    return this.repository.atualizarContrato(token, payload)
  }
}
