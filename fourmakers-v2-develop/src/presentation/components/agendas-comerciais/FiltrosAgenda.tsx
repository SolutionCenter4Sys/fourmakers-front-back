import { Button } from '@/components/ui/button'
import { Calendar } from '@/components/ui/calendar'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Checkbox } from '@/components/ui/checkbox'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { ScrollArea } from '@/components/ui/scroll-area'
import { Spinner } from '@/components/ui/spinner'
import { Building2, CalendarIcon, Search, SlidersHorizontal, Users, X } from '@/components/ui/system-icons'
import { cn } from '@/lib/utils'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { fetchColaboradores } from '@app/store/slices/colaboradoresSlice'
import { container } from '@core/di/container'
import { ListarClientesUseCase } from '@domain/usecases/ListarClientesUseCase'
import { useLimparBuscaAoFecharPopover } from '@shared/hooks/useLimparBuscaAoFecharPopover'
import { useGestoresExternos } from '@presentation/hooks/useGestoresExternos'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { useEffect, useState } from 'react'

import type { Colaborador } from '@domain/entities/Colaborador'
import { obterFiltrosIniciais, type FiltrosAgenda } from '@presentation/hooks/useAgendasComerciais'

interface FiltrosAgendaProps {
  filtros: FiltrosAgenda
  onFiltroChange: (filtros: Partial<FiltrosAgenda>) => void
}

const tipoInteracaoOptions = ['Reunião', 'Ligação', 'Chat', 'Email', 'Presencial']

/**
 * Componente de sidebar com filtros para agendas comerciais.
 * Permite filtrar por período (data início/fim), minhas agendas,
 * tipo de interação e colaboradores usando multi-selects.
 */
