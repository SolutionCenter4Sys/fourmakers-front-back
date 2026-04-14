import { inject, injectable } from 'tsyringe'
import type { ParametrizacaoNotificacaoCandidatosRepository } from '@domain/repositories/ParametrizacaoNotificacaoCandidatosRepository'
import type {
  AtualizarParametrizacaoNotificacaoCandidatosPayload,
  AtualizarParametrizacaoNotificacaoCandidatosResponse,
} from '@domain/entities/ParametrizacaoNotificacaoCandidatos'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarParametrizacaoNotificacaoCandidatosUseCase {
  constructor(
    @inject(DiTokens.parametrizacaoNotificacaoCandidatosRepository)
    private readonly repository: ParametrizacaoNotificacaoCandidatosRepository,
  ) {}

  async execute(
    token: string,
    payload: AtualizarParametrizacaoNotificacaoCandidatosPayload,
  ): Promise<AtualizarParametrizacaoNotificacaoCandidatosResponse> {
    return this.repository.atualizarParametrizacao(token, payload)
  }
}
