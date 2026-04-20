import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type { PerfilCorporativoPayload, PerfilCorporativoResponse } from '@domain/entities/Organograma'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarPerfilCorporativoUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(
    token: string,
    payload: PerfilCorporativoPayload,
  ): Promise<PerfilCorporativoResponse> {
    return this.repository.criarPerfilCorporativo(token, payload)
  }
}
