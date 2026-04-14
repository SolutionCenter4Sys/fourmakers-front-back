import { useState, useMemo } from 'react'
import {
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip'
import { MoreHorizontal, Target, ArrowUp, ArrowDown, ArrowUpDown } from '@/components/ui/system-icons'
import type { MinhaEquipeColaborador } from '@domain/entities/MinhaEquipeColaborador'
import { useDragScroll } from '@presentation/hooks/useDragScroll'
import { cn } from '@/lib/utils'

interface EquipeTableProps {
  colaboradores: MinhaEquipeColaborador[]
  onOpenRadar: (colaborador: MinhaEquipeColaborador) => void
  isLoading?: boolean
}

type SortColumn = 'nomeColaborador' | 'nomeCliente' | 'perfil' | 'nomeGestorOperacional' | 'match' | 'nomeGestorCliente' | null
type SortDirection = 'asc' | 'desc' | null

export const EquipeTable = ({
  colaboradores,
  onOpenRadar,
  isLoading,
}: EquipeTableProps) => {
  // Hook para drag-to-scroll horizontal
  const scrollRef = useDragScroll<HTMLDivElement>()
  
  // Estado de ordenação
  const [sortColumn, setSortColumn] = useState<SortColumn>(null)
  const [sortDirection, setSortDirection] = useState<SortDirection>(null)

  const getMatchColor = (match: number) => {
    if (match >= 71) return 'bg-success/10 text-success border-success'
    if (match >= 41) return 'bg-warning/10 text-warning border-warning'
    return 'bg-destructive/10 text-destructive border-destructive'
  }

  // Dados ordenados
  const sortedColaboradores = useMemo(() => {
    if (!sortColumn || !sortDirection) {
      return colaboradores
    }

    // Função para obter valor de ordenação de um colaborador
    const getSortValue = (colab: MinhaEquipeColaborador, column: SortColumn): string | number | null => {
      if (!column) return null
      
      switch (column) {
        case 'nomeColaborador':
          return colab.nomeColaborador || null
        case 'nomeCliente':
          return colab.nomeCliente || null
        case 'perfil':
          return colab.perfil || null
        case 'nomeGestorOperacional':
          return colab.gestoresOperacionais?.[0] || colab.nomeGestorOperacional || null
        case 'match':
          return colab.match ?? null
        case 'nomeGestorCliente':
          return colab.nomeGestorCliente || null
        default:
          return null
      }
    }

    // Função de comparação segura que trata valores nulos
    const compareValues = (a: string | number | null, b: string | number | null): number => {
      // Ambos nulos: iguais
      if (a === null && b === null) return 0
      
      // A é nulo: vai para o final
      if (a === null) return 1
      
      // B é nulo: vai para o final
      if (b === null) return -1
      
      // Comparação de strings
      if (typeof a === 'string' && typeof b === 'string') {
        return a.localeCompare(b, 'pt-BR', { sensitivity: 'base' })
      }
      
      // Comparação de números
      if (typeof a === 'number' && typeof b === 'number') {
        return a - b
      }
      
      // Comparação mista: converter para string
      return String(a).localeCompare(String(b), 'pt-BR', { sensitivity: 'base' })
    }

    return [...colaboradores].sort((a, b) => {
      const aValue = getSortValue(a, sortColumn)
      const bValue = getSortValue(b, sortColumn)
      const comparison = compareValues(aValue, bValue)
      return sortDirection === 'asc' ? comparison : -comparison
    })
  }, [colaboradores, sortColumn, sortDirection])

  // Handler para ordenação
  const handleSort = (column: SortColumn) => {
    if (sortColumn === column) {
      // Se já está ordenando por esta coluna, alternar direção
      if (sortDirection === 'asc') {
        setSortDirection('desc')
      } else if (sortDirection === 'desc') {
        // Se já está em desc, remover ordenação
        setSortColumn(null)
        setSortDirection(null)
      }
    } else {
      // Se é uma nova coluna, começar com ascendente
      setSortColumn(column)
      setSortDirection('asc')
    }
  }

  // Componente de header com ordenação
  const SortableTableHead = ({ 
    column, 
    label, 
    className 
  }: { 
    column: SortColumn
    label: string
    className?: string 
  }) => {
    const isSorted = sortColumn === column
    const SortIcon = isSorted 
      ? (sortDirection === 'asc' ? ArrowUp : ArrowDown)
      : ArrowUpDown

    return (
      <TableHead
        className={cn(
          'whitespace-nowrap cursor-pointer select-none hover:bg-muted/50 transition-colors',
          className
        )}
        onClick={(e) => {
          e.stopPropagation()
          handleSort(column)
        }}
        onMouseDown={(e) => {
          // Prevenir que o drag-to-scroll capture este evento
          e.stopPropagation()
        }}
      >
        <div className="flex items-center gap-2">
          <span>{label}</span>
          <SortIcon className={cn(
            'h-4 w-4 text-muted-foreground',
            isSorted && 'text-foreground'
          )} />
        </div>
      </TableHead>
    )
  }

  if (isLoading) {
    return (
      <div className="border rounded-lg overflow-hidden bg-card">
        <div ref={scrollRef} className="overflow-x-auto">
          <table className="w-full caption-bottom text-sm" style={{ minWidth: '800px' }}>
            <TableHeader>
              <TableRow>
                <TableHead>Nome</TableHead>
                <TableHead>Cliente</TableHead>
                <TableHead>Perfil</TableHead>
                <TableHead>Gestor Operacional</TableHead>
                <TableHead>Match</TableHead>
                <TableHead>Gestor Cliente</TableHead>
                <TableHead className="text-right">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {[1, 2, 3].map((i) => (
                <TableRow key={i}>
                  <TableCell>
                    <div className="h-4 bg-muted animate-pulse rounded w-32" />
                  </TableCell>
                  <TableCell>
                    <div className="h-4 bg-muted animate-pulse rounded w-24" />
                  </TableCell>
                  <TableCell>
                    <div className="h-4 bg-muted animate-pulse rounded w-28" />
                  </TableCell>
                  <TableCell>
                    <div className="h-4 bg-muted animate-pulse rounded w-28" />
                  </TableCell>
                  <TableCell>
                    <div className="h-4 bg-muted animate-pulse rounded w-16" />
                  </TableCell>
                  <TableCell>
                    <div className="h-4 bg-muted animate-pulse rounded w-24" />
                  </TableCell>
                  <TableCell className="text-right">
                    <div className="h-8 bg-muted animate-pulse rounded w-8 ml-auto" />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </table>
        </div>
      </div>
    )
  }

  if (colaboradores.length === 0) {
    return (
      <div className="border rounded-lg p-8 text-center text-muted-foreground bg-card">
        Nenhum colaborador encontrado
      </div>
    )
  }

  return (
    <div className="border rounded-lg overflow-hidden bg-card">
      <div ref={scrollRef} className="overflow-x-auto">
        <table className="w-full caption-bottom text-sm" style={{ minWidth: '800px' }}>
          <TableHeader>
            <TableRow>
              <SortableTableHead column="nomeColaborador" label="Nome" />
              <SortableTableHead column="nomeCliente" label="Cliente" />
              <SortableTableHead column="perfil" label="Perfil" />
              <SortableTableHead column="nomeGestorOperacional" label="Gestor Operacional" />
              <SortableTableHead column="match" label="Match" />
              <SortableTableHead column="nomeGestorCliente" label="Gestor Cliente" />
              <TableHead className="text-right whitespace-nowrap">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {sortedColaboradores.map((colab) => (
              <TableRow key={colab.id}>
                <TableCell className="font-medium whitespace-nowrap">
                  {colab.nomeColaborador}
                </TableCell>
                <TableCell className="whitespace-nowrap">{colab.nomeCliente}</TableCell>
                <TableCell className="whitespace-nowrap">{colab.perfil}</TableCell>
                <TableCell className="whitespace-nowrap">
                  <GestorOperacionalCell colaborador={colab} />
                </TableCell>
                <TableCell className="whitespace-nowrap">
                  <Badge className={getMatchColor(colab.match)}>
                    {colab.match}%
                  </Badge>
                </TableCell>
                <TableCell className="whitespace-nowrap">{colab.nomeGestorCliente}</TableCell>
                <TableCell className="text-right whitespace-nowrap">
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => onOpenRadar(colab)}
                    className="gap-2"
                  >
                    <Target className="h-4 w-4" />
                    Radar
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </table>
      </div>
    </div>
  )
}

interface GestorOperacionalCellProps {
  colaborador: MinhaEquipeColaborador
}

const GestorOperacionalCell = ({ colaborador }: GestorOperacionalCellProps) => {
  const gestoresRaw = colaborador.gestoresOperacionais?.filter(Boolean) ?? []
  const gestoresUnicos = Array.from(new Set(gestoresRaw))
  const primaryGestor =
    gestoresUnicos[0] || colaborador.nomeGestorOperacional || 'Não informado'
  const hasMultipleGestores = gestoresUnicos.length > 1

  if (!hasMultipleGestores) {
    return <span>{primaryGestor}</span>
  }

  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <span className="flex items-center gap-1 cursor-help">
          {primaryGestor}
          <MoreHorizontal className="h-4 w-4 text-muted-foreground" />
        </span>
      </TooltipTrigger>
      <TooltipContent>
        <div className="space-y-1">
          {gestoresUnicos.map((gestor) => (
            <p key={gestor} className="text-sm text-foreground">
              {gestor}
            </p>
          ))}
        </div>
      </TooltipContent>
    </Tooltip>
  )
}
