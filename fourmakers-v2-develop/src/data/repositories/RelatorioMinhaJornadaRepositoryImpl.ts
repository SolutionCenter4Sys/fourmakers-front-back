import { inject, injectable } from 'tsyringe'

import type { RelatorioMinhaJornadaRepository } from '@domain/repositories/RelatorioMinhaJornadaRepository'
import { DiTokens } from '@core/di/tokens'
import { RelatorioMinhaJornadaApi } from '@data/api/RelatorioMinhaJornadaApi'

/**
 * Repository implementation for report generation.
 * 
 * This implementation bridges the domain layer with browser APIs,
 * handling blob creation, URL generation, and download triggering.
 * All infrastructure concerns are isolated here.
 */
@injectable()
export class RelatorioMinhaJornadaRepositoryImpl
  implements RelatorioMinhaJornadaRepository
{
  constructor(
    @inject(DiTokens.relatorioMinhaJornadaApi)
    private readonly api: RelatorioMinhaJornadaApi,
  ) {}

  async generateRelatorioLogDetalhado(token: string): Promise<void> {
    // Call API to get binary response
    const response = await this.api.getRelatorioLogDetalhado(token)

    // Extract filename from Content-Disposition header if present
    const contentDisposition = response.headers.get('Content-Disposition')
    let filename = 'relatorio-log-detalhado.xlsx' // Default filename

    if (contentDisposition) {
      const filenameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/)
      if (filenameMatch && filenameMatch[1]) {
        filename = filenameMatch[1].replace(/['"]/g, '')
      }
    }

    // Convert response to blob
    const blob = await response.blob()

    // Create object URL
    const url = URL.createObjectURL(blob)

    // Create temporary anchor element and trigger download
    const link = document.createElement('a')
    link.href = url
    link.download = filename
    document.body.appendChild(link)
    link.click()

    // Cleanup: remove link and revoke object URL
    document.body.removeChild(link)
    URL.revokeObjectURL(url)
  }
}
