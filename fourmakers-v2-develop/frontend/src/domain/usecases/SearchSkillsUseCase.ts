import { inject, injectable } from 'tsyringe'

import type {
  SkillsRepository,
  SkillOption,
} from '@domain/repositories/SkillsRepository'
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import { DiTokens } from '@core/di/tokens'

const MIN_SEARCH_LENGTH = 3

@injectable()
export class SearchSkillsUseCase {
  constructor(
    @inject(DiTokens.skillsRepository)
    private readonly repository: SkillsRepository
  ) {}

  async execute(params: {
    skillType: MinhaJornadaSkillType
    searchTerm: string
    token: string
  }): Promise<SkillOption[]> {
    const { skillType, searchTerm, token } = params

    // Business rule: minimum 3 characters required for search
    if (searchTerm.trim().length < MIN_SEARCH_LENGTH) {
      return []
    }

    return this.repository.searchByType(skillType, searchTerm, token)
  }
}

