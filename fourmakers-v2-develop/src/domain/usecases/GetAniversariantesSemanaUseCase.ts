import { inject, injectable } from 'tsyringe'

import type { AniversariantesRepository } from '@domain/repositories/AniversariantesRepository'
import type { AniversariantesSemanaResponse, AniversariantesSemanaParams } from '@domain/entities/Aniversariante'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetAniversariantesSemanaUseCase {
  constructor(
    @inject(DiTokens.aniversariantesRepository)
    private readonly repository: AniversariantesRepository,
  ) {}

  async execute(
    token: string,
    params?: AniversariantesSemanaParams
  ): Promise<AniversariantesSemanaResponse> {
    return this.repository.listarAniversariantesSemana(token, params)
  }
}