export function FiltrosAgenda({ filtros, onFiltroChange }: FiltrosAgendaProps) {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const { colaboradores, status: colaboradoresStatus } = useAppSelector((state) => state.colaboradores)
  

  const [tipoPopoverOpen, setTipoPopoverOpen] = useState(false)
  const [colaboradoresPopoverOpen, setColaboradoresPopoverOpen] = useState(false)
  const [clientesPopoverOpen, setClientesPopoverOpen] = useState(false)
  const [gestoresPopoverOpen, setGestoresPopoverOpen] = useState(false)

  const [colaboradorSearch, setColaboradorSearch] = useState('')
  const [colaboradorSearchDebounced, setColaboradorSearchDebounced] = useState('')
  const [clienteSearch, setClienteSearch] = useState('')
  const [gestorSearch, setGestorSearch] = useState('')

  const [clientes, setClientes] = useState<Array<{ id: string; name: string }>>([])
  const [clientesLoading, setClientesLoading] = useState(false)

  const [colaboradoresSelecionados, setColaboradoresSelecionados] = useState<Colaborador[]>([])
  const [clientesSelecionados, setClientesSelecionados] = useState<Array<{ id: string; name: string }>>([])
  const [gestoresSelecionados, setGestoresSelecionados] = useState<Array<{ codGestorExterno: string; nome: string; email: string }>>([])

  const orgId = user?.colaboradorOrg?.orgId || 0

  // Hook para buscar gestores externos quando cliente for selecionado
  const { gestores, loading: gestoresLoading } = useGestoresExternos({
    token,
    codigoCliente: clientesSelecionados.length > 0 ? clientesSelecionados[0].id : null,
    busca: gestorSearch,
    enabled: clientesSelecionados.length > 0,
  })

  useLimparBuscaAoFecharPopover(
    colaboradoresPopoverOpen,
    setColaboradorSearch,
    setColaboradorSearchDebounced,
  )

  // Debounce do termo de busca de colaborador (300 ms) para evitar requisição a cada tecla
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      setColaboradorSearchDebounced(colaboradorSearch)
    }, 300)
    return () => clearTimeout(timeoutId)
  }, [colaboradorSearch])

  // Buscar colaboradores quando o popover abrir ou o termo com debounce mudar
  useEffect(() => {
    if (colaboradoresPopoverOpen && token && orgId > 0) {
      dispatch(
        fetchColaboradores({
          token,
          orgId,
          cursor: 0,
          limite: 1000,
          nomeOuEmail: colaboradorSearchDebounced,
        }),
      )
    }
  }, [colaboradoresPopoverOpen, token, orgId, dispatch, colaboradorSearchDebounced])

  // Buscar clientes quando o popover abrir
  useEffect(() => {
    const loadClients = async () => {
      if (!token || !clientesPopoverOpen) {
        if (!clientesPopoverOpen) {
          setClientes([])
        }
        return
      }

      setClientesLoading(true)
      try {
        const useCase = container.resolve(ListarClientesUseCase)
        const data = await useCase.execute(token, clienteSearch.trim())
        // ClienteListItem já retorna { id, name }, não precisa mapear
        setClientes(data)
      } catch (error) {
        console.error('Erro ao buscar clientes:', error)
        setClientes([])
      } finally {
        setClientesLoading(false)
      }
    }

    const delay = clientesPopoverOpen && !clienteSearch.trim() ? 0 : 300
    const timeoutId = setTimeout(() => {
      void loadClients()
    }, delay)

    return () => clearTimeout(timeoutId)
  }, [token, clienteSearch, clientesPopoverOpen])

  useLimparBuscaAoFecharPopover(
    clientesPopoverOpen,
    setClienteSearch,
    undefined,
    { delay: 100, soLimparSePreenchido: true },
    clienteSearch,
  )

  useLimparBuscaAoFecharPopover(gestoresPopoverOpen, setGestorSearch)

  const toggleTipoInteracao = (tipo: string) => {
    const newTipos = filtros.tipoInteracao.includes(tipo)
      ? filtros.tipoInteracao.filter((t) => t !== tipo)
      : [...filtros.tipoInteracao, tipo]
    onFiltroChange({ tipoInteracao: newTipos })
  }

  const handleSelectColaborador = (colaborador: Colaborador) => {
    const newSelecionados = [...colaboradoresSelecionados, colaborador]
    setColaboradoresSelecionados(newSelecionados)
    onFiltroChange({
      colaboradores: newSelecionados.map((c) => c.cpf ?? c.codColaborador ?? ''),
    })
  }

  const handleRemoveColaborador = (cpfOuCod: string) => {
    const newSelecionados = colaboradoresSelecionados.filter(
      (c) => (c.cpf ?? c.codColaborador ?? '') !== cpfOuCod,
    )
    setColaboradoresSelecionados(newSelecionados)
    onFiltroChange({ colaboradores: newSelecionados.map((c) => c.cpf ?? c.codColaborador ?? '') })
  }

  const handleSelectCliente = (cliente: { id: string; name: string }) => {
    const newSelecionados = [...clientesSelecionados, cliente]
    setClientesSelecionados(newSelecionados)
    onFiltroChange({ clientes: newSelecionados.map(c => c.id) })
  }

  const handleRemoveCliente = (id: string) => {
    const newSelecionados = clientesSelecionados.filter((c) => c.id !== id)
    setClientesSelecionados(newSelecionados)
    onFiltroChange({ clientes: newSelecionados.map(c => c.id) })
    // Limpar gestores se cliente for removido
    if (newSelecionados.length === 0) {
      setGestoresSelecionados([])
      onFiltroChange({ gestores: [] })
    }
  }

  const handleSelectGestor = (gestor: { codGestorExterno: string; nome: string; email: string }) => {
    const newSelecionados = [...gestoresSelecionados, gestor]
    setGestoresSelecionados(newSelecionados)
    onFiltroChange({ gestores: newSelecionados.map(g => g.codGestorExterno) })
  }

  const handleRemoveGestor = (codGestorExterno: string) => {
    const newSelecionados = gestoresSelecionados.filter((g) => g.codGestorExterno !== codGestorExterno)
    setGestoresSelecionados(newSelecionados)
    onFiltroChange({ gestores: newSelecionados.map(g => g.codGestorExterno) })
  }

  const limparFiltros = () => {
    setColaboradoresSelecionados([])
    setClientesSelecionados([])
    setGestoresSelecionados([])
    const padrao = obterFiltrosIniciais()
    onFiltroChange({
      dataInicio: padrao.dataInicio,
      dataFim: padrao.dataFim,
      colaboradores: [],
      clientes: [],
      gestores: [],
      tipoInteracao: [],
      minhasAgendas: false,
      apenasConvitesPendentes: false,
    })
  }

  const colaboradoresFiltrados = colaboradorSearch.trim()
    ? colaboradores.filter(
      (colab) => {
        const termo = colaboradorSearch.toLowerCase()
        const nome = (colab.nome ?? '').toLowerCase()
        const email = (colab.email ?? '').toLowerCase()
        const cpf = (colab.cpf ?? '').toLowerCase()
        return nome.includes(termo) || email.includes(termo) || cpf.includes(termo)
      },
    )
    : colaboradores

  return (
    <Card className="w-80 h-fit">
      <CardHeader>
        <CardTitle>Filtros</CardTitle>
        <CardDescription>Refine sua busca de agendas</CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        {/* Período - Data Início */}
        <div className="space-y-2">
          <Label>Data Início</Label>
          <Popover>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={cn(
                  'w-full justify-start text-left font-normal',
                  !filtros.dataInicio && 'text-muted-foreground',
                )}
              >
                <CalendarIcon className="mr-2 h-4 w-4" />
                {filtros.dataInicio ? (
                  format(filtros.dataInicio, 'PPP', { locale: ptBR })
                ) : (
                  <span>Selecione</span>
                )}
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={filtros.dataInicio || undefined}
                onSelect={(date) => onFiltroChange({ dataInicio: date || null })}
                initialFocus
              />
            </PopoverContent>
          </Popover>
        </div>

        {/* Período - Data Fim */}
        <div className="space-y-2">
          <Label>Data Fim</Label>
          <Popover>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={cn(
                  'w-full justify-start text-left font-normal',
                  !filtros.dataFim && 'text-muted-foreground',
                )}
              >
                <CalendarIcon className="mr-2 h-4 w-4" />
                {filtros.dataFim ? (
                  format(filtros.dataFim, 'PPP', { locale: ptBR })
                ) : (
                  <span>Selecione</span>
                )}
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="single"
                selected={filtros.dataFim || undefined}
                onSelect={(date) => onFiltroChange({ dataFim: date || null })}
                initialFocus
              />
            </PopoverContent>
          </Popover>
        </div>

        {/* Checkbox Minhas Agendas */}
        <div className="flex items-center space-x-2">
          <Checkbox
            id="minhas-agendas"
            checked={filtros.minhasAgendas}
            onCheckedChange={(checked) => onFiltroChange({ minhasAgendas: !!checked })}
          />
          <Label htmlFor="minhas-agendas" className="cursor-pointer">
            Apenas Minhas Agendas
          </Label>
        </div>

        {/* Checkbox Convites Pendentes */}
        <div className="flex items-center space-x-2">
          <Checkbox
            id="convites-pendentes"
            checked={filtros.apenasConvitesPendentes}
            onCheckedChange={(checked) => onFiltroChange({ apenasConvitesPendentes: !!checked })}
          />
          <Label htmlFor="convites-pendentes" className="cursor-pointer">
            Apenas Convites Pendentes
          </Label>
        </div>

        {/* Multi-select Tipo de Interação */}
        <div className="space-y-2">
          <Label>Tipo de Interação</Label>
          <Popover open={tipoPopoverOpen} onOpenChange={setTipoPopoverOpen}>
            <PopoverTrigger asChild>
              <button
                type="button"
                className="flex h-10 w-full items-center justify-between rounded-lg border border-border bg-input px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 dark:border-input"
              >
                <span className="truncate flex-1 text-left">
                  {filtros.tipoInteracao.length > 0
                    ? `${filtros.tipoInteracao.length} selecionado(s)`
                    : 'Selecionar tipos'}
                </span>
                <SlidersHorizontal className="h-4 w-4 ml-2 shrink-0 opacity-50" />
              </button>
            </PopoverTrigger>
            <PopoverContent className="w-64" align="start">
              <div className="space-y-2">
                {tipoInteracaoOptions.map((tipo) => (
                  <div key={tipo} className="flex items-center space-x-2">
                    <Checkbox
                      id={`tipo-${tipo}`}
                      checked={filtros.tipoInteracao.includes(tipo)}
                      onCheckedChange={() => toggleTipoInteracao(tipo)}
                    />
                    <label htmlFor={`tipo-${tipo}`} className="text-sm cursor-pointer flex-1 leading-none">
                      {tipo}
                    </label>
                  </div>
                ))}
              </div>
            </PopoverContent>
          </Popover>
        </div>

        {/* Multi-select Colaboradores */}
        <div className="space-y-2">
          <Label>Colaboradores</Label>
          <Popover open={colaboradoresPopoverOpen} onOpenChange={setColaboradoresPopoverOpen}>
            <PopoverTrigger asChild>
              <button
                type="button"
                className="flex h-10 w-full items-center justify-between rounded-lg border border-border bg-input px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 dark:border-input"
              >
                <span className="truncate flex-1 text-left">
                  {colaboradoresSelecionados.length > 0
                    ? `${colaboradoresSelecionados.length} selecionado(s)`
                    : 'Selecionar colaboradores'}
                </span>
                <Users className="h-4 w-4 ml-2 shrink-0 opacity-50" />
              </button>
            </PopoverTrigger>
            <PopoverContent className="w-80 p-0" align="start">
              <div className="flex items-center border-b px-3">
                <Search className="mr-2 h-4 w-4 shrink-0 opacity-50" />
                <Input
                  placeholder="Buscar colaborador..."
                  value={colaboradorSearch}
                  onChange={(e) => setColaboradorSearch(e.target.value)}
                  className="border-0 focus-visible:ring-0"
                />
              </div>
              <ScrollArea className="h-[200px]">
                <div className="p-2">
                  {colaboradoresStatus === 'loading' ? (
                    <div className="flex items-center justify-center py-8">
                      <Spinner size={16} className="text-primary" />
                      <span className="ml-2 text-sm text-muted-foreground">Carregando...</span>
                    </div>
                  ) : colaboradoresFiltrados.length === 0 ? (
                    <div className="py-8 text-center text-sm text-muted-foreground">
                      Nenhum colaborador encontrado
                    </div>
                  ) : (
                    colaboradoresFiltrados.map((colaborador, index) => {
                      const idColab = colaborador.cpf ?? colaborador.codColaborador ?? `colab-${index}`
                      const isSelected = colaboradoresSelecionados.some(
                        (c) => (c.cpf ?? c.codColaborador) === (colaborador.cpf ?? colaborador.codColaborador),
                      )
                      return (
                        <div
                          key={idColab}
                          className="flex items-center space-x-2 rounded-lg p-2 hover:bg-accent cursor-pointer"
                          onClick={() => {
                            if (!isSelected) {
                              handleSelectColaborador(colaborador)
                            }
                          }}
                        >
                          <Checkbox checked={isSelected} />
                          <div className="flex-1 min-w-0">
                            <div className="text-sm font-medium truncate">{colaborador.nome ?? ''}</div>
                            <div className="text-xs text-muted-foreground truncate">{colaborador.email ?? ''}</div>
                          </div>
                        </div>
                      )
                    })
                  )}
                </div>
              </ScrollArea>
            </PopoverContent>
          </Popover>
          {colaboradoresSelecionados.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {colaboradoresSelecionados.map((colaborador, index) => {
                const idColab = colaborador.cpf ?? colaborador.codColaborador ?? `colab-sel-${index}`
                const idParaRemover = colaborador.cpf ?? colaborador.codColaborador ?? ''
                return (
                  <div
                    key={idColab}
                    className="flex items-center gap-1 px-2 py-1 bg-secondary rounded-md text-xs"
                  >
                    <span className="truncate max-w-[150px]">{colaborador.nome ?? ''}</span>
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      className="h-4 w-4 p-0 hover:bg-destructive hover:text-destructive-foreground"
                      onClick={() => handleRemoveColaborador(idParaRemover)}
                    >
                      <X className="h-3 w-3" />
                    </Button>
                  </div>
                )
              })}
            </div>
          )}
        </div>

        {/* Multi-select Clientes */}
        <div className="space-y-2">
          <Label>Clientes</Label>
          <Popover open={clientesPopoverOpen} onOpenChange={setClientesPopoverOpen}>
            <PopoverTrigger asChild>
              <button
                type="button"
                className="flex h-10 w-full items-center justify-between rounded-lg border border-border bg-input px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 dark:border-input"
              >
                <span className="truncate flex-1 text-left">
                  {clientesSelecionados.length > 0
                    ? `${clientesSelecionados.length} selecionado(s)`
                    : 'Selecionar clientes'}
                </span>
                <Building2 className="h-4 w-4 ml-2 shrink-0 opacity-50" />
              </button>
            </PopoverTrigger>
            <PopoverContent className="w-80 p-0" align="start">
              <div className="flex items-center border-b px-3">
                <Search className="mr-2 h-4 w-4 shrink-0 opacity-50" />
                <Input
                  placeholder="Buscar cliente..."
                  value={clienteSearch}
                  onChange={(e) => setClienteSearch(e.target.value)}
                  className="border-0 focus-visible:ring-0"
                />
              </div>
              <ScrollArea className="h-[200px]">
                <div className="p-2">
                  {clientesLoading ? (
                    <div className="flex items-center justify-center py-8">
                      <Spinner size={16} className="text-primary" />
                      <span className="ml-2 text-sm text-muted-foreground">Carregando...</span>
                    </div>
                  ) : clientes.length === 0 ? (
                    <div className="py-8 text-center text-sm text-muted-foreground">
                      {clienteSearch.trim() ? 'Nenhum cliente encontrado' : 'Digite para buscar clientes'}
                    </div>
                  ) : (
                    clientes.map((cliente) => {
                      const isSelected = clientesSelecionados.some((c) => c.id === cliente.id)
                      return (
                        <div
                          key={cliente.id}
                          className="flex items-center space-x-2 rounded-lg p-2 hover:bg-accent cursor-pointer"
                          onClick={() => {
                            if (!isSelected) {
                              handleSelectCliente(cliente)
                            }
                          }}
                        >
                          <Checkbox checked={isSelected} />
                          <div className="flex-1 min-w-0">
                            <div className="text-sm font-medium truncate">{cliente.name}</div>
                            <div className="text-xs text-muted-foreground truncate">{cliente.id}</div>
                          </div>
                        </div>
                      )
                    })
                  )}
                </div>
              </ScrollArea>
            </PopoverContent>
          </Popover>
          {clientesSelecionados.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {clientesSelecionados.map((cliente) => (
                <div
                  key={cliente.id}
                  className="flex items-center gap-1 px-2 py-1 bg-secondary rounded-md text-xs"
                >
                  <span className="truncate max-w-[150px]">{cliente.name}</span>
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    className="h-4 w-4 p-0 hover:bg-destructive hover:text-destructive-foreground"
                    onClick={() => handleRemoveCliente(cliente.id)}
                  >
                    <X className="h-3 w-3" />
                  </Button>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Multi-select Gestores Externos */}
        {clientesSelecionados.length > 0 && (
          <div className="space-y-2">
            <Label>Gestores Externos</Label>
            <Popover open={gestoresPopoverOpen} onOpenChange={setGestoresPopoverOpen}>
              <PopoverTrigger asChild>
                <button
                  type="button"
                  className="flex h-10 w-full items-center justify-between rounded-lg border border-border bg-input px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 dark:border-input"
                >
                  <span className="truncate flex-1 text-left">
                    {gestoresSelecionados.length > 0
                      ? `${gestoresSelecionados.length} selecionado(s)`
                      : 'Selecionar gestores'}
                  </span>
                  <Users className="h-4 w-4 ml-2 shrink-0 opacity-50" />
                </button>
              </PopoverTrigger>
              <PopoverContent className="w-80 p-0" align="start">
                <div className="flex items-center border-b px-3">
                  <Search className="mr-2 h-4 w-4 shrink-0 opacity-50" />
                  <Input
                    placeholder="Buscar gestor..."
                    value={gestorSearch}
                    onChange={(e) => setGestorSearch(e.target.value)}
                    className="border-0 focus-visible:ring-0"
                  />
                </div>
                <ScrollArea className="h-[200px]">
                  <div className="p-2">
                    {gestoresLoading ? (
                      <div className="flex items-center justify-center py-8">
                        <Spinner size={16} className="text-primary" />
                        <span className="ml-2 text-sm text-muted-foreground">Carregando...</span>
                      </div>
                    ) : gestores.length === 0 ? (
                      <div className="py-8 text-center text-sm text-muted-foreground">
                        {gestorSearch.trim() ? 'Nenhum gestor encontrado' : 'Nenhum gestor disponível'}
                      </div>
                    ) : (
                      gestores.map((gestor) => {
                        const isSelected = gestoresSelecionados.some(
                          (g) => g.codGestorExterno === gestor.codGestorExterno,
                        )
                        return (
                          <div
                            key={gestor.codGestorExterno}
                            className="flex items-center space-x-2 rounded-lg p-2 hover:bg-accent cursor-pointer"
                            onClick={() => {
                              if (!isSelected) {
                                handleSelectGestor(gestor)
                              }
                            }}
                          >
                            <Checkbox checked={isSelected} />
                            <div className="flex-1 min-w-0">
                              <div className="text-sm font-medium truncate">{gestor.nome}</div>
                              <div className="text-xs text-muted-foreground truncate">{gestor.email}</div>
                            </div>
                          </div>
                        )
                      })
                    )}
                  </div>
                </ScrollArea>
              </PopoverContent>
            </Popover>
            {gestoresSelecionados.length > 0 && (
              <div className="flex flex-wrap gap-2">
                {gestoresSelecionados.map((gestor) => (
                  <div
                    key={gestor.codGestorExterno}
                    className="flex items-center gap-1 px-2 py-1 bg-secondary rounded-md text-xs"
                  >
                    <span className="truncate max-w-[150px]">{gestor.nome}</span>
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      className="h-4 w-4 p-0 hover:bg-destructive hover:text-destructive-foreground"
                      onClick={() => handleRemoveGestor(gestor.codGestorExterno)}
                    >
                      <X className="h-3 w-3" />
                    </Button>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {/* Botão Limpar Filtros */}
        <Button variant="outline" className="w-full" onClick={limparFiltros}>
          Limpar Filtros
        </Button>
      </CardContent>
    </Card>
  )
}
