import { useState, useMemo, useRef, useCallback } from "react";
import { useNotaFiscalStatus, useNotasFiscaisPorVigenciaColaborador, useRubricasColaboradorParaLiberacaoDeNf, useInserirNotaFiscal } from "@/hooks/useNotasFiscais";
import type { ListarNotasFiscaisPorVigenciaVisaoGestorParams, RubricaColaborador } from "@domain/entities/NotaFiscalGestao";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { DataTable, DadosBancariosCard } from "@presentation/components/common";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
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
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { CalendarIcon, Eye, ChevronRight, Loader2, AlertTriangle, Upload, X, FileText } from "@/components/ui/system-icons";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { cn } from "@/lib/utils";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { toast } from "@/hooks/use-toast";
import { useParametros } from "@/hooks/useParametros";
import { getArquivoTipo } from "@/shared/utils/fileUtils";

/** Trunca nome de arquivo para exibição; tooltip exibe o nome completo. */
const MAX_NOME_ARQUIVO_EXIBICAO = 38;
function truncarNomeArquivo(nome: string): string {
  if (nome.length <= MAX_NOME_ARQUIVO_EXIBICAO) return nome;
  return `${nome.slice(0, MAX_NOME_ARQUIVO_EXIBICAO)}…`;
}

// Função auxiliar para formatar competência
const formatarCompetencia = (mes: number, ano: number): string => {
  const meses = [
    "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
    "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
  ];
  return `${meses[mes - 1]}/${ano}`;
};

