import { useState, useEffect, useMemo, useCallback, useRef } from 'react'
import { Search, ChevronsUpDown } from 'lucide-react'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'

interface SimpleOption {
  id: string
  descricao: string
}

interface AutocompleteSimplesMapaProps {
  value: string
  onValueChange: (value: string) => void
  options: SimpleOption[]
  disabled?: boolean
  placeholder?: string
  className?: string
  icon?: React.ReactNode
  error?: boolean
}

export function AutocompleteSimplesMapa({
  value,
  onValueChange,
  options,
  disabled = false,
  placeholder = 'Selecione...',
  className,
  icon,
  error = false,
}: AutocompleteSimplesMapaProps) {
  const wrapperRef = useRef<HTMLDivElement>(null)
  const dropdownRef = useRef<HTMLDivElement>(null)

  const [searchTerm, setSearchTerm] = useState('')
  const [listaOpen, setListaOpen] = useState(false)

  const filteredOptions = useMemo(() => {
    const term = searchTerm.toLowerCase()
    return options.filter((opt) => opt.descricao.toLowerCase().includes(term))
  }, [options, searchTerm])

  const selectedOption = useMemo(
    () => options.find((opt) => opt.id === value),
    [options, value]
  )

  const handleSelect = useCallback(
    (optValue: string) => {
      onValueChange(optValue)
      setListaOpen(false)
      setSearchTerm('')
    },
    [onValueChange]
  )

  useEffect(() => {
    if (listaOpen) {
      setTimeout(() => {
        if (dropdownRef.current) {
          dropdownRef.current.scrollIntoView({ behavior: 'smooth', block: 'nearest' })
        }
      }, 100)
    }
  }, [listaOpen])

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
    <div ref={wrapperRef} className={cn('relative w-full', className)}>
      <div className="relative">
        {icon && (
          <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground z-10 pointer-events-none">
            {icon}
          </div>
        )}
        <Button
          type="button"
          variant="outline"
          role="combobox"
          aria-expanded={listaOpen}
          className={cn(
            'h-10 w-full justify-between text-sm border-border rounded-lg',
            icon && 'pl-12',
            error && 'border-destructive border-2 focus-visible:border-destructive focus-visible:ring-destructive'
          )}
          onClick={() => !disabled && setListaOpen((o) => !o)}
          disabled={disabled}
        >
          <span className="flex-1 min-w-0 truncate text-left">
            {selectedOption ? selectedOption.descricao : placeholder}
          </span>
          <ChevronsUpDown className="h-4 w-4 shrink-0 opacity-50" aria-hidden />
        </Button>
      </div>

      {listaOpen && !disabled && (
        <div
          ref={dropdownRef}
          className="absolute top-full left-0 right-0 z-50 mt-1 rounded-lg border border-border bg-popover text-popover-foreground shadow-md"
        >
          <div className="p-2 border-b border-border bg-popover">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="text"
                placeholder="Buscar..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyDown={(e) => e.stopPropagation()}
                className="h-9 pl-9 text-xs rounded-lg border-border"
                autoFocus
                autoComplete="off"
              />
            </div>
          </div>
          <ul className="max-h-[200px] overflow-auto py-1" role="listbox">
            {filteredOptions.length > 0 ? (
              filteredOptions.map((opt) => (
                <li
                  key={opt.id}
                  role="option"
                  className="cursor-pointer px-3 py-2 text-sm text-foreground transition-colors hover:bg-accent hover:text-accent-foreground"
                  onClick={() => handleSelect(opt.id)}
                >
                  {opt.descricao}
                </li>
              ))
            ) : (
              <li className="py-4 text-center text-xs text-muted-foreground italic">
                Nenhum resultado encontrado
              </li>
            )}
          </ul>
        </div>
      )}
    </div>
  )
}
