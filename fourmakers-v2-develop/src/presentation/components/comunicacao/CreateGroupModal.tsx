import { useState, useEffect, useCallback } from 'react';
import { container } from 'tsyringe';
import {
  X,
  Save,
  Users,
  Lock,
  Globe,
  Settings,
  AlertCircle,
  MessageSquare,
  Heart,
  Shield,
} from 'lucide-react';
import type {
  CommunityGroup,
  GroupType,
  GroupSettings,
} from '@domain/entities/comunicacao';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoComunidadeRepository } from '@domain/repositories/ComunicacaoComunidadeRepository';
import { comunidadeDetalheToCommunityGroup } from '@shared/utils/comunicacaoComunidadeMapper';
import { AtualizarComunidadeUseCase } from '@domain/usecases/AtualizarComunidadeUseCase';
import { CriarComunicacaoComunidadeUseCase } from '@domain/usecases/CriarComunicacaoComunidadeUseCase';
import { useComunicacaoGrupos } from '@presentation/hooks/useComunicacaoGrupos';
import { useAppSelector } from '@app/store/hooks';
import { toast } from 'sonner';
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
import { Textarea } from '@/components/ui/textarea';
import { Switch } from '@/components/ui/switch';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { UserGroupsSelector } from './UserGroupsSelector';
import { GroupMembersSelector, type GroupMemberItem } from './GroupMembersSelector';

interface CreateGroupModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSave?: (group: CommunityGroup) => void;
  /** Chamado após criar/editar comunidade via API (ex.: refetch da lista). */
  onCreated?: () => void;
  /** Quando definido, o modal abre em modo edição com o formulário populado. */
  initialGroup?: CommunityGroup | null;
}

