/**
 * Modal de drilldown do Dashboard Comercial.
 * O acesso a este modal está inativo nesta entrega — funcionalidade ainda está sendo validada.
 * Os gatilhos (clique nos KPIs/gráficos) estão desabilitados em BigNumbersComercial e GraficosRadarComercial.
 */
import { useState, useMemo } from 'react'
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from '@/components/ui/sheet'
import { Badge } from '@/components/ui/badge'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { TablePagination } from '@presentation/components/common'
import { Spinner } from '@/components/ui/spinner'
import { format, parseISO } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import type {
  FiltrosComercial,
  LinhaEvidencia,
  IdIndicador,
  AgendaRealizadaDetalhe,
} from '@shared/types/dashboardComercialTypes'
import { obterLabelPeriodo, obterLabelCliente, obterLabelComercial } from '@shared/utils/dashboardComercialLabels'

const ITENS_POR_PAGINA_PADRAO = 10

function formatarDataCriacao(iso: string): string {
  try {
    return format(parseISO(iso), "dd/MM/yyyy 'às' HH:mm", { locale: ptBR })
  } catch {
    return iso
  }
}

interface DrilldownSheetProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  idIndicador?: IdIndicador
  titulo: string
  comoCalculamos: string
  dadosEvidencia: LinhaEvidencia[]
  filtrosAtivos: FiltrosComercial
  emBreve?: boolean
  dadosAgendaRealizadaDetalhe?: AgendaRealizadaDetalhe[] | null
  agendaRealizadaDetalheCarregando?: boolean
  agendaRealizadaDetalheErro?: string | null
}

