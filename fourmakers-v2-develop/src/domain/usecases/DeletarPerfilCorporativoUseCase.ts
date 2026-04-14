import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DeletarPerfilCorporativoUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(token: string, perfilCorpId: string): Promise<void> {
    return this.repository.deletarPerfilCorporativo(token, perfilCorpId)
  }
}