export function CreateGroupModal({
  open,
  onOpenChange,
  onSave,
  onCreated,
  initialGroup,
}: CreateGroupModalProps) {
  const token = useAppSelector((state) => state.auth.token);
  const codigoColaboradorInterno = useAppSelector(
    (state) => state.auth.user?.colaborador?.codigoColaboradorInterno ?? '',
  );
  const criarComunidadeUseCase = container.resolve(CriarComunicacaoComunidadeUseCase);
  const atualizarComunidadeUseCase = container.resolve(AtualizarComunidadeUseCase);
  const repository = container.resolve<ComunicacaoComunidadeRepository>(DiTokens.comunicacaoComunidadeRepository);
  const { grupos, loading: loadingGrupos, error: errorGrupos, loadGrupos } = useComunicacaoGrupos();

  const [loadingDetail, setLoadingDetail] = useState(false);

  useEffect(() => {
    if (open) void loadGrupos();
  }, [open, loadGrupos]);

  // Ao abrir em modo edição, busca o detalhe da comunidade e preenche o formulário.
  useEffect(() => {
    if (!open || !initialGroup?.id || !token) return;
    let cancelled = false;
    setLoadingDetail(true);
    (async () => {
      try {
        const detalhe = await repository.obterComunidadePorId(token, initialGroup.id);
        if (cancelled) return;
        const fullGroup = comunidadeDetalheToCommunityGroup(detalhe, token);
        setName(fullGroup.name);
        setDescription(fullGroup.description);
        setCoverImage(fullGroup.coverImage ?? '');
        setCoverFile(null);
        setGroupType(fullGroup.type);
        setAllowMemberPosts(fullGroup.settings.allowMemberPosts);
        setAllowMemberLeave(fullGroup.settings.allowMemberLeave);
        setCommentsEnabledByDefault(fullGroup.settings.commentsEnabledByDefault);
        setLikesEnabledByDefault(fullGroup.settings.likesEnabledByDefault);
        setSelectedUserGroups(fullGroup.linkedUserGroups ?? []);
        setModerators(fullGroup.type === 'free' ? fullGroup.moderators.map((m) => ({ userId: m.userId, userName: m.userName, userEmail: '' })) : []);
        setAdministrators(fullGroup.type === 'private' ? fullGroup.moderators.map((m) => ({ userId: m.userId, userName: m.userName, userEmail: '' })) : []);
        const membersFromDetail = detalhe.membros?.map((m) => ({ userId: m.codigoInternoColaborador, userName: m.nomeCompleto ?? '', userEmail: '' })) ?? [];
        setIndividualMembers(membersFromDetail);
      } catch (e) {
        if (!cancelled) {
          toast.error(e instanceof Error ? e.message : 'Erro ao carregar dados da comunidade.');
          onOpenChange(false);
        }
      } finally {
        if (!cancelled) setLoadingDetail(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [open, initialGroup?.id, token, repository, onOpenChange]);

  // Ao abrir em modo criação, reseta o formulário.
  useEffect(() => {
    if (open && !initialGroup) {
      setName('');
      setDescription('');
      setCoverImage('');
      setCoverFile(null);
      setGroupType('free');
      setAllowMemberPosts(true);
      setAllowMemberLeave(true);
      setCommentsEnabledByDefault(true);
      setLikesEnabledByDefault(true);
      setSelectedUserGroups([]);
      setIndividualMembers([]);
      setModerators([]);
      setAdministrators([]);
      setActiveTab('info');
    }
  }, [open, initialGroup]);

  const [activeTab, setActiveTab] = useState<'info' | 'settings'>('info');
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [coverImage, setCoverImage] = useState('');
  const [coverFile, setCoverFile] = useState<File | null>(null);
  const [groupType, setGroupType] = useState<GroupType>('free');
  const [allowMemberPosts, setAllowMemberPosts] = useState(true);
  const [allowMemberLeave, setAllowMemberLeave] = useState(true);
  const [commentsEnabledByDefault, setCommentsEnabledByDefault] = useState(true);
  const [likesEnabledByDefault, setLikesEnabledByDefault] = useState(true);
  const [selectedUserGroups, setSelectedUserGroups] = useState<string[]>([]);
  const [individualMembers, setIndividualMembers] = useState<GroupMemberItem[]>([]);
  const [moderators, setModerators] = useState<GroupMemberItem[]>([]);
  const [administrators, setAdministrators] = useState<GroupMemberItem[]>([]);
  const [submitting, setSubmitting] = useState(false);

  const totalMembersFromGroups = selectedUserGroups.reduce(
    (sum, groupId) => sum + (grupos.find((g) => g.id === groupId)?.quantidadeParticipantes ?? 0),
    0,
  );

  const buildFormData = useCallback((): FormData => {
    const form = new FormData();
    // Ordem igual ao curl que funciona: capa → campos texto/config → moderadores → grupos → participantes → ativo
    if (coverFile) {
      form.append('capaComunidade', coverFile, coverFile.name);
    }
    form.append('nome', name.trim());
    form.append('descricao', description.trim());
    form.append('tipo', groupType === 'free' ? 'publica' : 'privada');
    form.append('permitePostagemMembro', allowMemberPosts ? 'true' : 'false');
    form.append('permiteSair', (groupType === 'free' || allowMemberLeave) ? 'true' : 'false');
    form.append('publicacaoConfiguracaoPolitica', 'sugerir');
    form.append('publicacaoPermiteComentario', commentsEnabledByDefault ? 'true' : 'false');
    form.append('publicacaoPermiteLikeHabilitado', likesEnabledByDefault ? 'true' : 'false');
    if (groupType === 'free') {
      moderators.forEach((m) => form.append('codigosInternoColaboradoresModeradores', m.userId));
    } else if (groupType === 'private') {
      administrators.forEach((m) => form.append('codigosInternoColaboradoresModeradores', m.userId));
    }
    if (groupType === 'private') {
      selectedUserGroups.forEach((id) => form.append('gruposComunidade', id));
      individualMembers.forEach((m) => form.append('codigoInternoColaboradoresParticipantes', m.userId));
    }
    form.append('ativo', 'true');
    return form;
  }, [
    name,
    description,
    groupType,
    allowMemberPosts,
    allowMemberLeave,
    commentsEnabledByDefault,
    likesEnabledByDefault,
    codigoColaboradorInterno,
    coverFile,
    selectedUserGroups,
    individualMembers,
    moderators,
    administrators,
  ]);

  const handleSave = async () => {
    if (!token) {
      toast.error('Sessão expirada. Faça login novamente.');
      return;
    }
    const settings: GroupSettings = {
      allowFreeEntry: groupType === 'free',
      allowMemberPosts,
      allowMemberLeave,
      requiresApproval: false,
      commentsEnabledByDefault,
      likesEnabledByDefault,
    };
    const totalMembers = totalMembersFromGroups + individualMembers.length;
    const isEdit = !!initialGroup;

    setSubmitting(true);
    try {
      const formData = buildFormData();
      const res = isEdit
        ? await atualizarComunidadeUseCase.execute(token, initialGroup!.id, formData)
        : await criarComunidadeUseCase.execute(token, formData);

      if (res.sucesso) {
        const communityId = initialGroup?.id ?? res.retorno?.id ?? `g${Date.now()}`;
        const group: CommunityGroup = {
          id: communityId,
          name: name.trim(),
          description: description.trim(),
          coverImage: coverImage || undefined,
          type: groupType,
          creatorId: initialGroup?.creatorId ?? codigoColaboradorInterno ?? 'current-user',
          creatorName: initialGroup?.creatorName ?? 'Usuário Atual',
          moderators: groupType === 'free' ? moderators.map((m) => ({ id: m.userId, userId: m.userId, userName: m.userName, role: 'moderator' as const, joinedAt: '' })) : [],
          status: 'active',
          createdAt: initialGroup?.createdAt ?? new Date().toISOString().split('T')[0],
          memberCount: totalMembers,
          postCount: initialGroup?.postCount ?? 0,
          settings,
          linkedUserGroups: selectedUserGroups,
          individualMembers:
            individualMembers.length > 0 ? individualMembers.map((m) => m.userId) : undefined,
        };
        if (isEdit) {
          toast.success(res.mensagem ?? 'Comunidade atualizada com sucesso!', {
            description: `As alterações em "${name.trim()}" foram salvas.`,
          });
        } else {
          toast.success(res.mensagem ?? 'Comunidade criada com sucesso!', {
            description: `A comunidade "${name.trim()}" foi criada.`,
          });
        }
        onCreated?.();
        onSave?.(group);
        setName('');
        setDescription('');
        setCoverImage('');
        setCoverFile(null);
        setGroupType('free');
        setSelectedUserGroups([]);
        setIndividualMembers([]);
        setModerators([]);
        setAdministrators([]);
        setActiveTab('info');
        onOpenChange(false);
      } else {
        toast.error(res.mensagem ?? (isEdit ? 'Não foi possível atualizar a comunidade.' : 'Não foi possível criar a comunidade.'));
      }
    } catch (e) {
      toast.error(
        e instanceof Error ? e.message : (isEdit ? 'Erro ao atualizar comunidade. Tente novamente.' : 'Erro ao criar comunidade. Tente novamente.'),
      );
    } finally {
      setSubmitting(false);
    }
  };

  const toggleUserGroup = (groupId: string) => {
    setSelectedUserGroups((prev) =>
      prev.includes(groupId) ? prev.filter((id) => id !== groupId) : [...prev, groupId],
    );
  };

  const isValid =
    !loadingDetail &&
    name.trim() &&
    description.trim() &&
    (groupType === 'free' ||
      selectedUserGroups.length > 0 ||
      individualMembers.length > 0);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-3xl max-h-[90vh] overflow-hidden flex flex-col">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Users className="w-5 h-5" />
            {initialGroup ? 'Editar Comunidade' : 'Criar Nova Comunidade'}
          </DialogTitle>
          <DialogDescription>
            {initialGroup
              ? 'Altere as informações e configurações da comunidade.'
              : 'Crie comunidades para organizar comunicações e documentos'}
          </DialogDescription>
        </DialogHeader>

        <Tabs
          value={activeTab}
          onValueChange={(v) => setActiveTab(v as 'info' | 'settings')}
          className="flex-1 flex flex-col overflow-hidden"
        >
          <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-pillToken gap-2 w-full shadow-sm">
            <TabsTrigger
              value="info"
              className="relative flex-1 flex items-center justify-center gap-2 text-sm font-medium py-2.5 px-5 rounded-pillToken transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              Informações Básicas
            </TabsTrigger>
            <TabsTrigger
              value="settings"
              className="relative flex-1 flex items-center justify-center gap-2 text-sm font-medium py-2.5 px-5 rounded-pillToken transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              Configurações
            </TabsTrigger>
          </TabsList>

          <div className="flex-1 overflow-y-auto py-4">
            {loadingDetail && (
              <div className="flex items-center justify-center py-8 text-muted-foreground text-sm">
                Carregando dados da comunidade...
              </div>
            )}
            {!loadingDetail && (
            <>
            <TabsContent value="info" className="space-y-4 mt-0">
              <div className="space-y-2">
                <Label htmlFor="name">
                  Nome da Comunidade <span className="text-destructive">*</span>
                </Label>
                <Input
                  id="name"
                  placeholder="Ex: Comunicados Oficiais"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="description">
                  Descrição <span className="text-destructive">*</span>
                </Label>
                <Textarea
                  id="description"
                  placeholder="Descreva o propósito desta comunidade..."
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  rows={4}
                />
              </div>
              <Separator />
              <div className="space-y-3">
                <Label>Tipo da Comunidade</Label>
                <div className="grid grid-cols-2 gap-3">
                  <button
                    type="button"
                    onClick={() => setGroupType('free')}
                    className={`p-4 rounded-lg border-2 transition-all text-left ${
                      groupType === 'free'
                        ? 'border-success bg-success/10'
                        : 'border-border hover:border-success/50'
                    }`}
                  >
                    <div className="flex items-start gap-3">
                      <Globe className={`w-5 h-5 ${groupType === 'free' ? 'text-success' : 'text-muted-foreground'}`} />
                      <div>
                        <p className="font-semibold">Livre (Aberta)</p>
                        <p className="text-xs text-muted-foreground mt-1">Qualquer usuário pode entrar</p>
                      </div>
                    </div>
                  </button>
                  <button
                    type="button"
                    onClick={() => setGroupType('private')}
                    className={`p-4 rounded-lg border-2 transition-all text-left ${
                      groupType === 'private'
                        ? 'border-primary bg-primary/10'
                        : 'border-border hover:border-primary/50'
                    }`}
                  >
                    <div className="flex items-start gap-3">
                      <Lock className={`w-5 h-5 ${groupType === 'private' ? 'text-primary' : 'text-muted-foreground'}`} />
                      <div>
                        <p className="font-semibold">Privada</p>
                        <p className="text-xs text-muted-foreground mt-1">Entrada sob aprovação</p>
                      </div>
                    </div>
                  </button>
                </div>
              </div>
              {groupType === 'free' && (
                <>
                  <Separator />
                  <div className="space-y-3">
                    <p className="text-xs text-muted-foreground">
                      Opcional. Defina quem pode moderar a comunidade (criar posts, aprovar conteúdo, etc.).
                    </p>
                    <GroupMembersSelector
                      members={moderators}
                      onMembersChange={setModerators}
                      label="Moderadores"
                      emptyMessage="Nenhum moderador adicionado. Pesquise e adicione acima."
                      open={open}
                    />
                  </div>
                </>
              )}
              {groupType === 'private' && (
                <>
                  <Separator />
                  <div className="space-y-3">
                    {selectedUserGroups.length === 0 && individualMembers.length === 0 && (
                      <Card className="border-warning/50 bg-warning/10">
                        <CardContent className="p-3 flex items-start gap-2">
                          <AlertCircle className="w-4 h-4 text-warning flex-shrink-0 mt-0.5" />
                          <p className="text-xs text-warning">
                            Comunidades privadas precisam ter pelo menos um grupo de usuários ou usuários individuais vinculados
                          </p>
                        </CardContent>
                      </Card>
                    )}
                    <UserGroupsSelector
                      grupos={grupos}
                      loading={loadingGrupos}
                      error={errorGrupos}
                      selectedIds={selectedUserGroups}
                      onToggle={toggleUserGroup}
                      title="Grupos de Usuários"
                      description="Selecione grupos de usuários para a comunidade privada"
                      showTotalMembers={true}
                    />
                  </div>

                  <Separator />

                  <div className="space-y-3">
                    <p className="text-xs text-muted-foreground">
                      Adicione usuários específicos que não fazem parte dos grupos selecionados
                    </p>
                    <GroupMembersSelector
                      members={individualMembers}
                      onMembersChange={setIndividualMembers}
                      label="Usuários Individuais"
                      emptyMessage="Nenhum usuário adicionado. Pesquise e adicione acima."
                      open={open}
                    />
                    {(selectedUserGroups.length > 0 || individualMembers.length > 0) && (
                      <Card className="bg-primary/5 border-primary/20">
                        <CardContent className="p-3">
                          <div className="flex items-center justify-between text-sm">
                            <span className="text-muted-foreground">Total de membros na comunidade:</span>
                            <Badge variant="outline" className="ml-2">
                              {totalMembersFromGroups + individualMembers.length}
                            </Badge>
                          </div>
                          {selectedUserGroups.length > 0 && individualMembers.length > 0 && (
                            <div className="text-xs text-muted-foreground mt-2 space-y-1">
                              <div>
                                •{' '}
                                {totalMembersFromGroups}{' '}
                                de grupos de usuários
                              </div>
                              <div>• {individualMembers.length} usuários individuais</div>
                            </div>
                          )}
                        </CardContent>
                      </Card>
                    )}
                  </div>

                  <Separator />

                  <div className="space-y-3">
                    <p className="text-xs text-muted-foreground">
                      Defina quem pode administrar a comunidade (criar posts, aprovar conteúdo, etc.).
                    </p>
                    <GroupMembersSelector
                      members={administrators}
                      onMembersChange={setAdministrators}
                      label="Administradores"
                      emptyMessage="Nenhum administrador adicionado. Pesquise e adicione acima."
                      open={open}
                    />
                  </div>
                </>
              )}
              <Separator />
              <div className="space-y-2">
                <Label htmlFor="coverImage">Imagem de Capa</Label>
                <p className="text-xs text-muted-foreground mb-1">
                  Opcional. Envie um arquivo de imagem para a capa da comunidade.
                </p>
                <Input
                  id="coverImage"
                  type="file"
                  accept="image/*"
                  onChange={(e) => {
                    const file = e.target.files?.[0];
                    setCoverFile(file ?? null);
                    if (!file) setCoverImage('');
                  }}
                  className="cursor-pointer"
                />
                {coverFile && (
                  <p className="text-xs text-muted-foreground">
                    Arquivo selecionado: {coverFile.name}
                  </p>
                )}
              </div>
            </TabsContent>
            <TabsContent value="settings" className="space-y-6 mt-0">
              <div className="flex items-center gap-2">
                <Settings className="w-5 h-5 text-primary" />
                <Label>Permissões</Label>
              </div>
              <Card className="border-2">
                <CardContent className="p-4 space-y-4">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <MessageSquare className="w-5 h-5 text-primary" />
                      <div>
                        <p className="font-medium text-sm">Permitir membros postarem</p>
                        <p className="text-xs text-muted-foreground">Membros podem criar posts</p>
                      </div>
                    </div>
                    <Switch checked={allowMemberPosts} onCheckedChange={setAllowMemberPosts} />
                  </div>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <Shield className="w-5 h-5 text-primary" />
                      <div>
                        <p className="font-medium text-sm">Permitir sair da comunidade</p>
                        <p className="text-xs text-muted-foreground">
                          Membros podem sair quando quiserem
                        </p>
                      </div>
                    </div>
                    <Switch
                      checked={allowMemberLeave}
                      onCheckedChange={setAllowMemberLeave}
                    />
                  </div>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <MessageSquare className="w-4 h-4 text-muted-foreground" />
                      <div>
                        <p className="font-medium text-sm">Comentários habilitados por padrão</p>
                      </div>
                    </div>
                    <Switch checked={commentsEnabledByDefault} onCheckedChange={setCommentsEnabledByDefault} />
                  </div>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <Heart className="w-4 h-4 text-muted-foreground" />
                      <div>
                        <p className="font-medium text-sm">Likes habilitados por padrão</p>
                      </div>
                    </div>
                    <Switch checked={likesEnabledByDefault} onCheckedChange={setLikesEnabledByDefault} />
                  </div>
                </CardContent>
              </Card>
            </TabsContent>
            </>
            )}
          </div>
        </Tabs>

        <div className="pt-4 border-t flex justify-end gap-2">
          <Button variant="ghost" onClick={() => onOpenChange(false)} disabled={loadingDetail}>
            <X className="w-4 h-4 mr-2" />
            Cancelar
          </Button>
          <Button
            onClick={() => void handleSave()}
            disabled={!isValid || submitting || loadingDetail}
          >
            <Save className="w-4 h-4 mr-2" />
            {submitting
              ? (initialGroup ? 'Salvando...' : 'Criando...')
              : (initialGroup ? 'Salvar alterações' : 'Criar Comunidade')}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
