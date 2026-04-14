export interface ParametrizacaoNotificacaoCandidatos {
  titulo: string
  descricao: string
  emailsCC: string[] | string
}

export interface ObterParametrizacaoNotificacaoCandidatosResponse {
  sucesso: boolean
  retorno?: ParametrizacaoNotificacaoCandidatos[] | null
  mensagem?: string
}

/** Objeto retornado no campo "retorno" da API CandidatoTemplateEmailListarPorId. */
export interface CandidatoTemplateEmailRetorno {
  id: number
  orgId: number
  emailsCC: string
  titulo: string | null
  descricao: string
  dataCriacao?: string
  dataAlteracao?: string
}

/**
 * Resposta da API CandidatoTemplateEmailListarPorId.
 * Padrão de retorno aplicado nas telas: { retorno, sucesso, mensagem, erros }.
 */
export interface ObterParametrizacaoNotificacaoCandidatosPorIdResponse {
  retorno: CandidatoTemplateEmailRetorno | null
  sucesso: boolean
  mensagem: string | null
  erros?: string[] | null
}

export interface SalvarParametrizacaoNotificacaoCandidatosPayload {
  orgId: number
  titulo: string
  descricao: string
  emailsCC: string
}

export interface SalvarParametrizacaoNotificacaoCandidatosResponse {
  sucesso: boolean
  retorno?: ParametrizacaoNotificacaoCandidatos | null
  mensagem?: string
}

export interface AtualizarParametrizacaoNotificacaoCandidatosPayload {
  orgId: number
  titulo: string
  descricao: string
  emailsCC: string
}

export interface AtualizarParametrizacaoNotificacaoCandidatosResponse {
  sucesso: boolean
  retorno?: ParametrizacaoNotificacaoCandidatos | null
  mensagem?: string
}

export interface DeletarParametrizacaoNotificacaoCandidatosResponse {
  sucesso: boolean
  mensagem?: string
}
