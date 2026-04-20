import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type { PosicaoPayload, PosicaoResponse } from '@domain/entities/Organograma'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarPosicaoUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(token: string, payload: PosicaoPayload & { id: string }): Promise<PosicaoResponse> {
    return this.repository.atualizarPosicao(token, payload)
  }
}
