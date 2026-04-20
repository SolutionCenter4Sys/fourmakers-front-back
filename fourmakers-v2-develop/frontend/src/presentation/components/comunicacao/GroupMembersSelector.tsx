import { useState, useEffect, useMemo, useCallback } from 'react';
import { Users, UserPlus, ChevronDown } from 'lucide-react';
import { useColaboradoresDisponiveis } from '@presentation/hooks/useColaboradoresDisponiveis';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command';
import { cn } from '@/lib/utils';

export interface GroupMemberItem {
  userId: string;
  userName: string;
  userEmail: string;
}

export interface GroupMembersSelectorProps {
  /** Lista atual de membros (controlado pelo parent). */
  members: GroupMemberItem[];
  /** Chamado quando a lista de membros muda (adicionar/remover). */
  onMembersChange: (members: GroupMemberItem[]) => void;
  /** ID do grupo (opcional); passado para a API de colaboradores disponíveis. */
  grupoId?: string;
  /** Rótulo da seção (ex.: "Membros do Grupo", "Usuários Individuais"). */
  label?: string;
  /** Texto quando não há membros na lista. */
  emptyMessage?: string;
  /** Se o popover está dentro de um modal, informar se o modal está aberto para habilitar busca. */
  open?: boolean;
  disabled?: boolean;
  className?: string;
}

export function GroupMembersSelector({
  members,
  onMembersChange,
  grupoId,
  label = 'Membros do Grupo',
  emptyMessage = 'Nenhum membro adicionado ainda',
  open: parentOpen = true,
  disabled = false,
  className,
}: GroupMembersSelectorProps) {
  const [usuarioComboboxOpen, setUsuarioComboboxOpen] = useState(false);
  const [buscaUsuario, setBuscaUsuario] = useState('');
  const [debouncedBuscaUsuario, setDebouncedBuscaUsuario] = useState('');
  const [selectedUserId, setSelectedUserId] = useState('');
  const [selectedUserDisplay, setSelectedUserDisplay] = useState<{
    userName: string;
    userEmail: string;
  } | null>(null);

  useEffect(() => {
    const t = setTimeout(() => setDebouncedBuscaUsuario(buscaUsuario), 300);
    return () => clearTimeout(t);
  }, [buscaUsuario]);

  const { colaboradores, loading: loadingColaboradores } = useColaboradoresDisponiveis(
    grupoId,
    debouncedBuscaUsuario,
    parentOpen && usuarioComboboxOpen,
  );

  const usuariosParaSelecao = useMemo(
    () =>
      colaboradores.map((c) => ({
        userId: c.codigoColaboradorInterno,
        userName: c.nomeCompleto,
        userEmail: c.email,
      })),
    [colaboradores],
  );

  const displayLabel =
    selectedUserDisplay?.userName ??
    usuariosParaSelecao.find((u) => u.userId === selectedUserId)?.userName ??
    selectedUserId;

  const handlePopoverOpenChange = useCallback((open: boolean) => {
    setUsuarioComboboxOpen(open);
    if (!open) {
      setBuscaUsuario('');
      setDebouncedBuscaUsuario('');
    }
  }, []);

  const handleAddMember = useCallback(() => {
    if (!selectedUserId) return;
    const user =
      usuariosParaSelecao.find((u) => u.userId === selectedUserId) ??
      (selectedUserDisplay
        ? {
            userId: selectedUserId,
            userName: selectedUserDisplay.userName,
            userEmail: selectedUserDisplay.userEmail,
          }
        : null);
    if (!user || members.some((m) => m.userId === user.userId)) return;
    onMembersChange([
      ...members,
      { userId: user.userId, userName: user.userName, userEmail: user.userEmail },
    ]);
    setSelectedUserId('');
    setSelectedUserDisplay(null);
  }, [selectedUserId, selectedUserDisplay, usuariosParaSelecao, members, onMembersChange]);

  const handleRemoveMember = useCallback(
    (userId: string) => {
      onMembersChange(members.filter((m) => m.userId !== userId));
    },
    [members, onMembersChange],
  );

  return (
    <div className={cn('space-y-2', className)}>
      <Label>{label}</Label>
      <div className="flex gap-2">
        <Popover open={usuarioComboboxOpen} onOpenChange={handlePopoverOpenChange}>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              role="combobox"
              aria-expanded={usuarioComboboxOpen}
              disabled={disabled}
              className={cn(
                'flex-1 justify-between rounded-lg font-normal',
                !selectedUserId && 'text-muted-foreground',
              )}
            >
              {selectedUserId ? displayLabel : 'Selecione um usuário'}
              <ChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
            <Command shouldFilter={false}>
              <CommandInput
                placeholder="Digite ao menos 2 caracteres para buscar..."
                value={buscaUsuario}
                onValueChange={setBuscaUsuario}
              />
              <CommandList>
                <CommandEmpty>
                  {buscaUsuario.trim().length < 2
                    ? 'Digite ao menos 2 caracteres para buscar.'
                    : loadingColaboradores
                      ? 'Buscando...'
                      : 'Nenhum usuário encontrado.'}
                </CommandEmpty>
                <CommandGroup>
                  {usuariosParaSelecao
                    .filter((u) => !members.some((m) => m.userId === u.userId))
                    .map((u) => (
                      <CommandItem
                        key={u.userId}
                        value={u.userId}
                        onSelect={() => {
                          setSelectedUserId(u.userId);
                          setSelectedUserDisplay({
                            userName: u.userName,
                            userEmail: u.userEmail,
                          });
                          setUsuarioComboboxOpen(false);
                        }}
                      >
                        {u.userName}
                        <span className="text-foreground text-xs ml-1">({u.userEmail})</span>
                      </CommandItem>
                    ))}
                </CommandGroup>
              </CommandList>
            </Command>
          </PopoverContent>
        </Popover>
        <Button
          type="button"
          variant="outline"
          className="gap-2 shrink-0"
          onClick={handleAddMember}
          disabled={disabled || !selectedUserId}
        >
          <UserPlus className="w-4 h-4" />
          Adicionar
        </Button>
      </div>
      <div className="min-h-[120px] rounded-lg border border-dashed border-muted-foreground/30 p-4 flex flex-col items-center justify-center">
        {members.length === 0 ? (
          <>
            <Users className="w-10 h-10 text-muted-foreground/50 mb-2" />
            <p className="text-sm text-muted-foreground text-center">{emptyMessage}</p>
          </>
        ) : (
          <ul className="w-full space-y-2">
            {members.map((m) => (
              <li
                key={m.userId}
                className="flex items-center justify-between gap-2 py-2 px-3 rounded-lg bg-muted/30 text-sm"
              >
                <span>
                  {m.userName}
                  <span className="text-foreground text-xs ml-1">({m.userEmail})</span>
                </span>
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  className="text-destructive hover:text-destructive h-8 px-2"
                  onClick={() => handleRemoveMember(m.userId)}
                  disabled={disabled}
                >
                  Remover
                </Button>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
