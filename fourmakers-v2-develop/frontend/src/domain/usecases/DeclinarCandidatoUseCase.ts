import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { DeclinarCandidaturaPayload } from '@domain/entities/GestaoVagasCandidatos';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class DeclinarCandidatoUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository,
  ) {}

  async execute(token: string, payload: DeclinarCandidaturaPayload) {
    return this.repository.declinarCandidato(token, payload);
  }
}
