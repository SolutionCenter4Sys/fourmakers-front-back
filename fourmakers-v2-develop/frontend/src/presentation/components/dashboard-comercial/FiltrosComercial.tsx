import { useState, useEffect, useCallback, useRef } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  selectFiltros,
  setFilter,
  clearFilters,
} from '@app/store/slices/dashboardComercialSlice'
import { fetchColaboradores } from '@app/store/slices/colaboradoresSlice'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
  TooltipProvider,
} from '@/components/ui/tooltip'
import { Input } from '@/components/ui/input'
import { ScrollArea } from '@/components/ui/scroll-area'
import { SlidersHorizontal, Building2, Users, User } from '@/components/ui/system-icons'
import { Spinner } from '@/components/ui/spinner'
import { cn } from '@/lib/utils'
import { useLimparBuscaAoFecharPopover } from '@shared/hooks/useLimparBuscaAoFecharPopover'
import { useGestoresExternos } from '@presentation/hooks/useGestoresExternos'
import { container } from '@core/di/container'
import { ListarClientesUseCase } from '@domain/usecases/ListarClientesUseCase'
import type { Colaborador } from '@domain/entities/Colaborador'
import type { ClienteListItem } from '@domain/entities/Cliente'

export function FiltrosComercial() {
  const dispatch = useAppDispatch()
  const filtros = useAppSelector(selectFiltros)
  const { token, user } = useAppSelector((state) => state.auth)
  const {
    colaboradores,
    status: colaboradoresStatus,
  } = useAppSelector((state) => state.colaboradores)

  const [clientesPopoverOpen, setClientesPopoverOpen] = useState(false)
  const [comercialPopoverOpen, setComercialPopoverOpen] = useState(false)
  const [clienteSearch, setClienteSearch] = useState('')
  const [comercialSearch, setComercialSearch] = useState('')

  const [clientes, setClientes] = useState<ClienteListItem[]>([])
  const [clientesLoading, setClientesLoading] = useState(false)
  const [comercialSearchDebounced, setComercialSearchDebounced] = useState('')
  const [selectedClienteLabel, setSelectedClienteLabel] = useState<string | null>(null)
  const [selectedComercialLabel, setSelectedComercialLabel] = useState<string | null>(null)

  const orgId = user?.colaboradorOrg?.orgId ?? 0
  const codigoClienteSelecionado = filtros.codigoCliente
  const clienteObrigatorioParaComercial =
    !codigoClienteSelecionado || String(codigoClienteSelecionado).trim() === ''

  useLimparBuscaAoFecharPopover(clientesPopoverOpen, setClienteSearch)
  useLimparBuscaAoFecharPopover(
    comercialPopoverOpen,
    setComercialSearch,
    setComercialSearchDebounced,
  )

  // Debounce da busca de comercial (colaborador/gestor)
  useEffect(() => {
    const timer = setTimeout(() => {
      setComercialSearchDebounced(comercialSearch)
    }, 300)
    return () => clearTimeout(timer)
  }, [comercialSearch])

  const {
    gestores,
    loading: gestoresLoading,
    loadingMore: gestoresLoadingMore,
    hasMore: gestoresHasMore,
    loadMore: gestoresLoadMore,
  } = useGestoresExternos({
    token,
    codigoCliente: codigoClienteSelecionado ?? null,
    busca: comercialSearch,
    enabled: filtros.tipoComercial === 'gestor-externo' && comercialPopoverOpen,
    paginado: true,
  })

  const comercialScrollRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (comercialPopoverOpen && filtros.tipoComercial === 'colaborador' && token && orgId > 0) {
      dispatch(
        fetchColaboradores({
          token,
          orgId,
          cursor: 0,
          limite: 50,
          nomeOuEmail: comercialSearchDebounced,
          ...(codigoClienteSelecionado ? { codigoCliente: codigoClienteSelecionado } : {}),
        }),
      )
    }
  }, [
    comercialPopoverOpen,
    filtros.tipoComercial,
    token,
    orgId,
    dispatch,
    comercialSearchDebounced,
    codigoClienteSelecionado,
  ])

  const handleScrollComercial = useCallback(
    (e: React.UIEvent<HTMLDivElement>) => {
      const { scrollTop, scrollHeight, clientHeight } = e.currentTarget
      const margem = 60
      const pertoDoFim = scrollHeight - scrollTop - clientHeight < margem
      if (!pertoDoFim) return
      if (filtros.tipoComercial === 'gestor-externo' && gestoresHasMore && gestoresLoadMore) {
        gestoresLoadMore()
      }
    },
    [filtros.tipoComercial, gestoresHasMore, gestoresLoadMore],
  )

  // Buscar clientes — mesmo padrão de FiltrosAgenda (Agendas Comerciais)
  useEffect(() => {
    const loadClientes = async () => {
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
        setClientes(data)
      } catch (error) {
        console.error('Erro ao buscar clientes:', error)
        setClientes([])
      } finally {
        setClientesLoading(false)
      }
    }
    const delay = clientesPopoverOpen && !clienteSearch.trim() ? 0 : 300
    const timer = setTimeout(() => void loadClientes(), delay)
    return () => clearTimeout(timer)
  }, [token, clienteSearch, clientesPopoverOpen])

  const handleSelectCliente = useCallback(
    (cliente: ClienteListItem | null) => {
      setSelectedClienteLabel(cliente ? cliente.name : null)
      dispatch(
        setFilter({
          name: 'codigoCliente',
          value: cliente ? cliente.id : null,
        }),
      )
      dispatch(
        setFilter({
          name: 'nomeClienteSelecionado',
          value: cliente ? cliente.name : null,
        }),
      )
      // Ao alterar o cliente, volta o filtro comercial para "Todos"
      dispatch(setFilter({ name: 'codigoGestorExterno', value: null }))
      dispatch(setFilter({ name: 'codigoColaboradorAgendou', value: null }))
      dispatch(setFilter({ name: 'nomeComercialSelecionado', value: null }))
      setSelectedComercialLabel(null)
      setClientesPopoverOpen(false)
    },
    [dispatch],
  )

  const handleSelectComercial = useCallback(
    (item: { codigo: string; nome: string } | null) => {
      setSelectedComercialLabel(item ? item.nome : null)
      dispatch(
        setFilter({
          name: 'nomeComercialSelecionado',
          value: item ? item.nome : null,
        }),
      )
      if (filtros.tipoComercial === 'gestor-externo') {
        dispatch(
          setFilter({
            name: 'codigoGestorExterno',
            value: item ? item.codigo : null,
          }),
        )
      } else {
        dispatch(
          setFilter({
            name: 'codigoColaboradorAgendou',
            value: item ? item.codigo : null,
          }),
        )
      }
      setComercialPopoverOpen(false)
    },
    [dispatch, filtros.tipoComercial],
  )

  const handleLimparFiltros = useCallback(() => {
    setSelectedClienteLabel(null)
    setSelectedComercialLabel(null)
    dispatch(clearFilters())
  }, [dispatch])

  const clienteSelecionadoNome = useCallback(() => {
    if (!codigoClienteSelecionado) return 'Todos os clientes'
    if (selectedClienteLabel) return selectedClienteLabel
    const c = clientes.find((x) => x.id === codigoClienteSelecionado)
    return c ? c.name : codigoClienteSelecionado
  }, [codigoClienteSelecionado, clientes, selectedClienteLabel])

  const comercialSelecionadoNome = useCallback(() => {
    if (filtros.tipoComercial === 'gestor-externo') {
      if (!filtros.codigoGestorExterno) return 'Todos'
      if (selectedComercialLabel) return selectedComercialLabel
      const g = gestores.find((x) => x.codGestorExterno === filtros.codigoGestorExterno)
      return g ? g.nome : filtros.codigoGestorExterno
    }
    if (!filtros.codigoColaboradorAgendou) return 'Todos'
    if (selectedComercialLabel) return selectedComercialLabel
    const c = colaboradores.find(
      (x) => x.codColaborador === filtros.codigoColaboradorAgendou,
    )
    return c ? c.nome : filtros.codigoColaboradorAgendou
  }, [
    filtros.tipoComercial,
    filtros.codigoGestorExterno,
    filtros.codigoColaboradorAgendou,
    selectedComercialLabel,
    gestores,
    colaboradores,
  ])

  const termoBusca = (comercialSearchDebounced ?? '').toString().trim().toLowerCase()
  const colaboradoresFiltrados =
    termoBusca && filtros.tipoComercial === 'colaborador'
      ? colaboradores.filter((c) => {
          const nome = (c?.nome ?? '').toString().toLowerCase()
          const email = (c?.email ?? '').toString().toLowerCase()
          const cod = (c?.codColaborador ?? '').toString().toLowerCase()
          return nome.includes(termoBusca) || email.includes(termoBusca) || cod.includes(termoBusca)
        })
      : colaboradores

  // Remove itens com id ou nome nulos/vazios das listagens dos combos
  const clientesFiltradosSemNulos = clientes.filter(
    (c) => (c?.id ?? '').toString().trim() !== '' && (c?.name ?? '').toString().trim() !== '',
  )
  const gestoresSemNulos = gestores.filter(
    (g) =>
      (g?.codGestorExterno ?? '').toString().trim() !== '' && (g?.nome ?? '').toString().trim() !== '',
  )
  const colaboradoresFiltradosSemNulos = colaboradoresFiltrados.filter(
    (c) =>
      (c?.codColaborador ?? '').toString().trim() !== '' && (c?.nome ?? '').toString().trim() !== '',
  )

  return (
    <Card className="mb-6 rounded-lg border-borderSoft shadow-softToken">
      <CardHeader className="mb-4 p-4">
        <div className="flex items-center justify-between">
          <CardTitle className="text-lg font-semibold text-primaryText">
            Filtros
          </CardTitle>
          <Button
            variant="ghost"
            size="sm"
            className="flex items-center gap-1 text-sm text-secondaryText hover:text-primaryText"
            onClick={handleLimparFiltros}
          >
            <SlidersHorizontal size={14} />
            Limpar Filtros
          </Button>
        </div>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
          <div className="space-y-2">
            <Label htmlFor="data-inicio">Data início</Label>
            <Input
              id="data-inicio"
              type="date"
              value={filtros.dataInicio ?? ''}
              onChange={(e) =>
                dispatch(setFilter({ name: 'dataInicio', value: e.target.value || null }))
              }
              className="rounded-lg"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="data-fim">Data fim</Label>
            <Input
              id="data-fim"
              type="date"
              value={filtros.dataFim ?? ''}
              onChange={(e) =>
                dispatch(setFilter({ name: 'dataFim', value: e.target.value || null }))
              }
              className="rounded-lg"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="cliente">Cliente</Label>
            <Popover open={clientesPopoverOpen} onOpenChange={setClientesPopoverOpen}>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  role="combobox"
                  aria-expanded={clientesPopoverOpen}
                  className="w-full justify-between rounded-lg font-normal"
                >
                  <span className="truncate">{clienteSelecionadoNome()}</span>
                  <Building2 className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                <div className="flex items-center border-b p-2">
                  <Input
                    placeholder="Buscar cliente..."
                    value={clienteSearch}
                    onChange={(e) => setClienteSearch(e.target.value)}
                    className="h-9 border-0 focus-visible:ring-0"
                    autoFocus
                  />
                </div>
                <ScrollArea className="h-[200px] overflow-x-hidden">
                  <div className="p-1">
                    <button
                      type="button"
                      className={cn(
                        'flex w-full items-center gap-2 rounded-lg px-3 py-2 text-left text-sm hover:bg-accent',
                        !codigoClienteSelecionado && 'bg-accent',
                      )}
                      onClick={() => handleSelectCliente(null)}
                    >
                      Todos os clientes
                    </button>
                    {clientesLoading ? (
                      <div className="flex items-center justify-center py-6">
                        <Spinner size={20} className="text-primary" />
                      </div>
                    ) : clientesFiltradosSemNulos.length === 0 ? (
                      <div className="py-6 text-center text-sm text-muted-foreground">
                        Nenhum cliente encontrado
                      </div>
                    ) : (
                      clientesFiltradosSemNulos.map((cliente) => (
                        <button
                          key={cliente.id}
                          type="button"
                          className={cn(
                            'flex min-w-0 w-full items-center gap-2 rounded-lg px-3 py-2 text-left text-sm hover:bg-accent',
                            codigoClienteSelecionado === cliente.id && 'bg-accent',
                          )}
                          onClick={() => handleSelectCliente(cliente)}
                        >
                          <span className="min-w-0 truncate">{cliente.name}</span>
                        </button>
                      ))
                    )}
                  </div>
                </ScrollArea>
              </PopoverContent>
            </Popover>
          </div>

          <div className="space-y-2">
            <Label htmlFor="comercial">Comercial</Label>
            <TooltipProvider delayDuration={300}>
              <Popover
                open={comercialPopoverOpen}
                onOpenChange={(open) => {
                  if (open && clienteObrigatorioParaComercial) return
                  setComercialPopoverOpen(open)
                  if (!open) setComercialSearch('')
                }}
              >
                <Tooltip>
                  <TooltipTrigger asChild>
                    <span className={cn('block w-full', clienteObrigatorioParaComercial && 'cursor-not-allowed')}>
                      <PopoverTrigger asChild>
                        <Button
                          id="comercial"
                          variant="outline"
                          role="combobox"
                          aria-expanded={comercialPopoverOpen}
                          disabled={clienteObrigatorioParaComercial}
                          className="w-full justify-between rounded-lg font-normal"
                        >
                          <span className="truncate">{comercialSelecionadoNome()}</span>
                          {filtros.tipoComercial === 'gestor-externo' ? (
                            <User className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                          ) : (
                            <Users className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                          )}
                        </Button>
                      </PopoverTrigger>
                    </span>
                  </TooltipTrigger>
                  <TooltipContent side="top" className="max-w-[240px]">
                    {clienteObrigatorioParaComercial
                      ? 'Selecione um cliente para alterar o filtro Comercial.'
                      : comercialSelecionadoNome()}
                  </TooltipContent>
                </Tooltip>
              <PopoverContent
                className="w-[min(var(--radix-popover-trigger-width),320px)] p-0"
                align="start"
              >
                <div className="flex flex-col">
                    <div className="border-b p-3">
                      <p className="mb-2 text-xs font-medium text-muted-foreground">
                        Tipo de comercial
                      </p>
                      <div className="flex gap-1 rounded-lg bg-muted p-1">
                        <button
                          type="button"
                          onClick={() => {
                            setSelectedComercialLabel(null)
                            dispatch(
                              setFilter({
                                name: 'tipoComercial',
                                value: 'gestor-externo',
                              }),
                            )
                            dispatch(setFilter({ name: 'codigoGestorExterno', value: null }))
                            dispatch(setFilter({ name: 'codigoColaboradorAgendou', value: null }))
                            dispatch(setFilter({ name: 'nomeComercialSelecionado', value: null }))
                          }}
                          className={cn(
                            'flex-1 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                            filtros.tipoComercial === 'gestor-externo'
                              ? 'bg-background text-foreground shadow-sm'
                              : 'text-muted-foreground hover:text-foreground',
                          )}
                        >
                          Gestor Externo
                        </button>
                        <button
                          type="button"
                          onClick={() => {
                            setSelectedComercialLabel(null)
                            dispatch(
                              setFilter({
                                name: 'tipoComercial',
                                value: 'colaborador',
                              }),
                            )
                            dispatch(setFilter({ name: 'codigoGestorExterno', value: null }))
                            dispatch(setFilter({ name: 'codigoColaboradorAgendou', value: null }))
                            dispatch(setFilter({ name: 'nomeComercialSelecionado', value: null }))
                          }}
                          className={cn(
                            'flex-1 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                            filtros.tipoComercial === 'colaborador'
                              ? 'bg-background text-foreground shadow-sm'
                              : 'text-muted-foreground hover:text-foreground',
                          )}
                        >
                          Colaborador
                        </button>
                      </div>
                    </div>
                    <div className="border-b p-2">
                      <Input
                        placeholder={
                          filtros.tipoComercial === 'gestor-externo'
                            ? 'Buscar gestor...'
                            : 'Buscar colaborador...'
                        }
                        value={comercialSearch}
                        onChange={(e) => setComercialSearch(e.target.value)}
                        className="h-9 border-0 focus-visible:ring-0"
                        autoFocus
                      />
                    </div>
                    <div
                      ref={comercialScrollRef}
                      className="h-[200px] overflow-x-hidden overflow-y-auto"
                      onScroll={handleScrollComercial}
                    >
                      <div className="p-1">
                        <button
                          type="button"
                          className={cn(
                            'flex w-full items-center gap-2 rounded-lg px-3 py-2 text-left text-sm hover:bg-accent',
                            (filtros.tipoComercial === 'gestor-externo'
                              ? !filtros.codigoGestorExterno
                              : !filtros.codigoColaboradorAgendou) && 'bg-accent',
                          )}
                          onClick={() => handleSelectComercial(null)}
                        >
                          Todos
                        </button>
                        {filtros.tipoComercial === 'gestor-externo' ? (
                          gestoresLoading ? (
                            <div className="flex items-center justify-center py-6">
                              <Spinner size={20} className="text-primary" />
                            </div>
                          ) : gestoresSemNulos.length === 0 ? (
                            <div className="py-6 text-center text-sm text-muted-foreground">
                              Nenhum gestor encontrado
                            </div>
                          ) : (
                            <>
                              {gestoresSemNulos.map((gestor) => (
                              <button
                                key={gestor.codGestorExterno}
                                type="button"
                                className={cn(
                                  'flex min-w-0 w-full items-center gap-2 rounded-lg px-3 py-2 text-left text-sm hover:bg-accent',
                                  filtros.codigoGestorExterno === gestor.codGestorExterno &&
                                    'bg-accent',
                                )}
                                onClick={() =>
                                  handleSelectComercial({
                                    codigo: gestor.codGestorExterno,
                                    nome: gestor.nome,
                                  })
                                }
                              >
                                <span className="min-w-0 truncate">{gestor.nome}</span>
                              </button>
                            ))}
                              {gestoresLoadingMore && gestoresSemNulos.length > 0 && (
                                <div className="flex justify-center py-2">
                                  <Spinner size={18} className="text-primary" />
                                </div>
                              )}
                            </>
                          )
                        ) : colaboradoresStatus === 'loading' && colaboradoresFiltradosSemNulos.length === 0 ? (
                          <div className="flex items-center justify-center py-6">
                            <Spinner size={20} className="text-primary" />
                          </div>
                        ) : colaboradoresFiltradosSemNulos.length === 0 ? (
                          <div className="py-6 text-center text-sm text-muted-foreground">
                            Nenhum colaborador encontrado
                          </div>
                        ) : (
                          <>
                            {colaboradoresFiltradosSemNulos.map((colab: Colaborador) => (
                              <button
                                key={colab.codColaborador}
                                type="button"
                                className={cn(
                                  'flex min-w-0 w-full flex-col items-start gap-0 rounded-lg px-3 py-2 text-left text-sm hover:bg-accent',
                                  filtros.codigoColaboradorAgendou === colab.codColaborador &&
                                    'bg-accent',
                                )}
                                onClick={() =>
                                  handleSelectComercial({
                                    codigo: colab.codColaborador,
                                    nome: colab.nome,
                                  })
                                }
                              >
                                <span className="min-w-0 truncate w-full">{colab.nome}</span>
                                {colab.email ? (
                                  <span className="min-w-0 truncate w-full text-xs text-muted-foreground">
                                    {colab.email}
                                  </span>
                                ) : null}
                              </button>
                            ))}
                            {colaboradoresStatus === 'loading' && colaboradoresFiltradosSemNulos.length > 0 && (
                              <div className="flex justify-center py-2">
                                <Spinner size={18} className="text-primary" />
                              </div>
                            )}
                          </>
                        )}
                      </div>
                    </div>
                  </div>
              </PopoverContent>
            </Popover>
            </TooltipProvider>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
