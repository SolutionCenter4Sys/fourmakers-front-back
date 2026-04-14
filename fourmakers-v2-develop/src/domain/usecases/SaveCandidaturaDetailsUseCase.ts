import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidaturaEditPayload } from '@domain/entities/CandidaturaDetails';
import type { CandidaturaRepository, SaveCandidaturaResult } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class SaveCandidaturaDetailsUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository,
  ) {}

  async execute(token: string, payload: CandidaturaEditPayload): Promise<SaveCandidaturaResult> {
    return this.repository.saveCandidaturaDetails(token, payload);
  }
}
