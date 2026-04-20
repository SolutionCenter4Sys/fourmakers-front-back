import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Switch } from '@/components/ui/switch';
import { Badge } from '@/components/ui/badge';
import { Checkbox } from '@/components/ui/checkbox';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { Check, ChevronsUpDown, X } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import type { Grupo, ColaboradorSelecionado, FuncionalidadeSelecionada } from '@presentation/hooks/useGrupos';
import type { Colaborador } from '@domain/entities/Colaborador';
import type { Funcionalidade } from '@presentation/hooks/useParametrosFuncionalidades';
import type { EscopoType, ClienteSelecionado } from '@presentation/hooks/usePermissionamento';

interface CriarGrupoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  grupo?: Grupo | null;
  colaboradores: Colaborador[];
  colaboradoresLoading: boolean;
  colaboradoresSearch: string;
  setColaboradoresSearch: (search: string) => void;
  loadColaboradores: (search?: string) => void;
  funcionalidades: Funcionalidade[];
  clients: { id: string; name: string }[];
  clientSearch: string;
  setClientSearch: (search: string) => void;
  clientsLoading: boolean;
  setShouldLoadClients: (load: boolean) => void;
  onSave: (grupo: Omit<Grupo, 'id' | 'createdAt' | 'updatedAt'>) => void;
}

const escopoOptions = [
  { value: 'TODAS_ORGS_CLIENTES' as EscopoType, label: 'Todos os Clientes' },
  { value: 'CLIENTES_ESPECIFICOS' as EscopoType, label: 'Clientes específicos' },
];

