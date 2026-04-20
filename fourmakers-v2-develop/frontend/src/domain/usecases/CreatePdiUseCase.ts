import { inject, injectable } from 'tsyringe'

import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'
import type { CriarMinhaJornadaPdiPayload } from '@domain/entities/MinhaJornadaGoal'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class CreatePdiUseCase {
  constructor(
    @inject(DiTokens.minhaJornadaRepository)
    private readonly repository: MinhaJornadaRepository,
  ) {}

  async execute(payload: CriarMinhaJornadaPdiPayload): Promise<void> {
    console.debug('DEBUG CreatePdiUseCase.execute called with payload:', payload)
    // Validações básicas
    if (!payload.metas || payload.metas.length === 0) {
      throw new Error('Pelo menos uma meta é obrigatória para criar o PDI')
    }

    // Validar que todas as metas têm prazo futuro
    const hoje = new Date()
    hoje.setHours(0, 0, 0, 0)

    for (const meta of payload.metas) {
      if (!meta.prazo) {
        throw new Error('Todas as metas devem ter um prazo definido')
      }

      const prazoDate = new Date(meta.prazo)
      prazoDate.setHours(0, 0, 0, 0)

      if (prazoDate <= hoje) {
        throw new Error(
          'O prazo das metas deve ser uma data futura',
        )
      }

      if (!meta.skills || meta.skills.length === 0) {
        throw new Error('Cada meta deve ter pelo menos uma skill associada')
      }
    }

    if (!payload.uuid_colab || (payload.id_colab === undefined || payload.id_colab === null)) {
      throw new Error('ID do colaborador é obrigatório')
    }

    if (payload.org_id === undefined || payload.org_id === null) {
      throw new Error('ID da organização é obrigatório')
    }

    await this.repository.criarPdi(payload)
  }
}

