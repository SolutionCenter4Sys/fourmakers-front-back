import { useState, useEffect, useMemo } from "react";
import { Search, Mail, CalendarIcon, Check, ChevronsUpDown } from "@/components/ui/system-icons";
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
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
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
import { useManagementTab } from "@/hooks/useTimesheetComponentes";
import { cn } from "@/lib/utils";
import { useAppSelector } from "@app/store/hooks";
import { useParametros } from "@presentation/hooks/useParametros";
import { container } from "@core/di/container";
import { TimesheetComponentesApi, type GestorApontamento, type ProjetoGerenteDeProjeto, type GestorProjeto, type StatusApontamento, type PeriodoFechado } from "@data/api/TimesheetComponentesApi";
import { useNavigate } from "react-router-dom";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";

interface ManagementTabProps {
  /** Período fechado vindo do pai; ao atualizar (ex.: após fechar período no modal), a combo vigência aplica a regra do mês subsequente. */
  periodoFechado?: PeriodoFechado | null;
  onVigenciaChange?: (mes: number, ano: number) => void;
}

function getMesSubsequenteAoPeriodoFechado(dataFim: string): { mes: number; ano: number } {
  const datePart = dataFim.includes("T") ? dataFim.split("T")[0] : dataFim;
  const d = new Date(datePart + "T12:00:00");
  d.setDate(d.getDate() + 1);
  return { mes: d.getMonth() + 1, ano: d.getFullYear() };
}

