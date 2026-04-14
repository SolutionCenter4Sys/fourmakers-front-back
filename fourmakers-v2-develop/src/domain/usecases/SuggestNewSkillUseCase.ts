import { inject, injectable } from 'tsyringe'

import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'
import type { MinhaJornadaSugestao } from '@domain/entities/MinhaJornadaSugestao'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class SuggestNewSkillUseCase {
  constructor(
    @inject(DiTokens.minhaJornadaRepository)
    private readonly repository: MinhaJornadaRepository,
  ) {}

  async execute(params: {
    sugestao: MinhaJornadaSugestao
    token: string
  }): Promise<void> {
    const { sugestao, token } = params

    // Validações básicas
    if (!sugestao.codigoInternoColaborador) {
      throw new Error('ID do colaborador é obrigatório')
    }

    if (!sugestao.codigoGestorAdm) {
      throw new Error('ID do gestor administrativo é obrigatório')
    }

    if (!sugestao.perfilId) {
      throw new Error('ID do perfil é obrigatório')
    }

    if (sugestao.skillId === undefined || sugestao.skillId === null) {
      throw new Error('ID da skill é obrigatório')
    }

    if (sugestao.tipoId === undefined || sugestao.tipoId === null) {
      throw new Error('Tipo da skill é obrigatório')
    }

    if (sugestao.senioridadeId === undefined || sugestao.senioridadeId === null) {
      throw new Error('Nível de senioridade é obrigatório')
    }

    await this.repository.sugerirNovaSkill(sugestao, token)
  }
}

