import { useState, useEffect, useMemo, useCallback, useRef } from 'react'
import { User, Search } from 'lucide-react'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'
import { Spinner } from '@/components/ui/spinner'
import { container } from '@core/di/container'
import { ListarColaboradoresExternosAlocadosUseCase } from '@domain/usecases/ListarColaboradoresExternosAlocadosUseCase'
import { useDebounced } from '@shared/hooks/useDebounced'
import { toast } from 'sonner'
import { logError } from '@shared/utils/firebaseCrashlytics'

const TAMANHO_PAGINA = 10

interface ColaboradorAdaptado {
  codColaborador: string
  cpf: string
  nome: string
  email: string
}

export interface AutocompleteProfissionalMapaProps {
  label: string
  value: string
  onSelect: (value: string) => void
  onColaboradorSelect?: (colab: { nome: string; email: string } | null) => void
  token: string | null
  user: { colaboradorOrg?: { orgId: number } } | null
  /** Código do cliente do mapa (obrigatório para listar profissionais por cliente). */
  codigoCliente: string
  required?: boolean
  open?: boolean
  employeeName?: string
  refreshTrigger?: number
}

export function AutocompleteProfissionalMapa({
  label,
  value,
  onSelect,
  onColaboradorSelect,
  token,
  codigoCliente,
  required = false,
  open: modalOpen = false,
  employeeName,
  refreshTrigger,
}: AutocompleteProfissionalMapaProps) {
  const wrapperRef = useRef<HTMLDivElement>(null)
  const dropdownRef = useRef<HTMLDivElement>(null)

  const [allColaboradores, setAllColaboradores] = useState<ColaboradorAdaptado[]>([])
  const [searchTerm, setSearchTerm] = useState('')
  const debouncedSearch = useDebounced(searchTerm, 500)
  const [listaOpen, setListaOpen] = useState(false)
  const [loading, setLoading] = useState(false)
  const [isLoadingMore, setIsLoadingMore] = useState(false)
  const [hasMore, setHasMore] = useState(true)
  const cursorRef = useRef(0)
  const loadingRef = useRef(false)
  const hasMoreRef = useRef(hasMore)
  const isLoadingMoreRef = useRef(isLoadingMore)

  hasMoreRef.current = hasMore
  isLoadingMoreRef.current = isLoadingMore

  const loadColaboradores = useCallback(
    async (isInitial: boolean) => {
      if (!token || !codigoCliente || loadingRef.current) return

      loadingRef.current = true
      if (isInitial) {
        setLoading(true)
        setAllColaboradores([])
        cursorRef.current = 0
        setHasMore(true)
      } else {
        setIsLoadingMore(true)
      }

      try {
        const useCase = container.resolve(ListarColaboradoresExternosAlocadosUseCase)
        const currentCursor = isInitial ? 0 : cursorRef.current
        const response = await useCase.execute(
          token,
          codigoCliente,
          currentCursor,
          TAMANHO_PAGINA,
          debouncedSearch.trim() || undefined,
        )

        const adaptados: ColaboradorAdaptado[] = (response ?? []).map((colab) => ({
          codColaborador: colab.CodigoInternoColaborador,
          cpf: colab.CodigoInternoColaborador,
          nome: colab.NomeColaborador,
          email: '',
        }))

        if (isInitial) {
          setAllColaboradores(adaptados)
        } else {
          setAllColaboradores((prev) => [...prev, ...adaptados])
        }

        if (response.length < TAMANHO_PAGINA) setHasMore(false)
        cursorRef.current = currentCursor + response.length

        if (isInitial) {
          setTimeout(() => {
            if (dropdownRef.current) {
              dropdownRef.current.scrollIntoView({ behavior: 'smooth', block: 'nearest' })
            }
          }, 150)
        }
      } catch (error) {
        logError(error, { component: 'AutocompleteProfissionalMapa', action: 'loadColaboradores' })
        toast.error('Erro ao carregar profissionais')
      } finally {
        setLoading(false)
        setIsLoadingMore(false)
        loadingRef.current = false
      }
    },
    [token, codigoCliente, debouncedSearch],
  )

  const opcoesExibidas = useMemo(
    () => [
      { codColaborador: 'vacant', cpf: 'vacant', nome: '-- Vago --', email: '' },
      ...allColaboradores,
    ],
    [allColaboradores],
  )

  const selectedColaborador = useMemo(
    () =>
      allColaboradores.find((c) => c.codColaborador === value || c.cpf === value),
    [allColaboradores, value]
  )

  const textoExibido = useMemo(() => {
    if (value === 'vacant') return '-- Vago --'
    if (selectedColaborador) return selectedColaborador.nome
    return employeeName ?? 'Selecione um profissional...'
  }, [value, selectedColaborador, employeeName])

  const handleScrollLista = useCallback(
    (e: React.UIEvent<HTMLUListElement>) => {
      const { scrollTop, scrollHeight, clientHeight } = e.currentTarget
      const margemBottom = 80
      const isNearBottom = scrollHeight - scrollTop - clientHeight < margemBottom
      if (
        isNearBottom &&
        hasMoreRef.current &&
        !loadingRef.current &&
        !isLoadingMoreRef.current
      ) {
        void loadColaboradores(false)
      }
    },
    [loadColaboradores]
  )

  const handleSelect = useCallback(
    (cod: string, nome: string, email: string) => {
      onSelect(cod)
      if (cod === 'vacant' || !cod) {
        onColaboradorSelect?.(null)
      } else {
        onColaboradorSelect?.({ nome, email })
      }
      setListaOpen(false)
      setSearchTerm('')
    },
    [onSelect, onColaboradorSelect]
  )

  // Carregar ao abrir o dropdown ou quando o termo de busca (debounced) mudar
  useEffect(() => {
    if (!listaOpen || !token || !codigoCliente) return
    setAllColaboradores([])
    cursorRef.current = 0
    setHasMore(true)
    void loadColaboradores(true)
    setTimeout(() => {
      if (dropdownRef.current) {
        dropdownRef.current.scrollIntoView({ behavior: 'smooth', block: 'nearest' })
      }
    }, 100)
  }, [listaOpen, debouncedSearch, token, codigoCliente, loadColaboradores])

  useEffect(() => {
    if (refreshTrigger !== undefined && refreshTrigger > 0 && token && codigoCliente) {
      void loadColaboradores(true)
    }
  }, [refreshTrigger, token, codigoCliente, loadColaboradores])

  useEffect(() => {
    if (!modalOpen) {
      setListaOpen(false)
      setAllColaboradores([])
      setSearchTerm('')
      cursorRef.current = 0
    }
  }, [modalOpen])

  useEffect(() => {
    const handleMouseDown = (e: MouseEvent) => {
      if (wrapperRef.current && !wrapperRef.current.contains(e.target as Node)) {
        setListaOpen(false)
      }
    }
    document.addEventListener('mousedown', handleMouseDown)
    return () => document.removeEventListener('mousedown', handleMouseDown)
  }, [])

  return (
    <div ref={wrapperRef} className="w-full">
      <Label
        className={cn(
          'text-sm font-bold text-foreground mb-2 block',
          required && "after:content-['*'] after:ml-0.5 after:text-destructive"
        )}
      >
        {label}
      </Label>
      <div className="relative">
        <Button
          type="button"
          variant="outline"
          role="combobox"
          aria-expanded={listaOpen}
          className="h-10 pl-12 pr-10 rounded-lg w-full justify-between font-bold text-sm border-border"
          onClick={() => setListaOpen((o) => !o)}
        >
          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none">
            {(loading && !listaOpen) ? (
              <Spinner size={20} className="text-primary" />
            ) : (
              <User className="w-5 h-5" />
            )}
          </span>
          <span className="flex-1 min-w-0 truncate text-left pl-2">
            {textoExibido}
          </span>
        </Button>

        {listaOpen && (
          <div 
            ref={dropdownRef}
            className="absolute top-full left-0 right-0 z-50 mt-1 rounded-lg border border-border bg-popover text-popover-foreground shadow-md"
          >
            <div className="p-2 border-b border-border bg-popover">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                <Input
                  type="text"
                  placeholder="Buscar profissional..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  onKeyDown={(e) => e.stopPropagation()}
                  className="h-9 pl-9 text-xs rounded-lg border-border"
                  autoFocus
                  autoComplete="off"
                />
              </div>
            </div>
            <ul
              className="max-h-[200px] overflow-auto py-1"
              onScroll={handleScrollLista}
              role="listbox"
            >
              {loading && opcoesExibidas.length <= 1 ? (
                <li className="flex items-center justify-center gap-2 py-4 text-muted-foreground text-xs">
                  <Spinner size={16} className="text-primary" />
                  <span>Carregando profissionais...</span>
                </li>
              ) : opcoesExibidas.length === 0 ? (
                <li className="py-4 text-center text-xs text-muted-foreground italic">
                  Nenhum profissional encontrado
                </li>
              ) : (
                <>
                  {opcoesExibidas.map((colab) => {
                    const colabValue = colab.cpf || colab.codColaborador || 'vacant'
                    return (
                      <li
                        key={colab.codColaborador || colab.cpf}
                        role="option"
                        className="cursor-pointer px-3 py-2 text-sm text-foreground transition-colors hover:bg-accent hover:text-accent-foreground"
                        onClick={() =>
                          handleSelect(colabValue, colab.nome, colab.email ?? '')
                        }
                      >
                        <div className="flex flex-col">
                          <span className="font-bold text-xs">{colab.nome}</span>
                          {colab.email && (
                            <span className="text-xs text-muted-foreground">
                              {colab.email}
                            </span>
                          )}
                        </div>
                      </li>
                    )
                  })}
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
