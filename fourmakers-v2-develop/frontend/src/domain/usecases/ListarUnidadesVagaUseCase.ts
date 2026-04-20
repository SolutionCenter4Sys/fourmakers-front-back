import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { UnidadeItem } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class ListarUnidadesVagaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string): Promise<UnidadeItem[]> {
    return this.repository.listarUnidades(token);
  }
}
