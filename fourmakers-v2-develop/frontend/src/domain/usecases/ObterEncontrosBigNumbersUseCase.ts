import { inject, injectable } from 'tsyringe'
import type {
  EncontrosBigNumbersRepository,
  ObterBigNumbersParams,
} from '@domain/repositories/EncontrosBigNumbersRepository'
import type { KpiSaude, KpiAlcance } from '@shared/types/dashboardComercialTypes'
import { DiTokens } from '@core/di/tokens'

export interface EncontrosBigNumbersMapeado {
  kpiSaude: KpiSaude
  kpiAlcance: KpiAlcance
}

@injectable()
export class ObterEncontrosBigNumbersUseCase {
  constructor(
    @inject(DiTokens.encontrosBigNumbersRepository)
    private readonly repository: EncontrosBigNumbersRepository,
  ) {}

  async execute(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<EncontrosBigNumbersMapeado> {
    const retorno = await this.repository.obterBigNumbers(token, params)
    return {
      kpiSaude: {
        encontrosRealizados: retorno.reunioesRealizados ?? retorno.encontrosRealizados ?? 0,
        semInteracao: retorno.reunioesSemInteracao ?? retorno.encontrosSemInteracao ?? 0,
        acoesEmAtraso: retorno.acoesEmAtraso ?? 0,
      },
      kpiAlcance: {
        clientesImpactados: retorno.clientesImpactados ?? 0,
        gestoresImpactados: retorno.gestoresImpactados ?? 0,
        categoriasComInteracao: retorno.categoriasComInteracao ?? 0,
      },
    }
  }
}
