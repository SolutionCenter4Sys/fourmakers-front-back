import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaRepository, ListarCandidatosInscritosParams } from '@domain/repositories/VagaRepository';

@injectable()
export class ListarCandidatosInscritosUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(token: string, params: ListarCandidatosInscritosParams) {
    const res = await this.repository.listarCandidatosInscritos(token, params);
    return { retorno: res?.retorno ?? [], sucesso: res?.sucesso };
  }
}
