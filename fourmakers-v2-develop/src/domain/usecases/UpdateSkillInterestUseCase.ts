import { inject, injectable } from 'tsyringe'

import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'
import type { MinhaJornadaInteresse } from '@domain/entities/MinhaJornadaInteresse'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class UpdateSkillInterestUseCase {
  constructor(
    @inject(DiTokens.minhaJornadaRepository)
    private readonly repository: MinhaJornadaRepository,
  ) {}

  async execute(params: {
    interesses: MinhaJornadaInteresse[]
    token: string
  }): Promise<void> {
    const { interesses, token } = params

    if (!interesses || interesses.length === 0) {
      throw new Error('Lista de interesses não pode estar vazia')
    }

    await this.repository.registrarInteresseSkill(interesses, token)
  }
}

