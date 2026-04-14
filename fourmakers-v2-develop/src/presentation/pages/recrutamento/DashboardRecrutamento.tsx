import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Command, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  AlertTriangle,
  ArrowUpDown,
  Briefcase,
  Timer,
  TrendingDown,
  TrendingUp,
  Users,
  X,
  XCircle,
} from '@/components/ui/system-icons'
import { cn } from '@/lib/utils'

import { PageBreadcrumb, PageHeader } from '@presentation/components/common'
import { DashboardRecrutamentoModal } from '@presentation/components/recrutamento'
import { useDashboardRecrutamento } from '@presentation/hooks/recrutamento'

const ROTULO_EM_FOCO = 'Em Foco + 24 horas'

const cardsStatus = [
  { id: 'em-foco', titulo: ROTULO_EM_FOCO, subtitulo: 'Status de vagas', valor: 12, descricao: 'Vagas Paradas sem inicio', destaque: true, Icone: AlertTriangle },
  { id: 'em-andamento', titulo: 'Em Andamento', subtitulo: 'Status de vagas', valor: 36, descricao: 'Vagas ativas no funil', Icone: TrendingUp },
  { id: 'entrevista-cliente', titulo: 'Em Entrevista com Cliente', subtitulo: 'Status de vagas', valor: 8, descricao: 'Vagas na etapa cliente', Icone: Users },
  { id: 'tm-inicio', titulo: 'T.M. Início do Recrutamento', subtitulo: 'Status de vagas', valor: '6 dias', descricao: 'Tempo médio de início', Icone: Timer },
  { id: 'vagas-perdidas', titulo: 'Vagas Perdidas', subtitulo: 'Status de vagas', valor: 5, descricao: 'Perdas no período', Icone: TrendingDown },
  { id: 'vagas-canceladas', titulo: 'Vagas Canceladas', subtitulo: 'Status de vagas', valor: 3, descricao: 'Canceladas no período', Icone: XCircle },
]

const CARDS_COM_MODAL: string[] = ['em-foco', 'em-andamento', 'vagas-perdidas']

function renderSlaBadge(sla: string) {
  if (sla === 'Atraso' || sla === 'Atrasado') {
    return <Badge className="bg-destructive text-destructive-foreground">Atraso</Badge>
  }
  if (sla === 'No Prazo') {
    return <Badge className="bg-success text-success-foreground">No Prazo</Badge>
  }
  if (sla) {
    return <Badge className="bg-warning text-warning-foreground">{sla}</Badge>
  }
  return null
}

