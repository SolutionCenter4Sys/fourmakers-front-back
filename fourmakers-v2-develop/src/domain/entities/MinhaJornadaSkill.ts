export type MinhaJornadaSkillType =
  | 'hard'
  | 'soft'
  | 'methodology'
  | 'domain'
  | 'language'

export interface MinhaJornadaSkill {
  // Identificadores vindos do backend
  codigoInternoColaborador?: string
  codigoCliente?: string

  // Metadados de perfil/skill
  perfilTipoId: number
  tipoPerfil: string
  skillId: number
  habilidade: string
  senioridadeId?: number | null
  senioridade?: string | null
  interesse?: number

  // Campos derivados / normalizados (client-side)
  categoria?: 'hard' | 'soft' | 'methodology' | 'domain' | 'language'

  // Níveis de senioridade (exigido vs. atual)
  nivelExigido?: number | null
  nivelAtual?: number | null

  // Tier normalizado (1-5) calculado client-side
  senioridadeTierExigida?: number | null
  senioridadeTierAtual?: number | null

  // Flags de estado na UI
  ignorada?: boolean
  isInPdi?: boolean
}


