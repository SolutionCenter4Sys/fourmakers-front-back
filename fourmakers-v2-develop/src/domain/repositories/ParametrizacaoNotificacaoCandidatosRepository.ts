import type {
  AtualizarParametrizacaoNotificacaoCandidatosPayload,
  AtualizarParametrizacaoNotificacaoCandidatosResponse,
  DeletarParametrizacaoNotificacaoCandidatosResponse,
  ObterParametrizacaoNotificacaoCandidatosResponse,
  ObterParametrizacaoNotificacaoCandidatosPorIdResponse,
  SalvarParametrizacaoNotificacaoCandidatosPayload,
  SalvarParametrizacaoNotificacaoCandidatosResponse,
} from '@domain/entities/ParametrizacaoNotificacaoCandidatos'

export interface ParametrizacaoNotificacaoCandidatosRepository {
  obterParametrizacao(
    token: string,
    orgId: number,
  ): Promise<ObterParametrizacaoNotificacaoCandidatosResponse>
  obterParametrizacaoPorId(
    token: string,
    orgId: number,
  ): Promise<ObterParametrizacaoNotificacaoCandidatosPorIdResponse>
  salvarParametrizacao(
    token: string,
    payload: SalvarParametrizacaoNotificacaoCandidatosPayload,
  ): Promise<SalvarParametrizacaoNotificacaoCandidatosResponse>
  atualizarParametrizacao(
    token: string,
    payload: AtualizarParametrizacaoNotificacaoCandidatosPayload,
  ): Promise<AtualizarParametrizacaoNotificacaoCandidatosResponse>
  deletarParametrizacao(
    token: string,
    orgId: number,
  ): Promise<DeletarParametrizacaoNotificacaoCandidatosResponse>
}
