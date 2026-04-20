import { useState, useEffect, useMemo, useCallback } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Spinner } from '@/components/ui/spinner';
import type { Colaborador } from '@domain/entities/Colaborador';
import { GetColaboradoresUseCase } from '@domain/usecases/GetColaboradoresUseCase';
import { AdicionarUsuarioGrupoUseCase } from '@domain/usecases/AdicionarUsuarioGrupoUseCase';
import { toast } from 'sonner';
import { logUserAction } from '@shared/utils/firebaseAnalytics';
import { X } from '@/components/ui/system-icons';
import { cn } from '@/lib/utils';

const DEBOUNCE_MS = 300;
const MIN_CHARS = 2;
const LIMITE = 5000;

function useDebounced(value: string, delay: number): string {
  const [debounced, setDebounced] = useState(value);
  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay);
    return () => clearTimeout(timer);
  }, [value, delay]);
  return debounced;
}

interface AdicionarUsuarioGrupoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string;
  grupoAcessoId: number;
  grupoDescricao?: string;
  orgId: number;
  onSuccess?: () => void;
}

export function AdicionarUsuarioGrupoModal({
  open,
  onOpenChange,
  token,
  grupoAcessoId,
  grupoDescricao,
  orgId,
  onSuccess,
}: AdicionarUsuarioGrupoModalProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState<Colaborador[]>([]);
  const [selectedColaborador, setSelectedColaborador] = useState<Colaborador | null>(null);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [listOpen, setListOpen] = useState(false);

  const debouncedSearch = useDebounced(searchTerm, DEBOUNCE_MS);
  const user = useAppSelector((state) => state.auth.user);
  const getColaboradoresUseCase = useMemo(() => container.resolve(GetColaboradoresUseCase), []);
  const adicionarUsuarioGrupoUseCase = container.resolve(AdicionarUsuarioGrupoUseCase);

  // Ao abrir o modal, limpa seleção e busca
  useEffect(() => {
    if (!open) return;
    setSearchTerm('');
    setSearchResults([]);
    setSelectedColaborador(null);
    setListOpen(false);
  }, [open]);

  // Busca colaboradores só com 2+ caracteres, após 300ms
  useEffect(() => {
    if (!open || !token || !orgId || debouncedSearch.trim().length < MIN_CHARS) {
      setSearchResults([]);
      setLoading(false);
      return;
    }
    setLoading(true);
    getColaboradoresUseCase
      .execute(token, {
        cursor: 0,
        limite: LIMITE,
        nomeOuEmail: debouncedSearch.trim(),
        org: orgId,
      })
      .then((res) => {
        const lista = (res?.retorno ?? []) as Colaborador[];
        setSearchResults(lista.filter((c) => c.ativo === true && c.usuarioId != null));
      })
      .catch(() => setSearchResults([]))
      .finally(() => setLoading(false));
  }, [open, token, orgId, debouncedSearch, getColaboradoresUseCase]);

  const handleInputChange = useCallback((value: string) => {
    setSearchTerm(value);
    setSelectedColaborador(null);
    setListOpen(value.trim().length >= MIN_CHARS);
  }, []);

  const handleSelect = useCallback((c: Colaborador) => {
    setSelectedColaborador(c);
    setSearchTerm('');
    setListOpen(false);
  }, []);

  const handleClearSelection = useCallback(() => {
    setSelectedColaborador(null);
    setSearchTerm('');
  }, []);

  const handleAtribuir = async () => {
    if (!selectedColaborador?.usuarioId) {
      toast.error('Selecione um colaborador.');
      return;
    }
    setSubmitting(true);
    try {
      const res = await adicionarUsuarioGrupoUseCase.execute(token, selectedColaborador.usuarioId, grupoAcessoId);
      if (res.sucesso) {
        logUserAction('Permissionamento', 'AdicionarUsuarioGrupo', { grupoAcessoId, usuarioId: selectedColaborador.usuarioId }, user);
        toast.success(res.mensagem ?? 'Usuário adicionado ao grupo com sucesso.');
        onSuccess?.();
        setSelectedColaborador(null);
        setSearchTerm('');
      } else {
        toast.error(res.mensagem ?? 'Erro ao adicionar usuário ao grupo.');
      }
    } catch (err: unknown) {
      const msg =
        err && typeof err === 'object' && err !== null && 'mensagem' in err
          ? String((err as { mensagem?: string }).mensagem)
          : err instanceof Error
            ? err.message
            : 'Erro ao adicionar usuário ao grupo.';
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  };

  const displayValue = selectedColaborador ? `${selectedColaborador.nome} (${selectedColaborador.usuarioId})` : searchTerm;
  const showList = listOpen && searchTerm.trim().length >= MIN_CHARS;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[420px]">
        <DialogHeader>
          <DialogTitle>Adicionar pessoa ao grupo</DialogTitle>
          <DialogDescription>
            {grupoDescricao ? `Grupo: ${grupoDescricao}. ` : ''}
            Digite pelo menos 2 caracteres para buscar o colaborador e selecione na lista.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-4 py-4">
          <div className="space-y-2">
            <Label>Colaborador</Label>
            <div className="relative">
              <div className="flex gap-1">
                <Input
                  type="text"
                  placeholder="Buscar por nome ou e-mail (mín. 2 caracteres)"
                  value={displayValue}
                  onChange={(e) => handleInputChange(e.target.value)}
                  onFocus={() => searchTerm.trim().length >= MIN_CHARS && setListOpen(true)}
                  onBlur={() => setTimeout(() => setListOpen(false), 200)}
                  className={cn(selectedColaborador && 'pr-8')}
                  disabled={!!selectedColaborador}
                />
                {selectedColaborador && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="shrink-0"
                    onClick={handleClearSelection}
                    aria-label="Limpar seleção"
                  >
                    <X className="h-4 w-4" />
                  </Button>
                )}
              </div>
              {showList && (
                <div
                  className="absolute z-50 mt-1 w-full rounded-md border bg-popover shadow-md max-h-[300px] overflow-y-auto"
                  onMouseDown={(e) => e.preventDefault()}
                >
                  {loading ? (
                    <div className="flex items-center justify-center gap-2 p-4 text-sm text-muted-foreground">
                      <Spinner size={16} className="text-current" />
                      Buscando...
                    </div>
                  ) : searchResults.length === 0 ? (
                    <div className="p-4 text-sm text-muted-foreground">Nenhum colaborador encontrado.</div>
                  ) : (
                    <ul className="py-1">
                      {searchResults.map((c) => (
                        <li key={c.usuarioId ?? c.codColaborador}>
                          <button
                            type="button"
                            className="w-full px-3 py-2 text-left text-sm hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground outline-none"
                            onClick={() => handleSelect(c)}
                          >
                            {c.nome} ({c.usuarioId})
                          </button>
                        </li>
                      ))}
                    </ul>
                  )}
                </div>
              )}
            </div>
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={submitting}>
            Fechar
          </Button>
          <Button onClick={handleAtribuir} disabled={submitting || !selectedColaborador}>
            {submitting ? (
              <>
                <Spinner className="mr-2" size={16} />
                Atribuindo...
              </>
            ) : (
              'Atribuir usuário'
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
