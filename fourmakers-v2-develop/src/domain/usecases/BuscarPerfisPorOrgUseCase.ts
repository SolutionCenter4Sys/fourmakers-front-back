import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type { PerfilCorporativoResponse } from '@domain/entities/Organograma'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarPerfisPorOrgUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(
    token: string,
    orgId: number,
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[]; retorno: PerfilCorporativoResponse[] }> {
    return this.repository.buscarPerfisPorOrg(token, orgId)
  }
}
