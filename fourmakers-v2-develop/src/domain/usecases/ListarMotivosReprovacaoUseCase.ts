import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class ListarMotivosReprovacaoUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository
  ) {}

  async execute(token: string) {
    return this.repository.listarMotivosReprovacao(token);
  }
}
