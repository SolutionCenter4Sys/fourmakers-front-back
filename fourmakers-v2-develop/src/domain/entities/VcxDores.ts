// Entidades para operações CRUD de Dores VCX

/**
 * Tipo genérico para respostas da API VCX
 * Segue o padrão ApiGenericResult<T> documentado na API
 */
export interface ApiGenericResult<T> {
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
  retorno: T
}

/**
 * Payload para criar ou atualizar uma Dor
 */
export interface DorPayload {
  organogramaPosicaoId: string
  titulo: string
  descricao?: string | null
  vcxImpactosId?: string | null
  vcxUrgenciasId?: string | null
}

/**
 * Resposta da API com dados completos de uma Dor
 */
export interface DorResponse {
  id: string
  organogramaPosicaoId: string
  titulo: string
  descricao: string | null
  dataCriacao: string
  dataAlteracao: string
  vcxImpactosId: string | null
  vcxUrgenciasId: string | null
  vcxImpactosDescricao: string | null
  vcxUrgenciasDescricao: string | null
}

/**
 * Resposta da API para opções de Impacto
 * Usado para popular dropdown de Impacto
 */
export interface ImpactoResponse {
  id: string
  descricao: string
}

/**
 * Resposta da API para opções de Urgência
 * Usado para popular dropdown de Urgência
 */
export interface UrgenciaResponse {
  id: string
  descricao: string
}
