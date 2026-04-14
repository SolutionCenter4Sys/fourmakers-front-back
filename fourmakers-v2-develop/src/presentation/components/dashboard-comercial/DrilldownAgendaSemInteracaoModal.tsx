/**
 * Modal de drilldown "Agendas Sem Interação" do Dashboard Comercial.
 * Exibe lista de agendas sem interação registrada conforme endpoint AgendaSemInteracaoDetalhe.
 */
import { useState, useMemo, useCallback } from 'react'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from '@/components/ui/dialog'
import { Badge } from '@/components/ui/badge'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table'
import { Spinner } from '@/components/ui/spinner'
import type { FiltrosComercial, AgendaSemInteracaoDetalhe } from '@shared/types/dashboardComercialTypes'
import { obterLabelPeriodo, obterLabelCliente, obterLabelComercial, formatarApenasDataDrilldown } from '@shared/utils/dashboardComercialLabels'

export interface DrilldownAgendaSemInteracaoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  titulo: string
  comoCalculamos: string
  filtrosAtivos: FiltrosComercial
  dados: AgendaSemInteracaoDetalhe[] | null
  carregando: boolean
  erro: string | null
}

export function DrilldownAgendaSemInteracaoModal({
  open,
  onOpenChange,
  titulo,
  comoCalculamos,
  filtrosAtivos,
  dados,
  carregando,
  erro,
}: DrilldownAgendaSemInteracaoModalProps) {
  const [termoBusca, setTermoBusca] = useState('')

  const listaBase = useMemo(() => dados ?? [], [dados])
  const listaFiltrada = useMemo(() => {
    const termoNorm = termoBusca.trim().toLowerCase()
    if (!termoNorm) return listaBase
    return listaBase.filter((item) => {
      const tituloItem = (item.titulo ?? '').toLowerCase()
      const nomeCliente = (item.nomeCliente ?? '').toLowerCase()
      const tipo = (item.tipoAgendaDescricao ?? '').toLowerCase()
      return tituloItem.includes(termoNorm) || nomeCliente.includes(termoNorm) || tipo.includes(termoNorm)
    })
  }, [listaBase, termoBusca])

  const handleOpenChange = useCallback(
    (next: boolean) => {
      if (!next) setTermoBusca('')
      onOpenChange(next)
    },
    [onOpenChange],
  )

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{titulo}</DialogTitle>
          <DialogDescription>
            Listagem de agendas sem interação registrada no período. Use a busca para filtrar por título, cliente ou tipo.
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
                <p className="text-sm text-muted-foreground">Carregando agendas sem interação...</p>
              </div>
            </div>
          ) : erro ? (
            <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-4">
              <p className="text-sm text-destructive">{erro}</p>
            </div>
          ) : (
            <>
              <div className="space-y-2">
                <Label htmlFor="drilldown-busca-sem-interacao">Buscar por título, cliente ou tipo</Label>
                <Input
                  id="drilldown-busca-sem-interacao"
                  type="search"
                  placeholder="Título, cliente ou tipo..."
                  value={termoBusca}
                  onChange={(e) => setTermoBusca(e.target.value)}
                  className="rounded-lg"
                  aria-label="Buscar agendas sem interação"
                />
              </div>

              <div>
                <h4 className="mb-2 text-sm font-semibold text-foreground">Agendas sem interação</h4>
                <div className="rounded-md border border-border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Título</th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Data agendada</th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Nome cliente</th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">Tipo agenda</th>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {listaFiltrada.length === 0 ? (
                        <TableRow>
                          <TableCell colSpan={4} className="h-24 text-center text-muted-foreground">
                            Nenhum registro encontrado
                          </TableCell>
                        </TableRow>
                      ) : (
                        listaFiltrada.map((item) => (
                          <TableRow key={item.agendaId}>
                            <TableCell className="font-medium">{item.titulo ?? '—'}</TableCell>
                            <TableCell className="text-muted-foreground">{formatarApenasDataDrilldown(item.dataAgendada)}</TableCell>
                            <TableCell>{item.nomeCliente ?? '—'}</TableCell>
                            <TableCell className="text-muted-foreground">{item.tipoAgendaDescricao ?? '—'}</TableCell>
                          </TableRow>
                        ))
                      )}
                    </TableBody>
                  </Table>
                </div>
              </div>
            </>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
