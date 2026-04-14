import { inject, injectable } from 'tsyringe'
import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { ObterDadosColaboradorResponse } from '@domain/entities/ColaboradorDadosPessoais'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterDadosColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, codigoInternoColaborador: string): Promise<ObterDadosColaboradorResponse> {
    return this.repository.obterDadosColaborador(token, codigoInternoColaborador)
  }
}

