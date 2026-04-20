import { inject, injectable } from 'tsyringe'

import type {
  SkillsRepository,
  NivelOption,
} from '@domain/repositories/SkillsRepository'
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetSkillLevelsUseCase {
  constructor(
    @inject(DiTokens.skillsRepository)
    private readonly repository: SkillsRepository
  ) {}

  async execute(params: {
    skillType: MinhaJornadaSkillType
    token: string
  }): Promise<NivelOption[]> {
    const { skillType, token } = params

    return this.repository.getNiveisByType(skillType, token)
  }
}

