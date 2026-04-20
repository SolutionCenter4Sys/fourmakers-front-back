import { useState, useCallback, useRef, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Plus,
  Megaphone,
  CalendarClock,
  Search,
  Users,
  Send,
  Archive,
  CircleCheck,
  ChevronDown,
  ChevronUp,
  SlidersHorizontal,
  CheckCircle,
  XCircle,
  Eye,
  MoreHorizontal,
  Pencil,
  Trash2,
} from 'lucide-react';
import type { Announcement, CommunityPersona, CommunityPost, UserGroup } from '@domain/entities/comunicacao';
import { AprovarPublicacaoUseCase } from '@domain/usecases/AprovarPublicacaoUseCase';
import { ArquivarPublicacaoUseCase } from '@domain/usecases/ArquivarPublicacaoUseCase';
import { ExcluirPublicacaoUseCase } from '@domain/usecases/ExcluirPublicacaoUseCase';
import { PublicarAgoraPublicacaoUseCase } from '@domain/usecases/PublicarAgoraPublicacaoUseCase';
import { ObterConfirmacoesLeituraUseCase } from '@domain/usecases/ObterConfirmacoesLeituraUseCase';
import { RejeitarPublicacaoUseCase } from '@domain/usecases/RejeitarPublicacaoUseCase';
import { useAppSelector } from '@app/store/hooks';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Textarea } from '@/components/ui/textarea';
import { Spinner } from '@/components/ui/spinner';
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible';
import { DataTable, type Column } from '@presentation/components/common';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { CreateAnnouncementModal } from './CreateAnnouncementModal';
import { AnnouncementAcknowledgmentsModal } from './AnnouncementAcknowledgmentsModal';
import { RequiredItemDetailModal } from './RequiredItemDetailModal';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';

interface AnnouncementsViewProps {
  persona: CommunityPersona;
  /** Lista de publicações no formato do feed (para exibir com PostCard). */
  posts?: CommunityPost[];
  announcements?: Announcement[];
  setAnnouncements?: React.Dispatch<React.SetStateAction<Announcement[]>>;
  userGroups?: UserGroup[];
  loading?: boolean;
  /** Carregando mais itens (infinite scroll). */
  loadingMore?: boolean;
  /** Ainda há mais itens para carregar. */
  hasMore?: boolean;
  /** Chamado ao chegar no fim da lista (carregar mais). */
  onLoadMore?: () => void | Promise<void>;
  error?: string | null;
  onRefetch?: () => void | Promise<void>;
  /** Exibe botão "Novo Comunicado" e ações de editar/excluir quando true (PermissoesUsuarioLogado.permiteCriarPublicacaoInformativo). */
  permiteCriarPublicacaoInformativo?: boolean;
  /** Exibe ação "Aprovar" em publicações pendentes quando true (PermissoesUsuarioLogado.aprovaPublicacaoInformativo). */
  aprovaPublicacaoInformativo?: boolean;
}

