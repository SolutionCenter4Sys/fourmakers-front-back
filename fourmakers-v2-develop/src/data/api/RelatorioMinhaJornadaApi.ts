import { httpClient } from './httpClient'

/**
 * API client for report generation endpoints.
 * 
 * This class handles HTTP communication for report downloads,
 * returning raw Response objects for binary data handling.
 */
export class RelatorioMinhaJornadaApi {
  /**
   * Fetches the detailed log report as a binary file.
   * 
   * @param token - Authentication token
   * @returns Promise resolving to the Response object containing the binary data
   * @throws Error if the request fails or returns non-2xx status
   */
  async getRelatorioLogDetalhado(token: string): Promise<Response> {
    return httpClient.getBlob(
      '/api/Competencia/DashboardMinhaJornada/RelatorioLogDetalhado',
      { token }
    )
  }
}
