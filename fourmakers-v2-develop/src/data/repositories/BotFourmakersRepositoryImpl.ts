import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { BotFourmakersRepository } from '@domain/repositories/BotFourmakersRepository';
import type {
  InserirQuestaoPayload,
  InserirQuestaoResponse,
  ListarChatsResponse,
  ListarQuestoesParams,
  ListarQuestoesResponse,
  RemoverChatParams,
  RemoverChatResponse,
  InserirFeedbackPayload,
  InserirFeedbackResponse
} from '@domain/entities/BotFourmakers';
import type { BotFourmakersApi } from '@data/api/BotFourmakersApi';

@injectable()
export class BotFourmakersRepositoryImpl implements BotFourmakersRepository {
  constructor(
    @inject(DiTokens.botFourmakersApi)
    private readonly botFourmakersApi: BotFourmakersApi
  ) {}

  async inserirQuestao(
    token: string,
    payload: InserirQuestaoPayload
  ): Promise<InserirQuestaoResponse> {
    return this.botFourmakersApi.inserirQuestao(token, payload);
  }

  async listarChats(
    token: string
  ): Promise<ListarChatsResponse> {
    return this.botFourmakersApi.listarChats(token);
  }

  async listarQuestoes(
    token: string,
    params: ListarQuestoesParams
  ): Promise<ListarQuestoesResponse> {
    return this.botFourmakersApi.listarQuestoes(token, params);
  }

  async removerChat(
    token: string,
    params: RemoverChatParams
  ): Promise<RemoverChatResponse> {
    return this.botFourmakersApi.removerChat(token, params);
  }

  async inserirFeedback(
    token: string,
    payload: InserirFeedbackPayload
  ): Promise<InserirFeedbackResponse> {
    return this.botFourmakersApi.inserirFeedback(token, payload);
  }
}
