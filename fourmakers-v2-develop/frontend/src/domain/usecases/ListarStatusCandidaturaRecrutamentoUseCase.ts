import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { StatusRecrutamentoRepository } from '@domain/repositories/StatusRecrutamentoRepository';

@injectable()
export class ListarStatusCandidaturaRecrutamentoUseCase {
  constructor(
    @inject(DiTokens.statusRecrutamentoRepository)
    private readonly repository: StatusRecrutamentoRepository,
  ) {}

  async execute(token: string) {
    return this.repository.listarStatusCandidatura(token);
  }
}
