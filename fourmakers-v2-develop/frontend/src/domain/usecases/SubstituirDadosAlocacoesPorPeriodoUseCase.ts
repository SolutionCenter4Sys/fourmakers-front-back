import { inject, injectable } from 'tsyringe'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { 
  SubstituirDadosAlocacoesPorPeriodoPayload,
  SubstituirDadosAlocacoesPorPeriodoResponse
} from '@domain/entities/MapaAlocacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class SubstituirDadosAlocacoesPorPeriodoUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository,
  ) {}

  async execute(
    token: string,
    payload: SubstituirDadosAlocacoesPorPeriodoPayload
  ): Promise<SubstituirDadosAlocacoesPorPeriodoResponse> {
    if (!payload.codigoColaboradorNovo && !payload.codigoProjetoNovo) {
      throw new Error('Código do colaborador ou código do projeto deve ser informado')
    }

    if (!payload.periodosIdSubstituidos || payload.periodosIdSubstituidos.length === 0) {
      throw new Error('Pelo menos um período deve ser informado')
    }

    return this.repository.substituirDadosAlocacoesPorPeriodo(token, payload)
  }
}

