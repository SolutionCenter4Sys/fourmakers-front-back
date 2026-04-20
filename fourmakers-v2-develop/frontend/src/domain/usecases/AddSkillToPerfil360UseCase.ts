import { inject, injectable } from 'tsyringe'

import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class AddSkillToPerfil360UseCase {
  constructor(
    @inject(DiTokens.minhaJornadaRepository)
    private readonly repository: MinhaJornadaRepository,
  ) {}

  async execute(params: {
    tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
    items: Array<{
      id: number
      descricao: string
      nivelId: number
      cpf: string
      gestorExternoPerfil: string
      minhaJornada: boolean
    }>
    token: string
  }): Promise<void> {
    const { tipo, items, token } = params

    if (!items || items.length === 0) {
      throw new Error('Lista de skills não pode estar vazia')
    }

    // Validar que todos os items têm campos obrigatórios (gestorExternoPerfil pode ser string vazia no portal público)
    for (const item of items) {
      if (item.id === undefined || item.id === null) {
        throw new Error('Todos os items devem ter id preenchido')
      }
      if (!item.descricao || typeof item.descricao !== 'string') {
        throw new Error('Todos os items devem ter descricao preenchida')
      }
      if (item.nivelId === undefined || item.nivelId === null) {
        throw new Error('Todos os items devem ter nivelId preenchido')
      }
      if (!item.cpf || typeof item.cpf !== 'string') {
        throw new Error('Todos os items devem ter cpf preenchido')
      }
      if (item.gestorExternoPerfil === undefined || item.gestorExternoPerfil === null) {
        throw new Error('Todos os items devem ter gestorExternoPerfil (pode ser string vazia)')
      }
    }

    await this.repository.adicionarSkillPerfil360(
      { tipo, items },
      token,
    )
  }
}
