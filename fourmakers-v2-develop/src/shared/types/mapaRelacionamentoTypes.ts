import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import type { ClientesGestaoAlocadosResponse } from '@domain/entities/ClienteGestaoAlocados'

/**
 * Interface para resposta alternativa de clientes
 * Usado quando o endpoint alternativo retorna estrutura diferente
 */
export interface ResponseAlternativo extends ClientesGestaoAlocadosResponse {}

/**
 * Interface para cliente raw retornado pelo endpoint alternativo
 */
export interface ClienteRaw {
  codigoCliente?: string
  id?: string
  nomeCliente?: string
  qtdAlocados?: number
  qtdGestoresSemPerfil?: number
  qtdGestores?: number
}

/**
 * Estado do modal passado no payload
 */
export interface ModalState {
  departamento: {
    originalId?: string
    novoNome?: string
    foiCriado: boolean
    foiEditado: boolean
  }
  perfilAtuacao: {
    originalId?: string
    novaDescricao?: string
    foiCriado: boolean
    foiEditado: boolean
  }
  colaborador: {
    originalId?: string
    novoId?: string
    foiAlterado: boolean
  }
  posicao: {
    originalId?: string
    foiCriado: boolean
    foiEditado: boolean
  }
  alocacao: {
    originalId?: string
    originalColaboradorId?: string
    foiCriada: boolean
    foiEditada: boolean
  }
}

/**
 * Payload com estado do modal
 */
export interface PayloadComModalState extends NoMapaRelacionamento {
  modalState?: ModalState
}

/**
 * Payload com flag isGestor
 */
export interface PayloadComIsGestor extends NoMapaRelacionamento {
  isGestor?: boolean
}

/**
 * Payload completo com todas as propriedades dinâmicas
 */
export interface PayloadCompleto extends PayloadComModalState, PayloadComIsGestor {
  _paiPosicaoId?: string
  _paiId?: string
  _isCreatingNew?: boolean
}

/**
 * Interface para cliente retornado pela API Organograma
 * Pode não incluir todos os campos opcionais
 */
export interface ClienteOrganogramaResponse {
  id: string
  codigoCliente: string
  nomeCliente: string
  qtdAlocados?: number
  qtdGestoresSemPerfil?: number
  qtdGestores?: number
}
