import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type { RelatorioParceriaResponse } from '@domain/entities/RelatorioParceria'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class GerarRelatorioParceriaUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(token: string): Promise<RelatorioParceriaResponse> {
    return this.repository.gerarRelatorioParceria(token)
  }
}