export function AnnouncementsView({
  persona,
  posts: propsPosts = [],
  announcements: propsAnnouncements,
  setAnnouncements: setPropsAnnouncements,
  userGroups = [],
  loading = false,
  loadingMore = false,
  hasMore = false,
  onLoadMore,
  error: errorProp = null,
  onRefetch,
  permiteCriarPublicacaoInformativo = false,
  aprovaPublicacaoInformativo = false,
}: AnnouncementsViewProps) {
  const loadMoreSentinelRef = useRef<HTMLDivElement>(null);
  const [internalAnnouncements, setInternalAnnouncements] = useState<Announcement[]>([]);
  const announcements = propsAnnouncements ?? internalAnnouncements;
  const setAnnouncements = setPropsAnnouncements ?? setInternalAnnouncements;
  const posts = propsPosts ?? [];
  const canLoadMore = hasMore && !loading && !loadingMore && !!onLoadMore;
  type TabFiltro = 'todos' | 'ativos' | 'agendados' | 'arquivados';
  const [activeTab, setActiveTab] = useState<TabFiltro>('todos');
  const [searchTerm, setSearchTerm] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [selectedMonth, setSelectedMonth] = useState('');
  const [selectedYear, setSelectedYear] = useState('');
  const [filterTipo, setFilterTipo] = useState<'' | 'informativo' | 'documento'>('');
  const [filterRequerLeitura, setFilterRequerLeitura] = useState<'' | 'sim' | 'nao'>('');
  const [filtrosOpen, setFiltrosOpen] = useState(false);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [announcementToEdit, setAnnouncementToEdit] = useState<Announcement | null>(null);
  const [selectedPostForAck, setSelectedPostForAck] = useState<CommunityPost | null>(null);
  const [isAckModalOpen, setIsAckModalOpen] = useState(false);
  const [announcementToDelete, setAnnouncementToDelete] = useState<Announcement | null>(null);
  const [publicandoAgoraId, setPublicandoAgoraId] = useState<string | null>(null);
  const [aprovacaoActionId, setAprovacaoActionId] = useState<string | null>(null);
  const [postToReject, setPostToReject] = useState<CommunityPost | null>(null);
  const [motivoRejeicao, setMotivoRejeicao] = useState('');
  const [postForPreview, setPostForPreview] = useState<CommunityPost | null>(null);
  type AgendadosSubTab = 'todos' | 'agendados' | 'aprovacao';
  const [agendadosSubTab, setAgendadosSubTab] = useState<AgendadosSubTab>('todos');

  const token = useAppSelector((state) => state.auth.token);
  const aprovarPublicacaoUseCase = container.resolve(AprovarPublicacaoUseCase);
  const arquivarPublicacaoUseCase = container.resolve(ArquivarPublicacaoUseCase);
  const excluirPublicacaoUseCase = container.resolve(ExcluirPublicacaoUseCase);
  const publicarAgoraPublicacaoUseCase = container.resolve(PublicarAgoraPublicacaoUseCase);
  const rejeitarPublicacaoUseCase = container.resolve(RejeitarPublicacaoUseCase);
  const obterConfirmacoesLeituraUseCase = container.resolve(ObterConfirmacoesLeituraUseCase);

  const [ackData, setAckData] = useState<{
    totalRecipients: number;
    acknowledgments: import('@domain/entities/comunicacao').ConfirmacaoLeitura[];
  } | null>(null);
  const [loadingAck, setLoadingAck] = useState(false);

  const canCreate = permiteCriarPublicacaoInformativo;
  const canApprove = aprovaPublicacaoInformativo;
  const canViewAcknowledgments = persona === 'manager' || persona === 'analytics';
  const showHistoryTab = canCreate;

  useEffect(() => {
    if (!canLoadMore || !onLoadMore) return;
    const el = loadMoreSentinelRef.current;
    if (!el) return;
    const observer = new IntersectionObserver(
      (entries) => {
        if (!entries[0]?.isIntersecting) return;
        void onLoadMore();
      },
      { rootMargin: '200px', threshold: 0 },
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, [canLoadMore, onLoadMore]);

  const handleViewAcknowledgments = async (post: CommunityPost) => {
    if (!token || !post.requiresAcknowledgment) return;
    setSelectedPostForAck(post);
    setAckData(null);
    setLoadingAck(true);
    try {
      const result = await obterConfirmacoesLeituraUseCase.execute(token, post.id);
      setAckData({
        totalRecipients: result.totalDestinatarios,
        acknowledgments: result.confirmacoes,
      });
      setIsAckModalOpen(true);
    } catch {
      toast.error('Não foi possível carregar os aceites.');
    } finally {
      setLoadingAck(false);
    }
  };

  const handleArquivarPost = useCallback(
    async (post: CommunityPost) => {
      const ann = announcements.find((a) => a.id === post.id);
      if (ann) await handleArchive(ann);
    },
    [announcements],
  );

  const handlePublicarAgora = useCallback(
    async (post: CommunityPost) => {
      if (!token) return;
      setPublicandoAgoraId(post.id);
      try {
        const res = await publicarAgoraPublicacaoUseCase.execute(token, post.id);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Publicação publicada com sucesso.');
          await onRefetch?.();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível publicar agora.');
        }
      } catch {
        toast.error('Erro ao publicar. Tente novamente.');
      } finally {
        setPublicandoAgoraId(null);
      }
    },
    [token, publicarAgoraPublicacaoUseCase, onRefetch],
  );

  const handleAprovar = useCallback(
    async (post: CommunityPost) => {
      if (!token) return;
      setAprovacaoActionId(post.id);
      try {
        const res = await aprovarPublicacaoUseCase.execute(token, post.id);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Comunicado aprovado.');
          await onRefetch?.();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível aprovar.');
        }
      } catch {
        toast.error('Erro ao aprovar. Tente novamente.');
      } finally {
        setAprovacaoActionId(null);
      }
    },
    [token, aprovarPublicacaoUseCase, onRefetch],
  );

  const handleRejeitar = useCallback(
    async (post: CommunityPost, justificativa: string) => {
      if (!token) return;
      const motivo = justificativa.trim();
      if (!motivo) {
        toast.error('Informe a justificativa da rejeição.');
        return;
      }
      setAprovacaoActionId(post.id);
      try {
        const res = await rejeitarPublicacaoUseCase.execute(token, post.id, motivo);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Comunicado rejeitado.');
          setPostToReject(null);
          setMotivoRejeicao('');
          await onRefetch?.();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível rejeitar.');
        }
      } catch {
        toast.error('Erro ao rejeitar. Tente novamente.');
      } finally {
        setAprovacaoActionId(null);
      }
    },
    [token, rejeitarPublicacaoUseCase, onRefetch],
  );

  const openRejectModal = useCallback((post: CommunityPost) => {
    setPostToReject(post);
    setMotivoRejeicao('');
  }, []);

  const closeRejectModal = useCallback(() => {
    setPostToReject(null);
    setMotivoRejeicao('');
  }, []);

  const handleArchive = async (announcement: Announcement) => {
    const id = announcement.id;
    if (onRefetch && token) {
      try {
        const res = await arquivarPublicacaoUseCase.execute(token, id);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Comunicado arquivado com sucesso.');
          await onRefetch();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível arquivar o comunicado.');
        }
      } catch {
        toast.error('Erro ao arquivar. Tente novamente.');
      }
    } else {
      const now = new Date().toISOString();
      setAnnouncements((prev) =>
        prev.map((a) =>
          a.id === id ? { ...a, status: 'archived' as const, updatedAt: now } : a,
        ),
      );
    }
  };

  const handleDelete = async (announcement: Announcement) => {
    const id = announcement.id;
    if (onRefetch && token) {
      try {
        const res = await excluirPublicacaoUseCase.execute(token, id);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Comunicado excluído.');
          await onRefetch();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível excluir o comunicado.');
        }
      } catch {
        toast.error('Erro ao excluir. Tente novamente.');
      }
    } else {
      const now = new Date().toISOString();
      setAnnouncements((prev) =>
        prev.map((a) =>
          a.id === id ? { ...a, status: 'deleted' as const, updatedAt: now } : a,
        ),
      );
    }
  };

  const handleConfirmDelete = () => {
    const toDelete = announcementToDelete;
    if (!toDelete) return;
    setAnnouncementToDelete(null);
    void handleDelete(toDelete);
  };

  /** Filtro por publicacaoStatus: ativa -> published, agendada -> scheduled; arquivados = arquivadas OU expiradas */
  const STATUS_BY_TAB: Record<TabFiltro, Announcement['status'] | null> = {
    todos: null,
    ativos: 'published',
    agendados: 'scheduled',
    arquivados: 'archived',
  };
  const postStatusByTab: Record<TabFiltro, CommunityPost['status'] | null> = {
    todos: null,
    ativos: 'published',
    agendados: 'scheduled',
    arquivados: 'archived',
  };

  const isExpirada = (expiresAt?: string | null) =>
    !!expiresAt && new Date(expiresAt).getTime() < Date.now();

  let visibleAnnouncements = announcements.filter((announcement) => {
    if (!showHistoryTab && announcement.status !== 'published') return false;
    if (activeTab === 'arquivados') {
      return announcement.status === 'archived' || isExpirada(announcement.expiresAt);
    }
    const statusFilter = STATUS_BY_TAB[activeTab];
    if (statusFilter === null) return true;
    return announcement.status === statusFilter;
  });

  let visiblePosts = posts.filter((post) => {
    if (!showHistoryTab && post.status !== 'published') return false;
    if (activeTab === 'arquivados') {
      return post.status === 'archived' || isExpirada(post.expiresAt);
    }
    const statusFilter = postStatusByTab[activeTab];
    if (statusFilter === null) return true;
    return post.status === statusFilter;
  });

  if (searchTerm) {
    const term = searchTerm.toLowerCase();
    visibleAnnouncements = visibleAnnouncements.filter(
      (a) =>
        a.title.toLowerCase().includes(term) ||
        a.targetUserGroupNames.some((name) => name.toLowerCase().includes(term)),
    );
    visiblePosts = visiblePosts.filter(
      (p) =>
        p.title.toLowerCase().includes(term) ||
        (p.groupName && p.groupName.toLowerCase().includes(term)),
    );
  }

  if (filterTipo) {
    visiblePosts = visiblePosts.filter((p) =>
      filterTipo === 'documento' ? p.type === 'document' : p.type !== 'document',
    );
    visibleAnnouncements = visibleAnnouncements.filter((a) => {
      const post = posts.find((x) => x.id === a.id);
      if (!post) return true;
      return filterTipo === 'documento' ? post.type === 'document' : post.type !== 'document';
    });
  }

  if (filterRequerLeitura) {
    const requer = filterRequerLeitura === 'sim';
    visiblePosts = visiblePosts.filter((p) => p.requiresAcknowledgment === requer);
    visibleAnnouncements = visibleAnnouncements.filter((a) => a.requiresAcknowledgment === requer);
  }

  if (startDate || endDate || selectedMonth || selectedYear) {
    const filterByDate = (d: Date) => {
      if (selectedYear && d.getFullYear() !== parseInt(selectedYear)) return false;
      if (selectedMonth && d.getMonth() !== parseInt(selectedMonth)) return false;
      if (startDate && new Date(startDate) > d) return false;
      if (endDate && new Date(endDate) < d) return false;
      return true;
    };
    visibleAnnouncements = visibleAnnouncements.filter((a) =>
      filterByDate(new Date(a.publishedAt || a.createdAt)),
    );
    visiblePosts = visiblePosts.filter((p) =>
      filterByDate(new Date(p.publishedAt ?? p.createdAt ?? 0)),
    );
  }

  visibleAnnouncements = [...visibleAnnouncements].sort((a, b) => {
    if (a.isPinned && !b.isPinned) return -1;
    if (!a.isPinned && b.isPinned) return 1;
    return (
      new Date(b.publishedAt || b.createdAt).getTime() -
      new Date(a.publishedAt || a.createdAt).getTime()
    );
  });

  const sortedVisiblePosts = [...visiblePosts].sort((a, b) => {
    if (a.isPinned && !b.isPinned) return -1;
    if (!a.isPinned && b.isPinned) return 1;
    const dateA = new Date(a.publishedAt ?? a.createdAt ?? 0).getTime();
    const dateB = new Date(b.publishedAt ?? b.createdAt ?? 0).getTime();
    return dateB - dateA;
  });

  /** Posts para a aba Agendados/Aprova: scheduled + pending_approval, filtrados pela sub-aba. */
  const agendadosPosts = posts.filter(
    (p) => p.status === 'scheduled' || p.status === 'pending_approval',
  );
  const agendadosFilteredBySubTab =
    agendadosSubTab === 'todos'
      ? agendadosPosts
      : agendadosSubTab === 'agendados'
        ? agendadosPosts.filter((p) => p.status === 'scheduled')
        : agendadosPosts.filter((p) => p.status === 'pending_approval');
  const sortedAgendadosPosts = [...agendadosFilteredBySubTab].sort((a, b) => {
    if (a.isPinned && !b.isPinned) return -1;
    if (!a.isPinned && b.isPinned) return 1;
    const dateA = new Date(a.publishedAt ?? a.scheduledAt ?? a.createdAt ?? 0).getTime();
    const dateB = new Date(b.publishedAt ?? b.scheduledAt ?? b.createdAt ?? 0).getTime();
    return dateB - dateA;
  });

  const statusLabels: Record<CommunityPost['status'], string> = {
    draft: 'Rascunho',
    scheduled: 'Agendado',
    published: 'Publicado',
    pending_approval: 'Aguardando aprovação',
    archived: 'Arquivado',
  };

  const announcementsTableColumns: Column[] = [
    { id: 'title', label: 'Título', width: 'min-w-[200px]', sortable: true },
    { id: 'content', label: 'Conteúdo', width: 'min-w-[200px]', sortable: false },
    { id: 'authorName', label: 'Autor', width: 'w-[180px]', sortable: true },
    { id: 'type', label: 'Tipo comunicado', width: 'w-[130px]', sortable: true },
    { id: 'status', label: 'Status', width: 'w-[140px]', sortable: true },
    { id: 'requiresAcknowledgment', label: 'Requer leitura', width: 'w-[120px]', sortable: true },
    { id: 'ocultarNoFeed', label: 'Oculto no feed', width: 'w-[120px]', sortable: true },
    { id: 'aceites', label: 'Aceites', width: 'w-[140px]', align: 'center', sortable: false },
    { id: 'attachmentsCount', label: 'Anexos', width: 'w-[90px]', align: 'center', sortable: false },
    { id: 'publishedAt', label: 'Data', width: 'w-[120px]', sortable: true },
    { id: 'acoes', label: 'Ações', width: 'w-[140px]', align: 'center' },
  ];

  const stripHtml = (html: string) => {
    if (!html || typeof html !== 'string') return '';
    const tmp = document.createElement('div');
    tmp.innerHTML = html;
    return (tmp.textContent ?? tmp.innerText ?? '').trim();
  };

  /** Trunca texto ao limite de caracteres; evita quebra em duas linhas. Detalhes no Preview. */
  const truncateCell = (text: string, maxChars: number) => {
    if (!text) return '—';
    return text.length > maxChars ? text.slice(0, maxChars) + '…' : text;
  };

  const cellSingleLine = 'truncate block whitespace-nowrap overflow-hidden max-w-full';

  const tipoComunicadoLabel = (type: CommunityPost['type']) =>
    type === 'document' ? 'Documento' : 'Informativo';

  const MAX_CHARS = {
    title: 40,
    content: 50,
    authorName: 28,
    type: 12,
    status: 22,
    requiresAcknowledgment: 3,
    date: 10,
  } as const;

  const renderAnnouncementCell = (post: CommunityPost, columnId: string) => {
    switch (columnId) {
      case 'title': {
        const t = truncateCell(post.title ?? '', MAX_CHARS.title);
        return <span className={`font-medium ${cellSingleLine}`} title={post.title ?? undefined}>{t}</span>;
      }
      case 'content': {
        const raw = stripHtml(post.content ?? '');
        const t = truncateCell(raw, MAX_CHARS.content);
        return <span className={`text-sm text-muted-foreground ${cellSingleLine}`} title={raw || undefined}>{t || '—'}</span>;
      }
      case 'authorName': {
        const t = truncateCell(post.authorName ?? '', MAX_CHARS.authorName);
        return <span className={`text-muted-foreground text-sm ${cellSingleLine}`} title={post.authorName ?? undefined}>{t}</span>;
      }
      case 'type': {
        const label = tipoComunicadoLabel(post.type);
        const t = truncateCell(label, MAX_CHARS.type);
        return <span className={`text-sm ${cellSingleLine}`} title={label}>{t}</span>;
      }
      case 'status': {
        const label = statusLabels[post.status] ?? post.status;
        const t = truncateCell(String(label), MAX_CHARS.status);
        return <span className={`text-sm ${cellSingleLine}`} title={String(label)}>{t}</span>;
      }
      case 'requiresAcknowledgment': {
        const label = post.requiresAcknowledgment ? 'Sim' : 'Não';
        return <span className={`text-sm ${cellSingleLine}`}>{label}</span>;
      }
      case 'ocultarNoFeed': {
        const label = post.ocultarNoFeed ? 'Sim' : 'Não';
        return <span className={`text-sm ${cellSingleLine}`}>{label}</span>;
      }
      case 'aceites':
        if (!post.requiresAcknowledgment) return <span className={`text-sm ${cellSingleLine} text-muted-foreground`}>—</span>;
        return (
          <div className="flex items-center justify-center gap-1.5 flex-wrap">
            <span className={`text-sm tabular-nums ${cellSingleLine}`}>{post.acknowledgmentCount ?? 0}</span>
            {canViewAcknowledgments && (post.status === 'published' || post.status === 'archived') && (
              <Button
                variant="ghost"
                size="sm"
                className="h-7 px-2 text-xs"
                onClick={(e) => { e.stopPropagation(); void handleViewAcknowledgments(post); }}
                disabled={loadingAck && selectedPostForAck?.id === post.id}
              >
                {loadingAck && selectedPostForAck?.id === post.id ? (
                  <Spinner className="w-3.5 h-3.5" />
                ) : (
                  'Ver aceites'
                )}
              </Button>
            )}
          </div>
        );
      case 'attachmentsCount':
        return <span className={`text-sm tabular-nums ${cellSingleLine}`}>{(post.attachments?.length ?? 0)}</span>;
      case 'publishedAt': {
        const raw = post.publishedAt ?? post.scheduledAt ?? post.createdAt;
        const d = raw ? new Date(raw) : null;
        const label = d && !Number.isNaN(d.getTime()) ? format(d, 'dd/MM/yyyy', { locale: ptBR }) : '—';
        const t = truncateCell(label, MAX_CHARS.date);
        return <span className={`text-sm ${cellSingleLine}`} title={label}>{t}</span>;
      }
      case 'acoes':
        return (
          <div className="flex items-center justify-center gap-1">
            <Button
              variant="ghost"
              size="icon"
              className="h-8 w-8 shrink-0"
              onClick={(e) => { e.stopPropagation(); setPostForPreview(post); }}
              title="Preview"
            >
              <Eye className="w-4 h-4" />
            </Button>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="h-8 w-8 shrink-0" onClick={(e) => e.stopPropagation()}>
                  <MoreHorizontal className="w-4 h-4" />
                </Button>
              </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-48">
              {canCreate && (
                <DropdownMenuItem onClick={() => { const ann = announcements.find((a) => a.id === post.id); if (ann) { setAnnouncementToEdit(ann); setIsCreateModalOpen(true); } }}>
                  <Pencil className="w-4 h-4 mr-2" />
                  Editar
                </DropdownMenuItem>
              )}
              {post.status === 'draft' && (
                <DropdownMenuItem
                  disabled={publicandoAgoraId === post.id}
                  onClick={() => handlePublicarAgora(post)}
                >
                  {publicandoAgoraId === post.id ? <Spinner className="w-4 h-4 mr-2" /> : <Send className="w-4 h-4 mr-2" />}
                  Publicar agora
                </DropdownMenuItem>
              )}
              {post.status === 'pending_approval' && (
                <>
                  {canApprove && (
                    <DropdownMenuItem disabled={aprovacaoActionId !== null} onClick={() => handleAprovar(post)}>
                      {aprovacaoActionId === post.id ? <Spinner className="w-4 h-4 mr-2" /> : <CheckCircle className="w-4 h-4 mr-2" />}
                      {post.scheduledAt ? 'Aprovar' : 'Aprovar e publicar'}
                    </DropdownMenuItem>
                  )}
                  <DropdownMenuItem disabled={aprovacaoActionId !== null} onClick={() => openRejectModal(post)} className="text-destructive">
                    <XCircle className="w-4 h-4 mr-2" />
                    Rejeitar
                  </DropdownMenuItem>
                </>
              )}
              {post.status === 'published' && (
                <DropdownMenuItem onClick={() => handleArquivarPost(post)}>
                  <Archive className="w-4 h-4 mr-2" />
                  Arquivar
                </DropdownMenuItem>
              )}
              {canCreate && (
                <DropdownMenuItem
                  onClick={() => setAnnouncementToDelete(announcements.find((a) => a.id === post.id) ?? null)}
                  className="text-destructive"
                >
                  <Trash2 className="w-4 h-4 mr-2" />
                  Excluir
                </DropdownMenuItem>
              )}
              {canViewAcknowledgments && post.requiresAcknowledgment && (post.status === 'published' || post.status === 'archived') && (
                <DropdownMenuItem onClick={() => handleViewAcknowledgments(post)}>
                  <Users className="w-4 h-4 mr-2" />
                  Ver Aceites
                </DropdownMenuItem>
              )}
            </DropdownMenuContent>
          </DropdownMenu>
          </div>
        );
      default:
        return null;
    }
  };

  const renderList = () => {
    const listEmpty = posts.length === 0 || sortedVisiblePosts.length === 0;
    if (listEmpty) {
      return (
        <Card>
          <CardContent className="p-12 text-center">
            <Megaphone className="w-12 h-12 mx-auto text-muted-foreground/50 mb-4" />
            <h3 className="font-semibold text-lg mb-2">Nenhum comunicado encontrado</h3>
            <p className="text-muted-foreground text-sm">
              {searchTerm
                ? 'Tente buscar com outros termos'
                : activeTab !== 'todos'
                  ? `Nenhum comunicado em ${({ ativos: 'ativos', rascunhos: 'rascunhos', agendados: 'agendados', arquivados: 'arquivados' } as const)[activeTab] ?? activeTab}.`
                  : 'Aguarde novos comunicados'}
            </p>
          </CardContent>
        </Card>
      );
    }

    if (posts.length > 0) {
      return (
        <>
          <Card className="rounded-lg overflow-hidden">
            <DataTable<CommunityPost>
              columns={announcementsTableColumns}
              data={sortedVisiblePosts}
              keyExtractor={(p) => p.id}
              renderCell={renderAnnouncementCell}
              emptyMessage="Nenhum comunicado encontrado para os filtros selecionados."
              defaultSort={{ columnId: 'publishedAt', direction: 'desc' }}
              stickyHeader
              stickyColumnId="acoes"
            />
          </Card>
          {canLoadMore && (
            <div
              ref={loadMoreSentinelRef}
              className="min-h-[40px] flex items-center justify-center py-4"
              aria-hidden
            />
          )}
          {loadingMore && (
            <div className="flex justify-center py-4">
              <Spinner className="h-6 w-6 text-muted-foreground" />
            </div>
          )}
        </>
      );
    }

    return null;
  };

  return (
    <div className="space-y-6">
      {loading && (
        <div className="flex justify-center py-12">
          <Spinner className="h-8 w-8 text-muted-foreground" />
        </div>
      )}
      {!loading && errorProp && (
        <Card className="border-destructive/50 bg-destructive/5">
          <CardContent className="py-6">
            <p className="text-destructive text-center">{errorProp}</p>
            {onRefetch && (
              <div className="flex justify-center mt-3">
                <Button variant="outline" size="sm" onClick={() => void onRefetch()}>
                  Tentar novamente
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      )}
      {!loading && !errorProp && (
        <>
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold flex items-center gap-2">
            <Megaphone className="w-6 h-6" />
            Comunicados
          </h2>
          <p className="text-muted-foreground mt-1">
            Comunicados diretos para grupos de trabalho
          </p>
        </div>
        {canCreate && (
          <Button
            className="gap-2 rounded-lg"
            onClick={() => {
              setAnnouncementToEdit(null);
              setIsCreateModalOpen(true);
            }}
          >
            <Plus className="w-4 h-4" />
            Novo Comunicado
          </Button>
        )}
      </div>

      <Collapsible open={filtrosOpen} onOpenChange={setFiltrosOpen}>
        <Card>
          <CardContent className="p-0">
            <CollapsibleTrigger asChild>
              <Button
                variant="ghost"
                className="w-full justify-between rounded-b-none rounded-t-lg px-4 py-3 hover:bg-muted/50"
              >
                <span className="flex items-center gap-2 font-medium">
                  <SlidersHorizontal className="h-4 w-4 text-muted-foreground" />
                  Filtros
                  {(searchTerm || startDate || endDate || selectedMonth || selectedYear || filterTipo || filterRequerLeitura) && (
                    <span className="rounded-full bg-primary/10 px-2 py-0.5 text-xs font-medium text-primary">
                      {[searchTerm, startDate, endDate, selectedMonth, selectedYear, filterTipo, filterRequerLeitura].filter(Boolean).length} ativo(s)
                    </span>
                  )}
                </span>
                {filtrosOpen ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </Button>
            </CollapsibleTrigger>
            <CollapsibleContent>
              <div className="border-t px-4 py-4">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="search">Buscar</Label>
                    <div className="relative">
                      <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                      <Input
                        id="search"
                        placeholder="Buscar comunicado..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="pl-10 rounded-lg"
                      />
                    </div>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="startDate">Data Início</Label>
                    <Input
                      id="startDate"
                      type="date"
                      value={startDate}
                      onChange={(e) => setStartDate(e.target.value)}
                      className="rounded-lg"
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="endDate">Data Fim</Label>
                    <Input
                      id="endDate"
                      type="date"
                      value={endDate}
                      onChange={(e) => setEndDate(e.target.value)}
                      className="rounded-lg"
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="monthYear">Mês/Ano</Label>
                    <Input
                      id="monthYear"
                      type="month"
                      value={
                        selectedMonth && selectedYear
                          ? `${selectedYear}-${String(parseInt(selectedMonth) + 1).padStart(2, '0')}`
                          : ''
                      }
                      onChange={(e) => {
                        if (e.target.value) {
                          const [year, month] = e.target.value.split('-');
                          setSelectedYear(year);
                          setSelectedMonth(String(parseInt(month) - 1));
                        } else {
                          setSelectedYear('');
                          setSelectedMonth('');
                        }
                      }}
                      className="rounded-lg"
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="filterTipo">Tipo</Label>
                    <Select value={filterTipo || 'todos'} onValueChange={(v) => setFilterTipo((v === 'todos' ? '' : v) as '' | 'informativo' | 'documento')}>
                      <SelectTrigger id="filterTipo" className="rounded-lg">
                        <SelectValue placeholder="Todos" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="todos">Todos</SelectItem>
                        <SelectItem value="informativo">Informativo</SelectItem>
                        <SelectItem value="documento">Documento</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="filterRequerLeitura">Requer leitura</Label>
                    <Select value={filterRequerLeitura || 'todos'} onValueChange={(v) => setFilterRequerLeitura((v === 'todos' ? '' : v) as '' | 'sim' | 'nao')}>
                      <SelectTrigger id="filterRequerLeitura" className="rounded-lg">
                        <SelectValue placeholder="Todos" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="todos">Todos</SelectItem>
                        <SelectItem value="sim">Sim</SelectItem>
                        <SelectItem value="nao">Não</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </div>
                {(searchTerm || startDate || endDate || selectedMonth || selectedYear || filterTipo || filterRequerLeitura) && (
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => {
                      setSearchTerm('');
                      setStartDate('');
                      setEndDate('');
                      setSelectedMonth('');
                      setSelectedYear('');
                      setFilterTipo('');
                      setFilterRequerLeitura('');
                    }}
                    className="mt-4 rounded-lg"
                  >
                    Limpar Filtros
                  </Button>
                )}
              </div>
            </CollapsibleContent>
          </CardContent>
        </Card>
      </Collapsible>

      {showHistoryTab ? (
        <Tabs
          value={activeTab}
          onValueChange={(v) => setActiveTab(v as TabFiltro)}
        >
          <TabsList className="inline-flex h-auto flex-wrap bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-pillToken gap-2 mb-4 shadow-sm">
            <TabsTrigger
              value="todos"
              className="gap-2 rounded-pillToken py-2.5 px-4 text-sm font-medium transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <Megaphone className="w-4 h-4" />
              Todos
            </TabsTrigger>
            <TabsTrigger
              value="ativos"
              className="gap-2 rounded-pillToken py-2.5 px-4 text-sm font-medium transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <CircleCheck className="w-4 h-4" />
              Ativos
            </TabsTrigger>
            <TabsTrigger
              value="agendados"
              className="gap-2 rounded-pillToken py-2.5 px-4 text-sm font-medium transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <CalendarClock className="w-4 h-4" />
              Agendados / Aprova
            </TabsTrigger>
            <TabsTrigger
              value="arquivados"
              className="gap-2 rounded-pillToken py-2.5 px-4 text-sm font-medium transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <Archive className="w-4 h-4" />
              Arquivados / Expirados
            </TabsTrigger>
          </TabsList>
          <TabsContent value="todos" className="mt-0">
            {renderList()}
          </TabsContent>
          <TabsContent value="ativos" className="mt-0">
            {renderList()}
          </TabsContent>
          <TabsContent value="agendados" className="mt-0">
            <Tabs
              value={agendadosSubTab}
              onValueChange={(v) => setAgendadosSubTab(v as AgendadosSubTab)}
            >
              <TabsList className="inline-flex h-auto flex-wrap bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-lg gap-2 mb-4">
                <TabsTrigger
                  value="todos"
                  className="rounded-lg py-2 px-4 text-sm font-medium data-[state=inactive]:text-muted-foreground data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
                >
                  Todos
                </TabsTrigger>
                <TabsTrigger
                  value="agendados"
                  className="rounded-lg py-2 px-4 text-sm font-medium data-[state=inactive]:text-muted-foreground data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
                >
                  Agendados
                </TabsTrigger>
                <TabsTrigger
                  value="aprovacao"
                  className="rounded-lg py-2 px-4 text-sm font-medium data-[state=inactive]:text-muted-foreground data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
                >
                  Aguardando aprovação
                </TabsTrigger>
              </TabsList>
              <TabsContent value="todos" className="mt-0">
                {agendadosPosts.length === 0 ? (
                  <Card>
                    <CardContent className="p-12 text-center">
                      <Megaphone className="w-12 h-12 mx-auto text-muted-foreground/50 mb-4" />
                      <p className="text-muted-foreground">Nenhum comunicado agendado ou em aprovação.</p>
                    </CardContent>
                  </Card>
                ) : (
                  <Card className="rounded-lg overflow-hidden">
                    <DataTable<CommunityPost>
                      columns={announcementsTableColumns}
                      data={sortedAgendadosPosts}
                      keyExtractor={(p) => p.id}
                      renderCell={renderAnnouncementCell}
                      emptyMessage="Nenhum comunicado encontrado."
                      defaultSort={{ columnId: 'publishedAt', direction: 'desc' }}
                      stickyHeader
                      stickyColumnId="acoes"
                    />
                  </Card>
                )}
              </TabsContent>
              <TabsContent value="agendados" className="mt-0">
                {sortedAgendadosPosts.length === 0 ? (
                  <Card>
                    <CardContent className="p-12 text-center">
                      <Megaphone className="w-12 h-12 mx-auto text-muted-foreground/50 mb-4" />
                      <p className="text-muted-foreground">Nenhum comunicado agendado.</p>
                    </CardContent>
                  </Card>
                ) : (
                  <Card className="rounded-lg overflow-hidden">
                    <DataTable<CommunityPost>
                      columns={announcementsTableColumns}
                      data={sortedAgendadosPosts}
                      keyExtractor={(p) => p.id}
                      renderCell={renderAnnouncementCell}
                      emptyMessage="Nenhum comunicado agendado."
                      defaultSort={{ columnId: 'publishedAt', direction: 'desc' }}
                      stickyHeader
                      stickyColumnId="acoes"
                    />
                  </Card>
                )}
              </TabsContent>
              <TabsContent value="aprovacao" className="mt-0">
                {sortedAgendadosPosts.length === 0 ? (
                  <Card>
                    <CardContent className="p-12 text-center">
                      <Megaphone className="w-12 h-12 mx-auto text-muted-foreground/50 mb-4" />
                      <p className="text-muted-foreground">Nenhum comunicado aguardando aprovação.</p>
                    </CardContent>
                  </Card>
                ) : (
                  <Card className="rounded-lg overflow-hidden">
                    <DataTable<CommunityPost>
                      columns={announcementsTableColumns}
                      data={sortedAgendadosPosts}
                      keyExtractor={(p) => p.id}
                      renderCell={renderAnnouncementCell}
                      emptyMessage="Nenhum comunicado aguardando aprovação."
                      defaultSort={{ columnId: 'publishedAt', direction: 'desc' }}
                      stickyHeader
                      stickyColumnId="acoes"
                    />
                  </Card>
                )}
              </TabsContent>
            </Tabs>
          </TabsContent>
          <TabsContent value="arquivados" className="mt-0">
            {renderList()}
          </TabsContent>
        </Tabs>
      ) : (
        renderList()
      )}

      {canCreate && (
        <CreateAnnouncementModal
          open={isCreateModalOpen}
          onOpenChange={(open) => {
            setIsCreateModalOpen(open);
            if (!open) setAnnouncementToEdit(null);
          }}
          userGroups={userGroups}
          onSave={(a) => {
            if (announcementToEdit) {
              setAnnouncements((prev) =>
                prev.map((x) => (x.id === a.id ? a : x)),
              );
            } else {
              setAnnouncements((prev) => [a, ...prev]);
            }
            void onRefetch?.();
          }}
          initialAnnouncement={announcementToEdit}
        />
      )}

      {selectedPostForAck && ackData && (
        <AnnouncementAcknowledgmentsModal
          open={isAckModalOpen}
          onOpenChange={(open) => {
            setIsAckModalOpen(open);
            if (!open) {
              setSelectedPostForAck(null);
              setAckData(null);
            }
          }}
          announcementTitle={selectedPostForAck.title}
          requiresAcknowledgment={selectedPostForAck.requiresAcknowledgment}
          totalRecipients={ackData.totalRecipients}
          acknowledgments={ackData.acknowledgments}
        />
      )}

      <Dialog open={!!postToReject} onOpenChange={(open) => !open && closeRejectModal()}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Rejeitar comunicado</DialogTitle>
            <DialogDescription>
              Informe o motivo da rejeição. Este campo é obrigatório.
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-2 py-2">
            <Label htmlFor="motivo-rejeicao">Justificativa</Label>
            <Textarea
              id="motivo-rejeicao"
              value={motivoRejeicao}
              onChange={(e) => setMotivoRejeicao(e.target.value)}
              placeholder="Descreva o motivo da rejeição..."
              rows={4}
              className="resize-none"
            />
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={closeRejectModal}>
              Cancelar
            </Button>
            <Button
              variant="destructive"
              disabled={!motivoRejeicao.trim() || aprovacaoActionId !== null}
              onClick={() => postToReject && handleRejeitar(postToReject, motivoRejeicao)}
            >
              {aprovacaoActionId === postToReject?.id ? (
                <Spinner className="h-4 w-4 shrink-0" />
              ) : (
                'Rejeitar'
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {postForPreview && (
        <RequiredItemDetailModal
          open={!!postForPreview}
          onOpenChange={(open) => !open && setPostForPreview(null)}
          item={{ ...postForPreview, itemType: 'post' as const }}
          isAlreadyAcknowledged={!!postForPreview.acknowledgedAt}
          acknowledgedAt={postForPreview.acknowledgedAt}
          onConfirmadoLeitura={onRefetch ? () => void onRefetch() : undefined}
          hideAcknowledge
        />
      )}

      <AlertDialog open={!!announcementToDelete} onOpenChange={(open) => !open && setAnnouncementToDelete(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir comunicado</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja excluir &quot;{announcementToDelete?.title}&quot;? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={handleConfirmDelete} className="bg-destructive text-destructive-foreground hover:bg-destructive/90">
              Excluir
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
        </>
      )}
    </div>
  );
}
