import { useState, useRef, useEffect, useCallback } from 'react';
import { createPortal } from 'react-dom';
import { container } from 'tsyringe';
import {
  Heart,
  MessageCircle,
  Eye,
  Pin,
  Clock,
  Calendar,
  FileText,
  CheckCircle2,
  AlertCircle,
  MoreHorizontal,
  Send,
  ChevronUp,
  Pencil,
  Trash2,
  Reply,
  X,
  ChevronLeft,
  ChevronRight,
  ZoomIn,
} from 'lucide-react';
import type {
  CommunityPost,
  CommunityPersona,
  PostAttachment,
  PostComment,
} from '@domain/entities/comunicacao';
import { InserirComentarioPublicacaoUseCase } from '@domain/usecases/InserirComentarioPublicacaoUseCase';
import { InserirInteracaoComentarioUseCase } from '@domain/usecases/InserirInteracaoComentarioUseCase';
import { ExcluirInteracaoComentarioUseCase } from '@domain/usecases/ExcluirInteracaoComentarioUseCase';
import { InserirInteracaoPublicacaoUseCase } from '@domain/usecases/InserirInteracaoPublicacaoUseCase';
import { ExcluirInteracaoPublicacaoUseCase } from '@domain/usecases/ExcluirInteracaoPublicacaoUseCase';
import { AtualizarComentarioPublicacaoUseCase } from '@domain/usecases/AtualizarComentarioPublicacaoUseCase';
import { ExcluirComentarioPublicacaoUseCase } from '@domain/usecases/ExcluirComentarioPublicacaoUseCase';
import { ObterPublicacaoPorIdUseCase } from '@domain/usecases/ObterPublicacaoPorIdUseCase';
import { useAppSelector } from '@app/store/hooks';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from '@/components/ui/carousel';
import { format, formatDistanceToNow, differenceInMinutes } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';
import { contentWithYoutubeEmbeds } from '@shared/utils/youtubeEmbed';
import { parseApiDate } from '@shared/utils/dateUtils';
import { RequiredItemDetailModal, type RequiredItem } from './RequiredItemDetailModal';
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
import { Spinner } from '@/components/ui/spinner';
import { Skeleton } from '@/components/ui/skeleton';

interface PostCardProps {
  post: CommunityPost;
  persona: CommunityPersona;
  compact?: boolean;
  /** Código interno do usuário logado; só exibe "Arquivar post" se post.authorId === codigoColaboradorInternoUsuarioLogado. */
  codigoColaboradorInternoUsuarioLogado?: string;
  /** Chamado ao clicar em Arquivar (feed com API). Após sucesso, o parent deve refazer o fetch. */
  onArquivar?: (post: CommunityPost) => void | Promise<void>;
  /** Chamado ao clicar em Excluir e confirmar (feed com API). Após sucesso, o parent deve refazer o fetch. */
  onExcluir?: (post: CommunityPost) => void | Promise<void>;
  /** Chamado ao clicar em Editar (abre modal de edição no parent). */
  onEditar?: (post: CommunityPost) => void;
  /** Quando retorna true para um post, o item "Editar" fica desabilitado (ex.: comunicado com confirmação de leitura já preenchida). */
  editarDesabilitado?: (post: CommunityPost) => boolean;
  /** Chamado após confirmar leitura obrigatória no modal (para refetch do feed). */
  onConfirmadoLeitura?: () => void | Promise<void>;
  /** Chamado após enviar um comentário (para refetch do feed e atualizar contagem). */
  onComentarioEnviado?: () => void | Promise<void>;
  /** Na rota Comunicados > Todos: exibe as ações (Arquivar/Excluir) para todos os usuários, não só o autor. */
  acoesVisiveisParaTodos?: boolean;
  /** Conteúdo opcional exibido no rodapé do card (ex.: Total de confirmações + Ver Aceites na rota Comunicados). */
  footerExtra?: React.ReactNode;
  /** Exibe o status da publicação (Publicado, Arquivado, etc.) na rota Comunicados > Todos. */
  mostrarStatusPublicacao?: boolean;
  /** Quando true, não exibe o botão "Li e estou ciente" no card (ex.: tab Comunicados). */
  ocultarBotaoLiEstouCiente?: boolean;
}

const mediaTypes = ['image', 'video'] as const;
function isMedia(
  a: PostAttachment,
): a is PostAttachment & { type: 'image' | 'video' } {
  return mediaTypes.includes(a.type as 'image' | 'video');
}

function getInitials(name: string): string {
  return name
    .trim()
    .split(/\s+/)
    .map((s) => s[0])
    .join('')
    .slice(0, 2)
    .toUpperCase();
}

const statusLabels: Record<CommunityPost['status'], string> = {
  draft: 'Rascunho',
  scheduled: 'Agendado',
  published: 'Publicado',
  pending_approval: 'Aguardando aprovação',
  archived: 'Arquivado',
};

