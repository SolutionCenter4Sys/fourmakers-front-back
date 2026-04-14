/**
 * Tipos para Match de Talentos (Beta).
 * Alinhados à documentação técnica e contratos do backend (.NET 8).
 */

export type MatchEngine = 'actual' | 'beta';

export interface MatchCandidate {
  id: string;
  codigoInternoColaborador: string;
  nome: string;
  /** Resumo mock para exibição no card (ex.: cargo, localização). */
  resumo?: string;
}

/** Skill exibida no card (extraída de comparativo_por_skill). */
export interface MatchCardSkill {
  skill: string;
  nivel: string;
}

/**
 * Item unificado para exibição no card (Motor Atual = API, Motor Beta = mock).
 * retornoMatch preenchido apenas quando vem da API (abre modal de cálculo).
 */
export interface MatchCardItem {
  id: string;
  codigoInternoColaborador: string;
  nome: string;
  match: number;
  skills: MatchCardSkill[];
  candidaturasLabel: string;
  origem: string;
  resumo?: string;
  /** Preenchido quando origem é API; usado no modal de cálculo do match (RetornoMatchRaw). */
  retornoMatch?: unknown;
}

export interface FeedbackPayload {
  recruiter: string;
  query: string;
  candidateName: string;
  candidateId: string;
  engine: MatchEngine;
  rankingPosition: number;
  isRelevant: boolean;
  reason: string | null;
  /** Preenchido ao preferir Motor Beta (IA); id da chamada Labs MatchSemantico para futuro POST. */
  idLogMatchSemantico?: string | null;
  /** Preenchido ao preferir Motor Atual; id da chamada BuscarBancoTalentosComPromptMatch para futuro POST. */
  idLogRankCandidatesIds?: string | null;
}

export interface Feedback extends FeedbackPayload {
  id: number;
  dataCriacao: string;
}

export interface EngineStats {
  engine: MatchEngine;
  total: number;
  positive: number;
}

export interface MatchStats {
  metrics: {
    total: number;
    positive: number;
    negative: number;
  };
  engineStats: EngineStats[];
  /** Precision@3: proporção de relevantes entre os 3 primeiros por (query, engine). */
  precisionAt3?: number;
  /** Precision@5: proporção de relevantes entre os 5 primeiros por (query, engine). */
  precisionAt5?: number;
}
