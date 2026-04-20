import { inject, injectable } from 'tsyringe'
import type { MinhaEquipeRepository } from '@domain/repositories/MinhaEquipeRepository'
import type { MinhaEquipeSugestao } from '@domain/entities/MinhaEquipeSugestao'
import type { MinhaEquipePDI } from '@domain/entities/MinhaEquipePDI'
import { DiTokens } from '@core/di/tokens'

export interface GetColaboradorRadarParams {
  token: string
  codInternoGestor: string
  perfilId: string
  codigoInternoColaborador: string
  uuid_colab: string
  uuid_manager: string
}

export interface GetColaboradorRadarResult {
  pdiMetas: MinhaEquipePDI[]
  sugestoes: MinhaEquipeSugestao[]
}

@injectable()
export class GetColaboradorRadarUseCase {
  constructor(
    @inject(DiTokens.minhaEquipeRepository)
    private readonly repository: MinhaEquipeRepository
  ) {}

  async execute(
    params: GetColaboradorRadarParams
  ): Promise<GetColaboradorRadarResult> {
    const {
      token,
      codInternoGestor,
      perfilId,
      codigoInternoColaborador,
      uuid_colab,
      uuid_manager,
    } = params

    // Busca paralela de PDI e sugestões
    const [pdiMetas, sugestoes] = await Promise.all([
      this.repository.buscarPDIColaborador({
        token,
        uuid_colab,
        uuid_manager,
      }),
      this.repository.buscarSugestoesColaborador({
        token,
        codInternoGestor,
        perfilId,
        codInternoColaborador: codigoInternoColaborador,
      }),
    ])

    return { pdiMetas, sugestoes }
  }
}
