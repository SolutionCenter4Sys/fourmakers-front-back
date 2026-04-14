import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type { DepartamentoPayload, DepartamentoResponse } from '@domain/entities/Organograma'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarDepartamentoUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(token: string, payload: DepartamentoPayload): Promise<DepartamentoResponse> {
    return this.repository.criarDepartamento(token, payload)
  }
}
