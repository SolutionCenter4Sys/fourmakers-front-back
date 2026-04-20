import { inject, injectable } from 'tsyringe'
import { DiTokens } from '@core/di/tokens'
import type { CompetenciaRepository } from '@domain/repositories/CompetenciaRepository'
import type { 
  ListarTodasSkillsColaboradorParams,
  ListarTodasSkillsColaboradorResponse
} from '@domain/entities/Competencia'

@injectable()
export class ListarTodasSkillsColaboradorUseCase {
  constructor(
    @inject(DiTokens.competenciaRepository)
    private readonly repository: CompetenciaRepository
  ) {}

  async execute(
    token: string,
    params: ListarTodasSkillsColaboradorParams
  ): Promise<ListarTodasSkillsColaboradorResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório')
    }

    if (!params.codigoInternoColaborador) {
      throw new Error('codigoInternoColaborador é obrigatório')
    }

    return this.repository.listarTodasSkillsColaborador(token, params)
  }
}
