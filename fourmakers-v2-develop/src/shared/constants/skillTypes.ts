import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'

/**
 * Maps frontend skill types to API tipoSkill integer values
 */
export const TIPO_SKILL_MAP: Record<MinhaJornadaSkillType, number> = {
  hard: 1,
  soft: 8,
  methodology: 3,
  domain: 4,
  language: 9,
}

/**
 * Human-readable labels in Portuguese for each skill type
 */
export const SKILL_TYPE_LABELS: Record<MinhaJornadaSkillType, string> = {
  hard: 'Hardskill',
  soft: 'Softskill',
  methodology: 'Metodologia',
  domain: 'Domínio de Negócio',
  language: 'Idioma',
}


/**
 * Maps API tipoSkill integer values to human-readable labels
 */
export const TIPO_SKILL_REVERSE_MAP: Record<number, string> = {
  1: 'Hardskill',
  3: 'Metodologia',
  4: 'Domínio de Negócio',
  8: 'Softskill',
  9: 'Idioma',
}

/**
 * Converts API tipoSkill integer to human-readable string
 */
export const mapTipoSkillFromInt = (tipoSkillInt: number | string | undefined): string => {
  if (tipoSkillInt === undefined || tipoSkillInt === null) return ''
  
  const intValue = typeof tipoSkillInt === 'string' ? parseInt(tipoSkillInt, 10) : tipoSkillInt
  
  if (isNaN(intValue)) return String(tipoSkillInt)
  
  return TIPO_SKILL_REVERSE_MAP[intValue] || String(tipoSkillInt)
}
