import { inject, injectable } from 'tsyringe'

import type {
  AtualizarParametrizacaoNotificacaoCandidatosPayload,
  AtualizarParametrizacaoNotificacaoCandidatosResponse,
  DeletarParametrizacaoNotificacaoCandidatosResponse,
  ObterParametrizacaoNotificacaoCandidatosResponse,
  ObterParametrizacaoNotificacaoCandidatosPorIdResponse,
  SalvarParametrizacaoNotificacaoCandidatosPayload,
  SalvarParametrizacaoNotificacaoCandidatosResponse,
} from '@domain/entities/ParametrizacaoNotificacaoCandidatos'
import type { ParametrizacaoNotificacaoCandidatosRepository } from '@domain/repositories/ParametrizacaoNotificacaoCandidatosRepository'

import { ParametrizacaoNotificacaoCandidatosApi } from '@data/api/ParametrizacaoNotificacaoCandidatosApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ParametrizacaoNotificacaoCandidatosRepositoryImpl
  implements ParametrizacaoNotificacaoCandidatosRepository {
  constructor(
    @inject(DiTokens.parametrizacaoNotificacaoCandidatosApi)
    private readonly api: ParametrizacaoNotificacaoCandidatosApi,
  ) {}

  async obterParametrizacao(
    token: string,
    orgId: number,
  ): Promise<ObterParametrizacaoNotificacaoCandidatosResponse> {
    return this.api.obterParametrizacao(token, orgId)
  }

  async obterParametrizacaoPorId(
    token: string,
    orgId: number,
  ): Promise<ObterParametrizacaoNotificacaoCandidatosPorIdResponse> {
    return this.api.obterParametrizacaoPorId(token, orgId)
  }

  async salvarParametrizacao(
    token: string,
    payload: SalvarParametrizacaoNotificacaoCandidatosPayload,
  ): Promise<SalvarParametrizacaoNotificacaoCandidatosResponse> {
    return this.api.salvarParametrizacao(token, payload)
  }

  async atualizarParametrizacao(
    token: string,
    payload: AtualizarParametrizacaoNotificacaoCandidatosPayload,
  ): Promise<AtualizarParametrizacaoNotificacaoCandidatosResponse> {
    return this.api.atualizarParametrizacao(token, payload)
  }

  async deletarParametrizacao(
    token: string,
    orgId: number,
  ): Promise<DeletarParametrizacaoNotificacaoCandidatosResponse> {
    return this.api.deletarParametrizacao(token, orgId)
  }
}
