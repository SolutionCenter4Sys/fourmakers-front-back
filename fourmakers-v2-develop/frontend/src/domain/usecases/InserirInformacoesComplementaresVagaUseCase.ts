import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { InserirInformacoesComplementaresPayload } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class InserirInformacoesComplementaresVagaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(
    token: string,
    payload: InserirInformacoesComplementaresPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string | null }> {
    return this.repository.inserirInformacoesComplementaresVagaRecrutamento(token, payload);
  }
}
