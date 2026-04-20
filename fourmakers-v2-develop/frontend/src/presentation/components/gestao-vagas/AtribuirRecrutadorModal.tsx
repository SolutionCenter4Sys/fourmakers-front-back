import { useState, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command';
import type { ColaboradorCch } from '@domain/entities/ColaboradorCch';
import { ListarColaboradoresOrgUseCase } from '@domain/usecases/ListarColaboradoresOrgUseCase';
import { AdicionarRecrutadorVagaUseCase } from '@domain/usecases/AdicionarRecrutadorVagaUseCase';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';
import { ChevronDown } from '@/components/ui/system-icons';
import { normalizarParaBusca } from '@shared/utils/stringUtils';

interface AtribuirRecrutadorModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  vagaId: string;
  vagaTitulo?: string;
  /** Nome do recrutador atual (para pré-selecionar no dropdown quando já existe responsável). */
  currentRecrutadorNome?: string | null;
  token: string | null;
  onSuccess?: () => void;
}

export function AtribuirRecrutadorModal({
  open,
  onOpenChange,
  vagaId,
  vagaTitulo,
  currentRecrutadorNome,
  token,
  onSuccess,
}: AtribuirRecrutadorModalProps) {
  const [colaboradores, setColaboradores] = useState<ColaboradorCch[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [selectedCodigo, setSelectedCodigo] = useState<string>('');
  const [recrutadorPopoverOpen, setRecrutadorPopoverOpen] = useState(false);

  const listarColaboradoresOrg = container.resolve(ListarColaboradoresOrgUseCase);
  const adicionarRecrutadorVaga = container.resolve(AdicionarRecrutadorVagaUseCase);

  useEffect(() => {
    if (!open || !token) {
      setColaboradores([]);
      setSelectedCodigo('');
      return;
    }
    let cancelled = false;
    setLoading(true);
    const nomeAtual = currentRecrutadorNome != null ? String(currentRecrutadorNome).trim() : '';
    listarColaboradoresOrg
      .execute(token, { busca: '', cursor: 0, limite: 50000 })
      .then((res) => {
        if (cancelled) return;
        const list = res?.ColaboradoresCchResult ?? [];
        setColaboradores(list);
        if (nomeAtual && list.length > 0) {
          const found = list.find(
            (c) => (c.nm_Profissional ?? '').trim().toLowerCase() === nomeAtual.toLowerCase()
          );
          setSelectedCodigo(found ? found.codigoColaboradorInterno : '');
        } else {
          setSelectedCodigo('');
        }
      })
      .catch(() => {
        if (!cancelled) setColaboradores([]);
        if (!cancelled) setSelectedCodigo('');
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [open, token, currentRecrutadorNome, listarColaboradoresOrg]);

  const handleSave = async () => {
    if (!selectedCodigo || !token) return;
    setSaving(true);
    try {
      const res = await adicionarRecrutadorVaga.execute(token, vagaId, selectedCodigo);
      if (res?.sucesso !== false) {
        const mensagem = res?.retorno ?? 'Recrutador cadastrado com sucesso.';
        toast.success(mensagem);
        onOpenChange(false);
        onSuccess?.();
      } else {
        toast.error(res?.mensagem ?? 'Erro ao atribuir recrutador.');
      }
    } catch {
      toast.error('Erro ao atribuir recrutador.');
    } finally {
      setSaving(false);
    }
  };

  const canSave = !!selectedCodigo && !saving;
  const selectedColaborador = colaboradores.find((c) => c.codigoColaboradorInterno === selectedCodigo);
  const displayValue = selectedColaborador?.nm_Profissional ?? selectedColaborador?.codigoColaboradorInterno ?? '';

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md" aria-labelledby="atribuir-recrutador-title">
        <DialogHeader>
          <DialogTitle id="atribuir-recrutador-title">Recrutador/a</DialogTitle>
          <DialogDescription>
            Insira o nome do recrutador responsável por esta vaga{vagaTitulo ? `: ${vagaTitulo}` : ''}.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-2">
          <Label id="recrutador-label">Recrutador</Label>
          {loading ? (
            <p className="text-sm text-muted-foreground">Carregando colaboradores…</p>
          ) : (
            <Popover open={recrutadorPopoverOpen} onOpenChange={setRecrutadorPopoverOpen} modal>
              <PopoverTrigger asChild>
                <Button
                  id="recrutador-select"
                  variant="outline"
                  role="combobox"
                  aria-expanded={recrutadorPopoverOpen}
                  aria-labelledby="recrutador-label"
                  aria-haspopup="listbox"
                  className="w-full justify-between font-normal"
                  disabled={colaboradores.length === 0}
                >
                  <span className={cn(!displayValue && 'text-muted-foreground')}>
                    {displayValue || 'Insira o nome do recrutador'}
                  </span>
                  <ChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </PopoverTrigger>
              <PopoverContent
                className="w-[var(--radix-popover-trigger-width)] max-h-[min(280px,40vh)] overflow-hidden p-0"
                align="start"
                side="bottom"
                sideOffset={4}
                avoidCollisions={false}
              >
                <Command
                  shouldFilter={true}
                  filter={(value, search) => {
                    const nVal = normalizarParaBusca(value);
                    const nSearch = normalizarParaBusca(search);
                    if (!nSearch) return 1;
                    return nVal.includes(nSearch) ? 1 : 0;
                  }}
                >
                  <CommandInput placeholder="Buscar recrutador..." />
                  <CommandList className="max-h-[min(220px,calc(40vh-3.5rem))]">
                    <CommandEmpty>Nenhum colaborador encontrado.</CommandEmpty>
                    <CommandGroup>
                      {colaboradores.map((c) => (
                        <CommandItem
                          key={c.codigoColaboradorInterno}
                          value={c.nm_Profissional ?? c.codigoColaboradorInterno}
                          onSelect={() => {
                            setSelectedCodigo(c.codigoColaboradorInterno);
                            setRecrutadorPopoverOpen(false);
                          }}
                        >
                          {c.nm_Profissional ?? c.codigoColaboradorInterno}
                        </CommandItem>
                      ))}
                    </CommandGroup>
                  </CommandList>
                </Command>
              </PopoverContent>
            </Popover>
          )}
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={saving}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={!canSave}>
            Salvar
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
