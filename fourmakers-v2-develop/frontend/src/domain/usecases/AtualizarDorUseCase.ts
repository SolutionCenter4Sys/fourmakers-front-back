import { inject, injectable } from 'tsyringe'

import type { DorPayload, DorResponse } from '@domain/entities/VcxDores'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AtualizarDorUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(
    token: string,
    payload: DorPayload & { id: string },
  ): Promise<DorResponse> {
    // Validação de campos obrigatórios
    if (!payload.id) {
      throw new Error('O campo id é obrigatório para atualização')
    }

    if (!payload.titulo || !payload.titulo.trim()) {
      throw new Error('O campo título é obrigatório')
    }

    if (!payload.organogramaPosicaoId) {
      throw new Error('O campo organogramaPosicaoId é obrigatório')
    }

    // Validação de tamanho máximo
    if (payload.titulo.length > 255) {
      throw new Error('O título deve ter no máximo 255 caracteres')
    }

    if (payload.descricao && payload.descricao.length > 65535) {
      throw new Error('A descrição deve ter no máximo 65535 caracteres')
    }

    return this.repository.atualizarDor(token, payload)
  }
}
