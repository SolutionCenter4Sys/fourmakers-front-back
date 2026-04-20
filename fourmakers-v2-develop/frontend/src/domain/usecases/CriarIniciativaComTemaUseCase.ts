import { inject, injectable } from 'tsyringe'

import type { IniciativaPayload, IniciativaResponse } from '@domain/entities/VcxIniciativas'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class CriarIniciativaComTemaUseCase {
  constructor(
    @inject(DiTokens.vcxRepository)
    private readonly repository: VcxRepository,
  ) {}

  async execute(
    token: string,
    payload: IniciativaPayload,
    temaDescricao: string,
  ): Promise<IniciativaResponse> {
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

    if (!temaDescricao || !temaDescricao.trim()) {
      throw new Error('O campo tema é obrigatório')
    }

    // Validação de tamanho máximo
    if (payload.titulo.length > 255) {
      throw new Error('O título deve ter no máximo 255 caracteres')
    }

    if (payload.descricao && payload.descricao.length > 65535) {
      throw new Error('A descrição deve ter no máximo 65535 caracteres')
    }

    // Criar ou encontrar tema
    const temas = await this.repository.listarTemas(token)
    let temaId = temas.find(
      (t) => t.descricao.toLowerCase() === temaDescricao.toLowerCase(),
    )?.id

    if (!temaId) {
      const novoTema = await this.repository.criarTema(token, {
        descricao: temaDescricao,
      })
      temaId = novoTema.id
    }

    // Atualizar payload com o ID do tema
    const payloadCompleto = {
      ...payload,
      vcxTemasId: temaId,
    }

    return this.repository.criarIniciativa(token, payloadCompleto)
  }
}
