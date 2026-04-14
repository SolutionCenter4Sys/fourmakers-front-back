import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { OrigemColaboradorItem } from '@domain/entities/GestaoVagasCandidatos';
import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository';

@injectable()
export class ListarOrigensColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository
  ) {}

  async execute(token: string): Promise<OrigemColaboradorItem[]> {
    return this.repository.listarOrigensColaborador(token);
  }
}
