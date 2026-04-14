import { inject, injectable } from 'tsyringe'

import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class RemoveSkillFromPerfil360UseCase {
  constructor(
    @inject(DiTokens.minhaJornadaRepository)
    private readonly repository: MinhaJornadaRepository,
  ) {}

  async execute(params: {
    tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
    idOuCompetenciaId: number
    cpf: string
    token: string
  }): Promise<void> {
    const { tipo, idOuCompetenciaId, cpf, token } = params
    if (idOuCompetenciaId == null || typeof idOuCompetenciaId !== 'number') {
      throw new Error('idOuCompetenciaId é obrigatório')
    }
    if (!cpf || typeof cpf !== 'string') {
      throw new Error('cpf é obrigatório')
    }
    await this.repository.removerSkillPerfil360(
      { tipo, idOuCompetenciaId, cpf },
      token,
    )
  }
}
