import { useState, useEffect, useRef } from 'react';
import { container } from 'tsyringe';
import {
  X,
  Save,
  Megaphone,
  FileText,
  FolderOpen,
  FolderPlus,
  Paperclip,
  Trash2,
  File,
  Tag,
  Calendar,
  Users,
  CheckCircle2,
  MessageSquare,
  Heart,
  Archive,
} from 'lucide-react';
import type {
  Announcement,
  AnnouncementType,
  AutoriaTipo,
  PostAttachment,
  UserGroup,
} from '@domain/entities/comunicacao';
import { AdicionarAnexosPublicacaoUseCase } from '@domain/usecases/AdicionarAnexosPublicacaoUseCase';
import { AtualizarPublicacaoUseCase } from '@domain/usecases/AtualizarPublicacaoUseCase';
import { CriarPublicacaoUseCase } from '@domain/usecases/CriarPublicacaoUseCase';
import { ExcluirAnexoPublicacaoUseCase } from '@domain/usecases/ExcluirAnexoPublicacaoUseCase';
import { useAppSelector } from '@app/store/hooks';
import { useComunicacaoGrupos } from '@presentation/hooks/useComunicacaoGrupos';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Switch } from '@/components/ui/switch';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { RichTextEditor } from './RichTextEditor';
import { UserGroupsSelector } from './UserGroupsSelector';
import { useComunicacaoBibliotecaLabels } from '@presentation/hooks/useComunicacaoBibliotecaLabels';
import { useComunicacaoLabels } from '@presentation/hooks/useComunicacaoLabels';
import { toast } from 'sonner';
import { Spinner } from '@/components/ui/spinner';
import { cn } from '@/lib/utils';

interface CreateAnnouncementModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  userGroups?: UserGroup[];
  onSave?: (announcement: Announcement) => void;
  /** Quando definido, o modal abre em modo edição com o formulário populado. */
  initialAnnouncement?: Announcement | null;
}

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + ' B';
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
  return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
}

/** Infere o tipo do anexo pelo MIME do arquivo (mesma regra do modal Criar Novo Post). */
function inferirTipoAnexo(file: File): 'imagem' | 'video' | 'documento' {
  const type = (file.type || '').toLowerCase();
  if (type.startsWith('image/')) return 'imagem';
  if (type.startsWith('video/')) return 'video';
  return 'documento';
}

