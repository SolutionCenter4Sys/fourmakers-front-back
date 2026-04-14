import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidatoListItem } from '@domain/entities/CandidatoListItem';
import type { CandidatoListRepository } from '@domain/repositories/CandidatoListRepository';

@injectable()
export class ListCandidatosUseCase {
  constructor(
    @inject(DiTokens.candidatoListRepository)
    private readonly repository: CandidatoListRepository,
  ) {}

  async execute(token: string, vagaId: string): Promise<CandidatoListItem[]> {
    return this.repository.listCandidatos(token, vagaId);
  }
}
