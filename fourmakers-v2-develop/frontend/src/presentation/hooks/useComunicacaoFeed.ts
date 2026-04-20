import { useState, useCallback, useEffect, useRef } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import type {
  ComunicacaoFeedParams,
  ComunicacaoFeedResult,
  ComunicacaoGrupo,
} from '@domain/entities/comunicacao';
import { ListarComunicacaoFeedUseCase } from '@domain/usecases/ListarComunicacaoFeedUseCase';
import { ListarComunicacaoGruposUseCase } from '@domain/usecases/ListarComunicacaoGruposUseCase';

const FEED_PAGE_SIZE = 10;

export interface UseComunicacaoFeedOptions {
  /** Id da comunidade (vindo da URL /comunicacao/grupo/:groupId). Quando informado, não chama loadFeed/loadRequiredFeed no mount; apenas loadCommunityFeed(comunidadeId) é chamado. */
  comunidadeId?: string | null;
}

export function useComunicacaoFeed(options: UseComunicacaoFeedOptions = {}) {
  const { comunidadeId } = options;
  const token = useAppSelector((state) => state.auth.token);
  const listarComunicacaoFeedUseCase = container.resolve(ListarComunicacaoFeedUseCase);
  const listarComunicacaoGruposUseCase = container.resolve(ListarComunicacaoGruposUseCase);

  const [feed, setFeed] = useState<ComunicacaoFeedResult | null>(null);
  const [requiredFeed, setRequiredFeed] = useState<ComunicacaoFeedResult | null>(null);
  const [communityFeed, setCommunityFeed] = useState<ComunicacaoFeedResult | null>(null);
  const [grupos, setGrupos] = useState<ComunicacaoGrupo[]>([]);
  const [loading, setLoading] = useState(false);
  const [loadingRequired, setLoadingRequired] = useState(false);
  const [loadingCommunityFeed, setLoadingCommunityFeed] = useState(false);
  const [loadingMoreFeed, setLoadingMoreFeed] = useState(false);
  const [loadingMoreCommunityFeed, setLoadingMoreCommunityFeed] = useState(false);
  const [loadingGrupos, setLoadingGrupos] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const feedPageRef = useRef(1);
  const communityFeedPageRef = useRef(1);
  const [lastFeedPageFull, setLastFeedPageFull] = useState(false);
  const [lastCommunityFeedPageFull, setLastCommunityFeedPageFull] = useState(false);

  const loadFeed = useCallback(
    async (params?: ComunicacaoFeedParams) => {
      if (!token) return;
      setLoading(true);
      setError(null);
      feedPageRef.current = 1;
      try {
        const result = await listarComunicacaoFeedUseCase.execute(token, {
          ...params,
          pagina: 1,
          quantidadePorPagina: FEED_PAGE_SIZE,
          excluirPendenteAprovacao: true,
          /** Tab feed: exibir apenas publicações não ocultas no feed. */
          ...(comunidadeId ? {} : { somenteNaoOcultoNoFeed: true }),
        });
        setFeed(result);
        setLastFeedPageFull((result.publicacoes?.length ?? 0) >= FEED_PAGE_SIZE);
      } catch (e) {
        setError(e instanceof Error ? e.message : 'Erro ao carregar feed.');
        setFeed(null);
        setLastFeedPageFull(false);
      } finally {
        setLoading(false);
      }
    },
    [token, listarComunicacaoFeedUseCase, comunidadeId],
  );

  const loadMoreFeed = useCallback(
    async (params?: ComunicacaoFeedParams) => {
      if (!token || loading || loadingMoreFeed) return;
      const nextPage = feedPageRef.current + 1;
      setLoadingMoreFeed(true);
      try {
        const result = await listarComunicacaoFeedUseCase.execute(token, {
          somenteLeituraObrigatoria: false,
          tipo: '',
          labels: params?.labels ?? [],
          comunidadeId: '',
          pagina: nextPage,
          quantidadePorPagina: FEED_PAGE_SIZE,
          excluirPendenteAprovacao: true,
          somenteNaoOcultoNoFeed: true,
        });
      feedPageRef.current = nextPage;
      const addedCountRef = { current: 0 };
      setFeed((prev) => {
        if (!prev) return result;
        const existingIds = new Set((prev.publicacoes ?? []).map((p) => p.id));
        const newItems = (result.publicacoes ?? []).filter((p) => !existingIds.has(p.id));
        addedCountRef.current = newItems.length;
        return {
          ...prev,
          publicacoes: [...(prev.publicacoes ?? []), ...newItems],
          quantidadeTotal: result.quantidadeTotal ?? prev.quantidadeTotal,
        };
      });
      setLastFeedPageFull(addedCountRef.current >= FEED_PAGE_SIZE);
    } catch {
      setLastFeedPageFull(false);
    } finally {
      setLoadingMoreFeed(false);
    }
  },
    [token, listarComunicacaoFeedUseCase, loading, loadingMoreFeed],
  );

  const hasMoreFeed = useCallback(() => lastFeedPageFull, [lastFeedPageFull]);

  const loadRequiredFeed = useCallback(async () => {
    if (!token) return;
    setLoadingRequired(true);
    try {
      const result = await listarComunicacaoFeedUseCase.execute(token, {
        somenteLeituraObrigatoria: true,
        excluirPendenteAprovacao: true,
      });
      setRequiredFeed(result);
    } catch {
      setRequiredFeed(null);
    } finally {
      setLoadingRequired(false);
    }
  }, [token, listarComunicacaoFeedUseCase]);

  const loadCommunityFeed = useCallback(
    async (comunidadeIdParam: string) => {
      if (!token || !comunidadeIdParam) return;
      setLoadingCommunityFeed(true);
      setCommunityFeed(null);
      communityFeedPageRef.current = 1;
      try {
        const result = await listarComunicacaoFeedUseCase.execute(token, {
          somenteLeituraObrigatoria: false,
          tipo: '',
          labels: [],
          comunidadeId: comunidadeIdParam,
          pagina: 1,
          quantidadePorPagina: FEED_PAGE_SIZE,
          excluirPendenteAprovacao: true,
        });
        setCommunityFeed(result);
        setLastCommunityFeedPageFull((result.publicacoes?.length ?? 0) >= FEED_PAGE_SIZE);
      } catch {
        setCommunityFeed(null);
        setLastCommunityFeedPageFull(false);
      } finally {
        setLoadingCommunityFeed(false);
      }
    },
    [token, listarComunicacaoFeedUseCase],
  );

  const loadMoreCommunityFeed = useCallback(
    async (comunidadeIdParam: string) => {
      if (!token || !comunidadeIdParam || loadingCommunityFeed || loadingMoreCommunityFeed) return;
      const nextPage = communityFeedPageRef.current + 1;
      setLoadingMoreCommunityFeed(true);
      try {
        const result = await listarComunicacaoFeedUseCase.execute(token, {
          somenteLeituraObrigatoria: false,
          tipo: '',
          labels: [],
          comunidadeId: comunidadeIdParam,
          pagina: nextPage,
          quantidadePorPagina: FEED_PAGE_SIZE,
          excluirPendenteAprovacao: true,
        });
        communityFeedPageRef.current = nextPage;
        const addedCountRef = { current: 0 };
        setCommunityFeed((prev) => {
          if (!prev) return result;
          const existingIds = new Set((prev.publicacoes ?? []).map((p) => p.id));
          const newItems = (result.publicacoes ?? []).filter((p) => !existingIds.has(p.id));
          addedCountRef.current = newItems.length;
          return {
            ...prev,
            publicacoes: [...(prev.publicacoes ?? []), ...newItems],
            quantidadeTotal: result.quantidadeTotal ?? prev.quantidadeTotal,
          };
        });
        setLastCommunityFeedPageFull(addedCountRef.current >= FEED_PAGE_SIZE);
      } catch {
        setLastCommunityFeedPageFull(false);
      } finally {
        setLoadingMoreCommunityFeed(false);
      }
    },
    [token, listarComunicacaoFeedUseCase, loadingCommunityFeed, loadingMoreCommunityFeed],
  );

  const hasMoreCommunityFeed = useCallback(() => lastCommunityFeedPageFull, [lastCommunityFeedPageFull]);

  const loadGrupos = useCallback(async () => {
    if (!token) return;
    setLoadingGrupos(true);
    try {
      const result = await listarComunicacaoGruposUseCase.execute(token);
      setGrupos(result ?? []);
    } catch {
      setGrupos([]);
    } finally {
      setLoadingGrupos(false);
    }
  }, [token, listarComunicacaoGruposUseCase]);

  useEffect(() => {
    if (!token) return;
    if (comunidadeId) {
      loadCommunityFeed(comunidadeId);
    } else {
      loadFeed();
      loadRequiredFeed();
    }
    loadGrupos();
  }, [token, comunidadeId, loadFeed, loadRequiredFeed, loadCommunityFeed, loadGrupos]);

  return {
    feed,
    requiredFeed,
    communityFeed,
    grupos,
    loading,
    loadingRequired,
    loadingCommunityFeed,
    loadingMoreFeed,
    loadingMoreCommunityFeed,
    loadingGrupos,
    error,
    loadFeed,
    loadMoreFeed,
    loadRequiredFeed,
    loadCommunityFeed,
    loadMoreCommunityFeed,
    hasMoreFeed,
    hasMoreCommunityFeed,
    loadGrupos,
  };
}
