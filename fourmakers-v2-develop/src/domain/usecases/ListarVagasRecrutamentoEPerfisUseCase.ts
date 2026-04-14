import { inject, injectable } from 'tsyringe';
import type { VagasRepository } from '@domain/repositories/VagasRepository';
import type {
  ListarVagasRecrutamentoEPerfisParams,
  ListarVagasRecrutamentoEPerfisResult,
} from '@domain/repositories/VagasRepository';
import { DiTokens } from '@core/di/tokens';

@injectable()
export class ListarVagasRecrutamentoEPerfisUseCase {
  constructor(
    @inject(DiTokens.vagasRepository)
    private readonly repository: VagasRepository
  ) {}

  async execute(
    token: string,
    params: ListarVagasRecrutamentoEPerfisParams
  ): Promise<ListarVagasRecrutamentoEPerfisResult> {
    return this.repository.listarVagasRecrutamentoEPerfis(token, params);
  }
}
