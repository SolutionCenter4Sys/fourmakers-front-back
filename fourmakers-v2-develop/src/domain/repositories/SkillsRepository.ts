import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import type { SugestaoHistoryItem } from '@data/mappers/sugestaoMappers'

/**
 * Skill option for selection
 */
export interface SkillOption {
  id: number
  nome: string
  descricao?: string
}

/**
 * Skill level option for selection
 */
export interface NivelOption {
  id: number
  descricao: string
}

/**
 * Repository interface for skill-related operations
 */
export interface SkillsRepository {
  /**
   * Search skills by type and search term
   */
  searchByType(
    skillType: MinhaJornadaSkillType,
    searchTerm: string,
    token: string
  ): Promise<SkillOption[]>

  /**
   * Get available levels for a skill type
   */
  getNiveisByType(
    skillType: MinhaJornadaSkillType,
    token: string
  ): Promise<NivelOption[]>

  /**
   * Get suggestion history for a collaborator
   */
  getSuggestionHistory(
    params: {
      codInternoGestor: string
      perfilId: string
      codInternoColaborador: string
      skillType: MinhaJornadaSkillType
    },
    token: string
  ): Promise<SugestaoHistoryItem[]>
}

