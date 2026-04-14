import { inject, injectable } from 'tsyringe'
import type { HistoricoCandidaturaRepository } from '@domain/repositories/HistoricoCandidaturaRepository'
import type { HistoricoCandidaturaResponse } from '@domain/entities/HistoricoCandidatura'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterHistoricoCandidaturaUseCase {
  constructor(
    @inject(DiTokens.historicoCandidaturaRepository)
    private readonly repository: HistoricoCandidaturaRepository,
  ) {}

  async execute(
    token: string,
    codigoInternoColaborador: string,
    busca?: string,
  ): Promise<HistoricoCandidaturaResponse> {
    return this.repository.obterHistorico(token, codigoInternoColaborador, busca)
  }
}
