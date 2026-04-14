import { useState, useEffect, useRef, useCallback, memo } from 'react';
import { container } from '@core/di/container';
import { ListarColaboradoresOrgUseCase } from '@domain/usecases/ListarColaboradoresOrgUseCase';
import type { ColaboradorCch } from '@domain/entities/ColaboradorCch';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command';
import { ChevronDown } from '@/components/ui/system-icons';

const DEBOUNCE_MS = 350;
/** Limite de itens na lista para evitar travar a tela (a API filtra por busca). */
const LIMITE_LISTA = 50;

export interface ColaboradorSearchFieldProps {
  label: string;
  token: string | null;
  valueCodigo: string | null | undefined;
  valueDisplayName: string | null | undefined;
  onSelect: (codigoColaboradorInterno: string, displayName: string) => void;
  placeholder?: string;
  disabled?: boolean;
}

function ColaboradorSearchFieldInner({
  label,
  token,
  valueCodigo,
  valueDisplayName,
  onSelect,
  placeholder = 'Buscar por nome ou código...',
  disabled = false,
}: ColaboradorSearchFieldProps) {
  const [open, setOpen] = useState(false);
  const [busca, setBusca] = useState('');
  const [list, setList] = useState<ColaboradorCch[]>([]);
  const [loading, setLoading] = useState(false);
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const useCaseRef = useRef<ListarColaboradoresOrgUseCase | null>(null);
  const requestIdRef = useRef(0);
  if (!useCaseRef.current) {
    useCaseRef.current = container.resolve(ListarColaboradoresOrgUseCase);
  }

  const fetchColaboradores = useCallback(
    (search: string) => {
      if (!token) {
        setList([]);
        return;
      }
      const uc = useCaseRef.current;
      if (!uc) return;
      const id = ++requestIdRef.current;
      setLoading(true);
      uc.execute(token, { busca: search.trim(), cursor: 0, limite: LIMITE_LISTA })
        .then((res) => {
          if (id !== requestIdRef.current) return;
          const items = res?.ColaboradoresCchResult ?? [];
          setList(Array.isArray(items) ? items : []);
        })
        .catch(() => {
          if (id !== requestIdRef.current) return;
          setList([]);
        })
        .finally(() => {
          if (id !== requestIdRef.current) return;
          setLoading(false);
        });
    },
    [token]
  );

  useEffect(() => {
    if (debounceRef.current) clearTimeout(debounceRef.current);
    if (!open) return;
    debounceRef.current = setTimeout(() => {
      fetchColaboradores(busca);
    }, DEBOUNCE_MS);
    return () => {
      if (debounceRef.current) clearTimeout(debounceRef.current);
    };
  }, [busca, open, fetchColaboradores]);

  const displayValue = valueDisplayName?.trim() || valueCodigo || '';
  const triggerValue = open ? busca : displayValue;

  const handleOpenChange = (next: boolean) => {
    setOpen(next);
    if (!next) setBusca('');
  };

  return (
    <div className="space-y-2">
      <Label>{label}</Label>
      <Popover open={open} onOpenChange={handleOpenChange}>
        <PopoverTrigger asChild>
          <div className="relative flex">
            <Input
              role="combobox"
              aria-expanded={open}
              disabled={disabled}
              value={triggerValue}
              onChange={(e) => {
                setBusca(e.target.value);
                if (!open) setOpen(true);
              }}
              onFocus={() => setOpen(true)}
              placeholder={placeholder}
              className="pr-9"
            />
            <ChevronDown className="absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 shrink-0 opacity-50 pointer-events-none" />
          </div>
        </PopoverTrigger>
        <PopoverContent
          className="w-[var(--radix-popover-trigger-width)] p-0"
          align="start"
          onOpenAutoFocus={(e) => e.preventDefault()}
        >
          <Command shouldFilter={false}>
            <CommandInput
              placeholder="Buscar por nome ou código..."
              value={busca}
              onValueChange={setBusca}
            />
            <CommandList>
              {loading && (
                <div className="py-2 px-2 text-center text-xs text-muted-foreground border-b">
                  <span className="inline-flex items-center gap-2">
                    <Spinner size={12} />
                    <span>Carregando...</span>
                  </span>
                </div>
              )}
              <CommandEmpty>Nenhum colaborador encontrado.</CommandEmpty>
              <CommandGroup>
                {list.map((c) => {
                  const nome = c.nm_Profissional?.trim() || c.cd_Profissional || c.codigoColaboradorInterno || '';
                  return (
                    <CommandItem
                      key={c.codigoColaboradorInterno}
                      value={`${c.codigoColaboradorInterno}-${nome}`}
                      onSelect={() => {
                        onSelect(c.codigoColaboradorInterno, nome);
                        setOpen(false);
                        setBusca('');
                      }}
                    >
                      {nome}
                    </CommandItem>
                  );
                })}
              </CommandGroup>
            </CommandList>
          </Command>
        </PopoverContent>
      </Popover>
    </div>
  );
}

export const ColaboradorSearchField = memo(ColaboradorSearchFieldInner);
