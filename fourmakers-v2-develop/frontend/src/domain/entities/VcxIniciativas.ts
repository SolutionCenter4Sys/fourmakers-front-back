// Entidades para operações CRUD de Iniciativas VCX

/**
 * Reutiliza o tipo genérico ApiGenericResult<T> de VcxDores.ts
 * Importar de VcxDores para manter consistência
 */
import type { ApiGenericResult } from '@domain/entities/VcxDores'

/**
 * Payload para criar ou atualizar uma Iniciativa
 */
export interface IniciativaPayload {
  organogramaPosicaoId: string
  titulo: string
  descricao?: string | null
  vcxStatusId?: string | null
  vcxTemasId?: string | null
}

/**
 * Resposta da API com dados completos de uma Iniciativa
 */
export interface IniciativaResponse {
  id: string
  organogramaPosicaoId: string
  titulo: string
  descricao: string | null
  dataCriacao: string
  dataAlteracao: string
  vcxStatusId: string | null
  vcxTemasId: string | null
  vcxStatusDescricao: string | null
  vcxTemasDescricao: string | null
}

/**
 * Resposta da API para opções de Status de Iniciativas
 * Usado para popular dropdown de Status
 */
export interface StatusIniciativaResponse {
  id: string
  descricao: string
}

/**
 * Resposta da API para Temas
 * Usado para autocomplete de Temas
 */
export interface TemaResponse {
  id: string
  descricao: string
  orgId: number
}

/**
 * Payload para criar um novo Tema
 */
export interface TemaPayload {
  descricao: string
}

/**
 * Exporta o tipo ApiGenericResult para uso nas APIs
 */
export type { ApiGenericResult }
