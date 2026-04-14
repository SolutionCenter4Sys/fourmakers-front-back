import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaFilhaItem } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class ListarVagasRecrutamentoPorParentEmAndamentoUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, vagaIdParent: string): Promise<VagaFilhaItem[]> {
    return this.repository.listarVagasRecrutamentoPorParentEmAndamento(token, vagaIdParent);
  }
}
