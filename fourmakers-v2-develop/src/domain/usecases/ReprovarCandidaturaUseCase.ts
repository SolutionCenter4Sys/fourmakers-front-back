import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ReprovarCandidaturaPayload } from '@domain/entities/GestaoVagasCandidatos';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class ReprovarCandidaturaUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository
  ) {}

  async execute(token: string, payload: ReprovarCandidaturaPayload) {
    return this.repository.reprovarCandidatura(token, payload);
  }
}
