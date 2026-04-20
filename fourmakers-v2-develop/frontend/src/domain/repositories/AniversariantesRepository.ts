import type { AniversariantesSemanaResponse, AniversariantesSemanaParams } from '@domain/entities/Aniversariante'

export interface AniversariantesRepository {
  listarAniversariantesSemana(
    token: string,
    params?: AniversariantesSemanaParams
  ): Promise<AniversariantesSemanaResponse>
}

