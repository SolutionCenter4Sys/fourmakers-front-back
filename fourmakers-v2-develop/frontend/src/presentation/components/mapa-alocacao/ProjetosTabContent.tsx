import { useState, useCallback, useEffect, useMemo } from "react";
import { Search, X, SlidersHorizontal, ChevronUp, ChevronDown, Plus } from "@/components/ui/system-icons";
import { Icon } from "@/components/ui/icon";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { DataTable, TablePagination } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
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
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { container } from "@/core/di/container";
import { DiTokens } from "@/core/di/tokens";
import { ConsultaProjetoHorasUseCase } from "@domain/usecases/ConsultaProjetoHorasUseCase";
import { ListarNomesGestoresUseCase } from "@domain/usecases/ListarNomesGestoresUseCase";
import { ListarStatusProjetoUseCase } from "@domain/usecases/ListarStatusProjetoUseCase";
import type { Cliente, StatusProjeto } from "@data/api/ProjetosApi";
import { ProjetosApi } from "@data/api/ProjetosApi";
import type { DiretoriaColaborador } from "@domain/entities/Diretoria";
import type { ProjetoHoras } from "@domain/entities/MapaAlocacao";
import { toast } from "sonner";
import { useNavigate } from "react-router-dom";
import { Skeleton } from "@/components/ui/skeleton";

interface GestorFiltro {
  codigoProfissional: string;
  nome: string;
}

enum DisponibilidadeHorario {
  Todos = 0,
  HorasPendentes = 1,
  TodasHorasAlocadas = 2,
  HorasAlocadasAMais = 3,
}

const DISPONIBILIDADE_OPTIONS = [
  { value: DisponibilidadeHorario.Todos, label: "Todos" },
  { value: DisponibilidadeHorario.HorasPendentes, label: "Horas pendentes" },
  { value: DisponibilidadeHorario.TodasHorasAlocadas, label: "Todas as horas alocadas" },
  { value: DisponibilidadeHorario.HorasAlocadasAMais, label: "Horas alocadas a mais" },
];

interface ProjetosTabContentProps {
  token: string | null;
  buscaProjetos: string;
  setBuscaProjetos: (value: string) => void;
  filtersProjetosOpen: boolean;
  setFiltersProjetosOpen: (open: boolean) => void;
  filtrosProjetos: {
    cdProjeto: string;
    disponibilidadeHorario: number;
    cdStatusProjeto: number;
    nomeProjeto: string;
    codDiretoria: number;
    cliente: string;
    gestorProjeto: string;
    cdCliente: string;
  };
  setFiltrosProjetos: (filtros: any) => void;
  currentPageProjetos: number;
  setCurrentPageProjetos: (page: number) => void;
  itemsPerPageProjetos: number;
  setItemsPerPageProjetos: (items: number) => void;
  clientesProjetos: Cliente[];
  setClientesProjetos: (clientes: Cliente[]) => void;
  statusProjetos: StatusProjeto[];
  setStatusProjetos: (status: StatusProjeto[]) => void;
  gestoresProjetos: GestorFiltro[];
  setGestoresProjetos: (gestores: GestorFiltro[]) => void;
  diretorias: DiretoriaColaborador[];
  loadingFiltrosProjetos: boolean;
  setLoadingFiltrosProjetos: (loading: boolean) => void;
  filtrosProjetosExpandidos: {
    projeto: boolean;
    cliente: boolean;
    status: boolean;
    disponibilidade: boolean;
    unidade: boolean;
    gestor: boolean;
  };
  setFiltrosProjetosExpandidos: (filtros: any) => void;
}

