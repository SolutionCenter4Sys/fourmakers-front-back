/** Payload da API Labs MatchSemantico BuscarMelhoresCandidatos. */
export interface BuscarMelhoresCandidatosPayload {
  vaga: string;
  cidade: string | null;
  estado: string | null;
  categoria: string | null;
  origem: string | null;
}

/** Resposta a ser tipada quando o contrato estiver definido. */
export type BuscarMelhoresCandidatosResponse = unknown;

/** Payload da API Labs MatchSemantico RegistrarFeedback. */
export interface RegistrarFeedbackPayload {
  idLogRankCandidatesIds: string;
  idLogMatchSemantico: string;
  /** true = usuário preferiu Motor Beta (IA), false = preferiu Motor Atual. */
  matchSemanticoMelhor: boolean;
}

export interface MatchSemanticoRepository {
  buscarMelhoresCandidatos(
    token: string,
    payload: BuscarMelhoresCandidatosPayload
  ): Promise<BuscarMelhoresCandidatosResponse>;

  registrarFeedback(token: string, payload: RegistrarFeedbackPayload): Promise<void>;
}
