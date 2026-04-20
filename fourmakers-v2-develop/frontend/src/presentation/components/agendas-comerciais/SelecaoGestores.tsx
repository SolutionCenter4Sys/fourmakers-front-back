import { Users, Search, X, Plus } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Input } from '@/components/ui/input'
import { Checkbox } from '@/components/ui/checkbox'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { ScrollArea } from '@/components/ui/scroll-area'
import { Spinner } from '@/components/ui/spinner'
import { cn } from '@/lib/utils'
import type { GestorExterno } from '@presentation/hooks/useGestoresExternos'

interface SelecaoGestoresProps {
  gestoresSelecionados: GestorExterno[]
  gestores: GestorExterno[]
  gestorSearch: string
  gestoresLoading: boolean
  isPopoverOpen: boolean
  clienteSelecionado: { id: string; name: string } | null
  error?: string
  disabled?: boolean
  onGestorSelect: (gestor: GestorExterno) => void
  onGestorRemove: (codGestorExterno: string) => void
  onSearchChange: (search: string) => void
  onPopoverChange: (open: boolean) => void
  onErrorClear?: () => void
  onAdicionarGestor?: () => void
}

export function SelecaoGestores({
  gestoresSelecionados,
  gestores,
  gestorSearch,
  gestoresLoading,
  isPopoverOpen,
  clienteSelecionado,
  error,
  disabled = false,
  onGestorSelect,
  onGestorRemove,
  onSearchChange,
  onPopoverChange,
  onErrorClear,
  onAdicionarGestor,
}: SelecaoGestoresProps) {
  if (!clienteSelecionado) {
    return null
  }

  return (
    <div className="space-y-2">
      <Label htmlFor="gestoresExternos">
        Gestores do Cliente <span className="text-destructive">*</span>{' '}
        <span className="text-muted-foreground text-xs">({gestoresSelecionados.length} selecionados)</span>
      </Label>
      <div className="flex items-center gap-2">
        <div className="flex-1">
          <Popover open={isPopoverOpen} onOpenChange={onPopoverChange}>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                role="combobox"
                className={cn('w-full justify-between', error && 'border-destructive')}
                disabled={!clienteSelecionado || disabled}
              >
                <span className="truncate">
                  {gestoresSelecionados.length > 0
                    ? `${gestoresSelecionados.length} gestor(es) selecionado(s)`
                    : 'Selecione os gestores externos...'}
                </span>
                <Users className="ml-2 h-4 w-4 shrink-0 opacity-50" />
              </Button>
            </PopoverTrigger>
        <PopoverContent className="w-[500px] p-0" align="start">
          <div className="flex items-center border-b px-3">
            <Search className="mr-2 h-4 w-4 shrink-0 opacity-50" />
            <Input
              placeholder="Buscar gestor..."
              value={gestorSearch}
              onChange={(e) => onSearchChange(e.target.value)}
              className="border-0 focus-visible:ring-0"
            />
          </div>
          <ScrollArea className="h-[300px]">
            <div 
              className="p-2"
              onWheel={(e) => {
                // Permitir scroll com roda do mouse
                e.stopPropagation()
              }}
            >
              {gestoresLoading ? (
                <div className="flex items-center justify-center py-8">
                  <Spinner size={16} className="text-primary" />
                  <span className="ml-2 text-sm text-muted-foreground">Carregando...</span>
                </div>
              ) : gestores.length === 0 ? (
                <div className="py-8 text-center text-sm text-muted-foreground">
                  {clienteSelecionado
                    ? gestorSearch.trim()
                      ? 'Nenhum gestor encontrado'
                      : 'Nenhum gestor disponível para este cliente'
                    : 'Selecione um cliente primeiro'}
                </div>
              ) : (
                gestores.map((gestor) => {
                  if (!gestor) {
                    return null // Validação defensiva - pular gestores null/undefined
                  }
                  // Verificar se o gestor está selecionado comparando codGestorExterno
                  // Permitir também gestores temporários (que podem ter tempId)
                  const isSelected = gestoresSelecionados.some(
                    (g) => {
                      if (!g?.codGestorExterno || !gestor?.codGestorExterno) {
                        return false
                      }
                      return g.codGestorExterno === gestor.codGestorExterno
                    },
                  )
                  return (
                    <div
                      key={gestor.codGestorExterno}
                      className="flex items-center space-x-2 rounded-lg p-2 hover:bg-accent cursor-pointer"
                      onClick={() => {
                        if (isSelected && gestor.codGestorExterno) {
                          onGestorRemove(gestor.codGestorExterno)
                        } else {
                          onGestorSelect(gestor)
                        }
                        if (onErrorClear && error) {
                          onErrorClear()
                        }
                      }}
                    >
                      <Checkbox checked={isSelected} />
                      <div className="flex-1 min-w-0">
                        <div className="text-sm font-medium truncate">{gestor.nome || 'Nome não informado'}</div>
                        <div className="text-xs text-muted-foreground truncate">
                          {gestor.email || 'Email não informado'} • {gestor.codGestorExterno}
                        </div>
                      </div>
                    </div>
                  )
                })
              )}
            </div>
          </ScrollArea>
        </PopoverContent>
      </Popover>
        </div>
        {onAdicionarGestor && (
          <Button
            type="button"
            variant="outline"
            size="icon"
            onClick={onAdicionarGestor}
            className="shrink-0 rounded-lg h-10 w-10"
            title="Adicionar gestor externo"
            disabled={disabled}
          >
            <Plus className="w-5 h-5" />
          </Button>
        )}
      </div>

      {/* Lista de gestores selecionados */}
      {gestoresSelecionados.length > 0 && (
        <div className="flex flex-wrap gap-2 mt-2">
          {gestoresSelecionados.map((gestor, index) => {
            // Usar codGestorExterno ou criar um key único baseado no índice e nome
            const gestorKey = gestor?.codGestorExterno || `gestor-${index}-${gestor?.nome || 'sem-nome'}`
            if (!gestor) {
              return null // Validação defensiva
            }
            return (
              <div
                key={gestorKey}
                className="flex items-center gap-1 px-2 py-1 bg-secondary rounded-md text-sm"
              >
                <span className="truncate max-w-[200px]">{gestor.nome || 'Nome não informado'}</span>
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  className="h-5 w-5 p-0 hover:bg-destructive hover:text-destructive-foreground"
                  onClick={() => {
                    // Usar codGestorExterno se disponível, senão usar o key
                    const idToRemove = gestor.codGestorExterno || gestorKey
                    onGestorRemove(idToRemove)
                  }}
                >
                  <X className="h-3 w-3" />
                </Button>
              </div>
            )
          })}
        </div>
      )}
      {error && <p className="text-sm text-destructive">{error}</p>}
    </div>
  )
}
