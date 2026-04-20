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

export interface BotFourmakersRepository {
  inserirQuestao(
    token: string,
    payload: InserirQuestaoPayload
  ): Promise<InserirQuestaoResponse>;

  listarChats(
    token: string
  ): Promise<ListarChatsResponse>;

  listarQuestoes(
    token: string,
    params: ListarQuestoesParams
  ): Promise<ListarQuestoesResponse>;

  removerChat(
    token: string,
    params: RemoverChatParams
  ): Promise<RemoverChatResponse>;

  inserirFeedback(
    token: string,
    payload: InserirFeedbackPayload
  ): Promise<InserirFeedbackResponse>;
}
