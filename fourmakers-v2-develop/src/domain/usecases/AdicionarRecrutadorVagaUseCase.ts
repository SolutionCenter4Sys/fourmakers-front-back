import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class AdicionarRecrutadorVagaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, vagaId: string, cod: string) {
    return this.repository.adicionarRecrutadorVaga(token, vagaId, cod);
  }
}
