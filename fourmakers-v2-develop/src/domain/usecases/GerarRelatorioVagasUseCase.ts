import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { RelatorioVagaResult, VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class GerarRelatorioVagasUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, dataInicio: string, dataFim: string): Promise<RelatorioVagaResult> {
    return this.repository.getRelatorioVagas(token, dataInicio, dataFim);
  }
}
