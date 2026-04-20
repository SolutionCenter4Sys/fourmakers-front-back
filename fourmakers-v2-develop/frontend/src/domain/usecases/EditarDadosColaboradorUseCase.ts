import { inject, injectable } from 'tsyringe'
import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type {
  EditarDadosColaboradorPayload,
  EditarDadosColaboradorResponse,
} from '@domain/entities/ColaboradorDadosPessoais'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class EditarDadosColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(
    token: string,
    payload: EditarDadosColaboradorPayload,
  ): Promise<EditarDadosColaboradorResponse> {
    return this.repository.editarDadosColaborador(token, payload)
  }
}

