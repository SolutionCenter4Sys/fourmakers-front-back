import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaRecrutamentoCompleto } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class AtualizarVagaRecrutamentoPorIdUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(
    token: string,
    body: VagaRecrutamentoCompleto
  ): Promise<{ sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    return this.repository.atualizarVagaRecrutamentoPorId(token, body);
  }
}