export function CriarGrupoModal({
  open,
  onOpenChange,
  grupo,
  colaboradores,
  colaboradoresLoading,
  colaboradoresSearch,
  setColaboradoresSearch,
  loadColaboradores,
  funcionalidades,
  clients,
  clientSearch,
  setClientSearch,
  clientsLoading,
  setShouldLoadClients,
  onSave,
}: CriarGrupoModalProps) {
  const [nome, setNome] = useState('');
  const [isAdmin, setIsAdmin] = useState(false);
  const [membrosSelecionados, setMembrosSelecionados] = useState<ColaboradorSelecionado[]>([]);
  const [escopo, setEscopo] = useState<EscopoType[]>([]);
  const [clientesSelecionados, setClientesSelecionados] = useState<ClienteSelecionado[]>([]);
  const [funcionalidadesSelecionadas, setFuncionalidadesSelecionadas] = useState<FuncionalidadeSelecionada[]>([]);
  const [isColaboradorPopoverOpen, setIsColaboradorPopoverOpen] = useState(false);
  const [isClientPopoverOpen, setIsClientPopoverOpen] = useState(false);

  // Carregar colaboradores quando o popover abrir
  useEffect(() => {
    if (isColaboradorPopoverOpen && colaboradoresSearch === '') {
      // Carregar lista completa ao abrir o popover
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
  }, [colaboradoresSearch]);

  // Quando isAdmin mudar, atualizar funcionalidades automaticamente
  useEffect(() => {
    if (isAdmin) {
      // Se for Admin, incluir todas as funcionalidades de todos os módulos
      const todasFuncionalidades: FuncionalidadeSelecionada[] = funcionalidades.map((func) => ({
        funcionalidadeId: func.id,
        moduloId: func.moduloId,
      }));
      setFuncionalidadesSelecionadas(todasFuncionalidades);
    } else {
      // Se não for Admin e estava editando um grupo Admin, limpar funcionalidades
      if (grupo?.isAdmin) {
        setFuncionalidadesSelecionadas([]);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAdmin]);

  useEffect(() => {
    if (open) {
      if (grupo) {
        setNome(grupo.nome);
        setIsAdmin(grupo.isAdmin);
        setMembrosSelecionados(grupo.membros);
        setEscopo(grupo.escopo || []);
        setClientesSelecionados(grupo.clientesSelecionados || []);
        // Se for Admin, não usar as funcionalidades salvas, mas sim todas as disponíveis
        if (grupo.isAdmin) {
          const todasFuncionalidades: FuncionalidadeSelecionada[] = funcionalidades.map((func) => ({
            funcionalidadeId: func.id,
            moduloId: func.moduloId,
          }));
          setFuncionalidadesSelecionadas(todasFuncionalidades);
        } else {
          setFuncionalidadesSelecionadas(grupo.funcionalidades);
        }
      } else {
        setNome('');
        setIsAdmin(false);
        setMembrosSelecionados([]);
        setEscopo([]);
        setClientesSelecionados([]);
        setFuncionalidadesSelecionadas([]);
      }
    }
  }, [open, grupo, funcionalidades]);

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

  const handleSelectColaborador = (colaborador: Colaborador) => {
    if (!membrosSelecionados.find((m) => m.cpf === colaborador.cpf)) {
      setMembrosSelecionados([
        ...membrosSelecionados,
        {
          cpf: colaborador.cpf,
          nome: colaborador.nome,
          email: colaborador.email,
          codColaborador: colaborador.codColaborador,
        },
      ]);
    }
    setIsColaboradorPopoverOpen(false);
    setColaboradoresSearch('');
  };

  const handleRemoveColaborador = (cpf: string) => {
    setMembrosSelecionados(membrosSelecionados.filter((m) => m.cpf !== cpf));
  };


  const handleSave = () => {
    if (!nome.trim()) {
      return;
    }

    // Validação: se selecionou "Clientes específicos", deve ter pelo menos 1 cliente
    if (escopo.includes('CLIENTES_ESPECIFICOS') && clientesSelecionados.length === 0) {
      return;
    }

    // Se for Admin, incluir todas as funcionalidades automaticamente
    const funcionalidadesParaSalvar = isAdmin
      ? funcionalidades.map((func) => ({
          funcionalidadeId: func.id,
          moduloId: func.moduloId,
        }))
      : funcionalidadesSelecionadas;

    onSave({
      nome: nome.trim(),
      isAdmin,
      membros: membrosSelecionados,
      perfisSelecionados: [], // Mantido para compatibilidade, mas não usado mais
      funcionalidades: funcionalidadesParaSalvar,
      parametros: [], // Parâmetros serão selecionados na lista de grupos
      escopo,
      clientesSelecionados,
    });

    setNome('');
    setIsAdmin(false);
    setMembrosSelecionados([]);
      setEscopo([]);
      setClientesSelecionados([]);
      setFuncionalidadesSelecionadas([]);
      setColaboradoresSearch('');
      setClientSearch('');
      onOpenChange(false);
  };

  const handleClose = (open: boolean) => {
    if (!open) {
      setNome('');
      setIsAdmin(false);
      setMembrosSelecionados([]);
      setEscopo([]);
      setClientesSelecionados([]);
      setFuncionalidadesSelecionadas([]);
      setColaboradoresSearch('');
      setClientSearch('');
    }
    onOpenChange(open);
  };

  const temClientesEspecificos = escopo.includes('CLIENTES_ESPECIFICOS');

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{grupo ? 'Editar Grupo' : 'Criar Grupo'}</DialogTitle>
          <DialogDescription>
            {grupo ? 'Edite as informações do grupo de acesso.' : 'Preencha os dados para criar um novo grupo de acesso.'}
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-6 py-4">
          <div className="space-y-2">
            <Label>
              Nome do Grupo * <span className="text-xs text-muted-foreground">(Ex: Recrutadores, Admin Org)</span>
            </Label>
            <Input
              placeholder="Nome do grupo"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
            />
          </div>

          <div className="flex items-center justify-between">
            <div className="space-y-0.5">
              <Label>Grupo Admin</Label>
              <p className="text-sm text-muted-foreground">
                Identifica se este é um grupo de administradores
              </p>
            </div>
            <Switch checked={isAdmin} onCheckedChange={setIsAdmin} />
          </div>

          <div className="space-y-2">
            <Label>Membros *</Label>
            <Popover 
              open={isColaboradorPopoverOpen} 
              onOpenChange={(open) => {
                setIsColaboradorPopoverOpen(open);
                if (open) {
                  setColaboradoresSearch('');
                  // Carregar colaboradores imediatamente ao abrir
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
                    {colaboradoresLoading ? 'Carregando...' : 'Buscar e selecionar colaboradores...'}
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
                          const isSelected = membrosSelecionados.some((m) => m.cpf === colaborador.cpf);
                          return (
                            <CommandItem
                              key={colaborador.cpf}
                              value={`${colaborador.nome} ${colaborador.email}`}
                              onSelect={() => handleSelectColaborador(colaborador)}
                              disabled={isSelected}
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

            {membrosSelecionados.length > 0 && (
              <div className="mt-2 space-y-2">
                <p className="text-sm text-muted-foreground">
                  Membros Selecionados ({membrosSelecionados.length})
                </p>
                <div className="flex flex-wrap gap-2">
                  {membrosSelecionados.map((membro) => (
                    <Badge key={membro.cpf} variant="secondary" className="flex items-center gap-1 pr-1">
                      <span>{membro.nome}</span>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-4 w-4 hover:bg-destructive hover:text-destructive-foreground"
                        onClick={() => handleRemoveColaborador(membro.cpf)}
                      >
                        <X className="h-3 w-3" />
                      </Button>
                    </Badge>
                  ))}
                </div>
              </div>
            )}
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

          {isAdmin && (
            <div className="space-y-2">
              <Label>Funcionalidades</Label>
              <p className="text-sm text-muted-foreground">
                Grupos Admin têm acesso a todos os módulos e funcionalidades do sistema ({funcionalidades.length} funcionalidades).
              </p>
            </div>
          )}
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => handleClose(false)}>
            Cancelar
          </Button>
          <Button 
            onClick={handleSave} 
            disabled={
              !nome.trim() || 
              membrosSelecionados.length === 0 || 
              escopo.length === 0 || 
              (temClientesEspecificos && clientesSelecionados.length === 0)
            }
          >
            {grupo ? 'Salvar' : 'Criar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
