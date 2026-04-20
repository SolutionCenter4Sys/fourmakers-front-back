import { useState, useEffect, useMemo, useCallback, useRef } from 'react'
import { Search, ChevronsUpDown, Plus, Edit } from 'lucide-react'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'

const IconEditManual = () => (
  <svg width="20" height="20" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
    <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7" />
    <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z" />
  </svg>
)

interface AutocompleteGenericoMapaProps<T extends Record<string, unknown>> {
  label: string
  icon: React.ReactNode
  value: string
  options: T[]
  onSelect: (value: string) => void
  onToggleManual: () => void
  isManual: boolean
  placeholder?: string
  displayKey: keyof T
  secondaryKey?: keyof T
  idKey: keyof T
  required?: boolean
  onCreateNew?: () => void
  onEdit?: () => void
}

export function AutocompleteGenericoMapa<T extends Record<string, unknown>>({
  label,
  icon,
  value,
  options,
  onSelect,
  onToggleManual,
  isManual,
  placeholder = 'Selecione...',
  displayKey,
  secondaryKey,
  idKey,
  required,
  onCreateNew,
  onEdit,
}: AutocompleteGenericoMapaProps<T>) {
  const wrapperRef = useRef<HTMLDivElement>(null)
  const dropdownRef = useRef<HTMLDivElement>(null)

  const [searchTerm, setSearchTerm] = useState('')
  const [listaOpen, setListaOpen] = useState(false)

  const validOptions = useMemo(
    () =>
      options.filter((opt) => {
        const displayVal = opt[displayKey]
        return displayVal && String(displayVal).trim() !== ''
      }),
    [options, displayKey]
  )

  const filteredOptions = useMemo(() => {
    const term = searchTerm.toLowerCase()
    return validOptions.filter((opt) => {
      const displayVal = String(opt[displayKey] || '').toLowerCase()
      const secondaryVal = secondaryKey ? String(opt[secondaryKey] || '').toLowerCase() : ''
      return displayVal.includes(term) || secondaryVal.includes(term)
    })
  }, [validOptions, searchTerm, displayKey, secondaryKey])

  const selectedOption = useMemo(
    () => validOptions.find((opt) => String(opt[idKey]) === value),
    [validOptions, value, idKey]
  )

  const valueStr = value ? String(value).trim() : ''
  const isEditDisabled = !valueStr || !selectedOption

  const handleSelect = useCallback(
    (optValue: string) => {
      onSelect(optValue)
      setListaOpen(false)
      setSearchTerm('')
    },
    [onSelect]
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

  if (isManual) {
    return (
      <div className="w-full">
        <Label
          className={cn(
            'text-sm font-bold text-foreground mb-2 block',
            required && "after:content-['*'] after:ml-0.5 after:text-destructive"
          )}
        >
          {label}
        </Label>
        <div className="flex items-center gap-2">
          <div className="relative flex-1">
            <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground">
              <IconEditManual />
            </div>
            <Input
              className="w-full h-10 pl-12 pr-4 text-sm font-medium"
              value={value}
              onChange={(e) => onSelect(e.target.value)}
              placeholder={`Digite ${label.toLowerCase()}...`}
              autoFocus
            />
          </div>
          <Button
            type="button"
            size="icon"
            onClick={onToggleManual}
            className="shrink-0 rounded-lg"
            title="Voltar para seleção"
          >
            {icon}
          </Button>
        </div>
      </div>
    )
  }

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
      <div className="flex items-center gap-2">
        <div className="relative flex-1">
          <Button
            type="button"
            variant="outline"
            role="combobox"
            aria-expanded={listaOpen}
            className="h-10 pl-12 pr-10 rounded-lg w-full justify-between font-bold text-sm border-border"
            onClick={() => setListaOpen((o) => !o)}
          >
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none">
              {icon}
            </span>
            <span className="flex-1 min-w-0 truncate text-left">
              {selectedOption ? String(selectedOption[displayKey]) : placeholder}
            </span>
            <ChevronsUpDown className="h-4 w-4 shrink-0 opacity-50" aria-hidden />
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
                      key={String(opt[idKey])}
                      role="option"
                      className="cursor-pointer px-3 py-2 text-sm text-foreground transition-colors hover:bg-accent hover:text-accent-foreground"
                      onClick={() => handleSelect(String(opt[idKey]))}
                    >
                      <div className="flex flex-col">
                        <span className="text-xs font-bold">{String(opt[displayKey])}</span>
                        {secondaryKey && opt[secondaryKey] && (
                          <span className="text-xs text-muted-foreground">
                            {String(opt[secondaryKey])}
                          </span>
                        )}
                      </div>
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
        {!isManual && (
          <>
            {onCreateNew && (
              <Button
                type="button"
                size="icon"
                variant="outline"
                onClick={onCreateNew}
                className="shrink-0 rounded-lg"
                title="Criar Novo"
              >
                <Plus className="w-5 h-5" />
              </Button>
            )}
            {onEdit && (
              <Button
                type="button"
                size="icon"
                variant={!isEditDisabled ? 'outline' : 'ghost'}
                onClick={(e) => {
                  e.preventDefault()
                  e.stopPropagation()
                  if (!isEditDisabled) {
                    onEdit()
                  }
                }}
                disabled={isEditDisabled}
                className={cn(
                  'shrink-0 rounded-lg transition-all',
                  isEditDisabled &&
                    'opacity-25 cursor-not-allowed !bg-muted/50 !border-muted-foreground/30 hover:!bg-muted/50 hover:!opacity-25 hover:!cursor-not-allowed !pointer-events-none'
                )}
                title={!isEditDisabled ? 'Editar' : 'Selecione um item para editar'}
              >
                <Edit
                  className={cn('w-5 h-5 transition-all', isEditDisabled && '!text-muted-foreground/50')}
                />
              </Button>
            )}
          </>
        )}
      </div>
    </div>
  )
}
