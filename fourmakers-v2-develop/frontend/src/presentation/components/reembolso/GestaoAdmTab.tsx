import { useState, useEffect } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { CalendarIcon, Search, Eye, DollarSign, CheckCircle2, Download } from "@/components/ui/system-icons";
import { Checkbox } from "@/components/ui/checkbox";
import { AlertDialog, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";
import { ReembolsosApi } from "@data/api/ReembolsosApi";
import { useAppSelector } from "@/app/store/hooks";
import { useToast } from "@/hooks/use-toast";
import { format } from "date-fns";
import { cn } from "@/lib/utils";
import { DataTable, TablePagination } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { useGestaoAdmTab } from "@/hooks/useReembolsoComponentes";
import { formatCurrency } from "@shared/utils/calculations";
import { StatusBadge } from "@presentation/components/common/StatusBadge";
import { Skeleton } from "@/components/ui/skeleton";
import type { SolicitacaoVisaoAdm, SolicitacaoObjetoVisaoAdm, SolicitacaoDocumento } from "@domain/entities/SolicitacaoReembolso";
import { processarUrlComToken } from "@shared/utils/urlUtils";

export const GestaoAdmTab = () => {
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
  const [solicitacaoSelecionada, setSolicitacaoSelecionada] = useState<SolicitacaoVisaoAdm | null>(null);
  const [modoBaixa, setModoBaixa] = useState(false);
  const [idsSelecionados, setIdsSelecionados] = useState<Set<number>>(new Set());
  const [modalConfirmarBaixaOpen, setModalConfirmarBaixaOpen] = useState(false);
  const [processandoBaixa, setProcessandoBaixa] = useState(false);
  const { token } = useAppSelector((state) => state.auth);
  const [filtrosAplicados, setFiltrosAplicados] = useState<{
    dataInicio?: Date;
    dataFim?: Date;
    codigoCliente?: string;
    codigoProjeto?: string;
    status?: string;
  }>({});
  
  const { stats, colaboradores, statusList, clientesList, projetosList, loading, loadProjetos } = useGestaoAdmTab(filtrosAplicados);
  const { toast } = useToast();
  
  const confirmarBaixas = async () => {
    if (!token) {
      console.error('Token não disponível');
      return;
    }
    
    if (idsSelecionados.size === 0) {
      console.error('Nenhum ID selecionado');
      return;
    }
    
    const idsArray = Array.from(idsSelecionados);
    console.log('=== CONFIRMAR BAIXAS ===');
    console.log('IDs selecionados:', idsArray);
    console.log('Quantidade:', idsArray.length);
    console.log('Payload que será enviado:', JSON.stringify(idsArray));
    
    try {
      setProcessandoBaixa(true);
      // Criar nova instância diretamente para evitar problemas de cache do DI container
      const reembolsosApi = new ReembolsosApi();
      console.log('Chamando API gerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIds com IDs:', idsArray);
      
      const response = await reembolsosApi.gerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIds(
        token,
        idsArray
      );
      
      console.log('Resposta da API:', response);
      
      if (response.sucesso) {
        console.log('Baixas confirmadas com sucesso!');
        
        // Fechar todos os modais primeiro
        setModalConfirmarBaixaOpen(false);
        setModalDetalhesOpen(false);
        setModoBaixa(false);
        setIdsSelecionados(new Set());
        setSolicitacaoSelecionada(null);
        
        // Exibir toast de sucesso
        toast({
          title: "Baixas confirmadas",
          description: `${idsArray.length} lançamento(s) atualizado(s) com status "Pago" com sucesso.`,
          variant: "default",
        });
        
        // Recarregar dados - forçar atualização dos filtros para triggerar o useEffect
        // Criar um novo objeto para garantir que o React detecte a mudança
        const filtrosAtuais = { ...filtrosAplicados };
        setFiltrosAplicados({});
        // Usar setTimeout para garantir que o estado seja atualizado antes de recarregar
        setTimeout(() => {
          setFiltrosAplicados(filtrosAtuais);
        }, 50);
      } else {
        console.error('API retornou sucesso=false:', response);
        toast({
          title: "Erro ao confirmar baixas",
          description: response.mensagem || "Ocorreu um erro ao processar as baixas.",
          variant: "destructive",
        });
      }
    } catch (err) {
      console.error('Erro ao confirmar baixas:', err);
      if (err instanceof Error) {
        console.error('Mensagem de erro:', err.message);
        console.error('Stack:', err.stack);
      }
      
      // Fechar modais mesmo em caso de erro
      setModalConfirmarBaixaOpen(false);
      setModalDetalhesOpen(false);
      setModoBaixa(false);
      
      // Exibir toast de erro
      toast({
        title: "Erro ao confirmar baixas",
        description: err instanceof Error ? err.message : "Ocorreu um erro ao processar as baixas. Tente novamente.",
        variant: "destructive",
      });
    } finally {
      setProcessandoBaixa(false);
    }
  };

  // Filtrar colaboradores com base na busca
  const filteredColaboradores = colaboradores.filter((colaborador) => {
    if (!busca) return true;
    const searchLower = busca.toLowerCase();
    return (
      colaborador.nomeColaborador.toLowerCase().includes(searchLower) ||
      colaborador.objetivo?.toLowerCase().includes(searchLower) ||
      colaborador.destino?.toLowerCase().includes(searchLower) ||
      colaborador.cliente.toLowerCase().includes(searchLower) ||
      colaborador.projeto.toLowerCase().includes(searchLower) ||
      colaborador.somaValores.toLowerCase().includes(searchLower) ||
      colaborador.dataSolicitacao.toLowerCase().includes(searchLower)
    );
  });

  // Calcular dados paginados
  const totalItems = filteredColaboradores.length;
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const paginatedColaboradores = filteredColaboradores.slice(startIndex, endIndex);

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
    { id: "nomeColaborador", label: "Colaborador(a)", sortable: true },
    { id: "saldoAdiantamentos", label: "Saldo Adiantamento", sortable: true },
    { id: "objetivo", label: "Objetivo", sortable: true },
    { id: "destino", label: "Destino", sortable: true },
    { id: "periodo", label: "Período", sortable: true },
    { id: "clienteProjeto", label: "Cliente/Projeto", sortable: true },
    { id: "somaValores", label: "Soma de valores", sortable: true },
    { id: "dataSolicitacao", label: "Data de solicitação", sortable: true },
    { id: "pagoTotal", label: "Pago/Total", sortable: true },
    { id: "status", label: "Status", sortable: false },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[180px]" },
  ];

  // Função para contar status dos itens
  const contarStatusPorColaborador = (colaborador: typeof colaboradores[0]) => {
    if (!colaborador.objeto || colaborador.objeto.length === 0) {
      return {};
    }

    const contagem: Record<string, number> = {};
    colaborador.objeto.forEach((item) => {
      const status = item.status || 'Sem Status';
      contagem[status] = (contagem[status] || 0) + 1;
    });

    return contagem;
  };

  const renderCell = (colaborador: typeof colaboradores[0], columnId: string) => {
    switch (columnId) {
      case "nomeColaborador":
        return <span className="font-medium">{colaborador.nomeColaborador}</span>;
      case "saldoAdiantamentos":
        return <span className="font-medium tabular-nums">{formatCurrency(colaborador.saldoAdiantamentos)}</span>;
      case "objetivo":
        return <span>{colaborador.objetivo}</span>;
      case "destino":
        return <span>{colaborador.destino || '-'}</span>;
      case "periodo":
        return <span>{colaborador.periodo}</span>;
      case "clienteProjeto":
        const clienteProjetoText = `${colaborador.cliente} / ${colaborador.projeto}`;
        return (
          <div className="max-w-[200px]">
            <span 
              className="font-medium truncate block" 
              title={clienteProjetoText}
            >
              {clienteProjetoText}
            </span>
          </div>
        );
      case "somaValores":
        return <span className="font-semibold">{colaborador.somaValores}</span>;
      case "dataSolicitacao":
        return <span>{colaborador.dataSolicitacao}</span>;
      case "pagoTotal":
        return <span>{colaborador.pagoTotal}</span>;
      case "status":
        const contagemStatus = contarStatusPorColaborador(colaborador);
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
        // Verificar se há algum objeto com statusId = 3 (Aprovado)
        const temAprovado = colaborador.objeto?.some(item => item.statusId === 3) || false;
        
        return (
          <div className="flex items-center gap-2">
            <Button 
              variant="ghost" 
              size="sm"
              onClick={() => {
                // Buscar a solicitação completa da lista de colaboradores
                const solicitacaoCompleta = colaborador.solicitacaoCompleta;
                if (solicitacaoCompleta) {
                  setSolicitacaoSelecionada(solicitacaoCompleta);
                  setModoBaixa(false);
                  setIdsSelecionados(new Set());
                  setModalDetalhesOpen(true);
                }
              }}
            >
              <Eye className="h-4 w-4 mr-2" />
              Detalhes
            </Button>
            {temAprovado && (
              <Button 
                variant="ghost" 
                size="sm"
                onClick={() => {
                  const solicitacaoCompleta = colaborador.solicitacaoCompleta;
                  if (solicitacaoCompleta) {
                    setSolicitacaoSelecionada(solicitacaoCompleta);
                    setModoBaixa(true);
                    setIdsSelecionados(new Set());
                    setModalDetalhesOpen(true);
                  }
                }}
                className="text-primary hover:text-primary"
              >
                <DollarSign className="h-4 w-4 mr-2" />
                Baixar
              </Button>
            )}
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="space-y-6">
      {/* Filtros */}
      <Card>
        <CardContent className="p-4 md:p-6">
          {loading ? (
            <div className="flex flex-wrap gap-3 md:gap-4 items-end">
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
          <div className="flex flex-wrap gap-3 md:gap-4 items-end">
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

      {/* Big Numbers */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {loading ? (
          <>
            {[1, 2, 3].map((i) => (
              <Card key={i}>
                <CardContent className="p-4">
                  <div className="flex items-center gap-3">
                    <Skeleton className="h-9 w-9 rounded-lg" />
                    <div className="space-y-2">
                      <Skeleton className="h-3 w-20" />
                      <Skeleton className="h-5 w-12" />
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </>
        ) : (
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
        )}
      </div>

      {/* Tabela */}
      <Card>
        <CardContent className="p-4 md:p-6">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
            <h2 className="text-xl font-semibold">Colaboradores</h2>
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
                {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map((i) => (
                  <Skeleton key={i} className="h-10 flex-1 min-w-[80px]" />
                ))}
              </div>
              {[1, 2, 3, 4, 5, 6].map((row) => (
                <div key={row} className="flex gap-4 overflow-hidden">
                  {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map((i) => (
                    <Skeleton key={i} className="h-10 flex-1 min-w-[80px]" />
                  ))}
                </div>
              ))}
            </div>
          ) : (
            <>
              <DataTable
                columns={columns}
                data={paginatedColaboradores}
                keyExtractor={(item) => item.id.toString()}
                renderCell={renderCell}
                emptyMessage="Nenhum colaborador encontrado"
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

      {/* Modal de Detalhes do Reembolso */}
      <Dialog 
        open={modalDetalhesOpen} 
        onOpenChange={(open) => {
          if (!processandoBaixa) {
            setModalDetalhesOpen(open);
            if (!open) {
              // Limpar dados ao fechar o modal
              setSolicitacaoSelecionada(null);
              setModoBaixa(false);
              setIdsSelecionados(new Set());
            }
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

                {/* Tabela de itens usando DataTable */}
                <Card>
                  <CardContent className="p-6">
                    <div className="mb-4">
                      <h3 className="text-lg font-semibold text-foreground">Itens da Solicitação</h3>
                      <p className="text-sm text-muted-foreground mt-1">
                        {solicitacaoSelecionada.objeto?.length || 0} item(ns) encontrado(s)
                      </p>
                    </div>
                    
                    {solicitacaoSelecionada.objeto && solicitacaoSelecionada.objeto.length > 0 ? (
                      <>
                        {modoBaixa && (
                          <div className="mb-4 flex items-center justify-between">
                            <div className="flex items-center gap-2">
                              <Checkbox
                                id="select-all"
                                checked={
                                  solicitacaoSelecionada.objeto
                                    .filter(item => item.statusId === 3)
                                    .every(item => idsSelecionados.has(item.id))
                                }
                                onCheckedChange={(checked) => {
                                  const novosIds = new Set(idsSelecionados);
                                  const itensAprovados = solicitacaoSelecionada.objeto?.filter(item => item.statusId === 3) || [];
                                  if (checked) {
                                    itensAprovados.forEach(item => novosIds.add(item.id));
                                  } else {
                                    itensAprovados.forEach(item => novosIds.delete(item.id));
                                  }
                                  setIdsSelecionados(novosIds);
                                }}
                                disabled={processandoBaixa}
                              />
                              <Label htmlFor="select-all" className="cursor-pointer">
                                Selecionar todos os aprovados
                              </Label>
                            </div>
                            {idsSelecionados.size > 0 && (
                              <Button
                                onClick={() => setModalConfirmarBaixaOpen(true)}
                                disabled={processandoBaixa}
                                className="bg-primary hover:bg-primary/90"
                              >
                                <CheckCircle2 className="h-4 w-4 mr-2" />
                                Confirmar baixas ({idsSelecionados.size})
                              </Button>
                            )}
                          </div>
                        )}
                        <DataTable
                          columns={[
                            ...(modoBaixa ? [{ id: "selecao", label: "Seleção", sortable: false, width: "w-[80px]" }] : []),
                            { id: "categoria", label: "Categoria", sortable: true },
                            { id: "valorSolicitado", label: "Valor Solicitado", sortable: true },
                            { id: "valorAprovado", label: "Valor Aprovado", sortable: true },
                            { id: "observacao", label: "Obs", sortable: false },
                            { id: "arquivo", label: "Arquivo", sortable: false, width: "w-[100px]" },
                            { id: "status", label: "Status", sortable: true, width: "w-[120px]" },
                          ]}
                          data={solicitacaoSelecionada.objeto}
                          keyExtractor={(item) => item.id.toString()}
                          renderCell={(item: SolicitacaoObjetoVisaoAdm, columnId: string) => {
                            switch (columnId) {
                              case "selecao":
                                if (modoBaixa && item.statusId === 3) {
                                  return (
                                    <Checkbox
                                      checked={idsSelecionados.has(item.id)}
                                      onCheckedChange={(checked) => {
                                        const novosIds = new Set(idsSelecionados);
                                        if (checked) {
                                          novosIds.add(item.id);
                                        } else {
                                          novosIds.delete(item.id);
                                        }
                                        setIdsSelecionados(novosIds);
                                      }}
                                      disabled={processandoBaixa}
                                    />
                                  );
                                }
                                return null;
                              case "categoria":
                                return <span className="font-medium">{item.categoria || item.descricao || "-"}</span>;
                              case "valorSolicitado":
                                // Usar 'valor' se existir, senão usar 'valorSolicitado'
                                const valorSolicitado = item.valor ?? item.valorSolicitado ?? 0;
                                return <span className="font-medium">{formatCurrency(valorSolicitado)}</span>;
                              case "valorAprovado":
                                return <span className="font-medium">{formatCurrency(item.valorAprovado)}</span>;
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
                              case "status":
                                return <StatusBadge status={item.status} variant="reembolso" />;
                              default:
                                return null;
                            }
                          }}
                          emptyMessage="Nenhum item encontrado"
                        />
                      </>
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

      {/* Modal de Confirmação - Confirmar Baixas */}
      <AlertDialog open={modalConfirmarBaixaOpen} onOpenChange={(open) => {
        if (!processandoBaixa) {
          setModalConfirmarBaixaOpen(open);
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar Baixas</AlertDialogTitle>
            <AlertDialogDescription>
              Ao confirmar, todos os lançamentos selecionados serão atualizados com status "Pago".
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={processandoBaixa}>Cancelar</AlertDialogCancel>
            <Button
              onClick={async () => {
                console.log('Botão Confirmar clicado!');
                await confirmarBaixas();
              }}
              disabled={processandoBaixa}
              className="bg-green-600 hover:bg-green-700 text-white"
            >
              {processandoBaixa ? "Processando..." : "Confirmar"}
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

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
