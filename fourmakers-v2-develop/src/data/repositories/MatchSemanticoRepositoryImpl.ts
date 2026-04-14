import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { MatchSemanticoApi } from '@data/api/MatchSemanticoApi';
import type {
  MatchSemanticoRepository,
  BuscarMelhoresCandidatosPayload,
  RegistrarFeedbackPayload,
} from '@domain/repositories/MatchSemanticoRepository';

@injectable()
export class MatchSemanticoRepositoryImpl implements MatchSemanticoRepository {
  constructor(
    @inject(DiTokens.matchSemanticoApi)
    private readonly api: MatchSemanticoApi,
  ) {}

  async buscarMelhoresCandidatos(token: string, payload: BuscarMelhoresCandidatosPayload) {
    return this.api.buscarMelhoresCandidatos(token, payload);
  }

  async registrarFeedback(token: string, payload: RegistrarFeedbackPayload): Promise<void> {
    return this.api.registrarFeedback(token, payload);
  }
}
