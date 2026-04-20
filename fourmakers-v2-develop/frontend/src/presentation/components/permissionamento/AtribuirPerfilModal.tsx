import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { Check, ChevronsUpDown } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import type { Colaborador } from '@domain/entities/Colaborador';
import type { PerfilPermissionamento } from '@presentation/hooks/usePermissionamento';

interface AtribuirPerfilModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  colaboradores: Colaborador[];
  colaboradoresLoading: boolean;
  colaboradoresSearch: string;
  setColaboradoresSearch: (search: string) => void;
  loadColaboradores: (search?: string) => void;
  perfis: PerfilPermissionamento[];
  getEscopoText: (perfil: PerfilPermissionamento) => string;
  onSave: (colaboradorCpf: string, perfilId: string) => void;
}

export function AtribuirPerfilModal({
  open,
  onOpenChange,
  colaboradores,
  colaboradoresLoading,
  colaboradoresSearch,
  setColaboradoresSearch,
  loadColaboradores,
  perfis,
  getEscopoText,
  onSave,
}: AtribuirPerfilModalProps) {
  const [colaboradorSelecionado, setColaboradorSelecionado] = useState<Colaborador | null>(null);
  const [perfilSelecionado, setPerfilSelecionado] = useState('');
  const [isColaboradorPopoverOpen, setIsColaboradorPopoverOpen] = useState(false);

  // Carregar colaboradores quando o popover abrir
  useEffect(() => {
    if (isColaboradorPopoverOpen && colaboradoresSearch === '') {
      loadColaboradores('');
    }
  }, [isColaboradorPopoverOpen]);

  // Debounce para busca de colaboradores quando o usuário digitar
  useEffect(() => {
    if (!isColaboradorPopoverOpen || colaboradoresSearch === '') return;
    
    const timer = setTimeout(() => {
      loadColaboradores(colaboradoresSearch);
    }, 300);

    return () => clearTimeout(timer);
  }, [colaboradoresSearch, isColaboradorPopoverOpen]);

  useEffect(() => {
    if (!open) {
      setColaboradorSelecionado(null);
      setPerfilSelecionado('');
      setColaboradoresSearch('');
    }
  }, [open, setColaboradoresSearch]);

  const handleSave = () => {
    if (!colaboradorSelecionado || !perfilSelecionado) {
      return;
    }

    onSave(colaboradorSelecionado.cpf, perfilSelecionado);
    setColaboradorSelecionado(null);
    setPerfilSelecionado('');
    setColaboradoresSearch('');
    onOpenChange(false);
  };

  const handleClose = (open: boolean) => {
    if (!open) {
      setColaboradorSelecionado(null);
      setPerfilSelecionado('');
      setColaboradoresSearch('');
    }
    onOpenChange(open);
  };

  const perfisAtivos = perfis.filter((p) => p.status === 'Ativo');

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Atribuir Perfil</DialogTitle>
          <DialogDescription>
            Selecione um colaborador e um perfil para atribuir.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-6 py-4">
          <div className="space-y-2">
            <Label>Colaborador *</Label>
            <Popover 
              open={isColaboradorPopoverOpen} 
              onOpenChange={(open) => {
                setIsColaboradorPopoverOpen(open);
                if (open) {
                  setColaboradoresSearch('');
                  loadColaboradores('');
                }
              }}
            >
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  role="combobox"
                  className="w-full justify-between"
                >
                  <span className="text-muted-foreground">
                    {colaboradorSelecionado 
                      ? `${colaboradorSelecionado.nome} (${colaboradorSelecionado.email})`
                      : colaboradoresLoading 
                        ? 'Carregando...' 
                        : 'Buscar e selecionar colaborador...'}
                  </span>
                  <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="p-0 w-[var(--radix-popover-trigger-width)] max-w-[400px]">
                <Command>
                  <CommandInput
                    placeholder="Buscar colaborador..."
                    value={colaboradoresSearch}
                    onValueChange={setColaboradoresSearch}
                  />
                  <CommandList className="max-h-[300px]">
                    {colaboradoresLoading ? (
                      <div className="flex items-center justify-center p-4">
                        <Spinner size={16} className="text-current" />
                      </div>
                    ) : colaboradores.length === 0 ? (
                      <CommandEmpty>
                        {colaboradoresSearch ? 'Nenhum colaborador encontrado' : 'Digite para buscar colaboradores'}
                      </CommandEmpty>
                    ) : (
                      <CommandGroup>
                        {colaboradores.map((colaborador) => {
                          const isSelected = colaboradorSelecionado?.cpf === colaborador.cpf;
                          return (
                            <CommandItem
                              key={colaborador.cpf}
                              value={`${colaborador.nome} ${colaborador.email}`}
                              onSelect={() => {
                                setColaboradorSelecionado(colaborador);
                                setIsColaboradorPopoverOpen(false);
                                setColaboradoresSearch('');
                              }}
                            >
                              <Check
                                className={`mr-2 h-4 w-4 ${isSelected ? 'opacity-100' : 'opacity-0'}`}
                              />
                              <div className="flex flex-col">
                                <span>{colaborador.nome}</span>
                                <span className="email-text text-xs text-muted-foreground">
                                  {colaborador.email}
                                </span>
                              </div>
                            </CommandItem>
                          );
                        })}
                      </CommandGroup>
                    )}
                  </CommandList>
                </Command>
              </PopoverContent>
            </Popover>
          </div>

          <div className="space-y-2">
            <Label>Perfil *</Label>
            <Select value={perfilSelecionado} onValueChange={setPerfilSelecionado}>
              <SelectTrigger>
                <SelectValue placeholder="Selecione um perfil" />
              </SelectTrigger>
              <SelectContent>
                {perfisAtivos.map((perfil) => (
                  <SelectItem key={perfil.id} value={perfil.id}>
                    {perfil.nome}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {perfilSelecionado && (
              <div className="mt-2 p-3 bg-muted rounded-lg">
                <p className="text-sm font-medium mb-1">Escopo do Perfil:</p>
                <p className="text-sm text-muted-foreground">
                  {getEscopoText(perfisAtivos.find((p) => p.id === perfilSelecionado)!)}
                </p>
              </div>
            )}
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => handleClose(false)}>
            Cancelar
          </Button>
          <Button 
            onClick={handleSave} 
            disabled={!colaboradorSelecionado || !perfilSelecionado}
          >
            Atribuir
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
