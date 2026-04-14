import { useState, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Check, Plus } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import { ListarClientesGestaoAlocadosUseCase } from '@domain/usecases/ListarClientesGestaoAlocadosUseCase';
import { CriarGrupoAcessoUseCase } from '@domain/usecases/CriarGrupoAcessoUseCase';
import { EditarGrupoAcessoUseCase } from '@domain/usecases/EditarGrupoAcessoUseCase';
import { toast } from 'sonner';

/** Aceita apenas letras (incl. acentuadas), espaços e underscore. */
const DESCRICAO_REGEX = /^[\p{L}\s_]*$/u;

function descricaoValida(val: string): boolean {
  return DESCRICAO_REGEX.test(val) && val.trim().length > 0;
}

/** Remove caracteres inválidos do input (não aceita números nem especiais). */
function sanitizeDescricao(val: string): string {
  return val
    .split('')
    .filter((c) => /[\p{L}\s_]/u.test(c))
    .join('');
}

/** Remove acentos (ex.: "ã" → "a", "é" → "e"). */
function removeAcentos(val: string): string {
  return val.normalize('NFD').replace(/\p{Mark}/gu, '');
}

/** Normaliza o nome do grupo: sem acentos, caixa alta e espaços viram underscore (ex.: "grupão" → "GRUPAO", "recrutamento admin" → "RECRUTAMENTO_ADMIN"). */
function normalizeNomeGrupo(val: string): string {
  const sanitized = sanitizeDescricao(val);
  const semAcentos = removeAcentos(sanitized);
  return semAcentos.toUpperCase().replace(/\s+/g, '_');
}

interface ClienteOption {
  codigoCliente: string;
  nomeCliente: string;
}

/** Quando definido, o modal abre em modo edição com os dados preenchidos. */
export interface GrupoAcessoInitialEdit {
  id: number;
  descricao: string;
  clientes: { codigoCliente: string }[];
}

interface CriarGrupoAcessoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string;
  onSuccess: () => void;
  /** Se definido, exibe o modal em modo edição com estes dados. */
  initialGrupo?: GrupoAcessoInitialEdit | null;
}

