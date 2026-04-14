import { useState, useEffect, useMemo, useCallback, useRef } from 'react'
import type { ClienteMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { Building, ChevronsUpDown } from 'lucide-react'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'
import { Spinner } from '@/components/ui/spinner'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { listarClientesMapa } from '@app/store/slices/mapaRelacionamentoSlice'
import { toast } from 'sonner'
import { useDebounced } from '@shared/hooks/useDebounced'

const PAGE_SIZE = 50

export interface AutocompleteClienteMapaProps {
  selectedCode: string
  /** Nome do cliente para exibição quando o cliente não está na lista (ex.: após persistência). Evita mostrar código no lugar do nome. */
  selectedName?: string
  /** Recebe código e, quando disponível, o cliente completo (ex.: seleção a partir da busca). */
  onSelect: (code: string, cliente?: ClienteMapaRelacionamento) => void
  onNameChange?: (name: string) => void
  variant?: 'header' | 'hero'
  clientes?: ClienteMapaRelacionamento[]
}

export function AutocompleteClienteMapa({
  selectedCode,
  selectedName,
  onSelect,
  onNameChange,
  variant = 'header',
  clientes = [],
}: AutocompleteClienteMapaProps) {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const wrapperRef = useRef<HTMLDivElement>(null)

  const [searchTerm, setSearchTerm] = useState('')
  const [open, setOpen] = useState(false)
  const [isSearching, setIsSearching] = useState(false)
  const [isLoadingMore, setIsLoadingMore] = useState(false)
  const [hasMore, setHasMore] = useState(true)
  const [cursorLocal, setCursorLocal] = useState(PAGE_SIZE)
  const [clientesExibidos, setClientesExibidos] = useState<ClienteMapaRelacionamento[]>([])
  const loadingRef = useRef(false)

  const debouncedSearch = useDebounced(searchTerm, 500)

  const selectedClient = useMemo(
    () =>
      clientes.find((c) => c.codigoCliente === selectedCode) ??
      clientesExibidos.find((c) => c.codigoCliente === selectedCode),
    [clientes, clientesExibidos, selectedCode]
  )

  useEffect(() => {
    if (selectedClient && onNameChange) {
      onNameChange(selectedClient.nomeCliente)
    }
    // Não definir onNameChange(selectedCode) quando não há selectedClient — evita exibir código no lugar do nome
  }, [selectedClient, onNameChange])

  // Busca com termo (debounced): primeira página via dispatch; resultado acumulado em clientesExibidos
  useEffect(() => {
    if (!token || !user?.colaboradorOrg?.orgId || !open) return
    if (!debouncedSearch.trim()) {
      setClientesExibidos([])
      return
    }

    const buscar = async () => {
      setIsSearching(true)
      setHasMore(true)
      try {
        const orgId = user.colaboradorOrg.orgId
        const result = await dispatch(
          listarClientesMapa({
            token,
            orgId,
            limite: PAGE_SIZE,
            cursor: 0,
            nomeCliente: debouncedSearch,
          })
        ).unwrap()
        setClientesExibidos(Array.isArray(result) ? result : [])
        setCursorLocal(PAGE_SIZE)
      } catch {
        toast.error('Erro ao buscar clientes')
        setClientesExibidos([])
      } finally {
        setIsSearching(false)
      }
    }

    void buscar()
  }, [debouncedSearch, token, user, dispatch, open])

  const loadMoreClientes = useCallback(async () => {
    if (!token || !user?.colaboradorOrg?.orgId || !hasMore || isLoadingMore || loadingRef.current) return

    loadingRef.current = true
    setIsLoadingMore(true)
    try {
      const orgId = user.colaboradorOrg.orgId
      const result = await dispatch(
        listarClientesMapa({
          token,
          orgId,
          limite: PAGE_SIZE,
          cursor: cursorLocal,
          nomeCliente: debouncedSearch,
        })
      ).unwrap()
      const proxima = Array.isArray(result) ? result : []
      if (proxima.length < PAGE_SIZE) setHasMore(false)
      setClientesExibidos((prev) => [...prev, ...proxima])
      setCursorLocal((c) => c + PAGE_SIZE)
    } catch {
      toast.error('Erro ao carregar mais clientes')
    } finally {
      setIsLoadingMore(false)
      loadingRef.current = false
    }
  }, [token, user, hasMore, isLoadingMore, cursorLocal, debouncedSearch, dispatch])

  const handleScrollLista = useCallback(
    (e: React.UIEvent<HTMLUListElement>) => {
      const { scrollTop, scrollHeight, clientHeight } = e.currentTarget
      const isNearBottom = scrollHeight - scrollTop - clientHeight < 100
      if (isNearBottom && hasMore && !isLoadingMore) {
        void loadMoreClientes()
      }
    },
    [hasMore, isLoadingMore, loadMoreClientes]
  )

  const handleSelect = useCallback(
    (client: ClienteMapaRelacionamento) => {
      onSelect(client.codigoCliente, client)
      onNameChange?.(client.nomeCliente)
      setOpen(false)
      setSearchTerm('')
    },
    [onSelect, onNameChange]
  )

  // Lista a exibir: com termo de busca = clientesExibidos; sem termo = clientes (props/store)
  const listaExibida = useMemo(() => {
    if (debouncedSearch.trim()) return clientesExibidos
    return clientes.filter((c) =>
      c.nomeCliente.toLowerCase().includes(searchTerm.toLowerCase())
    )
  }, [debouncedSearch, clientesExibidos, clientes, searchTerm])

  // Clique fora: fechar lista
  useEffect(() => {
    const handleMouseDown = (e: MouseEvent) => {
      if (wrapperRef.current && !wrapperRef.current.contains(e.target as Node)) {
        setOpen(false)
      }
    }
    document.addEventListener('mousedown', handleMouseDown)
    return () => document.removeEventListener('mousedown', handleMouseDown)
  }, [])

  const triggerHeight = variant === 'hero' ? 'h-14' : 'h-10'
  const triggerPadding = variant === 'hero' ? 'px-8' : 'px-5'
  const triggerTextSize = variant === 'hero' ? 'text-base' : 'text-sm'
  const iconSize = variant === 'hero' ? 'h-5 w-5' : 'h-4 w-4'

  return (
    <div
      ref={wrapperRef}
      className={cn('relative w-full', variant === 'hero' && 'max-w-md mx-auto')}
    >
      <div className="relative">
        <Button
          type="button"
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className={cn(
            'w-full justify-between font-bold gap-2',
            triggerHeight,
            triggerPadding,
            triggerTextSize,
            variant === 'hero' && 'rounded-full border-2 shadow-md',
            variant === 'header' && 'rounded-lg shadow-sm'
          )}
          onClick={() => setOpen((o) => !o)}
        >
          <Building
            className={cn(iconSize, 'shrink-0 text-primary', variant === 'hero' && 'scale-125')}
            aria-hidden
          />
          <span className="flex-1 min-w-0 truncate text-left">
            {selectedClient ? selectedClient.nomeCliente : (selectedName || selectedCode || 'Selecionar Cliente')}
          </span>
          <ChevronsUpDown className="h-4 w-4 shrink-0 opacity-50" aria-hidden />
        </Button>

        {open && (
          <div
            className={cn(
              'absolute top-full left-0 right-0 z-50 mt-1 rounded-lg border border-border bg-popover text-popover-foreground shadow-md',
              variant === 'hero' && 'max-w-md'
            )}
          >
            <div className="p-2 border-b border-border bg-popover">
              <div className="relative">
                <Building className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                <Input
                  type="text"
                  placeholder="Pesquisar cliente..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  onKeyDown={(e) => e.stopPropagation()}
                  className="h-9 pl-9 text-sm rounded-lg border-border"
                  autoFocus
                  autoComplete="off"
                />
              </div>
            </div>
            <ul
              className="max-h-[min(70vh,400px)] overflow-auto py-1"
              onScroll={handleScrollLista}
              role="listbox"
            >
              {isSearching && listaExibida.length === 0 ? (
                <li className="flex items-center justify-center gap-2 py-4 text-muted-foreground text-sm">
                  <Spinner size={20} className="text-primary" />
                  <span>Buscando clientes...</span>
                </li>
              ) : listaExibida.length === 0 ? (
                <li className="py-4 text-center text-sm text-muted-foreground">
                  {searchTerm.trim() ? 'Nenhum cliente encontrado.' : 'Digite para buscar clientes'}
                </li>
              ) : (
                <>
                  {listaExibida.map((client) => (
                    <li
                      key={client.codigoCliente}
                      role="option"
                      className={cn(
                        'flex items-center gap-2 px-3 py-3 cursor-pointer text-sm font-bold text-foreground transition-colors hover:bg-accent hover:text-accent-foreground'
                      )}
                      onClick={() => handleSelect(client)}
                    >
                      <Building className="h-4 w-4 shrink-0 text-muted-foreground" />
                      <span>{client.nomeCliente}</span>
                    </li>
                  ))}
                  {isLoadingMore && (
                    <li className="flex justify-center py-2">
                      <Spinner size={16} className="text-primary" />
                    </li>
                  )}
                </>
              )}
            </ul>
          </div>
        )}
      </div>
    </div>
  )
}