export const ProjetosTabContent = ({
  token,
  buscaProjetos,
  setBuscaProjetos,
  filtersProjetosOpen,
  setFiltersProjetosOpen,
  filtrosProjetos,
  setFiltrosProjetos,
  currentPageProjetos,
  setCurrentPageProjetos,
  itemsPerPageProjetos,
  setItemsPerPageProjetos,
  clientesProjetos,
  setClientesProjetos,
  statusProjetos,
  setStatusProjetos,
  gestoresProjetos,
  setGestoresProjetos,
  diretorias,
  loadingFiltrosProjetos,
  setLoadingFiltrosProjetos,
  filtrosProjetosExpandidos,
  setFiltrosProjetosExpandidos,
}: ProjetosTabContentProps) => {
  const navigate = useNavigate();
  const [projetos, setProjetos] = useState<ProjetoHoras[]>([]);
  const [loadingProjetos, setLoadingProjetos] = useState(false);
  const [hasMoreProjetos, setHasMoreProjetos] = useState(false);

  // Carregar projetos
  const carregarProjetos = useCallback(async () => {
    if (!token) return;

    try {
      setLoadingProjetos(true);
      const useCase = container.resolve(ConsultaProjetoHorasUseCase);
      const response = await useCase.execute(token, {
        cursor: (currentPageProjetos - 1) * itemsPerPageProjetos,
        limite: itemsPerPageProjetos,
        cdProjeto: filtrosProjetos.cdProjeto || undefined,
        disponibilidadeHorario: filtrosProjetos.disponibilidadeHorario || 0,
        cdStatusProjeto: filtrosProjetos.cdStatusProjeto || 0,
        nomeProjeto: buscaProjetos.trim() || undefined,
        codDiretoria: filtrosProjetos.codDiretoria || 0,
        cliente: filtrosProjetos.cliente || undefined,
        gestorProjeto: filtrosProjetos.gestorProjeto || undefined,
        cdCliente: filtrosProjetos.cdCliente || undefined,
      });

      if (response.sucesso && Array.isArray(response.retorno)) {
        setProjetos(response.retorno);
        // Se retornou menos que o limite, não há mais páginas
        setHasMoreProjetos(response.retorno.length === itemsPerPageProjetos);
      } else {
        setProjetos([]);
        setHasMoreProjetos(false);
      }
    } catch (error) {
      console.error("Erro ao carregar projetos:", error);
      toast.error("Erro ao carregar projetos");
      setProjetos([]);
      setHasMoreProjetos(false);
    } finally {
      setLoadingProjetos(false);
    }
  }, [token, currentPageProjetos, itemsPerPageProjetos, filtrosProjetos, buscaProjetos]);

  // Carregar dados dos filtros
  const carregarDadosFiltros = useCallback(async () => {
    if (!token) return;

    try {
      setLoadingFiltrosProjetos(true);

      // Carregar clientes
      const projetosApi = container.resolve<ProjetosApi>(DiTokens.projetosApi);
      const clientesData = await projetosApi.listarClientesOrg(token);
      const clientesArray = Array.isArray(clientesData) ? clientesData : [];
      // Remover duplicatas
      const seenClientes = new Set<string>();
      const clientesUnicos = clientesArray.filter((cliente) => {
        if (seenClientes.has(cliente.codigoCliente)) return false;
        seenClientes.add(cliente.codigoCliente);
        return true;
      });
      setClientesProjetos(clientesUnicos);

      // Carregar status de projeto
      const listarStatusProjetoUseCase = container.resolve(ListarStatusProjetoUseCase);
      const statusResponse = await listarStatusProjetoUseCase.execute(token);
      if (statusResponse.sucesso && statusResponse.retorno) {
        setStatusProjetos(statusResponse.retorno);
      }

      // Carregar gestores (se houver unidade selecionada)
      if (filtrosProjetos.codDiretoria) {
        const listarNomesGestoresUseCase = container.resolve(ListarNomesGestoresUseCase);
        const gestoresResponse = await listarNomesGestoresUseCase.execute(token, {
          codDiretoria: filtrosProjetos.codDiretoria || undefined,
        });
        if (gestoresResponse.sucesso && gestoresResponse.retorno) {
          setGestoresProjetos(gestoresResponse.retorno.map(g => ({ codigoProfissional: g.codigoProfissional, nome: g.nome })));
        }
      } else {
        setGestoresProjetos([]);
      }
    } catch (error) {
      console.error("Erro ao carregar dados dos filtros:", error);
    } finally {
      setLoadingFiltrosProjetos(false);
    }
  }, [token, filtrosProjetos.codDiretoria]);

  useEffect(() => {
    if (token) {
      carregarProjetos();
    }
  }, [token, carregarProjetos]);

  // Handler para Enter no campo de busca
  const handleBuscaKeyDown = useCallback((e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      setCurrentPageProjetos(1);
      if (token) {
        carregarProjetos();
      }
    }
  }, [token, carregarProjetos]);

  useEffect(() => {
    if (token && filtersProjetosOpen) {
      carregarDadosFiltros();
    }
  }, [token, filtersProjetosOpen, carregarDadosFiltros]);

  // Carregar gestores quando unidade mudar
  useEffect(() => {
    const carregarGestores = async () => {
      if (!token || !filtrosProjetos.codDiretoria) {
        setGestoresProjetos([]);
        return;
      }

      try {
        const listarNomesGestoresUseCase = container.resolve(ListarNomesGestoresUseCase);
        const gestoresResponse = await listarNomesGestoresUseCase.execute(token, {
          codDiretoria: filtrosProjetos.codDiretoria || undefined,
        });
        if (gestoresResponse.sucesso && gestoresResponse.retorno) {
          setGestoresProjetos(gestoresResponse.retorno.map(g => ({ codigoProfissional: g.codigoProfissional, nome: g.nome })));
        }
      } catch (error) {
        console.error("Erro ao carregar gestores:", error);
        setGestoresProjetos([]);
      }
    };

    if (filtersProjetosOpen) {
      carregarGestores();
    }
  }, [token, filtrosProjetos.codDiretoria, filtersProjetosOpen, setGestoresProjetos]);

  // Aplicar filtros
  const handleAplicarFiltros = useCallback(() => {
    setCurrentPageProjetos(1);
    setFiltersProjetosOpen(false);
  }, [setFiltersProjetosOpen]);

  // Limpar filtros
  const handleLimparFiltros = useCallback(() => {
    setFiltrosProjetos({
      cdProjeto: "",
      disponibilidadeHorario: 0,
      cdStatusProjeto: 0,
      nomeProjeto: "",
      codDiretoria: 0,
      cliente: "",
      gestorProjeto: "",
      cdCliente: "",
    });
    setBuscaProjetos("");
    setCurrentPageProjetos(1);
  }, [setFiltrosProjetos, setBuscaProjetos]);

  // Verificar se há filtros aplicados
  

  // Calcular total de itens (usar projetos diretamente, pois a busca é feita na API)
  const totalItems = projetos.length;

  // Colunas da tabela
  const columns: Column[] = useMemo(
    () => [
      { id: "codigo_projeto", label: "Código", sortable: true, width: "w-[120px]" },
      { id: "nome_projeto", label: "Nome do Projeto", sortable: true, width: "w-[250px]" },
      { id: "cliente", label: "Cliente", sortable: true, width: "w-[200px]" },
      { id: "status_projeto", label: "Status", sortable: true, width: "w-[180px]" },
      { id: "horas_tecnicas", label: "Horas Técnicas", sortable: true, width: "w-[150px]" },
      { id: "horas_alocadas", label: "Horas Alocadas", sortable: true, width: "w-[150px]" },
      { id: "horas_disponiveis", label: "Horas Disponíveis", sortable: true, width: "w-[150px]" },
      { id: "quantidade_alocados", label: "Alocados", sortable: true, width: "w-[120px]" },
      { id: "quantidadeDeGestores", label: "Gestores", sortable: true, width: "w-[120px]" },
      { id: "quantidadeDeTBDS", label: "TBDS", sortable: true, width: "w-[120px]" },
      { id: "nome_gerente_projeto", label: "Gerente de Projeto", sortable: true, width: "w-[200px]" },
      { id: "data_inicio", label: "Data Início", sortable: true, width: "w-[150px]" },
      { id: "data_fim", label: "Data Fim", sortable: true, width: "w-[150px]" },
    ],
    []
  );

  // Renderizar célula
  const renderCell = (item: ProjetoHoras, columnId: string) => {
    switch (columnId) {
      case "codigo_projeto":
        return <span className="font-medium">{item.codigo_projeto}</span>;
      case "nome_projeto":
        return <span>{item.nome_projeto || "-"}</span>;
      case "cliente":
        return <span>{item.cliente || "-"}</span>;
      case "status_projeto":
        return (
          <Badge variant="outline" className="text-xs">
            {item.status_projeto || "-"}
          </Badge>
        );
      case "horas_tecnicas":
        return <span>{item.horas_tecnicas || 0}h</span>;
      case "horas_alocadas":
        return <span>{item.horas_alocadas || 0}h</span>;
      case "horas_disponiveis":
        return <span>{item.horas_disponiveis || 0}h</span>;
      case "quantidade_alocados":
        return <span>{item.quantidade_alocados || 0}</span>;
      case "quantidadeDeGestores":
        return <span>{item.quantidadeDeGestores || 0}</span>;
      case "quantidadeDeTBDS":
        return <span>{item.quantidadeDeTBDS || 0}</span>;
      case "nome_gerente_projeto":
        return <span>{item.nome_gerente_projeto || "-"}</span>;
      case "data_inicio":
        return item.data_inicio
          ? new Date(item.data_inicio).toLocaleDateString("pt-BR")
          : "-";
      case "data_fim":
        return item.data_fim
          ? new Date(item.data_fim).toLocaleDateString("pt-BR")
          : "-";
      default:
        return "-";
    }
  };

  return (
    <>
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="text-[20px] font-semibold">Projetos</CardTitle>
            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                className="gap-2"
                onClick={() => navigate("/page/mapa/listadeprojetos")}
              >
                <Plus className="h-4 w-4" />
                Novo Projeto
              </Button>
              <Button
                className="gap-2"
                onClick={() => navigate("/mapa-alocacao/nova")}
              >
                <Plus className="h-4 w-4" />
                Criar nova alocação
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {/* Barra de busca e filtros */}
          <div className="flex items-center gap-2 mb-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar por projeto"
                value={buscaProjetos}
                onChange={(e) => setBuscaProjetos(e.target.value)}
                onKeyDown={handleBuscaKeyDown}
                className="pl-10"
              />
              {buscaProjetos && (
                <Button
                  variant="ghost"
                  size="icon"
                  className="absolute right-1 top-1/2 transform -translate-y-1/2 h-6 w-6"
                  onClick={() => setBuscaProjetos("")}
                >
                  <X className="h-4 w-4" />
                </Button>
              )}
            </div>
            <TooltipProvider>
              <Tooltip>
                <TooltipTrigger asChild>
                  <div>
                    <Button
                      variant="outline"
                      disabled
                      className={cn(
                        "gap-2",
                        "cursor-not-allowed"
                      )}
                    >
                      <SlidersHorizontal className="h-4 w-4" />
                      Filtros
                    </Button>
                  </div>
                </TooltipTrigger>
                <TooltipContent>
                  <p>Em desenvolvimento</p>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          </div>

          {/* Tabela */}
          {loadingProjetos ? (
            <div className="space-y-4">
              {/* Skeleton do cabeçalho da tabela */}
              <div className="flex items-center gap-4 border-b pb-4 overflow-x-auto">
                <Skeleton className="h-10 w-32 flex-shrink-0" />
                <Skeleton className="h-10 w-64 flex-shrink-0" />
                <Skeleton className="h-10 w-48 flex-shrink-0" />
                <Skeleton className="h-10 w-44 flex-shrink-0" />
                <Skeleton className="h-10 w-36 flex-shrink-0" />
                <Skeleton className="h-10 w-36 flex-shrink-0" />
                <Skeleton className="h-10 w-36 flex-shrink-0" />
                <Skeleton className="h-10 w-28 flex-shrink-0" />
                <Skeleton className="h-10 w-28 flex-shrink-0" />
                <Skeleton className="h-10 w-28 flex-shrink-0" />
                <Skeleton className="h-10 w-48 flex-shrink-0" />
                <Skeleton className="h-10 w-36 flex-shrink-0" />
                <Skeleton className="h-10 w-36 flex-shrink-0" />
              </div>
              {/* Skeleton das linhas */}
              {[...Array(8)].map((_, index) => (
                <div key={index} className="flex items-center gap-4 py-3 border-b overflow-x-auto">
                  <Skeleton className="h-10 w-32 flex-shrink-0" />
                  <Skeleton className="h-10 w-64 flex-shrink-0" />
                  <Skeleton className="h-10 w-48 flex-shrink-0" />
                  <Skeleton className="h-10 w-44 flex-shrink-0" />
                  <Skeleton className="h-10 w-36 flex-shrink-0" />
                  <Skeleton className="h-10 w-36 flex-shrink-0" />
                  <Skeleton className="h-10 w-36 flex-shrink-0" />
                  <Skeleton className="h-10 w-28 flex-shrink-0" />
                  <Skeleton className="h-10 w-28 flex-shrink-0" />
                  <Skeleton className="h-10 w-28 flex-shrink-0" />
                  <Skeleton className="h-10 w-48 flex-shrink-0" />
                  <Skeleton className="h-10 w-36 flex-shrink-0" />
                  <Skeleton className="h-10 w-36 flex-shrink-0" />
                </div>
              ))}
            </div>
          ) : (
            <>
              <DataTable
                columns={columns}
                data={projetos}
                keyExtractor={(item) => item.codigo_projeto}
                renderCell={renderCell}
                emptyMessage="Nenhum projeto encontrado. Use o campo de busca ou os filtros para encontrar projetos."
              />
              {totalItems > 0 && (
                <TablePagination
                  currentPage={currentPageProjetos}
                  totalItems={totalItems}
                  itemsPerPage={itemsPerPageProjetos}
                  hasMore={hasMoreProjetos}
                  onPageChange={(page) => {
                    setCurrentPageProjetos(page);
                  }}
                  onItemsPerPageChange={(items) => {
                    setItemsPerPageProjetos(Number(items));
                    setCurrentPageProjetos(1);
                  }}
                />
              )}
            </>
          )}
        </CardContent>
      </Card>

      {/* Filtros Sidebar */}
      <Sheet open={filtersProjetosOpen} onOpenChange={setFiltersProjetosOpen}>
        <SheetContent side="right" className="w-full sm:max-w-md overflow-y-auto">
          <SheetHeader>
            <SheetTitle>Filtros</SheetTitle>
          </SheetHeader>

          <div className="mt-6 space-y-4">
            {/* Projeto */}
            <Collapsible
              open={filtrosProjetosExpandidos.projeto}
              onOpenChange={(open) =>
                setFiltrosProjetosExpandidos((prev: typeof filtrosProjetosExpandidos) => ({ ...prev, projeto: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="folder" size={20} className="text-muted-foreground" />
                  <span>Projeto</span>
                </div>
                {filtrosProjetosExpandidos.projeto ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <div className="space-y-3">
                  <div>
                    <label className="text-xs text-muted-foreground mb-1 block">
                      Código do Projeto
                    </label>
                    <Input
                      placeholder="Digite o código..."
                      value={filtrosProjetos.cdProjeto}
                      onChange={(e) =>
                        setFiltrosProjetos((prev: typeof filtrosProjetos) => ({
                          ...prev,
                          cdProjeto: e.target.value,
                        }))
                      }
                      disabled={loadingFiltrosProjetos}
                    />
                  </div>
                  <div>
                    <label className="text-xs text-muted-foreground mb-1 block">
                      Nome do Projeto
                    </label>
                    <Input
                      placeholder="Digite o nome..."
                      value={filtrosProjetos.nomeProjeto}
                      onChange={(e) =>
                        setFiltrosProjetos((prev: typeof filtrosProjetos) => ({
                          ...prev,
                          nomeProjeto: e.target.value,
                        }))
                      }
                      disabled={loadingFiltrosProjetos}
                    />
                  </div>
                </div>
              </CollapsibleContent>
            </Collapsible>

            {/* Cliente */}
            <Collapsible
              open={filtrosProjetosExpandidos.cliente}
              onOpenChange={(open) =>
                setFiltrosProjetosExpandidos((prev: typeof filtrosProjetosExpandidos) => ({ ...prev, cliente: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="building" size={20} className="text-muted-foreground" />
                  <span>Cliente</span>
                </div>
                {filtrosProjetosExpandidos.cliente ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <div className="space-y-3">
                  <div>
                    <label className="text-xs text-muted-foreground mb-1 block">
                      Cliente
                    </label>
                    <Select
                      value={filtrosProjetos.cdCliente || ""}
                      onValueChange={(value) =>
                        setFiltrosProjetos((prev: typeof filtrosProjetos) => ({
                          ...prev,
                          cdCliente: value,
                          cliente: clientesProjetos.find((c) => c.codigoCliente === value)?.nomeCliente || "",
                        }))
                      }
                      disabled={loadingFiltrosProjetos}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Selecione um cliente..." />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="">Todos</SelectItem>
                        {(clientesProjetos || []).filter(cliente => cliente?.codigoCliente != null).map((cliente) => (
                          <SelectItem key={cliente.codigoCliente} value={cliente.codigoCliente}>
                            {cliente.nomeCliente}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              </CollapsibleContent>
            </Collapsible>

            {/* Status */}
            <Collapsible
              open={filtrosProjetosExpandidos.status}
              onOpenChange={(open) =>
                setFiltrosProjetosExpandidos((prev: typeof filtrosProjetosExpandidos) => ({ ...prev, status: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="check-circle" size={20} className="text-muted-foreground" />
                  <span>Status</span>
                </div>
                {filtrosProjetosExpandidos.status ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select
                  value={(filtrosProjetos.cdStatusProjeto ?? 0).toString()}
                  onValueChange={(value) =>
                    setFiltrosProjetos((prev: typeof filtrosProjetos) => ({
                      ...prev,
                      cdStatusProjeto: Number(value),
                    }))
                  }
                  disabled={loadingFiltrosProjetos}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione..." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="0">Todos</SelectItem>
                    {(statusProjetos || []).filter(status => status?.codigoStatusProjeto != null).map((status) => (
                      <SelectItem key={status.codigoStatusProjeto} value={status.codigoStatusProjeto.toString()}>
                        {status.nomeStatusProjeto}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>

            {/* Disponibilidade de horário */}
            <Collapsible
              open={filtrosProjetosExpandidos.disponibilidade}
              onOpenChange={(open) =>
                setFiltrosProjetosExpandidos((prev: typeof filtrosProjetosExpandidos) => ({ ...prev, disponibilidade: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="schedule" size={20} className="text-muted-foreground" />
                  <span>Disponibilidade de horário</span>
                </div>
                {filtrosProjetosExpandidos.disponibilidade ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select
                  value={(filtrosProjetos.disponibilidadeHorario ?? 0).toString()}
                  onValueChange={(value) =>
                    setFiltrosProjetos((prev: typeof filtrosProjetos) => ({
                      ...prev,
                      disponibilidadeHorario: Number(value),
                    }))
                  }
                  disabled={loadingFiltrosProjetos}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione..." />
                  </SelectTrigger>
                  <SelectContent>
                    {DISPONIBILIDADE_OPTIONS.map((option) => (
                      <SelectItem key={option.value} value={option.value.toString()}>
                        {option.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>

            {/* Unidade */}
            <Collapsible
              open={filtrosProjetosExpandidos.unidade}
              onOpenChange={(open) =>
                setFiltrosProjetosExpandidos((prev: typeof filtrosProjetosExpandidos) => ({ ...prev, unidade: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="business" size={20} className="text-muted-foreground" />
                  <span>Unidade</span>
                </div>
                {filtrosProjetosExpandidos.unidade ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select
                  value={(filtrosProjetos.codDiretoria ?? 0).toString()}
                  onValueChange={(value) =>
                    setFiltrosProjetos((prev: typeof filtrosProjetos) => ({
                      ...prev,
                      codDiretoria: Number(value),
                    }))
                  }
                  disabled={loadingFiltrosProjetos}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione..." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="0">Todas</SelectItem>
                    {(diretorias || []).filter(dir => dir?.cod != null).map((dir) => (
                      <SelectItem key={dir.cod} value={dir.cod.toString()}>
                        {dir.diretoria}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>

            {/* Gestor */}
            <Collapsible
              open={filtrosProjetosExpandidos.gestor}
              onOpenChange={(open) =>
                setFiltrosProjetosExpandidos((prev: typeof filtrosProjetosExpandidos) => ({ ...prev, gestor: open }))
              }
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="person" size={20} className="text-muted-foreground" />
                  <span>Gestor de Projeto</span>
                </div>
                {filtrosProjetosExpandidos.gestor ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select
                  value={filtrosProjetos.gestorProjeto || ""}
                  onValueChange={(value) =>
                    setFiltrosProjetos((prev: typeof filtrosProjetos) => ({
                      ...prev,
                      gestorProjeto: value,
                    }))
                  }
                  disabled={loadingFiltrosProjetos || !filtrosProjetos.codDiretoria}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione..." />
                  </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="">Todos</SelectItem>
                        {(gestoresProjetos || []).filter(gestor => gestor?.codigoProfissional != null).map((gestor) => (
                          <SelectItem key={gestor.codigoProfissional} value={gestor.codigoProfissional}>
                            {gestor.nome}
                          </SelectItem>
                        ))}
                      </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>
          </div>

          <SheetFooter className="px-6 py-4 border-t gap-2 mt-6">
            <Button variant="outline" onClick={handleLimparFiltros}>
              Limpar
            </Button>
            <Button onClick={handleAplicarFiltros}>Aplicar</Button>
          </SheetFooter>
        </SheetContent>
      </Sheet>
    </>
  );
};
