import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { ArrowLeft, ChevronLeft, ChevronRight, Clock, CheckCircle2, XCircle, Timer } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { Checkbox } from "@/components/ui/checkbox";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { DataTable } from "@presentation/components/common/DataTable";
import type { Column } from "@/hooks/useColumnReorder";
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
import { useAppSelector } from "@app/store/hooks";
import { useParametros } from "@presentation/hooks/useParametros";
import { container } from "@core/di/container";
import { ListarTemplateSemanaVigenciaUseCase } from "@domain/usecases/ListarTemplateSemanaVigenciaUseCase";
import { TimesheetComponentesApi, type ColaboradorApontamento, type ApontamentoMensal, type SemanaTemplate } from "@data/api/TimesheetComponentesApi";
import { PageBreadcrumb } from "@presentation/components/common";
import { WeeklyViewApproval } from "@presentation/components/timesheet/WeeklyViewApproval";
import { toast } from "sonner";

interface SumarioPorProjetoTableProps {
  apontamentosMensais: ApontamentoMensal[];
  apontamentos: ColaboradorApontamento[];
  selectedApontamentos: Set<string>;
  onToggleProjeto: (projetoId: string, checked: boolean) => void;
  formatarHoras: (minutos: number) => string;
  /** Exibe a coluna Aprovador(es). Controlado por parâmetro MOSTRAR_COLUNA_APROVADORES_TIMESHEET no Redux. */
  mostrarColunaAprovadores?: boolean;
}

function SumarioPorProjetoTable({
  apontamentosMensais,
  apontamentos,
  selectedApontamentos,
  onToggleProjeto,
  formatarHoras,
  mostrarColunaAprovadores = false,
}: SumarioPorProjetoTableProps) {
  const getPendingIdsByProjeto = (projetoId: string) =>
    apontamentos
      .filter((ap) => ap.projeto.id === projetoId && ap.codStatusApontamentoGrupo === "1")
      .map((ap) => ap.id);

  const isProjetoRowChecked = (projetoId: string) => {
    const ids = getPendingIdsByProjeto(projetoId);
    return ids.length > 0 && ids.every((id) => selectedApontamentos.has(id));
  };

  const getStatusLabel = (codStatus: number) => {
    switch (codStatus) {
      case 1:
        return "Pendente";
      case 2:
        return "Aprovado";
      case 3:
        return "Reprovado";
      default:
        return "—";
    }
  };

  const getStatusBadgeClass = (codStatus: number) => {
    switch (codStatus) {
      case 1:
        return "bg-blue-600 text-white";
      case 2:
        return "bg-green-600 text-white";
      case 3:
        return "bg-red-600 text-white";
      default:
        return "bg-muted text-muted-foreground";
    }
  };

  const columns: Column[] = [
    { id: "checkbox", label: "", sortable: false, width: "48px" },
    { id: "clienteProjeto", label: "Cliente/Projeto", sortable: true },
    { id: "atividade", label: "Atividade", sortable: true },
    ...(mostrarColunaAprovadores ? [{ id: "aprovadores", label: "Aprovador(es)", sortable: false }] : []),
    { id: "status", label: "Status", sortable: true },
    { id: "horas", label: "Horas", sortable: true },
  ];

  const renderCell = (item: ApontamentoMensal, columnId: string) => {
    const { projeto, atividade, aprovadores, codStatusGrupoMensal, horas } = item;
    const isPendente = codStatusGrupoMensal === 1;
    const clienteProjetoLabel = `${projeto.codCliente} - ${projeto.nomeCliente} / ${projeto.id} - ${projeto.nomeProjeto}`;

    switch (columnId) {
      case "checkbox":
        if (!isPendente) return null;
        return (
          <Checkbox
            checked={isProjetoRowChecked(projeto.id)}
            onCheckedChange={(checked) => {
              onToggleProjeto(projeto.id, checked === true);
            }}
          />
        );
      case "clienteProjeto":
        return <span className="text-sm">{clienteProjetoLabel}</span>;
      case "atividade":
        return <span className="text-sm">{atividade}</span>;
      case "aprovadores":
        if (!mostrarColunaAprovadores) return null;
        if (!aprovadores?.length) return <span className="text-muted-foreground">—</span>;
        const primeiro = aprovadores[0];
        const restantes = aprovadores.length - 1;
        return (
          <div className="flex items-center gap-1 flex-wrap">
            <Badge variant="outline" className="text-xs font-normal">
              {primeiro.nome}
            </Badge>
            {restantes > 0 && (
              <Badge variant="outline" className="text-xs font-normal text-muted-foreground">
                +{restantes}
              </Badge>
            )}
          </div>
        );
      case "status":
        return (
          <Badge className={getStatusBadgeClass(codStatusGrupoMensal)}>
            {getStatusLabel(codStatusGrupoMensal)}
          </Badge>
        );
      case "horas":
        return <span className="text-sm font-medium">{formatarHoras(horas)}</span>;
      default:
        return null;
    }
  };

  const dataWithKey = apontamentosMensais.map((item, index) => ({
    ...item,
    _rowKey: `${item.projeto.id}-${item.atividade}-${index}`,
  }));

  const quantidadeProjetos = new Set(apontamentosMensais.map((m) => m.projeto.id)).size;
  const somaHorasTotal = apontamentosMensais.reduce((acc, m) => acc + m.horas, 0);

  return (
    <div>
      <DataTable
        columns={columns}
        data={dataWithKey}
        renderCell={renderCell}
        keyExtractor={(item) => (item as ApontamentoMensal & { _rowKey: string })._rowKey}
        emptyMessage="Nenhum projeto encontrado."
        stickyLeftColumnIds={["checkbox"]}
      />
      <div className="flex flex-wrap items-center gap-6 py-4 px-4 border-t bg-muted/30 text-sm">
        <span className="font-medium">
          Contagem de projetos: <span className="text-foreground">{quantidadeProjetos}</span>
        </span>
        <span className="font-medium">
          Soma de horas:{" "}
          <span className="text-foreground">{formatarHoras(somaHorasTotal)}</span>
        </span>
      </div>
    </div>
  );
}

