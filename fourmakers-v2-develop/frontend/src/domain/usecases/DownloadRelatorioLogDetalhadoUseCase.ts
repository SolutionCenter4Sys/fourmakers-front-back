import { inject, injectable } from 'tsyringe'

import type { RelatorioMinhaJornadaRepository } from '@domain/repositories/RelatorioMinhaJornadaRepository'
import { DiTokens } from '@core/di/tokens'

/**
 * Use Case for downloading the detailed log report.
 * 
 * This use case orchestrates the download action without knowing
 * about HTTP, blobs, or browser APIs. It delegates to the repository
 * which handles the infrastructure concerns.
 */
@injectable()
export class DownloadRelatorioLogDetalhadoUseCase {
  constructor(
    @inject(DiTokens.relatorioMinhaJornadaRepository)
    private readonly repository: RelatorioMinhaJornadaRepository,
  ) {}

  /**
   * Executes the download of the detailed log report.
   * 
   * @param token - Authentication token
   * @throws Error if token is invalid or download fails
   */
  async execute(token: string): Promise<void> {
    // Validate preconditions
    if (!token || token.trim() === '') {
      throw new Error('Token de autenticação é obrigatório')
    }

    // Delegate to repository
    await this.repository.generateRelatorioLogDetalhado(token)
  }
}
