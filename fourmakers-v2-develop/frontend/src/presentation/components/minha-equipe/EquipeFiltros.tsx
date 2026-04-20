import { Search, X } from '@/components/ui/system-icons'
import { Input } from '@/components/ui/input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Label } from '@/components/ui/label'

interface EquipeFiltrosProps {
  searchQuery: string
  onSearchChange: (value: string) => void
  clienteFilter: string | null
  onClienteChange: (value: string | null) => void
  gestorFilter: string | null
  onGestorChange: (value: string | null) => void
  clientesUnicos: string[]
  gestoresUnicos: string[]
  filtrosResetKey: number
  onLimparFiltros: () => void
}

export const EquipeFiltros = ({
  searchQuery,
  onSearchChange,
  clienteFilter,
  onClienteChange,
  gestorFilter,
  onGestorChange,
  clientesUnicos,
  gestoresUnicos,
  filtrosResetKey,
  onLimparFiltros,
}: EquipeFiltrosProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-lg">Filtros</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {/* Linha 1: Busca por nome */}
        <div className="flex gap-2">
          <div className="flex-1">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar colaborador..."
                value={searchQuery}
                onChange={(e) => onSearchChange(e.target.value)}
                className="pl-10"
              />
            </div>
          </div>
        </div>

        {/* Linha 2: Filtros */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {/* Cliente */}
          <div className="space-y-2">
            <Label>Cliente</Label>
            <Select
              key={`cliente-${filtrosResetKey}`}
              value={clienteFilter || undefined}
              onValueChange={(value) => onClienteChange(value === 'todos' ? null : (value || null))}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Todos os clientes" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos os clientes</SelectItem>
                {clientesUnicos.length > 0 ? (
                  clientesUnicos.map((cliente) => (
                    <SelectItem key={cliente} value={cliente}>
                      {cliente}
                    </SelectItem>
                  ))
                ) : (
                  <SelectItem value="no-data" disabled>
                    Nenhum cliente disponível
                  </SelectItem>
                )}
              </SelectContent>
            </Select>
          </div>

          {/* Gestor */}
          <div className="space-y-2">
            <Label>Gestor</Label>
            <Select
              key={`gestor-${filtrosResetKey}`}
              value={gestorFilter || undefined}
              onValueChange={(value) => onGestorChange(value === 'todos' ? null : (value || null))}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Todos os gestores" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos os gestores</SelectItem>
                {gestoresUnicos.length > 0 ? (
                  gestoresUnicos.map((gestor) => (
                    <SelectItem key={gestor} value={gestor}>
                      {gestor}
                    </SelectItem>
                  ))
                ) : (
                  <SelectItem value="no-data" disabled>
                    Nenhum gestor disponível
                  </SelectItem>
                )}
              </SelectContent>
            </Select>
          </div>
        </div>

        {/* Linha 3: Limpar Filtros */}
        <div className="flex justify-end pt-2">
          <Button
            variant="outline"
            onClick={onLimparFiltros}
            className="gap-2"
          >
            <X className="h-4 w-4" />
            Limpar Filtros
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}
