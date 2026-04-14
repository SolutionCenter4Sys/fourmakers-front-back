import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { ObterMeusColaboradoresResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterMeusColaboradoresUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository,
  ) {}

  async execute(token: string): Promise<ObterMeusColaboradoresResponse> {
    return this.repository.obterMeusColaboradores(token)
  }
}
