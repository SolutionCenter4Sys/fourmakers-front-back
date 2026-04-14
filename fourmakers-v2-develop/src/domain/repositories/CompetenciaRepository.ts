import type { 
  ListarSkillsSumarioParams, 
  ListarSkillsSumarioResponse,
  ListarTodasSkillsColaboradorParams,
  ListarTodasSkillsColaboradorResponse
} from '@domain/entities/Competencia'

export interface CompetenciaRepository {
  listarSkillsSumario(
    token: string,
    params?: ListarSkillsSumarioParams
  ): Promise<ListarSkillsSumarioResponse>

  listarTodasSkillsColaborador(
    token: string,
    params: ListarTodasSkillsColaboradorParams
  ): Promise<ListarTodasSkillsColaboradorResponse>
}
