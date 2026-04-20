import { inject, injectable } from 'tsyringe'

import type { SkillsRepository } from '@domain/repositories/SkillsRepository'
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import type { SugestaoHistoryItem } from '@data/mappers/sugestaoMappers'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetSuggestionHistoryUseCase {
  constructor(
    @inject(DiTokens.skillsRepository)
    private readonly repository: SkillsRepository
  ) {}

  async execute(params: {
    codInternoGestor: string
    perfilId: string
    codInternoColaborador: string
    skillType: MinhaJornadaSkillType
    token: string
  }): Promise<SugestaoHistoryItem[]> {
    const { token, ...historyParams } = params

    // Validate required parameters
    if (!historyParams.perfilId) {
      return []
    }

    if (!historyParams.codInternoColaborador) {
      return []
    }

    return this.repository.getSuggestionHistory(historyParams, token)
  }
}

