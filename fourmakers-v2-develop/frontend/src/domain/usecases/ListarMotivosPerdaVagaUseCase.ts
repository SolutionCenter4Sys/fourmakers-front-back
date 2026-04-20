import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { MotivoPerdaVagaItem } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class ListarMotivosPerdaVagaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string): Promise<MotivoPerdaVagaItem[]> {
    return this.repository.listarMotivosPerdaVaga(token);
  }
}
