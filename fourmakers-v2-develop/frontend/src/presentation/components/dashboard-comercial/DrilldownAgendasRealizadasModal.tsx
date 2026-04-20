/**
 * Modal de drilldown "Agendas Realizadas" do Dashboard Comercial.
 * Exibe lista com filtros preservados; filtro por cliente/gestor/colaborador e paginação são feitos no front.
 */
import { useState, useMemo, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from '@/components/ui/dialog'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table'
import { TablePagination } from '@presentation/components/common'
import { Spinner } from '@/components/ui/spinner'
import type { FiltrosComercial, AgendaRealizadaDetalhe } from '@shared/types/dashboardComercialTypes'
import { obterLabelPeriodo, obterLabelCliente, obterLabelComercial, formatarApenasDataDrilldown } from '@shared/utils/dashboardComercialLabels'

const ITENS_POR_PAGINA_PADRAO = 10

/** Filtro no front por cliente, gestor externo ou colaborador selecionado nos filtros do dashboard. */
function filtrarPorFiltrosComercial(
  lista: AgendaRealizadaDetalhe[],
  filtros: FiltrosComercial
): AgendaRealizadaDetalhe[] {
  return lista.filter((item) => {
    const codigoClienteFiltro = filtros.codigoCliente?.trim()
    if (codigoClienteFiltro) {
      const itemCliente = (item.codigoCliente ?? '').trim()
      if (itemCliente.toLowerCase() !== codigoClienteFiltro.toLowerCase()) return false
    }

    const codigoGestor = filtros.codigoGestorExterno?.trim()
    if (codigoGestor) {
      const temGestor = (item.gestores ?? []).some(
        (g) => (g.codigoGestorExterno ?? '').trim().toLowerCase() === codigoGestor.toLowerCase()
      )
      if (!temGestor) return false
    }

    const codigoColaborador = filtros.codigoColaboradorAgendou?.trim()
    const nomeComercial = filtros.nomeComercialSelecionado?.trim()
    if (filtros.tipoComercial === 'colaborador' && (codigoColaborador || nomeComercial)) {
      const organizadorNorm = (item.organizador ?? '').trim().toLowerCase()
      if (nomeComercial && organizadorNorm !== nomeComercial.toLowerCase()) return false
    }

    return true
  })
}

function filtrarPorTermo(
  lista: AgendaRealizadaDetalhe[],
  termo: string
): AgendaRealizadaDetalhe[] {
  const termoNorm = termo.trim().toLowerCase()
  if (!termoNorm) return lista
  return lista.filter((item) => {
    const titulo = (item.titulo ?? '').toLowerCase()
    const nomeCliente = (item.nomeCliente ?? '').toLowerCase()
    const organizador = (item.organizador ?? '').toLowerCase()
    const gestoresNomes = (item.gestores ?? []).map((g) => (g.nome ?? '').toLowerCase()).join(' ')
    return (
      titulo.includes(termoNorm) ||
      nomeCliente.includes(termoNorm) ||
      organizador.includes(termoNorm) ||
      gestoresNomes.includes(termoNorm)
    )
  })
}

export interface DrilldownAgendasRealizadasModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  titulo: string
  comoCalculamos: string
  filtrosAtivos: FiltrosComercial
  dadosAgendaRealizadaDetalhe: AgendaRealizadaDetalhe[] | null
  agendaRealizadaDetalheCarregando: boolean
  agendaRealizadaDetalheErro: string | null
}

