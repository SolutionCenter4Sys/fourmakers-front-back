import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { OpcaoContatoItem } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class ListarOpcoesContatoUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string): Promise<OpcaoContatoItem[]> {
    return this.repository.listarOpcoesContato(token);
  }
}
