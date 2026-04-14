import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type { PosicaoPayload, PosicaoResponse } from '@domain/entities/Organograma'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarPosicaoUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(token: string, payload: PosicaoPayload): Promise<PosicaoResponse> {
    return this.repository.criarPosicao(token, payload)
  }
}
