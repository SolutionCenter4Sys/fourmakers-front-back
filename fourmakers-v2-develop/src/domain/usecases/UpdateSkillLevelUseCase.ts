import { inject, injectable } from 'tsyringe'

import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class UpdateSkillLevelUseCase {
  constructor(
    @inject(DiTokens.minhaJornadaRepository)
    private readonly repository: MinhaJornadaRepository,
  ) {}

  async execute(params: {
    id: number
    nivelId: number
    cpf: string
    tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
    gestorExternoPerfil: string
    minhaJornada: boolean
    token: string
  }): Promise<void> {
    const { id, nivelId, cpf, tipo, gestorExternoPerfil, minhaJornada, token } = params

    if ((id === undefined || id === null) || (nivelId === undefined || nivelId === null) || !cpf) {
      throw new Error('Parâmetros obrigatórios não fornecidos')
    }

    await this.repository.atualizarNivelCompetencia(
      { id, nivelId, cpf, tipo, gestorExternoPerfil, minhaJornada },
      token,
    )
  }
}

