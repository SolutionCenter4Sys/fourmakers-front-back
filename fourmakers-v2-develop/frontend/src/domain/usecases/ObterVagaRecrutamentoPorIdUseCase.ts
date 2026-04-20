import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaRecrutamentoCompleto } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class ObterVagaRecrutamentoPorIdUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, vagaId: string): Promise<VagaRecrutamentoCompleto | null> {
    return this.repository.getVagaRecrutamentoPorId(token, vagaId);
  }
}
