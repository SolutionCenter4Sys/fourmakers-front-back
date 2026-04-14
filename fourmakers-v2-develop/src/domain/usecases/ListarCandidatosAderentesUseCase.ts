import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CandidatoAderenteRaw } from '@domain/entities/GestaoVagasCandidatos';
import type { VagaRepository, ListarCandidatosAderentesParams } from '@domain/repositories/VagaRepository';

@injectable()
export class ListarCandidatosAderentesUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  async execute(
    token: string,
    params: ListarCandidatosAderentesParams
  ): Promise<{ retorno: CandidatoAderenteRaw[]; sucesso?: boolean }> {
    const res = await this.repository.listarCandidatosAderentes(token, params);
    return { retorno: res?.retorno ?? [], sucesso: res?.sucesso };
  }
}
