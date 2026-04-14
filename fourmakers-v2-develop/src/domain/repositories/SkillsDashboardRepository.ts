import type { SkillLogEntry } from '@domain/entities/SkillLogEntry'

export interface SkillsDashboardRepository {
  getSkillLogs(token: string, orgId: number, limit?: number, cursor?: number): Promise<SkillLogEntry[]>
  getTotalUsers(token: string, orgId: number): Promise<number>
  getBigNumbers(token: string, orgId: number): Promise<{
    skillsAdicionadas: number
    skillsSugeridas: number
    adicionadasPDI: number
    skillsRejeitadas: number
  }>
  getTopDez(token: string, orgId: number): Promise<any>
}

