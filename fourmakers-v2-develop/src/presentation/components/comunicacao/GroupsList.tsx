import { useState } from 'react';
import { container } from 'tsyringe';
import {
  Users,
  Lock,
  Globe,
  Plus,
  Search,
  MoreHorizontal,
  MessageSquare,
  Pencil,
} from 'lucide-react';
import type { CommunityGroup, CommunityPersona } from '@domain/entities/comunicacao';
import { ArquivarComunidadeUseCase } from '@domain/usecases/ArquivarComunidadeUseCase';
import { useAppSelector } from '@app/store/hooks';
import { useComunicacaoComunidades } from '@presentation/hooks/useComunicacaoComunidades';
import { CreateGroupModal } from './CreateGroupModal';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Spinner } from '@/components/ui/spinner';
import { toast } from 'sonner';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
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

interface GroupsListProps {
  persona: CommunityPersona;
  onSelectGroup?: (group: CommunityGroup) => void;
}

export function GroupsList({ persona, onSelectGroup }: GroupsListProps) {
  const token = useAppSelector((state) => state.auth.token);
  const { comunidades, loading, error, loadComunidades } =
    useComunicacaoComunidades();
  const [localGroups, setLocalGroups] = useState<CommunityGroup[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [groupToEdit, setGroupToEdit] = useState<CommunityGroup | null>(null);
  const [groupToArquivar, setGroupToArquivar] = useState<CommunityGroup | null>(null);
  const [archivingId, setArchivingId] = useState<string | null>(null);
  const arquivarComunidadeUseCase = container.resolve(ArquivarComunidadeUseCase);

  const handleConfirmArquivar = async () => {
    if (!groupToArquivar) return;
    await handleArquivar(groupToArquivar);
    setGroupToArquivar(null);
  };

  const handleArquivar = async (group: CommunityGroup) => {
    if (!token) {
      toast.error('Sessão expirada. Faça login novamente.');
      return;
    }
    setArchivingId(group.id);
    try {
      const res = await arquivarComunidadeUseCase.execute(token, group.id);
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Comunidade arquivada.');
        await loadComunidades();
        setLocalGroups((prev) => prev.filter((g) => g.id !== group.id));
      } else {
        toast.error(res.mensagem ?? 'Não foi possível arquivar a comunidade.');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao arquivar comunidade.');
    } finally {
      setArchivingId(null);
    }
  };

  const currentUserCodigo = useAppSelector(
    (state) =>
      state.auth.user?.colaborador?.codigoColaboradorInterno ??
      state.auth.codColaborador ??
      '',
  );
  const groups = [
    ...comunidades,
    ...localGroups.filter((l) => !comunidades.some((c) => c.id === l.id)),
  ];
  const canManage = persona === 'manager' || persona === 'analytics';
  const canCreateGroup = persona === 'manager';
  /** Editar e Arquivar só aparecem se o usuário logado for moderador da comunidade. */
  const isModeratorOf = (group: CommunityGroup) =>
    Boolean(
      currentUserCodigo &&
        group.moderators?.some((m) => m.userId === currentUserCodigo),
    );

  const handleSaveGroup = (group: CommunityGroup) => {
    loadComunidades();
    setLocalGroups((prev) => [group, ...prev]);
    toast.success('Comunidade criada com sucesso!', {
      description: `A comunidade "${group.name}" foi criada e está ativa.`,
    });
  };

  const filteredGroups = groups.filter(
    (group) =>
      group.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      group.description.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  return (
    <div className="space-y-4">
      {error && (
        <div className="rounded-lg border border-destructive/50 bg-destructive/10 px-4 py-3 text-sm text-destructive">
          {error}
        </div>
      )}
      <div className="flex items-center justify-between gap-4">
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input
            placeholder="Buscar comunidades..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        {canCreateGroup && (
          <Button
            className="gap-2"
            onClick={() => {
              setGroupToEdit(null);
              setIsCreateModalOpen(true);
            }}
          >
            <Plus className="w-4 h-4" />
            Criar Comunidade
          </Button>
        )}
      </div>

      {loading ? (
        <div className="flex justify-center py-12">
          <Spinner className="h-8 w-8 text-muted-foreground" />
        </div>
      ) : (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {filteredGroups.map((group, index) => (
          <div
            key={group.id}
            className="transition-opacity"
            style={{ animationDelay: `${index * 50}ms` }}
          >
            <Card
              className="overflow-hidden hover:shadow-lg transition-all cursor-pointer group"
              onClick={() => onSelectGroup?.(group)}
            >
              <div className="relative h-32 overflow-hidden bg-muted">
                {group.coverImage ? (
                  <img
                    src={group.coverImage}
                    alt={group.name}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                  />
                ) : (
                  <div className="w-full h-full flex items-center justify-center">
                    <Users className="w-12 h-12 text-muted-foreground/50" />
                  </div>
                )}
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent" />
                <div className="absolute bottom-3 left-3 right-3">
                  <div className="flex items-center gap-2">
                    {group.type === 'private' ? (
                      <Lock className="w-4 h-4 text-white" />
                    ) : (
                      <Globe className="w-4 h-4 text-white" />
                    )}
                    <h3 className="font-semibold text-white truncate">
                      {group.name}
                    </h3>
                  </div>
                </div>
                {canManage && isModeratorOf(group) && (
                  <div className="absolute top-2 right-2">
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 bg-black/30 hover:bg-black/50 text-white"
                          onClick={(e) => e.stopPropagation()}
                        >
                          <MoreHorizontal className="w-4 h-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem
                          onClick={(e) => {
                            e.preventDefault();
                            e.stopPropagation();
                            setGroupToEdit(group);
                            setIsCreateModalOpen(true);
                          }}
                        >
                          <Pencil className="w-4 h-4 mr-2" />
                          Editar
                        </DropdownMenuItem>
                        <DropdownMenuItem
                          className="text-destructive"
                          disabled={archivingId !== null}
                          onClick={(e) => {
                            e.preventDefault();
                            e.stopPropagation();
                            setGroupToArquivar(group);
                          }}
                        >
                          Arquivar comunidade
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                )}
              </div>

              <CardContent className="p-4">
                <p className="text-sm text-muted-foreground line-clamp-2 mb-3">
                  {group.description}
                </p>

                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3 text-sm text-muted-foreground">
                    <div className="flex items-center gap-1">
                      <Users className="w-4 h-4" />
                      <span>{group.memberCount}</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <MessageSquare className="w-4 h-4" />
                      <span>{group.postCount}</span>
                    </div>
                  </div>

                  <Badge
                    variant="outline"
                    className={
                      group.type === 'private'
                        ? 'bg-primary/10 text-primary border-primary/30'
                        : 'bg-success/10 text-success border-success/30'
                    }
                  >
                    {group.type === 'private' ? 'Privado' : 'Aberto'}
                  </Badge>
                </div>

                <div className="flex items-center gap-2 mt-3 pt-3 border-t">
                  <Badge variant="secondary" className="text-xs">
                    {group.settings.allowMemberPosts
                      ? 'Membros podem postar'
                      : 'Apenas moderadores'}
                  </Badge>
                </div>
              </CardContent>
            </Card>
          </div>
        ))}
      </div>
      )}

      {!loading && filteredGroups.length === 0 && (
        <div className="text-center py-12">
          <Users className="w-12 h-12 mx-auto text-muted-foreground/50 mb-3" />
          <p className="text-muted-foreground">
            Não há comunidades.
            {canCreateGroup && ' Crie a primeira para começar.'}
          </p>
        </div>
      )}

      <CreateGroupModal
        open={isCreateModalOpen}
        onOpenChange={(open) => {
          setIsCreateModalOpen(open);
          if (!open) setGroupToEdit(null);
        }}
        onSave={handleSaveGroup}
        onCreated={loadComunidades}
        initialGroup={groupToEdit}
      />

      <AlertDialog
        open={!!groupToArquivar}
        onOpenChange={(open) => {
          if (!open) setGroupToArquivar(null);
        }}
      >
        <AlertDialogContent onClick={(e) => e.stopPropagation()}>
          <AlertDialogHeader>
            <AlertDialogTitle>Arquivar comunidade</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja arquivar a comunidade &quot;{groupToArquivar?.name}&quot;?
              Ela não aparecerá mais na lista de comunidades ativas.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={!!archivingId}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={(e) => {
                e.preventDefault();
                void handleConfirmArquivar();
              }}
              disabled={!!archivingId}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {archivingId ? (
                <>
                  <Spinner className="w-4 h-4 mr-2" />
                  Arquivando...
                </>
              ) : (
                'Arquivar'
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
