import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidateDetails } from '@domain/entities/CandidateDetails';
import type { CandidateRepository } from '@domain/repositories/CandidateRepository';

@injectable()
export class GetCandidateDetailsUseCase {
  constructor(
    @inject(DiTokens.candidateRepository)
    private readonly repository: CandidateRepository
  ) {}

  async execute(token: string, candidateId: string): Promise<CandidateDetails> {
    return this.repository.getCandidateDetails(token, candidateId);
  }
}