export default function MeuFaturamento() {
  const hoje = new Date();
  const [statusId, setStatusId] = useState<string>("");
  const [mes, setMes] = useState<number>(hoje.getMonth() + 1);
  const [ano, setAno] = useState<number>(hoje.getFullYear());
  const { status: statusOptions, loading: loadingStatus } = useNotaFiscalStatus();
  const { isEnabled } = useParametros();
  const mostrarDadosBancarios = isEnabled("PRESTADORES_COLETA_DADOS_BANCARIOS");
  const [temDadosBancarios, setTemDadosBancarios] = useState<boolean>(false);

  // Callback para atualizar estado de dados bancários
  const handleDadosBancariosChange = useCallback((temDados: boolean) => {
    setTemDadosBancarios(temDados);
  }, []);
  
  // Memoizar params para evitar re-renders infinitos
  const params = useMemo<ListarNotasFiscaisPorVigenciaVisaoGestorParams | undefined>(() => {
    if (!mes || !ano) return undefined;
    return {
      mes,
      ano,
      statusId: statusId || undefined,
      cursor: 0,
      limite: 100,
    };
  }, [mes, ano, statusId]);

  // Buscar notas fiscais apenas se mes e ano estiverem selecionados
  const { notasFiscais, loading: loadingNotas, error } = useNotasFiscaisPorVigenciaColaborador(params);

  // Filtrar por status se selecionado
  const notasFiltradas = useMemo(() => {
    if (!statusId) return notasFiscais;
    const statusIdNum = parseInt(statusId);
    return notasFiscais.filter(nf => nf.notaFiscalStatusId === statusIdNum);
  }, [notasFiscais, statusId]);

  // Buscar rubricas para liberação de NF
  const paramsRubricas = useMemo(() => {
    if (!mes || !ano) return undefined;
    return { mes, ano };
  }, [mes, ano]);

  const { rubricas, loading: loadingRubricas, error: errorRubricas } = useRubricasColaboradorParaLiberacaoDeNf(paramsRubricas);

  // Estado para rubricas selecionadas
  const [rubricasSelecionadas, setRubricasSelecionadas] = useState<Set<string>>(new Set());

  // Estado para modal de upload
  const [modalUploadOpen, setModalUploadOpen] = useState(false);
  const [arquivoNotaFiscal, setArquivoNotaFiscal] = useState<File | null>(null);
  const [numeroNf, setNumeroNf] = useState<string>("");
  const [isDragging, setIsDragging] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { inserirNotaFiscal, loading: loadingUpload } = useInserirNotaFiscal();

  // Calcular soma total das rubricas selecionadas
  const somaTotal = useMemo(() => {
    let total = 0;
    rubricasSelecionadas.forEach((rubricaId) => {
      const rubrica = rubricas.find((r) => r.id === rubricaId);
      if (rubrica) {
        if (rubrica.natureza === "Débito") {
          total -= rubrica.valor;
        } else {
          total += rubrica.valor;
        }
      }
    });
    return total;
  }, [rubricasSelecionadas, rubricas]);

  // Função para selecionar/deselecionar rubrica
  const handleToggleRubrica = (rubricaId: string, emUso: boolean) => {
    if (emUso) return; // Não permitir selecionar se emUso = true
    setRubricasSelecionadas((prev) => {
      const novo = new Set(prev);
      if (novo.has(rubricaId)) {
        novo.delete(rubricaId);
      } else {
        novo.add(rubricaId);
      }
      return novo;
    });
  };

  // Função para selecionar todas as rubricas disponíveis
  const handleSelectAllRubricas = () => {
    const rubricasDisponiveis = rubricas.filter((r) => !r.emUso);
    if (rubricasSelecionadas.size === rubricasDisponiveis.length) {
      setRubricasSelecionadas(new Set());
    } else {
      setRubricasSelecionadas(new Set(rubricasDisponiveis.map((r) => r.id)));
    }
  };

  const handleLimpar = () => {
    setStatusId("");
    const hoje = new Date();
    setMes(hoje.getMonth() + 1);
    setAno(hoje.getFullYear());
  };

  const getStatusColor = (statusDescricao: string | null | undefined) => {
    if (!statusDescricao) return "bg-muted text-muted-foreground";
    
    const status = statusDescricao.toLowerCase();
    if (status.includes("aprovada")) {
      return "bg-success/10 text-success border-success/20";
    }
    if (status.includes("emitida") || status.includes("paga")) {
      return "bg-info/10 text-info border-info/20";
    }
    if (status.includes("pendente") || status.includes("aguarda") || status.includes("preparação") || status.includes("análise")) {
      return "bg-warning/10 text-warning border-warning/20";
    }
    if (status.includes("cancelada") || status.includes("reprovada")) {
      return "bg-destructive/10 text-destructive border-destructive/20";
    }
    return "bg-muted text-muted-foreground";
  };

  // Função para converter arquivo para base64
  const fileToBase64 = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        const result = reader.result as string;
        // Remover o prefixo data:image/...;base64, ou data:application/...;base64,
        const base64 = result.split(',')[1];
        resolve(base64);
      };
      reader.onerror = (error) => reject(error);
    });
  };

  // Função para validar arquivo
  const validarArquivo = (file: File): { valid: boolean; error?: string } => {
    const tiposPermitidos = ['application/pdf', 'image/png', 'image/jpeg', 'image/jpg'];
    const extensoesPermitidas = ['pdf', 'png', 'jpg', 'jpeg'];
    
    const extensao = file.name.split('.').pop()?.toLowerCase();
    
    if (!tiposPermitidos.includes(file.type) && (!extensao || !extensoesPermitidas.includes(extensao))) {
      return { valid: false, error: 'Apenas arquivos PDF, PNG e JPG são permitidos' };
    }

    // Validar tamanho (20MB máximo)
    if (file.size > 20 * 1024 * 1024) {
      return { valid: false, error: 'Arquivo muito grande. Tamanho máximo: 20MB' };
    }

    return { valid: true };
  };

  // Função para lidar com seleção de arquivo
  const handleFileSelect = (file: File) => {
    const validacao = validarArquivo(file);
    if (!validacao.valid) {
      toast({
        title: "Erro",
        description: validacao.error,
        variant: "destructive",
      });
      return;
    }
    setArquivoNotaFiscal(file);
  };

  // Função para drag and drop
  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(true);
  }, []);

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  }, []);

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);

    const file = e.dataTransfer.files?.[0];
    if (file) {
      handleFileSelect(file);
    }
  }, []);

  // Função para abrir modal de upload
  const handleAbrirModalUpload = () => {
    if (rubricasSelecionadas.size === 0 || somaTotal < 0) return;
    
    // Validar se precisa ter dados bancários e se tem
    if (mostrarDadosBancarios && !temDadosBancarios) {
      toast({
        title: "Dados bancários obrigatórios",
        description: "É necessário cadastrar dados bancários antes de gerar a nota fiscal.",
        variant: "destructive",
      });
      return;
    }
    
    setModalUploadOpen(true);
    setArquivoNotaFiscal(null);
    setNumeroNf("");
  };

  // Função para fechar modal
  const handleFecharModal = () => {
    if (loadingUpload) return;
    setModalUploadOpen(false);
    setArquivoNotaFiscal(null);
    setNumeroNf("");
  };

  // Função para enviar nota fiscal
  const handleEnviarNotaFiscal = async () => {
    if (!arquivoNotaFiscal || !numeroNf.trim() || !mes || !ano) {
      toast({
        title: "Erro",
        description: "Preencha todos os campos obrigatórios",
        variant: "destructive",
      });
      return;
    }

    try {
      // Converter arquivo para base64
      const base64 = await fileToBase64(arquivoNotaFiscal);
      // Determinar tipo do arquivo pela extensão (ArquivoTipoEnum: 1=pdf, 2=png, 3=jpg, 4=jpeg, ...)
      const tipo = getArquivoTipo(arquivoNotaFiscal.name);

      // Enviar para API
      await inserirNotaFiscal({
        base64Objeto: {
          base64,
          tipo,
        },
        numeroNf: numeroNf.trim(),
        vigenciaMes: mes,
        vigenciaAno: ano,
        listaDeIdsRubricasLiberacao: Array.from(rubricasSelecionadas),
      });

      toast({
        title: "Sucesso",
        description: "Nota fiscal enviada com sucesso!",
      });

      // Fechar modal e limpar seleção
      handleFecharModal();
      setRubricasSelecionadas(new Set());
      
      // Recarregar dados
      // TODO: Adicionar refetch se necessário
    } catch (error) {
      toast({
        title: "Erro",
        description: error instanceof Error ? error.message : "Erro ao enviar nota fiscal",
        variant: "destructive",
      });
    }
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      <h1 className="page-title">Meu Faturamento</h1>

      {/* Dados Bancários */}
      {mostrarDadosBancarios && (
        <DadosBancariosCard onDadosBancariosChange={handleDadosBancariosChange} />
      )}

      {/* Filtros */}
      <Card>
        <CardContent className="pt-6">
          <h2 className="text-lg font-semibold mb-4">Filtros</h2>
          <div className="flex flex-col md:flex-row gap-4 items-end">
            {/* Status */}
            <div className="space-y-2 flex-1">
              <Label>Status</Label>
              <Select value={statusId || "todos"} onValueChange={(value) => setStatusId(value === "todos" ? "" : value)} disabled={loadingStatus}>
                <SelectTrigger>
                  <SelectValue placeholder={loadingStatus ? "Carregando..." : "Todos"} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todos">Todos</SelectItem>
                  {statusOptions.length > 0 &&
                    statusOptions.map((statusOption) => (
                      <SelectItem key={statusOption.id} value={statusOption.id.toString()}>
                        {statusOption.nome}
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
            </div>

            {/* Vigência */}
            <div className="space-y-2 flex-1">
              <Label>Vigência</Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !mes && !ano && "text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4 shrink-0" />
                    <span className="truncate">
                      {mes && ano ? formatarCompetencia(mes, ano) : "Selecione"}
                    </span>
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <MonthPicker
                    selectedMonth={mes}
                    selectedYear={ano}
                    onMonthChange={(month, year) => {
                      setMes(month);
                      setAno(year);
                    }}
                  />
                </PopoverContent>
              </Popover>
            </div>

            {/* Botões */}
            <div className="flex gap-3">
              <Button variant="ghost" onClick={handleLimpar} className="text-destructive hover:text-destructive">
                Limpar
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Rubricas à faturar */}
      {mes && ano && (
        <div className="space-y-4">
          <div>
            <h2 className="text-xl font-semibold">Rubricas à faturar</h2>
            <p className="text-sm text-muted-foreground">
              Selecione as rubricas pendentes para emissão das notas fiscais
            </p>
          </div>

          {loadingRubricas ? (
            <Card>
              <CardContent className="p-8 text-center">
                <Loader2 className="h-6 w-6 animate-spin mx-auto mb-2" />
                <p className="text-muted-foreground">Carregando rubricas...</p>
              </CardContent>
            </Card>
          ) : errorRubricas ? (
            <Card>
              <CardContent className="p-8 text-center">
                <p className="text-destructive">{errorRubricas}</p>
              </CardContent>
            </Card>
          ) : rubricas.length > 0 ? (
            <Card>
              <CardContent className="p-6">
                {rubricas.filter((r) => !r.emUso).length > 0 && (
                  <div className="mb-4 flex items-center gap-2">
                    <Checkbox
                      checked={
                        rubricas.filter((r) => !r.emUso).length > 0 &&
                        rubricas.filter((r) => !r.emUso).every((r) => rubricasSelecionadas.has(r.id))
                      }
                      onCheckedChange={handleSelectAllRubricas}
                    />
                    <Label className="text-sm font-medium">
                      Selecionar todas ({rubricas.filter((r) => !r.emUso).length} disponíveis)
                    </Label>
                  </div>
                )}

                <DataTable
                  columns={[
                    { id: "selecione", label: "Selecione", sortable: false, width: "w-[100px]" },
                    { id: "descricao", label: "Descrição", sortable: true },
                    { id: "valor", label: "Valor", sortable: true },
                    { id: "statusNF", label: "Status NF", sortable: false },
                    { id: "natureza", label: "Natureza", sortable: true },
                    { id: "emUso", label: "Em uso", sortable: true },
                  ]}
                  data={rubricas}
                  keyExtractor={(item) => item.id}
                  renderCell={(rubrica: RubricaColaborador, columnId: string) => {
                    switch (columnId) {
                      case "selecione":
                        if (rubrica.emUso) {
                          return <span className="text-muted-foreground text-sm">-</span>;
                        }
                        return (
                          <Checkbox
                            checked={rubricasSelecionadas.has(rubrica.id)}
                            onCheckedChange={() => handleToggleRubrica(rubrica.id, rubrica.emUso)}
                          />
                        );
                      case "descricao":
                        return <span className="font-medium">{rubrica.descricao}</span>;
                      case "valor":
                        return (
                          <span className="font-semibold">
                            {rubrica.valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                          </span>
                        );
                      case "statusNF":
                        if (!rubrica.notasFiscais || rubrica.notasFiscais.length === 0) {
                          return <span className="text-muted-foreground">-</span>;
                        }
                        const primeiraNF = rubrica.notasFiscais[0];
                        const totalNFs = rubrica.notasFiscais.length;
                        return (
                          <div className="flex items-center gap-2">
                            <Badge className={cn("font-medium", getStatusColor(primeiraNF.statusDescricao || ""))}>
                              {primeiraNF.statusDescricao || "Sem status"}
                            </Badge>
                            {totalNFs > 1 && (
                              <Badge variant="outline" className="text-xs">
                                +{totalNFs - 1}
                              </Badge>
                            )}
                          </div>
                        );
                      case "natureza":
                        return (
                          <Badge
                            variant="outline"
                            className={rubrica.natureza === "Débito" ? "text-destructive" : "text-success"}
                          >
                            {rubrica.natureza}
                          </Badge>
                        );
                      case "emUso":
                        return (
                          <Badge variant={rubrica.emUso ? "default" : "outline"}>
                            {rubrica.emUso ? "Sim" : "Não"}
                          </Badge>
                        );
                      default:
                        return null;
                    }
                  }}
                  emptyMessage="Nenhuma rubrica encontrada"
                />

                {rubricasSelecionadas.size > 0 && (
                  <div className="mt-6 pt-4 border-t space-y-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-muted-foreground mb-1">Total selecionado</p>
                        <p
                          className={cn(
                            "text-2xl font-bold",
                            somaTotal < 0 ? "text-destructive" : "text-success"
                          )}
                        >
                          {somaTotal.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                        </p>
                        {somaTotal < 0 && (
                          <p className="text-xs text-destructive mt-1">
                            O valor total não pode ser negativo para gerar nota fiscal
                          </p>
                        )}
                      </div>
                      <Button
                        disabled={
                          somaTotal < 0 || 
                          rubricasSelecionadas.size === 0 || 
                          (mostrarDadosBancarios && !temDadosBancarios)
                        }
                        className="gap-2"
                        onClick={handleAbrirModalUpload}
                      >
                        <Upload className="h-4 w-4" />
                        Upload da Nota
                      </Button>
                      {mostrarDadosBancarios && !temDadosBancarios && (
                        <p className="text-xs text-destructive mt-1">
                          Cadastre dados bancários para gerar nota fiscal
                        </p>
                      )}
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          ) : (
            <Card>
              <CardContent className="p-8 text-center">
                <p className="text-muted-foreground">Nenhuma rubrica encontrada para a vigência selecionada</p>
              </CardContent>
            </Card>
          )}
        </div>
      )}

      {/* Histórico de NF's */}
      <div className="space-y-4">
        <div>
          <h2 className="text-xl font-semibold">Histórico de NF's</h2>
          <p className="text-sm text-muted-foreground">
            Acompanhe o histórico de notas à emitir/emitidas
          </p>
        </div>

        {loadingNotas ? (
          <Card>
            <CardContent className="p-8 text-center">
              <Loader2 className="h-6 w-6 animate-spin mx-auto mb-2" />
              <p className="text-muted-foreground">Carregando notas fiscais...</p>
            </CardContent>
          </Card>
        ) : error ? (
          <Card>
            <CardContent className="p-8 text-center">
              <p className="text-destructive">{error}</p>
            </CardContent>
          </Card>
        ) : !mes || !ano ? (
          <Card>
            <CardContent className="p-8 text-center">
              <p className="text-muted-foreground">Selecione uma vigência para visualizar as notas fiscais</p>
            </CardContent>
          </Card>
        ) : notasFiltradas.length > 0 ? (
          <div className="space-y-4">
            {notasFiltradas.map((nf) => (
              <Card key={nf.id} className="hover:shadow-md transition-shadow">
                <CardContent className="p-6">
                  <div className="flex items-start justify-between mb-4">
                    <div>
                      <p className="text-sm text-muted-foreground mb-1">Vigência</p>
                      <h3 className="text-xl font-bold">
                        {nf.vigenciaMes && nf.vigenciaAno 
                          ? formatarCompetencia(nf.vigenciaMes, nf.vigenciaAno)
                          : formatarCompetencia(mes, ano)}
                      </h3>
                    </div>
                    <Badge className={cn("font-medium", getStatusColor(nf.notaFiscalStatusDescricao))}>
                      {nf.notaFiscalStatusDescricao || "Sem status"}
                    </Badge>
                  </div>

                  {nf.motivoReprovacao && (
                    <Alert variant="destructive" className="mb-4">
                      <AlertTriangle className="h-4 w-4" />
                      <AlertDescription>
                        <strong>Motivo da reprovação:</strong> {nf.motivoReprovacao}
                      </AlertDescription>
                    </Alert>
                  )}

                  <div className="mb-4">
                    <p className="text-sm text-muted-foreground mb-1">Valor total</p>
                    <p className="text-2xl font-bold">
                      {(nf.valor || nf.sumarioValorTotalDeRubricas || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                    </p>
                  </div>

                  <div className="flex items-center justify-between pt-4 border-t">
                    <Collapsible>
                      <CollapsibleTrigger className="flex items-center gap-1 text-sm font-medium hover:text-primary transition-colors">
                        <ChevronRight className="h-4 w-4" />
                        Detalhes
                      </CollapsibleTrigger>
                      <CollapsibleContent className="mt-4 space-y-2">
                        <div className="grid grid-cols-2 gap-4 text-sm">
                          {nf.nomeColaborador && (
                            <div>
                              <p className="text-muted-foreground">Colaborador</p>
                              <p className="font-medium">{nf.nomeColaborador}</p>
                            </div>
                          )}
                          {nf.documentoColaborador && (
                            <div>
                              <p className="text-muted-foreground">CPF</p>
                              <p className="font-medium">{nf.documentoColaborador}</p>
                            </div>
                          )}
                          {nf.dataEmissaoNotaFiscal && (
                            <div>
                              <p className="text-muted-foreground">Data de Emissão</p>
                              <p className="font-medium">
                                {format(new Date(nf.dataEmissaoNotaFiscal), "dd/MM/yyyy", { locale: ptBR })}
                              </p>
                            </div>
                          )}
                          {nf.numeroNf && (
                            <div>
                              <p className="text-muted-foreground">Número NF</p>
                              <p className="font-medium">{nf.numeroNf}</p>
                            </div>
                          )}
                          {nf.rubricas && nf.rubricas.length > 0 && (
                            <div className="col-span-2">
                              <p className="text-muted-foreground mb-2">Rubricas</p>
                              <div className="space-y-1">
                                {nf.rubricas.map((rubrica, idx) => (
                                  <div key={rubrica.id || idx} className="flex justify-between text-xs">
                                    <span>{rubrica.rubricaDescricao || rubrica.codigoRubrica}</span>
                                    <span className="font-medium">
                                      {rubrica.valor?.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                                    </span>
                                  </div>
                                ))}
                              </div>
                            </div>
                          )}
                        </div>
                      </CollapsibleContent>
                    </Collapsible>

                    {nf.urlNotaFiscalDownload && (
                      <Button 
                        variant="outline" 
                        className="gap-2"
                        onClick={() => {
                          if (nf.urlNotaFiscalDownload) {
                            window.open(nf.urlNotaFiscalDownload, '_blank', 'noopener,noreferrer');
                          }
                        }}
                      >
                        <Eye className="h-4 w-4" />
                        Visualizar NF
                      </Button>
                    )}
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        ) : (
          <Card>
            <CardContent className="p-8 text-center">
              <p className="text-muted-foreground">Nenhuma nota fiscal encontrada para a vigência selecionada</p>
            </CardContent>
          </Card>
        )}
      </div>

      {/* Modal de Upload da Nota Fiscal */}
      <Dialog open={modalUploadOpen} onOpenChange={handleFecharModal}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>Upload da Nota Fiscal</DialogTitle>
            <DialogDescription>
              Anexe a sua nota fiscal emitida
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            {/* Campo Número da NF */}
            <div className="space-y-2">
              <Label htmlFor="numeroNf">
                Número da Nota Fiscal <span className="text-destructive">*</span>
              </Label>
              <Input
                id="numeroNf"
                placeholder="Ex: 12345"
                value={numeroNf}
                onChange={(e) => setNumeroNf(e.target.value)}
                disabled={loadingUpload}
              />
            </div>

            {/* Área de Upload */}
            <div className="space-y-2">
              <Label>Arquivo da Nota Fiscal <span className="text-destructive">*</span></Label>
              <input
                ref={fileInputRef}
                type="file"
                accept=".pdf,.png,.jpg,.jpeg,application/pdf,image/png,image/jpeg"
                onChange={(e) => {
                  const file = e.target.files?.[0];
                  if (file) {
                    handleFileSelect(file);
                  }
                }}
                className="hidden"
                disabled={loadingUpload}
              />

              {!arquivoNotaFiscal ? (
                <div
                  onClick={() => !loadingUpload && fileInputRef.current?.click()}
                  onDragOver={handleDragOver}
                  onDragLeave={handleDragLeave}
                  onDrop={handleDrop}
                  className={cn(
                    "border-2 border-dashed rounded-lg p-8 text-center transition-all cursor-pointer",
                    isDragging
                      ? "border-primary bg-primary/5"
                      : "border-muted-foreground/25 hover:border-primary/50 hover:bg-muted/50",
                    loadingUpload && "opacity-50 cursor-not-allowed"
                  )}
                >
                  <div className="flex flex-col items-center gap-3">
                    <Upload className="h-12 w-12 text-muted-foreground" />
                    <div>
                      <p className="text-sm font-medium text-foreground mb-1">
                        Clique para selecionar ou arraste o arquivo aqui
                      </p>
                      <p className="text-xs text-muted-foreground">
                        (pdf, jpg, png)
                      </p>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="border-2 border-border rounded-lg p-4 bg-muted/30">
                  <div className="flex items-center gap-4">
                    <div className="h-16 w-16 rounded-lg bg-muted flex items-center justify-center border border-border">
                      <FileText className="h-8 w-8 text-muted-foreground" />
                    </div>
                    <div className="flex-1 min-w-0">
                      <TooltipProvider>
                        <Tooltip>
                          <TooltipTrigger asChild>
                            <p className="text-sm font-medium text-foreground truncate block" title={arquivoNotaFiscal.name}>
                              {truncarNomeArquivo(arquivoNotaFiscal.name)}
                            </p>
                          </TooltipTrigger>
                          <TooltipContent side="top" className="max-w-[320px] break-all">
                            {arquivoNotaFiscal.name}
                          </TooltipContent>
                        </Tooltip>
                      </TooltipProvider>
                      <p className="text-xs text-muted-foreground mt-1">
                        {(arquivoNotaFiscal.size / 1024 / 1024).toFixed(2)} MB
                      </p>
                    </div>
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      onClick={() => setArquivoNotaFiscal(null)}
                      disabled={loadingUpload}
                      className="flex-shrink-0"
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  </div>
                  {!loadingUpload && (
                    <div className="mt-3 pt-3 border-t border-border">
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() => fileInputRef.current?.click()}
                        className="w-full"
                      >
                        <Upload className="h-4 w-4 mr-2" />
                        Trocar arquivo
                      </Button>
                    </div>
                  )}
                </div>
              )}
            </div>

            {/* Botões */}
            <div className="flex justify-end gap-3 pt-4">
              <Button
                variant="ghost"
                onClick={handleFecharModal}
                disabled={loadingUpload}
              >
                Cancelar
              </Button>
              <Button
                onClick={handleEnviarNotaFiscal}
                disabled={loadingUpload || !arquivoNotaFiscal || !numeroNf.trim()}
                className="gap-2"
              >
                {loadingUpload ? (
                  <>
                    <Loader2 className="h-4 w-4 animate-spin" />
                    Enviando...
                  </>
                ) : (
                  "Enviar"
                )}
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
