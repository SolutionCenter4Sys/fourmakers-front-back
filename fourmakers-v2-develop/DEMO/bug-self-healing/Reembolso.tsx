import { useState, useEffect, useMemo } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Receipt, Users, CheckSquare, Eye, Download, FileText, Zap, Loader2, Search, CalendarIcon } from "@/components/ui/system-icons";
import { GestaoAdmTab } from "@presentation/components/reembolso/GestaoAdmTab";
import { AprovacoesTab } from "@presentation/components/reembolso/AprovacoesTab";
import { StatCard } from "@presentation/components/common/StatCard";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { cn } from "@/lib/utils";
import { DataTable, PageBreadcrumb, PageHeader, TablePagination } from "@presentation/components/common";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";
import type { Column } from "@/hooks/useColumnReorder";
import { useReembolsos, type ReembolsoTableRow } from "@/hooks/useReembolsos";
import { formatCurrency } from "@shared/utils/calculations";
import { StatusBadge } from "@presentation/components/common/StatusBadge";
import type { SolicitacaoReembolso, SolicitacaoObjeto, SolicitacaoDocumento } from "@domain/entities/SolicitacaoReembolso";
import { container } from "@core/di/container";
import { DiTokens } from "@core/di/tokens";
import { ReembolsosApi } from "@data/api/ReembolsosApi";
import { useAppSelector } from "../../app/store/hooks";
import { useParametros } from "@/hooks/useParametros";
import { Skeleton } from "@/components/ui/skeleton";
import { processarUrlComToken } from "@shared/utils/urlUtils";

// Mapeamento entre valores da URL e valores das abas
const tabUrlToValue: Record<string, string> = {
  'gestaoadm': 'gestao-adm',
  'aprovacoes': 'aprovacoes',
};

const tabValueToUrl: Record<string, string> = {
  'gestao-adm': 'gestaoadm',
  'aprovacoes': 'aprovacoes',
};

/** Demo QA — rota do botão "Solicitar Reembolso". Patch: demo-preparar / correção automatizada. */
const ROTA_NOVA_SOLICITACAO_REEMBOLSO = "/inserir-reembolso-invalido"; // DEMO_BUG_ATIVO: true

