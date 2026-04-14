import { inject, injectable } from 'tsyringe'
import type {
  EncontrosBigNumbersRepository,
  ObterBigNumbersParams,
} from '@domain/repositories/EncontrosBigNumbersRepository'
import type { SerieGrafico } from '@shared/types/dashboardComercialTypes'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterBigNumbersCategoriaUseCase {
  constructor(
    @inject(DiTokens.encontrosBigNumbersRepository)
    private readonly repository: EncontrosBigNumbersRepository,
  ) {}

  async execute(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<SerieGrafico[]> {
    const itens = await this.repository.obterBigNumbersCategoria(
      token,
      params,
    )
    return itens.map((item) => ({
      name: item.categoriaDescricao,
      value: item.totalInteracoes,
    }))
  }
}