export default function AprovarTimesheetColaborador() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { token } = useAppSelector((state) => state.auth);
  const { isEnabled } = useParametros();
  const mostrarColunaAprovadores = isEnabled("MOSTRAR_COLUNA_APROVADORES_TIMESHEET");
  const obrigarJustificativaAprovar = isEnabled("TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_APROVAR");
  const obrigarJustificativaReprovar = isEnabled("TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_REPROVAR");

  const cpfColaborador = searchParams.get('cpfColaborador') || '';
  const mesParam = searchParams.get('mes') || '';
  const anoParam = searchParams.get('ano') || '';
  const nomeColaborador = decodeURIComponent(searchParams.get('nome') || '');
  
  const [mes] = useState<number>(mesParam ? parseInt(mesParam) : new Date().getMonth() + 1);
  const [ano] = useState<number>(anoParam ? parseInt(anoParam) : new Date().getFullYear());
  const [loading, setLoading] = useState(true);
  const [loadingTemplate, setLoadingTemplate] = useState(true);
  const [apontamentos, setApontamentos] = useState<ColaboradorApontamento[]>([]);
  const [apontamentosMensais, setApontamentosMensais] = useState<ApontamentoMensal[]>([]);
  const [semanas, setSemanas] = useState<SemanaTemplate[]>([]);
  const [vigencia, setVigencia] = useState<{ mes: number; ano: number; label: string; horas_trabalhadas: number } | null>(null);
  const [dataColetaDeDados, setDataColetaDeDados] = useState<string>("");
  const [totalizador, setTotalizador] = useState<{
    quantidadeTotalColaboradores: number;
    somaHorasLancadas: number;
    somaHorasAprovadas: number;
    somaHorasPendentes: number;
    somaHorasReprovdas: number;
  } | null>(null);
  
  // Lista de colaboradores para paginação (será preenchida com os colaboradores da lista de aprovações)
  const [colaboradoresList, setColaboradoresList] = useState<Array<{ cpf: string; nome: string }>>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  
  // Estado de seleção de apontamentos
  const [selectedApontamentos, setSelectedApontamentos] = useState<Set<string>>(new Set());
  
  // Estados para modais de aprovação/reprovação
  const [modalAprovarOpen, setModalAprovarOpen] = useState(false);
  const [modalReprovarOpen, setModalReprovarOpen] = useState(false);
  const [justificativa, setJustificativa] = useState("");
  const [processando, setProcessando] = useState(false);
  const [modalErroOpen, setModalErroOpen] = useState(false);
  const [mensagemErro, setMensagemErro] = useState("");

  // Carregar template da semana
  useEffect(() => {
    const loadTemplate = async () => {
      if (!token || !mes || !ano) {
        setLoadingTemplate(false);
        return;
      }

      try {
        setLoadingTemplate(true);
        const useCase = container.resolve(ListarTemplateSemanaVigenciaUseCase);
        const response = await useCase.execute(token, mes, ano);
        
        console.log("Template response completo:", response);
        console.log("Mes:", mes, "Ano:", ano);
        
        // A resposta pode ter diferentes estruturas, vamos verificar todas as possibilidades
        let semanasEncontradas: SemanaTemplate[] = [];
        
        if (response) {
          // Estrutura direta: { semanas: [...] }
          if (response.semanas && Array.isArray(response.semanas)) {
            semanasEncontradas = response.semanas;
          }
          // Estrutura com retorno: { retorno: { semanas: [...] } } (resposta alternativa da API)
          else if ((response as unknown as { retorno?: { semanas: SemanaTemplate[] } }).retorno?.semanas && Array.isArray((response as unknown as { retorno?: { semanas: SemanaTemplate[] } }).retorno?.semanas)) {
            semanasEncontradas = (response as unknown as { retorno: { semanas: SemanaTemplate[] } }).retorno.semanas;
          }
          // Estrutura com dados: { dados: { semanas: [...] } }
          else if ((response as any).dados && (response as any).dados.semanas && Array.isArray((response as any).dados.semanas)) {
            semanasEncontradas = (response as any).dados.semanas;
          }
          // Se a resposta é um array direto
          else if (Array.isArray(response)) {
            semanasEncontradas = response;
          }
          // Estrutura com sucesso e retorno: { sucesso: true, retorno: [...] }
          else if ((response as any).sucesso && (response as any).retorno) {
            const retorno = (response as any).retorno;
            if (Array.isArray(retorno)) {
              semanasEncontradas = retorno;
            } else if (retorno.semanas && Array.isArray(retorno.semanas)) {
              semanasEncontradas = retorno.semanas;
            }
          }
        }
        
        if (semanasEncontradas.length > 0) {
          console.log("Semanas encontradas:", semanasEncontradas.length);
          setSemanas(semanasEncontradas);
        } else {
          console.warn("Nenhuma semana encontrada. Response:", response);
          setSemanas([]);
        }
      } catch (err) {
        console.error("Erro ao carregar template da semana:", err);
        setSemanas([]);
      } finally {
        setLoadingTemplate(false);
      }
    };

    loadTemplate();
  }, [token, mes, ano]);

  // Carregar dados do colaborador
  useEffect(() => {
    const loadData = async () => {
      if (!token || !cpfColaborador || !mes) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        const api = container.resolve(TimesheetComponentesApi);
        const response = await api.listarApontamentosPorVigenciaRelacionadosAoGerente(
          token,
          cpfColaborador,
          mes,
          ano
        );

        if (response.apontamentos_por_vigencia) {
          setApontamentos(response.apontamentos_por_vigencia.colaboradorApontamentos || []);
          setApontamentosMensais(response.apontamentos_por_vigencia.apontamentosMensais || []);
          setDataColetaDeDados(response.apontamentos_por_vigencia.dataColetaDeDados || "");
          
          if (response.apontamentos_por_vigencia.vigencia) {
            const vig = response.apontamentos_por_vigencia.vigencia;
            const meses = [
              "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
              "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
            ];
            setVigencia({
              mes: vig.mes,
              ano: vig.ano,
              label: `${meses[vig.mes - 1]}/${vig.ano}`,
              horas_trabalhadas: vig.horas_trabalhadas || 0,
            });
          }

          if (response.apontamentos_por_vigencia.totalizadorApontamentosBigNumbers) {
            setTotalizador(response.apontamentos_por_vigencia.totalizadorApontamentosBigNumbers);
          }
        }
      } catch (err) {
        console.error("Erro ao carregar dados:", err);
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [token, cpfColaborador, mes, ano]);

  // Carregar lista de colaboradores para paginação (precisamos buscar da lista de aprovações)
  useEffect(() => {
    const loadColaboradoresList = async () => {
      if (!token) return;

      try {
        const api = container.resolve(TimesheetComponentesApi);
        // Buscar a lista de projetos para obter os colaboradores
        const response = await api.listarProjetosVisaoGerenteDeProjeto(
          token,
          {
            codProjeto: '',
            mes: mes,
            ano: ano,
            cpfColaborador: '',
            codStatusGrupo: '0',
            cpfGerenteAdm: '',
          }
        );

        // Extrair colaboradores únicos
        const colaboradoresUnicos = new Map<string, string>();
        response.retorno.lista.forEach(proj => {
          if (!colaboradoresUnicos.has(proj.cpfColaborador)) {
            colaboradoresUnicos.set(proj.cpfColaborador, proj.nomeColaborador);
          }
        });

        const lista = Array.from(colaboradoresUnicos.entries()).map(([cpf, nome]) => ({
          cpf,
          nome,
        }));

        setColaboradoresList(lista);
        
        // Encontrar o índice atual
        const index = lista.findIndex(c => c.cpf === cpfColaborador);
        if (index !== -1) {
          setCurrentIndex(index);
        }
      } catch (err) {
        console.error("Erro ao carregar lista de colaboradores:", err);
      }
    };

    loadColaboradoresList();
  }, [token, mes, ano, cpfColaborador]);

  const handlePrevious = () => {
    if (currentIndex > 0) {
      const prevColab = colaboradoresList[currentIndex - 1];
      const nomeEncoded = encodeURIComponent(prevColab.nome);
      navigate(`/timesheet/aprovacao?cpfColaborador=${prevColab.cpf}&mes=${mes}&ano=${ano}&nome=${nomeEncoded}`);
    }
  };

  const handleNext = () => {
    if (currentIndex < colaboradoresList.length - 1) {
      const nextColab = colaboradoresList[currentIndex + 1];
      const nomeEncoded = encodeURIComponent(nextColab.nome);
      navigate(`/timesheet/aprovacao?cpfColaborador=${nextColab.cpf}&mes=${mes}&ano=${ano}&nome=${nomeEncoded}`);
    }
  };

  // Converter minutos para formato HH:MM
  const formatarHoras = (minutos: number): string => {
    const horas = Math.floor(minutos / 60);
    const mins = minutos % 60;
    return `${String(horas).padStart(2, '0')}:${String(mins).padStart(2, '0')}`;
  };

  // Função para recarregar os dados
  const recarregarDados = async () => {
    if (!token || !cpfColaborador || !mes) return;

    try {
      setLoading(true);
      const api = container.resolve(TimesheetComponentesApi);
      const response = await api.listarApontamentosPorVigenciaRelacionadosAoGerente(
        token,
        cpfColaborador,
        mes,
        ano
      );

      if (response.apontamentos_por_vigencia) {
        setApontamentos(response.apontamentos_por_vigencia.colaboradorApontamentos || []);
        setApontamentosMensais(response.apontamentos_por_vigencia.apontamentosMensais || []);
        setDataColetaDeDados(response.apontamentos_por_vigencia.dataColetaDeDados || "");
        
        if (response.apontamentos_por_vigencia.vigencia) {
          const vig = response.apontamentos_por_vigencia.vigencia;
          const meses = [
            "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
            "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
          ];
          setVigencia({
            mes: vig.mes,
            ano: vig.ano,
            label: `${meses[vig.mes - 1]}/${vig.ano}`,
            horas_trabalhadas: vig.horas_trabalhadas || 0,
          });
        }

        if (response.apontamentos_por_vigencia.totalizadorApontamentosBigNumbers) {
          setTotalizador(response.apontamentos_por_vigencia.totalizadorApontamentosBigNumbers);
        }
      }
    } catch (err) {
      console.error("Erro ao recarregar dados:", err);
    } finally {
      setLoading(false);
    }
  };

  // Função para aprovar apontamentos
  const handleAprovar = async () => {
    if (!token || selectedApontamentos.size === 0) return;

    try {
      setProcessando(true);
      const api = container.resolve(TimesheetComponentesApi);
      
      const ids = Array.from(selectedApontamentos);
      const dataColetaDeDadosParaEnvio = dataColetaDeDados || new Date().toISOString();

      const response = await api.aprovarApontamentoEmLoteGerenteDeProjeto(token, {
        ids,
        justificativa: justificativa || "",
        dataColetaDeDados: dataColetaDeDadosParaEnvio,
      });

      if (response.sucesso) {
        setModalAprovarOpen(false);
        setJustificativa("");
        setSelectedApontamentos(new Set());
        toast.success("Apontamentos aprovados com sucesso!");
        await recarregarDados();
      } else {
        // Se houver mensagem de erro, mostrar modal
        if (response.mensagem) {
          setMensagemErro(response.mensagem);
          setModalAprovarOpen(false);
          setModalErroOpen(true);
        } else {
          toast.error("Erro ao aprovar apontamentos.");
        }
      }
    } catch (err: any) {
      console.error("Erro ao aprovar apontamentos:", err);
      
      // Verificar se o erro contém mensagem da API
      const errorMessage = err?.message || err?.data?.mensagem || "";
      
      if (errorMessage) {
        setMensagemErro(errorMessage);
        setModalAprovarOpen(false);
        setModalErroOpen(true);
      } else {
        toast.error("Erro ao aprovar apontamentos. Tente novamente.");
      }
    } finally {
      setProcessando(false);
    }
  };

  // Função para reprovar apontamentos
  const handleReprovar = async () => {
    if (!token || selectedApontamentos.size === 0) return;

    try {
      setProcessando(true);
      const api = container.resolve(TimesheetComponentesApi);
      
      const ids = Array.from(selectedApontamentos);

      const response = await api.reprovarApontamentosEmLoteGerenteDeProjeto(token, {
        ids,
        justificativa: justificativa || "",
      });

      if (response.sucesso) {
        setModalReprovarOpen(false);
        setJustificativa("");
        setSelectedApontamentos(new Set());
        toast.success("Apontamentos reprovados com sucesso!");
        await recarregarDados();
      } else {
        // Se houver mensagem de erro, mostrar modal
        if (response.mensagem) {
          setMensagemErro(response.mensagem);
          setModalReprovarOpen(false);
          setModalErroOpen(true);
        } else {
          toast.error("Erro ao reprovar apontamentos.");
        }
      }
    } catch (err: any) {
      console.error("Erro ao reprovar apontamentos:", err);
      
      // Verificar se é um erro 400 com mensagem específica
      if (err?.response?.status === 400 || err?.message?.includes("400")) {
        try {
          // Tentar extrair a mensagem do erro se for uma resposta JSON
          const errorData = err?.response?.data || err?.data;
          if (errorData?.mensagem) {
            setMensagemErro(errorData.mensagem);
            setModalReprovarOpen(false);
            setModalErroOpen(true);
            return;
          }
        } catch (parseErr) {
          // Se não conseguir parsear, continuar com o tratamento padrão
        }
      }
      
      // Se a mensagem de erro contém "Período fechado", mostrar modal
      const errorMessage = err?.message || err?.toString() || "";
      if (errorMessage.includes("Período fechado") || errorMessage.includes("período fechado")) {
        setMensagemErro("Período fechado para lançamentos.");
        setModalReprovarOpen(false);
        setModalErroOpen(true);
      } else {
        toast.error("Erro ao reprovar apontamentos. Tente novamente.");
      }
    } finally {
      setProcessando(false);
    }
  };

  return (
    <div className="container mx-auto p-4 sm:p-6 pb-24 space-y-6">
      <PageBreadcrumb
        items={[
          { label: "Timesheet", href: "/timesheet?tab=aprovacao" },
          { label: "Aprovação", href: "/timesheet?tab=aprovacao" },
          { label: nomeColaborador || "Colaborador" },
        ]}
      />

      {/* Header com nome e paginação */}
      <Card>
        <CardContent className="p-4 sm:p-6">
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
            <div className="flex items-center gap-4">
              <Button
                variant="ghost"
                size="sm"
                onClick={() => navigate("/timesheet?tab=aprovacao")}
                className="flex-shrink-0"
              >
                <ArrowLeft className="h-4 w-4 mr-2" />
                Voltar
              </Button>
              <div>
                <h1 className="text-2xl font-semibold">{nomeColaborador || "Colaborador"}</h1>
                {vigencia && (
                  <p className="text-sm text-muted-foreground mt-1">
                    Vigência: {vigencia.label}
                  </p>
                )}
              </div>
            </div>
            
            {colaboradoresList.length > 0 && (
              <div className="flex items-center gap-3">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handlePrevious}
                  disabled={currentIndex === 0}
                >
                  <ChevronLeft className="h-4 w-4" />
                </Button>
                <span className="text-sm text-muted-foreground min-w-[120px] text-center">
                  {currentIndex + 1} / {colaboradoresList.length}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handleNext}
                  disabled={currentIndex === colaboradoresList.length - 1}
                >
                  <ChevronRight className="h-4 w-4" />
                </Button>
              </div>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Big Numbers */}
      {loading ? (
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
          {[...Array(4)].map((_, index) => (
            <Card key={index}>
              <CardContent className="p-4">
                <div className="flex items-start gap-3">
                  <Skeleton className="h-5 w-5 rounded" />
                  <div className="flex-1 space-y-2">
                    <Skeleton className="h-3 w-20" />
                    <Skeleton className="h-8 w-16" />
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : totalizador ? (
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
          <Card>
            <CardContent className="p-4">
              <div className="flex items-start gap-3">
                <div className="mt-1 text-foreground">
                  <Timer className="h-5 w-5" />
                </div>
                <div className="flex-1">
                  <p className="text-xs text-muted-foreground mb-1">Horas lançadas</p>
                  <p className="text-2xl font-bold text-foreground">
                    {formatarHoras(totalizador.somaHorasLancadas)}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="p-4">
              <div className="flex items-start gap-3">
                <div className="mt-1 text-green-600">
                  <CheckCircle2 className="h-5 w-5" />
                </div>
                <div className="flex-1">
                  <p className="text-xs text-muted-foreground mb-1">Aprovadas</p>
                  <p className="text-2xl font-bold text-green-600">
                    {formatarHoras(totalizador.somaHorasAprovadas)}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="p-4">
              <div className="flex items-start gap-3">
                <div className="mt-1 text-blue-600">
                  <Clock className="h-5 w-5" />
                </div>
                <div className="flex-1">
                  <p className="text-xs text-muted-foreground mb-1">Pendentes</p>
                  <p className="text-2xl font-bold text-blue-600">
                    {formatarHoras(totalizador.somaHorasPendentes)}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="p-4">
              <div className="flex items-start gap-3">
                <div className="mt-1 text-destructive">
                  <XCircle className="h-5 w-5" />
                </div>
                <div className="flex-1">
                  <p className="text-xs text-muted-foreground mb-1">Reprovados</p>
                  <p className="text-2xl font-bold text-destructive">
                    {formatarHoras(totalizador.somaHorasReprovdas)}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      ) : null}

      {/* Sumário por projeto */}
      {!loading && apontamentosMensais.length > 0 && (
        <div className="space-y-4">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <h2 className="text-xl font-semibold">Sumário por projeto</h2>
            <div className="flex items-center gap-2">
              <Checkbox
                id="sumario-select-all"
                checked={
                  apontamentos.filter((ap) => ap.codStatusApontamentoGrupo === "1").length > 0 &&
                  selectedApontamentos.size === apontamentos.filter((ap) => ap.codStatusApontamentoGrupo === "1").length
                }
                onCheckedChange={(checked) => {
                  if (checked) {
                    const pendentes = apontamentos
                      .filter((ap) => ap.codStatusApontamentoGrupo === "1")
                      .map((ap) => ap.id);
                    setSelectedApontamentos(new Set(pendentes));
                  } else {
                    setSelectedApontamentos(new Set());
                  }
                }}
              />
              <label htmlFor="sumario-select-all" className="text-sm font-medium cursor-pointer">
                Selecionar todos
              </label>
            </div>
          </div>
          <Card>
            <CardContent className="p-0">
              <SumarioPorProjetoTable
                apontamentosMensais={apontamentosMensais}
                apontamentos={apontamentos}
                selectedApontamentos={selectedApontamentos}
                onToggleProjeto={(projetoId, checked) => {
                  const pendentesDoProjeto = apontamentos.filter(
                    (ap) => ap.projeto.id === projetoId && ap.codStatusApontamentoGrupo === "1"
                  );
                  const ids = pendentesDoProjeto.map((ap) => ap.id);
                  setSelectedApontamentos((prev) => {
                    const next = new Set(prev);
                    if (checked) {
                      ids.forEach((id) => next.add(id));
                    } else {
                      ids.forEach((id) => next.delete(id));
                    }
                    return next;
                  });
                }}
                formatarHoras={formatarHoras}
                mostrarColunaAprovadores={mostrarColunaAprovadores}
              />
            </CardContent>
          </Card>
        </div>
      )}

      {/* Timesheet Semanal */}
      <div>
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold">Timesheet semanal</h2>
          {!loading && !loadingTemplate && (
            <div className="flex items-center gap-2">
              <Checkbox
                id="select-all"
                checked={selectedApontamentos.size > 0 && selectedApontamentos.size === apontamentos.filter(ap => ap.codStatusApontamentoGrupo === "1").length}
                onCheckedChange={(checked) => {
                  if (checked) {
                    // Selecionar todos os apontamentos pendentes (codStatusApontamentoGrupo = "1")
                    const pendentes = apontamentos
                      .filter(ap => ap.codStatusApontamentoGrupo === "1")
                      .map(ap => ap.id);
                    setSelectedApontamentos(new Set(pendentes));
                  } else {
                    setSelectedApontamentos(new Set());
                  }
                }}
              />
              <label htmlFor="select-all" className="text-sm font-medium cursor-pointer">
                Selecionar todos
              </label>
            </div>
          )}
        </div>
        {loading || loadingTemplate ? (
          <Card>
            <CardContent className="p-6">
              <Skeleton className="h-32 w-full" />
            </CardContent>
          </Card>
        ) : (
          <WeeklyViewApproval 
            semanas={semanas} 
            apontamentos={apontamentos} 
            apontamentosMensais={apontamentosMensais}
            mes={mes} 
            ano={ano}
            selectedApontamentos={selectedApontamentos}
            onToggleApontamento={(id) => {
              setSelectedApontamentos(prev => {
                const next = new Set(prev);
                if (next.has(id)) {
                  next.delete(id);
                } else {
                  next.add(id);
                }
                return next;
              });
            }}
            onSelectSemana={(semana) => {
              // Selecionar todos os apontamentos pendentes da semana
              const apontamentosSemana = apontamentos.filter(ap => {
                const apData = new Date(ap.data_registro).toISOString().split('T')[0];
                return semana.dias.some(dia => {
                  const diaData = new Date(dia.data).toISOString().split('T')[0];
                  return apData === diaData && ap.codStatusApontamentoGrupo === "1";
                });
              });
              setSelectedApontamentos(prev => {
                const next = new Set(prev);
                apontamentosSemana.forEach(ap => next.add(ap.id));
                return next;
              });
            }}
          />
        )}
      </div>
      
      {/* Barra fixa no rodapé */}
      <div className="fixed bottom-0 left-0 right-0 bg-background border-t border-border shadow-lg z-50 pointer-events-none">
        <div className="container mx-auto px-4 sm:px-6 py-4 pointer-events-auto">
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
            <div className="flex flex-wrap items-center gap-3">
              <Button
                onClick={() => setModalAprovarOpen(true)}
                disabled={selectedApontamentos.size === 0}
                className="bg-green-600 hover:bg-green-700 text-white disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <CheckCircle2 className="h-4 w-4 mr-2" />
                Aprovar
              </Button>
              <Button
                onClick={() => setModalReprovarOpen(true)}
                disabled={selectedApontamentos.size === 0}
                variant="destructive"
                className="disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <XCircle className="h-4 w-4 mr-2" />
                Reprovar
              </Button>
              <span className="text-sm text-muted-foreground whitespace-nowrap">
                {selectedApontamentos.size} apontamento{selectedApontamentos.size !== 1 ? 's' : ''} pendente{selectedApontamentos.size !== 1 ? 's' : ''} selecionado{selectedApontamentos.size !== 1 ? 's' : ''}.
              </span>
            </div>
            
            {colaboradoresList.length > 0 && (
              <div className="flex items-center gap-3">
                <span className="text-sm text-muted-foreground whitespace-nowrap">{nomeColaborador || "Colaborador"}</span>
                <span className="text-sm text-muted-foreground whitespace-nowrap">(Página {currentIndex + 1}/{colaboradoresList.length})</span>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handlePrevious}
                  disabled={currentIndex === 0}
                >
                  <ChevronLeft className="h-4 w-4" />
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handleNext}
                  disabled={currentIndex === colaboradoresList.length - 1}
                >
                  <ChevronRight className="h-4 w-4" />
                </Button>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Modal de Aprovação */}
      <AlertDialog open={modalAprovarOpen} onOpenChange={(open) => {
        if (!processando) {
          setModalAprovarOpen(open);
          if (!open) {
            setJustificativa("");
          }
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <div className="flex items-center gap-3">
              <Clock className="h-5 w-5 text-blue-600" />
              <AlertDialogTitle>Aprovar apontamentos</AlertDialogTitle>
            </div>
            <AlertDialogDescription>
              Justifique o motivo da aprovação das horas {obrigarJustificativaAprovar ? "(obrigatório)" : "(opcional)"}
            </AlertDialogDescription>
          </AlertDialogHeader>
          
          <div className="space-y-2 py-4">
            <Label htmlFor="justificativa-aprovar">
              Justificativa
              {obrigarJustificativaAprovar && <span className="text-destructive"> *</span>}
            </Label>
            <Textarea
              id="justificativa-aprovar"
              placeholder="Justificativa..."
              value={justificativa}
              onChange={(e) => setJustificativa(e.target.value)}
              className="min-h-[100px]"
              disabled={processando}
            />
          </div>

          <AlertDialogFooter>
            <AlertDialogCancel disabled={processando}>Não</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleAprovar}
              disabled={processando || (obrigarJustificativaAprovar && !justificativa.trim())}
              className="bg-primary"
            >
              {processando ? "Aprovando..." : "Aprovar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal de Reprovação */}
      <AlertDialog open={modalReprovarOpen} onOpenChange={(open) => {
        if (!processando) {
          setModalReprovarOpen(open);
          if (!open) {
            setJustificativa("");
          }
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <div className="flex items-center gap-3">
              <XCircle className="h-5 w-5 text-destructive" />
              <AlertDialogTitle>Reprovar apontamentos</AlertDialogTitle>
            </div>
            <AlertDialogDescription>
              Justifique o motivo da reprovação das horas {obrigarJustificativaReprovar ? "(obrigatório)" : "(opcional)"}
            </AlertDialogDescription>
          </AlertDialogHeader>
          
          <div className="space-y-2 py-4">
            <Label htmlFor="justificativa-reprovar">
              Justificativa
              {obrigarJustificativaReprovar && <span className="text-destructive"> *</span>}
            </Label>
            <Textarea
              id="justificativa-reprovar"
              placeholder="Justificativa..."
              value={justificativa}
              onChange={(e) => setJustificativa(e.target.value)}
              className="min-h-[100px]"
              disabled={processando}
            />
          </div>

          <AlertDialogFooter>
            <AlertDialogCancel disabled={processando}>Não</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleReprovar}
              disabled={processando || (obrigarJustificativaReprovar && !justificativa.trim())}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {processando ? "Reprovando..." : "Reprovar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal de Erro */}
      <AlertDialog open={modalErroOpen} onOpenChange={setModalErroOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <div className="flex items-center gap-3">
              <XCircle className="h-5 w-5 text-destructive" />
              <AlertDialogTitle>Erro</AlertDialogTitle>
            </div>
            <AlertDialogDescription>
              {mensagemErro || "Ocorreu um erro ao processar a solicitação."}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogAction onClick={() => setModalErroOpen(false)}>
              Fechar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}

