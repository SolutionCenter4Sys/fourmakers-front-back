import { useState } from 'react';
import { container } from 'tsyringe';
import {
  Edit,
  Trash2,
  Send,
  Calendar,
  Megaphone,
  CheckCircle,
  XCircle,
  Search,
  Archive,
  MoreHorizontal,
  FileText,
} from 'lucide-react';
import type { Announcement, PostAttachment, UserGroup } from '@domain/entities/comunicacao';
import { AprovarPublicacaoUseCase } from '@domain/usecases/AprovarPublicacaoUseCase';
import { PublicarAgoraPublicacaoUseCase } from '@domain/usecases/PublicarAgoraPublicacaoUseCase';
import { RejeitarPublicacaoUseCase } from '@domain/usecases/RejeitarPublicacaoUseCase';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Spinner } from '@/components/ui/spinner';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from '@/components/ui/carousel';
import { toast } from 'sonner';

const mediaTypes = ['image', 'video'] as const;
function isMediaAttachment(
  a: PostAttachment,
): a is PostAttachment & { type: 'image' | 'video' } {
  return mediaTypes.includes(a.type as 'image' | 'video');
}

interface ScheduledPostsViewProps {
  announcements: Announcement[];
  setAnnouncements: React.Dispatch<React.SetStateAction<Announcement[]>>;
  userGroups: UserGroup[];
  /** Quando informado, Aprovar / Rejeitar / Publicar agora chamam a API e onRefetch ao sucesso. */
  token?: string | null;
  onRefetch?: () => void | Promise<void>;
  /** Ao clicar em Editar, abre o modal de edição no parent. */
  onEditar?: (announcement: Announcement) => void;
  /** Se informado, desabilita o botão Editar quando retornar true (ex.: já tem confirmações de leitura). */
  editarDesabilitado?: (announcement: Announcement) => boolean;
  /** Ao clicar em Arquivar, arquiva o comunicado (parent pode chamar API e onRefetch). */
  onArquivar?: (announcement: Announcement) => void;
}

