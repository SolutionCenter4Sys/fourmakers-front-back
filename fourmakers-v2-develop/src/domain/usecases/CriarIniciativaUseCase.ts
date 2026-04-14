import { inject, injectable } from 'tsyringe'

import type { IniciativaPayload, IniciativaResponse } from '@domain/entities/VcxIniciativas'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarIniciativaUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(token: string, payload: IniciativaPayload): Promise<IniciativaResponse> {
    // Validação de campos obrigatórios
    if (!payload.titulo || !payload.titulo.trim()) {
      throw new Error('O campo título é obrigatório')
    }

    if (!payload.organogramaPosicaoId) {
      throw new Error('O campo organogramaPosicaoId é obrigatório')
    }

    if (!payload.descricao || !payload.descricao.trim()) {
      throw new Error('O campo objetivo/KPI é obrigatório')
    }

    // Validação de tamanho máximo
    if (payload.titulo.length > 255) {
      throw new Error('O título deve ter no máximo 255 caracteres')
    }

    if (payload.descricao && payload.descricao.length > 65535) {
      throw new Error('A descrição deve ter no máximo 65535 caracteres')
    }

    return this.repository.criarIniciativa(token, payload)
  }
}
