import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidaturaDetails } from '@domain/entities/CandidaturaDetails';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class GetCandidaturaDetailsUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository,
  ) {}

  async execute(token: string, candidateId: string, candidaturaId?: string): Promise<CandidaturaDetails> {
    return this.repository.getCandidaturaDetails(token, candidateId, candidaturaId);
  }
}
