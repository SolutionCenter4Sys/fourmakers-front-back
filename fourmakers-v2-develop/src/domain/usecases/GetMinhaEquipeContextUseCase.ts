import { inject, injectable } from 'tsyringe'
import type { MinhaEquipeRepository } from '@domain/repositories/MinhaEquipeRepository'
import type { MinhaEquipeColaborador } from '@domain/entities/MinhaEquipeColaborador'
import type { MinhaEquipeKPIs } from '@domain/entities/MinhaEquipeKPIs'
import { DiTokens } from '@core/di/tokens'

export interface GetMinhaEquipeContextParams {
  token: string
  codGestorAdm: string
  orgId: number
  codGestorOper: string
  isOrquestracao: boolean // Se true, codGestorAdm e codGestorOper vazios
}

export interface GetMinhaEquipeContextResult {
  colaboradores: MinhaEquipeColaborador[]
  kpis: MinhaEquipeKPIs
}

@injectable()
export class GetMinhaEquipeContextUseCase {
  constructor(
    @inject(DiTokens.minhaEquipeRepository)
    private readonly repository: MinhaEquipeRepository
  ) {}

  async execute(
    params: GetMinhaEquipeContextParams
  ): Promise<GetMinhaEquipeContextResult> {
    const { token, codGestorAdm, orgId, codGestorOper, isOrquestracao } = params

    // Ajustar parâmetros para orquestração (dados organization-wide)
    const finalCodGestorAdm = isOrquestracao ? '' : codGestorAdm
    const finalCodGestorOper = isOrquestracao ? '' : codGestorOper

    // Busca paralela de KPIs e colaboradores
    const [kpis, colaboradores] = await Promise.all([
      this.repository.buscarTotalizacaoIndicadores({
        token,
        codGestorAdm: finalCodGestorAdm,
        orgId,
        codGestorOper: finalCodGestorOper,
      }),
      this.repository.listarIndicadoresLiderados({
        token,
        codGestorAdm: finalCodGestorAdm,
        orgId,
        codGestorOper: finalCodGestorOper,
        cursor: 0,
        limite: 10000,
      }),
    ])

    return { colaboradores, kpis }
  }
}
