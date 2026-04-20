import { inject, injectable } from 'tsyringe'
import { DiTokens } from '@core/di/tokens'
import type { CompetenciaRepository } from '@domain/repositories/CompetenciaRepository'
import type { ListarSkillsSumarioParams, ListarSkillsSumarioResponse } from '@domain/entities/Competencia'

@injectable()
export class ListarSkillsSumarioUseCase {
  constructor(
    @inject(DiTokens.competenciaRepository)
    private readonly repository: CompetenciaRepository
  ) {}

  async execute(
    token: string,
    params?: ListarSkillsSumarioParams
  ): Promise<ListarSkillsSumarioResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório')
    }

    return this.repository.listarSkillsSumario(token, params)
  }
}
