// Interface para operações CRUD de Dores e Iniciativas VCX

import type {
  DorPayload,
  DorResponse,
  ImpactoResponse,
  UrgenciaResponse,
} from '@domain/entities/VcxDores'
import type {
  IniciativaPayload,
  IniciativaResponse,
  StatusIniciativaResponse,
  TemaResponse,
  TemaPayload,
} from '@domain/entities/VcxIniciativas'
import type { HistoricoDoresIniciativasResponse } from '@domain/entities/VcxHistorico'
import type { VcxAgendasPorColaboradorClienteResult } from '@domain/entities/VcxAgenda'

/**
 * Interface do repositório para operações de Dores e Iniciativas VCX
 * Define o contrato que deve ser implementado pela camada de dados
 */
export interface VcxRepository {
  /**
   * Lista todas as dores de uma posição do organograma
   * @param token - Token de autenticação
   * @param posicaoId - ID da posição (GUID)
   * @returns Lista de dores da posição
   */
  listarDoresPorPosicaoId(token: string, posicaoId: string): Promise<DorResponse[]>

  /**
   * Busca uma dor específica por ID
   * @param token - Token de autenticação
   * @param id - ID da dor (GUID)
   * @returns Dados completos da dor
   */
  buscarDorPorId(token: string, id: string): Promise<DorResponse>

  /**
   * Cria uma nova dor
   * @param token - Token de autenticação
   * @param payload - Dados da dor a ser criada
   * @returns Dados da dor criada
   */
  criarDor(token: string, payload: DorPayload): Promise<DorResponse>

  /**
   * Atualiza uma dor existente
   * @param token - Token de autenticação
   * @param payload - Dados da dor a ser atualizada (deve incluir id)
   * @returns Dados da dor atualizada
   */
  atualizarDor(token: string, payload: DorPayload & { id: string }): Promise<DorResponse>

  /**
   * Exclui uma dor
   * @param token - Token de autenticação
   * @param id - ID da dor a ser excluída (GUID)
   */
  excluirDor(token: string, id: string): Promise<void>

  /**
   * Lista todas as opções de Impacto disponíveis
   * @param token - Token de autenticação
   * @returns Lista de impactos (Alto, Médio, Baixo)
   */
  listarImpactosDores(token: string): Promise<ImpactoResponse[]>

  /**
   * Lista todas as opções de Urgência disponíveis
   * @param token - Token de autenticação
   * @returns Lista de urgências (Urgente, Normal, Baixa)
   */
  listarUrgenciasDores(token: string): Promise<UrgenciaResponse[]>

  // ============================================================================
  // MÉTODOS DE INICIATIVAS
  // ============================================================================

  /**
   * Lista todas as iniciativas de uma posição do organograma
   * @param token - Token de autenticação
   * @param posicaoId - ID da posição (GUID)
   * @returns Lista de iniciativas da posição
   */
  listarIniciativasPorPosicaoId(
    token: string,
    posicaoId: string,
  ): Promise<IniciativaResponse[]>

  /**
   * Busca uma iniciativa específica por ID
   * @param token - Token de autenticação
   * @param id - ID da iniciativa (GUID)
   * @returns Dados completos da iniciativa
   */
  buscarIniciativaPorId(token: string, id: string): Promise<IniciativaResponse>

  /**
   * Cria uma nova iniciativa
   * @param token - Token de autenticação
   * @param payload - Dados da iniciativa a ser criada
   * @returns Dados da iniciativa criada
   */
  criarIniciativa(
    token: string,
    payload: IniciativaPayload,
  ): Promise<IniciativaResponse>

  /**
   * Atualiza uma iniciativa existente
   * @param token - Token de autenticação
   * @param payload - Dados da iniciativa a ser atualizada (deve incluir id)
   * @returns Dados da iniciativa atualizada
   */
  atualizarIniciativa(
    token: string,
    payload: IniciativaPayload & { id: string },
  ): Promise<IniciativaResponse>

  /**
   * Exclui uma iniciativa
   * @param token - Token de autenticação
   * @param id - ID da iniciativa a ser excluída (GUID)
   */
  excluirIniciativa(token: string, id: string): Promise<void>

  /**
   * Lista todas as opções de Status de Iniciativas disponíveis
   * @param token - Token de autenticação
   * @returns Lista de status (Em Planejamento, Ativa, Pausada, Concluída)
   */
  listarStatusIniciativas(
    token: string,
  ): Promise<StatusIniciativaResponse[]>

  /**
   * Lista todos os temas disponíveis (para autocomplete)
   * @param token - Token de autenticação
   * @returns Lista de temas filtrados por organização do usuário
   */
  listarTemas(token: string): Promise<TemaResponse[]>

  /**
   * Cria um novo tema
   * @param token - Token de autenticação
   * @param payload - Dados do tema a ser criado
   * @returns Dados do tema criado
   */
  criarTema(token: string, payload: TemaPayload): Promise<TemaResponse>

  // ============================================================================
  // MÉTODOS DE HISTÓRICO
  // ============================================================================

  /**
   * Lista histórico de alterações de Dores e Iniciativas por posição
   * @param token - Token de autenticação
   * @param posicaoId - ID da posição (GUID)
   * @returns Histórico completo com logs de Dores e Iniciativas
   */
  listarHistoricoDoresIniciativasPorPosicaoId(
    token: string,
    posicaoId: string,
  ): Promise<HistoricoDoresIniciativasResponse>

  // ============================================================================
  // MÉTODOS DE AGENDA
  // ============================================================================

  /**
   * Lista agendas (reuniões) por colaborador e cliente com paginação por cursor
   * @param token - Token de autenticação
   * @param codigoColaborador - Código do colaborador
   * @param codigoCliente - Código do cliente
   * @param limit - Quantidade por página (default 20)
   * @param cursor - Offset para paginação (default 0)
   * @returns agendasAntigas e agendasNovas
   */
  listarAgendasPorColaboradorCliente(
    token: string,
    codigoColaborador: string,
    codigoCliente: string,
    limit?: number,
    cursor?: number,
  ): Promise<VcxAgendasPorColaboradorClienteResult>
}
