// Entidades para histórico de alterações de Dores e Iniciativas VCX

/**
 * Item de log de uma Dor
 * Representa o estado de uma Dor em um momento específico
 */
export interface DorLogItem {
  id: string
  organogramaPosicaoId: string
  titulo: string
  descricao: string
  dataCriacao: string
  dataAlteracao: string
  vcxImpactosId: string
  vcxUrgenciasId: string
  vcxImpactosDescricao: string
  vcxUrgenciasDescricao: string
}

/**
 * Item de log de uma Iniciativa
 * Representa o estado de uma Iniciativa em um momento específico
 */
export interface IniciativaLogItem {
  id: string
  organogramaPosicaoId: string
  titulo: string
  descricao: string
  dataCriacao: string
  dataAlteracao: string
  vcxStatusId: string
  vcxTemasId: string
  vcxStatusDescricao: string
  vcxTemasDescricao: string
}

/**
 * Tipo de ação realizada em um log
 */
export type AcaoLog = 'INSERT' | 'UPDATE' | 'DELETE'

/**
 * Log de alteração de uma Dor
 * Contém informações sobre quem alterou, quando e o que foi alterado
 */
export interface DorLog {
  id: string
  colaboradorCodigoInternoColaboradorAlterador: string
  organogramaPosicaoId: string
  acao: AcaoLog
  objeto: DorLogItem | null // Estado anterior (para UPDATE/DELETE)
  alteracao: DorLogItem | null // Estado novo (para INSERT/UPDATE)
  dataAlteracao: string
}

/**
 * Log de alteração de uma Iniciativa
 * Contém informações sobre quem alterou, quando e o que foi alterado
 */
export interface IniciativaLog {
  id: string
  colaboradorCodigoInternoColaboradorAlterador: string
  organogramaPosicaoId: string
  acao: AcaoLog
  objeto: IniciativaLogItem | null // Estado anterior (para UPDATE/DELETE)
  alteracao: IniciativaLogItem | null // Estado novo (para INSERT/UPDATE)
  dataAlteracao: string
}

/**
 * Resposta da API com histórico completo de Dores e Iniciativas
 */
export interface HistoricoDoresIniciativasResponse {
  doresLog: DorLog[]
  iniciativasLog: IniciativaLog[]
}
