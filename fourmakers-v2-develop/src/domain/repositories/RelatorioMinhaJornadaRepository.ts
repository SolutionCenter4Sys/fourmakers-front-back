/**
 * Repository interface for generating reports related to Minha Jornada dashboard.
 * 
 * This interface defines the contract for report generation operations,
 * keeping the domain layer agnostic of HTTP, blobs, and browser APIs.
 */
export interface RelatorioMinhaJornadaRepository {
  /**
   * Generates and triggers download of the detailed log report.
   * 
   * @param token - Authentication token
   * @returns Promise that resolves when the download is triggered
   * @throws Error if the download fails
   */
  generateRelatorioLogDetalhado(token: string): Promise<void>
}
