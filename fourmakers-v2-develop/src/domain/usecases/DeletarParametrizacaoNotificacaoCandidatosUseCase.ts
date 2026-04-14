import { inject, injectable } from 'tsyringe'
import type { ParametrizacaoNotificacaoCandidatosRepository } from '@domain/repositories/ParametrizacaoNotificacaoCandidatosRepository'
import type { DeletarParametrizacaoNotificacaoCandidatosResponse } from '@domain/entities/ParametrizacaoNotificacaoCandidatos'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class DeletarParametrizacaoNotificacaoCandidatosUseCase {
  constructor(
    @inject(DiTokens.parametrizacaoNotificacaoCandidatosRepository)
    private readonly repository: ParametrizacaoNotificacaoCandidatosRepository,
  ) {}

  async execute(
    token: string,
    orgId: number,
  ): Promise<DeletarParametrizacaoNotificacaoCandidatosResponse> {
    return this.repository.deletarParametrizacao(token, orgId)
  }
}
