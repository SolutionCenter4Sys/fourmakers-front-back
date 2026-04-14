import { useState, useCallback, useEffect, useRef } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import type { Announcement, CommunityPost } from '@domain/entities/comunicacao';
import { ListarComunicacaoFeedUseCase } from '@domain/usecases/ListarComunicacaoFeedUseCase';
import { communityPostToAnnouncement } from '@shared/utils/comunicacaoComunicadoMapper';

const ANNOUNCEMENTS_PAGE_SIZE = 15;

/** Payload base para ObterListaPublicacaoGeral na aba Comunicados. tipo vazio. */
const ANNOUNCEMENTS_FEED_PARAMS = {
  somenteLeituraObrigatoria: false,
  tipo: '' as const,
  labels: [] as string[],
  comunidadeId: '',
};

export interface UseComunicacaoAnnouncementsOptions {
  /** Só dispara o fetch quando true (ex.: aba Comunicados ativa). Evita request em outras abas. */
  enabled?: boolean;
}

/**
 * Lista comunicados na aba Comunicados (tab Todos, Histórico, Agendados).
 * POST ObterListaPublicacaoGeral com paginação; carrega mais ao fazer scroll (infinite scroll).
 */
export function useComunicacaoAnnouncements(options: UseComunicacaoAnnouncementsOptions = {}) {
  const { enabled = false } = options;
  const token = useAppSelector((state) => state.auth.token);
  const listarFeedUseCase = container.resolve(ListarComunicacaoFeedUseCase);
  const pageRef = useRef(1);

  const [posts, setPosts] = useState<CommunityPost[]>([]);
  const [announcements, setAnnouncements] = useState<Announcement[]>([]);
  const [loading, setLoading] = useState(false);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState(true);

  const load = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    pageRef.current = 1;
    try {
      const result = await listarFeedUseCase.execute(token, {
        ...ANNOUNCEMENTS_FEED_PARAMS,
        pagina: 1,
        quantidadePorPagina: ANNOUNCEMENTS_PAGE_SIZE,
      });
      const publicacoes = result.publicacoes ?? [];
      const semComunidade = publicacoes.filter((p) => !p.comunidadeId);
      setPosts(semComunidade);
      setAnnouncements(semComunidade.map(communityPostToAnnouncement));
      setHasMore(publicacoes.length >= ANNOUNCEMENTS_PAGE_SIZE);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Falha ao carregar comunicados.');
      setPosts([]);
      setAnnouncements([]);
      setHasMore(false);
    } finally {
      setLoading(false);
    }
  }, [token, listarFeedUseCase]);

  const loadMore = useCallback(async () => {
    if (!token || loading || loadingMore || !hasMore) return;
    const nextPage = pageRef.current + 1;
    setLoadingMore(true);
    try {
      const result = await listarFeedUseCase.execute(token, {
        ...ANNOUNCEMENTS_FEED_PARAMS,
        pagina: nextPage,
        quantidadePorPagina: ANNOUNCEMENTS_PAGE_SIZE,
      });
      const publicacoes = result.publicacoes ?? [];
      const semComunidade = publicacoes.filter((p) => !p.comunidadeId);
      pageRef.current = nextPage;
      setPosts((prev) => {
        const existingIds = new Set(prev.map((p) => p.id));
        const newItems = semComunidade.filter((p) => !existingIds.has(p.id));
        return [...prev, ...newItems];
      });
      setAnnouncements((prev) => {
        const existingIds = new Set(prev.map((a) => a.id));
        const newAnnouncements = semComunidade
          .filter((p) => !existingIds.has(p.id))
          .map(communityPostToAnnouncement);
        return [...prev, ...newAnnouncements];
      });
      setHasMore(publicacoes.length >= ANNOUNCEMENTS_PAGE_SIZE);
    } catch {
      setHasMore(false);
    } finally {
      setLoadingMore(false);
    }
  }, [token, listarFeedUseCase, loading, loadingMore, hasMore]);

  useEffect(() => {
    if (enabled && token) void load();
  }, [enabled, token, load]);

  return { posts, announcements, loading, loadingMore, hasMore, error, refetch: load, loadMore };
}
