import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { MudarStatusCandidaturaPayload } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class MudarStatusCandidaturaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, payload: MudarStatusCandidaturaPayload) {
    return this.repository.mudarStatusCandidatura(token, payload);
  }
}
