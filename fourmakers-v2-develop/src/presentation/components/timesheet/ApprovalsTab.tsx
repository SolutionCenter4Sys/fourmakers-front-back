import { useState, useEffect, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { Search, Check, ChevronsUpDown, X, Clock, CheckCircle2, AlertCircle } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Checkbox } from "@/components/ui/checkbox";
import { Skeleton } from "@/components/ui/skeleton";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { DataTable } from "@presentation/components/common/DataTable";
import { TablePagination } from "@presentation/components/common/TablePagination";
import { StatusBadge } from "@presentation/components/common/StatusBadge";
import type { Column } from "@/hooks/useColumnReorder";
import { useApprovalsTab } from "@/hooks/useTimesheetComponentes";
import { cn } from "@/lib/utils";
import { useAppSelector } from "@app/store/hooks";
import { useParametros } from "@presentation/hooks/useParametros";
import { container } from "@core/di/container";
import { TimesheetComponentesApi, type GestorApontamento, type ProjetoGerenteDeProjeto, type StatusApontamento, type AprovarProjetoVigenciaMensagem } from "@data/api/TimesheetComponentesApi";
import { toast } from "sonner";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";

interface ApprovalsTabProps {
  onVigenciaChange?: (mes: number, ano: number) => void;
}

export const ApprovalsTab = ({ onVigenciaChange }: ApprovalsTabProps) => {
  const navigate = useNavigate();
  
  // Inicializar com mês/ano atual
  const getCurrentMonthYear = () => {
    const now = new Date();
    return {
      mes: now.getMonth() + 1,
      ano: now.getFullYear(),
    };
  };

  const currentMonthYear = getCurrentMonthYear();
  const [mesSelecionado, setMesSelecionado] = useState<string>(String(currentMonthYear.mes));
  const [anoSelecionado, setAnoSelecionado] = useState<string>(String(currentMonthYear.ano));
  const [vigencias, setVigencias] = useState<Array<{ mes: number; ano: number; label: string }>>([]);
  const [vigenciaSelecionada, setVigenciaSelecionada] = useState<string>("");
  const [loadingVigencias, setLoadingVigencias] = useState(true);
  const [mesVigente, setMesVigente] = useState<{ mes: number; ano: number; label: string } | null>(null);
  const [gestorAdm, setGestorAdm] = useState("");
  const [gestorAdmOpen, setGestorAdmOpen] = useState(false);
  const [gestores, setGestores] = useState<GestorApontamento[]>([]);
  const { token } = useAppSelector((state) => state.auth);
  const { getParametro, isEnabled } = useParametros();
  const labelColaboradores = getParametro("LABEL_COLABORADORES_TIMESHEET") || "Colaboradores";
  const labelColaborador = getParametro("LABEL_COLABORADOR_TIMESHEET") || "Colaborador(a)";
  const obrigarJustificativaAprovar = isEnabled("TIMESHEET_OBRIGAR_JUSTIFICATIVA_AO_APROVAR");
  const [projeto, setProjeto] = useState("");
  const [projetoOpen, setProjetoOpen] = useState(false);
  const [projetosList, setProjetosList] = useState<ProjetoGerenteDeProjeto[]>([]);
  const [status, setStatus] = useState("0");
  const [statusList, setStatusList] = useState<StatusApontamento[]>([]);
  const [colaborador, setColaborador] = useState("");
  const [colaboradorOpen, setColaboradorOpen] = useState(false);
  const [colaboradoresList, setColaboradoresList] = useState<Array<{ colaboradorCpf: string; nomeColaborador: string; orgId: number }>>([]);
  const [loadingColaboradores, setLoadingColaboradores] = useState(false);
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [modalAprovarOpen, setModalAprovarOpen] = useState(false);
  const [justificativa, setJustificativa] = useState("");
  const [aprovando, setAprovando] = useState(false);
  const [modalResultadoAprovacaoOpen, setModalResultadoAprovacaoOpen] = useState(false);
  const [resultadoAprovacao, setResultadoAprovacao] = useState<{
    quantidadeProjetosAprovados: number;
    quantidadeProjetosPulados: number;
    mensagens: AprovarProjetoVigenciaMensagem[];
  } | null>(null);
  
  // Estados de paginação
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(20);
  
  // Estado de busca
  const [searchText, setSearchText] = useState("");
  
  // Parâmetros para a busca
  const [searchParams, setSearchParams] = useState<{
    cpfColaborador?: string
    codStatusGrupo?: string
    cpfGerenteAdm?: string
    mes: number
    ano: number
    codProjeto?: string
  }>({
    mes: currentMonthYear.mes,
    ano: currentMonthYear.ano,
    codStatusGrupo: '0',
  });

  // Memoizar os parâmetros para evitar recriação do objeto a cada render
  const approvalsTabParams = useMemo(() => {
    if (!token) return undefined;
    
    return {
      token,
      cpfColaborador: searchParams.cpfColaborador,
      codStatusGrupo: searchParams.codStatusGrupo,
      cpfGerenteAdm: searchParams.cpfGerenteAdm,
      mes: searchParams.mes,
      ano: searchParams.ano,
      codProjeto: searchParams.codProjeto,
      statusList: statusList.map(s => ({ codStatusGrupo: s.codStatusGrupo, descricao: s.descricao })),
    };
  }, [
    token,
    searchParams.cpfColaborador,
    searchParams.codStatusGrupo,
    searchParams.cpfGerenteAdm,
    searchParams.mes,
    searchParams.ano,
    searchParams.codProjeto,
    statusList,
  ]);

  const { stats, projetos, dataColetaDeDados, totalizador, loading, refetch } = useApprovalsTab(approvalsTabParams);

  // Forçar refetch quando parâmetros mudarem
  useEffect(() => {
    if (token && approvalsTabParams) {
      refetch();
    }
  }, [token, searchParams, refetch]);

  // Filtrar projetos baseado no texto de busca
  const projetosFiltrados = useMemo(() => {
    if (!searchText.trim()) {
      return projetos;
    }
    const searchLower = searchText.toLowerCase();
    return projetos.filter((proj) => {
      return (
        proj.colaborador.toLowerCase().includes(searchLower) ||
        proj.projeto.toLowerCase().includes(searchLower) ||
        proj.cliente.toLowerCase().includes(searchLower) ||
        proj.gestorAdm.toLowerCase().includes(searchLower) ||
        proj.status.toLowerCase().includes(searchLower)
      );
    });
  }, [projetos, searchText]);

  // Calcular paginação
  const totalItemsFiltrados = projetosFiltrados.length;
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const projetosPaginados = projetosFiltrados.slice(startIndex, endIndex);

  // Calcular total de itens estimado (similar ao ManagementTab)
  const [calculatedTotalItems, setCalculatedTotalItems] = useState(0);
  
  useEffect(() => {
    if (projetosFiltrados.length > 0) {
      if (endIndex < totalItemsFiltrados) {
        setCalculatedTotalItems(endIndex + 1);
      } else {
        setCalculatedTotalItems(totalItemsFiltrados);
      }
    } else {
      setCalculatedTotalItems(0);
    }
  }, [projetosFiltrados, currentPage, itemsPerPage, endIndex, totalItemsFiltrados]);

  // Carregar gestores de apontamento
  useEffect(() => {
    const loadGestores = async () => {
      if (!token) {
        return;
      }

      try {
        const api = container.resolve(TimesheetComponentesApi);
        const response = await api.listarGestoresApontamento(token, 'pt-BR');
        setGestores(response.retorno || []);
      } catch (err) {
        console.error("Erro ao carregar gestores:", err);
        setGestores([]);
      }
    };

    loadGestores();
  }, [token]);

  // Carregar projetos
  useEffect(() => {
    const loadProjetos = async () => {
      if (!token) {
        return;
      }

      try {
        const api = container.resolve(TimesheetComponentesApi);
        const projetosData = await api.listarProjetosGerenteDeProjeto(token, 'pt-BR');
        // A API retorna um array direto
        setProjetosList(Array.isArray(projetosData) ? projetosData : []);
      } catch (err) {
        console.error("Erro ao carregar projetos:", err);
        setProjetosList([]);
      }
    };

    loadProjetos();
  }, [token]);

  // Carregar status
  useEffect(() => {
    const loadStatus = async () => {
      if (!token) {
        return;
      }

      try {
        const api = container.resolve(TimesheetComponentesApi);
        const statusData = await api.listarStatus(token);
        setStatusList(statusData);
      } catch (err) {
        console.error("Erro ao carregar status:", err);
        setStatusList([]);
      }
    };

    loadStatus();
  }, [token]);

  // Vigência = mês do dia seguinte à data de fechamento (ex.: dataFim 20/01 -> Jan; dataFim 31/01 -> Fev). Se não existir na lista, injeta e seleciona.
  const getMesSubsequenteAoPeriodoFechado = (dataFim: string): { mes: number; ano: number } => {
    const datePart = dataFim.includes("T") ? dataFim.split("T")[0] : dataFim;
    const d = new Date(datePart + "T12:00:00");
    d.setDate(d.getDate() + 1); // dia seguinte à data de fechamento
    const mes = d.getMonth() + 1;
    const ano = d.getFullYear();
    return { mes, ano };
  };

  const formatarLabelVigencia = (mes: number, ano: number): string => {
    const nomes = ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"];
    return `${nomes[mes - 1]}/${ano}`;
  };

  // Carregar vigencias
  useEffect(() => {
    const loadVigencias = async () => {
      if (!token) {
        setLoadingVigencias(false);
        return;
      }

      try {
        setLoadingVigencias(true);
        const api = container.resolve(TimesheetComponentesApi);
        const [response, periodo] = await Promise.all([
          api.listarVigenciasApontamentosGerenteDeProjeto(token),
          api.buscarPeriodoFechado(token, "pt-BR"),
        ]);
        let meses = response.meses || [];
        if (response.mesVigente) setMesVigente(response.mesVigente);

        let mesDefault: number;
        let anoDefault: number;
        if (periodo?.dataFim) {
          const { mes: nextMes, ano: nextAno } = getMesSubsequenteAoPeriodoFechado(periodo.dataFim);
          const existeNaLista = meses.some((v) => v.mes === nextMes && v.ano === nextAno);
          if (!existeNaLista) {
            meses = [...meses, { mes: nextMes, ano: nextAno, label: formatarLabelVigencia(nextMes, nextAno) }];
          }
          mesDefault = nextMes;
          anoDefault = nextAno;
        } else if (response.mesVigente) {
          mesDefault = response.mesVigente.mes;
          anoDefault = response.mesVigente.ano;
        } else if (meses.length > 0) {
          mesDefault = meses[0].mes;
          anoDefault = meses[0].ano;
        } else {
          return;
        }
        setVigencias(meses);
        const vigenciaKey = `${mesDefault}-${anoDefault}`;
        setVigenciaSelecionada(vigenciaKey);
        setMesSelecionado(String(mesDefault));
        setAnoSelecionado(String(anoDefault));
        setSearchParams((prev) => ({ ...prev, mes: mesDefault, ano: anoDefault }));
      } catch (err) {
        console.error("Erro ao carregar vigencias:", err);
        setVigencias([]);
      } finally {
        setLoadingVigencias(false);
      }
    };

    loadVigencias();
  }, [token]);

  // Reportar vigência selecionada ao pai (para exportar relatório usar mes/ano da aba)
  useEffect(() => {
    if (!mesSelecionado || !anoSelecionado || !onVigenciaChange) return;
    const m = parseInt(mesSelecionado, 10);
    const a = parseInt(anoSelecionado, 10);
    if (!Number.isNaN(m) && !Number.isNaN(a)) onVigenciaChange(m, a);
  }, [mesSelecionado, anoSelecionado, onVigenciaChange]);

  // Carregar colaboradores quando projeto ou vigência mudarem
  useEffect(() => {
    const loadColaboradores = async () => {
      if (!token || !projeto || !mesSelecionado || !anoSelecionado) {
        setColaboradoresList([]);
        setColaborador("");
        return;
      }

      try {
        setLoadingColaboradores(true);
        const api = container.resolve(TimesheetComponentesApi);
        const colaboradoresData = await api.listarColaboradoresVinculadosGerenteDeProjeto(
          token,
          projeto,
          parseInt(mesSelecionado),
          parseInt(anoSelecionado)
        );
        setColaboradoresList(colaboradoresData || []);
        // Limpar seleção de colaborador quando mudar o projeto
        setColaborador("");
      } catch (err) {
        console.error("Erro ao carregar colaboradores:", err);
        setColaboradoresList([]);
      } finally {
        setLoadingColaboradores(false);
      }
    };

    loadColaboradores();
  }, [token, projeto, mesSelecionado, anoSelecionado]);

  const columns: Column[] = [
    { id: "checkbox", label: "", sortable: false, width: "50px" },
    { id: "colaborador", label: labelColaborador, sortable: true },
    { id: "situacao", label: "Situação", sortable: true },
    { id: "dataSituacao", label: "Data situação", sortable: true },
    { id: "cliente", label: "Cliente", sortable: true },
    { id: "projeto", label: "Projeto", sortable: true },
    { id: "fimProjeto", label: "Fim do Projeto", sortable: true },
    { id: "gestorAdm", label: "Gestor Adm", sortable: true },
    { id: "status", label: "Status", sortable: true },
    { id: "horas", label: "Horas", sortable: true },
  ];

  const handleSelectItem = (id: string) => {
    setSelectedItems(prev => 
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    );
  };

  const renderCell = (item: typeof projetos[0], columnId: string) => {
    // Verificar se deve mostrar checkbox baseado nos dados do item
    const mostrarCheckbox = item.codStatusMensal === 1 && item.ativo === true;

    switch (columnId) {
      case "checkbox":
        if (!mostrarCheckbox) {
          return null;
        }
        return (
          <Checkbox
            checked={selectedItems.includes(item.id)}
            onCheckedChange={() => handleSelectItem(item.id)}
          />
        );
      case "colaborador":
        return (
          <button
            onClick={() => {
              const nomeEncoded = encodeURIComponent(item.colaborador);
              const cpf = (item as any).cpfColaborador;
              const mes = (item as any).mes;
              const ano = (item as any).ano;
              if (cpf && mes && ano) {
                navigate(`/timesheet/aprovacao?cpfColaborador=${cpf}&mes=${mes}&ano=${ano}&nome=${nomeEncoded}`);
              }
            }}
            className="font-medium text-primary underline hover:underline cursor-pointer text-left"
          >
            {item.colaborador}
          </button>
        );
      case "situacao":
        const isInativo = item.situacao === 'Inativo';
        return (
          <Badge className={isInativo ? "bg-red-100 text-red-700 hover:bg-red-100" : "bg-green-100 text-green-700 hover:bg-green-100"}>
            {item.situacao}
          </Badge>
        );
      case "dataSituacao":
        return item.dataSituacao;
      case "cliente":
        return item.cliente;
      case "projeto":
        return item.projeto;
      case "fimProjeto":
        return item.fimProjeto || '---';
      case "gestorAdm":
        return item.gestorAdm;
      case "status":
        return <StatusBadge status={item.status} variant="timesheet" />;
      case "horas":
        return item.horas;
      default:
        return null;
    }
  };

  const handleBuscar = () => {
    setSearchParams({
      cpfColaborador: colaborador || undefined,
      codStatusGrupo: status || '0',
      cpfGerenteAdm: gestorAdm || undefined,
      mes: parseInt(mesSelecionado),
      ano: parseInt(anoSelecionado),
      codProjeto: projeto || undefined,
    });
    
    if (refetch) {
      refetch();
    }
  };

  const handleLimpar = () => {
    // Resetar para a vigência vigente
    if (mesVigente) {
      const vigenciaKey = `${mesVigente.mes}-${mesVigente.ano}`;
      setVigenciaSelecionada(vigenciaKey);
      setMesSelecionado(String(mesVigente.mes));
      setAnoSelecionado(String(mesVigente.ano));
      setSearchParams({
        mes: mesVigente.mes,
        ano: mesVigente.ano,
        codStatusGrupo: '0',
      });
    } else if (vigencias.length > 0) {
      // Fallback: usar a primeira vigência da lista
      const vigenciaVigente = vigencias[0];
      const vigenciaKey = `${vigenciaVigente.mes}-${vigenciaVigente.ano}`;
      setVigenciaSelecionada(vigenciaKey);
      setMesSelecionado(String(vigenciaVigente.mes));
      setAnoSelecionado(String(vigenciaVigente.ano));
      setSearchParams({
        mes: vigenciaVigente.mes,
        ano: vigenciaVigente.ano,
        codStatusGrupo: '0',
      });
    } else {
      setMesSelecionado(String(currentMonthYear.mes));
      setAnoSelecionado(String(currentMonthYear.ano));
      setVigenciaSelecionada("");
      setSearchParams({
        mes: currentMonthYear.mes,
        ano: currentMonthYear.ano,
        codStatusGrupo: '0',
      });
    }
    setProjeto("");
    setColaborador("");
    setGestorAdm("");
    setStatus("0");
    setSearchText("");
    setCurrentPage(1);
    setSelectedItems([]);
    
    if (refetch) {
      refetch();
    }
  };

  const handleAprovar = async () => {
    if (!token || projetosSelecionadosValidos.length === 0) return;

    try {
      setAprovando(true);
      const api = container.resolve(TimesheetComponentesApi);
      
      // Obter cpf e codProjeto de cada registro selecionado (payload esperado pela API)
      const projetosSelecionados = projetosSelecionadosValidos
        .map(id => {
          const projeto = projetos.find(p => p.id === id);
          if (!projeto?.cpfColaborador || !projeto?.codProjeto) return null;
          return { cpf: projeto.cpfColaborador, codProjeto: projeto.codProjeto };
        })
        .filter((p): p is { cpf: string; codProjeto: string } => p !== null);

      if (projetosSelecionados.length === 0) return;

      // Usar dataColetaDeDados da resposta da API (ou data atual como fallback)
      const dataColetaDeDadosParaEnvio = dataColetaDeDados || new Date().toISOString();

      const response = await api.aprovarProjetoVigenciaEmLote(token, {
        mes: parseInt(mesSelecionado),
        ano: parseInt(anoSelecionado),
        justificativa: justificativa || "",
        projetos: projetosSelecionados,
        dataColetaDeDados: dataColetaDeDadosParaEnvio,
      });

      setModalAprovarOpen(false);
      setJustificativa("");
      setSelectedItems([]);
      if (refetch) refetch();

      const retorno = response.retorno;
      const haPulados = (retorno?.quantidadeProjetosPulados ?? 0) > 0;
      if (haPulados && retorno) {
        setResultadoAprovacao({
          quantidadeProjetosAprovados: retorno.quantidadeProjetosAprovados,
          quantidadeProjetosPulados: retorno.quantidadeProjetosPulados,
          mensagens: retorno.mensagens ?? [],
        });
        setModalResultadoAprovacaoOpen(true);
      } else {
        toast.success("Projetos aprovados com sucesso!");
      }
    } catch (err) {
      console.error("Erro ao aprovar projetos:", err);
      toast.error("Erro ao aprovar projetos. Tente novamente.");
    } finally {
      setAprovando(false);
    }
  };

  // Projetos selecionados válidos (apenas os que podem ser aprovados)
  const projetosSelecionadosValidos = useMemo(() => {
    return selectedItems.filter(id => {
      const projeto = projetos.find(p => p.id === id);
      return projeto?.codStatusMensal === 1 && projeto?.ativo === true;
    });
  }, [selectedItems, projetos]);

  return (
    <div className="space-y-6 relative pb-20">
      {/* Filtros Superiores */}
      <Card>
        <CardContent className="p-4 sm:p-6">
          <div className="flex flex-wrap gap-3 sm:gap-4 items-end">
            <div className="space-y-2 w-full sm:min-w-[180px] sm:w-auto">
              <label className="text-sm font-medium">Vigência</label>
              <Select 
                value={vigenciaSelecionada} 
                onValueChange={(value) => {
                  setVigenciaSelecionada(value);
                  const [mes, ano] = value.split('-');
                  setMesSelecionado(mes);
                  setAnoSelecionado(ano);
                  setSearchParams({
                    ...searchParams,
                    mes: parseInt(mes),
                    ano: parseInt(ano),
                  });
                }}
                disabled={loadingVigencias}
              >
                <SelectTrigger className="border-muted-foreground/30">
                  <SelectValue placeholder={loadingVigencias ? "Carregando..." : "Selecione"} />
                </SelectTrigger>
                <SelectContent>
                  {vigencias.map((vigencia) => {
                    const key = `${vigencia.mes}-${vigencia.ano}`;
                    return (
                      <SelectItem key={key} value={key}>
                        {vigencia.label}
                      </SelectItem>
                    );
                  })}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2 min-w-[200px] flex-1">
              <label className="text-sm font-medium">Projeto</label>
              <Popover open={projetoOpen} onOpenChange={setProjetoOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={projetoOpen}
                    className="w-full justify-between border-muted-foreground/30"
                  >
                    <span className="truncate flex-1 text-left mr-2">
                      {projeto
                        ? (() => {
                            const proj = projetosList.find((p) => p.codProjeto === projeto);
                            return proj ? `${proj.codProjeto} - ${proj.nomeProjeto}` : "Selecione";
                          })()
                        : "0 - Todos"}
                    </span>
                    <ChevronsUpDown className="h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[400px] p-0">
                  <Command>
                    <CommandInput placeholder="Buscar projeto..." />
                    <CommandList>
                      <CommandEmpty>Nenhum projeto encontrado.</CommandEmpty>
                      <CommandGroup>
                        <CommandItem
                          value="0"
                          onSelect={() => {
                            setProjeto("");
                            setProjetoOpen(false);
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              projeto === "" ? "opacity-100" : "opacity-0"
                            )}
                          />
                          0 - Todos
                        </CommandItem>
                        {projetosList.map((proj) => (
                          <CommandItem
                            key={proj.codProjeto}
                            value={proj.codProjeto}
                            onSelect={() => {
                              setProjeto(proj.codProjeto);
                              setProjetoOpen(false);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                projeto === proj.codProjeto ? "opacity-100" : "opacity-0"
                              )}
                            />
                            {proj.codProjeto} - {proj.nomeProjeto}
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2 flex-1 w-full sm:min-w-[200px]">
              <label className="text-sm font-medium">{labelColaborador}</label>
              <Popover open={colaboradorOpen} onOpenChange={setColaboradorOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={colaboradorOpen}
                    className="w-full justify-between border-muted-foreground/30"
                    disabled={!projeto || loadingColaboradores}
                  >
                    {colaborador
                      ? colaboradoresList.find((c) => c.colaboradorCpf === colaborador)?.nomeColaborador || "Selecione"
                      : loadingColaboradores ? "Carregando..." : projeto ? "Todos" : "Selecione um projeto"}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[400px] p-0">
                  <Command>
                    <CommandInput placeholder={`Buscar ${labelColaborador.toLowerCase()}...`} />
                    <CommandList>
                      <CommandEmpty>
                        {!projeto ? "Selecione um projeto primeiro" : `Nenhum ${labelColaborador.toLowerCase()} encontrado.`}
                      </CommandEmpty>
                      <CommandGroup>
                        <CommandItem
                          value="0"
                          onSelect={() => {
                            setColaborador("");
                            setColaboradorOpen(false);
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              colaborador === "" ? "opacity-100" : "opacity-0"
                            )}
                          />
                          Todos
                        </CommandItem>
                        {colaboradoresList.map((colab) => (
                          <CommandItem
                            key={colab.colaboradorCpf}
                            value={colab.colaboradorCpf}
                            onSelect={() => {
                              setColaborador(colab.colaboradorCpf);
                              setColaboradorOpen(false);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                colaborador === colab.colaboradorCpf ? "opacity-100" : "opacity-0"
                              )}
                            />
                            {colab.nomeColaborador}
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2 min-w-[200px] flex-1">
              <label className="text-sm font-medium">Gestor Adm</label>
              <Popover open={gestorAdmOpen} onOpenChange={setGestorAdmOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={gestorAdmOpen}
                    className="w-full justify-between border-muted-foreground/30"
                  >
                    {gestorAdm && Array.isArray(gestores)
                      ? gestores.find((g) => g.codigoGerente === gestorAdm)?.nomeCompleto || "Selecione"
                      : "Todos"}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[400px] p-0">
                  <Command>
                    <CommandInput placeholder="Buscar gestor..." />
                    <CommandList>
                      <CommandEmpty>Nenhum gestor encontrado.</CommandEmpty>
                      <CommandGroup>
                        <CommandItem
                          value="0"
                          onSelect={() => {
                            setGestorAdm("");
                            setGestorAdmOpen(false);
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              gestorAdm === "" ? "opacity-100" : "opacity-0"
                            )}
                          />
                          Todos
                        </CommandItem>
                        {Array.isArray(gestores) && gestores.map((gestor, index) => (
                          <CommandItem
                            key={`${gestor.codigoGerente}-${index}`}
                            value={gestor.codigoGerente}
                            onSelect={() => {
                              setGestorAdm(gestor.codigoGerente);
                              setGestorAdmOpen(false);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                gestorAdm === gestor.codigoGerente ? "opacity-100" : "opacity-0"
                              )}
                            />
                            {gestor.nomeCompleto}
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2 w-full sm:min-w-[180px] sm:w-auto">
              <label className="text-sm font-medium">Status</label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    className="w-full justify-between border-muted-foreground/30"
                  >
                    {status === "0" || status === ""
                      ? "Todos"
                      : statusList.find((s) => s.codStatusGrupo.toString() === status)?.descricao || "Selecione"}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[300px] p-0">
                  <Command>
                    <CommandInput placeholder="Buscar status..." />
                    <CommandList>
                      <CommandEmpty>Nenhum status encontrado.</CommandEmpty>
                      <CommandGroup>
                        <CommandItem
                          value="0"
                          onSelect={() => {
                            setStatus("0");
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              status === "0" || status === "" ? "opacity-100" : "opacity-0"
                            )}
                          />
                          Todos
                        </CommandItem>
                        {statusList.map((s) => (
                          <CommandItem
                            key={s.codStatusGrupo}
                            value={s.codStatusGrupo.toString()}
                            onSelect={() => {
                              setStatus(s.codStatusGrupo.toString());
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                status === s.codStatusGrupo.toString() ? "opacity-100" : "opacity-0"
                              )}
                            />
                            {s.descricao}
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <Button 
              onClick={handleBuscar}
              className="px-8"
            >
              Buscar
            </Button>

            <Button 
              variant="outline" 
              onClick={handleLimpar}
              className="text-muted-foreground hover:text-foreground"
            >
              Limpar
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Big Numbers */}
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3 sm:gap-4">
        {loading ? (
          <>
            {[...Array(6)].map((_, index) => (
              <Card key={index} className="bg-card">
                <CardContent className="p-3 sm:p-4">
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
          </>
        ) : (
          stats.map((stat, index) => (
            <Card key={index} className="bg-card">
              <CardContent className="p-3 sm:p-4">
                <div className="flex items-start gap-3">
                  <div className={`mt-1 ${stat.textColor}`}>
                    <stat.icon className="h-5 w-5" />
                  </div>
                  <div className="flex-1">
                    <p className="text-xs text-muted-foreground mb-1">
                      {stat.label}
                    </p>
                    <p className={`text-2xl font-bold ${stat.textColor}`}>
                      {stat.value}
                    </p>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))
        )}
      </div>

      {/* Meus Projetos */}
      <div className="space-y-4">
        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
          <h2 className="text-2xl font-semibold">Meus projetos</h2>
          <div className="flex items-center gap-2 w-full sm:w-auto">
            <div className="relative flex-1 sm:flex-initial">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar..."
                value={searchText}
                onChange={(e) => {
                  setSearchText(e.target.value);
                  setCurrentPage(1); // Reset para primeira página ao buscar
                }}
                className="pl-10 w-full sm:w-[300px] border-muted-foreground/30"
              />
              {searchText && (
                <Button
                  variant="ghost"
                  size="sm"
                  className="absolute right-1 top-1/2 -translate-y-1/2 h-6 w-6 p-0"
                  onClick={() => {
                    setSearchText("");
                    setCurrentPage(1);
                  }}
                >
                  <X className="h-4 w-4" />
                </Button>
              )}
            </div>
          </div>
        </div>

        <Card>
          <CardContent className="p-6">
            {loading ? (
              <div className="space-y-3">
                {[...Array(5)].map((_, index) => (
                  <div key={index} className="flex items-center gap-4 p-4 border-b">
                    <Skeleton className="h-4 w-4 rounded" />
                    <Skeleton className="h-4 w-[180px]" />
                    <Skeleton className="h-4 w-[80px]" />
                    <Skeleton className="h-4 w-[120px]" />
                    <Skeleton className="h-4 w-[150px]" />
                    <Skeleton className="h-4 w-[120px]" />
                    <Skeleton className="h-4 w-[100px]" />
                  </div>
                ))}
              </div>
            ) : (
              <>
                <DataTable
                  columns={columns}
                  data={projetosPaginados}
                  renderCell={renderCell}
                  keyExtractor={(item) => item.id}
                />
                {/* Totais do totalizador (quantidadeTotalColaboradores, quantidadeTotalProjetos, somaHoras) */}
                {!loading && totalizador && (
                  <div className="flex flex-wrap items-center gap-6 py-4 px-1 border-t bg-muted/30 text-sm">
                    <span className="font-medium">
                      Contagem de {labelColaboradores}:{" "}
                      <span className="text-foreground">{totalizador.quantidadeTotalColaboradores}</span>
                    </span>
                    <span className="font-medium">
                      Contagem de projetos:{" "}
                      <span className="text-foreground">{totalizador.quantidadeTotalProjetos}</span>
                    </span>
                    <span className="font-medium">
                      Soma de horas:{" "}
                      <span className="text-foreground">
                        {(() => {
                          const min = totalizador.somaHoras;
                          const h = Math.floor(min / 60);
                          const m = min % 60;
                          return `${String(h).padStart(2, "0")}:${String(m).padStart(2, "0")}`;
                        })()}
                      </span>
                    </span>
                  </div>
                )}
                {(calculatedTotalItems > 0 || totalItemsFiltrados > 0) && (
                  <TablePagination
                    currentPage={currentPage}
                    totalItems={calculatedTotalItems || totalItemsFiltrados}
                    itemsPerPage={itemsPerPage}
                    onPageChange={setCurrentPage}
                    onItemsPerPageChange={(value) => {
                      setItemsPerPage(typeof value === 'string' ? parseInt(value, 10) : value);
                      setCurrentPage(1);
                    }}
                  />
                )}
              </>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Botão FAB Aprovar */}
      <div className="fixed bottom-6 right-6 z-50 flex flex-row gap-3">
        <Button
          onClick={() => setModalAprovarOpen(true)}
          disabled={projetosSelecionadosValidos.length === 0}
          size="lg"
          className="bg-green-600 hover:bg-green-700 text-white shadow-lg hover:shadow-xl transition-all duration-200 rounded-full h-12 px-6 min-w-[140px]"
        >
          <CheckCircle2 className="h-5 w-5 mr-2" />
          Aprovar
        </Button>
      </div>

      {/* Modal de Aprovação */}
      <AlertDialog open={modalAprovarOpen} onOpenChange={(open) => {
        if (!aprovando) {
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
            <Label htmlFor="justificativa">
              Justificativa
              {obrigarJustificativaAprovar && <span className="text-destructive"> *</span>}
            </Label>
            <Textarea
              id="justificativa"
              placeholder="Justificativa..."
              value={justificativa}
              onChange={(e) => setJustificativa(e.target.value)}
              className="min-h-[100px]"
            />
          </div>

          <AlertDialogFooter>
            <AlertDialogCancel disabled={aprovando}>Não</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleAprovar}
              disabled={aprovando || (obrigarJustificativaAprovar && !justificativa.trim())}
              className="bg-primary"
            >
              {aprovando ? "Aprovando..." : "Aprovar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal: Nem todos os projetos foram aprovados */}
      <Dialog open={modalResultadoAprovacaoOpen} onOpenChange={setModalResultadoAprovacaoOpen}>
        <DialogContent className="max-w-md sm:max-w-lg">
          <DialogHeader>
            <DialogTitle className="text-center">Nem todos os projetos foram aprovados.</DialogTitle>
          </DialogHeader>
          {resultadoAprovacao && (
            <>
              <div className="grid grid-cols-2 gap-4 py-2">
                <Card className="border-green-200 bg-green-50/50">
                  <CardContent className="flex items-center gap-3 p-4">
                    <CheckCircle2 className="h-8 w-8 shrink-0 text-green-600" />
                    <div>
                      <p className="text-sm font-medium text-muted-foreground">Projetos aprovados</p>
                      <p className="text-2xl font-semibold text-green-700">{resultadoAprovacao.quantidadeProjetosAprovados}</p>
                    </div>
                  </CardContent>
                </Card>
                <Card className="border-red-200 bg-red-50/50">
                  <CardContent className="flex items-center gap-3 p-4">
                    <AlertCircle className="h-8 w-8 shrink-0 text-red-600" />
                    <div>
                      <p className="text-sm font-medium text-muted-foreground">Projetos não aprovados</p>
                      <p className="text-2xl font-semibold text-red-700">{resultadoAprovacao.quantidadeProjetosPulados}</p>
                    </div>
                  </CardContent>
                </Card>
              </div>
              {resultadoAprovacao.mensagens.length > 0 && (
                <div className="space-y-2">
                  <p className="text-sm font-medium">Logs:</p>
                  <div className="max-h-[240px] overflow-y-auto space-y-2 rounded-md border border-border p-2">
                    {resultadoAprovacao.mensagens.map((msg, idx) => {
                      const item = projetos.find(
                        (p) => p.cpfColaborador === msg.cpf && p.codProjeto === msg.codProjeto
                      );
                      const linha = item
                        ? `${item.projeto} - ${item.colaborador} - ${msg.mensagem}`
                        : `${msg.codProjeto} - ${msg.cpf} - ${msg.mensagem}`;
                      return (
                        <div
                          key={`${msg.cpf}-${msg.codProjeto}-${idx}`}
                          className="rounded bg-red-50 px-3 py-2 text-sm text-red-900 border border-red-100"
                        >
                          {linha}
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}
            </>
          )}
          <DialogFooter className="sm:justify-center">
            <Button variant="outline" onClick={() => setModalResultadoAprovacaoOpen(false)}>
              Voltar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};