export function ScheduledPostsView({
  announcements,
  setAnnouncements,
  userGroups: _userGroups,
  token,
  onRefetch,
  onEditar,
  editarDesabilitado,
  onArquivar,
}: ScheduledPostsViewProps) {
  const [statusFilter, setStatusFilter] = useState<
    'all' | 'scheduled' | 'pending_approval'
  >('all');
  const [searchQuery, setSearchQuery] = useState('');
  const [actionId, setActionId] = useState<string | null>(null);
  const [announcementToReject, setAnnouncementToReject] = useState<Announcement | null>(null);
  const [motivoRejeicao, setMotivoRejeicao] = useState('');

  const publicarAgoraUseCase = container.resolve(PublicarAgoraPublicacaoUseCase);
  const aprovarUseCase = container.resolve(AprovarPublicacaoUseCase);
  const rejeitarUseCase = container.resolve(RejeitarPublicacaoUseCase);

  const items = announcements.filter(
    (a) => a.status === 'scheduled' || a.status === 'pending_approval',
  );
  const filteredItems =
    statusFilter === 'all'
      ? items
      : items.filter((a) => a.status === statusFilter);

  const searchLower = searchQuery.trim().toLowerCase();
  const searchFilteredItems = searchLower
    ? filteredItems.filter(
        (a) =>
          a.title.toLowerCase().includes(searchLower) ||
          a.authorName.toLowerCase().includes(searchLower) ||
          a.content.toLowerCase().includes(searchLower) ||
          a.targetUserGroupNames.some((name) =>
            name.toLowerCase().includes(searchLower),
          ),
      )
    : filteredItems;

  const handlePublishNow = async (id: string) => {
    if (token && onRefetch) {
      setActionId(id);
      try {
        const res = await publicarAgoraUseCase.execute(token, id);
        if (res.sucesso) {
          setAnnouncements((prev) =>
            prev.map((a) =>
              a.id === id
                ? {
                    ...a,
                    status: 'published' as const,
                    publishedAt: new Date().toISOString(),
                    updatedAt: new Date().toISOString(),
                  }
                : a,
            ),
          );
          toast.success(res.mensagem ?? 'Comunicado publicado agora.');
          await onRefetch();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível publicar agora.');
        }
      } catch (e) {
        toast.error(e instanceof Error ? e.message : 'Erro ao publicar.');
      } finally {
        setActionId(null);
      }
    } else {
      setAnnouncements((prev) =>
        prev.map((a) =>
          a.id === id
            ? {
                ...a,
                status: 'published' as const,
                publishedAt: new Date().toISOString(),
                updatedAt: new Date().toISOString(),
              }
            : a,
        ),
      );
      toast.success('Comunicado publicado', {
        description: 'O comunicado foi publicado agora.',
      });
    }
  };

  const handleApprove = async (id: string) => {
    if (token && onRefetch) {
      setActionId(id);
      try {
        const res = await aprovarUseCase.execute(token, id);
        if (res.sucesso) {
          setAnnouncements((prev) =>
            prev.map((a) =>
              a.id === id
                ? {
                    ...a,
                    status: 'published' as const,
                    publishedAt: new Date().toISOString(),
                    updatedAt: new Date().toISOString(),
                  }
                : a,
            ),
          );
          toast.success(res.mensagem ?? 'Comunicado aprovado.');
          await onRefetch();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível aprovar.');
        }
      } catch (e) {
        toast.error(e instanceof Error ? e.message : 'Erro ao aprovar.');
      } finally {
        setActionId(null);
      }
    } else {
      setAnnouncements((prev) =>
        prev.map((a) =>
          a.id === id
            ? {
                ...a,
                status: 'published' as const,
                publishedAt: new Date().toISOString(),
                updatedAt: new Date().toISOString(),
              }
            : a,
        ),
      );
      toast.success('Comunicado aprovado', {
        description: 'O comunicado foi publicado.',
      });
    }
  };

  const handleReject = async (id: string, justificativa: string) => {
    const motivo = justificativa.trim();
    if (!motivo) {
      toast.error('Informe a justificativa da rejeição.');
      return;
    }
    if (token && onRefetch) {
      setActionId(id);
      try {
        const res = await rejeitarUseCase.execute(token, id, motivo);
        if (res.sucesso) {
          setAnnouncements((prev) => prev.filter((a) => a.id !== id));
          setAnnouncementToReject(null);
          setMotivoRejeicao('');
          toast.success(res.mensagem ?? 'Comunicado rejeitado.');
          await onRefetch();
        } else {
          toast.error(res.mensagem ?? 'Não foi possível rejeitar.');
        }
      } catch (e) {
        toast.error(e instanceof Error ? e.message : 'Erro ao rejeitar.');
      } finally {
        setActionId(null);
      }
    } else {
      setAnnouncements((prev) => prev.filter((a) => a.id !== id));
      setAnnouncementToReject(null);
      setMotivoRejeicao('');
      toast.error('Comunicado rejeitado', {
        description: 'O comunicado foi removido da fila de aprovação.',
      });
    }
  };

  const openRejectModal = (announcement: Announcement) => {
    setAnnouncementToReject(announcement);
    setMotivoRejeicao('');
  };

  const closeRejectModal = () => {
    setAnnouncementToReject(null);
    setMotivoRejeicao('');
  };

  const handleDelete = (id: string) => {
    setAnnouncements((prev) => prev.filter((a) => a.id !== id));
    toast.error('Agendamento cancelado');
  };

  if (items.length === 0) {
    return (
      <>
        <Card className="rounded-lg">
          <CardContent className="p-12 text-center">
            <Megaphone className="w-12 h-12 mx-auto text-muted-foreground/50 mb-4" />
            <h3 className="font-semibold text-lg mb-2">
              Nenhum comunicado agendado ou em aprovação
            </h3>
            <p className="text-muted-foreground text-sm">
              Comunicados agendados e os que precisam de aprovação aparecerão
              aqui
            </p>
          </CardContent>
        </Card>
      </>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-4 mb-6">
        <div className="flex flex-col sm:flex-row flex-wrap items-stretch sm:items-center gap-3">
          <div className="relative flex-1 min-w-[200px] max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
            <Input
              type="search"
              placeholder="Buscar por título, autor ou conteúdo..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pl-9 rounded-lg"
            />
          </div>
          <div className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-pillToken gap-2 shadow-sm">
            <Button
              variant="ghost"
              size="sm"
              className={`rounded-pillToken py-2.5 px-4 text-sm font-medium transition-all duration-200 ${
                statusFilter === 'all'
                  ? 'bg-primary text-primary-foreground shadow-md scale-[1.02] hover:bg-primary hover:text-primary-foreground'
                  : 'text-muted-foreground hover:text-foreground hover:bg-muted/50'
              }`}
              onClick={() => setStatusFilter('all')}
            >
              Todos
            </Button>
            <Button
              variant="ghost"
              size="sm"
              className={`rounded-pillToken py-2.5 px-4 text-sm font-medium transition-all duration-200 ${
                statusFilter === 'scheduled'
                  ? 'bg-primary text-primary-foreground shadow-md scale-[1.02] hover:bg-primary hover:text-primary-foreground'
                  : 'text-muted-foreground hover:text-foreground hover:bg-muted/50'
              }`}
              onClick={() => setStatusFilter('scheduled')}
            >
              Agendados
            </Button>
            <Button
              variant="ghost"
              size="sm"
              className={`rounded-pillToken py-2.5 px-4 text-sm font-medium transition-all duration-200 ${
                statusFilter === 'pending_approval'
                  ? 'bg-primary text-primary-foreground shadow-md scale-[1.02] hover:bg-primary hover:text-primary-foreground'
                  : 'text-muted-foreground hover:text-foreground hover:bg-muted/50'
              }`}
              onClick={() => setStatusFilter('pending_approval')}
            >
              Aguardando aprovação
            </Button>
          </div>
        </div>
      </div>

      {searchFilteredItems.length === 0 ? (
        <Card className="rounded-lg">
          <CardContent className="p-8 text-center">
            <Megaphone className="w-10 h-10 mx-auto text-muted-foreground/50 mb-3" />
            <p className="text-muted-foreground">
              {searchQuery.trim()
                ? 'Nenhum comunicado encontrado para esta busca.'
                : statusFilter === 'all'
                  ? 'Nenhum comunicado agendado ou em aprovação'
                  : statusFilter === 'scheduled'
                    ? 'Nenhum comunicado agendado'
                    : 'Nenhum comunicado aguardando aprovação'}
            </p>
          </CardContent>
        </Card>
      ) : (
        searchFilteredItems.map((announcement) => {
          const isPendingApproval = announcement.status === 'pending_approval';
          const mediaAttachments = (announcement.attachments ?? []).filter(isMediaAttachment);
          const documentAttachments = (announcement.attachments ?? []).filter(
            (a) => a.type === 'document',
          );
          const authorInitials = announcement.authorName
            .split(' ')
            .filter(Boolean)
            .map((n) => n[0])
            .slice(0, 2)
            .join('')
            .toUpperCase() || '?';
          return (
            <Card
              key={announcement.id}
              className={`overflow-hidden transition-all hover:shadow-md rounded-xl ${
                isPendingApproval
                  ? 'border-2 border-orange-400/70 bg-orange-50/80 dark:bg-orange-950/30 dark:border-orange-500/50'
                  : 'border border-border'
              }`}
            >
              <CardHeader className="pb-3">
                <div className="flex items-start justify-between gap-4">
                  <div className="flex items-center gap-3">
                    <Avatar className="h-10 w-10 shrink-0">
                      {announcement.authorAvatar ? (
                        <AvatarImage
                          src={announcement.authorAvatar}
                          alt={announcement.authorName}
                        />
                      ) : null}
                      <AvatarFallback className="bg-gradient-to-br from-primary to-accent text-primary-foreground text-sm">
                        {authorInitials}
                      </AvatarFallback>
                    </Avatar>
                    <div>
                      <div className="flex items-center gap-2 mb-1">
                        <span className="font-semibold text-sm">
                          {announcement.authorName}
                        </span>
                      </div>
                      <div className="flex flex-wrap items-center gap-2 text-xs text-muted-foreground">
                        {announcement.targetUserGroupNames.length > 0 ? (
                          announcement.targetUserGroupNames.slice(0, 2).map((name, i) => (
                            <Badge
                              key={i}
                              variant="outline"
                              className="text-xs rounded-lg"
                            >
                              {name}
                            </Badge>
                          ))
                        ) : (
                          <Badge variant="outline" className="text-xs rounded-lg">
                            Comunicado
                          </Badge>
                        )}
                        {announcement.targetUserGroupNames.length > 2 && (
                          <span className="text-muted-foreground">
                            +{announcement.targetUserGroupNames.length - 2}
                          </span>
                        )}
                        <span>•</span>
                        {isPendingApproval ? (
                          <Badge
                            variant="outline"
                            className="text-xs rounded-lg border-orange-400/60 bg-orange-100 text-orange-800 dark:bg-orange-900/40 dark:text-orange-300 dark:border-orange-500/50"
                          >
                            Aguardando Aprovação
                          </Badge>
                        ) : (
                          <>
                            <Badge
                              variant="outline"
                              className="text-xs rounded-lg bg-muted/60"
                            >
                              <Calendar className="w-3 h-3 mr-1" />
                              Agendado
                            </Badge>
                            {announcement.scheduledAt && (
                              <span>
                                {format(
                                  new Date(announcement.scheduledAt),
                                  "d 'de' MMM 'às' HH:mm",
                                  { locale: ptBR },
                                )}
                              </span>
                            )}
                          </>
                        )}
                      </div>
                    </div>
                  </div>
                  {(onEditar || onArquivar) && (
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 shrink-0"
                          onClick={(e) => e.stopPropagation()}
                        >
                          <MoreHorizontal className="w-4 h-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        {onEditar && (
                          <DropdownMenuItem
                            disabled={editarDesabilitado?.(announcement)}
                            title={editarDesabilitado?.(announcement) ? 'Edição indisponível: já existem confirmações de leitura' : undefined}
                            onClick={(e) => {
                              e.preventDefault();
                              if (!editarDesabilitado?.(announcement)) onEditar(announcement);
                            }}
                          >
                            <Edit className="w-4 h-4 mr-2" />
                            Editar
                          </DropdownMenuItem>
                        )}
                        {onArquivar && (
                          <DropdownMenuItem
                            className="text-destructive"
                            onClick={(e) => {
                              e.preventDefault();
                              onArquivar(announcement);
                            }}
                          >
                            <Archive className="w-4 h-4 mr-2" />
                            Arquivar
                          </DropdownMenuItem>
                        )}
                      </DropdownMenuContent>
                    </DropdownMenu>
                  )}
                </div>
              </CardHeader>
              <CardContent className="space-y-4 pt-0">
                <h3 className="text-lg font-semibold leading-tight">
                  {announcement.title}
                </h3>
                <div
                  className="text-sm text-muted-foreground line-clamp-2 prose prose-sm dark:prose-invert max-w-none"
                  dangerouslySetInnerHTML={{ __html: announcement.content }}
                />
                {mediaAttachments.length > 0 && (
                  <div className="-mx-2 sm:-mx-4 relative rounded-lg overflow-hidden bg-muted/30">
                    {mediaAttachments.length === 1 ? (
                      <div className="relative w-full overflow-hidden aspect-video max-h-[320px]">
                        {mediaAttachments[0].type === 'image' ? (
                          <img
                            src={mediaAttachments[0].url}
                            alt={mediaAttachments[0].name}
                            className="w-full h-full object-contain object-center"
                          />
                        ) : (
                          <video
                            src={mediaAttachments[0].url}
                            controls
                            className="w-full h-full object-contain"
                          />
                        )}
                      </div>
                    ) : (
                      <Carousel className="w-full" opts={{ align: 'start', loop: true }}>
                        <CarouselContent className="-ml-0">
                          {mediaAttachments.map((att) => (
                            <CarouselItem key={att.id} className="pl-0">
                              <div className="w-full overflow-hidden min-h-[200px] max-h-[320px] aspect-video">
                                {att.type === 'image' ? (
                                  <img
                                    src={att.url}
                                    alt={att.name}
                                    className="w-full h-full object-contain object-center"
                                  />
                                ) : (
                                  <video
                                    src={att.url}
                                    controls
                                    className="w-full h-full object-contain"
                                  />
                                )}
                              </div>
                            </CarouselItem>
                          ))}
                        </CarouselContent>
                        <CarouselPrevious className="left-2 h-8 w-8 rounded-full border-2 bg-background/80 hover:bg-background" />
                        <CarouselNext className="right-2 h-8 w-8 rounded-full border-2 bg-background/80 hover:bg-background" />
                      </Carousel>
                    )}
                  </div>
                )}
                {documentAttachments.length > 0 && (
                  <div className="flex flex-wrap gap-2">
                    {documentAttachments.map((attachment) => (
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
                          {attachment.size != null && attachment.size > 0 && (
                            <span className="text-xs text-muted-foreground">
                              {attachment.size >= 1024 * 1024
                                ? `${(attachment.size / 1024 / 1024).toFixed(2)} MB`
                                : `${(attachment.size / 1024).toFixed(1)} KB`}
                            </span>
                          )}
                        </div>
                      </a>
                    ))}
                  </div>
                )}
                {isPendingApproval && (
                  <p className="text-sm text-orange-700 dark:text-orange-400">
                    Este comunicado foi criado por um usuário de um grupo que
                    possui &quot;Requer aprovação de comunicados&quot;. Aprove ou
                    rejeite.
                  </p>
                )}
                <div className="flex flex-wrap items-center gap-2 pt-1 border-t border-border/50">
                  {isPendingApproval ? (
                    <>
                      <Button
                        size="sm"
                        onClick={() => void handleApprove(announcement.id)}
                        disabled={actionId !== null}
                        className="gap-2 rounded-lg"
                      >
                        {actionId === announcement.id ? (
                          <Spinner className="h-4 w-4" />
                        ) : (
                          <CheckCircle className="w-4 h-4" />
                        )}
                        {announcement.scheduledAt ? 'Aprovar' : 'Aprovar e publicar'}
                      </Button>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => openRejectModal(announcement)}
                        disabled={actionId !== null}
                        className="gap-2 text-destructive hover:text-destructive hover:bg-destructive/10 rounded-lg"
                      >
                        {actionId === announcement.id ? (
                          <Spinner className="h-4 w-4" />
                        ) : (
                          <XCircle className="w-4 h-4" />
                        )}
                        Rejeitar
                      </Button>
                    </>
                  ) : (
                    <>
                      <Button
                        size="sm"
                        onClick={() => void handlePublishNow(announcement.id)}
                        disabled={actionId !== null}
                        className="gap-2 rounded-lg"
                      >
                        {actionId === announcement.id ? (
                          <Spinner className="h-4 w-4" />
                        ) : (
                          <Send className="w-4 h-4" />
                        )}
                        Publicar agora
                      </Button>
                      <AlertDialog>
                        <AlertDialogTrigger asChild>
                          <Button
                            variant="outline"
                            size="sm"
                            className="gap-2 text-destructive hover:text-destructive rounded-lg"
                          >
                            <Trash2 className="w-4 h-4" />
                            Cancelar
                          </Button>
                        </AlertDialogTrigger>
                        <AlertDialogContent className="rounded-lg">
                          <AlertDialogHeader>
                            <AlertDialogTitle>
                              Cancelar agendamento?
                            </AlertDialogTitle>
                            <AlertDialogDescription>
                              O comunicado agendado será removido da lista. Esta
                              ação não pode ser desfeita.
                            </AlertDialogDescription>
                          </AlertDialogHeader>
                          <AlertDialogFooter>
                            <AlertDialogCancel className="rounded-lg">
                              Voltar
                            </AlertDialogCancel>
                            <AlertDialogAction
                              onClick={() => handleDelete(announcement.id)}
                              className="bg-destructive text-destructive-foreground hover:bg-destructive/90 rounded-lg"
                            >
                              Sim, cancelar
                            </AlertDialogAction>
                          </AlertDialogFooter>
                        </AlertDialogContent>
                      </AlertDialog>
                    </>
                  )}
                </div>
              </CardContent>
            </Card>
          );
        })
      )}

      <Dialog open={!!announcementToReject} onOpenChange={(open) => !open && closeRejectModal()}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Rejeitar comunicado</DialogTitle>
            <DialogDescription>
              Informe o motivo da rejeição. Este campo é obrigatório.
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-2 py-2">
            <Label htmlFor="motivo-rejeicao-scheduled">Justificativa</Label>
            <Textarea
              id="motivo-rejeicao-scheduled"
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
              disabled={!motivoRejeicao.trim() || actionId !== null}
              onClick={() =>
                announcementToReject &&
                void handleReject(announcementToReject.id, motivoRejeicao)
              }
            >
              {actionId === announcementToReject?.id ? (
                <Spinner className="h-4 w-4 shrink-0" />
              ) : (
                'Rejeitar'
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
