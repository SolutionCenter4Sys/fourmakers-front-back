/**
 * Mapeia o retorno da API Labs MatchSemantico (BuscarMelhoresCandidatos) para MatchCardItem.
 * Score 0–1 vira match 0–100; parecer_IA_senioridade fica em retornoMatch para o modal.
 */

import type { MatchCardItem } from '@shared/types/matchTalentos';

export interface LabsResultItem {
  score: number;
  id: string;
  nome: string;
  sobre?: string;
  cidade?: string;
  estado?: string;
  experiencia?: unknown[];
  categoria?: string;
  origem?: string;
  is_pula_pula?: boolean;
  parecer_IA_senioridade?: {
    parecer: boolean;
    motivo_parecer?: string[];
  };
}

export interface LabsApiResponse {
  retorno?: {
    sucesso?: boolean;
    /** Formato atual da API Labs. */
    result?: LabsResultItem[];
    /** Formato alternativo (resposta). */
    resposta?: LabsResultItem[];
    httpStatus?: string;
    mensagem?: string;
    idLogMatchSemantico?: string;
  };
  sucesso?: boolean;
  mensagem?: string;
  erros?: unknown;
}

/** Converte score 0–1 em percentual para exibição (0–100). Regras visuais no card: >=80 verde, >=60 warning, <60 destructive. */
export function labsScoreToMatchPercent(score: number): number {
  const pct = Math.round(score * 1000) / 10;
  return Math.min(100, Math.max(0, pct));
}

function formatExperienciasLabel(experiencia: unknown[] | undefined): string {
  const n = Array.isArray(experiencia) ? experiencia.length : 0;
  return n === 1 ? '1 experiência' : `${n} experiências`;
}

export function mapLabsResultToCardItem(raw: LabsResultItem): MatchCardItem {
  const score = typeof raw.score === 'number' ? raw.score : 0;
  const matchPct = labsScoreToMatchPercent(score);
  const id = raw.id ?? '';
  const resumo = [raw.categoria, raw.cidade || raw.estado ? [raw.cidade, raw.estado].filter(Boolean).join(', ') : null]
    .filter(Boolean)
    .join(' · ') || (raw.sobre ? raw.sobre.slice(0, 80) + (raw.sobre.length > 80 ? '…' : '') : undefined);

  return {
    id,
    codigoInternoColaborador: id,
    nome: raw.nome ?? '—',
    match: matchPct,
    skills: [],
    candidaturasLabel: formatExperienciasLabel(raw.experiencia),
    origem: raw.origem ?? '—',
    resumo: resumo || undefined,
    retornoMatch: {
      score,
      parecer_IA_senioridade: raw.parecer_IA_senioridade,
    },
  };
}
