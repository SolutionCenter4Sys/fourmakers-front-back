import { inject, injectable } from 'tsyringe'

import type { SkillsDashboardRepository } from '@domain/repositories/SkillsDashboardRepository'
import type { SkillLogEntry } from '@domain/entities/SkillLogEntry'

import { DiTokens } from '@core/di/tokens'
import { SkillsDashboardApi } from '@data/api/SkillsDashboardApi'

@injectable()
export class SkillsDashboardRepositoryImpl implements SkillsDashboardRepository {
  constructor(
    @inject(DiTokens.skillsDashboardApi)
    private readonly api: SkillsDashboardApi,
  ) {}

  async getSkillLogs(token: string, orgId: number, limit: number = 10, cursor: number = 0): Promise<SkillLogEntry[]> {
    return this.api.getSkillLogs(token, orgId, limit, cursor)
  }

  async getTotalUsers(token: string, orgId: number): Promise<number> {
    return this.api.getTotalUsers(token, orgId)
  }

  async getBigNumbers(token: string, orgId: number): Promise<{
    skillsAdicionadas: number
    skillsSugeridas: number
    adicionadasPDI: number
    skillsRejeitadas: number
  }> {
    return this.api.getBigNumbers(token, orgId)
  }

  async getTopDez(token: string, orgId: number): Promise<any> {
    return this.api.getTopDez(token, orgId)
  }
}