export function CreateAnnouncementModal({
  open,
  onOpenChange,
  userGroups: _userGroups = [],
  onSave,
  initialAnnouncement,
}: CreateAnnouncementModalProps) {
  const token = useAppSelector((state) => state.auth.token);
  const { grupos, loading: loadingGrupos, error: errorGrupos, loadGrupos } = useComunicacaoGrupos();

  const [activeTab, setActiveTab] = useState<'content' | 'settings'>('content');
  const [originUserGroupId, setOriginUserGroupId] = useState<string>('');
  const [submitting, setSubmitting] = useState(false);
  const isEdit = !!initialAnnouncement;

  const [announcementType, setAnnouncementType] = useState<AnnouncementType>('informative');
  const { labels } = useComunicacaoBibliotecaLabels({
    enabled: announcementType === 'document',
  });
  const folderOptions = labels.map((l) => l.nome);
  const { labels: tagLabelOptions } = useComunicacaoLabels({ enabled: open });

  useEffect(() => {
    if (open) void loadGrupos();
  }, [open, loadGrupos]);

  // Preenche o formulário ao abrir em modo edição.
  useEffect(() => {
    if (!open || !initialAnnouncement) return;
    const a = initialAnnouncement;
    setTitle(a.title ?? '');
    setSubtitulo(a.subtitulo ?? '');
    setAutoriaTipo(a.autoriaTipo ?? 'pessoal');
    setContent(a.content ?? '');
    setAnnouncementType(a.announcementType ?? 'informative');
    setSelectedUserGroups(a.targetUserGroupIds ?? []);
    setEnviarPara(a.targetUserGroupIds?.length ? 'grupos' : 'todos');
    setAttachments(a.attachments ?? []);
    setAttachmentFiles([]);
    setTags(a.tags ?? []);
    setTagInput('');
    setFolderPaths((a.folderPaths ?? (a.folderPath ? [a.folderPath] : [])).filter((p): p is string => p != null));
    setFolderSelectValue('');
    setNewFolderName('');
    setShowNewFolderInput(false);
    setStatus(a.status === 'scheduled' ? 'scheduled' : 'published');
    if (a.scheduledAt) {
      const d = new Date(a.scheduledAt);
      setScheduledDate(d.toISOString().slice(0, 10));
      setScheduledTime(d.toTimeString().slice(0, 5));
    } else {
      setScheduledDate('');
      setScheduledTime('');
    }
    setExpiresDate(a.expiresAt ? a.expiresAt.slice(0, 10) : '');
    setOcultarNoFeed(a.ocultarNoFeed ?? false);
    setRequiresAcknowledgment(a.requiresAcknowledgment ?? false);
    setAllowComments(a.allowComments ?? true);
    setAllowLikes(a.allowLikes ?? true);
    setIsPinned(a.isPinned ?? false);
    setAllowDownload(a.allowDownload ?? true);
    setAtivo(String(a.status ?? '').toLowerCase() !== 'archived');
    setAnexosToDelete([]);
    setActiveTab('content');
  }, [open, initialAnnouncement]);

  // Reseta o formulário ao abrir em modo criação (sem initialAnnouncement).
  useEffect(() => {
    if (open && !initialAnnouncement) {
      setActiveTab('content');
      setOriginUserGroupId('');
      setAnnouncementType('informative');
      setTitle('');
      setSubtitulo('');
      setAutoriaTipo('pessoal');
      setContent('');
      setSelectedUserGroups([]);
      setEnviarPara('todos');
      setAttachments([]);
      setAttachmentFiles([]);
      setTags([]);
      setTagInput('');
      setFolderPaths([]);
      setFolderSelectValue('');
      setNewFolderName('');
      setShowNewFolderInput(false);
      setStatus('published');
      setScheduledDate('');
      setScheduledTime('');
      setExpiresDate('');
      setOcultarNoFeed(false);
      setRequiresAcknowledgment(false);
      setAllowComments(true);
      setAllowLikes(true);
      setIsPinned(false);
      setAllowDownload(true);
      setAtivo(true);
      setAnexosToDelete([]);
    }
  }, [open, initialAnnouncement]);

  const [ativo, setAtivo] = useState(true);
  const [title, setTitle] = useState('');
  const [subtitulo, setSubtitulo] = useState('');
  const [autoriaTipo, setAutoriaTipo] = useState<AutoriaTipo>('pessoal');
  const [content, setContent] = useState('');
  const [selectedUserGroups, setSelectedUserGroups] = useState<string[]>([]);

  const [anexosToDelete, setAnexosToDelete] = useState<string[]>([]);
  const [attachments, setAttachments] = useState<PostAttachment[]>([]);
  const [attachmentFiles, setAttachmentFiles] = useState<File[]>([]);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [tags, setTags] = useState<string[]>([]);
  const [tagInput, setTagInput] = useState('');

  const [folderPaths, setFolderPaths] = useState<string[]>([]);
  const [folderSelectValue, setFolderSelectValue] = useState('');
  const [newFolderName, setNewFolderName] = useState('');
  const [showNewFolderInput, setShowNewFolderInput] = useState(false);

  const [status, setStatus] = useState<'published' | 'scheduled'>('published');
  const [scheduledDate, setScheduledDate] = useState('');
  const [scheduledTime, setScheduledTime] = useState('');
  const [expiresDate, setExpiresDate] = useState('');
  const [ocultarNoFeed, setOcultarNoFeed] = useState(false);
  const [requiresAcknowledgment, setRequiresAcknowledgment] = useState(false);
  const [allowComments, setAllowComments] = useState(true);
  const [allowLikes, setAllowLikes] = useState(true);
  const [isPinned, setIsPinned] = useState(false);
  const [allowDownload, setAllowDownload] = useState(true);
  const [enviarPara, setEnviarPara] = useState<'todos' | 'grupos'>('todos');

  const originGrupo = originUserGroupId
    ? grupos.find((g) => g.id === originUserGroupId)
    : null;
  const goesToApproval = Boolean(originGrupo?.publicacaoInformativoRequerAprovacao);

  const toggleUserGroup = (groupId: string) => {
    setSelectedUserGroups((prev) =>
      prev.includes(groupId) ? prev.filter((id) => id !== groupId) : [...prev, groupId],
    );
  };

  const handleReset = () => {
    setActiveTab('content');
    setOriginUserGroupId('');
    setAnnouncementType('informative');
    setTitle('');
    setSubtitulo('');
    setAutoriaTipo('pessoal');
    setContent('');
    setSelectedUserGroups([]);
    setEnviarPara('todos');
    setAnexosToDelete([]);
    setAttachments([]);
    setAttachmentFiles([]);
    setTags([]);
    setTagInput('');
    setFolderPaths([]);
    setFolderSelectValue('');
    setNewFolderName('');
    setShowNewFolderInput(false);
    setStatus('published');
    setScheduledDate('');
    setScheduledTime('');
    setExpiresDate('');
    setOcultarNoFeed(false);
    setRequiresAcknowledgment(false);
    setAllowComments(true);
    setAllowLikes(true);
    setIsPinned(false);
    setAllowDownload(true);
  };

  const handleAddAttachmentClick = () => {
    fileInputRef.current?.click();
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files ? Array.from(e.target.files) : [];
    if (files.length === 0) return;
    const newAttachments: PostAttachment[] = files.map((file) => ({
      id: `att-${Date.now()}-${file.name}`,
      type: 'document',
      url: '',
      name: file.name,
      size: file.size,
      mimeType: file.type || undefined,
    }));
    setAttachments((prev) =>
      announcementType === 'document' ? newAttachments : [...prev, ...newAttachments],
    );
    setAttachmentFiles((prev) =>
      announcementType === 'document' ? files : [...prev, ...files],
    );
    e.target.value = '';
  };

  const handleRemoveAttachment = (index: number) => {
    setAttachments((prev) => prev.filter((_, i) => i !== index));
    setAttachmentFiles((prev) => prev.filter((_, i) => i !== index));
  };

  const handleAddTag = () => {
    const trimmedTag = tagInput.trim();
    if (trimmedTag && !tags.includes(trimmedTag)) {
      setTags((prev) => [...prev, trimmedTag]);
      setTagInput('');
    }
  };

  const handleRemoveTag = (tagToRemove: string) => {
    setTags((prev) => prev.filter((tag) => tag !== tagToRemove));
  };

  const handleTagInputKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleAddTag();
    }
  };

  const handleAddFolder = (path: string) => {
    if (!path || folderPaths.includes(path)) return;
    setFolderPaths((prev) => [...prev, path]);
    setFolderSelectValue('');
  };

  const handleRemoveFolder = (pathToRemove: string) => {
    setFolderPaths((prev) => prev.filter((p) => p !== pathToRemove));
  };

  const handleCreateFolder = () => {
    const trimmedFolder = newFolderName.trim();
    if (trimmedFolder && !folderPaths.includes(trimmedFolder)) {
      setFolderPaths((prev) => [...prev, trimmedFolder]);
      setNewFolderName('');
      setShowNewFolderInput(false);
      toast.success(`A pasta "${trimmedFolder}" foi criada com sucesso.`);
    }
  };

  const buildFormDataForComunicado = (): FormData => {
    const form = new FormData();
    const tipo = announcementType === 'document' ? 'documento' : 'informativo';
    form.append('tipo', tipo);
    form.append('titulo', title.trim());
    if (subtitulo.trim()) form.append('subtitulo', subtitulo.trim());
    form.append('autoriaTipo', autoriaTipo);
    form.append('conteudo', content.trim());
    form.append('configuracaoInteracao.ocultarNoFeed', String(ocultarNoFeed));
    form.append('configuracaoInteracao.requerConfirmacaoLeitura', String(requiresAcknowledgment));
    form.append('configuracaoInteracao.permiteComentarios', String(allowComments));
    form.append('configuracaoInteracao.permiteCurtidas', String(allowLikes));
    form.append('configuracaoInteracao.permiteDownload', String(allowDownload));
    form.append('configuracaoInteracao.fixada', String(isPinned));
    if (enviarPara === 'todos') {
      form.append('grupoIds', '[]');
    } else {
      selectedUserGroups.forEach((id) => form.append('grupoIds', id));
    }
    if (announcementType === 'document' && folderPaths.length > 0) {
      folderPaths.forEach((path) => form.append('labels', path));
    }
    if (announcementType === 'informative' && tags.length > 0) {
      tags.forEach((tag) => form.append('labels', tag));
    }
    if (status === 'scheduled' && scheduledDate && scheduledTime) {
      const dataAgendamentoPublicacao = `${scheduledDate}T${scheduledTime}:00`;
      form.append('dataAgendamentoPublicacao', dataAgendamentoPublicacao);
    }
    form.append('dataValidadePublicacao', expiresDate ? `${expiresDate}T23:59:59` : '');
    // Em edição, anexos são enviados via DELETE/POST após o PUT.
    if (!isEdit) {
      attachmentFiles.forEach((file, index) => {
        const anexoTipo = inferirTipoAnexo(file);
        form.append(`anexos[${index}].tipo`, anexoTipo);
        form.append(`anexos[${index}].nomeArquivo`, file.name);
        form.append('anexosUpload', file, file.name);
      });
    }
    return form;
  };

  const buildFormDataAnexosComunicado = (): FormData => {
    const form = new FormData();
    attachmentFiles.forEach((file, index) => {
      const anexoTipo = inferirTipoAnexo(file);
      form.append(`anexos[${index}].tipo`, anexoTipo);
      form.append(`anexos[${index}].nomeArquivo`, file.name);
      form.append('anexosUpload', file, file.name);
    });
    return form;
  };

  const markAnexoForDeletion = (anexoId: string) => {
    setAnexosToDelete((prev) => (prev.includes(anexoId) ? prev : [...prev, anexoId]));
  };

  const handleRemoveNewFile = (index: number) => {
    setAttachmentFiles((prev) => prev.filter((_, i) => i !== index));
    setAttachments((prev) => {
      const initialIds = (initialAnnouncement?.attachments ?? []).map((a) => a.id);
      const newOnes = prev.filter((a) => !initialIds.includes(a.id));
      const kept = newOnes.filter((_, i) => i !== index);
      return prev.filter((a) => initialIds.includes(a.id)).concat(kept);
    });
  };

  const handleSubstituteDocument = () => {
    const existing = (initialAnnouncement?.attachments ?? []).filter(
      (a) => !anexosToDelete.includes(a.id),
    );
    if (existing[0]) markAnexoForDeletion(existing[0].id);
    fileInputRef.current?.click();
  };

  const handleSave = async () => {
    if (!token) {
      toast.error('Faça login para publicar.');
      return;
    }
    const isEnviarParaTodos = enviarPara === 'todos';
    const selectedGroups = isEnviarParaTodos
      ? []
      : grupos.filter((g) => selectedUserGroups.includes(g.id));
    const totalMembers = selectedGroups.reduce(
      (sum, g) => sum + (g.quantidadeParticipantes ?? 0),
      0,
    );
    const finalStatus = goesToApproval ? ('pending_approval' as const) : status;

    setSubmitting(true);
    try {
      const formData = buildFormDataForComunicado();
      if (isEdit && initialAnnouncement) {
        formData.append('ativo', ativo ? 'true' : 'false');
        const atualizarUseCase = container.resolve(AtualizarPublicacaoUseCase);
        const res = await atualizarUseCase.execute(token, initialAnnouncement.id, formData);
        if (!res.sucesso) {
          toast.error(res.mensagem ?? 'Não foi possível atualizar o comunicado.');
          setSubmitting(false);
          return;
        }
        const publicacaoId = initialAnnouncement.id;
        const excluirAnexoUseCase = container.resolve(ExcluirAnexoPublicacaoUseCase);
        for (const anexoId of anexosToDelete) {
          await excluirAnexoUseCase.execute(token, publicacaoId, anexoId);
        }
        if (attachmentFiles.length > 0) {
          const adicionarAnexosUseCase = container.resolve(AdicionarAnexosPublicacaoUseCase);
          const formAnexos = buildFormDataAnexosComunicado();
          await adicionarAnexosUseCase.execute(token, publicacaoId, formAnexos);
        }
        const remainingAttachments = (initialAnnouncement.attachments ?? []).filter(
          (a) => !anexosToDelete.includes(a.id),
        );
        const announcement: Announcement = {
          ...initialAnnouncement,
          title: title.trim(),
          subtitulo: subtitulo.trim() || undefined,
          autoriaTipo,
          content: content.trim(),
          targetUserGroupIds: isEnviarParaTodos ? [] : selectedUserGroups,
          targetUserGroupNames: selectedGroups.map((g) => g.nome),
          attachments: remainingAttachments,
          folderPaths: announcementType === 'document' && folderPaths.length > 0 ? folderPaths : undefined,
          tags: announcementType === 'informative' && tags.length > 0 ? tags : undefined,
          status: ativo ? finalStatus : 'archived',
          ativo,
          scheduledAt:
            status === 'scheduled' && scheduledDate && scheduledTime
              ? `${scheduledDate}T${scheduledTime}:00`
              : initialAnnouncement.scheduledAt,
          expiresAt: expiresDate ? `${expiresDate}T23:59:59` : initialAnnouncement.expiresAt,
          requiresAcknowledgment,
          allowComments,
          allowLikes,
          isPinned,
          allowDownload: announcementType === 'document' ? allowDownload : undefined,
          ocultarNoFeed,
          updatedAt: new Date().toISOString(),
        };
        onSave?.(announcement);
        toast.success(res.mensagem ?? 'Comunicado atualizado com sucesso.');
        handleReset();
        onOpenChange(false);
        setSubmitting(false);
        return;
      }
      const useCase = container.resolve(CriarPublicacaoUseCase);
      const res = await useCase.execute(token, formData);

      if (res.sucesso) {
        const createdId = res.retorno?.id ?? `an${Date.now()}`;
        const announcement: Announcement = {
          id: createdId,
          targetUserGroupIds: isEnviarParaTodos ? [] : selectedUserGroups,
          targetUserGroupNames: selectedGroups.map((g) => g.nome),
          authorId: 'current-user',
          authorName: 'Você',
          announcementType,
          title: title.trim(),
          subtitulo: subtitulo.trim() || undefined,
          autoriaTipo,
          content: content.trim(),
          attachments: [...attachments],
          folderPaths:
            announcementType === 'document' && folderPaths.length > 0 ? folderPaths : undefined,
          tags: announcementType === 'informative' && tags.length > 0 ? tags : undefined,
          status: finalStatus,
          scheduledAt:
            status === 'scheduled' && scheduledDate && scheduledTime
              ? `${scheduledDate}T${scheduledTime}:00`
              : undefined,
          publishedAt: finalStatus === 'published' ? new Date().toISOString() : undefined,
          expiresAt: expiresDate ? `${expiresDate}T23:59:59` : undefined,
          requiresAcknowledgment,
          allowComments,
          allowLikes,
          isPinned,
          allowDownload: announcementType === 'document' ? allowDownload : undefined,
          ocultarNoFeed,
          likesCount: 0,
          commentsCount: 0,
          viewsCount: 0,
          acknowledgmentCount: 0,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        };
        onSave?.(announcement);
        const typeLabel = announcementType === 'document' ? 'Documento' : 'Informativo';
        if (finalStatus === 'pending_approval') {
          toast.success('Comunicado enviado para aprovação');
        } else if (isEnviarParaTodos) {
          toast.success(
            status === 'scheduled'
              ? 'Comunicado agendado para toda a empresa'
              : `Comunicado ${typeLabel} criado e enviado para toda a empresa`,
          );
        } else {
          toast.success(
            status === 'scheduled'
              ? `Comunicado agendado para ${selectedGroups.length} grupo(s) (${totalMembers} pessoas)`
              : `Comunicado ${typeLabel} criado e enviado para ${selectedGroups.length} grupo(s) (${totalMembers} pessoas)`,
          );
        }
        handleReset();
        onOpenChange(false);
      } else {
        toast.error(res.mensagem ?? 'Não foi possível criar o comunicado.');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao publicar comunicado. Tente novamente.');
    } finally {
      setSubmitting(false);
    }
  };

  const documentHasFile =
    announcementType === 'document' &&
    (isEdit
      ? (initialAnnouncement?.attachments ?? []).filter((a) => !anexosToDelete.includes(a.id))
            .length > 0 || attachmentFiles.length > 0
      : attachments.length > 0);
  /** Com "Oculta no feed", destinatários ficam só leitura (definidos antes ou padrão). */
  const destinatariosInativos = ocultarNoFeed;
  const isValid =
    title.trim() &&
    content.trim() &&
    (enviarPara === 'todos' || selectedUserGroups.length > 0) &&
    (announcementType === 'informative' ||
      (announcementType === 'document' && documentHasFile && folderPaths.length > 0));
  const canSchedule = status === 'scheduled' ? scheduledDate && scheduledTime : true;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Megaphone className="w-5 h-5 text-primary" />
            {isEdit ? 'Editar Comunicado' : 'Criar Novo Comunicado'}
          </DialogTitle>
          <DialogDescription>
            {isEdit
              ? 'Altere o conteúdo e as configurações do comunicado.'
              : 'Envie comunicados diretos para grupos de trabalho específicos'}
          </DialogDescription>
        </DialogHeader>

        <input
          ref={fileInputRef}
          type="file"
          accept=".pdf,.doc,.docx,.xls,.xlsx,image/*"
          multiple={announcementType === 'informative'}
          className="hidden"
          onChange={handleFileSelect}
        />

        <Tabs
          value={activeTab}
          onValueChange={(v: string) => setActiveTab(v as 'content' | 'settings')}
          className="flex-1 flex flex-col overflow-hidden"
        >
          <TabsList className="grid w-full grid-cols-2 rounded-lg bg-muted/30 border border-border/50 p-1">
            <TabsTrigger
              value="content"
              className="rounded-lg data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
            >
              Conteúdo
            </TabsTrigger>
            <TabsTrigger
              value="settings"
              className="rounded-lg data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
            >
              Configurações
            </TabsTrigger>
          </TabsList>

          <div className="flex-1 overflow-y-auto py-4">
            <TabsContent value="content" className="space-y-4 mt-0">
              {/* Autoria */}
              <div className="space-y-3">
                <Label>Autoria</Label>
                <RadioGroup
                  value={autoriaTipo}
                  onValueChange={(v) => setAutoriaTipo(v as AutoriaTipo)}
                  className="flex flex-col gap-2"
                >
                  <label className="flex items-center gap-2 cursor-pointer rounded-lg border border-border/50 p-3 hover:bg-muted/30 has-[:checked]:border-primary has-[:checked]:bg-primary/5">
                    <RadioGroupItem value="pessoal" id="autoria-pessoal" />
                    <span className="text-sm font-medium">Pessoal (Publicado por mim)</span>
                  </label>
                  <label className="flex items-center gap-2 cursor-pointer rounded-lg border border-border/50 p-3 hover:bg-muted/30 has-[:checked]:border-primary has-[:checked]:bg-primary/5">
                    <RadioGroupItem value="alternativo" id="autoria-alternativo" />
                    <span className="text-sm font-medium">Alternativo (Publicado pela empresa)</span>
                  </label>
                </RadioGroup>
              </div>

              <Separator />

              {/* Tipo de Comunicado */}
              <div className="space-y-3">
                <Label>
                  Tipo de Comunicado <span className="text-destructive">*</span>
                </Label>
                <div className="grid grid-cols-2 gap-3">
                  <button
                    onClick={() => setAnnouncementType('informative')}
                    type="button"
                    className={`p-4 rounded-lg border-2 transition-all text-left ${
                      announcementType === 'informative'
                        ? 'border-primary bg-primary/10'
                        : 'border-border hover:border-primary/50'
                    }`}
                  >
                    <FileText
                      className={`w-6 h-6 mb-2 ${
                        announcementType === 'informative' ? 'text-primary' : 'text-muted-foreground'
                      }`}
                    />
                    <p className="font-medium text-sm">Informativo</p>
                    <p className="text-xs text-muted-foreground mt-1">
                      Comunicado geral que pode incluir vários documentos anexos
                    </p>
                  </button>
                  <button
                    onClick={() => setAnnouncementType('document')}
                    type="button"
                    className={`p-4 rounded-lg border-2 transition-all text-left ${
                      announcementType === 'document'
                        ? 'border-primary bg-primary/10'
                        : 'border-border hover:border-primary/50'
                    }`}
                  >
                    <FolderOpen
                      className={`w-6 h-6 mb-2 ${
                        announcementType === 'document' ? 'text-primary' : 'text-muted-foreground'
                      }`}
                    />
                    <p className="font-medium text-sm">Documento</p>
                    <p className="text-xs text-muted-foreground mt-1">
                      Um único documento organizado em pasta específica
                    </p>
                  </button>
                </div>
              </div>

              <Separator />

              {/* Título */}
              <div className="space-y-2">
                <Label htmlFor="title">
                  Título do Comunicado <span className="text-destructive">*</span>
                </Label>
                <Input
                  id="title"
                  placeholder="Ex: Atualização Importante - Manutenção de Sistemas"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  className="rounded-lg"
                />
              </div>

              {/* Subtítulo */}
              <div className="space-y-2">
                <Label htmlFor="subtitulo">Subtítulo</Label>
                <Input
                  id="subtitulo"
                  placeholder="Ex: Resumo ou linha de apoio ao título (opcional)"
                  value={subtitulo}
                  onChange={(e) => setSubtitulo(e.target.value)}
                  className="rounded-lg"
                />
              </div>

              {/* Conteúdo */}
              <div className="space-y-2">
                <Label>
                  Conteúdo <span className="text-destructive">*</span>
                </Label>
                <RichTextEditor
                  key={open ? 'open' : 'closed'}
                  content={content}
                  onChange={setContent}
                  placeholder="Escreva o conteúdo do comunicado..."
                />
              </div>

              <Separator />

              {announcementType === 'informative' && (
                <>
                  <div className="space-y-3">
                    <div className="flex items-center justify-between">
                      <Label className="flex items-center gap-2">
                        <Paperclip className="w-4 h-4" />
                        Documentos Anexos (opcional)
                      </Label>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={handleAddAttachmentClick}
                        className="gap-2 rounded-lg"
                      >
                        <Paperclip className="w-3 h-3" />
                        Adicionar Documento
                      </Button>
                    </div>

                    {isEdit &&
                      (initialAnnouncement?.attachments ?? []).filter(
                        (a) => !anexosToDelete.includes(a.id),
                      ).length > 0 && (
                      <Card>
                        <CardContent className="p-3 space-y-2">
                          <p className="text-xs font-medium text-muted-foreground mb-1">
                            Anexos existentes (remover será aplicado ao salvar)
                          </p>
                          {(initialAnnouncement?.attachments ?? [])
                            .filter((a) => !anexosToDelete.includes(a.id))
                            .map((file) => (
                              <div
                                key={file.id}
                                className="flex items-center gap-3 p-2 rounded-lg hover:bg-muted/50 transition-colors"
                              >
                                <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                                  <File className="w-5 h-5 text-primary" />
                                </div>
                                <div className="flex-1 min-w-0">
                                  <p className="text-sm font-medium truncate">{file.name}</p>
                                  <p className="text-xs text-muted-foreground">
                                    {file.size ? formatFileSize(file.size) : '—'}
                                  </p>
                                </div>
                                <Button
                                  type="button"
                                  variant="ghost"
                                  size="icon"
                                  className="h-8 w-8 text-destructive hover:text-destructive flex-shrink-0 rounded-lg"
                                  onClick={() => markAnexoForDeletion(file.id)}
                                  title="Remover anexo (será excluído ao salvar)"
                                >
                                  <Trash2 className="w-3 h-3" />
                                </Button>
                              </div>
                            ))}
                        </CardContent>
                      </Card>
                    )}

                    {((isEdit && attachmentFiles.length > 0) || (!isEdit && attachments.length > 0)) && (
                      <Card>
                        <CardContent className="p-3 space-y-2">
                          {isEdit && (
                            <p className="text-xs font-medium text-muted-foreground mb-1">
                              Novos anexos (serão adicionados ao salvar)
                            </p>
                          )}
                          {isEdit
                            ? attachmentFiles.map((file, index) => (
                                <div
                                  key={`new-${index}-${file.name}`}
                                  className="flex items-center gap-3 p-2 rounded-lg hover:bg-muted/50 transition-colors"
                                >
                                  <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                                    <File className="w-5 h-5 text-primary" />
                                  </div>
                                  <div className="flex-1 min-w-0">
                                    <p className="text-sm font-medium truncate">{file.name}</p>
                                    <p className="text-xs text-muted-foreground">
                                      {formatFileSize(file.size)}
                                    </p>
                                  </div>
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8 text-destructive hover:text-destructive flex-shrink-0 rounded-lg"
                                    onClick={() => handleRemoveNewFile(index)}
                                  >
                                    <Trash2 className="w-3 h-3" />
                                  </Button>
                                </div>
                              ))
                            : attachments.map((file, index) => (
                                <div
                                  key={file.id}
                                  className="flex items-center gap-3 p-2 rounded-lg hover:bg-muted/50 transition-colors"
                                >
                                  <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                                    <File className="w-5 h-5 text-primary" />
                                  </div>
                                  <div className="flex-1 min-w-0">
                                    <p className="text-sm font-medium truncate">{file.name}</p>
                                    <p className="text-xs text-muted-foreground">
                                      {file.size ? formatFileSize(file.size) : '—'}
                                    </p>
                                  </div>
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8 text-destructive hover:text-destructive flex-shrink-0 rounded-lg"
                                    onClick={() => handleRemoveAttachment(index)}
                                  >
                                    <Trash2 className="w-3 h-3" />
                                  </Button>
                                </div>
                              ))}
                        </CardContent>
                      </Card>
                    )}

                    <p className="text-xs text-muted-foreground">
                      Adicione múltiplos documentos de apoio ao comunicado informativo
                    </p>
                  </div>

                  <Separator />

                  <div className="space-y-3">
                    <Label className="flex items-center gap-2">
                      <Tag className="w-4 h-4" />
                      Tags (opcional)
                      {tags.length > 0 && (
                        <Badge variant="secondary" className="ml-auto">
                          {tags.length}
                        </Badge>
                      )}
                    </Label>
                    <div className="flex flex-wrap gap-2">
                      {tagLabelOptions.length > 0 && (
                        <Select
                          value=""
                          onValueChange={(value) => {
                            if (value && !tags.includes(value)) {
                              setTags((prev) => [...prev, value]);
                            }
                          }}
                        >
                          <SelectTrigger
                            className="w-[200px] rounded-lg"
                            disabled={
                              tagLabelOptions.filter((l) => !tags.includes(l.nome)).length === 0
                            }
                          >
                            <SelectValue placeholder="Selecionar tag..." />
                          </SelectTrigger>
                          <SelectContent>
                            {tagLabelOptions
                              .filter((l) => !tags.includes(l.nome))
                              .map((label) => (
                                <SelectItem
                                  key={label.id}
                                  value={label.nome}
                                  className="rounded-lg"
                                >
                                  {label.nome}
                                </SelectItem>
                              ))}
                          </SelectContent>
                        </Select>
                      )}
                      <Input
                        id="tagInput"
                        placeholder="Ou digite uma tag e pressione Enter"
                        value={tagInput}
                        onChange={(e) => setTagInput(e.target.value)}
                        onKeyDown={handleTagInputKeyDown}
                        className="w-[200px] rounded-lg"
                      />
                      <Button
                        type="button"
                        variant="outline"
                        onClick={handleAddTag}
                        disabled={!tagInput.trim()}
                        className="rounded-lg"
                      >
                        Adicionar
                      </Button>
                    </div>
                    {tags.length > 0 && (
                      <div className="flex flex-wrap gap-2">
                        {tags.map((tag) => (
                          <Badge
                            key={tag}
                            variant="secondary"
                            className="gap-1 pr-1 pl-3 rounded-lg"
                          >
                            {tag}
                            <Button
                              type="button"
                              variant="ghost"
                              size="icon"
                              className="h-4 w-4 hover:bg-destructive/20 ml-1 rounded-lg"
                              onClick={() => handleRemoveTag(tag)}
                            >
                              <X className="w-3 h-3" />
                            </Button>
                          </Badge>
                        ))}
                      </div>
                    )}
                    <p className="text-xs text-muted-foreground">
                      Selecione tags da lista ou digite para adicionar. Ex: &quot;Urgente&quot;, &quot;RH&quot;
                    </p>
                  </div>
                </>
              )}

              {announcementType === 'document' && (
                <>
                  <div className="space-y-3">
                    <Label className="flex items-center gap-2">
                      <File className="w-4 h-4" />
                      Documento <span className="text-destructive">*</span>
                    </Label>

                    {(() => {
                      const existingDocs = isEdit
                        ? (initialAnnouncement?.attachments ?? []).filter(
                            (a) => !anexosToDelete.includes(a.id),
                          )
                        : [];
                      const hasExisting = existingDocs.length > 0;
                      const hasNew = attachmentFiles.length > 0;
                      const hasDocument = isEdit ? hasExisting || hasNew : attachments.length > 0;

                      if (!hasDocument) {
                        return (
                          <Card className="border-dashed rounded-lg">
                            <CardContent className="p-6">
                              <div className="flex flex-col items-center gap-3 text-center">
                                <File className="w-12 h-12 text-muted-foreground/50" />
                                <div>
                                  <p className="text-sm font-medium">Nenhum documento selecionado</p>
                                  <p className="text-xs text-muted-foreground">
                                    Adicione um único documento para este comunicado
                                  </p>
                                </div>
                                <Button
                                  type="button"
                                  variant="outline"
                                  size="sm"
                                  onClick={handleAddAttachmentClick}
                                  className="gap-2 rounded-lg"
                                >
                                  <Paperclip className="w-3 h-3" />
                                  Selecionar Documento
                                </Button>
                              </div>
                            </CardContent>
                          </Card>
                        );
                      }
                      return (
                        <div className="space-y-2">
                          {isEdit && hasExisting && (
                            <Card className="rounded-lg">
                              <CardContent className="p-3">
                                <p className="text-xs font-medium text-muted-foreground mb-1">
                                  Documento atual (remover será aplicado ao salvar)
                                </p>
                                <div className="flex items-center gap-3 p-2 rounded-lg hover:bg-muted/50 transition-colors">
                                  <div className="h-12 w-12 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                                    <File className="w-6 h-6 text-primary" />
                                  </div>
                                  <div className="flex-1 min-w-0">
                                    <p className="text-sm font-medium truncate">
                                      {existingDocs[0].name}
                                    </p>
                                    <p className="text-xs text-muted-foreground">
                                      {existingDocs[0].size
                                        ? formatFileSize(existingDocs[0].size)
                                        : '—'}
                                    </p>
                                  </div>
                                  <Button
                                    type="button"
                                    variant="outline"
                                    size="sm"
                                    className="rounded-lg"
                                    onClick={handleSubstituteDocument}
                                  >
                                    Substituir
                                  </Button>
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8 text-destructive hover:text-destructive flex-shrink-0 rounded-lg"
                                    onClick={() => markAnexoForDeletion(existingDocs[0].id)}
                                    title="Remover anexo (será excluído ao salvar)"
                                  >
                                    <Trash2 className="w-3 h-3" />
                                  </Button>
                                </div>
                              </CardContent>
                            </Card>
                          )}
                          {isEdit && hasNew && (
                            <Card className="rounded-lg">
                              <CardContent className="p-3">
                                <p className="text-xs font-medium text-muted-foreground mb-1">
                                  Novo documento (será adicionado ao salvar)
                                </p>
                                <div className="flex items-center gap-3 p-2 rounded-lg hover:bg-muted/50 transition-colors">
                                  <div className="h-12 w-12 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                                    <File className="w-6 h-6 text-primary" />
                                  </div>
                                  <div className="flex-1 min-w-0">
                                    <p className="text-sm font-medium truncate">
                                      {attachmentFiles[0].name}
                                    </p>
                                    <p className="text-xs text-muted-foreground">
                                      {formatFileSize(attachmentFiles[0].size)}
                                    </p>
                                  </div>
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8 text-destructive hover:text-destructive flex-shrink-0 rounded-lg"
                                    onClick={() => handleRemoveNewFile(0)}
                                  >
                                    <Trash2 className="w-3 h-3" />
                                  </Button>
                                </div>
                              </CardContent>
                            </Card>
                          )}
                          {!isEdit && (
                            <Card className="rounded-lg">
                              <CardContent className="p-3">
                                <div className="flex items-center gap-3 p-2 rounded-lg hover:bg-muted/50 transition-colors">
                                  <div className="h-12 w-12 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                                    <File className="w-6 h-6 text-primary" />
                                  </div>
                                  <div className="flex-1 min-w-0">
                                    <p className="text-sm font-medium truncate">
                                      {attachments[0].name}
                                    </p>
                                    <p className="text-xs text-muted-foreground">
                                      {attachments[0].size
                                        ? formatFileSize(attachments[0].size)
                                        : '—'}
                                    </p>
                                  </div>
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8 text-destructive hover:text-destructive flex-shrink-0 rounded-lg"
                                    onClick={() => handleRemoveAttachment(0)}
                                  >
                                    <Trash2 className="w-3 h-3" />
                                  </Button>
                                </div>
                              </CardContent>
                            </Card>
                          )}
                        </div>
                      );
                    })()}
                  </div>

                  <Separator />

                  <div className="space-y-3">
                    <Label className="flex items-center gap-2">
                      <FolderOpen className="w-4 h-4" />
                      Pastas de Destino <span className="text-destructive">*</span>
                    </Label>
                    <p className="text-xs text-muted-foreground">
                      Selecione uma pasta no dropdown para adicionar à lista. Repita para incluir
                      várias pastas.
                    </p>

                    <div className="flex flex-wrap gap-2 items-center">
                      <Select
                        value={folderSelectValue || undefined}
                        onValueChange={(v) => {
                          if (v) handleAddFolder(v);
                        }}
                      >
                        <SelectTrigger id="folderPath" className="w-[280px] rounded-lg">
                          <SelectValue placeholder="Selecione uma pasta para adicionar" />
                        </SelectTrigger>
                        <SelectContent>
                          {folderOptions.filter((f) => !folderPaths.includes(f)).map((folder) => (
                            <SelectItem key={folder} value={folder}>
                              <div className="flex items-center gap-2">
                                <FolderOpen className="w-4 h-4" />
                                {folder}
                              </div>
                            </SelectItem>
                          ))}
                          {folderOptions.filter((f) => !folderPaths.includes(f)).length === 0 && (
                            <div className="px-2 py-1.5 text-sm text-muted-foreground">
                              Todas as pastas já adicionadas
                            </div>
                          )}
                        </SelectContent>
                      </Select>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() => setShowNewFolderInput(!showNewFolderInput)}
                        className="gap-2 rounded-lg"
                      >
                        <FolderPlus className="w-3 h-3" />
                        {showNewFolderInput ? 'Cancelar' : 'Criar Nova Pasta'}
                      </Button>
                    </div>

                    <div className="rounded-lg border border-dashed border-muted-foreground/25 bg-muted/30 p-3 min-h-[44px]">
                      <p className="text-xs text-muted-foreground mb-2">Pastas adicionadas:</p>
                      {folderPaths.length === 0 ? (
                        <p className="text-sm text-muted-foreground italic">
                          Nenhuma pasta adicionada ainda. Selecione acima para adicionar.
                        </p>
                      ) : (
                        <div className="flex flex-wrap gap-2">
                          {folderPaths.map((path) => (
                            <Badge
                              key={path}
                              variant="secondary"
                              className="gap-1 pr-1 pl-3 py-1.5 font-normal rounded-lg"
                            >
                              <FolderOpen className="w-3.5 h-3.5" />
                              {path}
                              <Button
                                type="button"
                                variant="ghost"
                                size="icon"
                                className="h-4 w-4 hover:bg-destructive/20 ml-1 rounded-lg"
                                onClick={() => handleRemoveFolder(path)}
                              >
                                <X className="w-3 h-3" />
                              </Button>
                            </Badge>
                          ))}
                        </div>
                      )}
                    </div>

                    {showNewFolderInput && (
                      <Card className="border-primary/30 bg-primary/5 rounded-lg">
                        <CardContent className="p-4 space-y-3">
                          <Label htmlFor="newFolderName">Nome da Nova Pasta</Label>
                          <div className="flex gap-2">
                            <Input
                              id="newFolderName"
                              placeholder="Ex: RH/Políticas Internas"
                              value={newFolderName}
                              onChange={(e) => setNewFolderName(e.target.value)}
                              className="rounded-lg"
                            />
                            <Button
                              type="button"
                              onClick={handleCreateFolder}
                              disabled={
                                !newFolderName.trim() ||
                                folderPaths.includes(newFolderName.trim())
                              }
                              className="rounded-lg"
                            >
                              Adicionar
                            </Button>
                          </div>
                          <p className="text-xs text-muted-foreground">
                            Use &quot;/&quot; para subpastas. Ex: &quot;Departamento/Subdepartamento&quot;
                          </p>
                        </CardContent>
                      </Card>
                    )}

                    <div className="flex items-center justify-between rounded-lg border p-3">
                      <div className="flex items-center gap-3">
                        <div className="p-2 rounded-lg bg-primary/10">
                          <File className="w-4 h-4 text-primary" />
                        </div>
                        <div>
                          <p className="text-sm font-medium">Permitir download</p>
                          <p className="text-xs text-muted-foreground">
                            Quando o usuário abrir o preview do documento, poderá baixar o arquivo
                          </p>
                        </div>
                      </div>
                      <Switch checked={allowDownload} onCheckedChange={setAllowDownload} />
                    </div>
                  </div>
                </>
              )}

              <Separator />

              <div className="space-y-3">
                <Label htmlFor="expiresDate" className="flex items-center gap-2">
                  <Calendar className="w-4 h-4" />
                  Data de Validade (opcional)
                </Label>
                <Input
                  id="expiresDate"
                  type="date"
                  value={expiresDate}
                  onChange={(e) => setExpiresDate(e.target.value)}
                  min={new Date().toISOString().split('T')[0]}
                  className="rounded-lg"
                />
                <p className="text-xs text-muted-foreground">
                  Após esta data, o comunicado ficará invisível para usuários comuns
                </p>
              </div>

              <Separator />

              <div
                className={cn(
                  'space-y-3 rounded-lg transition-colors',
                  destinatariosInativos && 'border border-border/60 bg-muted/20 p-3',
                )}
              >
                <Label
                  className={cn(
                    'flex items-center gap-2',
                    destinatariosInativos && 'text-muted-foreground',
                  )}
                >
                  <Users className="w-4 h-4" />
                  Destinatários <span className="text-destructive">*</span>
                </Label>
                {destinatariosInativos && (
                  <p className="text-xs text-muted-foreground -mt-1">
                    Com <strong>Oculta no feed</strong> ativo em Configurações, a seleção de destinatários
                    não pode ser alterada. Desative essa opção para editar quem recebe o comunicado.
                  </p>
                )}
                <RadioGroup
                  value={enviarPara}
                  onValueChange={(v) => setEnviarPara(v as 'todos' | 'grupos')}
                  disabled={destinatariosInativos}
                  className="grid gap-3"
                >
                  <div
                    className={cn(
                      'flex items-center gap-3 p-3 rounded-lg border-2 transition-colors',
                      destinatariosInativos
                        ? 'cursor-not-allowed opacity-70'
                        : 'cursor-pointer',
                      enviarPara === 'todos'
                        ? 'border-primary bg-primary/5'
                        : cn('border-border', !destinatariosInativos && 'hover:border-primary/30'),
                    )}
                    onClick={() => {
                      if (!destinatariosInativos) setEnviarPara('todos');
                    }}
                  >
                    <RadioGroupItem value="todos" id="enviar-todos" />
                    <div className="flex-1">
                      <p className="text-sm font-medium">Enviar para todos da empresa</p>
                      <p className="text-xs text-muted-foreground">
                        O comunicado será exibido para todos os colaboradores
                      </p>
                    </div>
                  </div>
                  <div
                    className={cn(
                      'flex items-center gap-3 p-3 rounded-lg border-2 transition-colors',
                      destinatariosInativos
                        ? 'cursor-not-allowed opacity-70'
                        : 'cursor-pointer',
                      enviarPara === 'grupos'
                        ? 'border-primary bg-primary/5'
                        : cn('border-border', !destinatariosInativos && 'hover:border-primary/30'),
                    )}
                    onClick={() => {
                      if (!destinatariosInativos) setEnviarPara('grupos');
                    }}
                  >
                    <RadioGroupItem value="grupos" id="enviar-grupos" />
                    <div className="flex-1">
                      <p className="text-sm font-medium">Enviar para o(s) grupo(s) de usuário(s)</p>
                      <p className="text-xs text-muted-foreground">
                        Selecione um ou mais grupos abaixo
                      </p>
                    </div>
                  </div>
                </RadioGroup>

                {enviarPara === 'grupos' && (
                  <div className="space-y-2 pt-1">
                    <UserGroupsSelector
                      grupos={grupos}
                      loading={loadingGrupos}
                      error={errorGrupos}
                      selectedIds={selectedUserGroups}
                      onToggle={toggleUserGroup}
                      title="Enviar para o(s) grupo(s) de usuário(s)"
                      description="Selecione um ou mais grupos abaixo"
                      showTotalMembers={true}
                      disabled={destinatariosInativos}
                    />
                  </div>
                )}
              </div>
            </TabsContent>

            <TabsContent value="settings" className="space-y-6 mt-0">
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
                            Ao desligar, o comunicado será <strong>arquivado</strong> e deixará de ser exibido na lista de comunicados ativos.
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

              <div className="space-y-4">
                <Label>Publicação</Label>

                <div className="flex items-center justify-between gap-4 p-3 rounded-lg border">
                  <div>
                    <p className="font-medium text-sm">Oculta no Feed</p>
                    <p className="text-xs text-muted-foreground">
                      Quando ativo, o comunicado não será exibido no feed (apenas na lista de comunicados).
                    </p>
                    {ocultarNoFeed && (
                      <p className="text-xs text-muted-foreground mt-2">
                        A seção <strong>Destinatários</strong> na aba Conteúdo fica inativa enquanto esta
                        opção estiver ligada.
                      </p>
                    )}
                  </div>
                  <Switch
                    checked={ocultarNoFeed}
                    onCheckedChange={setOcultarNoFeed}
                  />
                </div>

                <div
                  className="flex items-center gap-2 p-3 rounded-lg border cursor-pointer hover:bg-muted/50 transition-colors"
                  onClick={() => setStatus('published')}
                >
                  <input
                    type="radio"
                    checked={status === 'published'}
                    onChange={() => setStatus('published')}
                    className="cursor-pointer"
                  />
                  <div className="flex-1">
                    <p className="font-medium text-sm">Publicar agora</p>
                    <p className="text-xs text-muted-foreground">
                      O comunicado será enviado imediatamente
                    </p>
                  </div>
                </div>

                <div
                  className="flex items-center gap-2 p-3 rounded-lg border cursor-pointer hover:bg-muted/50 transition-colors"
                  onClick={() => setStatus('scheduled')}
                >
                  <input
                    type="radio"
                    checked={status === 'scheduled'}
                    onChange={() => setStatus('scheduled')}
                    className="cursor-pointer"
                  />
                  <div className="flex-1">
                    <p className="font-medium text-sm">Agendar publicação</p>
                    <p className="text-xs text-muted-foreground">
                      Escolha data e hora para envio automático
                    </p>
                  </div>
                </div>

                {status === 'scheduled' && (
                  <div className="grid grid-cols-2 gap-3 pl-8">
                    <div className="space-y-2">
                      <Label htmlFor="scheduledDate">Data</Label>
                      <Input
                        id="scheduledDate"
                        type="date"
                        value={scheduledDate}
                        onChange={(e) => setScheduledDate(e.target.value)}
                        className="rounded-lg"
                      />
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="scheduledTime">Hora</Label>
                      <Input
                        id="scheduledTime"
                        type="time"
                        value={scheduledTime}
                        onChange={(e) => setScheduledTime(e.target.value)}
                        className="rounded-lg"
                      />
                    </div>
                  </div>
                )}
              </div>

              <div className="space-y-4">
                <Label>Configurações de Interação</Label>

                <div className="flex items-center justify-between p-3 rounded-lg border">
                  <div className="flex items-center gap-3">
                    <CheckCircle2 className="w-4 h-4 text-primary" />
                    <div>
                      <p className="text-sm font-medium">Requer confirmação de leitura</p>
                      <p className="text-xs text-muted-foreground">
                        Usuários devem confirmar que leram
                      </p>
                    </div>
                  </div>
                  <Switch
                    checked={requiresAcknowledgment}
                    onCheckedChange={setRequiresAcknowledgment}
                  />
                </div>

                <div className="flex items-center justify-between p-3 rounded-lg border">
                  <div className="flex items-center gap-3">
                    <MessageSquare className="w-4 h-4 text-primary" />
                    <div>
                      <p className="text-sm font-medium">Permitir comentários</p>
                      <p className="text-xs text-muted-foreground">Usuários podem comentar</p>
                    </div>
                  </div>
                  <Switch checked={allowComments} onCheckedChange={setAllowComments} />
                </div>

                <div className="flex items-center justify-between p-3 rounded-lg border">
                  <div className="flex items-center gap-3">
                    <Heart className="w-4 h-4 text-primary" />
                    <div>
                      <p className="text-sm font-medium">Permitir curtidas</p>
                      <p className="text-xs text-muted-foreground">Usuários podem curtir</p>
                    </div>
                  </div>
                  <Switch checked={allowLikes} onCheckedChange={setAllowLikes} />
                </div>

                <div className="flex items-center justify-between p-3 rounded-lg border">
                  <div className="flex items-center gap-3">
                    <span className="text-xl">📌</span>
                    <div>
                      <p className="text-sm font-medium">Fixar comunicado</p>
                      <p className="text-xs text-muted-foreground">Aparece no topo da lista</p>
                    </div>
                  </div>
                  <Switch checked={isPinned} onCheckedChange={setIsPinned} />
                </div>
              </div>
            </TabsContent>
          </div>
        </Tabs>

        <div className="flex justify-between items-center pt-4 border-t shrink-0">
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={submitting}
            className="rounded-lg"
          >
            Cancelar
          </Button>
          <Button
            onClick={() => void handleSave()}
            disabled={!isValid || !canSchedule || submitting}
            className="rounded-lg gap-2"
          >
            {submitting ? (
              <Spinner className="w-4 h-4" />
            ) : (
              <Save className="w-4 h-4" />
            )}
            {isEdit
              ? (submitting ? 'Salvando...' : 'Salvar alterações')
              : status === 'scheduled'
                ? 'Agendar Comunicado'
                : 'Publicar Comunicado'}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
