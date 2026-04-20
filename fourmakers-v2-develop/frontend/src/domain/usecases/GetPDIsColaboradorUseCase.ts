import { inject, injectable } from 'tsyringe'
import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'
import { DiTokens } from '@core/di/tokens'

export interface GetPDIsColaboradorParams {
  token: string
  uuid_colab: string
}

export interface GetPDIsColaboradorResult {
  pdis: Array<{
    id: number
    skills: Array<{
      id: number
      skill_name: string
      pdi_id: number
    }>
  }>
}

@injectable()
export class GetPDIsColaboradorUseCase {
  constructor(
    @inject(DiTokens.minhaJornadaRepository)
    private readonly repository: MinhaJornadaRepository,
  ) {}

  async execute(
    params: GetPDIsColaboradorParams
  ): Promise<GetPDIsColaboradorResult> {
    const { token, uuid_colab } = params

    const pdis = await this.repository.buscarPDIColaborador({
      token,
      uuid_colab,
    })

    return { pdis }
  }
}
