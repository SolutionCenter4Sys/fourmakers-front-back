import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { ScrollArea } from '@/components/ui/scroll-area'
import { Spinner } from '@/components/ui/spinner'
import { cn } from '@/lib/utils'
import type { Colaborador } from '@domain/entities/Colaborador'
import { Search, Users, X } from 'lucide-react'

export type ColaboradorSelectOption = {
  cpf?: string
  codColaborador?: string
  nome?: string
  email?: string
}

interface SelecaoColaboradoresProps {
  colaboradoresSelecionados: ColaboradorSelectOption[]
  colaboradores: Colaborador[]
  colaboradorSearch: string
  colaboradoresStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
  isPopoverOpen: boolean
  disabled?: boolean
  onColaboradorSelect: (colaborador: Colaborador) => void
  onColaboradorRemove: (cpf: string) => void
  onSearchChange: (search: string) => void
  onPopoverChange: (open: boolean) => void
  ownerColaboradorId?: string
}

export function SelecaoColaboradores({
  colaboradoresSelecionados,
  colaboradores,
  colaboradorSearch,
  colaboradoresStatus,
  isPopoverOpen,
  disabled = false,
  onColaboradorSelect,
  onColaboradorRemove,
  onSearchChange,
  onPopoverChange,
  ownerColaboradorId,
}: SelecaoColaboradoresProps) {
  return (
    <div className="space-y-2">
      <Label htmlFor="colaboradores">
        Colaboradores <span className="text-muted-foreground text-xs">({colaboradoresSelecionados.length} selecionados)</span>
      </Label>
      <Popover open={isPopoverOpen} onOpenChange={onPopoverChange}>
        <PopoverTrigger asChild>
          <Button
            variant="outline"
            role="combobox"
            className="w-full justify-between"
            disabled={disabled}
          >
            <span className="truncate">
              {colaboradoresSelecionados.length > 0
                ? `${colaboradoresSelecionados.length} colaborador(es) selecionado(s)`
                : 'Selecione os colaboradores...'}
            </span>
            <Users className="ml-2 h-4 w-4 shrink-0 opacity-50" />
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-[500px] p-0" align="start">
          <div className="flex items-center border-b px-3">
            <Search className="mr-2 h-4 w-4 shrink-0 opacity-50" />
            <Input
              placeholder="Buscar colaborador..."
              value={colaboradorSearch}
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
              {colaboradoresStatus === 'loading' ? (
                <div className="flex items-center justify-center py-8">
                  <Spinner size={16} className="text-primary" />
                  <span className="ml-2 text-sm text-muted-foreground">Carregando...</span>
                </div>
              ) : colaboradores.length === 0 ? (
                <div className="py-8 text-center text-sm text-muted-foreground">
                  Nenhum colaborador encontrado
                </div>
              ) : (
                colaboradores.map((colaborador, index) => {
                  const colaboradorId = colaborador.cpf || colaborador.codColaborador || `colaborador-${index}`
                  const isOwner = !!ownerColaboradorId && colaboradorId === ownerColaboradorId
                  const isSelected = colaboradoresSelecionados.some((c) =>
                    (c.cpf && colaborador.cpf && c.cpf === colaborador.cpf) ||
                    (c.codColaborador && colaborador.codColaborador && c.codColaborador === colaborador.codColaborador)
                  )
                  return (
                    <div
                      key={colaboradorId}
                      className={cn(
                        'flex items-center space-x-2 rounded-lg p-2 cursor-pointer',
                        isOwner ? 'opacity-90' : 'hover:bg-accent'
                      )}
                      onClick={() => {
                        if (isOwner) return
                        if (isSelected && colaboradorId) {
                          onColaboradorRemove(colaboradorId)
                        } else {
                          onColaboradorSelect(colaborador)
                        }
                      }}
                    >
                      <Checkbox checked={isSelected} disabled={isOwner} />
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2">
                          <span className="text-sm font-medium truncate">{colaborador.nome || 'Nome não informado'}</span>
                          {isOwner && (
                            <Badge variant="secondary" className="shrink-0 text-[10px] px-1.5 py-0 border-primary/30 bg-primary/5 text-primary">
                              Responsável
                            </Badge>
                          )}
                        </div>
                        <div className="text-xs text-muted-foreground truncate">
                          {colaborador.email || 'Email não informado'} • {colaborador.cpf || 'CPF não informado'}
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

      {/* Lista de colaboradores selecionados */}
      {colaboradoresSelecionados.length > 0 && (
        <div className="flex flex-wrap gap-2 mt-2">
          {colaboradoresSelecionados.map((colaborador, index) => {
            const colaboradorId = colaborador.cpf || colaborador.codColaborador || `colaborador-${index}`
            const isOwner = !!ownerColaboradorId && colaboradorId === ownerColaboradorId
            return (
              <div
                key={colaboradorId}
                className={cn(
                  'flex items-center gap-2 rounded-lg text-sm border px-2.5 py-1.5',
                  isOwner
                    ? 'border-primary/30 bg-primary/5 text-primaryText'
                    : 'border-borderSoft bg-secondary'
                )}
              >
                <span className="truncate max-w-[200px]">{colaborador.nome || 'Nome não informado'}</span>
                {isOwner ? (
                  <Badge variant="outline" className="shrink-0 text-[10px] px-1.5 py-0 border-primary/40 bg-primary/10 text-primary font-medium">
                    Responsável pela agenda
                  </Badge>
                ) : (
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    className="h-6 w-6 p-0 rounded-md hover:bg-destructive/10 hover:text-destructive text-muted-foreground"
                    onClick={() => colaboradorId && onColaboradorRemove(colaboradorId)}
                    aria-label="Remover colaborador"
                  >
                    <X className="h-3 w-3" />
                  </Button>
                )}
              </div>
            )
          })}
        </div>
      )}
    </div>
  )
}
