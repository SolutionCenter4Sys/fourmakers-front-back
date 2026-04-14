/**
 * Modal de drilldown "Clientes Impactados" do Dashboard Comercial.
 * Exibe lista de clientes agrupados com suas agendas; busca opcional por nome do cliente.
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
import type { FiltrosComercial, ClienteImpactadoDetalhe } from '@shared/types/dashboardComercialTypes'
import { obterLabelPeriodo, obterLabelCliente, obterLabelComercial, formatarApenasDataDrilldown } from '@shared/utils/dashboardComercialLabels'

function filtrarClientesPorTermo(
  lista: ClienteImpactadoDetalhe[],
  termo: string
): ClienteImpactadoDetalhe[] {
  const termoNorm = termo.trim().toLowerCase()
  if (!termoNorm) return lista
  return lista.filter((c) => {
    const nome = (c.nomeCliente ?? '').toLowerCase()
    const codigo = (c.codigoCliente ?? '').toLowerCase()
    return nome.includes(termoNorm) || codigo.includes(termoNorm)
  })
}

export interface DrilldownClientesImpactadosModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  titulo: string
  comoCalculamos: string
  filtrosAtivos: FiltrosComercial
  dadosClientesImpactados: ClienteImpactadoDetalhe[] | null
  carregando: boolean
  erro: string | null
}

export function DrilldownClientesImpactadosModal({
  open,
  onOpenChange,
  titulo,
  comoCalculamos,
  filtrosAtivos,
  dadosClientesImpactados,
  carregando,
  erro,
}: DrilldownClientesImpactadosModalProps) {
  const [termoBusca, setTermoBusca] = useState('')

  const listaBase = useMemo(() => dadosClientesImpactados ?? [], [dadosClientesImpactados])
  const clientesFiltrados = useMemo(
    () => filtrarClientesPorTermo(listaBase, termoBusca),
    [listaBase, termoBusca]
  )

  const handleOpenChange = useCallback(
    (next: boolean) => {
      if (!next) setTermoBusca('')
      onOpenChange(next)
    },
    [onOpenChange]
  )

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{titulo}</DialogTitle>
          <DialogDescription>
            Listagem de clientes impactados no período, com suas agendas. Use a busca para filtrar por nome ou código do cliente.
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
                <p className="text-sm text-muted-foreground">Carregando clientes impactados...</p>
              </div>
            </div>
          ) : erro ? (
            <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-4">
              <p className="text-sm text-destructive">{erro}</p>
            </div>
          ) : (
            <>
              <div className="space-y-2">
                <Label htmlFor="drilldown-busca-clientes">Buscar por nome ou código do cliente</Label>
                <Input
                  id="drilldown-busca-clientes"
                  type="search"
                  placeholder="Nome ou código do cliente..."
                  value={termoBusca}
                  onChange={(e) => setTermoBusca(e.target.value)}
                  className="rounded-lg"
                  aria-label="Buscar clientes por nome ou código"
                />
              </div>

              <div>
                <h4 className="mb-2 text-sm font-semibold text-foreground">Clientes impactados</h4>
                {clientesFiltrados.length === 0 ? (
                  <div className="rounded-md border border-border p-8 text-center text-muted-foreground">
                    Nenhum cliente encontrado
                  </div>
                ) : (
                  <div className="space-y-6">
                    {clientesFiltrados.map((cliente) => (
                      <div
                        key={cliente.codigoCliente}
                        className="rounded-lg border border-border bg-muted/20 p-4"
                      >
                        <div className="mb-3 flex items-baseline gap-2">
                          <span className="font-semibold text-foreground">
                            {cliente.nomeCliente ?? '—'}
                          </span>
                          <Badge variant="secondary" className="text-xs">
                            {cliente.agendas?.length ?? 0} agenda{(cliente.agendas?.length ?? 0) !== 1 ? 's' : ''}
                          </Badge>
                        </div>
                        {!cliente.agendas?.length ? (
                          <p className="text-sm text-muted-foreground">Nenhuma agenda no período</p>
                        ) : (
                          <div className="rounded-md border border-border">
                            <Table>
                              <TableHeader>
                                <TableRow>
                                  <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                                    Título
                                  </th>
                                  <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                                    Data
                                  </th>
                                  <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                                    Tipo
                                  </th>
                                </TableRow>
                              </TableHeader>
                              <TableBody>
                                {cliente.agendas.map((agenda) => (
                                  <TableRow key={agenda.agendaId}>
                                    <TableCell className="font-medium">
                                      {agenda.titulo ?? '—'}
                                    </TableCell>
                                    <TableCell className="text-muted-foreground">
                                      {formatarApenasDataDrilldown(agenda.dataAgendada)}
                                    </TableCell>
                                    <TableCell className="text-muted-foreground">
                                      {agenda.tipoAgendaDescricao ?? '—'}
                                    </TableCell>
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
            </>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
