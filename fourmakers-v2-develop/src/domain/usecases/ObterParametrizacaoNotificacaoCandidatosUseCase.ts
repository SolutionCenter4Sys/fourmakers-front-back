import { inject, injectable } from 'tsyringe'
import type { ParametrizacaoNotificacaoCandidatosRepository } from '@domain/repositories/ParametrizacaoNotificacaoCandidatosRepository'
import type { ObterParametrizacaoNotificacaoCandidatosResponse } from '@domain/entities/ParametrizacaoNotificacaoCandidatos'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterParametrizacaoNotificacaoCandidatosUseCase {
  constructor(
    @inject(DiTokens.parametrizacaoNotificacaoCandidatosRepository)
    private readonly repository: ParametrizacaoNotificacaoCandidatosRepository,
  ) {}

  async execute(
    token: string,
    orgId: number,
  ): Promise<ObterParametrizacaoNotificacaoCandidatosResponse> {
    return this.repository.obterParametrizacao(token, orgId)
  }
}
