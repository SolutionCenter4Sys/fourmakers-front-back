import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { RelatorioVagaResult, VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class GerarRelatorioVagasCandidaturasUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, dataInicio: string, dataFim: string): Promise<RelatorioVagaResult> {
    return this.repository.getRelatorioVagasCandidaturas(token, dataInicio, dataFim);
  }
}