export function DrilldownAgendasRealizadasModal({
  open,
  onOpenChange,
  titulo,
  comoCalculamos,
  filtrosAtivos,
  dadosAgendaRealizadaDetalhe,
  agendaRealizadaDetalheCarregando,
  agendaRealizadaDetalheErro,
}: DrilldownAgendasRealizadasModalProps) {
  const navigate = useNavigate()
  const [termoBusca, setTermoBusca] = useState('')
  const [paginaAtual, setPaginaAtual] = useState(1)
  const [itensPorPagina, setItensPorPagina] = useState(ITENS_POR_PAGINA_PADRAO)

  const listaBase = useMemo(
    () => dadosAgendaRealizadaDetalhe ?? [],
    [dadosAgendaRealizadaDetalhe]
  )
  const listaFiltradaPorFiltros = useMemo(
    () => filtrarPorFiltrosComercial(listaBase, filtrosAtivos),
    [listaBase, filtrosAtivos]
  )
  const listaFiltrada = useMemo(
    () => filtrarPorTermo(listaFiltradaPorFiltros, termoBusca),
    [listaFiltradaPorFiltros, termoBusca]
  )
  const totalItens = listaFiltrada.length
  const totalPaginas = Math.ceil(totalItens / itensPorPagina) || 1
  const paginaEfetiva = Math.min(paginaAtual, totalPaginas)
  const indiceInicio = (paginaEfetiva - 1) * itensPorPagina
  const dadosPagina = useMemo(
    () => listaFiltrada.slice(indiceInicio, indiceInicio + itensPorPagina),
    [listaFiltrada, indiceInicio, itensPorPagina]
  )

  const handleOpenChange = useCallback(
    (next: boolean) => {
      if (!next) {
        setPaginaAtual(1)
        setItensPorPagina(ITENS_POR_PAGINA_PADRAO)
        setTermoBusca('')
      }
      onOpenChange(next)
    },
    [onOpenChange]
  )

  const handleVerMais = useCallback(
    (agendaId: number) => {
      navigate('/agendas-comerciais', { state: { agendaId } })
      handleOpenChange(false)
    },
    [navigate, handleOpenChange]
  )

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{titulo}</DialogTitle>
          <DialogDescription>
            Listagem de agendas realizadas com os filtros do dashboard. Use a busca para filtrar por nome, cliente ou organizador.
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

          {agendaRealizadaDetalheCarregando ? (
            <div className="flex flex-1 items-center justify-center rounded-lg border border-border bg-muted/30 p-12">
              <div className="flex flex-col items-center gap-3">
                <Spinner size={32} className="text-primary" />
                <p className="text-sm text-muted-foreground">Carregando agendas...</p>
              </div>
            </div>
          ) : agendaRealizadaDetalheErro ? (
            <div className="rounded-lg border border-destructive/50 bg-destructive/10 p-4">
              <p className="text-sm text-destructive">{agendaRealizadaDetalheErro}</p>
            </div>
          ) : (
            <>
              <div className="space-y-2">
                <Label htmlFor="drilldown-busca-agendas">Buscar por nome</Label>
                <Input
                  id="drilldown-busca-agendas"
                  type="search"
                  placeholder="Título, cliente ou organizador..."
                  value={termoBusca}
                  onChange={(e) => {
                    setTermoBusca(e.target.value)
                    setPaginaAtual(1)
                  }}
                  className="rounded-lg"
                  aria-label="Buscar agendas por nome, cliente ou organizador"
                />
              </div>

              <div>
                <h4 className="mb-2 text-sm font-semibold text-foreground">Agendas realizadas</h4>
                <div className="rounded-md border border-border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                          Título
                        </th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                          Cliente
                        </th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                          Organizador
                        </th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                          Gestores
                        </th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                          Data
                        </th>
                        <th className="h-10 px-4 text-right align-middle font-medium text-muted-foreground">
                          Ação
                        </th>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {dadosPagina.length === 0 ? (
                        <TableRow>
                          <TableCell
                            colSpan={6}
                            className="h-24 text-center text-muted-foreground"
                          >
                            Nenhum registro encontrado
                          </TableCell>
                        </TableRow>
                      ) : (
                        dadosPagina.map((item) => {
                          const gestoresTexto = (item.gestores ?? [])
                            .map((g) => (g.nome?.trim() ?? '—'))
                            .join(', ') || '—'
                          return (
                            <TableRow
                              key={item.agendaId}
                            >
                              <TableCell className="font-medium">{item.titulo ?? '—'}</TableCell>
                              <TableCell>{item.nomeCliente ?? '—'}</TableCell>
                              <TableCell>{item.organizador ?? '—'}</TableCell>
                              <TableCell className="max-w-[200px]" title={gestoresTexto}>
                                <span className="line-clamp-2">{gestoresTexto}</span>
                              </TableCell>
                              <TableCell className="text-muted-foreground">
                                {formatarApenasDataDrilldown(item.dataAgendada)}
                              </TableCell>
                              <TableCell className="text-right">
                                <Button
                                  type="button"
                                  variant="link"
                                  className="h-auto p-0 text-primary"
                                  onClick={() => handleVerMais(item.agendaId)}
                                >
                                  Ver mais
                                </Button>
                              </TableCell>
                            </TableRow>
                          )
                        })
                      )}
                    </TableBody>
                  </Table>
                </div>
                {totalItens > 0 && (
                  <TablePagination
                    currentPage={paginaEfetiva}
                    totalItems={totalItens}
                    itemsPerPage={itensPorPagina}
                    onPageChange={setPaginaAtual}
                    onItemsPerPageChange={(v) => {
                      setItensPorPagina(Number(v))
                      setPaginaAtual(1)
                    }}
                  />
                )}
              </div>
            </>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
