import { inject, injectable } from 'tsyringe'

import type {
  ColaboradoresRepository,
  CertificadoColaboradorResult,
} from '@domain/repositories/ColaboradoresRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class BaixarCertificadoColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository,
  ) {}

  async execute(token: string, path: string): Promise<CertificadoColaboradorResult> {
    return this.repository.baixarCertificadoColaborador(token, path)
  }
}
