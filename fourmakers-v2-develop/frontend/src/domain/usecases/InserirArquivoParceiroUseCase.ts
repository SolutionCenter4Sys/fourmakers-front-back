import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  InserirArquivoParams,
  InserirArquivoResponse,
} from '@domain/entities/ArquivoParceiro'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirArquivoParceiroUseCase {
  constructor(
    @inject(DiTokens.parceriaRepository)
    private readonly repository: ParceriaRepository
  ) {}

  async execute(
    token: string,
    params: InserirArquivoParams
  ): Promise<InserirArquivoResponse> {
    return this.repository.inserirArquivo(token, params)
  }
}
