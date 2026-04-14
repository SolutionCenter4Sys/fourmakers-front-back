import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { BotFourmakersRepository } from '@domain/repositories/BotFourmakersRepository';
import type {
  ListarQuestoesParams,
  ListarQuestoesResponse
} from '@domain/entities/BotFourmakers';

@injectable()
export class ListarQuestoesUseCase {
  constructor(
    @inject(DiTokens.botFourmakersRepository)
    private readonly repository: BotFourmakersRepository
  ) {}

  async execute(
    token: string,
    params: ListarQuestoesParams
  ): Promise<ListarQuestoesResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    if (!params.chatId) {
      throw new Error('chatId é obrigatório');
    }

    return this.repository.listarQuestoes(token, params);
  }
}