export function DrilldownSheet({
  open,
  onOpenChange,
  idIndicador,
  titulo,
  comoCalculamos,
  dadosEvidencia,
  filtrosAtivos,
  emBreve = false,
  dadosAgendaRealizadaDetalhe = null,
  agendaRealizadaDetalheCarregando = false,
  agendaRealizadaDetalheErro = null,
}: DrilldownSheetProps) {
  const [paginaAtual, setPaginaAtual] = useState(1)
  const [itensPorPagina, setItensPorPagina] = useState(ITENS_POR_PAGINA_PADRAO)

  const dadosOrdenados = useMemo(() => {
    return [...dadosEvidencia].sort((a, b) => {
      const dataA = a.data ?? ''
      const dataB = b.data ?? ''
      return dataB.localeCompare(dataA)
    })
  }, [dadosEvidencia])

  const totalItens = dadosOrdenados.length
  const indiceInicio = (paginaAtual - 1) * itensPorPagina
  const dadosPagina = dadosOrdenados.slice(indiceInicio, indiceInicio + itensPorPagina)

  const isAgendasRealizadas = idIndicador === 'encontros-realizados'
  const agendasOrdenadas = useMemo(() => {
    if (!dadosAgendaRealizadaDetalhe || !Array.isArray(dadosAgendaRealizadaDetalhe)) return []
    return [...dadosAgendaRealizadaDetalhe].sort((a, b) =>
      b.dataAgendada.localeCompare(a.dataAgendada),
    )
  }, [dadosAgendaRealizadaDetalhe])
  const totalAgendas = agendasOrdenadas.length
  const indiceInicioAgendas = (paginaAtual - 1) * itensPorPagina
  const agendasPagina = agendasOrdenadas.slice(indiceInicioAgendas, indiceInicioAgendas + itensPorPagina)

  const handleOpenChange = (next: boolean) => {
    if (!next) {
      setPaginaAtual(1)
      setItensPorPagina(ITENS_POR_PAGINA_PADRAO)
    }
    onOpenChange(next)
  }

  const conteudoAgendasRealizadas = () => {
    if (agendaRealizadaDetalheCarregando) {
      return (
        <div className="flex flex-1 flex-col items-center justify-center rounded-lg border border-border bg-muted/30 p-12">
          <Spinner size={32} className="text-primary" />
          <p className="mt-4 text-sm text-muted-foreground">Carregando agendas realizadas...</p>
        </div>
      )
    }
    if (agendaRealizadaDetalheErro) {
      return (
        <Alert variant="destructive">
          <AlertTitle>Erro ao carregar dados</AlertTitle>
          <AlertDescription>{agendaRealizadaDetalheErro}</AlertDescription>
        </Alert>
      )
    }
    return (
      <>
        <div className="flex flex-wrap gap-2">
          <Badge variant="secondary">{obterLabelPeriodo(filtrosAtivos)}</Badge>
          <Badge variant="secondary">{obterLabelCliente(filtrosAtivos.codigoCliente ?? null, filtrosAtivos.nomeClienteSelecionado)}</Badge>
          <Badge variant="secondary">{obterLabelComercial(filtrosAtivos)}</Badge>
        </div>

        <div className="rounded-lg border border-border bg-muted/30 p-4">
          <h4 className="mb-2 text-sm font-semibold text-foreground">Como calculamos</h4>
          <p className="text-sm text-muted-foreground">{comoCalculamos}</p>
        </div>

        <div>
          <h4 className="mb-2 text-sm font-semibold text-foreground">Agendas realizadas</h4>
          <div className="rounded-md border">
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
                    Data
                  </th>
                  <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                    Gestores
                  </th>
                </TableRow>
              </TableHeader>
              <TableBody>
                {agendasPagina.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={5} className="h-24 text-center text-muted-foreground">
                      Nenhuma agenda realizada encontrada
                    </TableCell>
                  </TableRow>
                ) : (
                  agendasPagina.map((agenda) => (
                    <TableRow key={agenda.agendaId}>
                      <TableCell className="font-medium">{agenda.titulo}</TableCell>
                      <TableCell>{agenda.nomeCliente}</TableCell>
                      <TableCell>{agenda.organizador}</TableCell>
                      <TableCell className="text-muted-foreground">
                        {formatarDataCriacao(agenda.dataAgendada)}
                      </TableCell>
                      <TableCell className="text-muted-foreground">
                        {agenda.gestores?.length
                          ? agenda.gestores.map((g) => g.nome).join(', ')
                          : '—'}
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>
          {totalAgendas > 0 && (
            <TablePagination
              currentPage={paginaAtual}
              totalItems={totalAgendas}
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
    )
  }

  return (
    <Sheet open={open} onOpenChange={handleOpenChange}>
      <SheetContent side="right" className="w-full sm:max-w-xl overflow-y-auto">
        <SheetHeader>
          <SheetTitle>{titulo}</SheetTitle>
          <SheetDescription className="sr-only">
            Detalhamento e evidências do indicador {titulo}
          </SheetDescription>
        </SheetHeader>

        <div className="mt-6 space-y-4">
          {emBreve ? (
            <div className="flex flex-1 items-center justify-center rounded-lg border border-border bg-muted/30 p-12">
              <p className="text-center text-muted-foreground">Em Breve</p>
            </div>
          ) : isAgendasRealizadas ? (
            conteudoAgendasRealizadas()
          ) : (
            <>
              <div className="flex flex-wrap gap-2">
                <Badge variant="secondary">{obterLabelPeriodo(filtrosAtivos)}</Badge>
                <Badge variant="secondary">{obterLabelCliente(filtrosAtivos.codigoCliente ?? null, filtrosAtivos.nomeClienteSelecionado)}</Badge>
                <Badge variant="secondary">{obterLabelComercial(filtrosAtivos)}</Badge>
              </div>

              <div className="rounded-lg border border-border bg-muted/30 p-4">
                <h4 className="mb-2 text-sm font-semibold text-foreground">Como calculamos</h4>
                <p className="text-sm text-muted-foreground">{comoCalculamos}</p>
              </div>

              <div>
                <h4 className="mb-2 text-sm font-semibold text-foreground">Tabela de evidência</h4>
                <div className="rounded-md border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                          Cliente
                        </th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                          Gestor
                        </th>
                        <th className="h-10 px-4 text-left align-middle font-medium text-muted-foreground">
                          Data
                        </th>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {dadosPagina.length === 0 ? (
                        <TableRow>
                          <TableCell colSpan={3} className="h-24 text-center text-muted-foreground">
                            Nenhum registro encontrado
                          </TableCell>
                        </TableRow>
                      ) : (
                        dadosPagina.map((linha, idx) => (
                          <TableRow key={`${linha.cliente}-${linha.gestor}-${linha.data}-${idx}`}>
                            <TableCell className="font-medium">{linha.cliente}</TableCell>
                            <TableCell>{linha.gestor}</TableCell>
                            <TableCell className="text-muted-foreground">{linha.data}</TableCell>
                          </TableRow>
                        ))
                      )}
                    </TableBody>
                  </Table>
                </div>
                {totalItens > 0 && (
                  <TablePagination
                    currentPage={paginaAtual}
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
      </SheetContent>
    </Sheet>
  )
}
