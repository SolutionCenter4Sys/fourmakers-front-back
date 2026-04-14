import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type { PosicaoCompletaResponse } from '@domain/entities/Organograma'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarPosicoesUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(token: string, codigoCliente: string, orgId: number): Promise<PosicaoCompletaResponse> {
    return this.repository.listarOrganogramaCompleto(token, codigoCliente, orgId)
  }
}
