import { useState, useEffect } from "react";
import { useAppSelector } from "@/app/store/hooks";
import { store } from "@/app/store";
import { container } from "@core/di/container";
import { IntegracaoContabilApi } from "@data/api/IntegracaoContabilApi";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { MonthPicker } from "@/components/ui/month-picker";
import { Badge } from "@/components/ui/badge";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { Upload, FileText, Eye, CheckCircle, Trash2, Download, Loader2 } from "@/components/ui/system-icons";
import { useToast } from "@/hooks/use-toast";
import { StatusBadge } from "@presentation/components/common/StatusBadge";
import { TablePagination } from "@presentation/components/common";
import {
  AlertDialog,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Separator } from "@/components/ui/separator";
import { Building2, Calendar, Hash } from "@/components/ui/system-icons";
import { DataTable } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { cn } from "@/lib/utils";
import { processarUrlComToken } from "@shared/utils/urlUtils";

import type { LoteHoleriteDTO, DetalharLoteRetornoDTO, ItemLoteDTO } from "@shared/types/integracaoContabilApi";
import { TIPOS_PROCESSAMENTO_HOLERITE } from "@shared/types/integracaoContabilApi";
import { useIntegracaoContabil } from "@/hooks/useIntegracaoContabil";
import { useUnidadesRemessa } from "@/hooks/useUnidadesRemessa";

function downloadBlob(blob: Blob, fileName: string) {
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = fileName;
  a.click();
  URL.revokeObjectURL(url);
}

