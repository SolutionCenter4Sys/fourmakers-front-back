import { inject, injectable } from 'tsyringe'
import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { ListarEscolaridadeColaboradorResponse, ListarEscolaridadeColaboradorParams } from '@domain/entities/Escolaridade'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarEscolaridadeColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, params: ListarEscolaridadeColaboradorParams): Promise<ListarEscolaridadeColaboradorResponse> {
    return this.repository.listarEscolaridadeColaborador(token, params)
  }
}
