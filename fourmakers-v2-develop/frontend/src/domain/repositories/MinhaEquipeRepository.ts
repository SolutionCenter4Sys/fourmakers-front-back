import type { MinhaEquipeColaborador } from '@domain/entities/MinhaEquipeColaborador'
import type { MinhaEquipeKPIs } from '@domain/entities/MinhaEquipeKPIs'
import type { MinhaEquipeSugestao } from '@domain/entities/MinhaEquipeSugestao'
import type { MinhaEquipePDI } from '@domain/entities/MinhaEquipePDI'
import type { AprovarRejeitarSugestaoPayload } from '@domain/entities/MinhaEquipeSugestao'

/**
 * Contrato de acesso a dados para Minha Equipe e Orquestração
 */
export interface MinhaEquipeRepository {
  /**
   * Lista colaboradores com seus indicadores de desenvolvimento
   */
  listarIndicadoresLiderados(params: {
    token: string
    codGestorAdm: string
    orgId: number
    codGestorOper: string
    cursor: number
    limite: number
  }): Promise<MinhaEquipeColaborador[]>

  /**
   * Busca totalização de indicadores (KPIs/Big Numbers)
   */
  buscarTotalizacaoIndicadores(params: {
    token: string
    codGestorAdm: string
    orgId: number
    codGestorOper: string
  }): Promise<MinhaEquipeKPIs>

  /**
   * Busca metas de PDI de um colaborador específico
   */
  buscarPDIColaborador(params: {
    token: string
    uuid_colab: string
    uuid_manager: string
  }): Promise<MinhaEquipePDI[]>

  /**
   * Busca sugestões de habilidades de um colaborador
   */
  buscarSugestoesColaborador(params: {
    token: string
    codInternoGestor: string
    perfilId: string
    codInternoColaborador: string
  }): Promise<MinhaEquipeSugestao[]>

  /**
   * Aprova ou rejeita uma sugestão de habilidade
   */
  aprovarRejeitarSugestao(
    token: string,
    payload: AprovarRejeitarSugestaoPayload
  ): Promise<void>
}
