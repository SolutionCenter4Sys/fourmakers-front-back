import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type { AlocacaoPayload, AlocacaoResponse } from '@domain/entities/Organograma'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarAlocacaoUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(token: string, payload: AlocacaoPayload): Promise<AlocacaoResponse> {
    return this.repository.criarAlocacao(token, payload)
  }
}
