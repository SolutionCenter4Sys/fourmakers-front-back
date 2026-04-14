import type { Contrato } from './Contrato'

// Entidade principal para Parceiros/Empresas
export interface Parceiro {
  id: string // UUID
  nome: string
  avaliacao: number // 0-5
  logo: string | null
  unidade: string
  tipoEmpresa: TipoEmpresa
  website: string
  linkedIn: string
  descricaoLonga: string
  descricaoCurta: string
  tags: string[]
  contatos: ContatoParceiro[]
  contratos?: Contrato[] // Lista de contratos vinculados (pode vir do backend)
  orgId: number
  dataCriacao?: string
  dataAtualizacao?: string
  /** Nome do colaborador que criou ou editou o cadastro (editor) */
  nomeColaborador?: string
}

export type TipoEmpresa = 'Benefício' | 'Parceria' | 'Aliança' | 'Cliente' | 'Fornecedor'

export interface ContatoParceiro {
  nome: string
  email: string
  telefone: string
}

// Request/Response Types
export interface BuscarParceirosParams {
  orgId: number
  filtro?: string // "vencidos" | "proximos" | "indeterminado" | ""
  bucket?: string // "1-7" | "8-30" | "31-60" | "60" | ""
  cursor?: string | null
  limite?: number | null
}

export interface BuscarParceirosResponse {
  parceiros: Parceiro[]
  proximoCursor?: string | null
  total?: number
}

export interface InserirParceiroPayload {
  nome: string
  avaliacao: number
  logo?: string
  unidade: string
  tipoEmpresa: TipoEmpresa
  website: string
  linkedIn: string
  descricaoLonga: string
  descricaoCurta: string
  tags: string[]
  contatos: ContatoParceiro[]
  orgId: number
  codigoInternoColaborador?: string
}

export interface AtualizarParceiroPayload extends InserirParceiroPayload {
  id: string // UUID do parceiro
}

export interface ParceiroResponse {
  sucesso: boolean
  mensagem?: string
  dados?: Parceiro
  erros?: string[]
}

export interface DeletarParceiroParams {
  parceiroId: string
}

export interface DeletarParceiroResponse {
  sucesso: boolean
  mensagem?: string
  erros?: string[]
}
