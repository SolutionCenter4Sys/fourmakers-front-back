import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { BotFourmakersRepository } from '@domain/repositories/BotFourmakersRepository';
import type {
  InserirFeedbackPayload,
  InserirFeedbackResponse
} from '@domain/entities/BotFourmakers';

@injectable()
export class InserirFeedbackBotUseCase {
  constructor(
    @inject(DiTokens.botFourmakersRepository)
    private readonly repository: BotFourmakersRepository
  ) {}

  async execute(
    token: string,
    payload: InserirFeedbackPayload
  ): Promise<InserirFeedbackResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    if (!payload.questaoId) {
      throw new Error('questaoId é obrigatório');
    }

    return this.repository.inserirFeedback(token, payload);
  }
}
