import { useState, useEffect, useCallback, useRef } from 'react';
import { container } from 'tsyringe';
import {
  X,
  CheckCircle2,
  Megaphone,
  FileText,
  File,
  FolderOpen,
  Calendar,
  User,
  Users,
  Tag,
  Heart,
  MessageCircle,
  Eye,
  Reply,
  Send,
  ChevronUp,
  Trash2,
} from 'lucide-react';
import type { CommunityPost, Announcement, PostComment, PostAttachment } from '@domain/entities/comunicacao';
import { parseApiDate } from '@shared/utils/dateUtils';
import { ConfirmarLeituraObrigatoriaUseCase } from '@domain/usecases/ConfirmarLeituraObrigatoriaUseCase';
import { InserirInteracaoPublicacaoUseCase } from '@domain/usecases/InserirInteracaoPublicacaoUseCase';
import { ExcluirInteracaoPublicacaoUseCase } from '@domain/usecases/ExcluirInteracaoPublicacaoUseCase';
import { InserirInteracaoComentarioUseCase } from '@domain/usecases/InserirInteracaoComentarioUseCase';
import { ExcluirInteracaoComentarioUseCase } from '@domain/usecases/ExcluirInteracaoComentarioUseCase';
import { InserirComentarioPublicacaoUseCase } from '@domain/usecases/InserirComentarioPublicacaoUseCase';
import { ObterConfirmacoesLeituraUseCase } from '@domain/usecases/ObterConfirmacoesLeituraUseCase';
import { ObterPublicacaoPorIdUseCase } from '@domain/usecases/ObterPublicacaoPorIdUseCase';
import { ExcluirComentarioPublicacaoUseCase } from '@domain/usecases/ExcluirComentarioPublicacaoUseCase';
import type { ConfirmacaoLeitura } from '@domain/entities/comunicacao';
import { useAppSelector } from '@app/store/hooks';
import { DataTable } from '@presentation/components/common';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
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
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { DocumentPreview } from './DocumentPreview';
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from '@/components/ui/carousel';
import { Spinner } from '@/components/ui/spinner';
import { toast } from 'sonner';
import { format, formatDistanceToNow } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { contentWithYoutubeEmbeds } from '@shared/utils/youtubeEmbed';
import { cn } from '@/lib/utils';

function getIniciaisConfirmacao(name: string): string {
  const parts = String(name).trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
  return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
}

export type RequiredItem =
  | (CommunityPost & { itemType: 'post' })
  | (Announcement & { itemType: 'announcement' });

interface RequiredItemDetailModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  item: RequiredItem | null;
  isAlreadyAcknowledged?: boolean;
  acknowledgedAt?: string;
  /** Chamado após confirmar leitura com sucesso (refetch para o item sair da aba Pendentes). */
  onConfirmadoLeitura?: () => void | Promise<void>;
  /** Quando true, oculta o bloco "Li e Estou Ciente" (ex.: preview na tab announcements). */
  hideAcknowledge?: boolean;
  /** Quando true, oculta a opção "Ver aceites" (ex.: modal aberto da tab feed). */
  hideVerAceites?: boolean;
}

