import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  MatchSemanticoRepository,
  RegistrarFeedbackPayload,
} from '@domain/repositories/MatchSemanticoRepository';

@injectable()
export class RegistrarFeedbackMatchSemanticoUseCase {
  constructor(
    @inject(DiTokens.matchSemanticoRepository)
    private readonly repository: MatchSemanticoRepository,
  ) {}

  async execute(token: string, payload: RegistrarFeedbackPayload): Promise<void> {
    return this.repository.registrarFeedback(token, payload);
  }
}
