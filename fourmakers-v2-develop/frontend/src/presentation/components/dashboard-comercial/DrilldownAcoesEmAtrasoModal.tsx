/**
 * Modal de drilldown "Ações em Atraso" do Dashboard Comercial.
 * Exibe dados agrupados por status conforme endpoint AcoesEmAtrasoDetalhe.
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
import type { FiltrosComercial, AcoesEmAtrasoDetalhe } from '@shared/types/dashboardComercialTypes'
import { obterLabelPeriodo, obterLabelCliente, obterLabelComercial, formatarApenasDataDrilldown } from '@shared/utils/dashboardComercialLabels'

export interface DrilldownAcoesEmAtrasoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  titulo: string
  comoCalculamos: string
  filtrosAtivos: FiltrosComercial
  dados: AcoesEmAtrasoDetalhe[] | null
  carregando: boolean
  erro: string | null
}

export function DrilldownAcoesEmAtrasoModal({
  open,
  onOpenChange,
  titulo,
  comoCalculamos,
  filtrosAtivos,
  dados,
  carregando,
  erro,
}: DrilldownAcoesEmAtrasoModalProps) {
  const handleOpenChange = useCallback((next: boolean) => onOpenChange(next), [onOpenChange])

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{titulo}</DialogTitle>
          <DialogDescription>
            Listagem de ações em atraso agrupadas por status. Cada agenda pode ter múltiplas ações.
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
                <p className="text-sm text-muted-foreground">Carregando ações em atraso...</p>
              </div>
            </div>
          ) : erro ? (
            <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-4">
              <p className="text-sm text-destructive">{erro}</p>
            </div>
          ) : !dados?.length ? (
            <div className="rounded-md border border-border p-8 text-center text-muted-foreground">
              Nenhuma ação em atraso encontrada
            </div>
          ) : (
            <div className="space-y-6">
              {dados.map((grupo) => (
                <div key={grupo.statusAcaoId} className="rounded-lg border border-border bg-muted/20 p-4">
                  <div className="mb-3 flex items-baseline gap-2">
                    <span className="font-semibold text-foreground">{grupo.statusDescricao ?? '—'}</span>
                    <Badge variant="secondary" className="text-xs">
                      {(grupo.agendas?.length ?? 0)} registro(s)
                    </Badge>
                  </div>
                  {!grupo.agendas?.length ? (
                    <p className="text-sm text-muted-foreground">Nenhuma agenda neste status</p>
                  ) : (
                    <div className="rounded-md border border-border">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Ação</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Cliente</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Data agenda</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Tipo</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Título da Agenda</th>
                            <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Data limite ação</th>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {grupo.agendas.map((agenda, idx) => (
                            <TableRow key={`${agenda.agendaId}-${agenda.acaoId ?? idx}`}>
                              <TableCell className="font-medium">{agenda.descricaoAcao ?? '—'}</TableCell>
                              <TableCell>{agenda.nomeCliente ?? '—'}</TableCell>
                              <TableCell className="text-muted-foreground">{formatarApenasDataDrilldown(agenda.dataAgendada)}</TableCell>
                              <TableCell className="text-muted-foreground">{agenda.tipoAgendaDescricao ?? '—'}</TableCell>
                              <TableCell className="text-muted-foreground">{agenda.titulo ?? '—'}</TableCell>
                              <TableCell className="text-muted-foreground">{formatarApenasDataDrilldown(agenda.dataLimiteAcao)}</TableCell>
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
