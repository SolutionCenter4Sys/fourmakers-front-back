import type { HistoricoCandidaturaResponse } from '@domain/entities/HistoricoCandidatura'

export interface HistoricoCandidaturaRepository {
  obterHistorico(
    token: string,
    codigoInternoColaborador: string,
    busca?: string,
  ): Promise<HistoricoCandidaturaResponse>
}
