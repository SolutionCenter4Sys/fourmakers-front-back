/**
 * Modal de drilldown "Categorias com Interação" do Dashboard Comercial.
 * Exibe dados agrupados por categoria conforme endpoint CategoriaComInteracaoDetalhe.
 */
import { useCallback } from 'react'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from '@/components/ui/dialog'
import { Badge } from '@/components/ui/badge'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table'
import { Spinner } from '@/components/ui/spinner'
import type { FiltrosComercial, CategoriaComInteracaoDetalhe } from '@shared/types/dashboardComercialTypes'
import { obterLabelPeriodo, obterLabelCliente, obterLabelComercial, formatarApenasDataDrilldown } from '@shared/utils/dashboardComercialLabels'

export interface DrilldownCategoriaComInteracaoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  titulo: string
  comoCalculamos: string
  filtrosAtivos: FiltrosComercial
  dados: CategoriaComInteracaoDetalhe[] | null
  carregando: boolean
  erro: string | null
}

export function DrilldownCategoriaComInteracaoModal({
  open,
  onOpenChange,
  titulo,
  comoCalculamos,
  filtrosAtivos,
  dados,
  carregando,
  erro,
}: DrilldownCategoriaComInteracaoModalProps) {
  const handleOpenChange = useCallback((next: boolean) => onOpenChange(next), [onOpenChange])

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{titulo}</DialogTitle>
          <DialogDescription>
            Listagem de categorias com interação e suas agendas no período.
          </DialogDescription>
        </DialogHeader>

        <div className="mt-4 space-y-4">
          <div className="flex flex-wrap gap-2">
            <Badge variant="secondary">{obterLabelPeriodo(filtrosAtivos)}</Badge>
            <Badge variant="secondary">{obterLabelCliente(filtrosAtivos.codigoCliente, filtrosAtivos.nomeClienteSelecionado)}</Badge>
            <Badge variant="secondary">{obterLabelComercial(filtrosAtivos)}</Badge>
          </div>

          <div className="rounded-lg border border-border bg-muted/30 p-4">
            <h4 className="mb-2 text-sm font-semibold text-foreground">Como calculamos</h4>
            <p className="text-sm text-muted-foreground">{comoCalculamos}</p>
          </div>

          {carregando ? (
            <div className="flex flex-1 items-center justify-center rounded-lg border border-border bg-muted/30 p-12">
              <div className="flex flex-col items-center gap-3">
                <Spinner size={32} className="text-primary" />
                <p className="text-sm text-muted-foreground">Carregando categorias com interação...</p>
              </div>
            </div>
          ) : erro ? (
            <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-4">
              <p className="text-sm text-destructive">{erro}</p>
            </div>
          ) : !dados?.length ? (
            <div className="rounded-md border border-border p-8 text-center text-muted-foreground">
              Nenhuma categoria com interação encontrada
            </div>
          ) : (
            <div className="space-y-6">
              {dados.map((grupo) => (
                <div key={grupo.categoriaId} className="rounded-lg border border-border bg-muted/20 p-4">
                  <div className="mb-3 flex items-baseline gap-2">
                    <span className="font-semibold text-foreground">{grupo.categoriaDescricao ?? '—'}</span>
                    <Badge variant="secondary" className="text-xs">
                      {(grupo.agendas?.length ?? 0)} agenda(s)
                    </Badge>
                  </div>
                  {!grupo.agendas?.length ? (
                    <p className="text-sm text-muted-foreground">Nenhuma agenda nesta categoria</p>
                  ) : (
                    <div className="rounded-md border border-border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Título</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Cliente</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Data</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Tipo</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Subcategoria</th>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {grupo.agendas.map((agenda) => (
                            <TableRow key={agenda.agendaId}>
                              <TableCell className="font-medium">{agenda.titulo ?? '—'}</TableCell>
                              <TableCell>{agenda.nomeCliente ?? '—'}</TableCell>
                              <TableCell className="text-muted-foreground">{formatarApenasDataDrilldown(agenda.dataAgendada)}</TableCell>
                              <TableCell className="text-muted-foreground">{agenda.tipoAgendaDescricao ?? '—'}</TableCell>
                              <TableCell className="text-muted-foreground">{agenda.subcategoriaDescricao ?? '—'}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
