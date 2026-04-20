import type {
  Feedback360Item,
  Feedback360AvaliacaoItem,
  Feedback360RelacionamentoItem,
  Feedback360CriarPayload,
  Feedback360AtualizarPayload,
  Feedback360CreateResponse,
} from '@domain/entities/Feedback360';

export interface Feedback360Repository {
  listarAvaliacoes(token: string): Promise<Feedback360AvaliacaoItem[]>;
  listarRelacionamentos(token: string): Promise<Feedback360RelacionamentoItem[]>;
  listarEnviados(token: string): Promise<Feedback360Item[]>;
  listarRecebidos(token: string): Promise<Feedback360Item[]>;
  criar(token: string, payload: Feedback360CriarPayload): Promise<Feedback360CreateResponse>;
  atualizar(
    token: string,
    id: string,
    payload: Feedback360AtualizarPayload,
  ): Promise<Feedback360CreateResponse>;
}
