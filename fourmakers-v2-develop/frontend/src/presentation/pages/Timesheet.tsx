import { useState, useEffect, useCallback } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { Clock, Users, CheckSquare, Download, CalendarIcon, ArrowLeft, User, FileUp } from "lucide-react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { TimesheetTab } from "@presentation/components/timesheet/TimesheetTab";
import { ManagementTab } from "@presentation/components/timesheet/ManagementTab";
import { ApprovalsTab } from "@presentation/components/timesheet/ApprovalsTab";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { cn } from "@/lib/utils";
import { useAppSelector } from "@app/store/hooks";
import { useParametros } from "@presentation/hooks/useParametros";
import { container } from "@core/di/container";
import { TimesheetComponentesApi, type PeriodoFechado } from "@data/api/TimesheetComponentesApi";
import { toast } from "sonner";

// Mapeamento entre valores da URL e valores das abas
const tabUrlToValue: Record<string, string> = {
  'gestaoadm': 'management',
  'aprovacao': 'approvals',
};

const tabValueToUrl: Record<string, string> = {
  'management': 'gestaoadm',
  'approvals': 'aprovacao',
};

const Timesheet = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();
  const { token, user } = useAppSelector((state) => state.auth);
  const { getParametro } = useParametros();
  const labelColaborador = getParametro("LABEL_COLABORADOR_TIMESHEET") || "Colaborador(a)";
  const [modalFecharPeriodoOpen, setModalFecharPeriodoOpen] = useState(false);
  const [dataFim, setDataFim] = useState<Date | undefined>(undefined);
  const [fechandoPeriodo, setFechandoPeriodo] = useState(false);
  const [periodoFechado, setPeriodoFechado] = useState<PeriodoFechado | null>(null);
  const [modalExportarRelatorioOpen, setModalExportarRelatorioOpen] = useState(false);
  const [tipoRelatorio, setTipoRelatorio] = useState<string>("geral");
  const [exportando, setExportando] = useState(false);
  const [selectedPeriod, setSelectedPeriod] = useState<string>("");
  const [vigenciaManagement, setVigenciaManagement] = useState<{ mes: number; ano: number } | null>(null);
  const [vigenciaApprovals, setVigenciaApprovals] = useState<{ mes: number; ano: number } | null>(null);

  const handleVigenciaManagementChange = useCallback((mes: number, ano: number) => {
    setVigenciaManagement({ mes, ano });
  }, []);
  const handleVigenciaApprovalsChange = useCallback((mes: number, ano: number) => {
    setVigenciaApprovals({ mes, ano });
  }, []);

  // RELATORIO_DE_APONTAMENTO_SIMPLIFICADO = true → exibir os 3 layouts; false ou sem funcionalidade → não exibir (apenas Relatório Fechamento)
  const exibirOpcoesLayout = user?.funcionalidadeSistema?.some(
    (func) => func.descricao === "RELATORIO_DE_APONTAMENTO_SIMPLIFICADO" && func.ativo === true
  ) ?? false;
  
  // Ler parâmetros da URL
  const colaboradorId = searchParams.get('id');
  const colaboradorMes = searchParams.get('mes');
  const colaboradorAno = searchParams.get('ano');
  const colaboradorNome = searchParams.get('nome');

  const [activeTab, setActiveTab] = useState<string>(() => {
    // Se houver id na URL, sempre usar 'timesheet'
    if (colaboradorId) {
      return 'timesheet';
    }
    const tabParam = searchParams.get('tab');
    if (tabParam && tabUrlToValue[tabParam]) {
      return tabUrlToValue[tabParam];
    }
    return 'timesheet';
  });

  // Sincronizar com a URL quando ela mudar
  useEffect(() => {
    // Se houver id na URL, sempre manter 'timesheet'
    if (colaboradorId) {
      if (activeTab !== 'timesheet') {
        setActiveTab('timesheet');
      }
      return;
    }
    
    const tabParam = searchParams.get('tab');
    const tabFromUrl = tabParam && tabUrlToValue[tabParam] 
      ? tabUrlToValue[tabParam] 
      : 'timesheet';
    
    if (tabFromUrl !== activeTab) {
      setActiveTab(tabFromUrl);
    }
  }, [searchParams, activeTab, colaboradorId]);

  // Carregar período fechado
  useEffect(() => {
    const loadPeriodoFechado = async () => {
      if (!token) return;

      try {
        const api = container.resolve(TimesheetComponentesApi);
        const periodo = await api.buscarPeriodoFechado(token, 'pt-BR');
        setPeriodoFechado(periodo);
      } catch (err) {
        console.error("Erro ao carregar período fechado:", err);
        setPeriodoFechado(null);
      }
    };

    loadPeriodoFechado();
  }, [token]);

  // Ao abrir o modal Fechar Período, preencher a data com o período fechado atual (para edição)
  useEffect(() => {
    if (!modalFecharPeriodoOpen || !periodoFechado?.dataFim) return;
    const datePart = periodoFechado.dataFim.includes("T")
      ? periodoFechado.dataFim.split("T")[0]
      : periodoFechado.dataFim;
    const parsed = new Date(datePart + "T12:00:00");
    if (!Number.isNaN(parsed.getTime())) {
      setDataFim(parsed);
    }
  }, [modalFecharPeriodoOpen, periodoFechado?.dataFim]);

  // Atualizar URL quando a tab mudar
  const handleTabChange = (value: string) => {
    // Se houver id na URL, não permitir mudança de tab
    if (colaboradorId) {
      return;
    }
    
    setActiveTab(value);
    const urlTab = tabValueToUrl[value];
    if (urlTab) {
      setSearchParams({ tab: urlTab });
    } else {
      // Se não houver mapeamento, remove o parâmetro tab da URL
      const newParams = new URLSearchParams(searchParams);
      newParams.delete('tab');
      setSearchParams(newParams);
    }
  };

  const handleFecharPeriodo = async () => {
    if (!token || !dataFim) return;

    try {
      setFechandoPeriodo(true);
      const api = container.resolve(TimesheetComponentesApi);
      
      // Formatar data no formato esperado pela API (YYYY-MM-DD)
      const dataFimFormatada = format(dataFim, 'yyyy-MM-dd');
      
      await api.fecharAlterarPeriodo(token, dataFimFormatada);

      const periodo = await api.buscarPeriodoFechado(token, 'pt-BR');
      setPeriodoFechado(periodo);

      setModalFecharPeriodoOpen(false);
      setDataFim(undefined);
      toast.success("Período fechado com sucesso!");
    } catch (err) {
      console.error("Erro ao fechar período:", err);
      toast.error("Erro ao fechar período. Tente novamente.");
    } finally {
      setFechandoPeriodo(false);
    }
  };

  const isGerenciandoOutroColaborador = !!colaboradorId;

  // Função para fazer download do arquivo
  const downloadFile = (fileContents: string, fileName: string, contentType: string) => {
    // Converter base64 para blob
    const byteCharacters = atob(fileContents);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: contentType });

    // Criar link temporário para download
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  };

  // Função para exportar relatório
  const handleExportarRelatorio = async () => {
    if (!token) return;

    try {
      setExportando(true);
      const api = container.resolve(TimesheetComponentesApi);
      
      // Obter mes e ano da vigência da aba ativa
      let mes: number;
      let ano: number;
      const now = new Date();
      const fallbackMes = now.getMonth() + 1;
      const fallbackAno = now.getFullYear();

      if (colaboradorMes && colaboradorAno) {
        mes = parseInt(colaboradorMes, 10);
        ano = parseInt(colaboradorAno, 10);
      } else if (activeTab === "management" && vigenciaManagement) {
        mes = vigenciaManagement.mes;
        ano = vigenciaManagement.ano;
      } else if (activeTab === "approvals" && vigenciaApprovals) {
        mes = vigenciaApprovals.mes;
        ano = vigenciaApprovals.ano;
      } else if (selectedPeriod) {
        const [periodMes, periodAno] = selectedPeriod.split('-').map(Number);
        if (!Number.isNaN(periodMes) && !Number.isNaN(periodAno)) {
          mes = periodMes;
          ano = periodAno;
        } else {
          mes = fallbackMes;
          ano = fallbackAno;
        }
      } else {
        mes = fallbackMes;
        ano = fallbackAno;
      }

      // Sem opções de layout (false ou sem funcionalidade): usar sempre Relatório Fechamento
      const tipoEfetivo = exibirOpcoesLayout ? tipoRelatorio : "fechamento";

      let response;
      if (tipoEfetivo === "geral") {
        response = await api.relatorioApontamentos(token, mes, ano, 'pt-BR');
      } else if (tipoEfetivo === "fechamento") {
        response = await api.relatorioApontamentosSimplificado(token, mes, ano, 'pt-BR');
      } else if (tipoEfetivo === "naoApontado") {
        response = await api.relatorioColaboradoresQueNaoApontaram(token, mes, ano, 'pt-BR');
      } else {
        toast.error("Tipo de relatório inválido");
        return;
      }

      if (response.sucesso && response.retorno) {
        downloadFile(
          response.retorno.fileContents,
          response.retorno.fileDownloadName,
          response.retorno.contentType
        );
        toast.success("Relatório exportado com sucesso!");
        setModalExportarRelatorioOpen(false);
        setTipoRelatorio("geral");
      } else {
        toast.error(response.mensagem || "Erro ao exportar relatório");
      }
    } catch (err: any) {
      console.error("Erro ao exportar relatório:", err);
      toast.error(err?.message || "Erro ao exportar relatório. Tente novamente.");
    } finally {
      setExportando(false);
    }
  };

  return (
    <div className="container mx-auto p-4 sm:p-6 space-y-4 sm:space-y-6">
      <div className="flex flex-col gap-4">
        {/* Título */}
        <div>
          <h1 className="page-title">Timesheet</h1>
          {colaboradorNome && colaboradorMes && colaboradorAno && (
            <Card className="mt-4 bg-white border border-border shadow-sm">
              <CardContent className="pt-6">
                <div className="flex items-center gap-4">
                  {isGerenciandoOutroColaborador && (
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => navigate('/timesheet?tab=gestaoadm')}
                      className="h-9 px-3"
                      title="Voltar para Gestão ADM"
                    >
                      <ArrowLeft className="h-4 w-4 mr-2" />
                      Voltar
                    </Button>
                  )}
                  <div className="flex-1 space-y-2">
                    <p className="text-lg font-semibold text-foreground">
                      {decodeURIComponent(colaboradorNome)}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      Vigência: {colaboradorMes}/{colaboradorAno}
                    </p>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}
        </div>

        {/* Indicador de Gerenciamento de Outro Colaborador */}
        {isGerenciandoOutroColaborador && (
          <div className="flex items-center gap-2 px-4 py-3 bg-blue-50 border border-blue-200 rounded-lg">
            <User className="h-5 w-5 text-blue-600" />
            <p className="text-sm font-medium text-blue-900">
              Você está gerenciando o timesheet de <span className="font-semibold">{colaboradorNome ? decodeURIComponent(colaboradorNome) : labelColaborador.toLowerCase()}</span>
            </p>
          </div>
        )}
      </div>
      
      <Tabs value={activeTab} onValueChange={handleTabChange} className="w-full">
        {!isGerenciandoOutroColaborador && (
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-6">
            <div className="overflow-x-auto w-full sm:w-auto">
              <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm min-w-max">
              <TabsTrigger 
                value="timesheet" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <Clock className="h-4 w-4" />
                <span>Timesheet</span>
              </TabsTrigger>
              <TabsTrigger 
                value="management" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <Users className="h-4 w-4" />
                <span>Gestão ADM</span>
              </TabsTrigger>
              <TabsTrigger 
                value="approvals" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <CheckSquare className="h-4 w-4" />
                <span>Aprovações</span>
              </TabsTrigger>
            </TabsList>
            </div>
            
            <div className="flex items-center gap-3 flex-shrink-0">
              <Button 
                variant="outline" 
                className="border-foreground shadow-sm"
                onClick={() => {
                  console.log("Botão Exportar Relatório clicado");
                  setModalExportarRelatorioOpen(true);
                }}
              >
                <Download className="h-4 w-4 mr-2" />
                Exportar Relatório
              </Button>
              
              {activeTab === "management" && (
                <Button 
                  className="px-6 shadow-sm"
                  onClick={() => {
                    // Se houver período fechado, preencher a data
                    if (periodoFechado && periodoFechado.dataFim) {
                      const data = new Date(periodoFechado.dataFim);
                      setDataFim(data);
                    } else {
                      setDataFim(undefined);
                    }
                    setModalFecharPeriodoOpen(true);
                  }}
                >
                  Fechar período
                </Button>
              )}
            </div>
          </div>
        )}
        
        <TabsContent value="timesheet">
          <TimesheetTab 
            cpfColaborador={colaboradorId ? colaboradorId : undefined}
            mes={colaboradorMes ? parseInt(colaboradorMes) : undefined}
            ano={colaboradorAno ? parseInt(colaboradorAno) : undefined}
            onPeriodChange={(mes, ano) => {
              // Atualizar período selecionado
              setSelectedPeriod(`${mes}-${ano}`);
            }}
          />
        </TabsContent>
        
        {!isGerenciandoOutroColaborador && (
          <>
            <TabsContent value="management">
              <ManagementTab
                periodoFechado={periodoFechado}
                onVigenciaChange={handleVigenciaManagementChange}
              />
            </TabsContent>
            
            <TabsContent value="approvals">
              <ApprovalsTab onVigenciaChange={handleVigenciaApprovalsChange} />
            </TabsContent>
          </>
        )}
      </Tabs>

      {/* Modal de Fechar Período */}
      <AlertDialog open={modalFecharPeriodoOpen} onOpenChange={(open) => {
        if (!fechandoPeriodo) {
          setModalFecharPeriodoOpen(open);
          if (!open) {
            setDataFim(undefined);
          }
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <div className="flex items-center gap-3">
              <CalendarIcon className="h-5 w-5 text-primary" />
              <AlertDialogTitle>Fechar Período</AlertDialogTitle>
            </div>
            <AlertDialogDescription>
              Selecione a data desejada no calendário para permitir lançamentos somente após este dia:
            </AlertDialogDescription>
          </AlertDialogHeader>
          
          <div className="space-y-2 py-4">
            <Label>Até dia:</Label>
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
                  {dataFim ? (
                    format(dataFim, "dd/MM/yyyy", { locale: ptBR })
                  ) : (
                    <span>Selecione a data</span>
                  )}
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0" align="start">
                <Calendar
                  mode="single"
                  selected={dataFim}
                  onSelect={setDataFim}
                  locale={ptBR}
                  initialFocus
                  className="pointer-events-auto"
                />
              </PopoverContent>
            </Popover>
          </div>

          <AlertDialogFooter>
            <AlertDialogCancel disabled={fechandoPeriodo}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleFecharPeriodo}
              disabled={fechandoPeriodo || !dataFim}
            >
              {fechandoPeriodo ? "Fechando..." : "Confirmar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal de Exportar Relatório */}
      <AlertDialog open={modalExportarRelatorioOpen} onOpenChange={(open) => {
        if (!exportando) {
          setModalExportarRelatorioOpen(open);
          if (!open) {
            setTipoRelatorio("geral");
          }
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <div className="flex items-center gap-3">
              <FileUp className="h-5 w-5 text-primary" />
              <AlertDialogTitle className="text-primary">Exportar Relatório</AlertDialogTitle>
            </div>
            <AlertDialogDescription>
              {exibirOpcoesLayout ? "Selecione o layout desejado." : "Tem certeza que deseja exportar?"}
            </AlertDialogDescription>
          </AlertDialogHeader>
          
          {exibirOpcoesLayout && (
            <div className="py-4">
              <RadioGroup value={tipoRelatorio} onValueChange={setTipoRelatorio}>
                <div className="flex items-center space-x-2 py-2">
                  <RadioGroupItem value="geral" id="geral" />
                  <Label htmlFor="geral" className="font-normal cursor-pointer">
                    Relatório Geral
                  </Label>
                </div>
                <div className="flex items-center space-x-2 py-2">
                  <RadioGroupItem value="fechamento" id="fechamento" />
                  <Label htmlFor="fechamento" className="font-normal cursor-pointer">
                    Relatório Fechamento
                  </Label>
                </div>
                <div className="flex items-center space-x-2 py-2">
                  <RadioGroupItem value="naoApontado" id="naoApontado" />
                  <Label htmlFor="naoApontado" className="font-normal cursor-pointer">
                    Não Apontado
                  </Label>
                </div>
              </RadioGroup>
            </div>
          )}

          <AlertDialogFooter>
            <AlertDialogCancel disabled={exportando}>Fechar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleExportarRelatorio}
              disabled={exportando}
              className="bg-primary hover:bg-primary/90"
            >
              {exportando ? "Exportando..." : "Exportar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
};

export default Timesheet;
