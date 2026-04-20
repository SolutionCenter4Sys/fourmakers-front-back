import { inject, injectable } from 'tsyringe'

import type { SkillsDashboardRepository } from '@domain/repositories/SkillsDashboardRepository'
import type { SkillLogEntry } from '@domain/entities/SkillLogEntry'
import type { BigNumbersData, TopDezData } from '@domain/types/SkillsDashboardTypes'

import { DiTokens } from '@core/di/tokens'

export interface SkillsDashboardData {
  logs: SkillLogEntry[]
  totalUsers: number
  bigNumbers?: BigNumbersData
  topDez?: TopDezData
}

@injectable()
export class GetSkillsDashboardDataUseCase {
  constructor(
    @inject(DiTokens.skillsDashboardRepository)
    private readonly repository: SkillsDashboardRepository,
  ) {}

  async execute(token: string, orgId: number): Promise<SkillsDashboardData> {
    const results = await Promise.all([
      this.repository.getSkillLogs(token, orgId, 10, 0), // Buscar primeira página apenas
      this.repository.getTotalUsers(token, orgId),
      this.repository.getBigNumbers(token, orgId),
      this.repository.getTopDez(token, orgId),
    ])

    const [logs, totalUsers, bigNumbers, topDez] = results

    return {
      logs,
      totalUsers,
      bigNumbers,
      topDez,
    }
  }
}

