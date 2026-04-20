import { useState, useRef, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  FileText,
  Users,
  Upload,
  Image as ImageIcon,
  Video,
  Archive,
  X,
} from 'lucide-react';
import type { CommunityGroup, CommunityPost, PostAttachment } from '@domain/entities/comunicacao';
import { AdicionarAnexosPublicacaoUseCase } from '@domain/usecases/AdicionarAnexosPublicacaoUseCase';
import { AtualizarPublicacaoUseCase } from '@domain/usecases/AtualizarPublicacaoUseCase';
import { CriarPublicacaoUseCase } from '@domain/usecases/CriarPublicacaoUseCase';
import { ExcluirAnexoPublicacaoUseCase } from '@domain/usecases/ExcluirAnexoPublicacaoUseCase';
import { ObterPublicacaoPorIdUseCase } from '@domain/usecases/ObterPublicacaoPorIdUseCase';
import { useAppSelector } from '@app/store/hooks';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent } from '@/components/ui/card';
import { Switch } from '@/components/ui/switch';
import { Separator } from '@/components/ui/separator';
import { RichTextEditor } from './RichTextEditor';
import { toast } from 'sonner';
import { Spinner } from '@/components/ui/spinner';

type AnexoTipo = 'imagem' | 'video' | 'documento';

function inferirTipoAnexo(file: File): AnexoTipo {
  const type = (file.type || '').toLowerCase();
  if (type.startsWith('image/')) return 'imagem';
  if (type.startsWith('video/')) return 'video';
  return 'documento';
}

interface AnexoItem {
  file: File;
  tipo: AnexoTipo;
}

interface CreatePostModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  group: CommunityGroup;
  /** Quando definido, abre o modal em modo edição (preenche título/conteúdo e ao salvar chama PUT). */
  editPost?: CommunityPost | null;
  /** Chamado após publicar/atualizar com sucesso. Se receber post, adiciona ao estado; se não, o parent deve refazer o fetch do feed. */
  onSave?: (post?: CommunityPost) => void;
}

/** Extrai o id da publicação do retorno da API de criação (pode vir como id ou publicacaoId). */
function getCreatedPostId(retorno: unknown): string | undefined {
  if (!retorno || typeof retorno !== 'object') return undefined;
  const r = retorno as Record<string, unknown>;
  const id = r.id ?? r.publicacaoId;
  return typeof id === 'string' && id.trim() !== '' ? id : undefined;
}

/** Monta um post mínimo para exibição otimista no feed quando a API ainda não retornou o post completo. */
function buildStubPost(
  id: string,
  title: string,
  content: string,
  group: CommunityGroup,
  user: { nomeCompleto?: string; colaborador?: { codigoColaboradorInterno?: string } } | null | undefined,
  subtitulo?: string,
): CommunityPost {
  const now = new Date().toISOString();
  return {
    id,
    groupId: group.id,
    groupName: group.name,
    authorId: user?.colaborador?.codigoColaboradorInterno ?? '',
    authorName: user?.nomeCompleto ?? 'Você',
    type: 'text',
    title: title.trim() || 'Publicação',
    ...(subtitulo?.trim() ? { subtitulo: subtitulo.trim() } : {}),
    content: content.trim() || '',
    attachments: [],
    status: 'published',
    visibility: { type: 'all' },
    requiresAcknowledgment: false,
    allowComments: true,
    allowLikes: true,
    isPinned: false,
    likesCount: 0,
    commentsCount: 0,
    viewsCount: 0,
    acknowledgmentCount: 0,
    createdAt: now,
    updatedAt: now,
    comunidadeId: group.id,
    comunidadeNome: group.name,
  };
}

