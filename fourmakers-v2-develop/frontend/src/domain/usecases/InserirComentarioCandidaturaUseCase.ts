import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { InserirComentarioCandidaturaPayload } from '@domain/entities/GestaoVagasCandidatos';
import type { CandidaturaRepository, SaveCandidaturaResult } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class InserirComentarioCandidaturaUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository,
  ) {}

  async execute(token: string, payload: InserirComentarioCandidaturaPayload): Promise<SaveCandidaturaResult> {
    return this.repository.inserirComentarioCandidatura(token, payload);
  }
}

