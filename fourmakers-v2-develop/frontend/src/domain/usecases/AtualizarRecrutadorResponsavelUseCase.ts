import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class AtualizarRecrutadorResponsavelUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository,
  ) {}

  async execute(token: string, idCandidatura: string, codigoRecrutadorResponsavel: string) {
    return this.repository.atualizarRecrutadorResponsavel(token, idCandidatura, codigoRecrutadorResponsavel);
  }
}
