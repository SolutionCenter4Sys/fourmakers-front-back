import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { BotFourmakersRepository } from '@domain/repositories/BotFourmakersRepository';
import type { ListarChatsResponse } from '@domain/entities/BotFourmakers';

@injectable()
export class ListarChatsUseCase {
  constructor(
    @inject(DiTokens.botFourmakersRepository)
    private readonly repository: BotFourmakersRepository
  ) {}

  async execute(
    token: string
  ): Promise<ListarChatsResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    return this.repository.listarChats(token);
  }
}
