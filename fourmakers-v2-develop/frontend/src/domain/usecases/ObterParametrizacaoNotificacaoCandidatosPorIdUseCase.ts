import { inject, injectable } from 'tsyringe'
import type { ParametrizacaoNotificacaoCandidatosRepository } from '@domain/repositories/ParametrizacaoNotificacaoCandidatosRepository'
import type { ObterParametrizacaoNotificacaoCandidatosPorIdResponse } from '@domain/entities/ParametrizacaoNotificacaoCandidatos'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterParametrizacaoNotificacaoCandidatosPorIdUseCase {
  constructor(
    @inject(DiTokens.parametrizacaoNotificacaoCandidatosRepository)
    private readonly repository: ParametrizacaoNotificacaoCandidatosRepository,
  ) {}

  async execute(
    token: string,
    orgId: number,
  ): Promise<ObterParametrizacaoNotificacaoCandidatosPorIdResponse> {
    return this.repository.obterParametrizacaoPorId(token, orgId)
  }
}
