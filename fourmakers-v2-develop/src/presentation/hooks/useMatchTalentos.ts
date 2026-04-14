/**
 * Hook para a tela Match de Talentos (Beta).
 * Coluna 1: API BuscarBancoTalentosComPromptMatch (Motor Atual).
 * Coluna 2: mock (Motor Beta). Feedbacks em localStorage.
 */

import { useState, useCallback, useMemo, useRef, useEffect } from 'react';
import { container } from 'tsyringe';
import type { MatchCardItem, Feedback, MatchStats, MatchEngine } from '@shared/types/matchTalentos';
import { STORAGE_KEY_FEEDBACKS } from '@shared/constants/matchTalentosMock';
import { BuscarBancoTalentosComPromptMatchUseCase } from '@domain/usecases/BuscarBancoTalentosComPromptMatchUseCase';
import { BuscarMelhoresCandidatosMatchSemanticoUseCase } from '@domain/usecases/BuscarMelhoresCandidatosMatchSemanticoUseCase';
import { RegistrarFeedbackMatchSemanticoUseCase } from '@domain/usecases/RegistrarFeedbackMatchSemanticoUseCase';
import { mapColaboradorMatchToCardItem } from '@presentation/utils/mapColaboradorMatchToCardItem';
import { toast } from 'sonner';
import {
  mapLabsResultToCardItem,
  type LabsApiResponse,
} from '@presentation/utils/mapLabsResultToCardItem';

const LIMITE_PRIMEIRA_COLUNA = 5;
const LIMITE_SEGUNDA_COLUNA = 5;

function loadFeedbacksFromStorage(): Feedback[] {
  if (typeof window === 'undefined' || !window.localStorage) return [];
  try {
    const raw = window.localStorage.getItem(STORAGE_KEY_FEEDBACKS);
    if (!raw) return [];
    const parsed = JSON.parse(raw) as unknown;
    return Array.isArray(parsed) ? parsed : [];
  } catch {
    return [];
  }
}

function saveFeedbacksToStorage(feedbacks: Feedback[]): void {
  if (typeof window === 'undefined' || !window.localStorage) return;
  try {
    window.localStorage.setItem(STORAGE_KEY_FEEDBACKS, JSON.stringify(feedbacks));
  } catch {
    // ignore
  }
}

function computeStats(feedbacks: Feedback[]): MatchStats {
  const total = feedbacks.length;
  const positive = feedbacks.filter((f) => f.isRelevant).length;
  const negative = total - positive;

  const byEngine = feedbacks.reduce(
    (acc, f) => {
      const key = f.engine;
      if (!acc[key]) acc[key] = { total: 0, positive: 0 };
      acc[key].total += 1;
      if (f.isRelevant) acc[key].positive += 1;
      return acc;
    },
    {} as Record<MatchEngine, { total: number; positive: number }>
  );

  const engineStats: MatchStats['engineStats'] = [
    { engine: 'actual', total: byEngine.actual?.total ?? 0, positive: byEngine.actual?.positive ?? 0 },
    { engine: 'beta', total: byEngine.beta?.total ?? 0, positive: byEngine.beta?.positive ?? 0 },
  ];

  const withPosition = feedbacks.filter((f) => f.rankingPosition >= 1 && f.rankingPosition <= 5);
  const at3 = withPosition.filter((f) => f.rankingPosition <= 3);
  const at5 = withPosition;
  const precisionAt3 = at3.length > 0 ? at3.filter((f) => f.isRelevant).length / at3.length : 0;
  const precisionAt5 = at5.length > 0 ? at5.filter((f) => f.isRelevant).length / at5.length : 0;

  return {
    metrics: { total, positive, negative },
    engineStats,
    precisionAt3: Math.round(precisionAt3 * 100) / 100,
    precisionAt5: Math.round(precisionAt5 * 100) / 100,
  };
}