export function CriarGrupoAcessoModal({
  open,
  onOpenChange,
  token,
  onSuccess,
  initialGrupo,
}: CriarGrupoAcessoModalProps) {
  const isEdit = !!initialGrupo;
  const [descricao, setDescricao] = useState('');
  const [clientes, setClientes] = useState<ClienteOption[]>([]);
  const [clientSearch, setClientSearch] = useState('');
  const [clientesLoading, setClientesLoading] = useState(false);
  const [selectedCodigos, setSelectedCodigos] = useState<Set<string>>(new Set());
  const [submitting, setSubmitting] = useState(false);

  const listarClientesUseCase = container.resolve(ListarClientesGestaoAlocadosUseCase);
  const criarGrupoAcessoUseCase = container.resolve(CriarGrupoAcessoUseCase);
  const editarGrupoAcessoUseCase = container.resolve(EditarGrupoAcessoUseCase);

  const [debouncedClientSearch, setDebouncedClientSearch] = useState(clientSearch);
  useEffect(() => {
    const id = setTimeout(() => setDebouncedClientSearch(clientSearch), 300);
    return () => clearTimeout(id);
  }, [clientSearch]);

  useEffect(() => {
    if (!open) return;
    if (initialGrupo) {
      setDescricao(initialGrupo.descricao);
      setSelectedCodigos(new Set((initialGrupo.clientes ?? []).map((c) => c.codigoCliente)));
    } else {
      setDescricao('');
      setSelectedCodigos(new Set());
    }
    setClientSearch('');
  }, [open, initialGrupo]);

  useEffect(() => {
    if (!open || !token) return;
    setClientesLoading(true);
    listarClientesUseCase
      .execute(token, debouncedClientSearch.trim())
      .then((response) => {
        const lista = response?.retorno ?? [];
        setClientes(
          lista.map((c) => ({
            codigoCliente: c.codigoCliente ?? '',
            nomeCliente: c.nomeCliente ?? '',
          }))
        );
      })
      .catch(() => setClientes([]))
      .finally(() => setClientesLoading(false));
  }, [open, token, debouncedClientSearch, listarClientesUseCase]);

  const handleOpenChange = (next: boolean) => {
    if (!next) {
      if (!isEdit) {
        setDescricao('');
        setSelectedCodigos(new Set());
        setClientSearch('');
      }
    }
    onOpenChange(next);
  };

  const toggleCliente = (codigoCliente: string) => {
    setSelectedCodigos((prev) => {
      const next = new Set(prev);
      if (next.has(codigoCliente)) next.delete(codigoCliente);
      else next.add(codigoCliente);
      return next;
    });
  };

  const handleSubmit = async () => {
    const trimmed = descricao.trim();
    if (!trimmed) {
      toast.error('Informe a descrição do grupo.');
      return;
    }
    if (!descricaoValida(trimmed)) {
      toast.error('A descrição deve conter apenas letras, espaços e underscore. Não use números nem caracteres especiais.');
      return;
    }
    setSubmitting(true);
    try {
      const clientesPayload = Array.from(selectedCodigos).map((codigoCliente) => ({ codigoCliente }));
      if (isEdit && initialGrupo) {
        const res = await editarGrupoAcessoUseCase.execute(token, {
          id: initialGrupo.id,
          descricao: trimmed,
          clientes: clientesPayload,
        });
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Grupo atualizado com sucesso.');
          handleOpenChange(false);
          onSuccess();
        } else {
          toast.error(res.mensagem ?? 'Erro ao editar grupo.');
        }
      } else {
        const res = await criarGrupoAcessoUseCase.execute(token, {
          descricao: trimmed,
          clientes: clientesPayload,
        });
        if (res.sucesso) {
          toast.success('A lista de grupos foi atualizada.');
          handleOpenChange(false);
          onSuccess();
        } else {
          toast.error(res.mensagem ?? 'Erro ao criar grupo.');
        }
      }
    } catch (err: unknown) {
      const msg =
        err && typeof err === 'object' && err !== null && 'mensagem' in err
          ? String((err as { mensagem?: string }).mensagem)
          : err instanceof Error
            ? err.message
            : isEdit
              ? 'Erro ao editar grupo.'
              : 'Erro ao criar grupo.';
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-[480px]">
        <DialogHeader>
          <DialogTitle>{isEdit ? 'Editar Grupo de Acesso' : 'Criar Grupo'}</DialogTitle>
          <DialogDescription>
            {isEdit
              ? 'Altere a descrição e/ou os clientes vinculados ao grupo.'
              : 'Informe o nome do grupo e opcionalmente os clientes vinculados.'}
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-4 py-4">
          <div className="space-y-2">
            <Label htmlFor="descricao">Descrição do grupo</Label>
            <Input
              id="descricao"
              placeholder="Ex: RECRUTAMENTO_ADMIN"
              value={descricao}
              onChange={(e) => setDescricao(normalizeNomeGrupo(e.target.value))}
              maxLength={200}
            />
            <p className="text-xs text-muted-foreground">
              Nome em caixa alta, sem acentos, com underscore no lugar de espaços (ex.: grupão → GRUPAO, recrutamento admin → RECRUTAMENTO_ADMIN).
            </p>
          </div>
          <div className="space-y-2">
            <Label>Clientes (opcional)</Label>
            <Command shouldFilter={false} className="rounded-lg border">
              <CommandInput
                placeholder="Buscar cliente..."
                value={clientSearch}
                onValueChange={setClientSearch}
                className="text-sm"
              />
              <CommandList>
                {clientesLoading ? (
                  <CommandEmpty>Carregando...</CommandEmpty>
                ) : clientes.length === 0 ? (
                  <CommandEmpty>Nenhum cliente encontrado</CommandEmpty>
                ) : (
                  <CommandGroup>
                    <ScrollArea className="h-[200px]">
                      {clientes.map((c) => (
                        <CommandItem
                          key={c.codigoCliente}
                          value={c.codigoCliente}
                          onSelect={() => toggleCliente(c.codigoCliente)}
                          className="text-sm"
                        >
                          <Check
                            className={`mr-2 h-4 w-4 ${selectedCodigos.has(c.codigoCliente) ? 'opacity-100' : 'opacity-0'}`}
                          />
                          {c.nomeCliente} ({c.codigoCliente})
                        </CommandItem>
                      ))}
                    </ScrollArea>
                  </CommandGroup>
                )}
              </CommandList>
            </Command>
            {selectedCodigos.size > 0 && (
              <p className="text-xs text-muted-foreground">
                {selectedCodigos.size} cliente(s) selecionado(s).
              </p>
            )}
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => handleOpenChange(false)} disabled={submitting}>
            Cancelar
          </Button>
          <Button onClick={handleSubmit} disabled={submitting}>
            {submitting ? (
              <>
                <Spinner className="mr-2" size={16} />
                {isEdit ? 'Salvando...' : 'Criando...'}
              </>
            ) : isEdit ? (
              'Salvar alterações'
            ) : (
              <>
                <Plus className="mr-2 h-4 w-4" />
                Criar Grupo
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
