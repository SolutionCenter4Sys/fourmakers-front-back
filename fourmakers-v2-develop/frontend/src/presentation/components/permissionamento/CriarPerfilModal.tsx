import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { Badge } from '@/components/ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { Check, ChevronsUpDown, X } from '@/components/ui/system-icons';
import type { PerfilPermissionamento, EscopoType, ClienteSelecionado } from '@presentation/hooks/usePermissionamento';

interface CriarPerfilModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  perfil?: PerfilPermissionamento | null;
  clients: { id: string; name: string }[];
  clientSearch: string;
  setClientSearch: (search: string) => void;
  clientsLoading: boolean;
  setShouldLoadClients: (load: boolean) => void;
  onSave: (perfil: Omit<PerfilPermissionamento, 'id' | 'createdAt' | 'updatedAt'>) => void;
}

const escopoOptions = [
  { value: 'TODAS_ORGS_CLIENTES' as EscopoType, label: 'Todas as Orgs e Clientes' },
  { value: 'ORG_CLIENTES' as EscopoType, label: 'Somente Org e Clientes' },
  { value: 'CLIENTES_ORG' as EscopoType, label: 'Clientes da Org' },
  { value: 'CLIENTES_ESPECIFICOS' as EscopoType, label: 'Clientes específicos' },
];