export function useMatchTalentos(token: string | null) {
  const [feedbacks, setFeedbacks] = useState<Feedback[]>(loadFeedbacksFromStorage);
  const [query, setQuery] = useState('');
  const [resultsActual, setResultsActual] = useState<MatchCardItem[]>([]);
  const [resultsBeta, setResultsBeta] = useState<MatchCardItem[]>([]);
  const [loadingActual, setLoadingActual] = useState(false);
  const [loadingBeta, setLoadingBeta] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorActual, setErrorActual] = useState<string | null>(null);
  const [errorBeta, setErrorBeta] = useState<string | null>(null);
  const [idLogMatchSemantico, setIdLogMatchSemantico] = useState<string | null>(null);
  const [idLogRankCandidatesIds, setIdLogRankCandidatesIds] = useState<string | null>(null);
  /** Mensagem da API (validacao_informacoes.mensagem_usuario) quando Motor Atual retorna lista vazia. */
  const [mensagemUsuarioMotorAtual, setMensagemUsuarioMotorAtual] = useState<string | null>(null);
  /** Após clicar em "Prefiro essa resposta", fica true até a próxima busca (evita múltiplos registros por prompt). */
  const [preferenciaRegistrada, setPreferenciaRegistrada] = useState(false);

  /** Refs com os ids atuais para usar no clique (evita closure desatualizada na hora de chamar a API). */
  const idsRef = useRef({ idLogRankCandidatesIds: null as string | null, idLogMatchSemantico: null as string | null });
  useEffect(() => {
    idsRef.current = { idLogRankCandidatesIds, idLogMatchSemantico };
  }, [idLogRankCandidatesIds, idLogMatchSemantico]);

  const loading = loadingActual || loadingBeta;

  const COLUMN_ERROR_MESSAGE = 'Não foi possível obter os dados, realize um novo prompt.';

  const persistFeedbacks = useCallback((next: Feedback[]) => {
    setFeedbacks(next);
    saveFeedbacksToStorage(next);
  }, []);

  const runSearch = useCallback(async () => {
    if (!query.trim()) return;
    const q = query.trim();
    setError(null);
    setErrorActual(null);
    setErrorBeta(null);
    setResultsActual([]);
    setResultsBeta([]);
    setIdLogMatchSemantico(null);
    setIdLogRankCandidatesIds(null);
    setMensagemUsuarioMotorAtual(null);
    setPreferenciaRegistrada(false);
    setLoadingActual(!!token);
    setLoadingBeta(true);

    // A cada nova busca, reseta preferência dessa query para botões "Prefiro essa resposta" voltarem ao estado outlined
    setFeedbacks((prev) => {
      const next = prev.filter(
        (f) => !(f.candidateId === 'preferencia-coluna' && f.query === q)
      );
      if (next.length !== prev.length) saveFeedbacksToStorage(next);
      return next;
    });

    if (token) {
      const useCaseActual = container.resolve(BuscarBancoTalentosComPromptMatchUseCase);
      useCaseActual
        .execute(token, { texto_vaga: q, limite: 15 })
        .then((res) => {
          const colaboradores = res?.colaboradores ?? [];
          const items = (Array.isArray(colaboradores) ? colaboradores : [])
            .slice(0, LIMITE_PRIMEIRA_COLUNA)
            .map(mapColaboradorMatchToCardItem);
          setResultsActual(items);
          const raw = res as Record<string, unknown>;
          const idLog = raw?.idLogRankCandidatesIds ?? raw?.id_log_rank_candidates_ids ?? null;
          if (idLog && String(idLog).trim()) setIdLogRankCandidatesIds(String(idLog).trim());
          if (colaboradores.length === 0) {
            const prompt = raw?.prompt as Record<string, unknown> | undefined;
            const validacao =
              (prompt?.validacao_informacoes ?? prompt?.validacaoInformacoes) as Record<string, unknown> | undefined;
            const msg = validacao?.mensagem_usuario ?? validacao?.mensagemUsuario;
            const temValor = msg != null && typeof msg === 'string' && msg.trim() !== '';
            setMensagemUsuarioMotorAtual(temValor ? msg.trim() : null);
          } else {
            setMensagemUsuarioMotorAtual(null);
          }
        })
        .catch(() => {
          setErrorActual(COLUMN_ERROR_MESSAGE);
        })
        .finally(() => setLoadingActual(false));
    } else {
      setLoadingActual(false);
    }

    const runBeta = async () => {
      if (!token) {
        setResultsBeta([]);
        setLoadingBeta(false);
        return;
      }
      try {
        const matchSemanticoUseCase = container.resolve(BuscarMelhoresCandidatosMatchSemanticoUseCase);
        const res = await matchSemanticoUseCase.execute(token, {
          vaga: q,
          cidade: null,
          estado: null,
          categoria: null,
          origem: null,
        }) as LabsApiResponse | undefined;
        setErrorBeta(null);
        const rawBeta = res as Record<string, unknown> & {
          retorno?: { idLogMatchSemantico?: string; id_log_match_semantico?: string };
          data?: { retorno?: { idLogMatchSemantico?: string; id_log_match_semantico?: string } };
          idLogMatchSemantico?: string;
          id_log_match_semantico?: string;
        };
        const idLogBeta =
          rawBeta?.retorno?.idLogMatchSemantico ??
          rawBeta?.retorno?.id_log_match_semantico ??
          rawBeta?.data?.retorno?.idLogMatchSemantico ??
          rawBeta?.data?.retorno?.id_log_match_semantico ??
          rawBeta?.idLogMatchSemantico ??
          rawBeta?.id_log_match_semantico ??
          null;
        if (idLogBeta && String(idLogBeta).trim()) setIdLogMatchSemantico(String(idLogBeta).trim());
        const resposta = res?.retorno?.resposta ?? res?.retorno?.result;
        if (Array.isArray(resposta)) {
          const items = resposta.slice(0, LIMITE_SEGUNDA_COLUNA).map(mapLabsResultToCardItem);
          setResultsBeta(items);
        } else {
          setResultsBeta([]);
        }
      } catch {
        setResultsBeta([]);
        setErrorBeta(COLUMN_ERROR_MESSAGE);
      } finally {
        setLoadingBeta(false);
      }
    };
    runBeta();
  }, [query, token]);

  const addFeedback = useCallback(
    (payload: Omit<Feedback, 'id' | 'dataCriacao'>) => {
      const nextId = Math.max(0, ...feedbacks.map((f) => f.id)) + 1;
      const newFeedback: Feedback = {
        ...payload,
        id: nextId,
        dataCriacao: new Date().toISOString(),
      };
      const next = [...feedbacks, newFeedback];
      persistFeedbacks(next);
    },
    [feedbacks, persistFeedbacks]
  );

  const stats = useMemo(() => computeStats(feedbacks), [feedbacks]);

  /** Habilitar "Prefiro essa resposta" quando as duas chamadas terminaram e temos ambos os ids (permite feedback mesmo com lista vazia). */
  const ambasChamadasConcluidas = !loadingActual && !loadingBeta;
  const ambosIdsProntos = Boolean(idLogRankCandidatesIds && idLogMatchSemantico);
  const botoesPreferenciaHabilitados = ambasChamadasConcluidas && ambosIdsProntos;

  /** Chama a API RegistrarFeedback e marca preferência como registrada (desabilita botões até próxima busca). Usa ref para ter os ids no momento do clique. */
  const registrarFeedbackPreferencia = useCallback(
    (engine: MatchEngine) => {
      setPreferenciaRegistrada(true);
      const { idLogRankCandidatesIds: idRank, idLogMatchSemantico: idMatch } = idsRef.current;
      if (!token) {
        toast.error('Faça login para registrar a preferência.');
        return;
      }
      if (!idRank || !idMatch) {
        toast.error('Ids da busca ainda não disponíveis. Tente novamente em instantes.');
        return;
      }
      const useCase = container.resolve(RegistrarFeedbackMatchSemanticoUseCase);
      useCase
        .execute(token, {
          idLogRankCandidatesIds: idRank,
          idLogMatchSemantico: idMatch,
          matchSemanticoMelhor: engine === 'beta',
        })
        .then(() => {
          toast.success('Preferência registrada.');
        })
        .catch(() => {
          toast.error('Não foi possível registrar a preferência no servidor.');
        });
    },
    [token]
  );

  return {
    query,
    setQuery,
    resultsActual,
    resultsBeta,
    loading,
    loadingActual,
    loadingBeta,
    error,
    runSearch,
    feedbacks,
    addFeedback,
    stats,
    idLogMatchSemantico,
    idLogRankCandidatesIds,
    mensagemUsuarioMotorAtual,
    errorActual,
    errorBeta,
    preferenciaRegistrada,
    registrarFeedbackPreferencia,
    ambosIdsProntos,
    botoesPreferenciaHabilitados,
  };
}
