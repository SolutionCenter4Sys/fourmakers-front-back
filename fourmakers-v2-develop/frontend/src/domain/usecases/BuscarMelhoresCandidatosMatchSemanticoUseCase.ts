import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { MatchSemanticoRepository, BuscarMelhoresCandidatosPayload } from '@domain/repositories/MatchSemanticoRepository';

@injectable()
export class BuscarMelhoresCandidatosMatchSemanticoUseCase {
  constructor(
    @inject(DiTokens.matchSemanticoRepository)
    private readonly repository: MatchSemanticoRepository,
  ) {}

  async execute(token: string, payload: BuscarMelhoresCandidatosPayload) {
    return this.repository.buscarMelhoresCandidatos(token, payload);
  }
}
