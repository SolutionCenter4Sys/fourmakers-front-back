import { inject, injectable } from 'tsyringe'

import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'
import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import type { MinhaJornadaProfile } from '@domain/entities/MinhaJornadaProfile'
import type { MinhaJornadaAdherence } from '@domain/entities/MinhaJornadaAdherence'

import { DiTokens } from '@core/di/tokens'
import { calcularGaps } from '@shared/utils/minhaJornadaComparators'

export interface GetMinhaJornadaContextResult {
  perfil360: MinhaJornadaSkill[]
  perfilAtuacao: MinhaJornadaProfile | null
  perfisDisponiveis: MinhaJornadaProfile[]
  aderencia: MinhaJornadaAdherence | null
  gaps: MinhaJornadaSkill[]
}

@injectable()
export class GetMinhaJornadaContextUseCase {
  constructor(
    @inject(DiTokens.minhaJornadaRepository)
    private readonly repository: MinhaJornadaRepository,
  ) {}

  async execute(params: {
    codColaborador: string
    orgId: number
    token: string
  }): Promise<GetMinhaJornadaContextResult> {
    const { codColaborador, orgId, token } = params

    // Buscar dados em paralelo quando possível
    let perfil360: MinhaJornadaSkill[]
    let perfisDisponiveis: MinhaJornadaProfile[]
    
    try {
      const results = await Promise.all([
        this.repository.getPerfil360Skills(codColaborador, token),
        this.repository.getPerfisAtuacao(codColaborador, orgId, token).catch(() => []),
      ])
      
      perfil360 = results[0]
      perfisDisponiveis = results[1]
    } catch (err) {
      throw err
    }

    // Usar o primeiro perfil como padrão se disponível
    const perfilAtuacao = perfisDisponiveis.length > 0 ? perfisDisponiveis[0] : null

    // Se não houver perfil de atuação, retornar contexto básico
    if (!perfilAtuacao || !perfilAtuacao.perfilId) {
      return {
        perfil360,
        perfilAtuacao,
        perfisDisponiveis,
        aderencia: null,
        gaps: [],
      }
    }

    // Verificar se há habilidades no perfil antes de calcular aderência
    const hasHabilidades = perfilAtuacao.skills && perfilAtuacao.skills.length > 0

    // Buscar aderência apenas se houver perfilId válido E habilidades no perfil
    // Caso contrário, o endpoint retornará erro
    const aderencia = hasHabilidades
      ? await this.repository
          .getAdherence(codColaborador, perfilAtuacao.perfilId, token)
          .catch(() => null)
      : null

    // Calcular gaps comparando perfil de atuação com perfil 360
    // Só calcular se houver habilidades
    const gaps = hasHabilidades
      ? calcularGaps({
          requisitos: perfilAtuacao.skills,
          possuidas: perfil360,
        })
      : []

    return {
      perfil360,
      perfilAtuacao,
      perfisDisponiveis,
      aderencia,
      gaps,
    }
  }
}