export function RequiredItemDetailModal({
  open,
  onOpenChange,
  item,
  isAlreadyAcknowledged = false,
  acknowledgedAt,
  onConfirmadoLeitura,
  hideAcknowledge = false,
  hideVerAceites = false,
}: RequiredItemDetailModalProps) {
  const [acknowledged, setAcknowledged] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const token = useAppSelector((state) => state.auth.token);
  const confirmarLeituraObrigatoriaUseCase = container.resolve(
    ConfirmarLeituraObrigatoriaUseCase,
  );
  const inserirInteracaoPublicacaoUseCase = container.resolve(InserirInteracaoPublicacaoUseCase);
  const excluirInteracaoPublicacaoUseCase = container.resolve(ExcluirInteracaoPublicacaoUseCase);
  const inserirInteracaoComentarioUseCase = container.resolve(InserirInteracaoComentarioUseCase);
  const excluirInteracaoComentarioUseCase = container.resolve(ExcluirInteracaoComentarioUseCase);
  const inserirComentarioPublicacaoUseCase = container.resolve(InserirComentarioPublicacaoUseCase);
  const obterPublicacaoPorIdUseCase = container.resolve(ObterPublicacaoPorIdUseCase);
  const obterConfirmacoesLeituraUseCase = container.resolve(ObterConfirmacoesLeituraUseCase);
  const excluirComentarioPublicacaoUseCase = container.resolve(ExcluirComentarioPublicacaoUseCase);

  const codigoColaboradorInternoUsuarioLogado = useAppSelector(
    (state) => state.auth.user?.colaborador?.codigoColaboradorInterno ?? state.auth.codColaborador ?? null,
  );

  const [aceitesExpanded, setAceitesExpanded] = useState(false);
  const [aceitesLoading, setAceitesLoading] = useState(false);
  const [aceitesList, setAceitesList] = useState<ConfirmacaoLeitura[]>([]);

  // Estado para reações e comentários (quando item é post)
  const [reactionCounts, setReactionCounts] = useState<Record<string, number>>({});
  const [selectedEmoji, setSelectedEmoji] = useState<string | null>(null);
  const localEmojiRef = useRef<string | null>(null);
  const [localComments, setLocalComments] = useState<PostComment[]>([]);
  const [commentsCount, setCommentsCount] = useState(0);
  const [comentarioTexto, setComentarioTexto] = useState('');
  const [replyTexto, setReplyTexto] = useState('');
  const [replyingToCommentId, setReplyingToCommentId] = useState<string | null>(null);
  const [submittingComentario, setSubmittingComentario] = useState(false);
  const [submittingReply, setSubmittingReply] = useState(false);
  const [submittingReaction, setSubmittingReaction] = useState(false);
  const [likePopoverOpen, setLikePopoverOpen] = useState(false);
  const [likePopoverOpenCommentId, setLikePopoverOpenCommentId] = useState<string | null>(null);
  const [submittingReactionCommentId, setSubmittingReactionCommentId] = useState<string | null>(null);
  const [commentsExpanded, setCommentsExpanded] = useState(false);
  const [commentToDelete, setCommentToDelete] = useState<PostComment | null>(null);
  const [deletingCommentId, setDeletingCommentId] = useState<string | null>(null);
  const commentsSectionRef = useRef<HTMLDivElement>(null);
  const aceitesSectionRef = useRef<HTMLDivElement>(null);

  // Ao abrir a seção de comentários, rolar até ela para o usuário ver que abriu
  useEffect(() => {
    if (commentsExpanded && commentsSectionRef.current) {
      const el = commentsSectionRef.current;
      requestAnimationFrame(() => {
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
      });
    }
  }, [commentsExpanded]);

  // Ao abrir a seção "Ver aceites", rolar até ela para o usuário ver que abriu
  useEffect(() => {
    if (aceitesExpanded && aceitesSectionRef.current) {
      const el = aceitesSectionRef.current;
      requestAnimationFrame(() => {
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
      });
    }
  }, [aceitesExpanded]);

  // Resetar estado ao abrir o modal ou ao trocar de item (evita mostrar "aceito" de outro documento)
  useEffect(() => {
    if (open && item) {
      setAcknowledged(!!isAlreadyAcknowledged);
      setSubmitting(false);
    }
  }, [open, item?.id, isAlreadyAcknowledged]);

  useEffect(() => {
    if (!open) {
      setAceitesExpanded(false);
      setAceitesList([]);
      setCommentsExpanded(false);
      setCommentToDelete(null);
    }
  }, [open]);

  const isAnnouncement = item?.itemType === 'announcement';
  const announcement = item && isAnnouncement ? (item as Announcement) : null;
  const post = item && !isAnnouncement ? (item as CommunityPost) : null;

  const isActivePublication = post
    ? post.status !== 'draft' && post.status !== 'pending_approval'
    : false;
  const showLikesModal = Boolean(post?.allowLikes && isActivePublication);
  const showCommentsModal = Boolean(post?.allowComments && isActivePublication);

  const likeReactions = [
    { emoji: '❤️', label: 'Coração', apiKey: 'coracao' },
    { emoji: '🚀', label: 'Foguete', apiKey: 'foguete' },
    { emoji: '👏', label: 'Palmas', apiKey: 'palmas' },
    { emoji: '😄', label: 'Divertido', apiKey: 'divertido' },
    { emoji: '🔥', label: 'Foguinho', apiKey: 'foguinho' },
  ];
  const emojiParaChaveApi = (emoji: string) =>
    likeReactions.find((r) => r.emoji === emoji)?.apiKey ?? emoji;

  // Sincronizar estado de reações/comentários quando o post do item mudar
  useEffect(() => {
    if (!open || !post) return;
    const isCorrupted = (e: string | null) => !e || e === '?' || e.trim() === '';
    const apiEmoji = post.meuEmojiReacao ?? null;
    const resolvedEmoji = isCorrupted(apiEmoji) ? localEmojiRef.current : apiEmoji;
    setSelectedEmoji(resolvedEmoji);
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
    setCommentsCount(post.commentsCount);
    setLocalComments(post.comments ?? []);
  }, [open, post?.id, post?.reactionCounts, post?.meuEmojiReacao, post?.commentsCount, post?.comments]);

  const refreshPublicacao = useCallback(async () => {
    if (!token || !post) return;
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
      setCommentsCount(updated.commentsCount);
      setLocalComments(updated.comments ?? []);
    } catch {
      // falha silenciosa
    }
  }, [token, post?.id, obterPublicacaoPorIdUseCase]);

  const handleEmojiSelect = useCallback(
    async (emoji: string) => {
      if (!post?.allowLikes || post.status === 'pending_approval' || !token) return;
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
          }
          const res = await inserirInteracaoPublicacaoUseCase.execute(token, {
            publicacaoId: post.id,
            emoji: emojiParaChaveApi(emoji),
          });
          if (res?.sucesso !== false) {
            localEmojiRef.current = emoji;
            setSelectedEmoji(emoji);
            setReactionCounts((prev) => ({ ...prev, [emoji]: (prev[emoji] ?? 0) + 1 }));
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
    [post?.allowLikes, post?.status, post?.id, token, selectedEmoji, inserirInteracaoPublicacaoUseCase, excluirInteracaoPublicacaoUseCase, refreshPublicacao],
  );

  const handleEmojiSelectForComment = useCallback(
    async (c: PostComment, emoji: string) => {
      if (!post?.allowLikes || post.status === 'pending_approval' || !token) return;
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
    [post?.allowLikes, post?.status, token, inserirInteracaoComentarioUseCase, excluirInteracaoComentarioUseCase, refreshPublicacao],
  );

  const handleEnviarComentario = useCallback(async () => {
    const texto = comentarioTexto.trim();
    if (!post?.allowComments || post.status === 'pending_approval' || !token || !texto) return;
    setSubmittingComentario(true);
    try {
      const res = await inserirComentarioPublicacaoUseCase.execute(token, {
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
  }, [post?.allowComments, post?.status, post?.id, token, comentarioTexto, inserirComentarioPublicacaoUseCase, refreshPublicacao]);

  const handleEnviarResposta = useCallback(
    async (comentarioPaiId: string) => {
      const texto = replyTexto.trim();
      if (!post?.allowComments || post.status === 'pending_approval' || !token || !texto) return;
      setSubmittingReply(true);
      try {
        const res = await inserirComentarioPublicacaoUseCase.execute(token, {
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
    },
    [post?.allowComments, post?.status, post?.id, token, replyTexto, inserirComentarioPublicacaoUseCase, refreshPublicacao],
  );

  const handleExcluirComentario = useCallback(
    async (c: PostComment) => {
      if (!token || !post) return;
      setDeletingCommentId(c.id);
      try {
        const res = await excluirComentarioPublicacaoUseCase.execute(token, post.id, c.id);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Comentário excluído.');
          setCommentsCount((n) => Math.max(0, n - 1));
          setLocalComments((prev) => prev.filter((x) => x.id !== c.id && x.parentId !== c.id));
          void refreshPublicacao();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível excluir o comentário.');
        }
      } catch (e) {
        toast.error(e instanceof Error ? e.message : 'Erro ao excluir comentário.');
      } finally {
        setDeletingCommentId(null);
        setCommentToDelete(null);
      }
    },
    [token, post, excluirComentarioPublicacaoUseCase, refreshPublicacao],
  );

  if (!item) return null;

  const handleAcknowledge = async () => {
    if (isAnnouncement) {
      setAcknowledged(true);
      toast.success('Leitura confirmada', {
        description: 'Seu aceite foi registrado com sucesso.',
      });
      setTimeout(() => onOpenChange(false), 1500);
      return;
    }
    if (!token || !post) return;
    setSubmitting(true);
    try {
      const res = await confirmarLeituraObrigatoriaUseCase.execute(token, {
        publicacaoId: post.id,
      });
      if (res.sucesso) {
        setAcknowledged(true);
        await onConfirmadoLeitura?.();
        toast.success(res.mensagem ?? 'Leitura obrigatória confirmada com sucesso.');
        setTimeout(() => onOpenChange(false), 1500);
      } else {
        toast.error(res.mensagem ?? 'Não foi possível confirmar a leitura.');
      }
    } catch {
      toast.error('Erro ao confirmar leitura. Tente novamente.');
    } finally {
      setSubmitting(false);
    }
  };

  const formatFileSize = (bytes: number) => {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
  };

  const attachments = post?.attachments ?? announcement?.attachments ?? [];
  const hasDocumentAttachment =
    (announcement?.announcementType === 'document' &&
      announcement.attachments.length > 0) ||
    (!!post && post.attachments.some((a) => a.type === 'document'));

  const document = hasDocumentAttachment
    ? isAnnouncement
      ? announcement!.attachments[0]
      : post!.attachments.find((a) => a.type === 'document') ?? post!.attachments[0]
    : null;

  const mediaAttachments = attachments.filter(
    (a): a is PostAttachment & { type: 'image' | 'video' } =>
      a.type === 'image' || a.type === 'video',
  );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent
        className={`overflow-hidden flex flex-col ${
          hasDocumentAttachment && document
            ? 'max-w-[90rem] w-[95vw] h-[100dvh] max-h-[100dvh]'
            : 'max-w-6xl max-h-[90vh]'
        }`}
      >
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            {isAnnouncement ? (
              <>
                <Megaphone className="w-5 h-5" />
                Preview
              </>
            ) : (
              <>
                <FileText className="w-5 h-5" />
                Preview
              </>
            )}
          </DialogTitle>
        </DialogHeader>

        <div className="flex-1 overflow-y-auto">
          <div
            className={`grid gap-6 ${
              hasDocumentAttachment && document
                ? 'grid-cols-1 lg:grid-cols-[minmax(280px,380px)_1fr]'
                : 'grid-cols-1'
            }`}
          >
            <div className="space-y-4 min-w-0">
              <div>
                <div className="flex items-start gap-2 mb-2">
                  <h2 className="text-2xl font-bold flex-1">{item.title}</h2>
                  {isAnnouncement && (
                    <Badge
                      variant={
                        announcement!.announcementType === 'document'
                          ? 'default'
                          : 'outline'
                      }
                    >
                      {announcement!.announcementType === 'document'
                        ? '📄 Documento'
                        : '📰 Informativo'}
                    </Badge>
                  )}
                </div>

                <div className="flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
                  <div className="flex items-center gap-1">
                    <User className="w-4 h-4" />
                    <span>{item.authorName}</span>
                  </div>
                  {isAnnouncement &&
                    announcement!.targetUserGroupNames.length > 0 && (
                      <div className="flex items-center gap-1">
                        <Users className="w-4 h-4" />
                        <span>
                          {announcement!.targetUserGroupNames.join(', ')}
                        </span>
                      </div>
                    )}
                  {!isAnnouncement && post && (
                    <div className="flex items-center gap-1">
                      <Users className="w-4 h-4" />
                      <span>
                        {post.comunidadeId
                          ? (post.comunidadeNome ?? post.groupName)
                          : 'Comunicado'}
                      </span>
                    </div>
                  )}
                  {item.publishedAt && (() => {
                    const date = parseApiDate(item.publishedAt);
                    return date && !Number.isNaN(date.getTime()) ? (
                      <div className="flex items-center gap-1">
                        <Calendar className="w-4 h-4" />
                        <span>
                          {format(date, "dd 'de' MMMM 'de' yyyy", { locale: ptBR })}
                        </span>
                      </div>
                    ) : null;
                  })()}
                  {item.expiresAt && (() => {
                    const date = parseApiDate(item.expiresAt);
                    return date && !Number.isNaN(date.getTime()) ? (
                      <div className="flex items-center gap-1">
                        <Calendar className="w-4 h-4" />
                        <span>Validade: {format(date, "dd 'de' MMMM 'de' yyyy", { locale: ptBR })}</span>
                      </div>
                    ) : null;
                  })()}
                </div>
              </div>

              <Separator />

              {hasDocumentAttachment && document && (
                <>
                  <Card className="border-primary/30 bg-primary/5">
                    <CardContent className="p-4 space-y-3">
                      <div className="flex items-center gap-2">
                        <File className="w-5 h-5 text-primary" />
                        <h3 className="font-semibold text-primary">
                          Informações do Documento
                        </h3>
                      </div>
                      <div className="space-y-2">
                        <div className="flex items-start gap-2">
                          <FileText className="w-4 h-4 text-muted-foreground mt-0.5" />
                          <div className="flex-1">
                            <p className="text-xs text-muted-foreground">
                              Nome do arquivo
                            </p>
                            <p className="text-sm font-medium">
                              {document.name}
                            </p>
                          </div>
                        </div>
                        {document.size && (
                          <div className="flex items-start gap-2">
                            <File className="w-4 h-4 text-muted-foreground mt-0.5" />
                            <div className="flex-1">
                              <p className="text-xs text-muted-foreground">
                                Tamanho
                              </p>
                              <p className="text-sm font-medium">
                                {formatFileSize(document.size)}
                              </p>
                            </div>
                          </div>
                        )}
                        {(() => {
                          const folders = announcement?.folderPaths?.length
                            ? announcement.folderPaths
                            : announcement?.folderPath
                              ? [announcement.folderPath]
                              : post?.folderPath
                                ? [post.folderPath]
                                : [];
                          return folders.length > 0 ? (
                            <div className="flex items-start gap-2">
                              <FolderOpen className="w-4 h-4 text-muted-foreground mt-0.5 flex-shrink-0" />
                              <div className="flex-1">
                                <p className="text-xs text-muted-foreground mb-1.5">
                                  Pastas
                                </p>
                                <div className="flex flex-wrap gap-1.5">
                                  {folders.map((path) => (
                                    <Badge
                                      key={path}
                                      variant="secondary"
                                      className="font-normal"
                                    >
                                      {path}
                                    </Badge>
                                  ))}
                                </div>
                              </div>
                            </div>
                          ) : null;
                        })()}
                      </div>
                    </CardContent>
                  </Card>
                  <Separator />
                </>
              )}

              {isAnnouncement &&
                announcement?.tags &&
                announcement.tags.length > 0 && (
                  <>
                    <div className="space-y-2">
                      <div className="flex items-center gap-2">
                        <Tag className="w-4 h-4 text-muted-foreground" />
                        <h3 className="text-sm font-semibold">Tags</h3>
                      </div>
                      <div className="flex flex-wrap gap-2">
                        {announcement.tags.map((tag, idx) => (
                          <Badge key={idx} variant="outline">
                            {tag}
                          </Badge>
                        ))}
                      </div>
                    </div>
                    <Separator />
                  </>
                )}

              <div className="space-y-2">
                <h3 className="text-sm font-semibold">Conteúdo</h3>
                <div
                  className="prose prose-sm max-w-none"
                  dangerouslySetInnerHTML={{
                    __html: contentWithYoutubeEmbeds(item.content ?? ''),
                  }}
                />
              </div>

              {mediaAttachments.length > 0 && (
                <>
                  <Separator />
                  <div className="space-y-2">
                    <h3 className="text-sm font-semibold">Anexos</h3>
                    {mediaAttachments.length === 1 ? (
                      <div className="w-full overflow-hidden rounded-lg border bg-muted/30 min-h-[200px] max-h-[420px]">
                        {mediaAttachments[0].type === 'image' ? (
                          <img
                            src={mediaAttachments[0].url}
                            alt={mediaAttachments[0].name}
                            className="w-full max-h-[420px] object-contain object-center"
                          />
                        ) : (
                          <div className="relative w-full aspect-video max-h-[420px] bg-black">
                            <video
                              src={mediaAttachments[0].url}
                              controls
                              className="w-full h-full object-contain"
                            />
                          </div>
                        )}
                      </div>
                    ) : (
                      <Carousel className="w-full" opts={{ align: 'start', loop: true }}>
                        <CarouselContent className="-ml-0">
                          {mediaAttachments.map((att) => (
                            <CarouselItem key={att.id} className="pl-0">
                              <div className="w-full overflow-hidden rounded-lg border bg-muted/30 min-h-[200px] max-h-[420px]">
                                {att.type === 'image' ? (
                                  <img
                                    src={att.url}
                                    alt={att.name}
                                    className="w-full max-h-[420px] object-contain object-center"
                                  />
                                ) : (
                                  <div className="relative w-full aspect-video max-h-[420px] bg-black">
                                    <video
                                      src={att.url}
                                      controls
                                      className="w-full h-full object-contain"
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
                    )}
                  </div>
                </>
              )}

              {isAnnouncement &&
                announcement?.announcementType === 'informative' &&
                announcement.attachments.length > 0 && (
                  <>
                    <Separator />
                    <div className="space-y-3">
                      <div className="flex items-center gap-2">
                        <File className="w-4 h-4 text-muted-foreground" />
                        <h3 className="text-sm font-semibold">
                          Documentos Anexos ({announcement.attachments.length})
                        </h3>
                      </div>
                      <div className="space-y-2">
                        {announcement.attachments.map((attachment, idx) => (
                          <Card
                            key={idx}
                            className="hover:bg-muted/50 transition-colors"
                          >
                            <CardContent className="p-3">
                              <div className="flex items-center gap-3">
                                <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                                  <File className="w-5 h-5 text-primary" />
                                </div>
                                <div className="flex-1 min-w-0">
                                  <p className="text-sm font-medium truncate">
                                    {attachment.name}
                                  </p>
                                  {attachment.size && (
                                    <p className="text-xs text-muted-foreground">
                                      {formatFileSize(attachment.size)}
                                    </p>
                                  )}
                                </div>
                              </div>
                            </CardContent>
                          </Card>
                        ))}
                      </div>
                    </div>
                  </>
                )}

              <Separator />
              <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground">
                <div className="flex items-center gap-1">
                  <Eye className="w-4 h-4" />
                  <span>{item.viewsCount} visualizações</span>
                </div>
                {showLikesModal && (
                  <Popover open={likePopoverOpen} onOpenChange={setLikePopoverOpen}>
                    <PopoverTrigger asChild>
                      <div className="inline-flex items-center gap-1.5">
                        {Object.entries(reactionCounts).filter(([, c]) => c > 0).length === 0 ? (
                          <button
                            type="button"
                            className="flex items-center gap-1.5 text-sm transition-colors text-muted-foreground hover:text-primary"
                          >
                            <Heart className="w-4 h-4" />
                            <span>{item.likesCount} curtidas</span>
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
                    <PopoverContent side="top" sideOffset={6} className="w-auto p-2 rounded-lg">
                      <div className="flex items-center gap-1">
                        {likeReactions.map(({ emoji, label }) => {
                          const isSelected = emoji === selectedEmoji;
                          return (
                            <button
                              key={label}
                              type="button"
                              title={isSelected ? 'Remover reação' : label}
                              disabled={submittingReaction}
                              className={cn(
                                'text-lg p-1.5 rounded-md transition-colors disabled:opacity-50',
                                isSelected ? 'bg-muted ring-1 ring-border' : 'hover:bg-muted',
                              )}
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
                {showCommentsModal && (
                  <button
                    type="button"
                    onClick={() => setCommentsExpanded((v) => !v)}
                    className={cn(
                      'flex items-center gap-1.5 text-sm transition-colors',
                      commentsExpanded ? 'text-foreground' : 'text-muted-foreground hover:text-foreground',
                    )}
                  >
                    <MessageCircle className="w-4 h-4" />
                    <span>{commentsCount} comentários</span>
                  </button>
                )}
                {!hideVerAceites && (
                  <div className="flex items-center gap-2 flex-wrap">
                    <CheckCircle2 className="w-4 h-4" />
                    <span>{item.acknowledgmentCount} aceites</span>
                    {item.requiresAcknowledgment && (
                      <Button
                        variant="ghost"
                        size="sm"
                        className="h-7 text-xs"
                        onClick={async () => {
                          if (aceitesExpanded) {
                            setAceitesExpanded(false);
                            return;
                          }
                          setAceitesExpanded(true);
                          if (aceitesList.length > 0) return;
                          if (!token || !item) return;
                          setAceitesLoading(true);
                          try {
                            const result = await obterConfirmacoesLeituraUseCase.execute(token, item.id);
                            setAceitesList(result.confirmacoes);
                          } catch {
                            toast.error('Não foi possível carregar os aceites.');
                          } finally {
                            setAceitesLoading(false);
                          }
                        }}
                        disabled={aceitesLoading}
                      >
                        {aceitesLoading ? (
                          <Spinner className="w-3.5 h-3.5 mr-1" />
                        ) : aceitesExpanded ? (
                          'Ocultar'
                        ) : (
                          'Ver aceites'
                        )}
                      </Button>
                    )}
                  </div>
                )}
              </div>

              {!hideVerAceites && aceitesExpanded && item.requiresAcknowledgment && (
                <div ref={aceitesSectionRef}>
                  <Separator />
                  <div className="space-y-2">
                    <h3 className="text-sm font-semibold">Pessoas que leram e aceitaram</h3>
                    {aceitesLoading ? (
                      <div className="flex items-center justify-center py-8">
                        <Spinner className="w-8 h-8 text-muted-foreground" />
                      </div>
                    ) : (
                      <DataTable<ConfirmacaoLeitura>
                        columns={[
                          { id: 'foto', label: '', sortable: false, width: 'w-[52px]' },
                          { id: 'name', label: 'Nome completo', sortable: true },
                          { id: 'email', label: 'E-mail', sortable: true },
                          { id: 'cargo', label: 'Cargo', sortable: true },
                          { id: 'unidade', label: 'Unidade', sortable: true },
                        ]}
                        data={aceitesList}
                        keyExtractor={(r) => r.id}
                        renderCell={(row, columnId) => {
                          if (columnId === 'foto') {
                            return (
                              <Avatar className="h-9 w-9 shrink-0">
                                {row.urlFoto ? (
                                  <AvatarImage src={row.urlFoto} alt={row.name} className="object-cover" />
                                ) : null}
                                <AvatarFallback className="bg-primary/10 text-primary text-sm font-medium">
                                  {getIniciaisConfirmacao(row.name)}
                                </AvatarFallback>
                              </Avatar>
                            );
                          }
                          if (columnId === 'name') return <span className="font-medium">{row.name}</span>;
                          if (columnId === 'email') return <span className="text-sm text-muted-foreground">{row.email ?? '—'}</span>;
                          if (columnId === 'cargo') return <span className="text-sm">{row.cargo ?? '—'}</span>;
                          if (columnId === 'unidade') return <span className="text-sm">{row.unidade ?? '—'}</span>;
                          return null;
                        }}
                        emptyMessage="Nenhum aceite registrado."
                        stickyHeader
                      />
                    )}
                  </div>
                </div>
              )}

              {/* Comentários (abre ao clicar no ícone de comentários; layout igual ao card do feed) */}
              {post && showCommentsModal && commentsExpanded && (
                <div ref={commentsSectionRef}>
                  <Separator />
                  <div className="space-y-3">
                    <div className="flex items-center justify-between gap-2">
                      <p className="text-sm font-medium text-foreground">Comentários ({commentsCount})</p>
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
                      const isCommentAuthor = (c: PostComment) =>
                        !!codigoColaboradorInternoUsuarioLogado && c.authorId === codigoColaboradorInternoUsuarioLogado;
                      const topLevel = localComments.filter((c) => !c.parentId);
                      const getReplies = (parentId: string) =>
                        localComments.filter((c) => c.parentId === parentId);
                      const getInitials = (name: string) => {
                        const parts = String(name ?? '').trim().split(/\s+/).filter(Boolean);
                        if (parts.length === 0) return '?';
                        if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
                        return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
                      };
                      const renderComment = (c: PostComment, isReply: boolean) => {
                        const commentDate = parseApiDate(c.createdAt);
                        const isAuthor = isCommentAuthor(c);
                        const isDeleting = deletingCommentId === c.id;
                        return (
                          <li key={c.id} className={cn('flex gap-2', isReply && 'ml-6 mt-2 pl-2 border-l-2 border-muted')}>
                            <Avatar className="h-8 w-8 shrink-0">
                              {c.authorAvatar ? (
                                <AvatarImage src={c.authorAvatar} alt={c.authorName} />
                              ) : null}
                              <AvatarFallback className="text-xs bg-gradient-to-br from-primary to-accent text-primary-foreground">
                                {getInitials(c.authorName ?? '')}
                              </AvatarFallback>
                            </Avatar>
                            <div className="flex-1 min-w-0">
                              <p className="text-sm font-medium truncate">{c.authorName ?? 'Usuário'}</p>
                              <p className="text-xs text-muted-foreground">
                                {commentDate && !Number.isNaN(commentDate.getTime())
                                  ? formatDistanceToNow(commentDate, { addSuffix: true, locale: ptBR })
                                  : ''}
                              </p>
                              <div className="flex items-start justify-between gap-2 mt-0.5">
                                <p className="text-sm break-words flex-1 min-w-0">{c.content}</p>
                                {isAuthor && (
                                  <div className="flex items-center gap-0.5 shrink-0 opacity-70 hover:opacity-100">
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
                              {showLikesModal && (
                                <div className="mt-1.5 flex items-center gap-1">
                                  <Popover
                                    open={likePopoverOpenCommentId === c.id}
                                    onOpenChange={(open) => setLikePopoverOpenCommentId(open ? c.id : null)}
                                  >
                                    <PopoverTrigger asChild>
                                      <div className="inline-flex items-center gap-1">
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
                                    <PopoverContent side="top" sideOffset={4} className="w-auto p-2 rounded-lg">
                                      <div className="flex items-center gap-1">
                                        {likeReactions.map(({ emoji, label }) => {
                                          const isSelected = emoji === (c.meuEmojiReacao ?? null);
                                          return (
                                            <button
                                              key={label}
                                              type="button"
                                              title={isSelected ? 'Remover reação' : label}
                                              disabled={submittingReactionCommentId === c.id}
                                              className={cn(
                                                'text-base p-1 rounded-md transition-colors disabled:opacity-50',
                                                isSelected ? 'bg-muted ring-1 ring-border' : 'hover:bg-muted',
                                              )}
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
                                            onClick={() => { setReplyingToCommentId(null); setReplyTexto(''); }}
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
                                            {submittingReply ? <Spinner className="h-3.5 w-3.5" /> : <Send className="h-3.5 w-3.5" />}
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
                        onClick={() => void handleEnviarComentario()}
                        disabled={!comentarioTexto.trim() || submittingComentario}
                      >
                        {submittingComentario ? <Spinner className="h-3.5 w-3.5" /> : <Send className="h-3.5 w-3.5" />}
                        Enviar
                      </Button>
                    </div>
                  </div>
                </div>
              )}
            </div>

            {hasDocumentAttachment && document && (
              <div className="lg:sticky lg:top-4 h-[85dvh] min-h-[400px] min-w-0 flex flex-col">
                <DocumentPreview
                  url={document.url}
                  name={document.name || 'Documento'}
                  type={document.mimeType || 'application/pdf'}
                  allowDownload={
                    isAnnouncement &&
                    announcement?.announcementType === 'document'
                      ? (announcement?.allowDownload ?? false)
                      : (post?.allowDownload ?? false)
                  }
                  accessToken={token}
                />
              </div>
            )}
          </div>
        </div>

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
                Esta ação não pode ser desfeita. O comentário será removido.
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

        <div className="flex items-center justify-end gap-2 pt-4 border-t">
          <Button variant="ghost" onClick={() => onOpenChange(false)}>
            <X className="w-4 h-4 mr-2" />
            Fechar
          </Button>

          {!hideAcknowledge &&
            item.requiresAcknowledgment &&
            (isAlreadyAcknowledged ? (
              <Badge className="bg-success text-success-foreground gap-1 px-4 py-2">
                <CheckCircle2 className="w-4 h-4" />
                {acknowledgedAt
                  ? (() => {
                      const d = parseApiDate(acknowledgedAt);
                      return d ? `Aceito em ${format(d, "dd/MM/yyyy 'às' HH:mm", { locale: ptBR })}` : 'Leitura Aceita';
                    })()
                  : 'Leitura Aceita'}
              </Badge>
            ) : (
              <>
                {!acknowledged && (
                  <Button
                    onClick={handleAcknowledge}
                    disabled={submitting}
                    className="gap-2"
                  >
                    {submitting ? (
                      <Spinner className="h-4 w-4" />
                    ) : (
                      <CheckCircle2 className="w-4 h-4" />
                    )}
                    Li e Estou Ciente
                  </Button>
                )}
                {acknowledged && (
                  <Badge className="bg-success text-success-foreground gap-1 px-4 py-2">
                    <CheckCircle2 className="w-4 h-4" />
                    Leitura Confirmada
                  </Badge>
                )}
              </>
            ))}
        </div>
      </DialogContent>
    </Dialog>
  );
}
