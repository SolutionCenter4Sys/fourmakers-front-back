import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ObterTotaisInscritosRetorno } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class ObterTotaisInscritosUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, vagaId: string): Promise<ObterTotaisInscritosRetorno | null> {
    return this.repository.obterTotaisInscritos(token, vagaId);
  }
}
