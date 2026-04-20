import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  DeletarContratoParams,
  DeletarContratoResponse,
} from '@domain/entities/Contrato'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DeletarContratoUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(
    token: string,
    params: DeletarContratoParams
  ): Promise<DeletarContratoResponse> {
    return this.repository.deletarContrato(token, params)
  }
}
