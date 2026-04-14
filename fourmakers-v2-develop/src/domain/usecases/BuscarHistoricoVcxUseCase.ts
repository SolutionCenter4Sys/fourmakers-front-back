import { inject, injectable } from 'tsyringe'

import type { HistoricoDoresIniciativasResponse } from '@domain/entities/VcxHistorico'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarHistoricoVcxUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(
    token: string,
    posicaoId: string,
  ): Promise<HistoricoDoresIniciativasResponse> {
    return this.repository.listarHistoricoDoresIniciativasPorPosicaoId(
      token,
      posicaoId,
    )
  }
}
