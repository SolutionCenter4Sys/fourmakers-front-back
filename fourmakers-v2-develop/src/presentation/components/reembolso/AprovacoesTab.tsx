import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Search, Eye, CalendarIcon, Download } from "@/components/ui/system-icons";
import { format } from "date-fns";
import { DataTable, TablePagination } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { useAprovacoesTab } from "@/hooks/useReembolsoComponentes";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import type { SolicitacaoGerenteProjeto, SolicitacaoObjetoGerenteProjeto, SolicitacaoDocumento } from "@domain/entities/SolicitacaoReembolso";
import { StatusBadge } from "@presentation/components/common";
import { formatCurrency } from "@shared/utils/calculations";
import { cn } from "@/lib/utils";
import { Skeleton } from "@/components/ui/skeleton";
import { useAppSelector } from "@/app/store/hooks";
import { processarUrlComToken } from "@shared/utils/urlUtils";

export const AprovacoesTab = () => {
  const navigate = useNavigate();
  const { token } = useAppSelector((state) => state.auth);
  const [dataInicio, setDataInicio] = useState<Date>();
  const [dataFim, setDataFim] = useState<Date>();
  const [cliente, setCliente] = useState<string | undefined>(undefined);
  const [projeto, setProjeto] = useState<string | undefined>(undefined);
  const [status, setStatus] = useState<string | undefined>(undefined);
  const [busca, setBusca] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10);
  const [modalDetalhesOpen, setModalDetalhesOpen] = useState(false);
  const [modalDocumentosOpen, setModalDocumentosOpen] = useState(false);
  const [documentosSelecionados, setDocumentosSelecionados] = useState<SolicitacaoDocumento[]>([]);
  const [solicitacaoSelecionada, setSolicitacaoSelecionada] = useState<SolicitacaoGerenteProjeto | null>(null);
  const [filtrosAplicados, setFiltrosAplicados] = useState<{
    dataInicio?: Date;
    dataFim?: Date;
    codigoCliente?: string;
    codigoProjeto?: string;
    status?: string;
  }>({});
  
  const { solicitacoes, stats, statusList, clientesList, projetosList, loading, loadProjetos } = useAprovacoesTab(filtrosAplicados);

  // Filtrar solicitações com base na busca
  const filteredSolicitacoes = solicitacoes.filter((solicitacao) => {
    if (!busca) return true;
    const searchLower = busca.toLowerCase();
    return (
      solicitacao.nomeColaborador.toLowerCase().includes(searchLower) ||
      solicitacao.objetivo?.toLowerCase().includes(searchLower) ||
      solicitacao.destino?.toLowerCase().includes(searchLower) ||
      solicitacao.periodo?.toLowerCase().includes(searchLower) ||
      solicitacao.cliente.toLowerCase().includes(searchLower) ||
      solicitacao.projeto.toLowerCase().includes(searchLower) ||
      solicitacao.somaValores.toLowerCase().includes(searchLower) ||
      solicitacao.dataSolicitacao.toLowerCase().includes(searchLower)
    );
  });

  // Calcular dados paginados
  const totalItems = filteredSolicitacoes.length;
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const paginatedSolicitacoes = filteredSolicitacoes.slice(startIndex, endIndex);

  // Resetar página quando busca mudar
  useEffect(() => {
    setCurrentPage(1);
  }, [busca]);

  // Carregar projetos quando cliente mudar
  useEffect(() => {
    if (cliente && cliente !== "todos" && loadProjetos) {
      loadProjetos(cliente);
    } else if (!cliente || cliente === "todos") {
      // Limpar projetos quando não houver cliente selecionado
      setProjeto(undefined);
    }
  }, [cliente, loadProjetos]);

  // Handler para mudança de cliente
  const handleClienteChange = (value: string) => {
    if (value === "todos") {
      setCliente(undefined);
      setProjeto(undefined);
    } else {
      setCliente(value);
      setProjeto(undefined); // Limpar projeto ao trocar cliente
    }
  };

  const handleBuscar = () => {
    setFiltrosAplicados({
      dataInicio,
      dataFim,
      codigoCliente: cliente && cliente !== "todos" ? cliente : undefined,
      codigoProjeto: projeto && projeto !== "todos" ? projeto : undefined,
      status: status || undefined,
    });
  };

  const handleLimpar = () => {
    setDataInicio(undefined);
    setDataFim(undefined);
    setCliente(undefined);
    setProjeto(undefined);
    setStatus(undefined);
    setFiltrosAplicados({});
  };

  const columns: Column[] = [
    { id: "nomeColaborador", label: "Colaborador", sortable: true },
    { id: "objetivo", label: "Objetivo", sortable: true },
    { id: "destino", label: "Destino", sortable: true },
    { id: "periodo", label: "Período", sortable: true },
    { id: "clienteProjeto", label: "Cliente/Projeto", sortable: true },
    { id: "somaValores", label: "Soma de valores", sortable: true },
    { id: "dataSolicitacao", label: "Data de solicitação", sortable: true },
    { id: "status", label: "Status", sortable: false },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[120px]" },
  ];

  // Função para contar status dos itens
  const contarStatusPorSolicitacao = (solicitacao: typeof solicitacoes[0]) => {
    if (!solicitacao.solicitacaoCompleta?.objeto || solicitacao.solicitacaoCompleta.objeto.length === 0) {
      return {};
    }

    const contagem: Record<string, number> = {};
    solicitacao.solicitacaoCompleta.objeto.forEach((item) => {
      const status = item.status || 'Sem Status';
      contagem[status] = (contagem[status] || 0) + 1;
    });

    return contagem;
  };

  const renderCell = (solicitacao: typeof solicitacoes[0], columnId: string) => {
    switch (columnId) {
      case "nomeColaborador":
        return (
          <button
            onClick={() => {
              const nomeEncoded = encodeURIComponent(solicitacao.nomeColaborador);
              navigate(`/reembolso/aprovar?codColab=${solicitacao.codigoInternoColaborador}&nome=${nomeEncoded}`);
            }}
            className="font-medium text-primary hover:underline cursor-pointer text-left"
          >
            {solicitacao.nomeColaborador}
          </button>
        );
      case "objetivo":
        return <span>{solicitacao.objetivo}</span>;
      case "destino":
        return <span>{solicitacao.destino || '-'}</span>;
      case "periodo":
        return <span>{solicitacao.periodo}</span>;
      case "clienteProjeto":
        const clienteProjetoText = `${solicitacao.cliente} / ${solicitacao.projeto}`;
        return (
          <div className="max-w-[200px] truncate" title={clienteProjetoText}>
            <span className="font-medium">{clienteProjetoText}</span>
          </div>
        );
      case "somaValores":
        return <span className="font-semibold">{solicitacao.somaValores}</span>;
      case "dataSolicitacao":
        return <span>{solicitacao.dataSolicitacao}</span>;
      case "status":
        const contagemStatus = contarStatusPorSolicitacao(solicitacao);
        const statusEntries = Object.entries(contagemStatus);
        
        if (statusEntries.length === 0) {
          return <span className="text-muted-foreground">-</span>;
        }

        return (
          <div className="flex flex-wrap gap-2">
            {statusEntries.map(([status, quantidade]) => (
              <div key={status} className="inline-flex items-center gap-1.5">
                <StatusBadge status={status} variant="reembolso" />
                <span className="text-sm font-semibold text-foreground bg-btnGhostHover dark:bg-white/5 px-2 py-0.5 rounded-pillToken">
                  {quantidade}
                </span>
              </div>
            ))}
          </div>
        );
      case "acoes":
        return (
          <div className="flex items-center gap-2">
            <Button 
              variant="ghost" 
              size="sm"
              onClick={() => {
                setSolicitacaoSelecionada(solicitacao.solicitacaoCompleta);
                setModalDetalhesOpen(true);
              }}
            >
              <Eye className="h-4 w-4 mr-2" />
              Detalhes
            </Button>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="space-y-6">
      {/* Big Numbers */}
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
        {loading ? (
          <>
            {[1, 2, 3, 4, 5].map((i) => (
              <Card key={i}>
                <CardContent className="p-4">
                  <div className="flex items-center gap-3">
                    <Skeleton className="h-9 w-9 rounded-lg" />
                    <div className="space-y-2">
                      <Skeleton className="h-3 w-24" />
                      <Skeleton className="h-5 w-14" />
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </>
        ) : stats.length > 0 ? (
          stats.map((stat) => (
            <Card key={stat.title}>
              <CardContent className="p-4">
                <div className="flex items-center gap-3">
                  <div className={cn("p-2 rounded-lg", stat.bgColor)}>
                    <stat.icon className={cn("h-5 w-5", stat.color)} />
                  </div>
                  <div>
                    <p className="text-xs font-medium text-muted-foreground">
                      {stat.title}
                    </p>
                    <p className="text-lg font-bold text-foreground">
                      {stat.value}
                    </p>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))
        ) : (
          <div className="col-span-5 text-center py-8 text-muted-foreground">Nenhuma estatística disponível</div>
        )}
      </div>

      {/* Filtros */}
      <Card>
        <CardContent className="p-4 md:p-6">
          {loading ? (
            <div className="flex flex-wrap gap-4 items-end">
              <div className="space-y-2 w-full sm:w-40">
                <Skeleton className="h-4 w-20" />
                <Skeleton className="h-10 w-full" />
              </div>
              <div className="space-y-2 w-full sm:w-40">
                <Skeleton className="h-4 w-16" />
                <Skeleton className="h-10 w-full" />
              </div>
              <div className="space-y-2 w-full sm:flex-1 sm:min-w-[200px]">
                <Skeleton className="h-4 w-16" />
                <Skeleton className="h-10 w-full" />
              </div>
              <div className="space-y-2 w-full sm:flex-1 sm:min-w-[200px]">
                <Skeleton className="h-4 w-20" />
                <Skeleton className="h-10 w-full" />
              </div>
              <div className="space-y-2 w-full sm:w-48">
                <Skeleton className="h-4 w-14" />
                <Skeleton className="h-10 w-full" />
              </div>
              <div className="flex gap-3 w-full sm:w-auto">
                <Skeleton className="h-10 w-24" />
                <Skeleton className="h-10 w-20" />
              </div>
            </div>
          ) : (
          <div className="flex flex-wrap gap-4 items-end">
            <div className="space-y-2 w-full sm:w-40">
              <Label>Data Início</Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !dataInicio && "text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {dataInicio ? format(dataInicio, "dd/MM/yyyy") : "Selecione"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0 z-50" align="start">
                  <Calendar
                    mode="single"
                    selected={dataInicio}
                    onSelect={setDataInicio}
                    initialFocus
                    className="pointer-events-auto"
                  />
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2 w-full sm:w-40">
              <Label>Data Fim</Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !dataFim && "text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {dataFim ? format(dataFim, "dd/MM/yyyy") : "Selecione"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0 z-50" align="start">
                  <Calendar
                    mode="single"
                    selected={dataFim}
                    onSelect={setDataFim}
                    disabled={(date) => {
                      const today = new Date();
                      today.setHours(23, 59, 59, 999);
                      return date > today;
                    }}
                    initialFocus
                    className="pointer-events-auto"
                  />
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2 w-full sm:flex-1 sm:min-w-[200px]">
              <Label>Cliente</Label>
              <Select value={cliente || "todos"} onValueChange={handleClienteChange}>
                <SelectTrigger className="bg-background">
                  <SelectValue placeholder="Selecione o cliente" />
                </SelectTrigger>
                <SelectContent className="bg-white dark:bg-gray-800 z-50">
                  <SelectItem value="todos">Todos</SelectItem>
                  {clientesList.map((clienteItem) => (
                    <SelectItem key={clienteItem.codigoCliente} value={clienteItem.codigoCliente}>
                      {clienteItem.labelCodigoCliente}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2 w-full sm:flex-1 sm:min-w-[200px]">
              <Label>Projeto</Label>
              <Select 
                value={projeto || "todos"} 
                onValueChange={(value) => setProjeto(value === "todos" ? undefined : value)}
                disabled={!cliente || cliente === "todos"}
              >
                <SelectTrigger className="bg-background">
                  <SelectValue placeholder={cliente ? "Selecione o projeto" : "Selecione um cliente primeiro"} />
                </SelectTrigger>
                <SelectContent className="bg-white dark:bg-gray-800 z-50">
                  <SelectItem value="todos">Todos</SelectItem>
                  {projetosList.map((projetoItem) => (
                    <SelectItem key={projetoItem.codigoProjeto} value={projetoItem.codigoProjeto}>
                      {projetoItem.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2 w-full sm:w-48">
              <Label>Status</Label>
              <Select value={status || undefined} onValueChange={setStatus}>
                <SelectTrigger className="bg-background">
                  <SelectValue placeholder="Selecione o status" />
                </SelectTrigger>
                <SelectContent className="bg-white dark:bg-gray-800 z-50">
                  {statusList.map((statusItem) => (
                    <SelectItem key={statusItem.id} value={statusItem.descricao.toLowerCase()}>
                      {statusItem.descricao}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="flex gap-3 w-full sm:w-auto">
              <Button onClick={handleBuscar} className="flex-1 sm:flex-initial">Buscar</Button>
              <Button variant="outline" onClick={handleLimpar} className="flex-1 sm:flex-initial">Limpar</Button>
            </div>
          </div>
          )}
        </CardContent>
      </Card>

      {/* Tabela */}
      <Card>
        <CardContent className="p-4 md:p-6">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
            <h2 className="text-xl font-semibold">Aprovações</h2>
            <div className="relative w-full sm:w-80">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Busca"
                value={busca}
                onChange={(e) => setBusca(e.target.value)}
                className="pl-10"
              />
            </div>
          </div>

          {loading ? (
            <div className="space-y-3">
              <div className="flex gap-4 overflow-hidden">
                {[1, 2, 3, 4, 5, 6, 7, 8, 9].map((i) => (
                  <Skeleton key={i} className="h-10 flex-1 min-w-[80px]" />
                ))}
              </div>
              {[1, 2, 3, 4, 5, 6].map((row) => (
                <div key={row} className="flex gap-4 overflow-hidden">
                  {[1, 2, 3, 4, 5, 6, 7, 8, 9].map((i) => (
                    <Skeleton key={i} className="h-10 flex-1 min-w-[80px]" />
                  ))}
                </div>
              ))}
            </div>
          ) : (
            <>
              <DataTable
                columns={columns}
                data={paginatedSolicitacoes}
                keyExtractor={(item) => item.id.toString()}
                renderCell={renderCell}
                emptyMessage="Nenhuma solicitação encontrada"
                stickyColumnId="acoes"
              />
              {totalItems > 0 && (
                <TablePagination
                  currentPage={currentPage}
                  totalItems={totalItems}
                  itemsPerPage={itemsPerPage}
                  onPageChange={setCurrentPage}
                  onItemsPerPageChange={(items) => {
                    setItemsPerPage(Number(items));
                    setCurrentPage(1);
                  }}
                />
              )}
            </>
          )}
        </CardContent>
      </Card>

      {/* Modal de Detalhes */}
      <Dialog 
        open={modalDetalhesOpen} 
        onOpenChange={(open) => {
          setModalDetalhesOpen(open);
          if (!open) {
            setSolicitacaoSelecionada(null);
          }
        }}
      >
        <DialogContent className="max-w-6xl max-h-[90vh] overflow-hidden flex flex-col p-0 gap-0">
          {/* Header */}
          <div className="p-6 border-b bg-muted/30">
            <DialogHeader>
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                  <Eye className="h-5 w-5 text-primary" />
                </div>
                <div className="flex-1">
                  <DialogTitle className="text-2xl font-bold">Detalhes da solicitação de reembolso</DialogTitle>
                  <DialogDescription className="mt-1">
                    Visualize todas as informações da solicitação
                  </DialogDescription>
                </div>
              </div>
            </DialogHeader>
          </div>

          {/* Content - Scrollable */}
          <div className="flex-1 overflow-y-auto p-6">
            {solicitacaoSelecionada ? (
              <div className="space-y-6">
                {/* Nome do Colaborador */}
                <div className="mb-4">
                  <h3 className="text-xl font-bold text-foreground">{solicitacaoSelecionada.nomeColaborador || 'N/A'}</h3>
                </div>

                {/* Informações do cabeçalho */}
                <Card>
                  <CardContent className="p-6">
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                      <div className="space-y-1">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Objetivo</span>
                        <p className="text-sm font-semibold text-foreground">{solicitacaoSelecionada.objetivo || "-"}</p>
                      </div>
                      <div className="space-y-1">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Destino</span>
                        <p className="text-sm font-semibold text-foreground">{solicitacaoSelecionada.destino || "-"}</p>
                      </div>
                      <div className="space-y-1">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Período</span>
                        <p className="text-sm font-semibold text-foreground">{solicitacaoSelecionada.periodo || "-"}</p>
                      </div>
                      <div className="space-y-1 md:col-span-2">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Cliente / Projeto</span>
                        <p className="text-sm font-semibold text-foreground">{solicitacaoSelecionada.cliente} / {solicitacaoSelecionada.projeto}</p>
                      </div>
                      <div className="space-y-1">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Valor Total</span>
                        <p className="text-lg font-bold text-primary">{formatCurrency(solicitacaoSelecionada.somaValores)}</p>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Tabela de itens */}
                <Card>
                  <CardContent className="p-6">
                    <div className="mb-4">
                      <h3 className="text-lg font-semibold text-foreground">Itens da Solicitação</h3>
                      <p className="text-sm text-muted-foreground mt-1">
                        {solicitacaoSelecionada.objeto?.length || 0} item(ns) encontrado(s)
                      </p>
                    </div>
                    
                    {solicitacaoSelecionada.objeto && solicitacaoSelecionada.objeto.length > 0 ? (
                      <DataTable
                        columns={[
                          { id: "valor", label: "Valor", sortable: true },
                          { id: "status", label: "Status", sortable: true, width: "w-[120px]" },
                          { id: "observacao", label: "Obs", sortable: false },
                          { id: "arquivo", label: "Arquivo", sortable: false, width: "w-[100px]" },
                        ]}
                        data={solicitacaoSelecionada.objeto}
                        keyExtractor={(item) => item.id.toString()}
                        renderCell={(item: SolicitacaoObjetoGerenteProjeto, columnId: string) => {
                          switch (columnId) {
                            case "valor":
                              return <span className="font-medium">{formatCurrency(parseFloat(item.valor))}</span>;
                            case "status":
                              return <StatusBadge status={item.status} variant="reembolso" />;
                            case "observacao":
                              return <span className="text-sm text-muted-foreground">{item.observacao || "-"}</span>;
                            case "arquivo":
                              return item.solicitacaoDocumentos && item.solicitacaoDocumentos.length > 0 ? (
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={() => {
                                    setDocumentosSelecionados(item.solicitacaoDocumentos || []);
                                    setModalDocumentosOpen(true);
                                  }}
                                  className="text-primary hover:text-primary"
                                >
                                  <Download className="h-4 w-4 mr-2" />
                                  {item.solicitacaoDocumentos.length}
                                </Button>
                              ) : (
                                <span className="text-muted-foreground">-</span>
                              );
                            default:
                              return null;
                          }
                        }}
                        emptyMessage="Nenhum item encontrado"
                      />
                    ) : (
                      <div className="text-center py-8 text-muted-foreground">
                        Nenhum item encontrado
                      </div>
                    )}
                  </CardContent>
                </Card>
              </div>
            ) : (
              <div className="text-center py-8 text-muted-foreground">
                Carregando detalhes...
              </div>
            )}
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal de Visualização de Documentos */}
      <Dialog open={modalDocumentosOpen} onOpenChange={setModalDocumentosOpen}>
        <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                  <Download className="h-5 w-5 text-primary" />
                </div>
                <DialogTitle className="text-2xl font-bold">Documentos</DialogTitle>
              </div>
            </div>
            <DialogDescription>
              Visualize e baixe os documentos anexados
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4">
            {documentosSelecionados.length > 0 ? (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {documentosSelecionados.map((documento) => (
                  <Card key={documento.id} className="hover:shadow-md transition-shadow">
                    <CardContent className="p-4">
                      <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3 flex-1 min-w-0">
                          <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center shrink-0">
                            <Download className="h-5 w-5 text-primary" />
                          </div>
                          <div className="flex-1 min-w-0">
                            <p className="font-medium truncate">Documento {documento.id}</p>
                            <p className="text-sm text-muted-foreground uppercase">{documento.tipo}</p>
                          </div>
                        </div>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => {
                            const urlProcessada = processarUrlComToken(documento.url, token);
                            if (urlProcessada) {
                              window.open(urlProcessada, "_blank");
                            } else {
                              console.error('Erro ao processar URL do documento');
                            }
                          }}
                        >
                          <Download className="h-4 w-4 mr-2" />
                          Baixar
                        </Button>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            ) : (
              <div className="text-center py-8 text-muted-foreground">
                Nenhum documento disponível
              </div>
            )}
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};
