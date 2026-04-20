import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';
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

@injectable()
export class BotFourmakersApi {
  async inserirQuestao(
    token: string,
    payload: InserirQuestaoPayload
  ): Promise<InserirQuestaoResponse> {
    return httpClient.post<InserirQuestaoResponse>(
      '/api/BotFourmakers/Questao/Inserir',
      payload,
      { token }
    );
  }

  async listarChats(
    token: string
  ): Promise<ListarChatsResponse> {
    return httpClient.get<ListarChatsResponse>(
      '/api/BotFourmakers/Chat/Listar',
      { token }
    );
  }

  async listarQuestoes(
    token: string,
    params: ListarQuestoesParams
  ): Promise<ListarQuestoesResponse> {
    const url = `/api/BotFourmakers/Questao/Listar?chatId=${params.chatId}`;
    return httpClient.get<ListarQuestoesResponse>(url, { token });
  }

  async removerChat(
    token: string,
    params: RemoverChatParams
  ): Promise<RemoverChatResponse> {
    const url = `/api/BotFourmakers/Chat/Remover?chatId=${params.chatId}`;
    return httpClient.delete<RemoverChatResponse>(url, { token });
  }

  async inserirFeedback(
    token: string,
    payload: InserirFeedbackPayload
  ): Promise<InserirFeedbackResponse> {
    return httpClient.post<InserirFeedbackResponse>(
      '/api/BotFourmakers/Feedback/Inserir',
      payload,
      { token }
    );
  }
}