export const ManagementTab = ({ periodoFechado: periodoFechadoProp, onVigenciaChange }: ManagementTabProps) => {
  const navigate = useNavigate();
  const { getParametro } = useParametros();
  const labelColaboradores = getParametro("LABEL_COLABORADORES_TIMESHEET") || "Colaboradores";
  const labelColaborador = getParametro("LABEL_COLABORADOR_TIMESHEET") || "Colaborador(a)";
  // Inicializar com mês/ano atual
  const getCurrentMonthYear = () => {
    const now = new Date();
    return {
      mes: now.getMonth() + 1, // getMonth retorna 0-11, então adicionamos 1
      ano: now.getFullYear(),
    };
  };

  const currentMonthYear = getCurrentMonthYear();
  const [mesSelecionado, setMesSelecionado] = useState<string>(String(currentMonthYear.mes));
  const [anoSelecionado, setAnoSelecionado] = useState<string>(String(currentMonthYear.ano));
  const [popoverOpen, setPopoverOpen] = useState(false);
  const [gestorAdm, setGestorAdm] = useState("");
  const [gestorAdmOpen, setGestorAdmOpen] = useState(false);
  const [gestores, setGestores] = useState<GestorApontamento[]>([]);
  const [loadingGestores, setLoadingGestores] = useState(true);
  const { token, user } = useAppSelector((state) => state.auth);
  
  // Verificar se o usuário tem a funcionalidade APONTAMENTO_HORAS_COLABORADOR
  const temApontamentoHorasColaborador = user?.funcionalidadeSistema?.some(
    (func) => func.descricao === "APONTAMENTO_HORAS_COLABORADOR" && func.ativo === true
  ) ?? false;
  const [projeto, setProjeto] = useState("");
  const [projetoOpen, setProjetoOpen] = useState(false);
  const [projetos, setProjetos] = useState<ProjetoGerenteDeProjeto[]>([]);
  const [loadingProjetos, setLoadingProjetos] = useState(true);
  const [aprovador, setAprovador] = useState("");
  const [aprovadorOpen, setAprovadorOpen] = useState(false);
  const [aprovadores, setAprovadores] = useState<GestorProjeto[]>([]);
  const [loadingAprovadores, setLoadingAprovadores] = useState(true);
  const [status, setStatus] = useState("0");
  const [statusList, setStatusList] = useState<StatusApontamento[]>([]);
  const [loadingStatus, setLoadingStatus] = useState(true);
  const [searchColaborador, setSearchColaborador] = useState("");
  const [debouncedSearchColaborador, setDebouncedSearchColaborador] = useState("");
  const [modalNotificarOpen, setModalNotificarOpen] = useState(false);
  const [enviandoNotificacao, setEnviandoNotificacao] = useState(false);
  const [modalAprovadoresOpen, setModalAprovadoresOpen] = useState(false);
  const [aprovadoresModalList, setAprovadoresModalList] = useState<Array<{ nomeAprovador: string }>>([]);

  // Estados de paginação - mesmo comportamento de /colaboradores
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(20);
  
  // Parâmetros para a busca
  const [searchParams, setSearchParams] = useState<{
    nomeColaborador?: string
    codigoGerente?: string
    codigoStatus?: string
    mesVigencia: number
    anoVigencia: number
    codProjeto?: string
    codColaboradorExternoAprovador?: string
  }>({
    mesVigencia: currentMonthYear.mes,
    anoVigencia: currentMonthYear.ano,
  });

  // Vigência padrão = mês subsequente ao período fechado (ex.: fechado até 30/11/2025 -> Dez/2025)
  useEffect(() => {
    const loadPeriodoFechado = async () => {
      if (!token) return;
      try {
        const api = container.resolve(TimesheetComponentesApi);
        const periodo = await api.buscarPeriodoFechado(token, "pt-BR");
        if (periodo?.dataFim) {
          const { mes: nextMes, ano: nextAno } = getMesSubsequenteAoPeriodoFechado(periodo.dataFim);
          setMesSelecionado(String(nextMes));
          setAnoSelecionado(String(nextAno));
          setSearchParams((prev) => ({
            ...prev,
            mesVigencia: nextMes,
            anoVigencia: nextAno,
          }));
        }
      } catch (err) {
        console.error("Erro ao carregar período fechado:", err);
      }
    };
    loadPeriodoFechado();
  }, [token]);

  // Ao receber período fechado do pai (ex.: após fechar período no modal), aplicar a regra na combo vigência sem precisar de refresh
  useEffect(() => {
    if (!periodoFechadoProp?.dataFim) return;
    const { mes: nextMes, ano: nextAno } = getMesSubsequenteAoPeriodoFechado(periodoFechadoProp.dataFim);
    setMesSelecionado(String(nextMes));
    setAnoSelecionado(String(nextAno));
    setSearchParams((prev) => ({
      ...prev,
      mesVigencia: nextMes,
      anoVigencia: nextAno,
    }));
  }, [periodoFechadoProp?.dataFim]);

  // Reportar vigência selecionada ao pai (para exportar relatório usar mes/ano da aba)
  useEffect(() => {
    if (!mesSelecionado || !anoSelecionado || !onVigenciaChange) return;
    const m = parseInt(mesSelecionado, 10);
    const a = parseInt(anoSelecionado, 10);
    if (!Number.isNaN(m) && !Number.isNaN(a)) onVigenciaChange(m, a);
  }, [mesSelecionado, anoSelecionado, onVigenciaChange]);

  // Debounce do termo de busca e reset da página
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchColaborador(searchColaborador);
      setCurrentPage(1); // Volta para primeira página ao buscar
    }, 500);

    return () => clearTimeout(timer);
  }, [searchColaborador]);

  // Calcular cursor baseado na página atual (mesmo comportamento de /colaboradores)
  const cursor = (currentPage - 1) * itemsPerPage;

  // Memoizar os parâmetros para evitar recriação do objeto a cada render
  const managementTabParams = useMemo(() => {
    if (!token) return undefined;
    return {
      token,
      nomeColaborador: debouncedSearchColaborador,
      codigoGerente: searchParams.codigoGerente,
      codigoStatus: searchParams.codigoStatus,
      mesVigencia: searchParams.mesVigencia,
      anoVigencia: searchParams.anoVigencia,
      codProjeto: searchParams.codProjeto,
      codColaboradorExternoAprovador: searchParams.codColaboradorExternoAprovador,
      cursor,
      limite: itemsPerPage,
      statusList: statusList.map(s => ({ codStatusGrupo: s.codStatusGrupo, descricao: s.descricao })),
    };
  }, [
    token,
    debouncedSearchColaborador,
    searchParams.codigoGerente,
    searchParams.codigoStatus,
    searchParams.mesVigencia,
    searchParams.anoVigencia,
    searchParams.codProjeto,
    searchParams.codColaboradorExternoAprovador,
    currentPage,
    itemsPerPage,
    statusList,
  ]);

  const { stats, colaboradores, loading, totalItems, hasMore, refetch } = useManagementTab(managementTabParams);

  // Forçar refetch quando página ou limite mudarem (mesmo comportamento de /colaboradores)
  useEffect(() => {
    if (token && managementTabParams) {
      refetch();
    }
  }, [token, currentPage, itemsPerPage, debouncedSearchColaborador, refetch]);

  // Atualiza o total de itens estimado baseado na página atual e se há mais (mesmo comportamento de /colaboradores)
  const [calculatedTotalItems, setCalculatedTotalItems] = useState(0);
  
  useEffect(() => {
    if (colaboradores.length > 0) {
      if (hasMore) {
        // Se há mais, estimamos que há pelo menos mais uma página
        setCalculatedTotalItems((currentPage * itemsPerPage) + 1);
      } else {
        // Se não há mais, o total é exato
        setCalculatedTotalItems(((currentPage - 1) * itemsPerPage) + colaboradores.length);
      }
    } else if (currentPage === 1) {
      setCalculatedTotalItems(0);
    }
  }, [colaboradores, currentPage, itemsPerPage, hasMore]);

  const columns: Column[] = [
    { id: "nome", label: labelColaboradores, sortable: true },
    { id: "situacao", label: "Situação", sortable: true },
    { id: "dataSituacao", label: "Data situação", sortable: true },
    { id: "gestorAdm", label: "Gestor Adm", sortable: true },
    { id: "status", label: "Status", sortable: true },
    { id: "projeto", label: "Projeto", sortable: true },
    { id: "aprovador", label: "Aprovadores", sortable: true },
    { id: "horasTrabalhadas", label: "Horas Trabalhadas", sortable: true },
  ];

  const renderCell = (colaborador: typeof colaboradores[0], columnId: string) => {
    switch (columnId) {
      case "nome":
        const cpfColaborador = (colaborador as any).cpf;
        const handleNomeClick = () => {
          if (temApontamentoHorasColaborador && cpfColaborador) {
            navigate(`/timesheet?id=${cpfColaborador}&mes=${mesSelecionado}&ano=${anoSelecionado}&nome=${encodeURIComponent(colaborador.nome)}`);
          }
        };
        
        if (temApontamentoHorasColaborador && cpfColaborador) {
          return (
            <button
              onClick={handleNomeClick}
              className="font-medium text-primary underline hover:underline cursor-pointer text-left"
            >
              {colaborador.nome}
            </button>
          );
        }
        return <span className="font-medium text-primary underline hover:underline cursor-pointer">{colaborador.nome}</span>;
      case "situacao":
        const isInativo = colaborador.situacao === 'Inativo';
        return (
          <Badge className={isInativo ? "bg-red-100 text-red-700 hover:bg-red-100" : "bg-green-100 text-green-700 hover:bg-green-100"}>
            {colaborador.situacao}
          </Badge>
        );
      case "dataSituacao":
        return colaborador.dataSituacao;
      case "gestorAdm":
        return colaborador.gestorAdm;
      case "status":
        return <StatusBadge status={colaborador.status} variant="timesheet" />;
      case "projeto":
        return colaborador.projeto || '-';
      case "aprovador": {
        const aprovadores = colaborador.aprovadores || [];
        if (aprovadores.length === 0) {
          return <span className="text-muted-foreground">-</span>;
        }
        const primeiroAprovador = aprovadores[0];
        const restantes = aprovadores.length - 1;
        const textoExibicao = restantes > 0
          ? `${primeiroAprovador.nomeAprovador} + ${restantes}`
          : primeiroAprovador.nomeAprovador;
        const abrirModalAprovadores = () => {
          setAprovadoresModalList(aprovadores);
          setModalAprovadoresOpen(true);
        };
        if (aprovadores.length > 1) {
          return (
            <button
              type="button"
              onClick={abrirModalAprovadores}
              className="text-left font-medium text-primary hover:underline cursor-pointer inline-flex items-center gap-1"
            >
              {textoExibicao}
            </button>
          );
        }
        return <span className="text-sm">{primeiroAprovador.nomeAprovador}</span>;
      }
      case "horasTrabalhadas":
        return colaborador.horasTrabalhadas;
      default:
        return null;
    }
  };

  const handleNotificarAprovadores = async () => {
    if (!token) return;

    try {
      setEnviandoNotificacao(true);
      const api = container.resolve(TimesheetComponentesApi);
      
      await api.enviarEmailNotificacaoAprovadoresStatusPendentes(
        token,
        {
          nomeColaborador: debouncedSearchColaborador || undefined,
          codigoGerente: searchParams.codigoGerente || '0',
          codigoStatus: searchParams.codigoStatus || '0',
          mesVigencia: searchParams.mesVigencia,
          anoVigencia: searchParams.anoVigencia,
          codProjeto: searchParams.codProjeto || undefined,
          codColaboradorExternoAprovador: searchParams.codColaboradorExternoAprovador || undefined,
        }
      );

      setModalNotificarOpen(false);
      // Aqui você pode adicionar um toast de sucesso se quiser
    } catch (err) {
      console.error("Erro ao enviar notificação:", err);
      // Aqui você pode adicionar um toast de erro se quiser
    } finally {
      setEnviandoNotificacao(false);
    }
  };

  const handleBuscar = () => {
    // Resetar para primeira página ao buscar
    setCurrentPage(1);
    // Atualizar parâmetros de busca com os valores dos filtros
    setSearchParams({
      nomeColaborador: searchColaborador || undefined,
      codigoGerente: gestorAdm || '0',
      codigoStatus: status || '0',
      mesVigencia: parseInt(mesSelecionado),
      anoVigencia: parseInt(anoSelecionado),
      codProjeto: projeto || undefined,
      codColaboradorExternoAprovador: aprovador || undefined,
    });
    
    // Executar busca imediatamente após atualizar parâmetros
    if (refetch) {
      refetch();
    }
  };

  // Carregar gestores de apontamento
  useEffect(() => {
    const loadGestores = async () => {
      if (!token) {
        setLoadingGestores(false);
        return;
      }

      try {
        setLoadingGestores(true);
        const api = container.resolve(TimesheetComponentesApi);
        const response = await api.listarGestoresApontamento(token, 'pt-BR');
        setGestores(response.retorno || []);
      } catch (err) {
        console.error("Erro ao carregar gestores:", err);
        setGestores([]);
      } finally {
        setLoadingGestores(false);
      }
    };

    loadGestores();
  }, [token]);

  // Carregar projetos gerente de projeto
  useEffect(() => {
    const loadProjetos = async () => {
      if (!token) {
        setLoadingProjetos(false);
        return;
      }

      try {
        setLoadingProjetos(true);
        const api = container.resolve(TimesheetComponentesApi);
        const projetosData = await api.listarProjetosGerenteDeProjeto(token, 'pt-BR');
        setProjetos(projetosData || []);
      } catch (err) {
        console.error("Erro ao carregar projetos:", err);
        setProjetos([]);
      } finally {
        setLoadingProjetos(false);
      }
    };

    loadProjetos();
  }, [token]);

  // Carregar aprovadores (gestores de projeto)
  useEffect(() => {
    const loadAprovadores = async () => {
      if (!token) {
        setLoadingAprovadores(false);
        return;
      }

      try {
        setLoadingAprovadores(true);
        const api = container.resolve(TimesheetComponentesApi);
        const response = await api.listarGestoresProjeto(token, '', '', '', 'pt-BR');
        setAprovadores(response.retorno || []);
      } catch (err) {
        console.error("Erro ao carregar aprovadores:", err);
        setAprovadores([]);
      } finally {
        setLoadingAprovadores(false);
      }
    };

    loadAprovadores();
  }, [token]);

  // Carregar status
  useEffect(() => {
    const loadStatus = async () => {
      if (!token) {
        setLoadingStatus(false);
        return;
      }

      try {
        setLoadingStatus(true);
        const api = container.resolve(TimesheetComponentesApi);
        const statusData = await api.listarStatus(token, 'pt-BR');
        setStatusList(statusData || []);
      } catch (err) {
        console.error("Erro ao carregar status:", err);
        setStatusList([]);
      } finally {
        setLoadingStatus(false);
      }
    };

    loadStatus();
  }, [token]);

  const handleLimpar = () => {
    setGestorAdm("");
    setProjeto("");
    setAprovador("");
    setStatus("0");
    setSearchColaborador("");
    setCurrentPage(1);
    // Resetar para mês/ano atual
    const current = getCurrentMonthYear();
    setMesSelecionado(String(current.mes));
    setAnoSelecionado(String(current.ano));
    
    // Atualizar parâmetros de busca apenas com vigência (demais campos zerados)
    setSearchParams({
      nomeColaborador: '',
      codigoGerente: '0',
      codigoStatus: '0',
      mesVigencia: current.mes,
      anoVigencia: current.ano,
      codProjeto: '',
      codColaboradorExternoAprovador: '',
    });
    
    // Executar busca imediatamente após limpar
    if (refetch) {
      refetch();
    }
  };

  // Formatar a vigência para exibição (ex: "Dezembro/2025")
  const formatarVigencia = (mes: string, ano: string): string => {
    const meses = [
      "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
      "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    ];
    const mesIndex = parseInt(mes) - 1;
    return `${meses[mesIndex]}/${ano}`;
  };

  // Gerar lista de anos (últimos 5 anos até 5 anos no futuro)
  const gerarAnos = (): number[] => {
    const anos: number[] = [];
    const anoAtual = new Date().getFullYear();
    for (let i = anoAtual - 5; i <= anoAtual + 5; i++) {
      anos.push(i);
    }
    return anos;
  };

  return (
    <div className="space-y-6">
      {/* Filtros Superiores */}
      <Card>
        <CardContent className="p-4 sm:p-6">
          <div className="flex flex-wrap gap-3 sm:gap-4 items-end">
            <div className="space-y-2 w-full sm:min-w-[180px] sm:w-auto">
              <label className="text-sm font-medium">Vigência</label>
              <Popover open={popoverOpen} onOpenChange={setPopoverOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal border-muted-foreground/30",
                      !mesSelecionado && !anoSelecionado && "text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {mesSelecionado && anoSelecionado
                      ? formatarVigencia(mesSelecionado, anoSelecionado)
                      : "Selecione"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-4" align="start">
                  <div className="flex gap-3">
                    <div className="space-y-2">
                      <label className="text-sm font-medium">Mês</label>
                      <Select value={mesSelecionado} onValueChange={setMesSelecionado}>
                        <SelectTrigger className="w-[140px]">
                          <SelectValue placeholder="Mês" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="1">Janeiro</SelectItem>
                          <SelectItem value="2">Fevereiro</SelectItem>
                          <SelectItem value="3">Março</SelectItem>
                          <SelectItem value="4">Abril</SelectItem>
                          <SelectItem value="5">Maio</SelectItem>
                          <SelectItem value="6">Junho</SelectItem>
                          <SelectItem value="7">Julho</SelectItem>
                          <SelectItem value="8">Agosto</SelectItem>
                          <SelectItem value="9">Setembro</SelectItem>
                          <SelectItem value="10">Outubro</SelectItem>
                          <SelectItem value="11">Novembro</SelectItem>
                          <SelectItem value="12">Dezembro</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="space-y-2">
                      <label className="text-sm font-medium">Ano</label>
                      <Select value={anoSelecionado} onValueChange={setAnoSelecionado}>
                        <SelectTrigger className="w-[100px]">
                          <SelectValue placeholder="Ano" />
                        </SelectTrigger>
                        <SelectContent>
                          {gerarAnos().map((ano) => (
                            <SelectItem key={ano} value={String(ano)}>
                              {ano}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  </div>
                  <div className="flex justify-end mt-4">
                    <Button
                      size="sm"
                      onClick={() => setPopoverOpen(false)}
                      className="px-4"
                    >
                      Confirmar
                    </Button>
                  </div>
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
                    disabled={loadingGestores}
                  >
                    {gestorAdm
                      ? gestores.find((g) => g.codigoGerente === gestorAdm)
                        ? `${gestores.find((g) => g.codigoGerente === gestorAdm)!.codigoGerente} - ${gestores.find((g) => g.codigoGerente === gestorAdm)!.nomeCompleto}`
                        : "Todos"
                      : loadingGestores
                      ? "Carregando..."
                      : "Todos"}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                  <Command>
                    <CommandInput placeholder="Buscar gestor..." />
                    <CommandList>
                      <CommandEmpty>
                        {loadingGestores ? "Carregando..." : "Nenhum gestor encontrado."}
                      </CommandEmpty>
                      <CommandGroup>
                        <CommandItem
                          value="Todos"
                          onSelect={() => {
                            setGestorAdm("");
                            setGestorAdmOpen(false);
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              !gestorAdm ? "opacity-100" : "opacity-0"
                            )}
                          />
                          Todos
                        </CommandItem>
                        {gestores.map((gestor) => {
                          const value = gestor.codigoGerente;
                          const label = `${gestor.codigoGerente} - ${gestor.nomeCompleto}`;
                          return (
                            <CommandItem
                              key={value}
                              value={label}
                              onSelect={() => {
                                setGestorAdm(value === gestorAdm ? "" : value);
                                setGestorAdmOpen(false);
                              }}
                            >
                              <Check
                                className={cn(
                                  "mr-2 h-4 w-4",
                                  gestorAdm === value ? "opacity-100" : "opacity-0"
                                )}
                              />
                              {label}
                            </CommandItem>
                          );
                        })}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2 flex-1 min-w-[200px]">
              <label className="text-sm font-medium">Projeto</label>
              <Popover open={projetoOpen} onOpenChange={setProjetoOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={projetoOpen}
                    className="w-full justify-between border-muted-foreground/30"
                    disabled={loadingProjetos}
                  >
                    {projeto
                      ? projetos.find((p) => p.codProjeto === projeto)
                        ? `${projetos.find((p) => p.codProjeto === projeto)!.codProjeto} - ${projetos.find((p) => p.codProjeto === projeto)!.nomeProjeto}`
                        : "Todos"
                      : loadingProjetos
                      ? "Carregando..."
                      : "Todos"}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                  <Command>
                    <CommandInput placeholder="Buscar projeto..." />
                    <CommandList>
                      <CommandEmpty>
                        {loadingProjetos ? "Carregando..." : "Nenhum projeto encontrado."}
                      </CommandEmpty>
                      <CommandGroup>
                        <CommandItem
                          value="Todos"
                          onSelect={() => {
                            setProjeto("");
                            setProjetoOpen(false);
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              !projeto ? "opacity-100" : "opacity-0"
                            )}
                          />
                          Todos
                        </CommandItem>
                        {projetos.map((proj) => {
                          const value = proj.codProjeto;
                          const label = `${proj.codProjeto} - ${proj.nomeProjeto}`;
                          return (
                            <CommandItem
                              key={value}
                              value={label}
                              onSelect={() => {
                                setProjeto(value === projeto ? "" : value);
                                setProjetoOpen(false);
                              }}
                            >
                              <Check
                                className={cn(
                                  "mr-2 h-4 w-4",
                                  projeto === value ? "opacity-100" : "opacity-0"
                                )}
                              />
                              {label}
                            </CommandItem>
                          );
                        })}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2 flex-1 min-w-[200px]">
              <label className="text-sm font-medium">Aprovador</label>
              <Popover open={aprovadorOpen} onOpenChange={setAprovadorOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={aprovadorOpen}
                    className="w-full justify-between border-muted-foreground/30"
                    disabled={loadingAprovadores}
                  >
                    {aprovador
                      ? aprovadores.find((a) => a.codGerente === aprovador)
                        ? aprovadores.find((a) => a.codGerente === aprovador)!.labelGerenteProjeto
                        : "Todos"
                      : loadingAprovadores
                      ? "Carregando..."
                      : "Todos"}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                  <Command>
                    <CommandInput placeholder="Buscar aprovador..." />
                    <CommandList>
                      <CommandEmpty>
                        {loadingAprovadores ? "Carregando..." : "Nenhum aprovador encontrado."}
                      </CommandEmpty>
                      <CommandGroup>
                        <CommandItem
                          value="Todos"
                          onSelect={() => {
                            setAprovador("");
                            setAprovadorOpen(false);
                          }}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              !aprovador ? "opacity-100" : "opacity-0"
                            )}
                          />
                          Todos
                        </CommandItem>
                        {aprovadores.map((aprov) => {
                          const value = aprov.codGerente;
                          const label = aprov.labelGerenteProjeto;
                          return (
                            <CommandItem
                              key={value}
                              value={label}
                              onSelect={() => {
                                setAprovador(value === aprovador ? "" : value);
                                setAprovadorOpen(false);
                              }}
                            >
                              <Check
                                className={cn(
                                  "mr-2 h-4 w-4",
                                  aprovador === value ? "opacity-100" : "opacity-0"
                                )}
                              />
                              {label}
                            </CommandItem>
                          );
                        })}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <div className="space-y-2 w-full sm:min-w-[180px] sm:w-auto">
              <label className="text-sm font-medium">Status</label>
              <Select value={status} onValueChange={setStatus} disabled={loadingStatus}>
                <SelectTrigger className="border-muted-foreground/30">
                  <SelectValue placeholder={loadingStatus ? "Carregando..." : "Todos"} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="0">Todos</SelectItem>
                  {statusList.map((statusItem) => (
                    <SelectItem key={statusItem.id} value={String(statusItem.codStatusGrupo)}>
                      {statusItem.descricao}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
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

      {/* Indicadores Resumidos */}
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

      {/* Lista de Colaboradores */}
      <div className="space-y-4">
        <div>
          <h2 className="text-2xl font-semibold mb-2">{labelColaboradores}</h2>
          <p className="text-sm text-muted-foreground">
            Veja abaixo a lista de {labelColaboradores.toLowerCase()}, utilize o filtro para encontrar um {labelColaborador}
          </p>
        </div>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between mb-6">
              <div className="relative flex-1 max-w-md">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder={`Buscar ${labelColaboradores}`}
                  value={searchColaborador}
                  onChange={(e) => setSearchColaborador(e.target.value)}
                  className="pl-10"
                />
              </div>

              <Button 
                variant="outline" 
                className="ml-4 bg-white text-foreground border-foreground hover:bg-muted"
                onClick={() => setModalNotificarOpen(true)}
              >
                <Mail className="h-4 w-4 mr-2" />
                Notificar Aprovadores
              </Button>
            </div>

            {loading ? (
              <div className="space-y-3">
                {[...Array(5)].map((_, index) => (
                  <div key={index} className="flex items-center gap-4 p-4 border-b">
                    <Skeleton className="h-4 w-[200px]" />
                    <Skeleton className="h-4 w-[100px]" />
                    <Skeleton className="h-4 w-[120px]" />
                    <Skeleton className="h-4 w-[150px]" />
                    <Skeleton className="h-4 w-[100px]" />
                  </div>
                ))}
              </div>
            ) : (
              <>
                <DataTable
                  columns={columns}
                  data={colaboradores}
                  renderCell={renderCell}
                  keyExtractor={(item) => (item as any).id || (item as any).cpf || item.nome}
                />
                {/* Totais do totalizador (quantidadeTotalColaboradores e somaHorasLancadas) */}
                {!loading && stats.length > 0 && (
                  <div className="flex flex-wrap items-center gap-6 py-4 px-1 border-t bg-muted/30 text-sm">
                    <span className="font-medium">
                      Contagem {labelColaboradores}:{" "}
                      <span className="text-foreground">
                        {stats.find((s) => s.label === "Colaboradores")?.value ?? "0"}
                      </span>
                    </span>
                    <span className="font-medium">
                      Soma de horas:{" "}
                      <span className="text-foreground">
                        {stats.find((s) => s.label === "Horas lançadas")?.value ?? "00:00"}
                      </span>
                    </span>
                  </div>
                )}
                {(calculatedTotalItems > 0 || totalItems > 0) && (
                  <TablePagination
                    currentPage={currentPage}
                    totalItems={calculatedTotalItems || totalItems}
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
      </div>

      {/* Modal de Confirmação - Notificar Aprovadores */}
      <AlertDialog open={modalNotificarOpen} onOpenChange={(open) => {
        if (!enviandoNotificacao) {
          setModalNotificarOpen(open);
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <div className="flex items-center gap-3">
              <Mail className="h-5 w-5 text-blue-600" />
              <AlertDialogTitle>Notificar Aprovadores</AlertDialogTitle>
            </div>
            <AlertDialogDescription>
              Ao confirmar, todos os aprovadores receberão uma notificação por e-mail com os lançamentos pendentes de aprovação.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={enviandoNotificacao}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleNotificarAprovadores}
              disabled={enviandoNotificacao}
            >
              {enviandoNotificacao ? "Enviando..." : "Confirmar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal: lista de aprovadores (coluna Aprovadores do grid) */}
      <Dialog open={modalAprovadoresOpen} onOpenChange={setModalAprovadoresOpen}>
        <DialogContent className="max-w-sm">
          <DialogHeader>
            <DialogTitle>Aprovadores</DialogTitle>
          </DialogHeader>
          <ul className="list-disc list-inside space-y-1 py-2">
            {aprovadoresModalList.map((aprov, idx) => (
              <li key={idx} className="text-sm">
                {aprov.nomeAprovador}
              </li>
            ))}
          </ul>
          <DialogFooter>
            <Button variant="outline" onClick={() => setModalAprovadoresOpen(false)}>
              Fechar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};
