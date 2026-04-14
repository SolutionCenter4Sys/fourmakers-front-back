import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { InserirArquivoCandidaturaParams } from '@domain/entities/GestaoVagasCandidatos';
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class InserirArquivoCandidaturaUseCase {
  constructor(
    @inject(DiTokens.candidaturaRepository)
    private readonly repository: CandidaturaRepository,
  ) {}

  async execute(token: string, params: InserirArquivoCandidaturaParams) {
    return this.repository.inserirArquivo(token, params);
  }
}
