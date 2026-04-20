import { useState, useMemo, useCallback, useRef, useEffect } from 'react';
import { container } from 'tsyringe';
import { X, MessageSquare, Plus, Search } from 'lucide-react';
import type { Announcement, CommunityGroup, CommunityPost, CommunityPersona } from '@domain/entities/comunicacao';
import { ArquivarPublicacaoUseCase } from '@domain/usecases/ArquivarPublicacaoUseCase';
import { ExcluirPublicacaoUseCase } from '@domain/usecases/ExcluirPublicacaoUseCase';
import { communityPostToAnnouncement } from '@shared/utils/comunicacaoComunicadoMapper';
import { PostCard } from './PostCard';
import { RequiredReadingCarousel } from './RequiredReadingCarousel';
import { CreateAnnouncementModal } from './CreateAnnouncementModal';
import { CreatePostModal } from './CreatePostModal';
import { useComunicacaoFeed } from '@presentation/hooks/useComunicacaoFeed';
import { useComunicacaoLabels } from '@presentation/hooks/useComunicacaoLabels';
import { useAppSelector } from '@app/store/hooks';
import { mockPosts, mockGroups } from '@data/mocks/comunicacao/communityData';
import { mockAnnouncements } from '@data/mocks/comunicacao/announcementsData';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Spinner } from '@/components/ui/spinner';
import { Card, CardContent } from '@/components/ui/card';
import { toast } from 'sonner';

interface CommunityFeedProps {
  persona: CommunityPersona;
  groupId?: string;
  posts?: CommunityPost[];
  setPosts?: React.Dispatch<React.SetStateAction<CommunityPost[]>>;
  showFilters?: boolean;
  compactMode?: boolean;
  /** Quando dentro de uma comunidade, chamado no empty state para abrir o modal de criar post. */
  onEmptyStateCreatePost?: () => void;
  /** Chamado ao clicar em Editar em uma publicação (abre modal de edição no parent). */
  onEditar?: (post: CommunityPost) => void;
}

