import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { AtualizarCandidaturaPayload } from '@domain/entities/GestaoVagasCandidatos';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class AtualizarCandidaturaUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository,
  ) {}

  async execute(token: string, payload: AtualizarCandidaturaPayload) {
    return this.repository.atualizarCandidatura(token, payload);
  }
}
