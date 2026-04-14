import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaDetails } from '@domain/entities/VagaDetails';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class GetVagaDetalhesPublicoUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(codigoVaga: number): Promise<VagaDetails | null> {
    return this.repository.getVagaDetalhesPublico(codigoVaga);
  }
}
