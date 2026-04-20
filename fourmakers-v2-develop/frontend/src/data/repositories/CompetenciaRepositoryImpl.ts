import { inject, injectable } from 'tsyringe'
import { DiTokens } from '@core/di/tokens'
import type { CompetenciaRepository } from '@domain/repositories/CompetenciaRepository'
import type { 
  ListarSkillsSumarioParams, 
  ListarSkillsSumarioResponse,
  ListarTodasSkillsColaboradorParams,
  ListarTodasSkillsColaboradorResponse
} from '@domain/entities/Competencia'
import type { CompetenciasApi } from '@data/api/CompetenciasApi'

@injectable()
export class CompetenciaRepositoryImpl implements CompetenciaRepository {
  constructor(
    @inject(DiTokens.competenciasApi)
    private readonly competenciasApi: CompetenciasApi
  ) {}

  async listarSkillsSumario(
    token: string,
    params?: ListarSkillsSumarioParams
  ): Promise<ListarSkillsSumarioResponse> {
    return this.competenciasApi.listarSkillsSumario(token, params)
  }

  async listarTodasSkillsColaborador(
    token: string,
    params: ListarTodasSkillsColaboradorParams
  ): Promise<ListarTodasSkillsColaboradorResponse> {
    return this.competenciasApi.listarTodasSkillsColaborador(token, params)
  }
}
