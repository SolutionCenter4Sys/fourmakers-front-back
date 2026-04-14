import { inject, injectable } from 'tsyringe'
import type { HistoricoCandidaturaRepository } from '@domain/repositories/HistoricoCandidaturaRepository'
import type { HistoricoCandidaturaResponse } from '@domain/entities/HistoricoCandidatura'
import { HistoricoCandidaturaApi } from '@data/api/HistoricoCandidaturaApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class HistoricoCandidaturaRepositoryImpl implements HistoricoCandidaturaRepository {
  constructor(
    @inject(DiTokens.historicoCandidaturaApi)
    private readonly api: HistoricoCandidaturaApi,
  ) {}

  async obterHistorico(
    token: string,
    codigoInternoColaborador: string,
    busca?: string,
  ): Promise<HistoricoCandidaturaResponse> {
    return this.api.obterHistorico(token, codigoInternoColaborador, busca)
  }
}
