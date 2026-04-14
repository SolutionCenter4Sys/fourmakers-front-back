import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  BuscarParceirosParams,
  BuscarParceirosResponse,
} from '@domain/entities/Parceiro'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarTodosParceirosUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(
    token: string,
    params: BuscarParceirosParams
  ): Promise<BuscarParceirosResponse> {
    return this.repository.buscarTodosParceiros(token, params)
  }
}