export function CriarPerfilModal({
  open,
  onOpenChange,
  perfil,
  clients,
  clientSearch,
  setClientSearch,
  clientsLoading,
  setShouldLoadClients,
  onSave,
}: CriarPerfilModalProps) {
  const [nome, setNome] = useState('');
  const [escopo, setEscopo] = useState<EscopoType[]>([]);
  const [status, setStatus] = useState<'Ativo' | 'Inativo'>('Ativo');
  const [clientesSelecionados, setClientesSelecionados] = useState<ClienteSelecionado[]>([]);
  const [isClientPopoverOpen, setIsClientPopoverOpen] = useState(false);
  const [descricao, setDescricao] = useState('');

  // Carregar dados do perfil quando abrir para edição
  useEffect(() => {
    if (open) {
      if (perfil) {
        setNome(perfil.nome);
        setEscopo(perfil.escopo);
        setStatus(perfil.status);
        setClientesSelecionados(perfil.clientesSelecionados || []);
        setDescricao(perfil.descricao || '');
      } else {
        // Reset para novo perfil
        setNome('');
        setEscopo([]);
        setStatus('Ativo');
        setClientesSelecionados([]);
        setDescricao('');
      }
    }
  }, [open, perfil]);

  const handleEscopoChange = (value: EscopoType, checked: boolean) => {
    if (checked) {
      // Seleção única: ao marcar uma opção, desmarca todas as outras
      setEscopo([value]);
      
      // Se não for "Clientes específicos", limpa a lista de clientes
      if (value !== 'CLIENTES_ESPECIFICOS') {
        setClientesSelecionados([]);
      }
    } else {
      // Se desmarcar, limpa o escopo
      setEscopo([]);
      // Se desmarcar "Clientes específicos", limpa a lista
      if (value === 'CLIENTES_ESPECIFICOS') {
        setClientesSelecionados([]);
      }
    }
  };

  const handleSelectClient = (client: { id: string; name: string }) => {
    // Verifica se já está selecionado
    if (clientesSelecionados.some((c) => c.id === client.id)) {
      return;
    }

    setClientesSelecionados((prev) => [
      ...prev,
      { id: client.id, nome: client.name },
    ]);
    setClientSearch('');
    setIsClientPopoverOpen(false);
  };

  const handleRemoveClient = (clientId: string) => {
    setClientesSelecionados((prev) => prev.filter((c) => c.id !== clientId));
  };

  const handleSave = () => {
    if (!nome.trim()) {
      return;
    }

    // Validação: se selecionou "Clientes específicos", deve ter pelo menos 1 cliente
    if (escopo.includes('CLIENTES_ESPECIFICOS') && clientesSelecionados.length === 0) {
      return;
    }

    onSave({
      nome: nome.trim(),
      escopo,
      clientesSelecionados,
      status,
      descricao: descricao.trim() || undefined,
    });

    // Reset ao fechar
    setNome('');
    setEscopo([]);
    setStatus('Ativo');
    setClientesSelecionados([]);
    setDescricao('');
    setClientSearch('');
    onOpenChange(false);
  };

  const handleClose = (open: boolean) => {
    if (!open) {
      // Reset ao fechar sem salvar
      setNome('');
      setEscopo([]);
      setStatus('Ativo');
      setClientesSelecionados([]);
      setDescricao('');
      setClientSearch('');
    }
    onOpenChange(open);
  };

  const temClientesEspecificos = escopo.includes('CLIENTES_ESPECIFICOS');

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{perfil ? 'Editar Perfil' : 'Criar Perfil'}</DialogTitle>
          <DialogDescription>
            {perfil ? 'Edite as informações do perfil de permissionamento.' : 'Preencha os dados para criar um novo perfil de permissionamento.'}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6 py-4">
          <div className="space-y-2">
            <Label htmlFor="nome">Nome do Perfil *</Label>
            <Input
              id="nome"
              placeholder="Ex: Recrutador Senior"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label>Descrição</Label>
            <Input
              placeholder="Descreva o perfil..."
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
            />
          </div>

          <div className="space-y-4">
            <Label>Escopo *</Label>
            <div className="space-y-3">
              {escopoOptions.map((option) => (
                <div key={option.value} className="flex items-center space-x-2">
                  <Checkbox
                    id={option.value}
                    checked={escopo.includes(option.value)}
                    onCheckedChange={(checked) =>
                      handleEscopoChange(option.value, checked as boolean)
                    }
                  />
                  <Label
                    htmlFor={option.value}
                    className="text-sm font-normal cursor-pointer"
                  >
                    {option.label}
                  </Label>
                </div>
              ))}
            </div>

            {temClientesEspecificos && (
              <div className="mt-4 space-y-3 pl-6 border-l-2 border-borderDefault">
                <Label>Selecionar Clientes</Label>
                <Popover 
                  open={isClientPopoverOpen} 
                  onOpenChange={(open) => {
                    setIsClientPopoverOpen(open);
                    setShouldLoadClients(open);
                    if (open) {
                      // Ao abrir, reseta a busca para carregar lista completa
                      setClientSearch('');
                    } else {
                      // Ao fechar, limpa a busca
                      setClientSearch('');
                    }
                  }}
                >
                  <PopoverTrigger asChild>
                    <Button variant="outline" className="w-full justify-between text-sm font-normal">
                      {clientSearch || 'Buscar e selecionar clientes...'}
                      <ChevronsUpDown className="h-4 w-4 opacity-50" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent 
                    className="p-0 w-[320px]" 
                    side="bottom" 
                    align="start"
                    sideOffset={4}
                    avoidCollisions={false}
                    onOpenAutoFocus={(e) => e.preventDefault()}
                  >
                    <Command shouldFilter={false} className="overflow-visible">
                      <CommandInput
                        placeholder="Buscar cliente..."
                        value={clientSearch}
                        onValueChange={setClientSearch}
                        className="text-sm"
                      />
                      <div className="max-h-[300px] overflow-y-auto overflow-x-hidden">
                        <CommandList>
                        {clientsLoading ? (
                          <CommandEmpty>Carregando lista de clientes...</CommandEmpty>
                        ) : clients.length === 0 ? (
                          <CommandEmpty>
                            {clientSearch.trim().length > 0 ? 'Nenhum cliente encontrado' : 'Nenhum cliente disponível'}
                          </CommandEmpty>
                        ) : (
                          <CommandGroup>
                            {clients.map((client) => {
                              const isSelected = clientesSelecionados.some((c) => c.id === client.id);
                              return (
                                <CommandItem
                                  key={client.id}
                                  value={client.name}
                                  onSelect={() => handleSelectClient(client)}
                                  disabled={isSelected}
                                  className="text-sm"
                                >
                                  <Check
                                    className={`mr-2 h-4 w-4 ${
                                      isSelected ? 'opacity-100' : 'opacity-0'
                                    }`}
                                  />
                                  {client.name}
                                </CommandItem>
                              );
                            })}
                          </CommandGroup>
                        )}
                        </CommandList>
                      </div>
                    </Command>
                  </PopoverContent>
                </Popover>

                {clientesSelecionados.length > 0 && (
                  <div className="space-y-2">
                    <Label className="text-sm">Clientes Selecionados ({clientesSelecionados.length})</Label>
                    <div className="flex flex-wrap gap-2">
                      {clientesSelecionados.map((cliente) => (
                        <Badge
                          key={cliente.id}
                          variant="secondary"
                          className="flex items-center gap-2 pr-1"
                        >
                          {cliente.nome}
                          <button
                            type="button"
                            onClick={() => handleRemoveClient(cliente.id)}
                            className="text-muted-foreground hover:text-destructive"
                            aria-label={`Remover ${cliente.nome}`}
                          >
                            <X className="h-3 w-3" />
                          </button>
                        </Badge>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            )}
          </div>

          <div className="space-y-2">
            <Label>Status</Label>
            <Select value={status} onValueChange={(value) => setStatus(value as 'Ativo' | 'Inativo')}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Ativo">Ativo</SelectItem>
                <SelectItem value="Inativo">Inativo</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button
            onClick={handleSave}
            disabled={!nome.trim() || escopo.length === 0 || (temClientesEspecificos && clientesSelecionados.length === 0)}
          >
            {perfil ? 'Salvar' : 'Criar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