export default function Reembolso() {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const [dataInicio, setDataInicio] = useState<Date>();
  const [dataFim, setDataFim] = useState<Date>();
  const [modalDetalhesOpen, setModalDetalhesOpen] = useState(false);
  const [modalDocumentosOpen, setModalDocumentosOpen] = useState(false);
  const [reembolsoSelecionado, setReembolsoSelecionado] = useState<SolicitacaoReembolso | null>(null);
  const [documentosSelecionados, setDocumentosSelecionados] = useState<SolicitacaoDocumento[]>([]);
  const [modalRelatorioOpen, setModalRelatorioOpen] = useState(false);
  const [gerandoRelatorio, setGerandoRelatorio] = useState(false);
  const [buscaReembolsos, setBuscaReembolsos] = useState("");
  const [currentPageReembolsos, setCurrentPageReembolsos] = useState(1);
  const [itemsPerPageReembolsos, setItemsPerPageReembolsos] = useState(10);
  const { reembolsos, stats, loading, souAprovador, souGestor, solicitacoes } = useReembolsos(dataInicio, dataFim);
  const { token } = useAppSelector((state) => state.auth);
  const { getParametro, parametros } = useParametros();

  // Botão Remessa CNAB só visível se o usuário tiver o parâmetro HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO
  const exibirBotaoRemessaCNAB = useMemo(
    () => parametros.some((p) => p.codigoParametro === "HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO"),
    [parametros]
  );

  const [activeTab, setActiveTab] = useState<string>(() => {
    const tabParam = searchParams.get('tab');
    if (tabParam && tabUrlToValue[tabParam]) {
      return tabUrlToValue[tabParam];
    }
    return 'meus-reembolsos';
  });

  // Sincronizar com a URL quando ela mudar, respeitando permissões
  useEffect(() => {
    // Aguardar o carregamento dos dados antes de verificar permissões
    if (loading) return;
    
    const tabParam = searchParams.get('tab');
    const tabFromUrl = tabParam && tabUrlToValue[tabParam] 
      ? tabUrlToValue[tabParam] 
      : 'meus-reembolsos';
    
    // Verificar permissões antes de definir a tab
    if (tabFromUrl === 'gestao-adm' && !souGestor) {
      setActiveTab('meus-reembolsos');
      setSearchParams({}, { replace: true });
      return;
    }
    if (tabFromUrl === 'aprovacoes' && !souAprovador) {
      setActiveTab('meus-reembolsos');
      setSearchParams({}, { replace: true });
      return;
    }
    
    setActiveTab(tabFromUrl);
  }, [searchParams, souGestor, souAprovador, loading]);

  // Atualizar URL quando a aba mudar
  const handleTabChange = (value: string) => {
    // Verificar permissões antes de permitir mudança
    if (value === 'gestao-adm' && !souGestor) {
      return;
    }
    if (value === 'aprovacoes' && !souAprovador) {
      return;
    }
    
    setActiveTab(value);
    const urlValue = tabValueToUrl[value];
    if (urlValue) {
      setSearchParams({ tab: urlValue });
    } else {
      // Se for "meus-reembolsos", remove o parâmetro tab
      setSearchParams({});
    }
  };

  // Verificar se deve exibir o botão Gerar Relatório
  const exibirBotaoGerarRelatorio = useMemo(() => {
    const valorParametro = getParametro('EXIBIR_BOTAO_GERAR_PAGAMENTOS');
    
    // Se o parâmetro não existir: ocultar
    if (!valorParametro) return false;
    
    const valorLower = valorParametro.toLowerCase().trim();
    
    // Se 'TRUE' ou '1': exibir apenas se estiver na aba gestao-adm
    if (valorLower === 'true' || valorLower === '1') {
      return activeTab === 'gestao-adm';
    }
    
    // Se 'FALSE' ou '0': ocultar
    if (valorLower === 'false' || valorLower === '0') {
      return false;
    }
    
    // Caso padrão: ocultar
    return false;
  }, [getParametro, activeTab]);

  // Filtrar reembolsos com base na busca
  const filteredReembolsos = reembolsos.filter((reembolso) => {
    if (!buscaReembolsos) return true;
    const searchLower = buscaReembolsos.toLowerCase();
    return (
      reembolso.objetivo?.toLowerCase().includes(searchLower) ||
      reembolso.destino?.toLowerCase().includes(searchLower) ||
      reembolso.periodo?.toLowerCase().includes(searchLower) ||
      reembolso.clienteProjeto?.toLowerCase().includes(searchLower) ||
      reembolso.somaValores?.toString().toLowerCase().includes(searchLower) ||
      reembolso.dataSolicitacao?.toLowerCase().includes(searchLower)
    );
  });

  // Calcular dados paginados para reembolsos
  const totalItemsReembolsos = filteredReembolsos.length;
  const startIndexReembolsos = (currentPageReembolsos - 1) * itemsPerPageReembolsos;
  const endIndexReembolsos = startIndexReembolsos + itemsPerPageReembolsos;
  const paginatedReembolsos = filteredReembolsos.slice(startIndexReembolsos, endIndexReembolsos);

  // Resetar página quando busca mudar
  useEffect(() => {
    setCurrentPageReembolsos(1);
  }, [buscaReembolsos]);

  // Função para gerar e baixar relatório
  const handleGerarRelatorio = async () => {
    if (!token) {
      console.error('Token não encontrado');
      return;
    }

    setGerandoRelatorio(true);
    try {
      const reembolsosApi = container.resolve<ReembolsosApi>(DiTokens.reembolsosApi);
      const { arrayBuffer, contentType, fileName } = await reembolsosApi.gerarRelatorioSolicitacoesAguardandoPagamento(token);
      
      // Criar Blob a partir do ArrayBuffer com o Content-Type correto
      const blob = new Blob([arrayBuffer], { type: contentType });
      
      // Criar URL do objeto Blob
      const url = window.URL.createObjectURL(blob);
      
      // Criar link de download
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', fileName);
      document.body.appendChild(link);
      link.click();
      
      // Limpar: remover link e revogar URL do objeto
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
      
      setModalRelatorioOpen(false);
    } catch (error) {
      console.error('Erro ao gerar relatório:', error);
      alert('Erro ao gerar relatório. Tente novamente.');
    } finally {
      setGerandoRelatorio(false);
    }
  };

  const columns: Column[] = [
    { id: "objetivo", label: "Objetivo", sortable: true },
    { id: "destino", label: "Destino", sortable: true },
    { id: "periodo", label: "Periodo", sortable: true },
    { id: "clienteProjeto", label: "Cliente/Projeto", sortable: true },
    { id: "somaValores", label: "Soma valores", sortable: true },
    { id: "dataSolicitacao", label: "Data de solicitação", sortable: true },
    { id: "acoes", label: "Ações", sortable: false },
  ];

  const renderCell = (reembolso: ReembolsoTableRow, columnId: string) => {
    switch (columnId) {
      case "objetivo":
        return <span className="font-medium">{reembolso.objetivo || '-'}</span>;
      case "destino":
        return <span>{reembolso.destino || '-'}</span>;
      case "periodo":
        return <span>{reembolso.periodo || '-'}</span>;
      case "clienteProjeto":
        return <span className="font-medium">{reembolso.clienteProjeto}</span>;
      case "somaValores":
        return <span className="font-medium">{reembolso.somaValores}</span>;
      case "dataSolicitacao":
        return <span>{reembolso.dataSolicitacao}</span>;
      case "acoes":
        return (
          <Button 
            variant="ghost" 
            size="sm"
            onClick={() => {
              // Buscar a solicitação completa da lista de solicitações usando o objeto
              // O objeto já contém todos os dados necessários, então podemos buscar pela correspondência
              const solicitacaoCompleta = solicitacoes.find((s) => {
                // Comparar usando o primeiro item do objeto se existir
                if (s.objeto && s.objeto.length > 0 && reembolso.objeto && reembolso.objeto.length > 0) {
                  return s.objeto[0].id === reembolso.objeto[0].id;
                }
                // Fallback: comparar por período, objetivo e cliente/projeto
                const periodoMatch = s.periodo === reembolso.periodo;
                const objetivoMatch = s.objetivo === reembolso.objetivo;
                const clienteProjetoMatch = `${s.cliente} / ${s.projeto}` === reembolso.clienteProjeto;
                return periodoMatch && objetivoMatch && clienteProjetoMatch;
              });
              
              if (solicitacaoCompleta) {
                setReembolsoSelecionado(solicitacaoCompleta);
                setModalDetalhesOpen(true);
              }
            }}
          >
            <Eye className="h-4 w-4 mr-2" />
            Detalhes
          </Button>
        );
      default:
        return null;
    }
  };

  return (
    <div className="container mx-auto p-4 space-y-4">
      <PageBreadcrumb 
        items={[
          { label: 'Reembolso' }
        ]} 
      />
      
      <PageHeader 
        title="Reembolso"
        actions={
          <>
            {exibirBotaoGerarRelatorio && (
              <Button 
                onClick={() => setModalRelatorioOpen(true)}
                className="border-foreground shadow-sm w-full md:w-auto"
                variant="outline"
              >
                <FileText className="h-4 w-4 mr-2" />
                Gerar Relatório
              </Button>
            )}
            {activeTab === "gestao-adm" && exibirBotaoRemessaCNAB && (
              <Button
                onClick={() => navigate("/reembolso/remessa-cnab")}
                className="border-foreground shadow-sm w-full md:w-auto"
                variant="outline"
              >
                <FileText className="h-4 w-4 mr-2" />
                Remessa CNAB
              </Button>
            )}
            <Button onClick={() => navigate(ROTA_NOVA_SOLICITACAO_REEMBOLSO)} className="px-6 shadow-sm w-full md:w-auto">
              Solicitar Reembolso
            </Button>
          </>
        }
      />

      {/* Tabs */}
      <Tabs value={activeTab} onValueChange={handleTabChange} className="w-full">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-6">
          <div className="overflow-x-auto -mx-4 px-4 md:mx-0 md:px-0">
            <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm min-w-max">
            <TabsTrigger 
              value="meus-reembolsos" 
              className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <Receipt className="h-4 w-4" />
              <span>Meus Reembolsos</span>
              <span className="ml-1 px-2 py-0.5 text-xs font-semibold rounded-full bg-primary-foreground/20 data-[state=active]:bg-primary-foreground/30">
                {reembolsos.length}
              </span>
            </TabsTrigger>
            {souGestor && (
              <TabsTrigger 
                value="gestao-adm" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <Users className="h-4 w-4" />
                <span>Gestão ADM</span>
              </TabsTrigger>
            )}
            {souAprovador && (
              <TabsTrigger 
                value="aprovacoes" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <CheckSquare className="h-4 w-4" />
                <span>Aprovações</span>
              </TabsTrigger>
            )}
          </TabsList>
          </div>
        </div>

        <TabsContent value="meus-reembolsos" className="space-y-6">
          {/* Big Numbers */}
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {loading ? (
              <>
                {[1, 2, 3].map((i) => (
                  <Card key={i}>
                    <CardContent className="p-6">
                      <div className="flex items-center gap-4">
                        <Skeleton className="h-12 w-12 rounded-lg" />
                        <div className="space-y-2 flex-1">
                          <Skeleton className="h-4 w-24" />
                          <Skeleton className="h-7 w-16" />
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </>
            ) : (
              stats.map((stat) => (
                <StatCard 
                  key={stat.title}
                  title={stat.title}
                  value={stat.value}
                  icon={stat.icon}
                  color={stat.color}
                  bgColor={stat.bgColor}
                />
              ))
            )}
          </div>

          <Card>
            <CardContent className="p-6">
              {loading ? (
                <div className="flex flex-wrap gap-4 items-end">
                  <div className="flex-1 min-w-[200px] space-y-2">
                    <Skeleton className="h-4 w-20" />
                    <Skeleton className="h-10 w-full" />
                  </div>
                  <div className="flex-1 min-w-[200px] space-y-2">
                    <Skeleton className="h-4 w-16" />
                    <Skeleton className="h-10 w-full" />
                  </div>
                  <Skeleton className="h-10 w-24" />
                </div>
              ) : (
                <div className="flex flex-wrap gap-4 items-end">
                  <div className="flex-1 min-w-[200px]">
                    <label className="text-sm font-medium mb-2 block">Data Início</label>
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
                          {dataInicio ? format(dataInicio, "PPP", { locale: ptBR }) : "Selecione"}
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent className="w-auto p-0" align="start">
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

                  <div className="flex-1 min-w-[200px]">
                    <label className="text-sm font-medium mb-2 block">Data Fim</label>
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
                          {dataFim ? format(dataFim, "PPP", { locale: ptBR }) : "Selecione"}
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent className="w-auto p-0" align="start">
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

                  {(dataInicio || dataFim) && (
                    <Button
                      variant="outline"
                      onClick={() => {
                        setDataInicio(undefined);
                        setDataFim(undefined);
                      }}
                      className="flex-shrink-0"
                    >
                      Limpar
                    </Button>
                  )}
                </div>
              )}
            </CardContent>
          </Card>

            {/* Tabela */}
            <Card>
              <CardContent className="p-6">
                <div className="flex items-center justify-between mb-6">
                  <h2 className="text-xl font-semibold">Meus Reembolsos</h2>
                  <div className="relative w-80">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      placeholder="Busca"
                      value={buscaReembolsos}
                      onChange={(e) => setBuscaReembolsos(e.target.value)}
                      className="pl-10"
                    />
                  </div>
                </div>
                {loading ? (
                  <div className="space-y-3">
                    <div className="flex gap-4 overflow-hidden">
                      {[1, 2, 3, 4, 5, 6, 7].map((i) => (
                        <Skeleton key={i} className="h-10 flex-1 min-w-[100px]" />
                      ))}
                    </div>
                    {[1, 2, 3, 4, 5].map((row) => (
                      <div key={row} className="flex gap-4 overflow-hidden">
                        {[1, 2, 3, 4, 5, 6, 7].map((i) => (
                          <Skeleton key={i} className="h-10 flex-1 min-w-[100px]" />
                        ))}
                      </div>
                    ))}
                  </div>
                ) : (
                  <>
                    <DataTable
                      columns={columns}
                      data={paginatedReembolsos}
                      keyExtractor={(item) => item.id.toString()}
                      renderCell={renderCell}
                      emptyMessage="Nenhum reembolso encontrado"
                    />
                    {totalItemsReembolsos > 0 && (
                      <TablePagination
                        currentPage={currentPageReembolsos}
                        totalItems={totalItemsReembolsos}
                        itemsPerPage={itemsPerPageReembolsos}
                        onPageChange={setCurrentPageReembolsos}
                        onItemsPerPageChange={(items) => {
                          setItemsPerPageReembolsos(Number(items));
                          setCurrentPageReembolsos(1);
                        }}
                      />
                    )}
                  </>
                )}
              </CardContent>
            </Card>
        </TabsContent>

        {souGestor && (
          <TabsContent value="gestao-adm" className="space-y-6">
            <GestaoAdmTab />
          </TabsContent>
        )}

        {souAprovador && (
          <TabsContent value="aprovacoes">
            <AprovacoesTab />
          </TabsContent>
        )}
      </Tabs>

      {/* Modal de Detalhes do Reembolso */}
      <Dialog open={modalDetalhesOpen} onOpenChange={setModalDetalhesOpen}>
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
            {reembolsoSelecionado && (
              <div className="space-y-6">
                {/* Informações do cabeçalho */}
                <Card>
                  <CardContent className="p-6">
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                      <div className="space-y-1">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Objetivo</span>
                        <p className="text-sm font-semibold text-foreground">{reembolsoSelecionado.objetivo || "-"}</p>
                      </div>
                      <div className="space-y-1">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Destino</span>
                        <p className="text-sm font-semibold text-foreground">{reembolsoSelecionado.destino || "-"}</p>
                      </div>
                      <div className="space-y-1">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Período</span>
                        <p className="text-sm font-semibold text-foreground">{reembolsoSelecionado.periodo || "-"}</p>
                      </div>
                      <div className="space-y-1 md:col-span-2">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Cliente / Projeto</span>
                        <p className="text-sm font-semibold text-foreground">{reembolsoSelecionado.cliente} / {reembolsoSelecionado.projeto}</p>
                      </div>
                      <div className="space-y-1">
                        <span className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Valor Total</span>
                        <p className="text-lg font-bold text-primary">{formatCurrency(reembolsoSelecionado.somaValores)}</p>
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
                        {reembolsoSelecionado.objeto?.length || 0} item(ns) encontrado(s)
                      </p>
                    </div>
                    
                    {reembolsoSelecionado.objeto && reembolsoSelecionado.objeto.length > 0 ? (
                      <DataTable
                        columns={[
                          { id: "categoria", label: "Categoria", sortable: true },
                          { id: "valorSolicitado", label: "Valor Solicitado", sortable: true },
                          { id: "valorAprovado", label: "Valor Aprovado", sortable: true },
                          { id: "observacao", label: "Observação", sortable: false },
                          { id: "arquivo", label: "Arquivo", sortable: false, width: "w-[100px]" },
                          { id: "status", label: "Status", sortable: true, width: "w-[120px]" },
                        ]}
                        data={reembolsoSelecionado.objeto}
                        keyExtractor={(item) => item.id.toString()}
                        renderCell={(item: SolicitacaoObjeto, columnId: string) => {
                          switch (columnId) {
                            case "categoria":
                              return <span className="font-medium">{item.categoria}</span>;
                            case "valorSolicitado":
                              return <span className="font-medium">{formatCurrency(item.valorSolicitado)}</span>;
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
                                    setDocumentosSelecionados(item.solicitacaoDocumentos);
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
                    ) : (
                      <div className="text-center py-8 text-muted-foreground">
                        Nenhum item encontrado
                      </div>
                    )}
                  </CardContent>
                </Card>
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

      {/* Modal de Confirmação - Gerar Relatório */}
      <AlertDialog open={modalRelatorioOpen} onOpenChange={(open) => {
        if (!gerandoRelatorio) {
          setModalRelatorioOpen(open);
        }
      }}>
        <AlertDialogContent className="sm:max-w-lg">
          <AlertDialogHeader>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                <Zap className="h-5 w-5 text-primary" />
              </div>
              <AlertDialogTitle className="text-2xl font-bold">Gerar Relatório</AlertDialogTitle>
            </div>
            <AlertDialogDescription className="text-base text-muted-foreground mt-4">
              Confirma geração do relatório?
            </AlertDialogDescription>
            <AlertDialogDescription className="text-sm text-muted-foreground mt-2">
              Você está prestes a gerar um relatório de lançamentos com status "Aprovado/Aguardando Pagamento"
            </AlertDialogDescription>
          </AlertDialogHeader>
          
          {gerandoRelatorio ? (
            <div className="flex flex-col items-center justify-center py-6 space-y-4">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <p className="text-sm font-medium text-foreground">Aguarde estamos preparando o download</p>
            </div>
          ) : (
            <AlertDialogFooter>
              <AlertDialogCancel>Fechar</AlertDialogCancel>
              <AlertDialogAction 
                onClick={handleGerarRelatorio}
                className="bg-primary hover:bg-primary/90"
              >
                Confirmar
              </AlertDialogAction>
            </AlertDialogFooter>
          )}
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
