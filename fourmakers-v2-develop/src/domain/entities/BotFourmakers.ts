export interface Chat {
  id: number;
  mensagem: string;
  criadoEm: string;
}

export interface Questao {
  id: number;
  chatId: number;
  mensagem: string;
  criadoEm: string;
  chat: Chat;
  feedback: boolean | null;
  tipo: number; // 1 = pergunta do usuário, 2 = resposta do bot
  queryHabilidades: string;
}

export interface InserirQuestaoPayload {
  questao: string;
  chatId: number | null;
}

export interface InserirQuestaoResponse {
  retorno: Questao[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface ListarChatsResponse {
  retorno: Chat[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface ListarQuestoesParams {
  chatId: number;
}

export interface ListarQuestoesResponse {
  retorno: Questao[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface RemoverChatParams {
  chatId: number;
}

export interface RemoverChatResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface InserirFeedbackPayload {
  questaoId: number;
  comentario: string;
  feedback: boolean;
}

export interface InserirFeedbackResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}
