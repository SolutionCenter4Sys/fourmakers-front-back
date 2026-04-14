import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DeletarDepartamentoUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(token: string, departamentoId: string): Promise<void> {
    return this.repository.deletarDepartamento(token, departamentoId)
  }
}
