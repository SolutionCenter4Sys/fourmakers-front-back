import { useState } from 'react';
import { Users, Plus, Search, Pencil, Trash2 } from 'lucide-react';
import type { UserGroup } from '@domain/entities/comunicacao';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { CreateUserGroupModal } from './CreateUserGroupModal';
import { toast } from 'sonner';
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

interface UserGroupsManagerProps {
  userGroups: UserGroup[];
  setUserGroups: React.Dispatch<React.SetStateAction<UserGroup[]>>;
}

function getInitials(name: string): string {
  return name
    .split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2);
}

export function UserGroupsManager({
  userGroups,
  setUserGroups,
}: UserGroupsManagerProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState<UserGroup | null>(null);
  const [groupToDelete, setGroupToDelete] = useState<UserGroup | null>(null);

  const handleOpenCreate = () => {
    setEditingGroup(null);
    setIsCreateModalOpen(true);
  };

  const handleOpenEdit = (group: UserGroup) => {
    setEditingGroup(group);
    setIsCreateModalOpen(true);
  };

  const handleCloseModal = (open: boolean) => {
    if (!open) setEditingGroup(null);
    setIsCreateModalOpen(open);
  };

  const handleSaveGroup = (group: UserGroup) => {
    if (editingGroup) {
      setUserGroups((prev) =>
        prev.map((g) => (g.id === group.id ? group : g)),
      );
      toast.success('Grupo atualizado!', {
        description: `As alterações em "${group.name}" foram salvas.`,
      });
    } else {
      setUserGroups((prev) => [group, ...prev]);
      toast.success('Grupo criado com sucesso!', {
        description: `O grupo "${group.name}" foi criado.`,
      });
    }
    setEditingGroup(null);
    setIsCreateModalOpen(false);
  };

  const handleConfirmDelete = () => {
    if (!groupToDelete) return;
    setUserGroups((prev) => prev.filter((g) => g.id !== groupToDelete.id));
    toast.success('Grupo excluído.', {
      description: `"${groupToDelete.name}" foi removido.`,
    });
    setGroupToDelete(null);
  };

  const filteredGroups = userGroups.filter(
    (group) =>
      group.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      group.description.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  const modalOpen = isCreateModalOpen || !!editingGroup;

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between gap-4">
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input
            placeholder="Buscar grupos de usuários..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <Button className="gap-2" onClick={handleOpenCreate}>
          <Plus className="w-4 h-4" />
          Criar Grupo
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {filteredGroups.map((group) => (
          <Card
            key={group.id}
            className="overflow-hidden rounded-xl shadow-sm border border-border hover:shadow-md transition-shadow"
          >
            <CardContent className="p-4 flex flex-col gap-4">
              <div className="flex items-start justify-between gap-2">
                <div className="min-w-0 flex-1">
                  <h3 className="font-semibold text-foreground truncate">
                    {group.name}
                  </h3>
                  <p className="text-sm text-muted-foreground mt-0.5 line-clamp-2">
                    {group.description}
                  </p>
                </div>
                <Badge
                  variant={group.status === 'active' ? 'default' : 'secondary'}
                  className="shrink-0 rounded-full bg-primary text-primary-foreground"
                >
                  {group.status === 'active' ? 'Ativo' : 'Inativo'}
                </Badge>
              </div>

              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-foreground">
                  <Users className="w-4 h-4 text-muted-foreground" />
                  <span>
                    {group.members.length} membro
                    {group.members.length !== 1 ? 's' : ''}
                  </span>
                </div>
                {group.members.length > 0 && (
                  <div className="flex -space-x-2">
                    {group.members.slice(0, 3).map((m) => (
                      <Avatar
                        key={m.id}
                        className="h-8 w-8 border-2 border-background"
                      >
                        <AvatarFallback className="bg-primary text-primary-foreground text-xs">
                          {getInitials(m.userName)}
                        </AvatarFallback>
                      </Avatar>
                    ))}
                  </div>
                )}
              </div>

              <div className="flex gap-2 pt-1">
                <Button
                  variant="outline"
                  size="sm"
                  className="flex-1 gap-2 rounded-lg border-border"
                  onClick={() => handleOpenEdit(group)}
                >
                  <Pencil className="w-4 h-4" />
                  Editar
                </Button>
                <Button
                  variant="outline"
                  size="icon"
                  className="shrink-0 rounded-lg border-border text-destructive hover:bg-destructive/10 hover:text-destructive"
                  onClick={() => setGroupToDelete(group)}
                  aria-label="Excluir grupo"
                >
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {filteredGroups.length === 0 && (
        <div className="text-center py-12">
          <Users className="w-12 h-12 mx-auto text-muted-foreground/50 mb-3" />
          <p className="text-muted-foreground">
            {searchTerm
              ? 'Nenhum grupo encontrado'
              : 'Nenhum grupo de usuários cadastrado. Crie o primeiro!'}
          </p>
          {!searchTerm && (
            <Button className="mt-4 gap-2" onClick={handleOpenCreate}>
              <Plus className="w-4 h-4" />
              Criar Grupo
            </Button>
          )}
        </div>
      )}

      <CreateUserGroupModal
        open={modalOpen}
        onOpenChange={handleCloseModal}
        initialGroup={editingGroup}
        onSave={handleSaveGroup}
      />

      <AlertDialog open={!!groupToDelete} onOpenChange={(open) => !open && setGroupToDelete(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir grupo?</AlertDialogTitle>
            <AlertDialogDescription>
              O grupo &quot;{groupToDelete?.name}&quot; será excluído. Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleConfirmDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              Excluir
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
