import { inject, injectable } from 'tsyringe'

import type { SkillsDashboardRepository } from '@domain/repositories/SkillsDashboardRepository'
import type { SkillLogEntry } from '@domain/entities/SkillLogEntry'

import { DiTokens } from '@core/di/tokens'

export interface SkillLogsPaginatedResult {
  logs: SkillLogEntry[]
  hasMore: boolean
}

@injectable()
export class GetSkillLogsPaginatedUseCase {
  constructor(
    @inject(DiTokens.skillsDashboardRepository)
    private readonly repository: SkillsDashboardRepository,
  ) {}

  async execute(
    token: string,
    orgId: number,
    limit: number = 10,
    cursor: number = 0,
  ): Promise<SkillLogsPaginatedResult> {
    // Validações básicas
    if (!token || !orgId || orgId <= 0) {
      throw new Error('Token e orgId são obrigatórios')
    }

    if (limit <= 0) {
      throw new Error('Limit deve ser maior que zero')
    }

    if (cursor < 0) {
      throw new Error('Cursor deve ser maior ou igual a zero')
    }

    // Buscar logs do repositório
    // O repositório já retorna um array (a API trata respostas vazias como array vazio)
    const logs = await this.repository.getSkillLogs(token, orgId, limit, cursor)
    
    // Garantir que logs seja sempre um array (defesa em profundidade)
    const safeLogs = Array.isArray(logs) ? logs : []
    
    // Determinar se há mais dados baseado no tamanho retornado
    // Se retornou exatamente o limit, provavelmente há mais dados
    const hasMore = safeLogs.length === limit
    
    return {
      logs: safeLogs,
      hasMore,
    }
  }
}
