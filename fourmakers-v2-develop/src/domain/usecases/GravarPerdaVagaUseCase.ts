import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { GravarPerdaVagaPayload } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class GravarPerdaVagaUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(
    token: string,
    payload: GravarPerdaVagaPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string | null }> {
    return this.repository.gravarPerdaVaga(token, payload);
  }
}
