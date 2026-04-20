import { inject, injectable } from 'tsyringe';
import type { VagasRepository } from '@domain/repositories/VagasRepository';
import type { StatusVaga } from '@domain/entities/Vaga';
import { DiTokens } from '@core/di/tokens';

@injectable()
export class ListarStatusVagasUseCase {
  constructor(
    @inject(DiTokens.vagasRepository)
    private readonly repository: VagasRepository
  ) {}

  async execute(token: string): Promise<StatusVaga[]> {
    return this.repository.listarStatusVagas(token);
  }
}
