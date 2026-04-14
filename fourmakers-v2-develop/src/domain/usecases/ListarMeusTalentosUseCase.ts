import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class ListarMeusTalentosUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository
  ) {}

  async execute(token: string, params?: { busca?: string; cursor?: number; limite?: number }) {
    return this.repository.listarMeusTalentos(token, params);
  }
}
