import { useCallback, useRef } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { Spinner } from '@/components/ui/spinner'
import { cn } from '@/lib/utils'
import type { Colaborador } from '@domain/entities/Colaborador'
import { ChevronDown, Search, User } from 'lucide-react'

const ALTURA_SCROLL_PX = 300

export interface ResponsavelValue {
  codigoColaborador?: string
  nomeColaborador?: string
}

interface DropdownResponsavelProximosPassosProps {
  value: ResponsavelValue | null
  onChange: (colaborador: ResponsavelValue | null) => void
  colaboradores: Colaborador[]
  loading: boolean
  hasMore: boolean
  search: string
  onSearchChange: (value: string) => void
  onOpenChange: (open: boolean) => void
  onLoadMore: () => void
  disabled?: boolean
  id?: string
  placeholder?: string
}

export function DropdownResponsavelProximosPassos({
  value,
  onChange,
  colaboradores,
  loading,
  hasMore,
  search,
  onSearchChange,
  onOpenChange,
  onLoadMore,
  disabled = false,
  id,
  placeholder = 'Selecione o responsável',
}: DropdownResponsavelProximosPassosProps) {
  const scrollRef = useRef<HTMLDivElement>(null)
  const loadingMoreRef = useRef(false)

  const handleScroll = useCallback(
    (e: React.UIEvent<HTMLDivElement>) => {
      const el = e.currentTarget
      if (!hasMore || loading || loadingMoreRef.current) return
      const threshold = el.scrollHeight - el.clientHeight - 80
      if (el.scrollTop >= threshold) {
        loadingMoreRef.current = true
        onLoadMore()
        setTimeout(() => {
          loadingMoreRef.current = false
        }, 300)
      }
    },
    [hasMore, loading, onLoadMore],
  )

  /** Código interno do colaborador (UUID); no entity esse campo pode vir como cpf. Usar para registrar responsável no passo. */
  const codigoInternoColaborador = useCallback((c: Colaborador) => c.cpf || c.codColaborador || '', [])

  const selectColab = useCallback(
    (c: Colaborador) => {
      const cod = codigoInternoColaborador(c)
      const nome = c.nome || ''
      onChange(cod ? { codigoColaborador: cod, nomeColaborador: nome } : null)
      onOpenChange(false)
    },
    [onChange, onOpenChange, codigoInternoColaborador],
  )

  const displayValue = value?.nomeColaborador?.trim() || placeholder
  const isSelected = (c: Colaborador) => {
    const cod = codigoInternoColaborador(c)
    return !!cod && !!value?.codigoColaborador && cod === value.codigoColaborador
  }

  return (
    <Popover onOpenChange={onOpenChange}>
      <PopoverTrigger asChild>
        <Button
          id={id}
          variant="outline"
          role="combobox"
          className="w-full justify-between font-normal rounded-lg"
          disabled={disabled}
        >
          <span className="truncate">{displayValue}</span>
          <ChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-[var(--radix-popover-trigger-width)] min-w-[280px] p-0" align="start">
        <div className="flex items-center border-b px-3">
          <Search className="mr-2 h-4 w-4 shrink-0 opacity-50" />
          <Input
            placeholder="Buscar colaborador..."
            value={search}
            onChange={(e) => onSearchChange(e.target.value)}
            className="border-0 focus-visible:ring-0 rounded-none"
          />
        </div>
        <div
          ref={scrollRef}
          role="listbox"
          className="overflow-y-auto overflow-x-hidden p-2 overscroll-contain"
          style={{
            maxHeight: ALTURA_SCROLL_PX,
            overscrollBehavior: 'contain',
            WebkitOverflowScrolling: 'touch',
          }}
          onScroll={handleScroll}
          onWheel={(e) => e.stopPropagation()}
        >
            {loading && colaboradores.length === 0 ? (
              <div className="flex items-center justify-center py-8">
                <Spinner size={16} className="text-primary" />
                <span className="ml-2 text-sm text-muted-foreground">Carregando...</span>
              </div>
            ) : colaboradores.length === 0 ? (
              <div className="py-8 text-center text-sm text-muted-foreground">
                Nenhum colaborador encontrado
              </div>
            ) : (
              <>
                {colaboradores.map((colaborador, index) => {
                  const colaboradorId = colaborador.cpf || colaborador.codColaborador || `colab-${index}`
                  const selected = isSelected(colaborador)
                  return (
                    <div
                      key={colaboradorId}
                      className={cn(
                        'flex items-center gap-2 rounded-lg p-2 cursor-pointer hover:bg-accent',
                        selected && 'bg-primary/10 text-primary',
                      )}
                      onClick={() => selectColab(colaborador)}
                    >
                      <User className="h-4 w-4 shrink-0 opacity-70" />
                      <div className="flex-1 min-w-0">
                        <div className="text-sm font-medium truncate">
                          {colaborador.nome || 'Nome não informado'}
                        </div>
                        <div className="text-xs text-muted-foreground truncate">
                          {colaborador.email || ''} {colaborador.codColaborador ? `• ${colaborador.codColaborador}` : ''}
                        </div>
                      </div>
                    </div>
                  )
                })}
                {loading && colaboradores.length > 0 && (
                  <div className="flex items-center justify-center py-4">
                    <Spinner size={16} className="text-primary" />
                  </div>
                )}
                {hasMore && !loading && colaboradores.length > 0 && (
                  <div className="py-2 text-center text-xs text-muted-foreground">
                    Role para carregar mais
                  </div>
                )}
              </>
            )}
        </div>
      </PopoverContent>
    </Popover>
  )
}