export function PostCard({ post, persona, compact = false, codigoColaboradorInternoUsuarioLogado, onArquivar, onExcluir, onEditar, editarDesabilitado, onConfirmadoLeitura, onComentarioEnviado: _onComentarioEnviado, acoesVisiveisParaTodos = false, footerExtra, mostrarStatusPublicacao = false, ocultarBotaoLiEstouCiente = false }: PostCardProps) {
  const token = useAppSelector((state) => state.auth.token);
  const inserirInteracaoComentarioUseCase = container.resolve(InserirInteracaoComentarioUseCase);
  const excluirInteracaoComentarioUseCase = container.resolve(ExcluirInteracaoComentarioUseCase);
  const inserirInteracaoPublicacaoUseCase = container.resolve(InserirInteracaoPublicacaoUseCase);
  const excluirInteracaoPublicacaoUseCase = container.resolve(ExcluirInteracaoPublicacaoUseCase);
  const obterPublicacaoPorIdUseCase = container.resolve(ObterPublicacaoPorIdUseCase);
  const [likesCount, setLikesCount] = useState(post.likesCount);
  const [reactionCounts, setReactionCounts] = useState<Record<string, number>>(
    () => post.reactionCounts ?? {},
  );
  /** Emoji que o usuário logado já reagiu (só pode haver um). */
  const [selectedEmoji, setSelectedEmoji] = useState<string | null>(() => post.meuEmojiReacao ?? null);
  /**
   * Ref com o último emoji escolhido pelo usuário nesta sessão.
   * Usado como fallback quando a API retorna "?" (emoji corrompido no servidor — banco sem utf8mb4).
   */
  const localEmojiRef = useRef<string | null>(post.meuEmojiReacao ?? null);
  const [commentsCount, setCommentsCount] = useState(post.commentsCount);
  const [localComments, setLocalComments] = useState<PostComment[]>(() => post.comments ?? []);
  useEffect(() => {
    setCommentsCount(post.commentsCount);
  }, [post.id, post.commentsCount]);
  useEffect(() => {
    setLocalComments(post.comments ?? []);
  }, [post.id, post.comments]);
  const [acknowledged, setAcknowledged] = useState(() => !!post.acknowledgedAt);
  const [requiredReadingModalOpen, setRequiredReadingModalOpen] = useState(false);
  const [documentPreviewModalOpen, setDocumentPreviewModalOpen] = useState(false);
  useEffect(() => {
    setAcknowledged(!!post.acknowledgedAt);
  }, [post.id, post.acknowledgedAt]);
  const [contentExpanded, setContentExpanded] = useState(false);
  const [showMoreVisible, setShowMoreVisible] = useState(false);
  const [commentsExpanded, setCommentsExpanded] = useState(false);
  const [comentarioTexto, setComentarioTexto] = useState('');
  const [submittingComentario, setSubmittingComentario] = useState(false);
  const [editingCommentId, setEditingCommentId] = useState<string | null>(null);
  const [editingContent, setEditingContent] = useState('');
  const [deletingCommentId, setDeletingCommentId] = useState<string | null>(null);
  const [commentToDelete, setCommentToDelete] = useState<PostComment | null>(null);
  const [replyingToCommentId, setReplyingToCommentId] = useState<string | null>(null);
  const [replyTexto, setReplyTexto] = useState('');
  const [submittingReply, setSubmittingReply] = useState(false);
  const contentRef = useRef<HTMLDivElement>(null);

  /** Comentário só pode ser editado em até 15 minutos após a criação. */
  const canEditComment = (createdAt: string): boolean => {
    const date = parseApiDate(createdAt);
    if (!date || Number.isNaN(date.getTime())) return false;
    return differenceInMinutes(new Date(), date) < 15;
  };

  const isCommentAuthor = (c: PostComment): boolean =>
    !!codigoColaboradorInternoUsuarioLogado && c.authorId === codigoColaboradorInternoUsuarioLogado;
  const [mediaLoadedCount, setMediaLoadedCount] = useState(0);
  const [lightboxOpen, setLightboxOpen] = useState(false);
  const [lightboxIndex, setLightboxIndex] = useState(0);

  const openLightbox = useCallback((index: number) => {
    setLightboxIndex(index);
    setLightboxOpen(true);
  }, []);

  const lightboxPrev = useCallback(() => {
    setLightboxIndex((i) => (i > 0 ? i - 1 : i));
  }, []);

  const lightboxNext = useCallback((total: number) => {
    setLightboxIndex((i) => (i < total - 1 ? i + 1 : i));
  }, []);

  const mediaAttachments = post.attachments.filter(isMedia);

  useEffect(() => {
    setMediaLoadedCount(0);
  }, [post.id, mediaAttachments.length]);

  const handleMediaLoad = useCallback(() => {
    setMediaLoadedCount((c) => Math.min(c + 1, mediaAttachments.length));
  }, [mediaAttachments.length]);
  const documentAttachments = post.attachments.filter(
    (a) => a.type === 'document',
  );

  useEffect(() => {
    const el = contentRef.current;
    if (!el) return;
    const check = () => setShowMoreVisible(el.scrollHeight > el.clientHeight);
    check();
    const t = setTimeout(check, 100);
    return () => clearTimeout(t);
  }, [post.content, contentExpanded]);

  const isScheduled = post.status === 'scheduled';
  const isPendingApproval = post.status === 'pending_approval';
  const hasScheduledDate = !!post.scheduledAt && (post.status === 'scheduled' || post.status === 'pending_approval');
  const canManage = persona === 'manager' || persona === 'analytics';
  const podeArquivar = acoesVisiveisParaTodos
    ? !!onArquivar
    : canManage &&
      !!onArquivar &&
      codigoColaboradorInternoUsuarioLogado != null &&
      post.authorId === codigoColaboradorInternoUsuarioLogado;
  const podeExcluir = acoesVisiveisParaTodos
    ? !!onExcluir
    : canManage &&
      !!onExcluir &&
      codigoColaboradorInternoUsuarioLogado != null &&
      post.authorId === codigoColaboradorInternoUsuarioLogado;
  const podeEditar = acoesVisiveisParaTodos
    ? !!onEditar
    : canManage &&
      !!onEditar &&
      codigoColaboradorInternoUsuarioLogado != null &&
      post.authorId === codigoColaboradorInternoUsuarioLogado;
  const [postToDelete, setPostToDelete] = useState<CommunityPost | null>(null);
  const [likePopoverOpen, setLikePopoverOpen] = useState(false);
  const likePopoverCloseTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const commentPopoverCloseTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [submittingReaction, setSubmittingReaction] = useState(false);
  const [likePopoverOpenCommentId, setLikePopoverOpenCommentId] = useState<string | null>(null);
  const [submittingReactionCommentId, setSubmittingReactionCommentId] = useState<string | null>(null);

  useEffect(() => {
    setLikesCount(post.likesCount);

    const apiEmoji = post.meuEmojiReacao ?? null;
    const isCorrupted = (e: string | null) => !e || e === '?' || e.trim() === '';

    // Quando a API devolve "?" (emoji corrompido no servidor), usa o emoji salvo localmente
    const resolvedEmoji = isCorrupted(apiEmoji) ? localEmojiRef.current : apiEmoji;
    setSelectedEmoji(resolvedEmoji);

    // Substitui chaves "?" no mapa de contagens pelo emoji conhecido localmente
    const rawCounts = post.reactionCounts ?? {};
    const resolvedCounts = Object.entries(rawCounts).reduce<Record<string, number>>(
      (acc, [k, v]) => {
        const key = isCorrupted(k) && localEmojiRef.current ? localEmojiRef.current : k;
        acc[key] = (acc[key] ?? 0) + v;
        return acc;
      },
      {},
    );
    setReactionCounts(resolvedCounts);
  }, [post.id, post.likesCount, post.reactionCounts, post.meuEmojiReacao]);

  /**
   * Busca somente esta publicação no servidor e atualiza o estado local.
   * Substitui o reload completo do feed após interações (reação, comentário, etc.).
   */
  const refreshPublicacao = useCallback(async () => {
    if (!token) return;
    try {
      const updated = await obterPublicacaoPorIdUseCase.execute(token, post.id);
      const isCorrupted = (e: string | null) => !e || e === '?' || e.trim() === '';
      const apiEmoji = updated.meuEmojiReacao ?? null;
      const resolvedEmoji = isCorrupted(apiEmoji) ? localEmojiRef.current : apiEmoji;
      setSelectedEmoji(resolvedEmoji);
      const rawCounts = updated.reactionCounts ?? {};
      const resolvedCounts = Object.entries(rawCounts).reduce<Record<string, number>>(
        (acc, [k, v]) => {
          const key = isCorrupted(k) && localEmojiRef.current ? localEmojiRef.current : k;
          acc[key] = (acc[key] ?? 0) + v;
          return acc;
        },
        {},
      );
      setReactionCounts(resolvedCounts);
      setLikesCount(updated.likesCount);
      setCommentsCount(updated.commentsCount);
      setLocalComments(updated.comments ?? []);
    } catch {
      // Falha silenciosa: estado otimista permanece
    }
  }, [token, post.id, obterPublicacaoPorIdUseCase]);

  const handleEmojiSelect = useCallback(
    async (emoji: string) => {
      if (!post.allowLikes || post.status === 'pending_approval' || !token) return;
      setSubmittingReaction(true);
      try {
        const isDeselecting = emoji === selectedEmoji;

        if (isDeselecting) {
          const res = await excluirInteracaoPublicacaoUseCase.execute(token, { publicacaoId: post.id });
          if (res?.sucesso !== false) {
            localEmojiRef.current = null;
            setSelectedEmoji(null);
            setReactionCounts((prev) => {
              const next = { ...prev };
              const cur = (next[emoji] ?? 1) - 1;
              if (cur <= 0) delete next[emoji];
              else next[emoji] = cur;
              return next;
            });
            setLikesCount((prev) => Math.max(0, prev - 1));
            setLikePopoverOpen(false);
            void refreshPublicacao();
          } else {
            toast.error(res?.mensagem ?? 'Não foi possível remover a reação.');
          }
        } else {
          if (selectedEmoji) {
            await excluirInteracaoPublicacaoUseCase.execute(token, { publicacaoId: post.id });
            setReactionCounts((prev) => {
              const next = { ...prev };
              const cur = (next[selectedEmoji] ?? 1) - 1;
              if (cur <= 0) delete next[selectedEmoji];
              else next[selectedEmoji] = cur;
              return next;
            });
            setLikesCount((prev) => Math.max(0, prev - 1));
          }
          const res = await inserirInteracaoPublicacaoUseCase.execute(token, {
            publicacaoId: post.id,
            emoji: emojiParaChaveApi(emoji),
          });
          if (res?.sucesso !== false) {
            localEmojiRef.current = emoji;
            setSelectedEmoji(emoji);
            setReactionCounts((prev) => ({ ...prev, [emoji]: (prev[emoji] ?? 0) + 1 }));
            setLikesCount((prev) => prev + 1);
            setLikePopoverOpen(false);
            void refreshPublicacao();
          } else {
            toast.error(res?.mensagem ?? 'Não foi possível enviar a reação.');
          }
        }
      } catch (e) {
        toast.error(e instanceof Error ? e.message : 'Erro ao atualizar reação.');
      } finally {
        setSubmittingReaction(false);
      }
    },
    [
      post.allowLikes,
      post.status,
      post.id,
      token,
      selectedEmoji,
      inserirInteracaoPublicacaoUseCase,
      excluirInteracaoPublicacaoUseCase,
      refreshPublicacao,
    ],
  );

  const handleEmojiSelectForComment = useCallback(
    async (c: PostComment, emoji: string) => {
      if (!post.allowLikes || post.status === 'pending_approval' || !token) return;
      const commentSelectedEmoji = c.meuEmojiReacao ?? null;
      setSubmittingReactionCommentId(c.id);
      try {
        const isDeselecting = emoji === commentSelectedEmoji;
        if (isDeselecting) {
          const res = await excluirInteracaoComentarioUseCase.execute(token, { comentarioId: c.id });
          if (res?.sucesso !== false) {
            setLikePopoverOpenCommentId(null);
            void refreshPublicacao();
          } else {
            toast.error(res?.mensagem ?? 'Não foi possível remover a reação.');
          }
        } else {
          if (commentSelectedEmoji) {
            await excluirInteracaoComentarioUseCase.execute(token, { comentarioId: c.id });
          }
          const res = await inserirInteracaoComentarioUseCase.execute(token, {
            comentarioId: c.id,
            emoji: emojiParaChaveApi(emoji),
          });
          if (res?.sucesso !== false) {
            setLikePopoverOpenCommentId(null);
            void refreshPublicacao();
          } else {
            toast.error(res?.mensagem ?? 'Não foi possível enviar a reação.');
          }
        }
      } catch (e) {
        toast.error(e instanceof Error ? e.message : 'Erro ao atualizar reação.');
      } finally {
        setSubmittingReactionCommentId(null);
      }
    },
    [post.allowLikes, post.status, token, inserirInteracaoComentarioUseCase, excluirInteracaoComentarioUseCase, refreshPublicacao],
  );

  const likeReactions = [
    { emoji: '❤️', label: 'Coração', apiKey: 'coracao' },
    { emoji: '🚀', label: 'Foguete', apiKey: 'foguete' },
    { emoji: '👏', label: 'Palmas', apiKey: 'palmas' },
    { emoji: '😄', label: 'Divertido', apiKey: 'divertido' },
    { emoji: '🔥', label: 'Foguinho', apiKey: 'foguinho' },
  ];

  /** Interação (like/comentário) só é exibida se a publicação estiver ativa (não rascunho, não aguardando aprovação) e permitir. */
  const isActivePublication =
    post.status !== 'draft' && post.status !== 'pending_approval';
  const showLikes = Boolean(post.allowLikes && isActivePublication);
  const showComments = Boolean(post.allowComments && isActivePublication);

  const emojiParaChaveApi = (emoji: string) =>
    likeReactions.find((r) => r.emoji === emoji)?.apiKey ?? emoji;

  const scheduleLikePopoverClose = () => {
    if (likePopoverCloseTimeoutRef.current) clearTimeout(likePopoverCloseTimeoutRef.current);
    likePopoverCloseTimeoutRef.current = setTimeout(() => setLikePopoverOpen(false), 150);
  };

  const cancelLikePopoverClose = () => {
    if (likePopoverCloseTimeoutRef.current) {
      clearTimeout(likePopoverCloseTimeoutRef.current);
      likePopoverCloseTimeoutRef.current = null;
    }
  };

  const scheduleCommentPopoverClose = (commentId: string) => {
    if (commentPopoverCloseTimeoutRef.current) clearTimeout(commentPopoverCloseTimeoutRef.current);
    commentPopoverCloseTimeoutRef.current = setTimeout(
      () => setLikePopoverOpenCommentId((cur) => (cur === commentId ? null : cur)),
      150,
    );
  };

  const cancelCommentPopoverClose = () => {
    if (commentPopoverCloseTimeoutRef.current) {
      clearTimeout(commentPopoverCloseTimeoutRef.current);
      commentPopoverCloseTimeoutRef.current = null;
    }
  };

  useEffect(() => () => {
    cancelLikePopoverClose();
    cancelCommentPopoverClose();
  }, []);

  const handleOpenRequiredReadingModal = () => {
    setRequiredReadingModalOpen(true);
  };

  const requiredItem: RequiredItem | null = post.requiresAcknowledgment
    ? { ...post, itemType: 'post' as const }
    : null;

  /** Publicação tipo documento, ativa, aprovada, sem leitura obrigatória e com anexos: ao clicar no documento abre modal com preview e download se permitido. */
  const isDocumentPreviewEligible =
    post.type === 'document' &&
    post.status === 'published' &&
    (post.approvalStatus ?? '').toLowerCase() === 'aprovado' &&
    !post.requiresAcknowledgment &&
    documentAttachments.length > 0;

  const handleConfirmadoLeitura = () => {
    setAcknowledged(true);
    void onConfirmadoLeitura?.();
  };

  const handleEnviarComentario = async () => {
    const texto = comentarioTexto.trim();
    if (!post.allowComments || post.status === 'pending_approval' || !token || !texto) return;
    setSubmittingComentario(true);
    try {
      const useCase = container.resolve(InserirComentarioPublicacaoUseCase);
      const res = await useCase.execute(token, {
        publicacaoId: post.id,
        conteudo: texto,
      });
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Comentário enviado.');
        setComentarioTexto('');
        setCommentsCount((c) => c + 1);
        void refreshPublicacao();
      } else {
        toast.error(res.mensagem ?? 'Não foi possível enviar o comentário.');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao enviar comentário.');
    } finally {
      setSubmittingComentario(false);
    }
  };

  const handleEnviarResposta = async (comentarioPaiId: string) => {
    const texto = replyTexto.trim();
    if (!post.allowComments || post.status === 'pending_approval' || !token || !texto) return;
    setSubmittingReply(true);
    try {
      const useCase = container.resolve(InserirComentarioPublicacaoUseCase);
      const res = await useCase.execute(token, {
        publicacaoId: post.id,
        conteudo: texto,
        comentarioPaiId,
      });
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Resposta enviada.');
        setReplyTexto('');
        setReplyingToCommentId(null);
        setCommentsCount((c) => c + 1);
        void refreshPublicacao();
      } else {
        toast.error(res.mensagem ?? 'Não foi possível enviar a resposta.');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao enviar resposta.');
    } finally {
      setSubmittingReply(false);
    }
  };

  const handleSaveEditComentario = async () => {
    if (!token || !editingCommentId || !editingContent.trim()) return;
    try {
      const useCase = container.resolve(AtualizarComentarioPublicacaoUseCase);
      const res = await useCase.execute(token, {
        comentarioId: editingCommentId,
        conteudo: editingContent.trim(),
      });
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Comentário atualizado.');
        setEditingCommentId(null);
        setEditingContent('');
        void refreshPublicacao();
      } else {
        toast.error(res.mensagem ?? 'Não foi possível atualizar o comentário.');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao atualizar comentário.');
    }
  };

  const handleExcluirComentario = async (c: PostComment) => {
    if (!token) return;
    setDeletingCommentId(c.id);
    try {
      const useCase = container.resolve(ExcluirComentarioPublicacaoUseCase);
      const res = await useCase.execute(token, post.id, c.id);
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Comentário excluído.');
        setCommentsCount((n) => Math.max(0, n - 1));
        void refreshPublicacao();
      } else {
        toast.error(res.mensagem ?? 'Não foi possível excluir o comentário.');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao excluir comentário.');
    } finally {
      setDeletingCommentId(null);
    }
  };

  const dateRaw = post.publishedAt ?? post.scheduledAt ?? post.createdAt;
  const date = parseApiDate(dateRaw) ?? new Date();
  const timeAgo =
    Number.isNaN(date.getTime())
      ? 'Publicado'
      : formatDistanceToNow(date, { addSuffix: true, locale: ptBR });

  return (
    <Card
      className={`overflow-hidden transition-all hover:shadow-md ${isScheduled ? 'opacity-75 border-dashed' : ''} ${
        isPendingApproval
          ? 'border-2 border-orange-400/70 bg-orange-50/80 dark:bg-orange-950/30 dark:border-orange-500/50'
          : ''
      }`}
    >
      <CardHeader className={compact ? 'pb-2 pt-3' : 'pb-3'}>
        <div className="flex items-start justify-between gap-4">
          <div className="flex items-center gap-3">
            <Avatar className={compact ? 'h-7 w-7' : 'h-10 w-10'}>
              {post.authorAvatar ? (
                <AvatarImage src={post.authorAvatar} alt={post.authorName} />
              ) : null}
              <AvatarFallback className="bg-gradient-to-br from-primary to-accent text-primary-foreground">
                {post.authorName
                  .split(' ')
                  .filter(Boolean)
                  .map((n) => n[0])
                  .slice(0, 2)
                  .join('')
                  .toUpperCase() || '?'}
              </AvatarFallback>
            </Avatar>
            <div>
              <div className="flex items-center gap-2 mb-1">
                <span className="font-semibold text-sm">{post.authorName}</span>
              </div>
              <div className="flex items-center gap-2 text-xs text-muted-foreground flex-wrap">
                <Badge variant="outline" className="text-xs">
                  {post.comunidadeId
                    ? (post.comunidadeNome ?? post.groupName)
                    : 'Comunicado'}
                </Badge>
                {post.labels?.length ? (
                  post.labels.map((label) => (
                    <Badge key={label} variant="outline" className="text-xs">
                      {label}
                    </Badge>
                  ))
                ) : null}
                <span>•</span>
                <span>{timeAgo}</span>
                {mostrarStatusPublicacao ? (
                  <>
                    <span>•</span>
                    <Badge
                      variant="outline"
                      className={`text-xs ${
                        post.status === 'published'
                          ? 'bg-success/10 text-success border-success/30'
                          : post.status === 'archived'
                            ? 'bg-muted text-muted-foreground'
                            : post.status === 'scheduled'
                              ? 'bg-warning/10 text-warning border-warning/30'
                              : post.status === 'pending_approval'
                                ? 'border-orange-400/60 bg-orange-100 text-orange-800 dark:bg-orange-900/40 dark:text-orange-300 dark:border-orange-500/50'
                                : 'bg-muted/80 text-muted-foreground'
                      }`}
                    >
                      {statusLabels[post.status]}
                    </Badge>
                  </>
                ) : (
                  (isScheduled || isPendingApproval) && (
                    <>
                      <span>•</span>
                      <Badge variant="secondary" className="text-xs gap-1">
                        <Clock className="w-3 h-3" />
                        Agendado
                      </Badge>
                    </>
                  )
                )}
              </div>
            </div>
          </div>

          <div className="flex items-center gap-2">
            {post.isPinned && (
              <Badge className="bg-primary/10 text-primary border-primary/30">
                <Pin className="w-3 h-3 mr-1" />
                Fixado
              </Badge>
            )}
            {post.requiresAcknowledgment && (
              <Badge
                className={
                  acknowledged
                    ? 'bg-success/10 text-success border-success/30'
                    : 'bg-warning/10 text-warning border-warning/30'
                }
              >
                {acknowledged ? (
                  <>
                    <CheckCircle2 className="w-3 h-3 mr-1" />
                    Aceito
                  </>
                ) : (
                  <>
                    <AlertCircle className="w-3 h-3 mr-1" />
                    Aceite Obrigatório
                  </>
                )}
              </Badge>
            )}
            {(acoesVisiveisParaTodos || canManage) && (podeArquivar || podeExcluir || podeEditar) && (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="ghost" size="icon" className="h-8 w-8">
                    <MoreHorizontal className="w-4 h-4" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  {podeEditar && (
                    <DropdownMenuItem
                      disabled={editarDesabilitado?.(post)}
                      onClick={() => !editarDesabilitado?.(post) && onEditar?.(post)}
                      title={editarDesabilitado?.(post) ? 'Edição indisponível: já existem confirmações de leitura' : undefined}
                    >
                      <Pencil className="w-4 h-4 mr-2" />
                      Editar
                    </DropdownMenuItem>
                  )}
                  {podeArquivar && (
                    <DropdownMenuItem
                      className="text-destructive"
                      onClick={() => onArquivar?.(post)}
                    >
                      Arquivar post
                    </DropdownMenuItem>
                  )}
                  {podeExcluir && (
                    <DropdownMenuItem
                      className="text-destructive"
                      onClick={() => setPostToDelete(post)}
                    >
                      Excluir post
                    </DropdownMenuItem>
                  )}
                </DropdownMenuContent>
              </DropdownMenu>
            )}
          </div>
        </div>
      </CardHeader>

      <CardContent className={compact ? 'space-y-2 pt-0 pb-3' : 'space-y-4'}>
        <div>
          <h3 className={compact ? 'text-sm font-semibold leading-snug' : 'text-lg font-semibold'}>{post.title}</h3>
          {post.subtitulo?.trim() && (
            <p className={compact ? 'text-xs text-muted-foreground mt-0.5' : 'text-sm text-muted-foreground mt-1'}>{post.subtitulo.trim()}</p>
          )}
        </div>

        {hasScheduledDate && post.scheduledAt && (() => {
          const scheduledDate = parseApiDate(post.scheduledAt);
          const formatted =
            scheduledDate && !Number.isNaN(scheduledDate.getTime())
              ? format(scheduledDate, "d 'de' MMMM 'de' yyyy 'às' HH:mm", { locale: ptBR })
              : post.scheduledAt;
          return (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Calendar className="w-4 h-4 shrink-0 text-orange-500 dark:text-orange-400" />
              <span>
                Será publicado em: <strong className="text-foreground">{formatted}</strong>
              </span>
            </div>
          );
        })()}

        <div className="space-y-1">
          <div
            ref={contentRef}
            className={`prose prose-sm dark:prose-invert max-w-none break-words ${
              !contentExpanded ? (compact ? 'line-clamp-1' : 'line-clamp-2') : ''
            }`}
            style={{ overflow: contentExpanded ? 'visible' : undefined }}
            dangerouslySetInnerHTML={{
              __html: contentWithYoutubeEmbeds(post.content ?? ''),
            }}
          />
          {!compact && (showMoreVisible || contentExpanded) && (
            <button
              type="button"
              onClick={() => setContentExpanded((e) => !e)}
              className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
            >
              {contentExpanded ? 'Ver menos' : 'Ver mais'}
            </button>
          )}
        </div>

        {!compact && mediaAttachments.length > 0 && (
          <div className="-mx-6 -mb-1 mt-1 relative">
            {mediaAttachments.length === 1 ? (
              <div className="relative w-full overflow-hidden bg-muted/30">
                {mediaAttachments[0].type === 'image' ? (
                  <button
                    type="button"
                    className="w-full block cursor-zoom-in focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary relative group"
                    onClick={() => openLightbox(0)}
                    aria-label="Ampliar imagem"
                  >
                    <img
                      src={mediaAttachments[0].url}
                      alt={mediaAttachments[0].name}
                      className="w-full max-h-[600px] object-contain object-center"
                      onLoad={handleMediaLoad}
                      onError={handleMediaLoad}
                    />
                    <span className="absolute top-2 right-2 rounded-full bg-black/40 p-1.5 opacity-0 group-hover:opacity-100 transition-opacity">
                      <ZoomIn className="h-4 w-4 text-white" />
                    </span>
                  </button>
                ) : (
                  <div className="relative w-full aspect-video bg-black">
                    <video
                      src={mediaAttachments[0].url}
                      controls
                      className="w-full h-full object-contain"
                      onLoadedData={handleMediaLoad}
                      onError={handleMediaLoad}
                    />
                  </div>
                )}
                {mediaLoadedCount < 1 && (
                  <div
                    className="absolute inset-0 z-10 flex items-center justify-center bg-muted/50 transition-opacity duration-200"
                    aria-hidden
                  >
                    <Skeleton className="h-64 w-full rounded-none" />
                  </div>
                )}
              </div>
            ) : (
              <>
                <Carousel className="w-full" opts={{ align: 'start', loop: true }}>
                  <CarouselContent className="-ml-0">
                    {mediaAttachments.map((att, idx) => (
                      <CarouselItem key={att.id} className="pl-0">
                        <div className="w-full overflow-hidden bg-muted/30">
                          {att.type === 'image' ? (
                            <button
                              type="button"
                              className="w-full block cursor-zoom-in focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary relative group"
                              onClick={() => openLightbox(idx)}
                              aria-label="Ampliar imagem"
                            >
                              <img
                                src={att.url}
                                alt={att.name}
                                className="w-full max-h-[600px] object-contain object-center"
                                onLoad={handleMediaLoad}
                                onError={handleMediaLoad}
                              />
                              <span className="absolute top-2 right-2 rounded-full bg-black/40 p-1.5 opacity-0 group-hover:opacity-100 transition-opacity">
                                <ZoomIn className="h-4 w-4 text-white" />
                              </span>
                            </button>
                          ) : (
                            <div className="relative w-full aspect-video bg-black">
                              <video
                                src={att.url}
                                controls
                                className="w-full h-full object-contain"
                                onLoadedData={handleMediaLoad}
                                onError={handleMediaLoad}
                              />
                            </div>
                          )}
                        </div>
                      </CarouselItem>
                    ))}
                  </CarouselContent>
                  <CarouselPrevious className="left-2 h-9 w-9 rounded-full border-2 bg-background/80 hover:bg-background" />
                  <CarouselNext className="right-2 h-9 w-9 rounded-full border-2 bg-background/80 hover:bg-background" />
                </Carousel>
                {mediaLoadedCount < mediaAttachments.length && (
                  <div
                    className="absolute inset-0 top-0 left-0 right-0 bottom-0 z-10 min-h-[240px] flex items-center justify-center bg-muted/50 transition-opacity duration-200"
                    aria-hidden
                  >
                    <Skeleton className="h-full w-full min-h-[240px] rounded-none" />
                  </div>
                )}
              </>
            )}
          </div>
        )}

        {!compact && documentAttachments.length > 0 && (
          <div className="flex flex-wrap gap-2">
            {documentAttachments.map((attachment) =>
              isDocumentPreviewEligible ? (
                <button
                  key={attachment.id}
                  type="button"
                  onClick={() => setDocumentPreviewModalOpen(true)}
                  className="flex items-center gap-2 p-2 rounded-lg bg-muted hover:bg-muted/80 cursor-pointer transition-colors text-foreground text-left w-full max-w-[280px] border-0"
                >
                  <FileText className="w-5 h-5 text-primary shrink-0" />
                  <div className="flex flex-col min-w-0">
                    <span className="text-sm font-medium truncate max-w-[200px]">
                      {attachment.name}
                    </span>
                    {attachment.size && (
                      <span className="text-xs text-muted-foreground">
                        {(attachment.size / 1024 / 1024).toFixed(2)} MB
                      </span>
                    )}
                  </div>
                </button>
              ) : (
                <a
                  key={attachment.id}
                  href={attachment.url}
                  download={attachment.name}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="flex items-center gap-2 p-2 rounded-lg bg-muted hover:bg-muted/80 cursor-pointer transition-colors no-underline text-foreground"
                >
                  <FileText className="w-5 h-5 text-primary shrink-0" />
                  <div className="flex flex-col min-w-0">
                    <span className="text-sm font-medium truncate max-w-[200px]">
                      {attachment.name}
                    </span>
                    {attachment.size && (
                      <span className="text-xs text-muted-foreground">
                        {(attachment.size / 1024 / 1024).toFixed(2)} MB
                      </span>
                    )}
                  </div>
                </a>
              ),
            )}
          </div>
        )}

        {!compact && !ocultarBotaoLiEstouCiente && post.requiresAcknowledgment && post.status === 'published' && !acknowledged && (
          <div className="transition-opacity">
            <Button
              onClick={handleOpenRequiredReadingModal}
              className="w-full gap-2"
              variant="primary"
            >
              <CheckCircle2 className="w-4 h-4" />
              Li e estou ciente
            </Button>
          </div>
        )}

        {requiredItem && (
          <RequiredItemDetailModal
            open={requiredReadingModalOpen}
            onOpenChange={setRequiredReadingModalOpen}
            item={requiredItem}
            isAlreadyAcknowledged={!!post.acknowledgedAt}
            acknowledgedAt={post.acknowledgedAt}
            onConfirmadoLeitura={handleConfirmadoLeitura}
            hideVerAceites
          />
        )}

        {isDocumentPreviewEligible && (
          <RequiredItemDetailModal
            open={documentPreviewModalOpen}
            onOpenChange={setDocumentPreviewModalOpen}
            item={{ ...post, itemType: 'post' as const }}
            isAlreadyAcknowledged
            onConfirmadoLeitura={undefined}
            hideVerAceites
          />
        )}

        {/* Lightbox de imagens */}
        {lightboxOpen && (() => {
          const images = mediaAttachments.filter((a) => a.type === 'image');
          if (images.length === 0) return null;
          const current = images[lightboxIndex] ?? images[0];
          return createPortal(
            <div
              role="dialog"
              aria-modal="true"
              aria-label="Visualizar imagem"
              className="fixed inset-0 z-[200] bg-black/95 flex items-center justify-center"
              onClick={() => setLightboxOpen(false)}
            >
              {/* Fechar */}
              <button
                type="button"
                onClick={() => setLightboxOpen(false)}
                className="absolute top-4 right-4 z-10 rounded-full bg-white/20 hover:bg-white/40 p-2 transition-colors"
                aria-label="Fechar"
              >
                <X className="h-5 w-5 text-white" />
              </button>

              {/* Anterior */}
              {images.length > 1 && lightboxIndex > 0 && (
                <button
                  type="button"
                  onClick={(e) => { e.stopPropagation(); lightboxPrev(); }}
                  className="absolute left-4 z-10 rounded-full bg-white/20 hover:bg-white/40 p-2 transition-colors"
                  aria-label="Imagem anterior"
                >
                  <ChevronLeft className="h-6 w-6 text-white" />
                </button>
              )}

              {/* Imagem — stopPropagation para não fechar ao clicar na imagem */}
              <img
                src={current.url}
                alt={current.name}
                className="max-w-[90vw] max-h-[90vh] object-contain select-none"
                onClick={(e) => e.stopPropagation()}
              />

              {/* Próxima */}
              {images.length > 1 && lightboxIndex < images.length - 1 && (
                <button
                  type="button"
                  onClick={(e) => { e.stopPropagation(); lightboxNext(images.length); }}
                  className="absolute right-4 z-10 rounded-full bg-white/20 hover:bg-white/40 p-2 transition-colors"
                  aria-label="Próxima imagem"
                >
                  <ChevronRight className="h-6 w-6 text-white" />
                </button>
              )}

              {/* Contador */}
              {images.length > 1 && (
                <span className="absolute bottom-4 left-1/2 -translate-x-1/2 text-white/70 text-sm tabular-nums pointer-events-none">
                  {lightboxIndex + 1} / {images.length}
                </span>
              )}
            </div>,
            document.body,
          );
        })()}

        <AlertDialog
          open={postToDelete !== null}
          onOpenChange={(open) => {
            if (!open) setPostToDelete(null);
          }}
        >
          <AlertDialogContent className="rounded-lg">
            <AlertDialogHeader>
              <AlertDialogTitle>Excluir publicação?</AlertDialogTitle>
              <AlertDialogDescription>
                Esta ação não pode ser desfeita. A publicação será removida permanentemente.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter className="sm:gap-2">
              <AlertDialogCancel className="rounded-lg">Cancelar</AlertDialogCancel>
              <AlertDialogAction
                className="rounded-lg bg-destructive text-destructive-foreground hover:bg-destructive/90"
                onClick={() => {
                  const toDelete = postToDelete;
                  setPostToDelete(null);
                  if (toDelete) void onExcluir?.(toDelete);
                }}
              >
                Excluir
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>

        <AlertDialog
          open={commentToDelete !== null}
          onOpenChange={(open) => {
            if (!open) setCommentToDelete(null);
          }}
        >
          <AlertDialogContent className="rounded-lg">
            <AlertDialogHeader>
              <AlertDialogTitle>Excluir comentário?</AlertDialogTitle>
              <AlertDialogDescription>
                Esta ação não pode ser desfeita. O comentário será removido permanentemente.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter className="sm:gap-2">
              <AlertDialogCancel className="rounded-lg">Cancelar</AlertDialogCancel>
              <AlertDialogAction
                className="rounded-lg bg-destructive text-destructive-foreground hover:bg-destructive/90"
                onClick={() => {
                  const toDelete = commentToDelete;
                  setCommentToDelete(null);
                  if (toDelete) void handleExcluirComentario(toDelete);
                }}
              >
                Excluir
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>

        {showComments && commentsExpanded && (
          <div className="border-t pt-3 space-y-3">
            <div className="flex items-center justify-between gap-2">
              <p className="text-sm font-medium text-foreground">Comentários</p>
              <Button
                type="button"
                variant="ghost"
                size="sm"
                className="h-8 gap-1.5 text-muted-foreground hover:text-foreground shrink-0"
                onClick={() => setCommentsExpanded(false)}
              >
                <ChevronUp className="h-4 w-4" />
                Recolher
              </Button>
            </div>
            {localComments.length > 0 && (() => {
              const topLevel = localComments.filter((c: PostComment) => !c.parentId);
              const getReplies = (parentId: string) =>
                localComments.filter((c: PostComment) => c.parentId === parentId);
              const renderComment = (c: PostComment, isReply: boolean) => {
                const commentDate = parseApiDate(c.createdAt);
                const isAuthor = isCommentAuthor(c);
                const showEdit = isAuthor && canEditComment(c.createdAt);
                const isEditing = editingCommentId === c.id;
                const isDeleting = deletingCommentId === c.id;
                return (
                  <li key={c.id} className={`flex gap-2 ${isReply ? 'ml-6 mt-2 pl-2 border-l-2 border-muted' : ''}`}>
                    <Avatar className="h-8 w-8 shrink-0">
                      {c.authorAvatar ? (
                        <AvatarImage src={c.authorAvatar} alt={c.authorName} />
                      ) : null}
                      <AvatarFallback className="text-xs bg-gradient-to-br from-primary to-accent text-primary-foreground">
                        {getInitials(c.authorName)}
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium truncate">{c.authorName}</p>
                      <p className="text-xs text-muted-foreground">
                        {commentDate && !Number.isNaN(commentDate.getTime())
                          ? formatDistanceToNow(commentDate, {
                              addSuffix: true,
                              locale: ptBR,
                            })
                          : ''}
                      </p>
                      {isEditing ? (
                        <div className="mt-1 space-y-2">
                          <Textarea
                            value={editingContent}
                            onChange={(e) => setEditingContent(e.target.value)}
                            className="min-h-[60px] text-sm resize-none rounded-lg"
                            disabled={isDeleting}
                          />
                          <div className="flex gap-2">
                            <Button
                              size="sm"
                              variant="outline"
                              className="h-7 text-xs"
                              onClick={() => {
                                setEditingCommentId(null);
                                setEditingContent('');
                              }}
                              disabled={isDeleting}
                            >
                              Cancelar
                            </Button>
                            <Button
                              size="sm"
                              className="h-7 text-xs gap-1"
                              onClick={() => void handleSaveEditComentario()}
                              disabled={!editingContent.trim() || isDeleting}
                            >
                              Salvar
                            </Button>
                          </div>
                        </div>
                      ) : (
                        <div className="flex items-start justify-between gap-2 mt-0.5">
                          <p className="text-sm break-words flex-1 min-w-0">{c.content}</p>
                          {isAuthor && (
                            <div className="flex items-center gap-0.5 shrink-0 opacity-70 hover:opacity-100">
                              {showEdit && (
                                <button
                                  type="button"
                                  onClick={() => {
                                    setEditingCommentId(c.id);
                                    setEditingContent(c.content);
                                  }}
                                  className="p-1.5 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted"
                                  title="Editar comentário"
                                  disabled={isDeleting}
                                >
                                  <Pencil className="w-3.5 h-3.5" />
                                </button>
                              )}
                              <button
                                type="button"
                                onClick={() => setCommentToDelete(c)}
                                className="p-1.5 rounded-md text-muted-foreground hover:text-destructive hover:bg-muted"
                                title="Excluir comentário"
                                disabled={isDeleting}
                              >
                                {isDeleting ? (
                                  <Spinner className="w-3.5 h-3.5" />
                                ) : (
                                  <Trash2 className="w-3.5 h-3.5" />
                                )}
                              </button>
                            </div>
                          )}
                        </div>
                      )}
                      {showLikes && (
                        <div className="mt-1.5 flex items-center gap-1">
                          <Popover
                            open={likePopoverOpenCommentId === c.id}
                            onOpenChange={(open) => setLikePopoverOpenCommentId(open ? c.id : null)}
                          >
                            <PopoverTrigger asChild>
                              <div
                                className="inline-flex items-center gap-1"
                                onMouseEnter={() => {
                                  cancelCommentPopoverClose();
                                  setLikePopoverOpenCommentId(c.id);
                                }}
                                onMouseLeave={() => scheduleCommentPopoverClose(c.id)}
                              >
                                {(() => {
                                  const entries = c.reactionCounts
                                    ? Object.entries(c.reactionCounts).filter(([, n]) => n > 0)
                                    : [];
                                  if (entries.length === 0) {
                                    return (
                                      <button
                                        type="button"
                                        className="flex items-center gap-1 text-xs text-muted-foreground hover:text-primary disabled:opacity-50"
                                        disabled={submittingReactionCommentId === c.id}
                                      >
                                        <Heart className="w-3 h-3" />
                                        <span>{c.likesCount ?? 0}</span>
                                      </button>
                                    );
                                  }
                                  return entries.map(([em, n]) => {
                                    const isMyReaction = em === (c.meuEmojiReacao ?? null);
                                    return (
                                      <button
                                        key={em}
                                        type="button"
                                        disabled={submittingReactionCommentId === c.id}
                                        onClick={(e) => { e.stopPropagation(); void handleEmojiSelectForComment(c, em); }}
                                        title={isMyReaction ? 'Remover sua reação' : `Reagir com ${em}`}
                                        className={cn(
                                          'inline-flex items-center gap-0.5 text-xs rounded-full px-1.5 py-0.5 transition-colors disabled:opacity-50',
                                          isMyReaction
                                            ? 'bg-primary/15 ring-1 ring-primary/40 text-primary font-medium'
                                            : 'bg-muted/60 hover:bg-muted text-muted-foreground',
                                        )}
                                      >
                                        <span>{em}</span>
                                        <span>{n}</span>
                                      </button>
                                    );
                                  });
                                })()}
                              </div>
                            </PopoverTrigger>
                            <PopoverContent
                              side="top"
                              sideOffset={4}
                              className="w-auto p-2 rounded-lg"
                              onMouseDown={(e) => e.stopPropagation()}
                              onMouseEnter={cancelCommentPopoverClose}
                              onMouseLeave={() => setLikePopoverOpenCommentId(null)}
                            >
                              <div className="flex items-center gap-1">
                                {likeReactions.map(({ emoji, label }) => {
                                  const isSelected = emoji === (c.meuEmojiReacao ?? null);
                                  return (
                                    <button
                                      key={label}
                                      type="button"
                                      title={isSelected ? 'Remover reação' : label}
                                      disabled={submittingReactionCommentId === c.id}
                                      className={`text-base p-1 rounded-md transition-colors disabled:opacity-50 ${
                                        isSelected ? 'bg-muted ring-1 ring-border' : 'hover:bg-muted'
                                      }`}
                                      onClick={() => void handleEmojiSelectForComment(c, emoji)}
                                    >
                                      {emoji}
                                    </button>
                                  );
                                })}
                              </div>
                            </PopoverContent>
                          </Popover>
                        </div>
                      )}
                    </div>
                  </li>
                );
              };
              return (
                <ul className="space-y-3 max-h-64 overflow-y-auto">
                  {topLevel.map((c: PostComment) => {
                    const replies = getReplies(c.id);
                    const isReplying = replyingToCommentId === c.id;
                    return (
                      <li key={c.id} className="space-y-1">
                        {renderComment(c, false)}
                        {replies.length > 0 && (
                          <ul className="space-y-1 mt-1">
                            {replies.map((r: PostComment) => renderComment(r, true))}
                          </ul>
                        )}
                        {!c.parentId && (
                          <div className="ml-10 mt-1">
                            {!isReplying ? (
                              <button
                                type="button"
                                onClick={() => setReplyingToCommentId(c.id)}
                                className="text-xs text-muted-foreground hover:text-foreground flex items-center gap-1"
                              >
                                <Reply className="w-3.5 h-3.5" />
                                Responder
                              </button>
                            ) : (
                              <div className="space-y-2 mt-2">
                                <Textarea
                                  placeholder="Escreva sua resposta..."
                                  value={replyTexto}
                                  onChange={(e) => setReplyTexto(e.target.value)}
                                  className="min-h-[60px] text-sm resize-none rounded-lg"
                                  disabled={submittingReply}
                                />
                                <div className="flex gap-2">
                                  <Button
                                    size="sm"
                                    variant="outline"
                                    className="h-7 text-xs rounded-lg"
                                    onClick={() => {
                                      setReplyingToCommentId(null);
                                      setReplyTexto('');
                                    }}
                                    disabled={submittingReply}
                                  >
                                    Cancelar
                                  </Button>
                                  <Button
                                    size="sm"
                                    className="h-7 text-xs gap-1 rounded-lg"
                                    onClick={() => void handleEnviarResposta(c.id)}
                                    disabled={!replyTexto.trim() || submittingReply}
                                  >
                                    {submittingReply ? (
                                      <Spinner className="h-3.5 w-3.5" />
                                    ) : (
                                      <Send className="h-3.5 w-3.5" />
                                    )}
                                    Enviar resposta
                                  </Button>
                                </div>
                              </div>
                            )}
                          </div>
                        )}
                      </li>
                    );
                  })}
                </ul>
              );
            })()}
            <div className="space-y-2">
              <Label className="text-sm">Comentar</Label>
              <Textarea
                placeholder="Escreva seu comentário..."
                value={comentarioTexto}
                onChange={(e) => setComentarioTexto(e.target.value)}
                className="min-h-[80px] resize-none rounded-lg"
                disabled={submittingComentario}
              />
              <Button
                size="sm"
                className="gap-2"
                onClick={handleEnviarComentario}
                disabled={!comentarioTexto.trim() || submittingComentario}
              >
                {submittingComentario ? (
                  <Spinner className="h-3.5 w-3.5" />
                ) : (
                  <Send className="h-3.5 w-3.5" />
                )}
                Enviar
              </Button>
            </div>
          </div>
        )}

        {compact ? (
          <div className="flex items-center gap-3 pt-2 border-t">
            {showLikes && (
              <Popover open={likePopoverOpen} onOpenChange={setLikePopoverOpen}>
                <PopoverTrigger asChild>
                  <div
                    className="inline-flex items-center gap-1"
                    onMouseEnter={() => {
                      cancelLikePopoverClose();
                      setLikePopoverOpen(true);
                    }}
                    onMouseLeave={scheduleLikePopoverClose}
                  >
                    {Object.entries(reactionCounts).filter(([, c]) => c > 0).length === 0 ? (
                      <button
                        type="button"
                        className="flex items-center gap-1 text-xs transition-colors text-muted-foreground hover:text-primary"
                      >
                        <Heart className="w-3 h-3" />
                        <span>{likesCount}</span>
                      </button>
                    ) : (
                      Object.entries(reactionCounts)
                        .filter(([, c]) => c > 0)
                        .map(([emoji, c]) => {
                          const isMyReaction = emoji === selectedEmoji;
                          return (
                            <button
                              key={emoji}
                              type="button"
                              disabled={submittingReaction}
                              onClick={(e) => { e.stopPropagation(); void handleEmojiSelect(emoji); }}
                              title={isMyReaction ? 'Remover sua reação' : `Reagir com ${emoji}`}
                              className={cn(
                                'inline-flex items-center gap-0.5 text-xs rounded-full px-1.5 py-0.5 transition-colors disabled:opacity-50',
                                isMyReaction
                                  ? 'bg-primary/15 ring-1 ring-primary/40 text-primary font-medium'
                                  : 'bg-muted/60 hover:bg-muted text-muted-foreground',
                              )}
                            >
                              <span>{emoji}</span>
                              <span>{c}</span>
                            </button>
                          );
                        })
                    )}
                  </div>
                </PopoverTrigger>
                <PopoverContent
                  side="top"
                  sideOffset={6}
                  className="w-auto p-2 rounded-lg"
                  onMouseEnter={cancelLikePopoverClose}
                  onMouseLeave={() => setLikePopoverOpen(false)}
                >
                  <div className="flex items-center gap-1">
                    {likeReactions.map(({ emoji, label }) => {
                      const isSelected = emoji === selectedEmoji;
                      return (
                        <button
                          key={label}
                          type="button"
                          title={isSelected ? 'Remover reação' : label}
                          disabled={submittingReaction}
                          className={`text-lg p-1.5 rounded-md transition-colors disabled:opacity-50 ${
                            isSelected ? 'bg-muted ring-1 ring-border' : 'hover:bg-muted'
                          }`}
                          onClick={() => void handleEmojiSelect(emoji)}
                        >
                          {emoji}
                        </button>
                      );
                    })}
                  </div>
                </PopoverContent>
              </Popover>
            )}
            {showComments && (
              <button
                type="button"
                onClick={() => setCommentsExpanded((v) => !v)}
                className={`flex items-center gap-1 text-xs transition-colors ${
                  commentsExpanded ? 'text-foreground' : 'text-muted-foreground hover:text-foreground'
                }`}
              >
                <MessageCircle className="w-3 h-3" />
                <span>{commentsCount}</span>
              </button>
            )}
            <div className="ml-auto flex items-center gap-1 text-xs text-muted-foreground">
              <Eye className="w-3 h-3" />
              <span>{post.viewsCount}</span>
            </div>
          </div>
        ) : (
          <div className="flex items-center justify-between pt-3 border-t">
            <div className="flex items-center gap-4">
              {showLikes && (
                <Popover open={likePopoverOpen} onOpenChange={setLikePopoverOpen}>
                  <PopoverTrigger asChild>
                    <div
                      className="inline-flex items-center gap-1.5"
                      onMouseEnter={() => {
                        cancelLikePopoverClose();
                        setLikePopoverOpen(true);
                      }}
                      onMouseLeave={scheduleLikePopoverClose}
                    >
                      {Object.entries(reactionCounts).filter(([, c]) => c > 0).length === 0 ? (
                        <button
                          type="button"
                          className="flex items-center gap-1.5 text-sm transition-colors text-muted-foreground hover:text-primary"
                        >
                          <Heart className="w-4 h-4" />
                          <span>{likesCount}</span>
                        </button>
                      ) : (
                        Object.entries(reactionCounts)
                          .filter(([, c]) => c > 0)
                          .map(([emoji, c]) => {
                            const isMyReaction = emoji === selectedEmoji;
                            return (
                              <button
                                key={emoji}
                                type="button"
                                disabled={submittingReaction}
                                onClick={(e) => { e.stopPropagation(); void handleEmojiSelect(emoji); }}
                                title={isMyReaction ? 'Remover sua reação' : `Reagir com ${emoji}`}
                                className={cn(
                                  'inline-flex items-center gap-1 text-sm rounded-full px-2 py-0.5 transition-colors disabled:opacity-50',
                                  isMyReaction
                                    ? 'bg-primary/15 ring-1 ring-primary/40 text-primary font-medium'
                                    : 'bg-muted/60 hover:bg-muted text-muted-foreground',
                                )}
                              >
                                <span>{emoji}</span>
                                <span>{c}</span>
                              </button>
                            );
                          })
                      )}
                    </div>
                  </PopoverTrigger>
                  <PopoverContent
                    side="top"
                    sideOffset={6}
                    className="w-auto p-2 rounded-lg"
                    onMouseEnter={cancelLikePopoverClose}
                    onMouseLeave={() => setLikePopoverOpen(false)}
                  >
                    <div className="flex items-center gap-1">
                      {likeReactions.map(({ emoji, label }) => {
                        const isSelected = emoji === selectedEmoji;
                        return (
                          <button
                            key={label}
                            type="button"
                            title={isSelected ? 'Remover reação' : label}
                            disabled={submittingReaction}
                            className={`text-lg p-1.5 rounded-md transition-colors disabled:opacity-50 ${
                              isSelected ? 'bg-muted ring-1 ring-border' : 'hover:bg-muted'
                            }`}
                            onClick={() => void handleEmojiSelect(emoji)}
                          >
                            {emoji}
                          </button>
                        );
                      })}
                    </div>
                  </PopoverContent>
                </Popover>
              )}
              {showComments && (
                <button
                  type="button"
                  onClick={() => setCommentsExpanded((v) => !v)}
                  className={`flex items-center gap-1.5 text-sm transition-colors ${
                    commentsExpanded ? 'text-foreground' : 'text-muted-foreground hover:text-foreground'
                  }`}
                >
                  <MessageCircle className="w-4 h-4" />
                  <span>{commentsCount}</span>
                </button>
              )}
            </div>
            <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
              <Eye className="w-3.5 h-3.5" />
              <span>{post.viewsCount} visualizações</span>
            </div>
          </div>
        )}
      </CardContent>
      {footerExtra != null && (
        <div className="border-t bg-muted/20 px-4 py-3">
          {footerExtra}
        </div>
      )}
    </Card>
  );
}
