import { inject, injectable } from 'tsyringe'

import type {
  SkillsRepository,
  SkillOption,
  NivelOption,
} from '@domain/repositories/SkillsRepository'
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import {
  mapSugestaoHistoryFromApi,
  type SugestaoHistoryItem,
} from '@data/mappers/sugestaoMappers'
import { SkillsApi } from '@data/api/SkillsApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class SkillsRepositoryImpl implements SkillsRepository {
  constructor(
    @inject(DiTokens.skillsApi)
    private readonly skillsApi: SkillsApi
  ) {}

  async searchByType(
    skillType: MinhaJornadaSkillType,
    searchTerm: string,
    token: string
  ): Promise<SkillOption[]> {
    const results = await this.skillsApi.searchSkillsByType(
      token,
      skillType,
      searchTerm
    )

    return results.map((item) => ({
      id: item.id,
      nome: item.nome,
      descricao: item.descricao,
    }))
  }

  async getNiveisByType(
    skillType: MinhaJornadaSkillType,
    token: string
  ): Promise<NivelOption[]> {
    return this.skillsApi.getNiveisByType(token, skillType)
  }

  async getSuggestionHistory(
    params: {
      codInternoGestor: string
      perfilId: string
      codInternoColaborador: string
      skillType: MinhaJornadaSkillType
    },
    token: string
  ): Promise<SugestaoHistoryItem[]> {
    const response = await this.skillsApi.getSuggestionHistory(token, {
      codInternoGestor: params.codInternoGestor,
      perfilId: params.perfilId,
      codInternoColaborador: params.codInternoColaborador,
    })

    if (!response.sucesso || !response.retorno) {
      return []
    }

    return mapSugestaoHistoryFromApi(response.retorno, params.skillType)
  }
}

