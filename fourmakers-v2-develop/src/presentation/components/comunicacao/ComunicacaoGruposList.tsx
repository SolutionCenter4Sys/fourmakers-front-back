import { useState, useCallback } from 'react';
import { container } from 'tsyringe';
import { Users, Search, Plus, Pencil, Trash2 } from 'lucide-react';
import type { ComunicacaoGrupo, UserGroup } from '@domain/entities/comunicacao';
import { ObterComunicacaoGrupoPorIdUseCase } from '@domain/usecases/ObterComunicacaoGrupoPorIdUseCase';
import { DeletarComunicacaoGrupoUseCase } from '@domain/usecases/DeletarComunicacaoGrupoUseCase';
import { useComunicacaoGrupos } from '@presentation/hooks/useComunicacaoGrupos';
import { useAppSelector } from '@app/store/hooks';
import { CreateUserGroupModal } from './CreateUserGroupModal';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Spinner } from '@/components/ui/spinner';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { MoreHorizontal } from 'lucide-react';
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
import { toast } from 'sonner';

interface ComunicacaoGruposListProps {
  userGroups?: UserGroup[];
  setUserGroups?: React.Dispatch<React.SetStateAction<UserGroup[]>>;
}

function grupoDetalheToUserGroup(d: {
  id: string;
  nome: string;
  descricao: string;
  permiteCriarPublicacaoInformativo: boolean;
  publicacaoInformativoRequerAprovacao: boolean;
  aprovaPublicacaoInformativo: boolean;
  permiteCriarComunidade?: boolean;
  colaboradoresParticipantes: Array<{
    codigoColaboradorInterno: string;
    email: string;
    nomeCompleto: string;
  }>;
}): UserGroup {
  const now = new Date().toISOString().split('T')[0];
  return {
    id: d.id,
    name: d.nome,
    description: d.descricao,
    status: 'active',
    requiresApproval: d.publicacaoInformativoRequerAprovacao,
    permiteCriarPublicacaoInformativo: d.permiteCriarPublicacaoInformativo,
    aprovaPublicacaoInformativo: d.aprovaPublicacaoInformativo,
    permiteCriarComunidade: d.permiteCriarComunidade,
    members: d.colaboradoresParticipantes.map((c) => ({
      id: `ugm-${c.codigoColaboradorInterno}-${Date.now()}`,
      userId: c.codigoColaboradorInterno,
      userName: c.nomeCompleto,
      userEmail: c.email,
      addedAt: now,
    })),
    createdAt: now,
    updatedAt: now,
    createdBy: 'api',
  };
}

