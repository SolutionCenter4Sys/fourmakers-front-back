import { useState, useCallback, useEffect, useMemo } from "react";
import { useSearchParams } from "react-router-dom";
import { Search, X, SlidersHorizontal, ChevronUp, ChevronDown } from "@/components/ui/system-icons";
import { Icon } from "@/components/ui/icon";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { DataTable, TablePagination } from "@presentation/components/common";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import type { Column } from "@/hooks/useColumnReorder";
import { useAppSelector } from "@/app/store/hooks";
import { cn } from "@/lib/utils";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetFooter,
} from "@/components/ui/sheet";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { container } from "@/core/di/container";
import { GetMapaAlocacaoResumoUseCase } from "@domain/usecases/GetMapaAlocacaoResumoUseCase";
import { GetMapaAlocacaoRecursoUseCase } from "@domain/usecases/GetMapaAlocacaoRecursoUseCase";
import { ListarNomesGestoresUseCase } from "@domain/usecases/ListarNomesGestoresUseCase";
import { ListarColaboradoresETbdsUseCase } from "@domain/usecases/ListarColaboradoresETbdsUseCase";
import { ListarDiretoriasUseCase } from "@domain/usecases/ListarDiretoriasUseCase";
import type { Cliente, StatusProjeto } from "@data/api/ProjetosApi";
import type { GetMapaAlocacaoResumoResponse, AlocacaoRecurso, MesResumo } from "@domain/entities/MapaAlocacao";
import type { DiretoriaColaborador } from "@domain/entities/Diretoria";
import type { ColaboradorETbd } from "@domain/entities/MapaAlocacao";
import { toast } from "sonner";
import { ProjetosTabContent } from "./ProjetosTabContent";
import { Skeleton } from "@/components/ui/skeleton";

// Tipo para gestor (a API retorna com codigoProfissional e nome)
interface GestorFiltro {
  codigoProfissional: string;
  nome: string;
}

// Enum para Disponibilidade de horário
enum StatusHorasFiltro {
  Todos = 0,
  HorasPendentes = 1,
  TodasHorasAlocadas = 2,
  HorasAlocadasAMais = 3,
}

const STATUS_HORAS_OPTIONS = [
  { value: StatusHorasFiltro.Todos, label: "Todos" },
  { value: StatusHorasFiltro.HorasPendentes, label: "Horas pendentes" },
  { value: StatusHorasFiltro.TodasHorasAlocadas, label: "Todas as horas alocadas" },
  { value: StatusHorasFiltro.HorasAlocadasAMais, label: "Horas alocadas a mais" },
];