const IntegracaoContabil = () => {
  const token = useAppSelector((state) => state.auth.token);
  const { lotes, loadingLotes, errorLotes, refetchLotes } = useIntegracaoContabil();
  const { unidades, loading: loadingUnidades, error: errorUnidades } = useUnidadesRemessa();
  const [selectedFileRetorno, setSelectedFileRetorno] = useState<File | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);

  /** Vigência (competência) para gerar remessa: mês 1-12 e ano */
  const [vigenciaMes, setVigenciaMes] = useState<number | undefined>(undefined);
  const [vigenciaAno, setVigenciaAno] = useState<number | undefined>(undefined);
  /** CNPJ selecionado (id da unidade) */
  const [cnpjSelecionado, setCnpjSelecionado] = useState<string>("");
  const [gerarRemessaLoading, setGerarRemessaLoading] = useState(false);

  const [selectedLote, setSelectedLote] = useState<LoteHoleriteDTO | null>(null);
  const [detalheLote, setDetalheLote] = useState<DetalharLoteRetornoDTO | null>(null);
  const [loadingDetalhe, setLoadingDetalhe] = useState(false);
  const [errorDetalhe, setErrorDetalhe] = useState<string | null>(null);
  const [detalhePage, setDetalhePage] = useState(1);
  const [detalhePageSize, setDetalhePageSize] = useState(10);

  const [loteParaProcessar, setLoteParaProcessar] = useState<LoteHoleriteDTO | null>(null);
  const [isProcessarModalOpen, setIsProcessarModalOpen] = useState(false);
  const [tipoProcessamentoSelecionado, setTipoProcessamentoSelecionado] = useState<number>(0);
  const [loadingProcessar, setLoadingProcessar] = useState(false);

  const [loteParaExcluir, setLoteParaExcluir] = useState<LoteHoleriteDTO | null>(null);
  const [loadingExcluir, setLoadingExcluir] = useState(false);
  const [uploadRetornoLoading, setUploadRetornoLoading] = useState(false);

  const [pageRetorno, setPageRetorno] = useState(1);
  const [pageSizeRetorno, setPageSizeRetorno] = useState(10);

  const { toast } = useToast();

  useEffect(() => {
    if (!isDetailModalOpen || !selectedLote?.id) {
      if (!isDetailModalOpen) {
        setDetalheLote(null);
        setErrorDetalhe(null);
        setDetalhePage(1);
      }
      return;
    }
    const currentToken = store.getState().auth.token;
    if (!currentToken) return;
    let cancelled = false;
    setLoadingDetalhe(true);
    setErrorDetalhe(null);
    const api = container.resolve(IntegracaoContabilApi);
    api
      .detalharLote(currentToken, selectedLote.id)
      .then((data) => {
        if (!cancelled) {
          setDetalheLote(data);
          setDetalhePage(1);
        }
      })
      .catch((err) => {
        if (!cancelled) {
          setDetalheLote(null);
          setErrorDetalhe(err instanceof Error ? err.message : "Erro ao carregar detalhes do lote.");
        }
      })
      .finally(() => {
        if (!cancelled) setLoadingDetalhe(false);
      });
    return () => {
      cancelled = true;
    };
  }, [isDetailModalOpen, selectedLote?.id, token]);

  const vigenciaLabel =
    vigenciaMes != null && vigenciaAno != null
      ? `${String(vigenciaMes).padStart(2, "0")}/${vigenciaAno}`
      : "";

  const handleGerarRemessa = async () => {
    if (!vigenciaMes || !vigenciaAno) {
      toast({
        title: "Vigência obrigatória",
        description: "Selecione o mês e ano (vigência) para gerar a remessa.",
        variant: "destructive",
      });
      return;
    }
    if (!cnpjSelecionado?.trim()) {
      toast({
        title: "CNPJ obrigatório",
        description: "Selecione uma unidade (CNPJ) para gerar a remessa.",
        variant: "destructive",
      });
      return;
    }
    const currentToken = store.getState().auth.token;
    if (!currentToken) {
      toast({
        title: "Sessão inválida",
        description: "Faça login novamente para gerar a remessa.",
        variant: "destructive",
      });
      return;
    }
    setGerarRemessaLoading(true);
    try {
      const api = container.resolve(IntegracaoContabilApi);
      const competencia = `${String(vigenciaMes).padStart(2, "0")}/${vigenciaAno}`;
      const { blob, fileName } = await api.gerarRemessaContabilMensal(currentToken, cnpjSelecionado.trim(), competencia);
      downloadBlob(blob, fileName);
      toast({
        title: "Remessa gerada",
        description: "O arquivo foi gerado e o download foi iniciado.",
      });
    } catch (err) {
      toast({
        title: "Erro ao gerar remessa",
        description: err instanceof Error ? err.message : "Não foi possível gerar a remessa. Tente novamente.",
        variant: "destructive",
      });
    } finally {
      setGerarRemessaLoading(false);
    }
  };

  const handleVerDetalhes = (lote: LoteHoleriteDTO) => {
    setSelectedLote(lote);
    setIsDetailModalOpen(true);
  };

  const handleConfirmar = (lote: LoteHoleriteDTO) => {
    setLoteParaProcessar(lote);
    setTipoProcessamentoSelecionado(0);
    setIsProcessarModalOpen(true);
  };

  const handleProcessarLoteConfirmar = async () => {
    const currentToken = store.getState().auth.token;
    if (!currentToken || !loteParaProcessar) return;
    setLoadingProcessar(true);
    try {
      const api = container.resolve(IntegracaoContabilApi);
      await api.processarHolerite(currentToken, loteParaProcessar.id, tipoProcessamentoSelecionado);
      toast({ title: "Sucesso", description: "Lote enviado para processamento." });
      setIsProcessarModalOpen(false);
      setLoteParaProcessar(null);
      await refetchLotes();
    } catch (err) {
      toast({
        title: "Erro ao processar lote",
        description: err instanceof Error ? err.message : "Tente novamente.",
        variant: "destructive",
      });
    } finally {
      setLoadingProcessar(false);
    }
  };

  const handleExcluir = (lote: LoteHoleriteDTO) => {
    setLoteParaExcluir(lote);
  };

  const confirmarExcluir = async () => {
    const currentToken = store.getState().auth.token;
    if (!currentToken || !loteParaExcluir) return;
    setLoadingExcluir(true);
    try {
      const api = container.resolve(IntegracaoContabilApi);
      await api.deletarLote(currentToken, loteParaExcluir.id);
      toast({ title: "Sucesso", description: "Lote excluído com sucesso." });
      setLoteParaExcluir(null);
      await refetchLotes();
    } catch (err) {
      toast({
        title: "Erro ao excluir lote",
        description: err instanceof Error ? err.message : "Tente novamente.",
        variant: "destructive",
      });
    } finally {
      setLoadingExcluir(false);
    }
  };

  const handleFileSelectRetorno = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedFileRetorno(file);
    }
  };

  const handleUploadRetorno = async () => {
    const currentToken = store.getState().auth.token;
    if (!selectedFileRetorno || !currentToken) {
      if (!selectedFileRetorno) {
        toast({
          title: "Nenhum arquivo selecionado",
          description: "Por favor, selecione um arquivo antes de enviar.",
          variant: "destructive",
        });
      }
      return;
    }
    if (!selectedFileRetorno.type.toLowerCase().includes("pdf")) {
      toast({
        title: "Arquivo inválido",
        description: "O arquivo deve ser um PDF.",
        variant: "destructive",
      });
      return;
    }
    setUploadRetornoLoading(true);
    try {
      const api = container.resolve(IntegracaoContabilApi);
      await api.sumarioHolerite(currentToken, selectedFileRetorno);
      toast({
        title: "Arquivo enviado",
        description: `${selectedFileRetorno.name} foi processado com sucesso. O lote foi criado e aparecerá na lista.`,
      });
      setSelectedFileRetorno(null);
      await refetchLotes();
    } catch (err) {
      toast({
        title: "Erro ao enviar arquivo",
        description: err instanceof Error ? err.message : "Tente novamente.",
        variant: "destructive",
      });
    } finally {
      setUploadRetornoLoading(false);
    }
  };

  const columns: Column[] = [
    { id: "competencia", label: "Competência", sortable: true },
    { id: "empresa", label: "Empresa", sortable: true },
    { id: "cnpj", label: "CNPJ", sortable: true },
    { id: "arquivo", label: "Arquivo", sortable: false },
    { id: "paginas", label: "Páginas", sortable: true, width: "w-[100px]" },
    { id: "status", label: "Status", sortable: true },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[280px]" },
  ];

  const columnsDetalhe: Column[] = [
    { id: "colaborador", label: "Colaborador", sortable: true },
    { id: "cargo", label: "Cargo", sortable: true },
    { id: "cpf", label: "CPF", sortable: true },
    { id: "matricula", label: "Matrícula", sortable: true },
    { id: "arquivo", label: "Arquivo", sortable: false },
    { id: "erro", label: "Erro", sortable: false },
  ];

  const renderCell = (lote: LoteHoleriteDTO, columnId: string) => {
    const sumario = lote.sumarioHolerite;
    switch (columnId) {
      case "competencia":
        return <span className="font-medium">{sumario?.competencia ?? "—"}</span>;
      case "empresa":
        return <span className="truncate block max-w-[200px]" title={sumario?.empresa}>{sumario?.empresa ?? "—"}</span>;
      case "cnpj":
        return sumario?.cnpj ?? "—";
      case "arquivo":
        return lote.pdf ? (
          <Button
            variant="ghost"
            size="sm"
            className="h-8 gap-1.5"
            onClick={() => {
              const url = processarUrlComToken(lote.pdf, token);
              if (url) window.open(url, "_blank");
            }}
          >
            <FileText className="h-4 w-4" />
            Abrir PDF
          </Button>
        ) : (
          <span className="text-muted-foreground">—</span>
        );
      case "paginas":
        return <span className="text-center block">{lote.quantidadeDePaginas}</span>;
      case "status":
        return (
          <div className="flex items-center gap-2">
            <StatusBadge status={lote.status} variant="projeto" />
            {lote.aprovadoParaProcessamento && lote.dataFinalizacao == null && (
              <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" aria-label="Processando" />
            )}
          </div>
        );
      case "acoes":
        return (
          <div className="flex justify-end gap-2 flex-wrap">
            {(lote.status === "Finalizado" || lote.status === "Em processamento") && (
              <Button variant="ghost" size="sm" onClick={() => handleVerDetalhes(lote)}>
                <Eye className="h-4 w-4 mr-1" />
                Detalhes
              </Button>
            )}
            {!lote.aprovadoParaProcessamento && (
              <>
                <Button variant="ghost" size="sm" onClick={() => handleConfirmar(lote)}>
                  <CheckCircle className="h-4 w-4 mr-1" />
                  Confirmar
                </Button>
                <Button variant="ghost" size="sm" onClick={() => handleExcluir(lote)}>
                  <Trash2 className="h-4 w-4 mr-1" />
                  Excluir
                </Button>
              </>
            )}
          </div>
        );
      default:
        return null;
    }
  };

  const renderCellDetalhe = (item: ItemLoteDTO, columnId: string) => {
    switch (columnId) {
      case "colaborador":
        return <span className="font-medium">{item.nome || "—"}</span>;
      case "cargo":
        return item.cargo || "—";
      case "cpf":
        return <span className="font-mono text-sm">{item.cpf || "—"}</span>;
      case "matricula":
        return <span className="font-mono text-sm">{item.matricula || "—"}</span>;
      case "arquivo":
        return item.filePath ? (
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8"
            onClick={() => {
              const url = processarUrlComToken(item.filePath, token);
              if (url) window.open(url, "_blank");
            }}
            aria-label="Abrir arquivo"
          >
            <Download className="h-4 w-4" />
          </Button>
        ) : (
          "—"
        );
      case "erro":
        if (item.sucesso) {
          return <span className="text-sm text-green-600 font-medium">Sucesso</span>;
        }
        if (item.erro) {
          return (
            <Tooltip>
              <TooltipTrigger asChild>
                <Badge variant="destructive">Erro</Badge>
              </TooltipTrigger>
              <TooltipContent
                side="top"
                className="max-w-md whitespace-normal break-words text-left"
              >
                {item.erro}
              </TooltipContent>
            </Tooltip>
          );
        }
        return (
          <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" aria-label="Processando" />
        );
      default:
        return null;
    }
  };

  return (
    <div className="min-h-screen bg-primaryBackground">
      <div className="container py-8">
        <h1 className="page-title mb-8">Integração Contábil</h1>
        
        <Tabs defaultValue="remessa" className="w-full">
          <div className="mb-6">
            <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm">
              <TabsTrigger 
                value="remessa" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <Upload className="h-4 w-4" />
                <span>Remessa</span>
              </TabsTrigger>
              <TabsTrigger 
                value="retorno" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <Download className="h-4 w-4" />
                <span>Retorno</span>
              </TabsTrigger>
            </TabsList>
          </div>
          
          {/* Tab Remessa */}
          <TabsContent value="remessa">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Upload className="h-5 w-5" />
                  Gerar Remessa
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-muted-foreground mb-4">
                  Gere arquivos de remessa para envio ao sistema contábil
                </p>
                <div className="flex flex-wrap items-end gap-4">
                  <div className="space-y-2 min-w-[140px]">
                    <Label htmlFor="remessa-vigencia">Vigência</Label>
                    <Popover>
                      <PopoverTrigger asChild>
                        <Button
                          id="remessa-vigencia"
                          variant="outline"
                          className={cn(
                            "w-full justify-start text-left font-normal min-w-[140px]",
                            !vigenciaLabel && "text-muted-foreground"
                          )}
                        >
                          <Calendar className="mr-2 h-4 w-4 shrink-0" />
                          {vigenciaLabel || "mm/yyyy"}
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent className="w-auto p-0" align="start">
                        <MonthPicker
                          selectedMonth={vigenciaMes}
                          selectedYear={vigenciaAno}
                          onMonthChange={(month, year) => {
                            setVigenciaMes(month);
                            setVigenciaAno(year);
                          }}
                        />
                      </PopoverContent>
                    </Popover>
                  </div>
                  <div className="space-y-2 min-w-[200px]">
                    <Label htmlFor="remessa-cnpj">CNPJ</Label>
                    <Select
                      value={cnpjSelecionado}
                      onValueChange={setCnpjSelecionado}
                      disabled={loadingUnidades}
                    >
                      <SelectTrigger id="remessa-cnpj">
                        <SelectValue placeholder="Selecione" />
                      </SelectTrigger>
                      <SelectContent>
                        {unidades.map((u) => (
                          <SelectItem key={u.id} value={u.id}>
                            {u.descricao}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    {errorUnidades && (
                      <p className="text-sm text-destructive">{errorUnidades}</p>
                    )}
                  </div>
                  <Button
                    onClick={handleGerarRemessa}
                    disabled={!vigenciaLabel || !cnpjSelecionado || gerarRemessaLoading}
                  >
                    <Download className="h-4 w-4 mr-2" />
                    {gerarRemessaLoading ? "Gerando..." : "Gerar Remessa"}
                  </Button>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
          
          {/* Tab Retorno */}
          <TabsContent value="retorno">
            <div className="space-y-6">
              {/* Upload Section */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Upload className="h-5 w-5" />
                    Upload de Retorno
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-muted-foreground mb-4">
                    Carregue os holerites em PDF retornados pelo sistema contábil
                  </p>
                  <div className="space-y-4">
                    <div className="flex items-center gap-4">
                      <div className="flex-1">
                        <Input
                          type="file"
                          accept=".pdf,.zip"
                          onChange={handleFileSelectRetorno}
                          className="cursor-pointer"
                        />
                      </div>
                      <Button onClick={() => void handleUploadRetorno()} disabled={!selectedFileRetorno || uploadRetornoLoading}>
                        <Upload className="h-4 w-4 mr-2" />
                        {uploadRetornoLoading ? "Enviando..." : "Enviar"}
                      </Button>
                    </div>
                    {selectedFileRetorno && (
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <FileText className="h-4 w-4" />
                        <span>{selectedFileRetorno.name}</span>
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>

              {/* Table Section */}
              <Card>
                <div className="p-6">
                  <h3 className="text-lg font-semibold mb-4">Retornos Processados</h3>
                  {errorLotes && (
                    <p className="text-sm text-destructive mb-4">{errorLotes}</p>
                  )}
                  {loadingLotes ? (
                    <div className="text-center py-8 text-muted-foreground">Carregando retornos...</div>
                  ) : (
                    <div className="rounded-lg border">
                      <DataTable
                        columns={columns}
                        data={lotes.slice(
                          (pageRetorno - 1) * pageSizeRetorno,
                          pageRetorno * pageSizeRetorno
                        )}
                        keyExtractor={(item) => item.id}
                        renderCell={renderCell}
                        emptyMessage="Nenhum retorno processado"
                      />
                      {lotes.length > 0 && (
                        <TablePagination
                          currentPage={pageRetorno}
                          totalItems={lotes.length}
                          itemsPerPage={pageSizeRetorno}
                          onPageChange={setPageRetorno}
                          onItemsPerPageChange={(v) => {
                            setPageSizeRetorno(Number(v));
                            setPageRetorno(1);
                          }}
                        />
                      )}
                    </div>
                  )}
                </div>
              </Card>
            </div>
          </TabsContent>
        </Tabs>
      </div>

      {/* Modal de Detalhes */}
      <Dialog open={isDetailModalOpen} onOpenChange={setIsDetailModalOpen}>
        <DialogContent className="max-w-6xl max-h-[85vh] bg-white p-0 gap-0 overflow-hidden flex flex-col">
          {/* Header */}
          <div className="relative bg-white p-6 pb-4 border-b flex-shrink-0">
            <DialogHeader>
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-primary/10 rounded-lg">
                    <FileText className="h-6 w-6 text-primary" />
                  </div>
                  <DialogTitle className="text-2xl font-bold">Detalhes do Retorno</DialogTitle>
                </div>
              </div>
            </DialogHeader>
          </div>

          {/* Content - área rolável quando tabela for maior que o modal */}
          <div className="flex-1 min-h-0 flex flex-col overflow-y-auto overflow-x-hidden p-6">
            {selectedLote && (
              <div className="space-y-6 min-w-0">
                {/* Batch Info Cards */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <Card className="p-4 border-l-4 border-l-blue-500">
                    <div className="flex items-start gap-3">
                      <div className="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
                        <Building2 className="h-5 w-5 text-blue-600 dark:text-blue-300" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm text-muted-foreground mb-1">Empresa</p>
                        <p className="font-semibold text-sm truncate">{selectedLote.sumarioHolerite?.empresa ?? "—"}</p>
                      </div>
                    </div>
                  </Card>

                  <Card className="p-4 border-l-4 border-l-purple-500">
                    <div className="flex items-start gap-3">
                      <div className="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg">
                        <Hash className="h-5 w-5 text-purple-600 dark:text-purple-300" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm text-muted-foreground mb-1">CNPJ/CPF</p>
                        <p className="font-semibold text-sm">{selectedLote.sumarioHolerite?.cnpj ?? "—"}</p>
                      </div>
                    </div>
                  </Card>

                  <Card className="p-4 border-l-4 border-l-green-500">
                    <div className="flex items-start gap-3">
                      <div className="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
                        <Calendar className="h-5 w-5 text-green-600 dark:text-green-300" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm text-muted-foreground mb-1">Competência</p>
                        <p className="font-semibold text-sm">{selectedLote.sumarioHolerite?.competencia ?? "—"}</p>
                      </div>
                    </div>
                  </Card>

                  <Card className="p-4 border-l-4 border-l-orange-500">
                    <div className="flex items-start gap-3">
                      <div className="p-2 bg-orange-100 dark:bg-orange-900 rounded-lg">
                        <CheckCircle className="h-5 w-5 text-orange-600 dark:text-orange-300" />
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm text-muted-foreground mb-1">Status</p>
                        <StatusBadge status={selectedLote.status} variant="projeto" />
                      </div>
                    </div>
                  </Card>
                </div>

                {selectedLote.pdf && (
                  <div>
                    <Button
                      variant="outline"
                      onClick={() => {
                        const url = processarUrlComToken(selectedLote!.pdf, token);
                        if (url) window.open(url, "_blank");
                      }}
                    >
                      <FileText className="h-4 w-4 mr-2" />
                      Abrir PDF
                    </Button>
                  </div>
                )}

                <Separator />

                <div className="flex flex-col min-h-0">
                  <h3 className="text-lg font-semibold mb-4">Registros Processados</h3>
                  {loadingDetalhe && (
                    <p className="text-sm text-muted-foreground py-4">Carregando itens...</p>
                  )}
                  {errorDetalhe && !loadingDetalhe && (
                    <p className="text-sm text-destructive py-4">{errorDetalhe}</p>
                  )}
                  {detalheLote?.itens != null && detalheLote.itens.length > 0 && !loadingDetalhe && (
                    <TooltipProvider>
                      <div className="rounded-lg border">
                        <DataTable<ItemLoteDTO>
                          columns={columnsDetalhe}
                          data={detalheLote.itens.slice(
                            (detalhePage - 1) * detalhePageSize,
                            detalhePage * detalhePageSize
                          )}
                          keyExtractor={(item) => item.id}
                          renderCell={renderCellDetalhe}
                          emptyMessage="Nenhum registro nesta página"
                        />
                        <TablePagination
                          currentPage={detalhePage}
                          totalItems={detalheLote.itens.length}
                          itemsPerPage={detalhePageSize}
                          onPageChange={setDetalhePage}
                          onItemsPerPageChange={(v) => {
                            setDetalhePageSize(Number(v));
                            setDetalhePage(1);
                          }}
                        />
                      </div>
                    </TooltipProvider>
                  )}
                  {detalheLote?.itens != null && detalheLote.itens.length === 0 && !loadingDetalhe && (
                    <p className="text-sm text-muted-foreground py-4">Nenhum registro processado.</p>
                  )}
                </div>
              </div>
            )}
          </div>
        </DialogContent>
      </Dialog>

      {/* Confirmação Excluir Lote */}
      <AlertDialog open={!!loteParaExcluir} onOpenChange={(open) => !open && setLoteParaExcluir(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir lote</AlertDialogTitle>
            <AlertDialogDescription>
              Deseja realmente excluir este lote? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={loadingExcluir}>Cancelar</AlertDialogCancel>
            <Button
              variant="destructive"
              disabled={loadingExcluir}
              onClick={() => void confirmarExcluir()}
            >
              {loadingExcluir ? "Excluindo..." : "Excluir"}
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal Processar Lote */}
      <Dialog
        open={isProcessarModalOpen}
        onOpenChange={(open) => {
          if (!open) {
            setIsProcessarModalOpen(false);
            setLoteParaProcessar(null);
          }
        }}
      >
        <DialogContent className="max-w-md">
          <DialogHeader>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-primary/10 rounded-lg">
                <CheckCircle className="h-6 w-6 text-primary" />
              </div>
              <DialogTitle className="text-xl">Processar Lote</DialogTitle>
            </div>
          </DialogHeader>
          <div className="space-y-4 pt-2">
            <Label className="text-sm font-medium">Selecione o tipo de processamento</Label>
            <RadioGroup
              value={String(tipoProcessamentoSelecionado)}
              onValueChange={(v) => setTipoProcessamentoSelecionado(Number(v))}
              className="grid gap-3"
            >
              {TIPOS_PROCESSAMENTO_HOLERITE.map((op) => (
                <div key={op.value} className="flex items-center space-x-2">
                  <RadioGroupItem value={String(op.value)} id={`tipo-${op.value}`} />
                  <Label htmlFor={`tipo-${op.value}`} className="font-normal cursor-pointer">
                    {op.label}
                  </Label>
                </div>
              ))}
            </RadioGroup>
          </div>
          <div className="flex justify-end gap-2 pt-4 border-t">
            <Button
              variant="outline"
              onClick={() => {
                setIsProcessarModalOpen(false);
                setLoteParaProcessar(null);
              }}
              disabled={loadingProcessar}
            >
              Cancelar
            </Button>
            <Button onClick={handleProcessarLoteConfirmar} disabled={loadingProcessar}>
              {loadingProcessar ? "Processando..." : "Confirmar"}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default IntegracaoContabil;
