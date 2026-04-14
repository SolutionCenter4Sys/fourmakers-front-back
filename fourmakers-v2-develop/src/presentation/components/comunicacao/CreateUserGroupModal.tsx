import { useState, useEffect, useCallback } from 'react';
import { container } from 'tsyringe';
import { Users, Megaphone, FileText, ShieldCheck } from 'lucide-react';
import type { UserGroup, UserGroupMember } from '@domain/entities/comunicacao';
import { CriarComunicacaoGrupoUseCase } from '@domain/usecases/CriarComunicacaoGrupoUseCase';
import { AtualizarComunicacaoGrupoUseCase } from '@domain/usecases/AtualizarComunicacaoGrupoUseCase';
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
import { Textarea } from '@/components/ui/textarea';
import { Switch } from '@/components/ui/switch';
import { toast } from 'sonner';
import { GroupMembersSelector, type GroupMemberItem } from './GroupMembersSelector';

interface CreateUserGroupModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  initialGroup?: UserGroup | null;
  onSave?: (group: UserGroup) => void;
  /** Chamado após criar grupo via API (para refetch da lista). */
  onCreated?: () => void;
}

export function CreateUserGroupModal({
  open,
  onOpenChange,
  initialGroup,
  onSave,
  onCreated,
}: CreateUserGroupModalProps) {
  const isEdit = !!initialGroup;
  const token = useAppSelector((state) => state.auth.token);
  const criarGrupoUseCase = container.resolve(CriarComunicacaoGrupoUseCase);
  const atualizarGrupoUseCase = container.resolve(AtualizarComunicacaoGrupoUseCase);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [requiresApproval, setRequiresApproval] = useState(false);
  const [permiteCriarPublicacaoInformativo, setPermiteCriarPublicacaoInformativo] = useState(false);
  const [aprovaPublicacaoInformativo, setAprovaPublicacaoInformativo] = useState(false);
  const [permiteCriarComunidade, setPermiteCriarComunidade] = useState(false);
  const [members, setMembers] = useState<UserGroupMember[]>([]);
  const [submitting, setSubmitting] = useState(false);

  const membersAsItems: GroupMemberItem[] = members.map((m) => ({
    userId: m.userId,
    userName: m.userName,
    userEmail: m.userEmail,
  }));

  const handleMembersChange = useCallback((newItems: GroupMemberItem[]) => {
    setMembers(
      newItems.map((m, i) => ({
        id: `ugm-${m.userId}-${Date.now()}-${i}`,
        userId: m.userId,
        userName: m.userName,
        userEmail: m.userEmail,
        addedAt: new Date().toISOString().split('T')[0],
      })),
    );
  }, []);

  useEffect(() => {
    if (open && initialGroup) {
      setName(initialGroup.name);
      setDescription(initialGroup.description);
      setRequiresApproval(initialGroup.requiresApproval);
      setPermiteCriarPublicacaoInformativo(
        initialGroup.permiteCriarPublicacaoInformativo ?? true,
      );
      setAprovaPublicacaoInformativo(
        initialGroup.aprovaPublicacaoInformativo ?? false,
      );
      setPermiteCriarComunidade(initialGroup.permiteCriarComunidade ?? false);
      setMembers(initialGroup.members);
    }
    if (open && !initialGroup) {
      setName('');
      setDescription('');
      setRequiresApproval(false);
      setPermiteCriarPublicacaoInformativo(true);
      setAprovaPublicacaoInformativo(false);
      setPermiteCriarComunidade(false);
      setMembers([]);
    }
  }, [open, initialGroup]);

  const handleSave = async () => {
    const now = new Date().toISOString().split('T')[0];
    if (isEdit && initialGroup && token) {
      setSubmitting(true);
      try {
        const res = await atualizarGrupoUseCase.execute(
          token,
          initialGroup.id,
          {
            nome: name.trim(),
            descricao: description.trim(),
            permiteCriarPublicacaoInformativo,
            publicacaoInformativoRequerAprovacao: requiresApproval,
            aprovaPublicacaoInformativo,
            permiteCriarComunidade,
            codigoInternoColaboradoresParticipantes: members.map((m) => m.userId),
          },
        );
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Grupo atualizado com sucesso.', {
            description: `O grupo "${name}" foi atualizado.`,
          });
          onCreated?.();
          const group: UserGroup = {
            ...initialGroup,
            name,
            description,
            requiresApproval,
            permiteCriarPublicacaoInformativo,
            aprovaPublicacaoInformativo,
            permiteCriarComunidade,
            members,
            updatedAt: now,
          };
          onSave?.(group);
          setName('');
          setDescription('');
          setRequiresApproval(false);
          setPermiteCriarPublicacaoInformativo(true);
          setAprovaPublicacaoInformativo(false);
          setPermiteCriarComunidade(false);
          setMembers([]);
          onOpenChange(false);
        } else {
          toast.error(res.mensagem ?? 'Não foi possível atualizar o grupo.');
        }
      } catch {
        toast.error('Erro ao atualizar grupo. Tente novamente.');
      } finally {
        setSubmitting(false);
      }
      return;
    }
    if (!token) return;
    setSubmitting(true);
    try {
      const res = await criarGrupoUseCase.execute(token, {
        nome: name.trim(),
        descricao: description.trim(),
        permiteCriarPublicacaoInformativo,
        publicacaoInformativoRequerAprovacao: requiresApproval,
        aprovaPublicacaoInformativo,
        permiteCriarComunidade,
        codigoInternoColaboradoresParticipantes: members.map((m) => m.userId),
      });
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Grupo criado com sucesso!', {
          description: `O grupo "${name}" foi criado.`,
        });
        onCreated?.();
        const group: UserGroup = {
          id: `ug${Date.now()}`,
          name,
          description,
          status: 'active',
          requiresApproval,
          permiteCriarComunidade,
          members,
          createdAt: now,
          updatedAt: now,
          createdBy: 'current-user',
        };
        onSave?.(group);
        setName('');
        setDescription('');
        setRequiresApproval(false);
        setPermiteCriarPublicacaoInformativo(true);
        setAprovaPublicacaoInformativo(false);
        setPermiteCriarComunidade(false);
        setMembers([]);
        onOpenChange(false);
      } else {
        toast.error(res.mensagem ?? 'Não foi possível criar o grupo.');
      }
    } catch {
      toast.error('Erro ao criar grupo. Tente novamente.');
    } finally {
      setSubmitting(false);
    }
  };

  const isValid = name.trim() && description.trim();

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg flex flex-col max-h-[90vh] p-0 gap-0">
        <DialogHeader className="shrink-0 px-6 pr-14 pt-6 pb-4 border-b">
          <div className="flex items-center gap-2">
            <Users className="w-6 h-6 text-primary shrink-0" />
            <div>
              <DialogTitle>{isEdit ? 'Editar Grupo' : 'Criar Novo Grupo'}</DialogTitle>
            </div>
          </div>
        </DialogHeader>

        <div className="flex-1 min-h-0 overflow-y-auto px-6 py-4 space-y-4">
          <div className="space-y-2">
            <Label htmlFor="ug-name">
              Nome do Grupo <span className="text-destructive">*</span>
            </Label>
            <Input
              id="ug-name"
              placeholder="Ex: Departamento de TI"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="rounded-lg"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="ug-desc">
              Descrição <span className="text-destructive">*</span>
            </Label>
            <Textarea
              id="ug-desc"
              placeholder="Descreva o propósito deste grupo..."
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
              className="rounded-lg resize-y"
            />
          </div>

          {/* Bloco: Comunicados Oficiais */}
          <div className="rounded-lg border bg-muted/10 p-4 space-y-4">
            <h3 className="text-sm font-semibold text-foreground">Comunicados Oficiais</h3>
            <div className="flex items-center justify-between gap-4">
              <div className="flex items-start gap-3 min-w-0">
                <div className="p-2 rounded-lg bg-background">
                  <FileText className="w-5 h-5 text-primary" />
                </div>
                <div>
                  <p className="font-medium text-sm">Permitir criar comunicado</p>
                  <p className="text-xs text-muted-foreground mt-0.5">
                    Membros do grupo podem criar comunicados
                  </p>
                </div>
              </div>
              <Switch
                checked={permiteCriarPublicacaoInformativo}
                onCheckedChange={setPermiteCriarPublicacaoInformativo}
                className="shrink-0"
              />
            </div>
            <div className="flex items-center justify-between gap-4">
              <div className="flex items-start gap-3 min-w-0">
                <div className="p-2 rounded-lg bg-background">
                  <Megaphone className="w-5 h-5 text-primary" />
                </div>
                <div>
                  <p className="font-medium text-sm">Comunicado requer aprovação</p>
                  <p className="text-xs text-muted-foreground mt-0.5">
                    Comunicados criados por membros precisam ser aprovados antes de publicar
                  </p>
                </div>
              </div>
              <Switch
                checked={requiresApproval}
                onCheckedChange={setRequiresApproval}
                className="shrink-0"
              />
            </div>
            <div className="flex items-center justify-between gap-4">
              <div className="flex items-start gap-3 min-w-0">
                <div className="p-2 rounded-lg bg-background">
                  <ShieldCheck className="w-5 h-5 text-primary" />
                </div>
                <div>
                  <p className="font-medium text-sm">Aprovar comunicado</p>
                  <p className="text-xs text-muted-foreground mt-0.5">
                    Este grupo pode aprovar comunicado(s) pendente(s)
                  </p>
                </div>
              </div>
              <Switch
                checked={aprovaPublicacaoInformativo}
                onCheckedChange={setAprovaPublicacaoInformativo}
                className="shrink-0"
              />
            </div>
          </div>

          {/* Bloco: Comunidades */}
          <div className="rounded-lg border bg-muted/10 p-4 space-y-4">
            <h3 className="text-sm font-semibold text-foreground">Comunidades</h3>
            <div className="flex items-center justify-between gap-4">
              <div className="flex items-start gap-3 min-w-0">
                <div className="p-2 rounded-lg bg-background">
                  <Users className="w-5 h-5 text-primary" />
                </div>
                <div>
                  <p className="font-medium text-sm">Permite criar comunidades</p>
                  <p className="text-xs text-muted-foreground mt-0.5">
                    Membros do grupo podem criar novas comunidades
                  </p>
                </div>
              </div>
              <Switch
                checked={permiteCriarComunidade}
                onCheckedChange={setPermiteCriarComunidade}
                className="shrink-0"
              />
            </div>
          </div>

          <GroupMembersSelector
            members={membersAsItems}
            onMembersChange={handleMembersChange}
            grupoId={initialGroup?.id}
            label="Membros do Grupo"
            emptyMessage="Nenhum membro adicionado ainda"
            open={open}
          />
        </div>

        <div className="shrink-0 flex justify-end gap-2 px-6 py-4 border-t bg-background">
          <Button variant="ghost" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={!isValid || submitting}>
            {isEdit ? (submitting ? 'Salvando...' : 'Salvar') : submitting ? 'Criando...' : 'Criar Grupo'}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
