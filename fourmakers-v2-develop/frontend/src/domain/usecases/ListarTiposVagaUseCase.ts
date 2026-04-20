import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { TipoVagaItem } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class ListarTiposVagaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string): Promise<TipoVagaItem[]> {
    return this.repository.listarTiposVaga(token);
  }
}
