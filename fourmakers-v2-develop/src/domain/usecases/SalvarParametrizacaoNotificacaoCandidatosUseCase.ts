import { inject, injectable } from 'tsyringe'
import type { ParametrizacaoNotificacaoCandidatosRepository } from '@domain/repositories/ParametrizacaoNotificacaoCandidatosRepository'
import type {
  SalvarParametrizacaoNotificacaoCandidatosPayload,
  SalvarParametrizacaoNotificacaoCandidatosResponse,
} from '@domain/entities/ParametrizacaoNotificacaoCandidatos'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class SalvarParametrizacaoNotificacaoCandidatosUseCase {
  constructor(
    @inject(DiTokens.parametrizacaoNotificacaoCandidatosRepository)
    private readonly repository: ParametrizacaoNotificacaoCandidatosRepository,
  ) {}

  async execute(
    token: string,
    payload: SalvarParametrizacaoNotificacaoCandidatosPayload,
  ): Promise<SalvarParametrizacaoNotificacaoCandidatosResponse> {
    return this.repository.salvarParametrizacao(token, payload)
  }
}