export default function DashboardRecrutamento() {
  const {
    carregando,
    filtros,
    setFiltros,
    clientesDisponiveis,
    clientesSelecionados,
    buscaCliente,
    setBuscaCliente,
    setClientesAtivos,
    clientesAtivos: _clientesAtivos,
    mostrarListaClientes,
    setMostrarListaClientes,
    clienteRef,
    recrutadoresDisponiveis,
    recrutadoresSelecionados,
    buscaRecrutador,
    setBuscaRecrutador,
    setRecrutadoresAtivos,
    recrutadoresAtivos: _recrutadoresAtivos,
    mostrarListaRecrutadores,
    setMostrarListaRecrutadores,
    recrutadorRef,
    modalAberto,
    setModalAberto,
    cardAtivo,
    vagasEmFocoList,
    carregandoVagasEmFoco,
    funilData: _funilData,
    carregandoFunil,
    vagasPerdidasMotivosList,
    carregandoVagasPerdidas,
    fontesCandidatos,
    sortKey: _sortKey,
    sortDirection: _sortDirection,
    paginaAtual: _paginaAtual,
    setPaginaAtual,
    filtrosAtivos,
    handleFiltrar,
    handleLimparFiltros,
    handleAdicionarCliente,
    handleRemoverCliente,
    handleAdicionarRecrutador,
    handleRemoverRecrutador,
    handleAbrirModal,
    handleOrdenar,
    vagasEmFocoPaginadas,
    funilEtapasParaModal,
    totalFunil: _totalFunil,
    maxFunil,
    maxPerdidas,
    totalNovosCandidatosPeriodo,
    totalNovosCandidatosLinkedin,
    obterValorCard,
    tituloModal,
    descricaoModal,
    totalPaginas,
    paginaSegura,
  } = useDashboardRecrutamento()

  if (carregando) {
    return (
      <div className="container mx-auto p-4 md:p-6">
        <div className="text-center py-8 text-muted-foreground">Buscando dados</div>
      </div>
    )
  }

  return (
    <div className="container mx-auto p-4 md:p-6 space-y-6">
      <PageBreadcrumb
        items={[
          { label: 'Recrutamento' },
          { label: 'Dashboard de Recrutamento' },
        ]}
      />

      <PageHeader
        title="Dashboard de Recrutamento"
        description="Acompanhe as métricas e o status das vagas em tempo real."
      />

      <Card>
        <CardContent className="p-6">
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-5 items-end">
            <div className="space-y-2">
              <Label htmlFor="data-inicio-dashboard">Data início</Label>
              <Input
                id="data-inicio-dashboard"
                type="date"
                value={filtros.dataInicio}
                onChange={(event) =>
                  setFiltros((prev) => ({ ...prev, dataInicio: event.target.value }))
                }
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="data-fim-dashboard">Data fim</Label>
              <Input
                id="data-fim-dashboard"
                type="date"
                value={filtros.dataFim}
                onChange={(event) =>
                  setFiltros((prev) => ({ ...prev, dataFim: event.target.value }))
                }
              />
            </div>

            <div className="space-y-2">
              <Label>Clientes</Label>
              <div className="relative" ref={clienteRef}>
                <Command shouldFilter={false}>
                  <CommandInput
                    placeholder="Digite para buscar clientes..."
                    value={buscaCliente}
                    onValueChange={setBuscaCliente}
                    onFocus={() => setClientesAtivos(true)}
                    onClick={() => setMostrarListaClientes(true)}
                    className="text-sm"
                  />
                  {mostrarListaClientes && clientesDisponiveis.length > 0 && (
                    <CommandList className="absolute z-20 mt-8 w-full max-h-[200px] rounded-lg border border-borderSoft bg-background shadow-sm">
                      <CommandGroup>
                        {clientesDisponiveis
                          .filter(
                            (cliente) =>
                              !clientesSelecionados.some(
                                (selecionado) => selecionado.codigoCliente === cliente.codigoCliente,
                              ),
                          )
                          .map((cliente) => (
                            <CommandItem
                              key={cliente.codigoCliente}
                              value={cliente.nomeCliente}
                              onSelect={() => handleAdicionarCliente(cliente)}
                            >
                              {cliente.nomeCliente}
                            </CommandItem>
                          ))}
                      </CommandGroup>
                    </CommandList>
                  )}
                </Command>
              </div>
            </div>

            <div className="space-y-2">
              <Label>Recrutadores</Label>
              <div className="relative" ref={recrutadorRef}>
                <Command shouldFilter={false}>
                  <CommandInput
                    placeholder="Digite para buscar recrutadores..."
                    value={buscaRecrutador}
                    onValueChange={setBuscaRecrutador}
                    onFocus={() => setRecrutadoresAtivos(true)}
                    onClick={() => setMostrarListaRecrutadores(true)}
                    className="text-sm"
                  />
                  {mostrarListaRecrutadores && recrutadoresDisponiveis.length > 0 && (
                    <CommandList className="absolute z-20 mt-8 w-full max-h-[200px] rounded-lg border border-borderSoft bg-background shadow-sm">
                      <CommandGroup>
                        {recrutadoresDisponiveis
                          .filter(
                            (recrutador) =>
                              !recrutadoresSelecionados.some(
                                (selecionado) =>
                                  selecionado.codigoInternoColaborador === recrutador.codigoInternoColaborador,
                              ),
                          )
                          .map((recrutador) => (
                            <CommandItem
                              key={recrutador.codigoInternoColaborador}
                              value={recrutador.nome}
                              onSelect={() => handleAdicionarRecrutador(recrutador)}
                            >
                              {recrutador.nome}
                            </CommandItem>
                          ))}
                      </CommandGroup>
                    </CommandList>
                  )}
                </Command>
              </div>
            </div>

            <div className="flex flex-col gap-2 items-end">
              <Button
                type="button"
                variant="outline"
                onClick={handleFiltrar}
                disabled={!filtrosAtivos}
              >
                Filtrar
              </Button>
              {filtrosAtivos && (
                <button
                  type="button"
                  onClick={handleLimparFiltros}
                  className="text-sm text-muted-foreground hover:text-foreground hover:underline underline-offset-2 cursor-pointer bg-transparent border-0 p-0"
                  aria-label="Remover filtros"
                >
                  Remover filtros
                </button>
              )}
            </div>
          </div>

          {(clientesSelecionados.length > 0 || recrutadoresSelecionados.length > 0) && (
            <div className="mt-4 space-y-3">
              {clientesSelecionados.length > 0 && (
                <div className="space-y-2">
                  <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                    Clientes selecionados
                  </p>
                  <div className="flex flex-wrap gap-2">
                    {clientesSelecionados.map((cliente) => (
                      <Badge
                        key={cliente.codigoCliente}
                        variant="secondary"
                        className="flex items-center gap-2 cursor-pointer"
                        onClick={() => handleRemoverCliente(cliente.codigoCliente)}
                        role="button"
                        tabIndex={0}
                        onKeyDown={(e) => {
                          if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault()
                            handleRemoverCliente(cliente.codigoCliente)
                          }
                        }}
                        aria-label={`Remover ${cliente.nomeCliente}`}
                      >
                        {cliente.nomeCliente}
                        <X className="h-3 w-3 shrink-0" />
                      </Badge>
                    ))}
                  </div>
                </div>
              )}

              {recrutadoresSelecionados.length > 0 && (
                <div className="space-y-2">
                  <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">
                    Recrutadores selecionados
                  </p>
                  <div className="flex flex-wrap gap-2">
                    {recrutadoresSelecionados.map((recrutador) => (
                      <Badge
                        key={recrutador.codigoInternoColaborador}
                        variant="secondary"
                        className="flex items-center gap-2 cursor-pointer"
                        onClick={() => handleRemoverRecrutador(recrutador.codigoInternoColaborador)}
                        role="button"
                        tabIndex={0}
                        onKeyDown={(e) => {
                          if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault()
                            handleRemoverRecrutador(recrutador.codigoInternoColaborador)
                          }
                        }}
                        aria-label={`Remover ${recrutador.nome}`}
                      >
                        {recrutador.nome}
                        <X className="h-3 w-3 shrink-0" />
                      </Badge>
                    ))}
                  </div>
                </div>
              )}
            </div>
          )}
        </CardContent>
      </Card>

      <div className="space-y-4">
        <div>
          <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">
            Status de Vagas
          </h3>
        </div>

        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {cardsStatus.map((card) => {
            const abreModal = CARDS_COM_MODAL.includes(card.id)
            const Wrapper = abreModal ? 'button' : 'div'
            return (
              <Wrapper
                key={card.id}
                {...(abreModal ? { type: 'button' as const, className: 'text-left', onClick: () => handleAbrirModal(card.id) } : { className: 'text-left' })}
              >
                <Card
                  className={cn(
                    'border border-borderSoft bg-surfaceElevated transition-shadow',
                    abreModal && 'hover:shadow-md cursor-pointer',
                    card.destaque && 'border-destructive/40 bg-destructive text-destructive-foreground',
                  )}
                >
                  <CardContent className="p-6 space-y-3">
                    <div className="flex items-start justify-between gap-4">
                      <div>
                        <p className={cn('text-sm font-medium', card.destaque && 'text-destructive-foreground')}>
                          {card.titulo}
                        </p>
                        <p
                          className={cn(
                            'text-xs text-muted-foreground',
                            card.destaque && 'text-destructive-foreground/80',
                          )}
                        >
                          {card.subtitulo}
                        </p>
                      </div>
                      <card.Icone
                        className={cn('h-5 w-5 text-muted-foreground', card.destaque && 'text-destructive-foreground')}
                      />
                    </div>
                    <div>
                      <div className={cn('text-3xl font-semibold', card.destaque && 'text-destructive-foreground')}>
                        {obterValorCard(card.id) ?? card.valor}
                      </div>
                      <p className={cn('text-xs text-muted-foreground', card.destaque && 'text-destructive-foreground/80')}>
                        {card.descricao}
                      </p>
                    </div>
                  </CardContent>
                </Card>
              </Wrapper>
            )
          })}
        </div>
      </div>

      <Card>
        <CardContent className="p-6 space-y-6">
          <div>
            <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">
              Aquisição de Candidatos
            </h3>
            <p className="text-sm text-muted-foreground">
              Visão consolidada de novos candidatos e fontes de captação.
            </p>
          </div>

          <div className="grid gap-4 lg:grid-cols-3">
            <Card className="border border-borderSoft bg-surfaceElevated">
              <CardContent className="p-5 space-y-3">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium">Novos candidatos no período</p>
                    <p className="text-3xl font-semibold">{totalNovosCandidatosPeriodo}</p>
                  </div>
                  <Briefcase className="h-5 w-5 text-muted-foreground" />
                </div>
              </CardContent>
            </Card>

            <Card className="border border-borderSoft bg-surfaceElevated">
              <CardContent className="p-5 space-y-3">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium">Novos candidatos LinkedIn</p>
                    <p className="text-3xl font-semibold">{totalNovosCandidatosLinkedin}</p>
                  </div>
                  <Users className="h-5 w-5 text-muted-foreground" />
                </div>
              </CardContent>
            </Card>

            <Card className="border border-borderSoft bg-surfaceElevated">
              <CardContent className="p-5 space-y-3">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium">Fontes de candidatos</p>
                  </div>
                  <TrendingUp className="h-5 w-5 text-muted-foreground" />
                </div>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Fonte</TableHead>
                      <TableHead>Quantidade</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {fontesCandidatos.map((fonte) => (
                      <TableRow key={`${fonte.origem}-${fonte.orgId}`}>
                        <TableCell>{fonte.origem}</TableCell>
                        <TableCell>{fonte.total}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </div>
        </CardContent>
      </Card>

      <DashboardRecrutamentoModal
        open={modalAberto}
        onOpenChange={setModalAberto}
        titulo={tituloModal}
        descricao={descricaoModal}
        contentClassName={cardAtivo === 'em-foco' ? 'max-w-7xl w-[95vw]' : 'max-w-3xl'}
      >
        {cardAtivo === 'em-foco' && (
          <div className="space-y-4">
            {carregandoVagasEmFoco ? (
              <p className="text-sm text-muted-foreground">Carregando...</p>
            ) : vagasEmFocoList.length === 0 ? (
              <p className="text-sm text-muted-foreground py-4 text-center">
                Não existem dados neste período.
              </p>
            ) : (
              <>
                <div className="overflow-x-auto">
                  <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>
                        <Button variant="ghost" className="h-8 px-2" onClick={() => handleOrdenar('cliente')}>
                          Cliente
                          <ArrowUpDown className="ml-2 h-4 w-4" />
                        </Button>
                      </TableHead>
                      <TableHead>
                        <Button variant="ghost" className="h-8 px-2" onClick={() => handleOrdenar('codVaga')}>
                          Cód. Vaga
                          <ArrowUpDown className="ml-2 h-4 w-4" />
                        </Button>
                      </TableHead>
                      <TableHead>
                        <Button variant="ghost" className="h-8 px-2" onClick={() => handleOrdenar('vaga')}>
                          Vaga
                          <ArrowUpDown className="ml-2 h-4 w-4" />
                        </Button>
                      </TableHead>
                      <TableHead>
                        <Button variant="ghost" className="h-8 px-2" onClick={() => handleOrdenar('status')}>
                          Status
                          <ArrowUpDown className="ml-2 h-4 w-4" />
                        </Button>
                      </TableHead>
                      <TableHead>
                        <Button variant="ghost" className="h-8 px-2" onClick={() => handleOrdenar('responsavel')}>
                          Responsável
                          <ArrowUpDown className="ml-2 h-4 w-4" />
                        </Button>
                      </TableHead>
                      <TableHead>
                        <Button
                          variant="ghost"
                          className="h-8 px-2"
                          onClick={() => handleOrdenar('ultimaMovimentacaoDate')}
                        >
                          Última movimentação
                          <ArrowUpDown className="ml-2 h-4 w-4" />
                        </Button>
                      </TableHead>
                      <TableHead>
                        <Button variant="ghost" className="h-8 px-2" onClick={() => handleOrdenar('tempoEtapa')}>
                          Tempo na etapa
                          <ArrowUpDown className="ml-2 h-4 w-4" />
                        </Button>
                      </TableHead>
                      <TableHead className="text-center">
                        <Button variant="ghost" className="h-8 px-2" onClick={() => handleOrdenar('sla')}>
                          SLA
                          <ArrowUpDown className="ml-2 h-4 w-4" />
                        </Button>
                      </TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {vagasEmFocoPaginadas.map((vaga, idx) => (
                      <TableRow key={`${vaga.cliente}-${vaga.vaga}-${idx}`}>
                        <TableCell>{vaga.cliente}</TableCell>
                        <TableCell>{vaga.codVaga}</TableCell>
                        <TableCell>{vaga.vaga}</TableCell>
                        <TableCell>{vaga.status}</TableCell>
                        <TableCell>{vaga.responsavel}</TableCell>
                        <TableCell>{vaga.ultimaMovimentacao}</TableCell>
                        <TableCell>{vaga.tempoEtapa}</TableCell>
                        <TableCell className="text-center">{renderSlaBadge(vaga.sla)}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
                </div>

                <div className="flex items-center justify-between text-sm text-muted-foreground shrink-0 pt-2">
                  <span>
                    Página {paginaSegura} de {totalPaginas}
                  </span>
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setPaginaAtual((prev) => Math.max(1, prev - 1))}
                      disabled={paginaSegura === 1}
                    >
                      Anterior
                    </Button>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setPaginaAtual((prev) => Math.min(totalPaginas, prev + 1))}
                      disabled={paginaSegura === totalPaginas}
                    >
                      Próxima
                    </Button>
                  </div>
                </div>
              </>
            )}
          </div>
        )}

        {cardAtivo === 'em-andamento' && (
          <div className="space-y-4">
            {carregandoFunil ? (
              <p className="text-sm text-muted-foreground">Carregando...</p>
            ) : (
              <div className="space-y-3">
                {funilEtapasParaModal.map((item) => (
                  <div key={item.etapa} className="space-y-1">
                    <div className="flex items-center justify-between text-sm">
                      <span>{item.etapa}</span>
                      <span className="font-medium">{item.valor}</span>
                    </div>
                    <div className="h-3 rounded-full bg-muted">
                      <div
                        className="h-full rounded-full bg-primary"
                        style={{ width: `${(item.valor / maxFunil) * 100}%` }}
                      />
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {cardAtivo === 'vagas-perdidas' && (
          <div className="space-y-4">
            {carregandoVagasPerdidas ? (
              <p className="text-sm text-muted-foreground">Carregando...</p>
            ) : vagasPerdidasMotivosList.length === 0 ? (
              <p className="text-sm text-muted-foreground py-4 text-center">
                Não existem dados neste período.
              </p>
            ) : (
              <div className="space-y-3">
                {vagasPerdidasMotivosList.map((item, index) => (
                  <div
                    key={item.id ?? `motivo-${index}`}
                    className="space-y-1"
                  >
                    <div className="flex items-center justify-between text-sm">
                      <span>{item.motivo}</span>
                      <span className="font-medium">{item.total}</span>
                    </div>
                    <div className="h-3 rounded-full bg-muted">
                      <div
                        className="h-full rounded-full bg-destructive"
                        style={{ width: `${(item.total / maxPerdidas) * 100}%` }}
                      />
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}
      </DashboardRecrutamentoModal>
    </div>
  )
}
