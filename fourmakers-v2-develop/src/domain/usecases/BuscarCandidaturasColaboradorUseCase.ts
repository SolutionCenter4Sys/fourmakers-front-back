import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class BuscarCandidaturasColaboradorUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository,
  ) {}

  async execute(token: string, codColaborador: string) {
    return this.repository.buscarCandidaturasColaborador(token, codColaborador);
  }
}
