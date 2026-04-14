// Valores possíveis que vêm da API
export type SkillLogEvent = 
  | 'Inserir' 
  | 'Sugerir' 
  | 'Adicionar ao PDI' 
  | 'Rejeitar'
  | 'SUGERIDO'
  | 'SUGERIDA'
  | 'ADICIONADO_PERFIL'
  | 'ADICIONADO_PDI'
  | 'ATUALIZADO'
  | 'INTERESSADO'
  | 'NAO_INTERESSADO'
  | 'REJEITADO'
  | string // Permitir outros valores como fallback

export interface SkillLogEntry {
  id: number
  nome: string
  cliente: string
  perfil: string
  skill: string
  senioridade: string
  tipoSkill: string
  unidade: string
  evento: SkillLogEvent
  data: string // YYYY-MM-DD
}

