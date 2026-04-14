import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { BotFourmakersRepository } from '@domain/repositories/BotFourmakersRepository';
import type {
  InserirQuestaoPayload,
  InserirQuestaoResponse
} from '@domain/entities/BotFourmakers';

@injectable()
export class InserirQuestaoUseCase {
  constructor(
    @inject(DiTokens.botFourmakersRepository)
    private readonly repository: BotFourmakersRepository
  ) {}

  async execute(
    token: string,
    payload: InserirQuestaoPayload
  ): Promise<InserirQuestaoResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    if (!payload.questao || !payload.questao.trim()) {
      throw new Error('A questão é obrigatória');
    }

    return this.repository.inserirQuestao(token, payload);
  }
}