export function CommunityFeed({ persona, groupId, posts: controlledPosts, setPosts: _setPosts, showFilters = true, compactMode = false, onEmptyStateCreatePost, onEditar }: CommunityFeedProps) {
  const useApiFeed = !groupId && controlledPosts === undefined;
  const token = useAppSelector((state) => state.auth.token);
  const codigoDoPerfil = useAppSelector(
    (state) => state.auth.user?.colaborador?.codigoColaboradorInterno,
  );
  const {
    feed,
    requiredFeed,
    communityFeed,
    grupos,
    loading,
    loadingRequired,
    loadingCommunityFeed,
    loadingMoreFeed,
    loadingMoreCommunityFeed,
    error,
    loadFeed,
    loadMoreFeed,
    loadRequiredFeed,
    loadCommunityFeed,
    loadMoreCommunityFeed,
    hasMoreFeed,
    hasMoreCommunityFeed,
  } = useComunicacaoFeed({ comunidadeId: groupId ?? undefined });
  const loadMoreSentinelRef = useRef<HTMLDivElement>(null);
  const loadingMore = groupId ? loadingMoreCommunityFeed : loadingMoreFeed;
  const hasMore = groupId ? hasMoreCommunityFeed() : hasMoreFeed();
  const canLoadMore = (useApiFeed || !!groupId) && hasMore && !loadingMore && !loading && !(groupId && loadingCommunityFeed);

  const { labels: labelOptions } = useComunicacaoLabels({
    enabled: useApiFeed && !groupId,
  });
  const [selectedLabels, setSelectedLabels] = useState<string[]>([]);
  const [feedSearchTerm, setFeedSearchTerm] = useState('');

  const [editPost, setEditPost] = useState<CommunityPost | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editAnnouncement, setEditAnnouncement] = useState<Announcement | null>(null);
  const [isEditAnnouncementModalOpen, setIsEditAnnouncementModalOpen] = useState(false);

  const handleEditar = useCallback(
    (post: CommunityPost) => {
      if (onEditar) {
        onEditar(post);
      } else if (useApiFeed && !groupId) {
        const isFromCommunity = !!post.comunidadeId;
        if (isFromCommunity) {
          setEditPost(post);
          setIsEditModalOpen(true);
          setEditAnnouncement(null);
          setIsEditAnnouncementModalOpen(false);
        } else {
          setEditAnnouncement(communityPostToAnnouncement(post));
          setIsEditAnnouncementModalOpen(true);
          setEditPost(null);
          setIsEditModalOpen(false);
        }
      }
    },
    [onEditar, useApiFeed, groupId],
  );

  const handleEditModalSave = useCallback(() => {
    setEditPost(null);
    setIsEditModalOpen(false);
    void loadFeed();
  }, [loadFeed]);

  const handleEditAnnouncementSave = useCallback(
    (_announcement: Announcement) => {
      setEditAnnouncement(null);
      setIsEditAnnouncementModalOpen(false);
      void loadFeed();
      void loadRequiredFeed();
    },
    [loadFeed, loadRequiredFeed],
  );

  const labelFilterInitialMount = useRef(true);
  useEffect(() => {
    if (!useApiFeed || groupId) return;
    if (labelFilterInitialMount.current) {
      labelFilterInitialMount.current = false;
      return;
    }
    void loadFeed(
      selectedLabels.length > 0 ? { labels: selectedLabels } : undefined,
    );
  }, [selectedLabels, useApiFeed, groupId, loadFeed]);

  useEffect(() => {
    if (!canLoadMore) return;
    const el = loadMoreSentinelRef.current;
    if (!el) return;
    const observer = new IntersectionObserver(
      (entries) => {
        if (!entries[0]?.isIntersecting) return;
        if (groupId) void loadMoreCommunityFeed(groupId);
        else void loadMoreFeed(selectedLabels.length > 0 ? { labels: selectedLabels } : undefined);
      },
      { rootMargin: '200px', threshold: 0 },
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, [canLoadMore, groupId, loadMoreFeed, loadMoreCommunityFeed, selectedLabels]);

  const codigoColaboradorInternoUsuarioLogado =
    codigoDoPerfil ?? feed?.codigoColaboradorInternoViewer ?? communityFeed?.codigoColaboradorInternoViewer;
  const arquivarPublicacaoUseCase = container.resolve(ArquivarPublicacaoUseCase);
  const excluirPublicacaoUseCase = container.resolve(ExcluirPublicacaoUseCase);

  const handleArquivar = useCallback(
    async (post: CommunityPost) => {
      if (!token) return;
      try {
        const res = await arquivarPublicacaoUseCase.execute(token, post.id);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Publicação arquivada com sucesso.');
          await loadFeed();
          await loadRequiredFeed();
          if (groupId) await loadCommunityFeed(groupId);
        } else {
          toast.error(res.mensagem ?? 'Não foi possível arquivar a publicação.');
        }
      } catch {
        toast.error('Erro ao arquivar. Tente novamente.');
      }
    },
    [token, groupId, arquivarPublicacaoUseCase, loadFeed, loadRequiredFeed, loadCommunityFeed],
  );

  const handleExcluir = useCallback(
    async (post: CommunityPost) => {
      if (!token) return;
      try {
        const res = await excluirPublicacaoUseCase.execute(token, post.id);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Publicação excluída.');
          await loadFeed();
          await loadRequiredFeed();
          if (groupId) await loadCommunityFeed(groupId);
        } else {
          toast.error(res.mensagem ?? 'Não foi possível excluir a publicação.');
        }
      } catch {
        toast.error('Erro ao excluir. Tente novamente.');
      }
    },
    [token, groupId, excluirPublicacaoUseCase, loadFeed, loadRequiredFeed, loadCommunityFeed],
  );

  const handleConfirmadoLeitura = useCallback(async () => {
    await loadFeed();
    await loadRequiredFeed();
    if (groupId) await loadCommunityFeed(groupId);
  }, [groupId, loadFeed, loadRequiredFeed, loadCommunityFeed]);

  const handleComentarioEnviado = useCallback(async () => {
    await loadFeed();
    await loadRequiredFeed();
    if (groupId) await loadCommunityFeed(groupId);
  }, [groupId, loadFeed, loadRequiredFeed, loadCommunityFeed]);
  const [internalPosts] = useState<CommunityPost[]>(mockPosts);
  /** No detalhe da comunidade: priorizar API (communityFeed do hook); fallback em controlledPosts (parent já recebeu da API). Nunca mock. */
  const posts = useApiFeed
    ? (feed?.publicacoes ?? [])
    : groupId
      ? (communityFeed?.publicacoes ?? controlledPosts ?? [])
      : (controlledPosts ?? internalPosts);
  let visiblePosts = posts.filter((post) => {
    if (post.status === 'published') return true;
    if (
      post.status === 'scheduled' &&
      (persona === 'manager' || persona === 'analytics')
    )
      return true;
    return false;
  });

  // No detalhe da comunidade o feed já vem da API com comunidadeId; não filtrar de novo por groupId.
  if (groupId && useApiFeed) {
    visiblePosts = visiblePosts.filter((post) => post.groupId === groupId);
  }

  const availableGroups = useMemo(() => {
    if (useApiFeed && grupos.length > 0) {
      return grupos.map((g) => ({ id: g.id, name: g.nome }));
    }
    if (useApiFeed && posts.length > 0) {
      const seen = new Set<string>();
      return posts
        .filter((p) => p.groupId && p.status === 'published')
        .filter((p) => {
          if (seen.has(p.groupId)) return false;
          seen.add(p.groupId);
          return true;
        })
        .map((p) => ({ id: p.groupId, name: p.groupName }));
    }
    return mockGroups.filter((group) =>
      posts.some(
        (post) => post.groupId === group.id && post.status === 'published',
      ),
    );
  }, [useApiFeed, grupos, posts]);

  const editGroup = useMemo((): CommunityGroup | null => {
    if (!editPost || !isEditModalOpen || !editPost.comunidadeId) return null;
    const id = editPost.comunidadeId;
    const found = availableGroups.find((g) => g.id === id);
    const name = found?.name ?? editPost.comunidadeNome ?? editPost.groupName ?? '';
    return {
      id,
      name,
      description: '',
      type: 'free',
      creatorId: '',
      creatorName: '',
      moderators: [],
      status: 'active',
      createdAt: '',
      memberCount: 0,
      postCount: 0,
      settings: { allowFreeEntry: true, allowMemberPosts: true, allowMemberLeave: true, requiresApproval: false, commentsEnabledByDefault: true, likesEnabledByDefault: true },
      linkedUserGroups: [],
    };
  }, [editPost, isEditModalOpen, availableGroups]);

  const toggleLabelFilter = (nome: string) => {
    setSelectedLabels((prev) =>
      prev.includes(nome) ? prev.filter((n) => n !== nome) : [...prev, nome],
    );
  };

  const clearFilters = () => {
    setSelectedLabels([]);
  };

  const publishedAnnouncements = mockAnnouncements.filter((a) => {
    if (a.status !== 'published') return false;
    if (persona === 'manager' || persona === 'analytics') return true;
    if (a.expiresAt) {
      const expiryDate = new Date(a.expiresAt);
      const now = new Date();
      return now <= expiryDate;
    }
    return true;
  });

  const pendingAcknowledgmentPosts = visiblePosts.filter(
    (post) => post.requiresAcknowledgment && post.status === 'published',
  );
  const requiredAnnouncements = publishedAnnouncements.filter(
    (a) => a.requiresAcknowledgment,
  );
  const requiredItemsFromMock = [
    ...pendingAcknowledgmentPosts.map((post) => ({
      ...post,
      itemType: 'post' as const,
    })),
    ...requiredAnnouncements.map((announcement) => ({
      ...announcement,
      itemType: 'announcement' as const,
    })),
  ];
  const requiredPublicacoes = requiredFeed?.publicacoes ?? [];
  const pendingRequired = requiredPublicacoes.filter((p) => !p.acknowledgedAt);
  const acknowledgedRequired = requiredPublicacoes.filter((p) => !!p.acknowledgedAt);
  const requiredItems = useApiFeed
    ? pendingRequired.map((p) => ({ ...p, itemType: 'post' as const }))
    : requiredItemsFromMock;
  const acknowledgedItems = useApiFeed
    ? acknowledgedRequired.map((p) => ({ ...p, itemType: 'post' as const }))
    : [];

  const nonRequiredAnnouncements = publishedAnnouncements.filter(
    (a) => !a.requiresAcknowledgment,
  );
  const feedPosts = visiblePosts.filter((post) => !post.requiresAcknowledgment);

  /** Feed lista apenas publicações sem leitura obrigatória; as com requiresAcknowledgment ficam só na seção Documentos/Comunicados - Leitura Obrigatória. */
  const feedItems =
    useApiFeed || groupId
      ? visiblePosts.filter((post) => !post.requiresAcknowledgment)
      : [
          ...feedPosts,
          ...nonRequiredAnnouncements.map((announcement) => ({
          id: announcement.id,
          groupId: announcement.targetUserGroupIds[0] || '',
          groupName: announcement.targetUserGroupNames[0] || 'Comunicado',
          authorId: announcement.authorId,
          authorName: announcement.authorName,
          authorAvatar: announcement.authorAvatar,
          authorArea: announcement.authorArea,
          type: 'text' as const,
          title: announcement.title,
          content: announcement.content,
          attachments: announcement.attachments,
          status: announcement.status,
          publishedAt: announcement.publishedAt,
          visibility: { type: 'all' as const },
          requiresAcknowledgment: announcement.requiresAcknowledgment,
          allowComments: announcement.allowComments,
          allowLikes: announcement.allowLikes,
          isPinned: announcement.isPinned,
          likesCount: announcement.likesCount,
          commentsCount: announcement.commentsCount,
          viewsCount: announcement.viewsCount,
          acknowledgmentCount: announcement.acknowledgmentCount,
          createdAt: announcement.createdAt,
          updatedAt: announcement.updatedAt,
        })),
        ];

  const stripHtml = (html: string) =>
    (html || '').replace(/<[^>]*>/g, ' ').replace(/\s+/g, ' ').trim();

  const filteredBySearch = useMemo(() => {
    if (!feedSearchTerm.trim()) return feedItems;
    const q = feedSearchTerm.toLowerCase().trim();
    return feedItems.filter((item) => {
      const title = (item.title ?? '').toLowerCase();
      const content = stripHtml(item.content ?? '').toLowerCase();
      const author = (item.authorName ?? '').toLowerCase();
      const group = (item.groupName ?? '').toLowerCase();
      return title.includes(q) || content.includes(q) || author.includes(q) || group.includes(q);
    });
  }, [feedItems, feedSearchTerm]);

  const sortedFeedItems = useMemo(() => {
    return [...filteredBySearch].sort((a, b) => {
      if (a.isPinned && !b.isPinned) return -1;
      if (!a.isPinned && b.isPinned) return 1;
      const dateA = new Date(a.publishedAt || a.createdAt);
      const dateB = new Date(b.publishedAt || b.createdAt);
      return dateB.getTime() - dateA.getTime();
    });
  }, [filteredBySearch]);

  return (
    <div className="space-y-6">
      {(useApiFeed && loading) || (groupId && loadingCommunityFeed) ? (
        <div className="flex justify-center py-12">
          <Spinner className="h-8 w-8 text-muted-foreground" />
        </div>
      ) : null}
      {useApiFeed && error && (
        <div className="rounded-lg border border-destructive/50 bg-destructive/10 px-4 py-3 text-sm text-destructive">
          {error}
        </div>
      )}
      {(!useApiFeed || !loading) && !(groupId && loadingCommunityFeed) && (
        <>
          {!groupId && (useApiFeed || requiredItems.length > 0 || acknowledgedItems.length > 0) && (
            <RequiredReadingCarousel
              items={requiredItems}
              acknowledgedItems={acknowledgedItems}
              loading={useApiFeed && loadingRequired}
              onConfirmadoLeitura={useApiFeed ? loadRequiredFeed : undefined}
            />
          )}
          {groupId && showFilters && (
            <div className="flex items-center gap-3 min-w-0">
              <div className="flex-shrink-0 w-full sm:w-auto sm:min-w-[200px] sm:max-w-[260px]">
                <div className="relative">
                  <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
                  <Input
                    type="search"
                    placeholder="Buscar no feed..."
                    value={feedSearchTerm}
                    onChange={(e) => setFeedSearchTerm(e.target.value)}
                    className="h-8 w-full pl-8 pr-3 rounded-lg border border-border bg-surfaceElevated text-sm text-foreground placeholder:text-muted-foreground focus-visible:border-primary"
                  />
                </div>
              </div>
            </div>
          )}
          {!groupId && showFilters && (
            <div className="flex items-center gap-3 min-w-0 flex-wrap">
              {labelOptions.length > 0 && (
                <>
                  <span className="text-sm font-medium text-muted-foreground flex-shrink-0">
                    Filtro por tipo:
                  </span>
                  <div className="flex items-center gap-2 min-w-0 flex-1 basis-0">
                    <div className="rounded-lg border border-border/40 bg-muted/20 shadow-sm overflow-hidden max-w-[280px] min-h-[32px] flex items-stretch">
                      <div className="overflow-x-auto overflow-y-hidden flex items-center gap-1.5 py-1 px-1.5 [&::-webkit-scrollbar]:h-1.5 [&::-webkit-scrollbar]:w-1.5">
                        {labelOptions.map((label) => {
                          const isActive = selectedLabels.includes(label.nome);
                          return (
                            <Button
                              key={label.id}
                              variant="ghost"
                              size="sm"
                              className={`flex-shrink-0 rounded-md py-1.5 px-2.5 text-xs font-medium transition-all duration-200 h-7 ${
                                isActive
                                  ? 'bg-primary text-primary-foreground hover:bg-primary hover:text-primary-foreground'
                                  : 'text-muted-foreground hover:text-foreground hover:bg-muted/50'
                              }`}
                              onClick={() => toggleLabelFilter(label.nome)}
                            >
                              {label.nome}
                            </Button>
                          );
                        })}
                      </div>
                    </div>
                    {selectedLabels.length > 0 && (
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={clearFilters}
                        className="flex-shrink-0 h-7 rounded-md gap-1 text-xs text-muted-foreground hover:text-foreground hover:bg-muted/50 px-2"
                      >
                        <X className="w-3 h-3" />
                        Limpar
                      </Button>
                    )}
                  </div>
                </>
              )}
              <div className="flex-shrink-0 w-full sm:w-auto sm:min-w-[200px] sm:max-w-[260px]">
                <div className="relative">
                  <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
                  <Input
                    type="search"
                    placeholder="Buscar no feed..."
                    value={feedSearchTerm}
                    onChange={(e) => setFeedSearchTerm(e.target.value)}
                    className="h-8 w-full pl-8 pr-3 rounded-lg border border-border bg-surfaceElevated text-sm text-foreground placeholder:text-muted-foreground focus-visible:border-primary"
                  />
                </div>
              </div>
            </div>
          )}
          <div className="space-y-4">
            {sortedFeedItems.length === 0 ? (
              <Card className="overflow-hidden border-dashed border-2 border-muted/60 bg-muted/20">
                <CardContent className="flex flex-col items-center justify-center py-16 px-6 text-center">
                  <div className="flex h-20 w-20 items-center justify-center rounded-full bg-primary/10 text-primary mb-4">
                    <MessageSquare className="h-10 w-10" strokeWidth={1.5} />
                  </div>
                  <h3 className="text-lg font-semibold text-foreground mb-1">
                    {feedSearchTerm.trim()
                      ? 'Nenhum resultado para esta busca'
                      : selectedLabels.length > 0
                        ? 'Nenhuma publicação com esses filtros'
                        : groupId
                          ? 'Nenhuma publicação nesta comunidade'
                          : 'Nenhuma publicação disponível'}
                  </h3>
                  <p className="text-muted-foreground text-sm max-w-sm mb-6">
                    {feedSearchTerm.trim()
                      ? 'Tente outros termos na busca ou limpe o campo para ver todo o feed.'
                      : selectedLabels.length > 0
                        ? 'Tente ajustar os filtros acima para ver outras publicações.'
                        : groupId
                          ? 'Seja o primeiro a compartilhar uma novidade, dúvida ou ideia com o grupo.'
                          : 'As publicações aparecerão aqui quando forem criadas.'}
                  </p>
                  {groupId && onEmptyStateCreatePost && (
                    <Button
                      onClick={onEmptyStateCreatePost}
                      className="gap-2 rounded-lg"
                    >
                      <Plus className="h-4 w-4" />
                      Criar primeira publicação
                    </Button>
                  )}
                </CardContent>
              </Card>
            ) : (
              <>
                {sortedFeedItems.map((item, index) => (
                  <div
                    key={item.id}
                    className="transition-opacity"
                    style={{ animationDelay: `${index * 50}ms` }}
                  >
                    <PostCard
                      post={item as CommunityPost}
                      persona={persona}
                      compact={compactMode}
                      codigoColaboradorInternoUsuarioLogado={codigoColaboradorInternoUsuarioLogado}
                      onArquivar={useApiFeed || groupId ? handleArquivar : undefined}
                      onExcluir={useApiFeed || groupId ? handleExcluir : undefined}
                      onEditar={useApiFeed || groupId ? handleEditar : undefined}
                      onConfirmadoLeitura={useApiFeed || groupId ? handleConfirmadoLeitura : undefined}
                      onComentarioEnviado={useApiFeed || groupId ? handleComentarioEnviado : undefined}
                    />
                  </div>
                ))}
                {(useApiFeed || groupId) && (
                  <div ref={loadMoreSentinelRef} className="min-h-[40px] flex items-center justify-center py-4">
                    {loadingMore && (
                      <Spinner className="h-6 w-6 text-muted-foreground" />
                    )}
                  </div>
                )}
              </>
            )}
          </div>
        </>
      )}
      {useApiFeed && !groupId && editGroup && editPost && (
        <CreatePostModal
          open={isEditModalOpen}
          onOpenChange={(open) => {
            setIsEditModalOpen(open);
            if (!open) setEditPost(null);
          }}
          group={editGroup}
          editPost={editPost}
          onSave={handleEditModalSave}
        />
      )}
      {useApiFeed && !groupId && editAnnouncement && (
        <CreateAnnouncementModal
          open={isEditAnnouncementModalOpen}
          onOpenChange={(open) => {
            setIsEditAnnouncementModalOpen(open);
            if (!open) setEditAnnouncement(null);
          }}
          initialAnnouncement={editAnnouncement}
          onSave={handleEditAnnouncementSave}
        />
      )}
    </div>
  );
}
