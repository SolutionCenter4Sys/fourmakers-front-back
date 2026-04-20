import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidatarOutraPessoaPayload } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class CandidatarOutraPessoaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(
    token: string,
    payload: CandidatarOutraPessoaPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string }> {
    return this.repository.candidatarOutraPessoa(token, payload);
  }
}
