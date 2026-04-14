import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class CandidatarSeUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(
    token: string,
    payload: {
      codigoVaga: number;
      opcoesContatoIds: string[];
      pretencaoSalarial: string;
      modeloTrabalhoId: string;
    }
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }> {
    return this.repository.candidatarSe(token, payload);
  }
}
