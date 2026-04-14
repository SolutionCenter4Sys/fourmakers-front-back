/** Opção de avaliação retornada por ListarAvaliacoes (id + descricao com emoji). */
export interface Feedback360AvaliacaoItem {
  id: number;
  descricao: string;
}

/** Opção de relacionamento retornada por ListarRelacionamentos (Gestor, Colega, etc.). */
export interface Feedback360RelacionamentoItem {
  id: number;
  descricao: string;
}

/** Item de feedback na listagem (enviados/recebidos) e resposta de criar/atualizar — alinhado ao Feedback360DTO da API. */
export interface Feedback360Item {
  id: string;
  codigoInternoColaboradorRemetente?: string;
  codigoInternoColaboradorDestinatario?: string;
  nomeRemetente?: string | null;
  nomeDestinatario?: string | null;
  titulo: string;
  descricao: string;
  dataInteracao: string;
  feedback360RelacionamentoId: number;
  relacionamentoDescricao: string;
  feedback360AvaliacaoId: number;
  avaliacaoDescricao: string;
  dataCriacao: string;
  dataAlteracao?: string | null;
  editado?: boolean;
  editavel?: boolean;
}

/** Payload para criar feedback (POST /CriarFeedback) — EnviarFeedback360DTO. */
export interface Feedback360CriarPayload {
  codigoInternoColaboradorDestinatario: string;
  titulo: string;
  descricao: string;
  dataInteracao: string;
  feedback360RelacionamentoId: number;
  feedback360AvaliacaoId: number;
}

/** Payload para atualizar feedback (PUT /AtualizarFeedback) — AtualizarFeedback360DTO. */
export interface Feedback360AtualizarPayload {
  titulo: string;
  descricao: string;
  dataInteracao: string;
  feedback360RelacionamentoId: number;
  feedback360AvaliacaoId: number;
}

/** Resposta da API de listagem (enviados/recebidos). */
export interface Feedback360ListResponse {
  retorno?: Feedback360Item[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Resposta da API de criação ou atualização (um item). */
export interface Feedback360CreateResponse {
  retorno?: Feedback360Item | null;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Resposta da API ListarAvaliacoes. */
export interface Feedback360ListaAvaliacoesResponse {
  retorno?: Feedback360AvaliacaoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Resposta da API ListarRelacionamentos. */
export interface Feedback360ListaRelacionamentosResponse {
  retorno?: Feedback360RelacionamentoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}
