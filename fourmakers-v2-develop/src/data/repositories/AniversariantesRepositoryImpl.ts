import { inject, injectable } from 'tsyringe'

import type { AniversariantesRepository } from '@domain/repositories/AniversariantesRepository'
import type { AniversariantesSemanaResponse, AniversariantesSemanaParams } from '@domain/entities/Aniversariante'

import { ColaboradoresApi } from '@data/api/ColaboradoresApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class AniversariantesRepositoryImpl implements AniversariantesRepository {
  constructor(
    @inject(DiTokens.colaboradoresApi)
    private readonly api: ColaboradoresApi,
  ) {}

  async listarAniversariantesSemana(
    token: string,
    params?: AniversariantesSemanaParams
  ): Promise<AniversariantesSemanaResponse> {
    return this.api.aniversariantesSemana(token, params)
  }
}