export const VisaoGerencialTab = () => {
  const { token } = useAppSelector((state) => state.auth);
  const [searchParams, setSearchParams] = useSearchParams();
  
  // Estado para alternar entre Alocação e Projetos - inicializar da URL
  const [subTabAtiva, setSubTabAtiva] = useState<"alocacao" | "projetos">(() => {
    const viewParam = searchParams.get('view');
    if (viewParam === 'projetos') {
      return 'projetos';
    }
    return 'alocacao';
  });
  
  // Estados principais
  const [trimestral, setTrimestral] = useState(true);
  const [resumo, setResumo] = useState<GetMapaAlocacaoResumoResponse | null>(null);
  const [recursos, setRecursos] = useState<AlocacaoRecurso[]>([]);
  const [loading, setLoading] = useState(false);
  const [loadingRecursos, setLoadingRecursos] = useState(false);
  const [busca, setBusca] = useState("");
  const [filtersOpen, setFiltersOpen] = useState(false);
  
  // Estados para aba Projetos
  
  
  const [buscaProjetos, setBuscaProjetos] = useState("");
  const [filtersProjetosOpen, setFiltersProjetosOpen] = useState(false);
  const [currentPageProjetos, setCurrentPageProjetos] = useState(1);
  const [itemsPerPageProjetos, setItemsPerPageProjetos] = useState(20);
  
  
  // Estados de filtros - Alocação
  const [filtros, setFiltros] = useState({
    codigoDiretoria: "",
    gestorFiltro: "",
    colaboradorOuTbdFiltro: "",
    statusHorasFiltro: StatusHorasFiltro.Todos,
  });
  
  // Estados de filtros - Projetos
  const [filtrosProjetos, setFiltrosProjetos] = useState({
    cdProjeto: "",
    disponibilidadeHorario: 0,
    cdStatusProjeto: 0,
    nomeProjeto: "",
    codDiretoria: 0,
    cliente: "",
    gestorProjeto: "",
    cdCliente: "",
  });
  
  // Estados para listas de filtros - Alocação
  const [diretorias, setDiretorias] = useState<DiretoriaColaborador[]>([]);
  const [gestores, setGestores] = useState<GestorFiltro[]>([]);
  const [colaboradores, setColaboradores] = useState<ColaboradorETbd[]>([]);
  const [loadingFiltros, setLoadingFiltros] = useState(false);
  const [filtrosExpandidos, setFiltrosExpandidos] = useState({
    unidade: true,
    gestor: false,
    colaboradorTbd: false,
    disponibilidade: false,
  });
  
  // Estados para listas de filtros - Projetos
  const [clientesProjetos, setClientesProjetos] = useState<Cliente[]>([]);
  const [statusProjetos, setStatusProjetos] = useState<StatusProjeto[]>([]);
  const [gestoresProjetos, setGestoresProjetos] = useState<GestorFiltro[]>([]);
  const [loadingFiltrosProjetos, setLoadingFiltrosProjetos] = useState(false);
  const [filtrosProjetosExpandidos, setFiltrosProjetosExpandidos] = useState({
    projeto: true,
    cliente: false,
    status: false,
    disponibilidade: false,
    unidade: false,
    gestor: false,
  });
  
  // Paginação
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(20);
  const [hasMore, setHasMore] = useState(true);
  const [totalItems, setTotalItems] = useState(0);

  // Obter ano e mês atual
  const anoAtual = new Date().getFullYear();
  const mesInicial = 1;

  // Carregar resumo
  const carregarResumo = useCallback(async () => {
    if (!token) return;

    try {
      setLoading(true);
      const useCase = container.resolve(GetMapaAlocacaoResumoUseCase);
      const response = await useCase.execute(token, {
        trimestral,
        mesInicial,
        anoInicial: anoAtual,
      });
      setResumo(response);
    } catch (error) {
      console.error("Erro ao carregar resumo:", error);
      toast.error("Erro ao carregar resumo de alocação");
    } finally {
      setLoading(false);
    }
  }, [token, trimestral, anoAtual]);

  // Carregar recursos
  const carregarRecursos = useCallback(async () => {
    if (!token) return;

    try {
      setLoadingRecursos(true);
      const cursor = (currentPage - 1) * itemsPerPage;
      const useCase = container.resolve(GetMapaAlocacaoRecursoUseCase);
      const response = await useCase.execute(token, {
        cursor,
        limite: itemsPerPage,
        trimestral,
        hardskill: "",
        idioma: "",
        statusHorasFiltro: filtros.statusHorasFiltro,
        gestorFiltro: filtros.gestorFiltro || undefined,
        codigoDiretoria: filtros.codigoDiretoria || undefined,
        colaboradorOuTbdFiltro: filtros.colaboradorOuTbdFiltro || undefined,
      });
      // A estrutura da resposta é retorno.recurso
      const recursosData = Array.isArray(response?.retorno?.recurso) 
        ? response.retorno.recurso 
        : [];
      setRecursos(recursosData);
      // Verifica se há mais páginas baseado na quantidade retornada
      setHasMore(recursosData.length === itemsPerPage);
    } catch (error) {
      console.error("Erro ao carregar recursos:", error);
      toast.error("Erro ao carregar recursos");
      setRecursos([]);
      setHasMore(false);
    } finally {
      setLoadingRecursos(false);
    }
  }, [token, trimestral, currentPage, itemsPerPage, filtros]);

  // Carregar diretorias
  const carregarDiretorias = useCallback(async () => {
    if (!token) return;

    try {
      setLoadingFiltros(true);
      const useCase = container.resolve(ListarDiretoriasUseCase);
      const response = await useCase.execute(token);
      const diretoriasData = response.diretoriaColaborador || [];
      const seen = new Set<string>();
      const diretoriasUnicas = diretoriasData.filter((diretoria) => {
        if (seen.has(diretoria.cod)) return false;
        seen.add(diretoria.cod);
        return true;
      });
      setDiretorias(diretoriasUnicas);
    } catch (error) {
      console.error("Erro ao carregar diretorias:", error);
    } finally {
      setLoadingFiltros(false);
    }
  }, [token]);

  // Carregar gestores
  const carregarGestores = useCallback(async () => {
    if (!token) return;

    try {
      const useCase = container.resolve(ListarNomesGestoresUseCase);
      const response = await useCase.execute(token, {
        codDiretoria: filtros.codigoDiretoria ? Number(filtros.codigoDiretoria) : undefined,
      });
      // A API pode retornar response.retorno ou response.Gestor dependendo da implementação
      const gestoresData = (response as any).Gestor || response.retorno || [];
      const seen = new Set<string>();
      const gestoresUnicos = gestoresData.filter((gestor: any) => {
        const codigo = gestor.codigoProfissional || gestor.codigoGestor;
        if (seen.has(codigo)) return false;
        seen.add(codigo);
        return true;
      }).map((gestor: any) => ({
        codigoProfissional: gestor.codigoProfissional || gestor.codigoGestor,
        nome: gestor.nome || gestor.nomeGestor,
      }));
      setGestores(gestoresUnicos);
    } catch (error) {
      console.error("Erro ao carregar gestores:", error);
    }
  }, [token, filtros.codigoDiretoria]);

  // Carregar colaboradores
  const carregarColaboradores = useCallback(async () => {
    if (!token) return;

    try {
      const useCase = container.resolve(ListarColaboradoresETbdsUseCase);
      const response = await useCase.execute(token, {
        codigoDiretoria: filtros.codigoDiretoria ? Number(filtros.codigoDiretoria) : undefined,
      });
      setColaboradores(response.retorno || []);
    } catch (error) {
      console.error("Erro ao carregar colaboradores:", error);
    }
  }, [token, filtros.codigoDiretoria]);

  // Efeitos
  useEffect(() => {
    if (token) {
      carregarResumo();
    }
  }, [token, carregarResumo]);

  useEffect(() => {
    if (token) {
      carregarRecursos();
    }
  }, [token, carregarRecursos]);

  // Recarregar recursos quando trimestral mudar
  useEffect(() => {
    if (token) {
      carregarRecursos();
    }
  }, [trimestral, token, carregarRecursos]);

  useEffect(() => {
    carregarDiretorias();
  }, [carregarDiretorias]);

  useEffect(() => {
    if (filtros.codigoDiretoria) {
      carregarGestores();
      carregarColaboradores();
    }
  }, [filtros.codigoDiretoria, carregarGestores, carregarColaboradores]);

  // Sincronizar sub-aba com parâmetro view da URL
  useEffect(() => {
    const viewParam = searchParams.get('view');
    const tabParam = searchParams.get('tab');
    
    // Só sincronizar se estiver na aba visao-gerencial
    if (tabParam === 'visaogerencial') {
      if (viewParam === 'projetos' && subTabAtiva !== 'projetos') {
        setSubTabAtiva('projetos');
      } else if (viewParam !== 'projetos' && subTabAtiva === 'projetos' && !viewParam) {
        // Se o parâmetro view foi removido e estamos em projetos, voltar para alocação
        setSubTabAtiva('alocacao');
      }
    }
  }, [searchParams, subTabAtiva]);

  // Aplicar filtros
  const handleAplicarFiltros = useCallback(() => {
    setCurrentPage(1);
    if (token) {
      carregarRecursos();
    }
    setFiltersOpen(false);
  }, [token, carregarRecursos]);

  // Limpar filtros e busca
  const handleLimparFiltros = useCallback(() => {
    setFiltros({
      codigoDiretoria: "",
      gestorFiltro: "",
      colaboradorOuTbdFiltro: "",
      statusHorasFiltro: StatusHorasFiltro.Todos,
    });
    setBusca("");
    setCurrentPage(1);
    if (token) {
      carregarRecursos();
    }
  }, [token, carregarRecursos]);

  // Verificar se há filtros aplicados
  const temFiltrosAplicados = useMemo(() => {
    return !!(
      filtros.codigoDiretoria ||
      filtros.gestorFiltro ||
      filtros.colaboradorOuTbdFiltro ||
      filtros.statusHorasFiltro !== StatusHorasFiltro.Todos
    );
  }, [filtros]);

  // Contar quantidade de filtros aplicados
  const quantidadeFiltrosAplicados = useMemo(() => {
    let count = 0;
    if (filtros.codigoDiretoria) count++;
    if (filtros.gestorFiltro) count++;
    if (filtros.colaboradorOuTbdFiltro) count++;
    if (filtros.statusHorasFiltro !== StatusHorasFiltro.Todos) count++;
    return count;
  }, [filtros]);

  // Atualiza o total de itens estimado baseado na página atual e se há mais
  useEffect(() => {
    if (recursos.length > 0) {
      if (hasMore) {
        // Se há mais, estimamos que há pelo menos mais uma página
        setTotalItems((currentPage * itemsPerPage) + 1);
      } else {
        // Se não há mais, o total é exato
        setTotalItems(((currentPage - 1) * itemsPerPage) + recursos.length);
      }
    } else if (currentPage === 1) {
      setTotalItems(0);
    }
  }, [recursos, currentPage, itemsPerPage, hasMore]);

  // Preparar colunas dinâmicas baseadas nos meses do resumo ou dos recursos
  const meses = resumo?.retorno?.resumo?.meses || [];
  
  // Extrair meses únicos dos recursos se não houver no resumo
  const mesesDosRecursos = useMemo(() => {
    if (meses.length > 0) {
      // Normalizar formato: "jan./2026" -> "jan/2026"
      return meses.map((mes) => ({
        ...mes,
        nomeMesAtual: mes.nomeMesAtual.replace(/\./g, ''),
      }));
    }
    if (recursos.length > 0 && recursos[0].alocacao) {
      // Extrair meses únicos dos recursos
      const mesesUnicos = new Set<string>();
      recursos.forEach((recurso) => {
        recurso.alocacao?.forEach((aloc) => {
          mesesUnicos.add(aloc.mes);
        });
      });
      return Array.from(mesesUnicos).sort().map((mes) => ({
        nomeMesAtual: mes,
        ociosidadeMes: "0:00",
        forca: 0,
      }));
    }
    return [];
  }, [meses, recursos]);

  const columns: Column[] = useMemo(() => {
    const baseColumns: Column[] = [
      { id: "colaborador", label: "Colaborador(a)", sortable: true, width: "w-[250px]" },
    ];
    
    // Adicionar colunas de meses
    mesesDosRecursos.forEach((mes) => {
      const mesId = mes.nomeMesAtual;
      baseColumns.push({
        id: mesId,
        label: mesId,
        sortable: false,
        width: "w-[150px]",
      });
    });
    
    return baseColumns;
  }, [mesesDosRecursos]);

  // Renderizar célula
  const renderCell = (item: AlocacaoRecurso, columnId: string) => {
    if (columnId === "colaborador") {
      return (
        <div className="flex items-center gap-2">
          {item.ehTbd && (
            <Badge variant="outline" className="text-xs">
              TBD
            </Badge>
          )}
          <span>{item.nome}</span>
        </div>
      );
    }
    
    // Colunas de meses - buscar no array alocacao
    // Normalizar formato do mês para comparação (remover pontos)
    const mesNormalizado = columnId.replace(/\./g, '');
    const alocacaoMes = item.alocacao?.find((aloc) => {
      const alocMesNormalizado = aloc.mes.replace(/\./g, '');
      return alocMesNormalizado === mesNormalizado;
    });
    
    if (alocacaoMes) {
      const horas = alocacaoMes.horas;
      // Determinar cor baseada no statusHorasRecursoFiltro
      // 1 = Horas pendentes (amarelo), 2 = Todas as horas alocadas (verde), 3 = Horas alocadas a mais (vermelho)
      let bgColor = "bg-yellow-50 dark:bg-yellow-950/20 border border-yellow-200 dark:border-yellow-800";
      if (alocacaoMes.statusHorasRecursoFiltro === 2) {
        bgColor = "bg-green-50 dark:bg-green-950/20 border border-green-200 dark:border-green-800";
      } else if (alocacaoMes.statusHorasRecursoFiltro === 3) {
        bgColor = "bg-red-50 dark:bg-red-950/20 border border-red-200 dark:border-red-800";
      }
      
      return (
        <div className={cn("px-2 py-1 rounded text-center font-medium", bgColor)}>
          {horas}
        </div>
      );
    }
    
    return "-";
  };

  // Calcular percentual de ociosidade
  const ociosidadePercentual = resumo?.retorno?.resumo?.ociosidadeResumo
    ? (resumo.retorno.resumo.ociosidadeResumo * 100).toFixed(2)
    : "0";

  return (
    <div className="space-y-6">
      {/* Alternador Alocação / Projetos */}
      <Tabs 
        value={subTabAtiva} 
        onValueChange={(value) => {
          const newValue = value as "alocacao" | "projetos";
          setSubTabAtiva(newValue);
          
          // Atualizar parâmetro view na URL
          const newParams = new URLSearchParams(searchParams);
          if (newValue === 'projetos') {
            newParams.set('view', 'projetos');
          } else {
            newParams.delete('view');
          }
          setSearchParams(newParams);
        }}
      >
        <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm">
          <TabsTrigger 
            value="alocacao" 
            className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
          >
            <span>Alocação</span>
          </TabsTrigger>
          <TabsTrigger 
            value="projetos" 
            className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
          >
            <span>Projetos</span>
          </TabsTrigger>
        </TabsList>
      </Tabs>

      {/* Conteúdo da sub-aba Alocação */}
      {subTabAtiva === "alocacao" && (
        <>
          {/* Sumário de Alocação */}
          <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="text-[20px] font-semibold">Sumário de Alocação</CardTitle>
            {/* Toggle Trimestral/Anual */}
            <div className="flex items-center gap-2 bg-muted rounded-lg p-1">
              <Button
                variant={trimestral ? "primary" : "ghost"}
                size="sm"
                onClick={() => {
                  setTrimestral(true);
                  setCurrentPage(1);
                }}
                className={cn(
                  "px-4",
                  trimestral && "bg-primary text-primary-foreground"
                )}
              >
                Trimestral
              </Button>
              <Button
                variant={!trimestral ? "primary" : "ghost"}
                size="sm"
                onClick={() => {
                  setTrimestral(false);
                  setCurrentPage(1);
                }}
                className={cn(
                  "px-4",
                  !trimestral && "bg-primary text-primary-foreground"
                )}
              >
                Anual
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-6">
          {loading ? (
            <div className="space-y-6">
              {/* Skeleton Ociosidade */}
              <div className="space-y-2">
                <Skeleton className="h-5 w-32" />
                <Skeleton className="h-12 w-full" />
              </div>
              {/* Skeleton Força */}
              <div className="space-y-2">
                <Skeleton className="h-5 w-24" />
                <Skeleton className="h-12 w-full" />
              </div>
              {/* Skeleton Meses */}
              <div className="space-y-2">
                <Skeleton className="h-5 w-40" />
                <div className="grid grid-cols-3 gap-4">
                  {[...Array(6)].map((_, i) => (
                    <Skeleton key={i} className="h-20 w-full" />
                  ))}
                </div>
              </div>
            </div>
          ) : resumo?.retorno?.resumo ? (
            <>
              {/* Ociosidade Anual */}
              <div className="space-y-2">
                <div className="flex items-center justify-between text-sm">
                  <span className="font-medium">Ociosidade Anual</span>
                  <span className="font-semibold">{ociosidadePercentual}%</span>
                </div>
                <div className="w-full h-4 bg-muted/50 rounded-full overflow-hidden border border-border/50">
                  <div
                    className="h-full bg-foreground transition-all duration-500 rounded-full"
                    style={{ width: `${Math.min(Number(ociosidadePercentual), 100)}%` }}
                  />
                </div>
              </div>

              {/* Breakdown Mensal - Tabela com labels fixas e dados scrolláveis */}
              {meses.length > 0 && (
                <div className="border rounded-lg overflow-hidden bg-background">
                  <div className="relative overflow-x-auto">
                    <table className="w-full border-collapse min-w-full">
                      <tbody>
                        {/* Linha: Mês */}
                        <tr className="border-b">
                          <td className="sticky left-0 z-10 bg-background border-r px-4 py-3 text-sm font-medium whitespace-nowrap">
                            Mês
                          </td>
                          {meses.map((mes: MesResumo) => (
                            <td key={mes.nomeMesAtual} className="px-4 py-3 text-sm text-center min-w-[140px]">
                              {mes.nomeMesAtual}
                            </td>
                          ))}
                        </tr>
                        
                        {/* Linha: Ociosidade mensal Total */}
                        <tr className="border-b">
                          <td className="sticky left-0 z-10 bg-background border-r px-4 py-3 text-sm font-medium whitespace-nowrap">
                            Ociosidade mensal Total
                          </td>
                          {meses.map((mes: MesResumo) => (
                            <td key={`ociosidade-${mes.nomeMesAtual}`} className="px-4 py-3 text-sm font-semibold text-center min-w-[140px]">
                              {mes.ociosidadeMes}
                            </td>
                          ))}
                        </tr>
                        
                        {/* Linha: Força Mensal Utilizada */}
                        <tr>
                          <td className="sticky left-0 z-10 bg-background border-r px-4 py-3 text-sm font-medium whitespace-nowrap">
                            Força Mensal Utilizada
                          </td>
                          {meses.map((mes: MesResumo) => (
                            <td key={`forca-${mes.nomeMesAtual}`} className="px-4 py-3 min-w-[140px]">
                              <div className="space-y-2">
                                <div className="text-sm font-semibold text-center">{(mes.forca * 100).toFixed(2)}%</div>
                                <div className="w-full h-3 bg-muted/50 rounded-full overflow-hidden border border-border/50">
                                  <div
                                    className="h-full bg-primary transition-all duration-500 rounded-full"
                                    style={{ width: `${Math.min(mes.forca * 100, 100)}%` }}
                                  />
                                </div>
                              </div>
                            </td>
                          ))}
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>
              )}
            </>
          ) : (
            <div className="text-center py-8 text-muted-foreground">
              Nenhum dado disponível
            </div>
          )}
        </CardContent>
      </Card>

      {/* Tabela de Alocações */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="text-[20px] font-semibold">Alocações</CardTitle>
            <div className="flex items-center gap-2">
              {/* Campo de busca */}
              <div className="relative w-80">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Buscar..."
                  value={busca}
                  onChange={(e) => setBusca(e.target.value)}
                  className="pl-10"
                />
              </div>
              
              {/* Botão Filtros */}
              <Button
                variant="outline"
                className="gap-2"
                onClick={() => setFiltersOpen(true)}
              >
                <SlidersHorizontal className={cn(
                  "h-4 w-4 transition-colors",
                  temFiltrosAplicados && "text-primary"
                )} />
                <span>Filtros</span>
                {temFiltrosAplicados && (
                  <Badge 
                    variant="default" 
                    className="ml-0.5 h-5 min-w-[20px] px-1.5 flex items-center justify-center text-[10px] font-semibold leading-none"
                  >
                    {quantidadeFiltrosAplicados}
                  </Badge>
                )}
              </Button>
              
              {/* Botão Limpar */}
              <Button 
                variant="outline" 
                className="gap-2"
                onClick={handleLimparFiltros}
                disabled={!temFiltrosAplicados && !busca.trim() && recursos.length === 0}
              >
                <X className="h-4 w-4" />
                <span>Limpar</span>
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {loadingRecursos ? (
            <div className="space-y-4">
              {/* Skeleton das legendas */}
              <div className="flex items-center gap-6 pb-4 border-b">
                <Skeleton className="h-4 w-32" />
                <Skeleton className="h-4 w-28" />
                <Skeleton className="h-4 w-36" />
              </div>
              {/* Skeleton do cabeçalho da tabela */}
              <div className="flex items-center gap-4 border-b pb-4">
                <Skeleton className="h-10 w-48" />
                <Skeleton className="h-10 w-32" />
                <Skeleton className="h-10 w-32" />
                <Skeleton className="h-10 w-32" />
                <Skeleton className="h-10 w-32" />
              </div>
              {/* Skeleton das linhas */}
              {[...Array(8)].map((_, index) => (
                <div key={index} className="flex items-center gap-4 py-3 border-b">
                  <Skeleton className="h-10 w-48" />
                  <Skeleton className="h-10 w-32" />
                  <Skeleton className="h-10 w-32" />
                  <Skeleton className="h-10 w-32" />
                  <Skeleton className="h-10 w-32" />
                </div>
              ))}
            </div>
          ) : (
            <>
              {/* Legendas de Cores */}
              <div className="flex items-center gap-6 text-sm mb-4 pb-4 border-b">
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 rounded-full bg-green-100 dark:bg-green-900/50 border border-green-200 dark:border-green-800"></div>
                  <span className="text-muted-foreground">Todas as horas alocadas</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 rounded-full bg-yellow-100 dark:bg-yellow-900/50 border border-yellow-200 dark:border-yellow-800"></div>
                  <span className="text-muted-foreground">Horas pendentes</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 rounded-full bg-red-100 dark:bg-red-900/50 border border-red-200 dark:border-red-800"></div>
                  <span className="text-muted-foreground">Horas alocadas a mais</span>
                </div>
              </div>
              
              <DataTable
                columns={columns}
                data={Array.isArray(recursos) ? recursos.filter((item) =>
                  busca
                    ? item.nome
                        .toLowerCase()
                        .includes(busca.toLowerCase())
                    : true
                ) : []}
                keyExtractor={(item) => item.nome + (item.codigoColaborador || item.codigoTbd || '')}
                renderCell={renderCell}
                emptyMessage="Nenhuma alocação encontrada"
                stickyLeftColumnIds={["colaborador"]}
              />
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
            </>
          )}
        </CardContent>
      </Card>

      {/* Filters Drawer */}
      <Sheet open={filtersOpen} onOpenChange={setFiltersOpen}>
        <SheetContent className="w-[400px] sm:w-[440px] bg-white flex flex-col p-0 overflow-hidden">
          <SheetHeader className="px-6 pt-6 pb-4">
            <SheetTitle className="flex items-center gap-2 text-foreground">
              <SlidersHorizontal className="h-5 w-5" />
              Filtros
            </SheetTitle>
          </SheetHeader>

          <div className="flex-1 overflow-y-auto px-6 space-y-1 pb-4">
            {/* Unidade */}
            <Collapsible
              open={filtrosExpandidos.unidade}
              onOpenChange={(open) =>
                setFiltrosExpandidos((prev) => ({ ...prev, unidade: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="location_on" size={20} className="text-muted-foreground" />
                  <span>Unidade</span>
                </div>
                {filtrosExpandidos.unidade ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <div className="flex items-center gap-2">
                  <Select
                    value={filtros.codigoDiretoria || "todos"}
                    onValueChange={(value) =>
                      setFiltros((prev) => ({
                        ...prev,
                        codigoDiretoria: value === "todos" ? "" : value,
                      }))
                    }
                    disabled={loadingFiltros}
                  >
                    <SelectTrigger className="flex-1">
                      <SelectValue placeholder="Selecione.." />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="todos">Todos</SelectItem>
                      {diretorias.map((diretoria) => (
                        <SelectItem key={diretoria.cod} value={diretoria.cod}>
                          {diretoria.diretoria}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {filtros.codigoDiretoria && (
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      className="h-10 w-10 shrink-0"
                      onClick={() =>
                        setFiltros((prev) => ({ ...prev, codigoDiretoria: "" }))
                      }
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              </CollapsibleContent>
            </Collapsible>

            {/* Gestor */}
            <Collapsible
              open={filtrosExpandidos.gestor}
              onOpenChange={(open) =>
                setFiltrosExpandidos((prev) => ({ ...prev, gestor: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="person" size={20} className="text-muted-foreground" />
                  <span>Gestor</span>
                </div>
                {filtrosExpandidos.gestor ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select
                  value={filtros.gestorFiltro || "todos"}
                  onValueChange={(value) =>
                    setFiltros((prev) => ({
                      ...prev,
                      gestorFiltro: value === "todos" ? "" : value,
                    }))
                  }
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione.." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="todos">Todos</SelectItem>
                    {gestores.map((gestor) => (
                      <SelectItem
                        key={gestor.codigoProfissional}
                        value={gestor.codigoProfissional}
                      >
                        {gestor.nome}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>

            {/* Colaborador/TBD */}
            <Collapsible
              open={filtrosExpandidos.colaboradorTbd}
              onOpenChange={(open) =>
                setFiltrosExpandidos((prev) => ({ ...prev, colaboradorTbd: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="person" size={20} className="text-muted-foreground" />
                  <span>Colaborador/TBD</span>
                </div>
                {filtrosExpandidos.colaboradorTbd ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select
                  value={filtros.colaboradorOuTbdFiltro || "todos"}
                  onValueChange={(value) =>
                    setFiltros((prev) => ({
                      ...prev,
                      colaboradorOuTbdFiltro: value === "todos" ? "" : value,
                    }))
                  }
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione.." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="todos">Todos</SelectItem>
                    {colaboradores.map((colab) => (
                      <SelectItem key={colab.codProfissional} value={colab.codProfissional}>
                        {colab.labelCodigoNome}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>

            {/* Disponibilidade de horário */}
            <Collapsible
              open={filtrosExpandidos.disponibilidade}
              onOpenChange={(open) =>
                setFiltrosExpandidos((prev) => ({ ...prev, disponibilidade: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="schedule" size={20} className="text-muted-foreground" />
                  <span>Disponibilidade de horário</span>
                </div>
                {filtrosExpandidos.disponibilidade ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select
                  value={filtros.statusHorasFiltro.toString()}
                  onValueChange={(value) =>
                    setFiltros((prev) => ({
                      ...prev,
                      statusHorasFiltro: Number(value) as StatusHorasFiltro,
                    }))
                  }
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione.." />
                  </SelectTrigger>
                  <SelectContent>
                    {STATUS_HORAS_OPTIONS.map((option) => (
                      <SelectItem key={option.value} value={option.value.toString()}>
                        {option.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>
          </div>

          <SheetFooter className="px-6 py-4 border-t gap-2">
            <Button variant="outline" onClick={handleLimparFiltros}>
              Limpar
            </Button>
            <Button onClick={handleAplicarFiltros}>Aplicar</Button>
          </SheetFooter>
        </SheetContent>
      </Sheet>
        </>
      )}

      {/* Conteúdo da sub-aba Projetos */}
      {subTabAtiva === "projetos" && (
        <ProjetosTabContent
          token={token}
          buscaProjetos={buscaProjetos}
          setBuscaProjetos={setBuscaProjetos}
          filtersProjetosOpen={filtersProjetosOpen}
          setFiltersProjetosOpen={setFiltersProjetosOpen}
          filtrosProjetos={filtrosProjetos}
          setFiltrosProjetos={setFiltrosProjetos}
          currentPageProjetos={currentPageProjetos}
          setCurrentPageProjetos={setCurrentPageProjetos}
          itemsPerPageProjetos={itemsPerPageProjetos}
          setItemsPerPageProjetos={setItemsPerPageProjetos}
          clientesProjetos={clientesProjetos}
          setClientesProjetos={setClientesProjetos}
          statusProjetos={statusProjetos}
          setStatusProjetos={setStatusProjetos}
          gestoresProjetos={gestoresProjetos}
          setGestoresProjetos={setGestoresProjetos}
          diretorias={diretorias}
          loadingFiltrosProjetos={loadingFiltrosProjetos}
          setLoadingFiltrosProjetos={setLoadingFiltrosProjetos}
          filtrosProjetosExpandidos={filtrosProjetosExpandidos}
          setFiltrosProjetosExpandidos={setFiltrosProjetosExpandidos}
        />
      )}
    </div>
  );
};