export function ComunicacaoGruposList({
  userGroups: _userGroups = [],
  setUserGroups,
}: ComunicacaoGruposListProps) {
  const token = useAppSelector((state) => state.auth.token);
  const obterGrupoUseCase = container.resolve(ObterComunicacaoGrupoPorIdUseCase);
  const deletarGrupoUseCase = container.resolve(DeletarComunicacaoGrupoUseCase);
  const { grupos, loading, error, loadGrupos } = useComunicacaoGrupos();
  const [searchTerm, setSearchTerm] = useState('');
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState<UserGroup | null>(null);
  const [loadingEdit, setLoadingEdit] = useState(false);
  const [grupoToDelete, setGrupoToDelete] = useState<ComunicacaoGrupo | null>(null);
  const [deleting, setDeleting] = useState(false);

  const handleOpenCreate = () => {
    setEditingGroup(null);
    setIsCreateModalOpen(true);
  };

  const handleOpenEdit = useCallback(
    async (grupoId: string) => {
      if (!token) return;
      setLoadingEdit(true);
      try {
        const detalhe = await obterGrupoUseCase.execute(token, grupoId);
        if (detalhe) {
          setEditingGroup(grupoDetalheToUserGroup(detalhe));
          setIsCreateModalOpen(true);
        } else {
          toast.error('Não foi possível carregar o grupo.');
        }
      } catch {
        toast.error('Erro ao carregar grupo. Tente novamente.');
      } finally {
        setLoadingEdit(false);
      }
    },
    [token, obterGrupoUseCase],
  );

  const handleCloseModal = (open: boolean) => {
    if (!open) setEditingGroup(null);
    setIsCreateModalOpen(open);
  };

  const handleSaveGroup = (group: UserGroup) => {
    setUserGroups?.((prev) => [group, ...prev]);
    setIsCreateModalOpen(false);
    setEditingGroup(null);
  };

  const handleConfirmDelete = useCallback(async () => {
    if (!token || !grupoToDelete) return;
    setDeleting(true);
    try {
      const res = await deletarGrupoUseCase.execute(token, grupoToDelete.id);
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Grupo excluído com sucesso.');
        setGrupoToDelete(null);
        loadGrupos();
      } else {
        toast.error(res.mensagem ?? 'Não foi possível excluir o grupo.');
      }
    } catch {
      toast.error('Erro ao excluir grupo. Tente novamente.');
    } finally {
      setDeleting(false);
    }
  }, [token, grupoToDelete, deletarGrupoUseCase, loadGrupos]);

  const filteredGrupos = grupos.filter(
    (g) =>
      g.nome.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (g.descricao ?? '').toLowerCase().includes(searchTerm.toLowerCase()),
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
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input
            placeholder="Buscar grupos..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <Button className="gap-2" onClick={handleOpenCreate}>
          <Plus className="w-4 h-4" />
          Novo grupo
        </Button>
      </div>

      {loading ? (
        <div className="flex justify-center py-12">
          <Spinner className="h-8 w-8 text-muted-foreground" />
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filteredGrupos.map((grupo) => (
            <Card
              key={grupo.id}
              className="overflow-hidden rounded-xl shadow-sm border border-border hover:shadow-md transition-shadow"
            >
              <CardContent className="p-4 flex flex-col gap-4">
                <div className="flex items-start justify-between gap-2">
                  <div className="min-w-0 flex-1">
                    <h3 className="font-semibold text-foreground truncate">
                      {grupo.nome}
                    </h3>
                    <p className="text-sm text-muted-foreground mt-0.5 line-clamp-2">
                      {grupo.descricao || '—'}
                    </p>
                  </div>
                  <Badge
                    variant={
                      grupo.status === 'Ativo' ? 'default' : 'secondary'
                    }
                    className="shrink-0 rounded-full"
                  >
                    {grupo.status}
                  </Badge>
                </div>

                <div className="space-y-2">
                  <div className="flex items-center gap-2 text-sm text-foreground">
                    <Users className="w-4 h-4 text-muted-foreground" />
                    <span>
                      {grupo.quantidadeParticipantes} membro
                      {grupo.quantidadeParticipantes !== 1 ? 's' : ''}
                    </span>
                  </div>
                  {grupo.iniciaisMembros &&
                    grupo.iniciaisMembros.length > 0 && (
                      <div className="flex -space-x-2">
                        {grupo.iniciaisMembros.slice(0, 3).map((inicial, i) => (
                          <Avatar
                            key={`${grupo.id}-${i}`}
                            className="h-8 w-8 border-2 border-background"
                          >
                            <AvatarFallback className="bg-primary text-primary-foreground text-xs">
                              {inicial}
                            </AvatarFallback>
                          </Avatar>
                        ))}
                      </div>
                    )}
                </div>
                <div className="flex justify-end pt-2 border-t border-border/50">
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button
                        variant="ghost"
                        size="sm"
                        className="h-8 w-8 p-0"
                        disabled={loadingEdit}
                      >
                        <MoreHorizontal className="w-4 h-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem
                        onClick={() => handleOpenEdit(grupo.id)}
                        disabled={loadingEdit}
                      >
                        <Pencil className="w-4 h-4 mr-2" />
                        Editar
                      </DropdownMenuItem>
                      <DropdownMenuItem
                        className="text-destructive focus:text-destructive"
                        onClick={() => setGrupoToDelete(grupo)}
                        disabled={loadingEdit}
                      >
                        <Trash2 className="w-4 h-4 mr-2" />
                        Excluir
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {!loading && filteredGrupos.length === 0 && (
        <div className="text-center py-12">
          <Users className="w-12 h-12 mx-auto text-muted-foreground/50 mb-3" />
          <p className="text-muted-foreground">
            {searchTerm
              ? 'Nenhum grupo encontrado'
              : 'Nenhum grupo disponível'}
          </p>
        </div>
      )}

      <CreateUserGroupModal
        open={isCreateModalOpen}
        onOpenChange={handleCloseModal}
        initialGroup={editingGroup}
        onSave={handleSaveGroup}
        onCreated={loadGrupos}
      />

      <AlertDialog
        open={!!grupoToDelete}
        onOpenChange={(open) => !open && setGrupoToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir grupo</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja excluir o grupo &quot;{grupoToDelete?.nome}&quot;?
              Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deleting}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={(e) => {
                e.preventDefault();
                handleConfirmDelete();
              }}
              disabled={deleting}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleting ? 'Excluindo...' : 'Excluir'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
