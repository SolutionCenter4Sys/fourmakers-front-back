import type { CandidateDetails } from '@domain/entities/CandidateDetails';

export interface CandidateRepository {
  getCandidateDetails(token: string, candidateId: string): Promise<CandidateDetails>;
}