export function CreatePostModal({
  open,
  onOpenChange,
  group,
  editPost = null,
  onSave,
}: CreatePostModalProps) {
  const token = useAppSelector((state) => state.auth.token);
  const user = useAppSelector((state) => state.auth.user);
  const [title, setTitle] = useState('');
  const [subtitulo, setSubtitulo] = useState('');
  const [content, setContent] = useState('');
  const [anexos, setAnexos] = useState<AnexoItem[]>([]);
  const [anexosToDelete, setAnexosToDelete] = useState<string[]>([]);
  const [ativo, setAtivo] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const isEdit = !!editPost;

  const existingAttachments =
    isEdit && editPost?.attachments?.length
      ? editPost.attachments.filter((a) => !anexosToDelete.includes(a.id))
      : [];

  useEffect(() => {
    if (open && editPost) {
      setTitle(editPost.title ?? '');
      setSubtitulo(editPost.subtitulo ?? '');
      setContent(editPost.content ?? '');
      setAnexos([]);
      setAnexosToDelete([]);
      setAtivo(String(editPost.status ?? '').toLowerCase() !== 'archived');
    } else if (open && !editPost) {
      setAtivo(true);
      setAnexosToDelete([]);
    }
    if (!open) {
      setTitle('');
      setSubtitulo('');
      setContent('');
      setAnexos([]);
      setAnexosToDelete([]);
      setAtivo(true);
    }
  }, [open, editPost]);

  const buildFormData = (): FormData => {
    const form = new FormData();
    form.append('tipo', 'informativo');
    form.append('titulo', title.trim());
    if (subtitulo.trim()) form.append('subtitulo', subtitulo.trim());
    form.append('conteudo', content.trim());
    form.append('comunidadeId', group.id);
    form.append('configuracaoInteracao.ocultarNoFeed', String(isEdit && editPost?.ocultarNoFeed === true));
    form.append('configuracaoInteracao.requerConfirmacaoLeitura', 'false');
    form.append('configuracaoInteracao.permiteComentarios', 'true');
    form.append('configuracaoInteracao.permiteCurtidas', 'true');
    form.append('configuracaoInteracao.fixada', 'false');
    if (!isEdit) {
      form.append('autoriaTipo', 'pessoal');
      anexos.forEach((item, index) => {
        form.append(`anexos[${index}].tipo`, item.tipo);
        form.append(`anexos[${index}].nomeArquivo`, item.file.name);
        form.append('anexosUpload', item.file, item.file.name);
      });
    }
    return form;
  };

  const buildFormDataAnexos = (): FormData => {
    const form = new FormData();
    anexos.forEach((item, index) => {
      form.append(`anexos[${index}].tipo`, item.tipo);
      form.append('anexosUpload', item.file, item.file.name);
    });
    return form;
  };

  const resetForm = () => {
    setTitle('');
    setSubtitulo('');
    setContent('');
    setAnexos([]);
    setAnexosToDelete([]);
    setAtivo(true);
  };

  const handleSave = async () => {
    if (!token) {
      toast.error('Faça login para publicar.');
      return;
    }
    setSubmitting(true);
    try {
      if (isEdit && editPost) {
        const formData = buildFormData();
        formData.append('ativo', ativo ? 'true' : 'false');
        const atualizarUseCase = container.resolve(AtualizarPublicacaoUseCase);
        const res = await atualizarUseCase.execute(token, editPost.id, formData);
        if (!res.sucesso) {
          toast.error(res.mensagem ?? 'Não foi possível atualizar a publicação.');
          return;
        }
        const excluirAnexoUseCase = container.resolve(ExcluirAnexoPublicacaoUseCase);
        for (const anexoId of anexosToDelete) {
          await excluirAnexoUseCase.execute(token, editPost.id, anexoId);
        }
        if (anexos.length > 0) {
          const adicionarAnexosUseCase = container.resolve(AdicionarAnexosPublicacaoUseCase);
          const formAnexos = buildFormDataAnexos();
          await adicionarAnexosUseCase.execute(token, editPost.id, formAnexos);
        }
        toast.success(res.mensagem ?? 'Publicação atualizada com sucesso.');
        onSave?.();
        resetForm();
        onOpenChange(false);
      } else {
        const formData = buildFormData();
        const useCase = container.resolve(CriarPublicacaoUseCase);
        const res = await useCase.execute(token, formData);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Publicação criada com sucesso.');
          resetForm();
          onOpenChange(false);
          const newId = getCreatedPostId(res.retorno);
          let postToShow: CommunityPost | undefined;
          if (newId && token) {
            try {
              const obterPorId = container.resolve(ObterPublicacaoPorIdUseCase);
              postToShow = await obterPorId.execute(token, newId);
            } catch {
              postToShow = buildStubPost(newId, title, content, group, user, subtitulo);
            }
          } else {
            postToShow = buildStubPost(newId ?? `temp-${Date.now()}`, title, content, group, user, subtitulo);
          }
          onSave?.(postToShow);
        } else {
          toast.error(res.mensagem ?? 'Não foi possível criar a publicação.');
        }
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : (isEdit ? 'Erro ao atualizar.' : 'Erro ao publicar. Tente novamente.'));
    } finally {
      setSubmitting(false);
    }
  };

  const markAnexoForDeletion = (anexoId: string) => {
    setAnexosToDelete((prev) => (prev.includes(anexoId) ? prev : [...prev, anexoId]));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files ? Array.from(e.target.files) : [];
    const items: AnexoItem[] = files.map((file) => ({
      file,
      tipo: inferirTipoAnexo(file),
    }));
    setAnexos((prev) => [...prev, ...items]);
    e.target.value = '';
  };

  const removeAnexo = (index: number) => {
    setAnexos((prev) => prev.filter((_, i) => i !== index));
  };

  const isValid = title.trim().length > 0;

  const iconByTipo = (tipo: AnexoTipo) => {
    switch (tipo) {
      case 'imagem':
        return <ImageIcon className="h-4 w-4 text-primary shrink-0" />;
      case 'video':
        return <Video className="h-4 w-4 text-primary shrink-0" />;
      default:
        return <FileText className="h-4 w-4 text-muted-foreground shrink-0" />;
    }
  };

  const labelByTipo = (tipo: AnexoTipo) => {
    switch (tipo) {
      case 'imagem':
        return 'Imagem';
      case 'video':
        return 'Vídeo';
      default:
        return 'Documento';
    }
  };

  const iconForAttachment = (att: PostAttachment) => {
    if (att.type === 'image') return <ImageIcon className="h-4 w-4 text-primary shrink-0" />;
    if (att.type === 'video') return <Video className="h-4 w-4 text-primary shrink-0" />;
    return <FileText className="h-4 w-4 text-muted-foreground shrink-0" />;
  };
  const labelForAttachment = (att: PostAttachment) => {
    if (att.type === 'image') return 'Imagem';
    if (att.type === 'video') return 'Vídeo';
    return 'Documento';
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
        <DialogHeader className="pr-14">
          <div className="flex items-center justify-between gap-4">
            <div className="flex items-center gap-2 min-w-0">
              <FileText className="w-5 h-5 text-primary shrink-0" />
              <DialogTitle className="truncate">{isEdit ? 'Editar publicação' : 'Criar Novo Post'}</DialogTitle>
            </div>
            <Badge variant="secondary" className="font-normal shrink-0 max-w-[50%] truncate" title={group.name}>
              <Users className="w-3 h-3 mr-1 shrink-0" />
              <span className="truncate">{group.name}</span>
            </Badge>
          </div>
        </DialogHeader>

        <div className="flex-1 overflow-y-auto py-4">
          <div className="space-y-4">
            {isEdit && (
              <Card className="border-2 border-border">
                <CardContent className="p-4 space-y-3">
                  <div className="flex items-center justify-between gap-4">
                    <div className="flex items-start gap-3">
                      <div className="p-2 rounded-lg bg-muted">
                        <Archive className="w-4 h-4 text-muted-foreground" />
                      </div>
                      <div>
                        <p className="font-medium text-sm">Ativo / Inativo</p>
                        <p className="text-xs text-muted-foreground mt-1">
                          Ao desligar, a publicação será <strong>arquivada</strong> e deixará de ser exibida no feed.
                        </p>
                      </div>
                    </div>
                    <div className="flex items-center gap-2 shrink-0">
                      <span className="text-sm text-muted-foreground">
                        {ativo ? 'Ativo' : 'Inativo (arquivado)'}
                      </span>
                      <Switch
                        checked={ativo}
                        onCheckedChange={setAtivo}
                      />
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Título */}
            <div className="space-y-2">
              <Label htmlFor="post-title">
                Título <span className="text-destructive">*</span>
              </Label>
              <Input
                id="post-title"
                placeholder="Ex: Novo processo de aprovação de despesas"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                className="rounded-lg"
              />
            </div>

            {/* Subtítulo */}
            <div className="space-y-2">
              <Label htmlFor="post-subtitulo">Subtítulo</Label>
              <Input
                id="post-subtitulo"
                placeholder="Ex: Linha de apoio ao título (opcional)"
                value={subtitulo}
                onChange={(e) => setSubtitulo(e.target.value)}
                className="rounded-lg"
              />
            </div>

            {/* Conteúdo */}
            <div className="space-y-2">
              <Label>Conteúdo</Label>
              <RichTextEditor
                key={open ? (editPost?.id ?? 'create') : 'closed'}
                content={content}
                onChange={setContent}
                placeholder="Escreva o conteúdo do post..."
              />
              <input
                ref={fileInputRef}
                type="file"
                multiple
                accept="image/*,video/*,.pdf,.doc,.docx,.xls,.xlsx"
                className="hidden"
                onChange={handleFileChange}
              />
            {isEdit && existingAttachments.length > 0 && (
              <>
                <Separator />
                <div className="space-y-2">
                  <Label className="flex items-center gap-2">
                    <Upload className="w-4 h-4" />
                    Anexos existentes
                  </Label>
                  <p className="text-xs text-muted-foreground">
                    A exclusão será aplicada ao clicar em Salvar.
                  </p>
                  <ul className="space-y-2">
                    {existingAttachments.map((att) => (
                      <li
                        key={att.id}
                        className="flex items-center justify-between gap-2 text-sm py-2 px-3 rounded-lg bg-muted/50 border border-border"
                      >
                        <div className="flex items-center gap-2 min-w-0">
                          {iconForAttachment(att)}
                          <span className="truncate">{att.name}</span>
                          <Badge variant="secondary" className="text-xs shrink-0">
                            {labelForAttachment(att)}
                          </Badge>
                        </div>
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          className="h-7 px-2 text-destructive hover:text-destructive shrink-0"
                          onClick={() => markAnexoForDeletion(att.id)}
                        >
                          <X className="w-4 h-4 mr-1" />
                          Remover
                        </Button>
                      </li>
                    ))}
                  </ul>
                </div>
              </>
            )}
              <Button
                type="button"
                variant="outline"
                className="w-full gap-3 py-6 border-dashed border-2 hover:border-primary/50 hover:bg-primary/5 transition-colors"
                onClick={() => fileInputRef.current?.click()}
              >
                <span className="flex items-center gap-2 text-muted-foreground">
                  <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary/10 text-primary">
                    <ImageIcon className="h-4 w-4" />
                  </span>
                  <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary/10 text-primary">
                    <Video className="h-4 w-4" />
                  </span>
                  <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary/10 text-primary">
                    <FileText className="h-4 w-4" />
                  </span>
                </span>
                <span className="text-sm font-medium">
                  Adicionar fotos, vídeos ou documentos
                </span>
                {anexos.length > 0 && (
                  <Badge variant="secondary" className="shrink-0">
                    {anexos.length}
                  </Badge>
                )}
              </Button>
            </div>

            {/* Anexos (opcional) — lista dos arquivos já selecionados */}
            {anexos.length > 0 && (
              <>
                <Separator />
                <div className="space-y-2">
                <Label className="flex items-center gap-2">
                  <Upload className="w-4 h-4" />
                  Anexos adicionados ({anexos.length})
                </Label>
                <ul className="space-y-2">
                  {anexos.map((item, index) => (
                    <li
                      key={`${item.file.name}-${index}`}
                      className="flex items-center justify-between gap-2 text-sm py-2 px-3 rounded-lg bg-muted/50 border border-border"
                    >
                      <div className="flex items-center gap-2 min-w-0">
                        {iconByTipo(item.tipo)}
                        <span className="truncate">{item.file.name}</span>
                        <Badge variant="secondary" className="text-xs shrink-0">
                          {labelByTipo(item.tipo)}
                        </Badge>
                      </div>
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        className="h-7 px-2 text-destructive hover:text-destructive shrink-0"
                        onClick={() => removeAnexo(index)}
                      >
                        Remover
                      </Button>
                    </li>
                  ))}
                </ul>
                </div>
              </>
            )}
          </div>
        </div>

        {/* Footer */}
        <div className="flex items-center justify-end gap-2 pt-4 border-t shrink-0">
          <Button variant="ghost" onClick={() => onOpenChange(false)} disabled={submitting}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={!isValid || submitting}>
            {submitting ? (
              <Spinner className="h-4 w-4 mr-2" />
            ) : (
              <FileText className="w-4 h-4 mr-2" />
            )}
            {isEdit ? 'Salvar' : 'Publicar'}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
