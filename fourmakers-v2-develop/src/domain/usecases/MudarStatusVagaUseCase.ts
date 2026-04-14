import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaRepository } from '@domain/repositories/VagaRepository';
import type { MudarStatusVagaPayload } from '@domain/entities/GestaoVagasCandidatos';

@injectable()
export class MudarStatusVagaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, payload: MudarStatusVagaPayload) {
    return this.repository.mudarStatusVaga(token, payload);
  }
}
