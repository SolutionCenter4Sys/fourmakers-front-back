import { useState, useCallback, useRef, useEffect, useMemo } from "react";
import { Search, X, Check, Edit2, ChevronsUpDown, SlidersHorizontal, User, Table, ChevronUp, ChevronDown, Eye, Star } from "@/components/ui/system-icons";
import { Icon } from "@/components/ui/icon";
import { useNavigate, useLocation } from "react-router-dom";
import { formatarDataInput } from "@/shared/utils/formatUtils";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { DataTable, TablePagination } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { useAlocacoesTab } from "@/hooks/useMapaAlocacao";
import { useAppSelector } from "@/app/store/hooks";
import type { ListarAlocacoesPayload, AlocacaoColaboradorTbd } from "@domain/entities/MapaAlocacao";
import { cn } from "@/lib/utils";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command";
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
import { DiTokens } from "@/core/di/tokens";
import { ListarColaboradoresETbdsUseCase } from "@domain/usecases/ListarColaboradoresETbdsUseCase";
import { ListarProjetosColaboradorMapaUseCase } from "@domain/usecases/ListarProjetosColaboradorMapaUseCase";
import { ListarDepartamentosUseCase } from "@domain/usecases/ListarDepartamentosUseCase";
import { GetNotasFiscaisUseCase } from "@domain/usecases/GetNotasFiscaisUseCase";
import { ListarNomesGestoresUseCase } from "@domain/usecases/ListarNomesGestoresUseCase";
import { ListarStatusProjetoUseCase } from "@domain/usecases/ListarStatusProjetoUseCase";
import { SubstituirDadosAlocacoesPorPeriodoUseCase } from "@domain/usecases/SubstituirDadosAlocacoesPorPeriodoUseCase";
import { EditarAlocacaoUseCase } from "@domain/usecases/EditarAlocacaoUseCase";
import { RemoverAlocacoesEmLoteUseCase } from "@domain/usecases/RemoverAlocacoesEmLoteUseCase";
import { ListarPerfilAlocacaoUseCase } from "@domain/usecases/ListarPerfilAlocacaoUseCase";
import { AlteraPerfilAlocacaoUseCase } from "@domain/usecases/AlteraPerfilAlocacaoUseCase";
import { ListarSkillsPerfilGestorExternoPorIdUseCase } from "@domain/usecases/ListarSkillsPerfilGestorExternoPorIdUseCase";
import { ListarTodasSkillsColaboradorUseCase } from "@domain/usecases/ListarTodasSkillsColaboradorUseCase";
import type { PerfilAlocacaoItem, SkillPerfilGestorExterno } from "@domain/entities/MapaAlocacao";
import type { SkillColaborador } from "@domain/entities/Competencia";
import type { ColaboradorETbd, ProjetoColaboradorMapa, Gestor } from "@domain/entities/MapaAlocacao";
import type { Unidade } from "@domain/entities/NotaFiscalGestao";
import type { Departamento } from "@domain/entities/Departamento";
import type { Cliente, StatusProjeto } from "@data/api/ProjetosApi";
import type { ProjetosApi } from "@data/api/ProjetosApi";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";
import { useToast } from "@/hooks/use-toast";
import { Trash2 } from "@/components/ui/system-icons";
import { toast as sonnerToast } from "sonner";
import { Skeleton } from "@/components/ui/skeleton";

const STORAGE_KEY = 'mapa-alocacao-search-state';

export const AlocacoesTab = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { toast } = useToast();
  
  // Restaurar estado da busca do localStorage ou do state da navegação
  const getInitialBusca = () => {
    // Primeiro tenta do state da navegação (quando volta do Profile360)
    if ((location.state as { busca?: string })?.busca) {
      return (location.state as { busca: string }).busca;
    }
    // Depois tenta do localStorage
    try {
      const saved = localStorage.getItem(STORAGE_KEY);
      if (saved) {
        const parsed = JSON.parse(saved);
        return parsed.busca || "";
      }
    } catch (e) {
      console.error("Erro ao restaurar busca do localStorage:", e);
    }
    return "";
  };

  const getInitialFiltros = (): ListarAlocacoesPayload => {
    // Primeiro tenta do state da navegação
    if ((location.state as { filtros?: ListarAlocacoesPayload })?.filtros) {
      return (location.state as { filtros: ListarAlocacoesPayload }).filtros;
    }
    // Depois tenta do localStorage
    try {
      const saved = localStorage.getItem(STORAGE_KEY);
      if (saved) {
        const parsed = JSON.parse(saved);
        if (parsed.filtros) {
          return parsed.filtros;
        }
      }
    } catch (e) {
      console.error("Erro ao restaurar filtros do localStorage:", e);
    }
    return {
      pesquisa: "",
      codigoUnidade: null,
      codigoDepartamento: null,
      codigoGestorAdm: null,
      listaCodigoColabOuTbd: [],
      filtroTipoProfissional: 0,
      codigoGestorProjeto: null,
      listaCodigoClientes: [],
      apenasProjetosPrioritarios: false,
      filtroPrioridade: 0, // 0 = Todos, 1 = Prioritários, 2 = Não prioritários
      listaCodigoProjetos: [],
      statusProjeto: null,
    };
  };

  const [busca, setBusca] = useState(getInitialBusca);
  const [filtersOpen, setFiltersOpen] = useState(false);
  const [filtrosExpandidos, setFiltrosExpandidos] = useState<Record<string, boolean>>({
    unidade: true,
    departamento: false,
    gestorAdm: false,
    tipoColaborador: false,
    colaboradorTbd: false,
    gestorProjeto: false,
    clientes: false,
    statusProjeto: false,
    prioridade: false,
    projeto: false,
  });

  // Estados para listas de filtros
  const [unidades, setUnidades] = useState<Unidade[]>([]);
  const [departamentos, setDepartamentos] = useState<Departamento[]>([]);
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [colaboradoresFiltro, setColaboradoresFiltro] = useState<ColaboradorETbd[]>([]);
  const [projetosFiltro, setProjetosFiltro] = useState<ProjetoColaboradorMapa[]>([]);
  const [gestoresAdm, setGestoresAdm] = useState<Gestor[]>([]);
  const [gestoresProjeto, setGestoresProjeto] = useState<Gestor[]>([]);
  const [statusProjeto, setStatusProjeto] = useState<StatusProjeto[]>([]);
  const [loadingFiltros, setLoadingFiltros] = useState(false);
  
  // Estados para colaboradores e clientes selecionados (tags)
  const [colaboradoresSelecionados, setColaboradoresSelecionados] = useState<ColaboradorETbd[]>([]);
  const [clientesSelecionados, setClientesSelecionados] = useState<Cliente[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10);
  const [erroDialogOpen, setErroDialogOpen] = useState(false);
  const [erroMensagem, setErroMensagem] = useState("");
  const [substituindo, setSubstituindo] = useState(false);
  const [selectedIds, setSelectedIds] = useState<Set<number>>(new Set());
  const [modalExcluirOpen, setModalExcluirOpen] = useState(false);
  const [excluindo, setExcluindo] = useState(false);
  const token = useAppSelector((state) => state.auth.token);
  
  // Estados dos filtros
  const [filtros, setFiltros] = useState<ListarAlocacoesPayload>(getInitialFiltros);

  const { alocados, loading, error, buscarAlocacoes } = useAlocacoesTab(token);
  
  // Ref para controlar se já restaurou a busca
  const buscaRestauradaRef = useRef(false);
  
  // Estado para controlar qual célula está sendo editada
  const [editingCell, setEditingCell] = useState<{ rowId: number; columnId: string } | null>(null);
  const [editingValue, setEditingValue] = useState<string>("");
  const [perfilModalOpen, setPerfilModalOpen] = useState(false);
  const [perfilModalRowId, setPerfilModalRowId] = useState<number | null>(null);
  const [perfisDisponiveis, setPerfisDisponiveis] = useState<PerfilAlocacaoItem[]>([]);
  
  // Estados para modal de habilidades
  const [habilidadesModalOpen, setHabilidadesModalOpen] = useState(false);
  const [, setHabilidadesModalPerfilId] = useState<string>("");
  const [skillsPerfil, setSkillsPerfil] = useState<SkillPerfilGestorExterno[]>([]);
  const [loadingSkills, setLoadingSkills] = useState(false);
  
  // Estados para modal de habilidades Perfil 360
  const [habilidadesPerfil360ModalOpen, setHabilidadesPerfil360ModalOpen] = useState(false);
  const [, setHabilidadesPerfil360ModalCodigoInterno] = useState<string>("");
  const [skillsPerfil360, setSkillsPerfil360] = useState<SkillColaborador[]>([]);
  const [loadingSkillsPerfil360, setLoadingSkillsPerfil360] = useState(false);
  const [buscaSkillsPerfil360, setBuscaSkillsPerfil360] = useState("");
  const [perfilSearch, setPerfilSearch] = useState("");
  const [loadingPerfis, setLoadingPerfis] = useState(false);
  const [perfilSelecionado, setPerfilSelecionado] = useState<PerfilAlocacaoItem | null>(null);
  const [localAlocados, setLocalAlocados] = useState<AlocacaoColaboradorTbd[]>([]);
  const inputRef = useRef<HTMLInputElement>(null);
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  
  // Estados para combo de colaboradores
  const [colaboradores, setColaboradores] = useState<ColaboradorETbd[]>([]);
  const [loadingColaboradores, setLoadingColaboradores] = useState(false);
  const [colaboradorComboOpen, setColaboradorComboOpen] = useState<{ rowId: number } | null>(null);
  const [colaboradorSelecionado, setColaboradorSelecionado] = useState<{ rowId: number; colaborador: ColaboradorETbd } | null>(null);
  
  // Estados para combo de projetos
  const [projetos, setProjetos] = useState<ProjetoColaboradorMapa[]>([]);
  const [loadingProjetos, setLoadingProjetos] = useState(false);
  const [projetoComboOpen, setProjetoComboOpen] = useState<{ rowId: number } | null>(null);
  const [projetoSelecionado, setProjetoSelecionado] = useState<{ rowId: number; projeto: ProjetoColaboradorMapa } | null>(null);
  
  // Definir todas as colunas primeiro
  const allColumns: Column[] = useMemo(() => [
    { id: "checkbox", label: "", sortable: false, width: "w-12" },
    { id: "cv", label: "CV", sortable: false, width: "w-14" },
    { id: "colaborador", label: "Colaborador/TBD", sortable: true },
    { id: "departamento", label: "Departamento", sortable: true },
    { id: "gestorAdm", label: "Gestor Adm", sortable: true },
    { id: "clienteProjeto", label: "Cliente/Projeto", sortable: true },
    { id: "gestorProjeto", label: "Gestor projeto", sortable: true },
    { id: "perfil", label: "Perfil", sortable: true },
    { id: "inicio", label: "Início", sortable: true },
    { id: "fim", label: "Fim", sortable: true },
    { id: "horas", label: "Horas", sortable: true },
    { id: "percentual", label: "% Tempo", sortable: true },
    { id: "prioridade", label: "Prioridade", sortable: true },
    { id: "oportunidade", label: "Oportunidade", sortable: true },
    { id: "observacao", label: "Observação", sortable: true },
    { id: "statusProjeto", label: "Status Projeto", sortable: true },
    { id: "tbd", label: "TBD", sortable: true },
    { id: "retroalimentaCv", label: "Retroalimenta CV", sortable: true },
    { id: "habilidades", label: "Habilidades - Alocação", sortable: false },
    { id: "habilidadesPerfil360", label: "Habilidades Perfil 360", sortable: false },
  ], []);
  
  // Estado para visibilidade de colunas - inicializar todas como visíveis
  const [colunasVisibilidadeOpen, setColunasVisibilidadeOpen] = useState(false);
  
  // Estado para visibilidade de colunas - será inicializado no useEffect
  const [colunasVisiveis, setColunasVisiveis] = useState<Record<string, boolean>>({});
  
  // Restaurar busca quando voltar do Profile360 ou na inicialização
  useEffect(() => {
    // Evitar executar múltiplas vezes
    if (buscaRestauradaRef.current || !token) return;
    
    const state = location.state as { busca?: string; filtros?: ListarAlocacoesPayload } | null;
    const hasStateBusca = state?.busca !== undefined;
    const hasStateFiltros = state?.filtros !== undefined;
    
    // Se tiver busca restaurada (do state ou localStorage), executar
    const buscaParaExecutar = hasStateBusca ? state.busca : busca;
    const filtrosParaExecutar = hasStateFiltros ? state.filtros : filtros;
    
    if (buscaParaExecutar && buscaParaExecutar.trim()) {
      buscaRestauradaRef.current = true;
      const payload: ListarAlocacoesPayload = {
        ...filtrosParaExecutar,
        pesquisa: buscaParaExecutar.trim(),
      };
      buscarAlocacoes(payload);
    } else {
      buscaRestauradaRef.current = true;
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [location.state, token]);

  // Sincronizar dados locais quando alocados mudarem
  useEffect(() => {
    // Garantir que prioridade seja sempre booleano ao sincronizar
    const alocadosComPrioridadeCorrigida = alocados.map(alocado => {
      // Converter prioridade para booleano - tratar todos os casos possíveis
      let prioridadeBoolean: boolean;
      
      if (typeof alocado.prioridade === 'boolean') {
        // Se já é booleano, usar diretamente
        prioridadeBoolean = alocado.prioridade;
      } else if (alocado.prioridade === 1 || alocado.prioridade === "1") {
        prioridadeBoolean = true;
      } else if (alocado.prioridade === 0 || alocado.prioridade === "0" || alocado.prioridade === null || alocado.prioridade === undefined) {
        prioridadeBoolean = false;
      } else {
        // Fallback: converter para booleano
        prioridadeBoolean = Boolean(alocado.prioridade);
      }
      
      return {
        ...alocado,
        prioridade: prioridadeBoolean
      };
    });
    setLocalAlocados(alocadosComPrioridadeCorrigida);
  }, [alocados]);
  
  // Focar no input quando começar a editar
  useEffect(() => {
    if (editingCell) {
      setTimeout(() => {
        inputRef.current?.focus();
        textareaRef.current?.focus();
      }, 0);
    }
  }, [editingCell]);

  // Carregar perfis do projeto
  const carregarPerfis = useCallback(async (codProjeto: string) => {
    if (!token) return;
    
    setLoadingPerfis(true);
    try {
      const useCase = container.resolve(ListarPerfilAlocacaoUseCase);
      const response = await useCase.execute(token, {
        codProjeto: codProjeto,
        ocultarSkill: true,
      });
      
      if (response.retorno) {
        setPerfisDisponiveis(response.retorno);
      } else {
        setPerfisDisponiveis([]);
      }
    } catch (error) {
      console.error("Erro ao carregar perfis:", error);
      toast({
        title: "Erro",
        description: "Erro ao carregar perfis disponíveis",
        variant: "destructive",
      });
      setPerfisDisponiveis([]);
    } finally {
      setLoadingPerfis(false);
    }
  }, [token, toast]);

  // Filtrar perfis baseado na busca
  const perfisFiltrados = useMemo(() => {
    if (!perfilSearch.trim()) {
      return perfisDisponiveis;
    }
    const searchLower = perfilSearch.toLowerCase();
    return perfisDisponiveis.filter(perfil => 
      perfil.perfil.toLowerCase().includes(searchLower)
    );
  }, [perfisDisponiveis, perfilSearch]);

  // Carregar skills do Perfil 360
  const carregarSkillsPerfil360 = useCallback(async (codigoInternoColaborador: string) => {
    if (!token || !codigoInternoColaborador) return;

    try {
      setLoadingSkillsPerfil360(true);
      const useCase = container.resolve(ListarTodasSkillsColaboradorUseCase);
      const response = await useCase.execute(token, { codigoInternoColaborador });

      if (response.retorno) {
        setSkillsPerfil360(response.retorno);
      } else {
        setSkillsPerfil360([]);
        const mensagemErro = response.mensagem || "Erro ao carregar habilidades";
        toast({
          title: "Erro",
          description: mensagemErro,
          variant: "destructive",
        });
      }
    } catch (error) {
      setSkillsPerfil360([]);
      const mensagemErro = error instanceof Error ? error.message : "Erro ao carregar habilidades";
      toast({
        title: "Erro",
        description: mensagemErro,
        variant: "destructive",
      });
    } finally {
      setLoadingSkillsPerfil360(false);
    }
  }, [token, toast]);

  // Filtrar skills Perfil 360 baseado na busca
  const skillsPerfil360Filtradas = useMemo(() => {
    if (!buscaSkillsPerfil360.trim()) {
      return skillsPerfil360;
    }
    const searchLower = buscaSkillsPerfil360.toLowerCase();
    return skillsPerfil360.filter(skill => 
      skill.descricao.toLowerCase().includes(searchLower) ||
      skill.tipo.toLowerCase().includes(searchLower) ||
      skill.nivel.toLowerCase().includes(searchLower)
    );
  }, [skillsPerfil360, buscaSkillsPerfil360]);

  // Carregar skills do perfil
  const carregarSkillsPerfil = useCallback(async (perfilId: string) => {
    if (!token || !perfilId) return;

    // Extrair apenas a parte após o pipe "|" do perfilId
    // Exemplo: "1|923e562a-7e91-11f0-83d8-029c6b897a8d" -> "923e562a-7e91-11f0-83d8-029c6b897a8d"
    const perfilIdLimpo = perfilId.includes('|') 
      ? perfilId.split('|').slice(1).join('|') 
      : perfilId;

    try {
      setLoadingSkills(true);
      const useCase = container.resolve(ListarSkillsPerfilGestorExternoPorIdUseCase);
      const response = await useCase.execute(token, { perfilId: perfilIdLimpo });

      if (response.sucesso && response.retorno) {
        setSkillsPerfil(response.retorno);
      } else {
        setSkillsPerfil([]);
        const mensagemErro = response.mensagem || "Erro ao carregar habilidades";
        toast({
          title: "Erro",
          description: mensagemErro,
          variant: "destructive",
        });
      }
    } catch (error) {
      setSkillsPerfil([]);
      const mensagemErro = error instanceof Error ? error.message : "Erro ao carregar habilidades";
      toast({
        title: "Erro",
        description: mensagemErro,
        variant: "destructive",
      });
    } finally {
      setLoadingSkills(false);
    }
  }, [token, toast]);

  // Salvar perfil selecionado
  const salvarPerfil = useCallback(async () => {
    if (!perfilModalRowId || !perfilSelecionado || !token) return;

    try {
      setSubstituindo(true);
      const useCase = container.resolve(AlteraPerfilAlocacaoUseCase);
      const response = await useCase.execute(token, {
        periodoAlocacaoId: perfilModalRowId,
        perfilId: perfilSelecionado.id,
      });

      if (response.sucesso && response.AlocacaoEditada) {
        // Atualizar apenas a linha editada
        setLocalAlocados((alocadosAnteriores) => {
          const indexAtualizado = alocadosAnteriores.findIndex(
            a => a.periodoAlocadoId === response.AlocacaoEditada.periodoAlocadoId
          );
          
          if (indexAtualizado !== -1) {
            const novosAlocados = [...alocadosAnteriores];
            const alocadoComPrioridadeCorrigida = {
              ...response.AlocacaoEditada,
              prioridade: Boolean(response.AlocacaoEditada.prioridade)
            };
            novosAlocados[indexAtualizado] = alocadoComPrioridadeCorrigida;
            return novosAlocados;
          }
          
          return alocadosAnteriores;
        });

        toast({
          title: "Sucesso",
          description: "Perfil alterado com sucesso.",
          variant: "success",
        });

        setPerfilModalOpen(false);
        setPerfilModalRowId(null);
        setPerfilSelecionado(null);
        setPerfilSearch("");
        setPerfisDisponiveis([]);
      } else {
        const mensagemErro = response.mensagem || "Erro ao alterar perfil";
        toast({
          title: "Erro",
          description: mensagemErro,
          variant: "destructive",
        });
      }
    } catch (error) {
      const mensagemErro = error instanceof Error ? error.message : "Erro ao alterar perfil";
      toast({
        title: "Erro",
        description: mensagemErro,
        variant: "destructive",
      });
    } finally {
      setSubstituindo(false);
    }
  }, [perfilModalRowId, perfilSelecionado, token, toast]);

  // Carregar colaboradores quando abrir a combo
  const carregarColaboradores = useCallback(async () => {
    if (!token || colaboradores.length > 0) return;
    
    setLoadingColaboradores(true);
    try {
      const useCase = container.resolve(ListarColaboradoresETbdsUseCase);
      const response = await useCase.execute(token, {
        codigoDiretoria: 0,
        codigoGestor: 0,
        filtroTipoProfissional: 0,
        codigoDepartamento: "",
      });
      
      if (response.retorno) {
        setColaboradores(response.retorno);
      }
    } catch (error) {
      console.error("Erro ao carregar colaboradores:", error);
    } finally {
      setLoadingColaboradores(false);
    }
  }, [token, colaboradores.length]);

  // Função para carregar colaboradores baseado nos filtros
  const carregarColaboradoresFiltro = useCallback(async (filtrosAtuais?: ListarAlocacoesPayload) => {
    if (!token) return;
    
    const filtrosParaUsar = filtrosAtuais || filtros;
    
    try {
      const listarColaboradoresUseCase = container.resolve(ListarColaboradoresETbdsUseCase);
      const colaboradoresResponse = await listarColaboradoresUseCase.execute(token, {
        codigoDiretoria: filtrosParaUsar.codigoUnidade ? Number(filtrosParaUsar.codigoUnidade) : 0,
        codigoGestor: filtrosParaUsar.codigoGestorAdm ? Number(filtrosParaUsar.codigoGestorAdm) : 0,
        filtroTipoProfissional: filtrosParaUsar.filtroTipoProfissional || 0,
        codigoDepartamento: filtrosParaUsar.codigoDepartamento || "",
      });
      const colaboradoresData = colaboradoresResponse.retorno || [];
      
      // Remover duplicatas de forma mais eficiente usando Set
      // Usar setTimeout para não bloquear a UI
      setTimeout(() => {
        const seen = new Set<string>();
        const colaboradoresUnicos = colaboradoresData.filter((colab) => {
          if (seen.has(colab.codProfissional)) {
            return false;
          }
          seen.add(colab.codProfissional);
          return true;
        });
        setColaboradoresFiltro(colaboradoresUnicos);
      }, 0);
    } catch (error) {
      console.error("Erro ao carregar colaboradores:", error);
    }
  }, [token, filtros.codigoUnidade, filtros.codigoGestorAdm, filtros.filtroTipoProfissional, filtros.codigoDepartamento]);

  // Memoizar arrays filtrados para melhor performance
  const colaboradoresDisponiveis = useMemo(() => {
    return colaboradoresFiltro.filter(colab => 
      !colaboradoresSelecionados.find(c => c.codProfissional === colab.codProfissional)
    );
  }, [colaboradoresFiltro, colaboradoresSelecionados]);

  const clientesDisponiveis = useMemo(() => {
    return clientes.filter(cliente => 
      !clientesSelecionados.find(c => c.codigoCliente === cliente.codigoCliente)
    );
  }, [clientes, clientesSelecionados]);

  // Carregar dados dos filtros quando o drawer abrir
  useEffect(() => {
    const carregarDadosFiltros = async () => {
      if (!filtersOpen || !token) return;
      
      setLoadingFiltros(true);
      try {
        // Carregar unidades
        const getNotasFiscaisUseCase = container.resolve(GetNotasFiscaisUseCase);
        const unidadesResponse = await getNotasFiscaisUseCase.executeListarUnidades(token);
        const unidadesData = unidadesResponse.ListaUnidadesResult || unidadesResponse.retorno || [];
        // Remover duplicatas de forma eficiente usando Set
        const seen = new Set<string>();
        const unidadesUnicas = unidadesData.filter((unidade) => {
          if (seen.has(unidade.id)) return false;
          seen.add(unidade.id);
          return true;
        });
        setUnidades(unidadesUnicas);

        // Carregar departamentos (filtrar por unidade se selecionada)
        const listarDepartamentosUseCase = container.resolve(ListarDepartamentosUseCase);
        const departamentosResponse = await listarDepartamentosUseCase.execute(token, {
          codigoDiretoria: filtros.codigoUnidade || undefined,
        });
        const departamentosData = departamentosResponse.retorno || [];
        // Remover duplicatas de forma eficiente usando Set
        const seenDept = new Set<string>();
        const departamentosUnicos = departamentosData.filter((dept) => {
          if (seenDept.has(dept.cod)) return false;
          seenDept.add(dept.cod);
          return true;
        });
        setDepartamentos(departamentosUnicos);

        // Carregar clientes
        const projetosApi = container.resolve<ProjetosApi>(DiTokens.projetosApi);
        const clientesData = await projetosApi.listarClientesOrg(token, {
          codigoClienteFiltro: '', // Vazio para listar todos
          codigoGerenteProjeto: filtros.codigoGestorProjeto || undefined,
        });
        const clientesArray = Array.isArray(clientesData) ? clientesData : [];
        // Remover duplicatas de forma eficiente usando Set
        const seenClientes = new Set<string>();
        const clientesUnicos = clientesArray.filter((cliente) => {
          if (seenClientes.has(cliente.codigoCliente)) return false;
          seenClientes.add(cliente.codigoCliente);
          return true;
        });
        setClientes(clientesUnicos);

        // Carregar colaboradores/TBD para filtro (baseado nos filtros atuais)
        await carregarColaboradoresFiltro();

        // Carregar projetos para filtro
        const listarProjetosUseCase = container.resolve(ListarProjetosColaboradorMapaUseCase);
        const projetosResponse = await listarProjetosUseCase.execute(token, {
          codigoProfissional: "",
          ehTbd: null,
          codigoGerenteProjeto: "",
          listaCodigoCliente: [],
          status: "",
          prioritarioFiltro: 0,
        });
        const projetosData = projetosResponse.Projetos || [];
        // Remover duplicatas de forma eficiente usando Set
        const seenProj = new Set<string>();
        const projetosUnicos = projetosData.filter((projeto) => {
          if (seenProj.has(projeto.codigoProjeto)) return false;
          seenProj.add(projeto.codigoProjeto);
          return true;
        });
        setProjetosFiltro(projetosUnicos);

        // Carregar gestores adm e de projeto (filtrar por unidade se selecionada)
        const listarNomesGestoresUseCase = container.resolve(ListarNomesGestoresUseCase);
        const gestoresResponse = await listarNomesGestoresUseCase.execute(token, {
          codDiretoria: filtros.codigoUnidade ? Number(filtros.codigoUnidade) : undefined,
          codDepartamento: filtros.codigoDepartamento || undefined,
        });
        const gestoresData = gestoresResponse.Gestor || [];
        // Remover duplicatas de forma eficiente usando Set
        const seenGestor = new Set<string>();
        const gestoresUnicos = gestoresData.filter((gestor) => {
          if (seenGestor.has(gestor.codigoProfissional)) return false;
          seenGestor.add(gestor.codigoProfissional);
          return true;
        });
        setGestoresAdm(gestoresUnicos);
        setGestoresProjeto(gestoresUnicos);

        // Carregar status de projeto
        const listarStatusProjetoUseCase = container.resolve(ListarStatusProjetoUseCase);
        const statusProjetoResponse = await listarStatusProjetoUseCase.execute(token);
        const statusData = statusProjetoResponse.retorno || [];
        // Remover duplicatas de forma eficiente usando Set
        const seenStatus = new Set<number>();
        const statusUnicos = statusData.filter((status) => {
          if (seenStatus.has(status.codigoStatusProjeto)) return false;
          seenStatus.add(status.codigoStatusProjeto);
          return true;
        });
        setStatusProjeto(statusUnicos);
      } catch (error) {
        console.error("Erro ao carregar dados dos filtros:", error);
      } finally {
        setLoadingFiltros(false);
      }
    };

    carregarDadosFiltros();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtersOpen, token]);

  // Restaurar colaboradores selecionados quando os dados forem carregados
  useEffect(() => {
    if (filtros.listaCodigoColabOuTbd && filtros.listaCodigoColabOuTbd.length > 0 && colaboradoresFiltro.length > 0) {
      const colaboradoresParaRestaurar = colaboradoresFiltro.filter(colab => 
        filtros.listaCodigoColabOuTbd?.includes(colab.codProfissional)
      );
      // Só restaurar se os códigos não corresponderem aos já selecionados
      const codigosAtuais = colaboradoresSelecionados.map(c => c.codProfissional).sort().join(',');
      const codigosParaRestaurar = colaboradoresParaRestaurar.map(c => c.codProfissional).sort().join(',');
      if (codigosParaRestaurar && codigosAtuais !== codigosParaRestaurar) {
        setColaboradoresSelecionados(colaboradoresParaRestaurar);
      }
    } else if ((!filtros.listaCodigoColabOuTbd || filtros.listaCodigoColabOuTbd.length === 0) && colaboradoresSelecionados.length > 0) {
      // Limpar se não houver códigos nos filtros
      setColaboradoresSelecionados([]);
    }
  }, [colaboradoresFiltro, filtros.listaCodigoColabOuTbd]);

  // Restaurar clientes selecionados quando os dados forem carregados
  useEffect(() => {
    if (filtros.listaCodigoClientes && filtros.listaCodigoClientes.length > 0 && clientes.length > 0) {
      const clientesParaRestaurar = clientes.filter(cliente => 
        filtros.listaCodigoClientes?.includes(cliente.codigoCliente)
      );
      // Só restaurar se os códigos não corresponderem aos já selecionados
      const codigosAtuais = clientesSelecionados.map(c => c.codigoCliente).sort().join(',');
      const codigosParaRestaurar = clientesParaRestaurar.map(c => c.codigoCliente).sort().join(',');
      if (codigosParaRestaurar && codigosAtuais !== codigosParaRestaurar) {
        setClientesSelecionados(clientesParaRestaurar);
      }
    } else if ((!filtros.listaCodigoClientes || filtros.listaCodigoClientes.length === 0) && clientesSelecionados.length > 0) {
      // Limpar se não houver códigos nos filtros
      setClientesSelecionados([]);
    }
  }, [clientes, filtros.listaCodigoClientes]);

  // Carregar projetos quando abrir a combo
  const carregarProjetos = useCallback(async () => {
    if (!token || projetos.length > 0) return;
    
    setLoadingProjetos(true);
    try {
      const useCase = container.resolve(ListarProjetosColaboradorMapaUseCase);
      const response = await useCase.execute(token, {
        codigoProfissional: "",
        ehTbd: null,
        codigoGerenteProjeto: "",
        listaCodigoCliente: [],
        status: "",
        prioritarioFiltro: 0,
      });
      
      if (response.Projetos) {
        setProjetos(response.Projetos);
      }
    } catch (error) {
      console.error("Erro ao carregar projetos:", error);
    } finally {
      setLoadingProjetos(false);
    }
  }, [token, projetos.length]);

  // Filtrar colunas baseado na visibilidade
  const columns = useMemo(() => {
    // Se colunasVisiveis está vazio, mostrar todas (ainda não inicializado)
    if (Object.keys(colunasVisiveis).length === 0) {
      return allColumns;
    }
    // Filtrar colunas baseado na visibilidade
    return allColumns.filter(col => {
      const isVisible = colunasVisiveis[col.id];
      // Retornar true apenas se explicitamente true, false ou undefined = oculto
      return isVisible === true;
    });
  }, [allColumns, colunasVisiveis]);
  
  // Inicializar colunasVisiveis apenas uma vez quando o componente montar
  useEffect(() => {
    // Inicializar apenas se ainda não foi inicializado
    if (Object.keys(colunasVisiveis).length === 0) {
      const visiveis: Record<string, boolean> = {};
      allColumns.forEach(col => {
        visiveis[col.id] = true;
      });
      setColunasVisiveis(visiveis);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Executar apenas uma vez na montagem

  const handleBuscar = useCallback(() => {
    // Não fazer busca se o campo estiver vazio
    if (!busca.trim()) {
      return;
    }
    
    const payload: ListarAlocacoesPayload = {
      ...filtros,
      pesquisa: busca.trim(),
    };
    
    // Salvar estado no localStorage
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify({
        busca: busca.trim(),
        filtros: payload,
      }));
    } catch (e) {
      console.error("Erro ao salvar busca no localStorage:", e);
    }
    
    buscarAlocacoes(payload);
  }, [busca, filtros, buscarAlocacoes]);

  const handleAplicar = useCallback(() => {
    // Preparar payload no formato correto conforme especificado
    // O payload deve ter strings vazias para campos vazios e arrays vazios
    const payload: ListarAlocacoesPayload & { habilidades?: string[]; perfis?: string[]; codigoStatusProjeto?: number } = {
      pesquisa: busca.trim() || "",
      codigoUnidade: filtros.codigoUnidade ? String(filtros.codigoUnidade) : "",
      codigoDepartamento: filtros.codigoDepartamento || "",
      codigoGestorAdm: filtros.codigoGestorAdm || "",
      listaCodigoColabOuTbd: filtros.listaCodigoColabOuTbd || [],
      filtroTipoProfissional: filtros.filtroTipoProfissional || 0,
      codigoGestorProjeto: filtros.codigoGestorProjeto || "",
      listaCodigoClientes: filtros.listaCodigoClientes || [],
      apenasProjetosPrioritarios: filtros.apenasProjetosPrioritarios || false,
      filtroPrioridade: filtros.filtroPrioridade || 0,
      listaCodigoProjetos: filtros.listaCodigoProjetos || [],
      statusProjeto: filtros.statusProjeto || null,
      habilidades: [],
      perfis: [],
      codigoStatusProjeto: 0,
    };

    // Salvar estado no localStorage
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify({
        busca: busca.trim(),
        filtros: payload,
      }));
    } catch (e) {
      console.error("Erro ao salvar filtros no localStorage:", e);
    }

    // Chamar o endpoint para buscar alocações
    buscarAlocacoes(payload);
  }, [busca, filtros, buscarAlocacoes]);

  const handleLimparTabela = useCallback(() => {
    // Resetar busca
    setBusca("");
    
    // Resetar filtros para valores padrão
    const filtrosLimpos: ListarAlocacoesPayload = {
      pesquisa: "",
      codigoUnidade: null,
      codigoDepartamento: null,
      codigoGestorAdm: null,
      listaCodigoColabOuTbd: [],
      filtroTipoProfissional: 0,
      codigoGestorProjeto: null,
      listaCodigoClientes: [],
      apenasProjetosPrioritarios: false,
      filtroPrioridade: 0,
      listaCodigoProjetos: [],
      statusProjeto: null,
    };
    setFiltros(filtrosLimpos);
    
    // Limpar seleções de filtros
    setColaboradoresSelecionados([]);
    setClientesSelecionados([]);
    
    // Limpar tabela
    setLocalAlocados([]);
    
    // Limpar localStorage
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch (e) {
      console.error("Erro ao limpar busca do localStorage:", e);
    }
    
    // Resetar página
    setCurrentPage(1);
  }, []);

  const handleLimpar = useCallback(() => {
    setBusca("");
    const filtrosLimpos = {
      pesquisa: "",
      codigoUnidade: null,
      codigoDepartamento: null,
      codigoGestorAdm: null,
      listaCodigoColabOuTbd: [],
      filtroTipoProfissional: 0,
      codigoGestorProjeto: null,
      listaCodigoClientes: [],
      apenasProjetosPrioritarios: false,
      filtroPrioridade: 0,
      listaCodigoProjetos: [],
      statusProjeto: null,
    };
    setFiltros(filtrosLimpos);
    setColaboradoresSelecionados([]);
    setClientesSelecionados([]);

    // Limpar localStorage também
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch (e) {
      console.error("Erro ao limpar busca do localStorage:", e);
    }
  }, []);

  // Calcular dados paginados (sem filtro local, pois a busca é feita na API)
  const totalItems = localAlocados.length;
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  
  // Transformar dados para incluir propriedades que correspondem aos IDs das colunas
  // Isso facilita a ordenação automática do DataTable
  const transformedData = useMemo(() => {
    return localAlocados.map(alocado => {
      // Criar objeto base mantendo todas as propriedades originais
      const base = { ...alocado };
      
      // Adicionar propriedades extras para ordenação (sem sobrescrever as booleanas)
      return {
        ...base,
        colaborador: alocado.nomeColaborador || "",
        departamento: alocado.nomeDepartamento || "",
        gestorAdm: alocado.nomeGestorAdm || "",
        clienteProjeto: alocado.projeto?.labelClienteProjeto || "",
        gestorProjeto: alocado.projeto?.gestoresProjeto?.map(g => g.nomeGestor).join(", ") || "",
        inicio: alocado.dataInicio || "",
        fim: alocado.dataFim || "",
        horas: alocado.quantidadeDeHoras || 0,
        percentual: alocado.percentual || 0,
        statusProjeto: alocado.projeto?.statusProjeto || "",
        // Manter valores booleanos originais (tbd, prioridade, retroalimentaCv)
        // Para ordenação, usar os valores booleanos diretamente (true > false)
        // Manter o objeto perfil original e criar perfilNome para ordenação
        perfilNome: typeof alocado.perfil === 'string' ? alocado.perfil : (alocado.perfil?.perfil || ""),
        oportunidade: alocado.oportunidade || "",
        observacao: alocado.observacao || "",
        habilidades: alocado.habilidadesAlocacao?.join(", ") || "",
      } as unknown as AlocacaoColaboradorTbd & Record<string, unknown>;
    });
  }, [localAlocados]);
  
  const paginatedAlocados = transformedData.slice(startIndex, endIndex);

  // Contar quantos filtros estão aplicados
  const quantidadeFiltrosAplicados = useMemo(() => {
    let count = 0;
    if (filtros.codigoUnidade) count++;
    if (filtros.codigoDepartamento) count++;
    if (filtros.codigoGestorAdm) count++;
    if (filtros.listaCodigoColabOuTbd && filtros.listaCodigoColabOuTbd.length > 0) count++;
    if (filtros.filtroTipoProfissional && filtros.filtroTipoProfissional !== 0) count++;
    if (filtros.codigoGestorProjeto) count++;
    if (filtros.listaCodigoClientes && filtros.listaCodigoClientes.length > 0) count++;
    if (filtros.filtroPrioridade && filtros.filtroPrioridade !== 0) count++;
    if (filtros.listaCodigoProjetos && filtros.listaCodigoProjetos.length > 0) count++;
    if (filtros.statusProjeto) count++;
    return count;
  }, [filtros]);

  const temFiltrosAplicados = quantidadeFiltrosAplicados > 0;

  // Resetar página quando busca mudar
  useEffect(() => {
    setCurrentPage(1);
  }, [busca]);

  const formatarData = (dataStr: string): string => {
    if (!dataStr) return "";
    try {
      const data = new Date(dataStr);
      const dia = String(data.getDate()).padStart(2, "0");
      const mes = String(data.getMonth() + 1).padStart(2, "0");
      const ano = data.getFullYear();
      return `${dia}/${mes}/${ano}`;
    } catch {
      return dataStr;
    }
  };

  const parsearData = (dataStr: string): string => {
    // Converte DD/MM/YYYY para formato ISO
    if (!dataStr || dataStr.length !== 10) return "";
    try {
      const [dia, mes, ano] = dataStr.split("/");
      if (!dia || !mes || !ano) return "";
      return `${ano}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}T00:00:00`;
    } catch {
      return "";
    }
  };

  // Campos editáveis na ordem de navegação
  const camposEditaveis = [
    "colaborador",
    "clienteProjeto",
    "inicio",
    "fim",
    "horas",
    "percentual",
    "prioridade",
    "oportunidade",
    "observacao"
  ];

  const obterProximoCampo = (columnId: string): string | null => {
    const indexAtual = camposEditaveis.indexOf(columnId);
    if (indexAtual === -1 || indexAtual === camposEditaveis.length - 1) {
      return null; // Último campo ou campo não editável
    }
    return camposEditaveis[indexAtual + 1];
  };

  const iniciarEdicao = (rowId: number, columnId: string, valorAtual: string) => {
    setEditingCell({ rowId, columnId });
    setEditingValue(valorAtual);
    
    // Se for colaborador, abrir a combo automaticamente
    if (columnId === "colaborador") {
      setColaboradorComboOpen({ rowId });
      setColaboradorSelecionado(null);
      carregarColaboradores();
    }
    
    // Se for cliente/projeto, abrir a combo automaticamente
    if (columnId === "clienteProjeto") {
      setProjetoComboOpen({ rowId });
      setProjetoSelecionado(null);
      carregarProjetos();
    }
  };

  const navegarParaProximoCampo = (rowId: number, columnIdAtual: string) => {
    const proximoCampo = obterProximoCampo(columnIdAtual);
    if (proximoCampo) {
      // Salvar o campo atual antes de navegar (apenas se houver mudanças)
      salvarEdicao();
      
      // Pequeno delay para garantir que o save foi processado e o estado atualizado
      setTimeout(() => {
        const alocado = localAlocados.find(a => a.periodoAlocadoId === rowId);
        if (alocado) {
          let valorInicial = "";
          
          // Obter valor inicial baseado no próximo campo
          switch (proximoCampo) {
            case "colaborador":
              valorInicial = alocado.nomeColaborador || "";
              break;
            case "clienteProjeto":
              valorInicial = alocado.projeto?.labelClienteProjeto || "";
              break;
            case "inicio":
              valorInicial = formatarData(alocado.dataInicio);
              break;
            case "fim":
              valorInicial = formatarData(alocado.dataFim);
              break;
            case "horas":
              valorInicial = alocado.quantidadeDeHoras?.toString() || "0";
              break;
            case "percentual":
              valorInicial = `${alocado.percentual || 0}`;
              break;
            case "prioridade":
              valorInicial = alocado.prioridade ? "true" : "false";
              break;
            case "oportunidade":
              valorInicial = alocado.oportunidade || "";
              break;
            case "observacao":
              valorInicial = alocado.observacao || "";
              break;
          }
          
          iniciarEdicao(rowId, proximoCampo, valorInicial);
        }
      }, 100);
    }
  };

  const handleExcluirAlocacoes = async () => {
    if (!token || selectedIds.size === 0) return;

    setExcluindo(true);
    try {
      const idsArray = Array.from(selectedIds).map(id => id.toString());
      const useCase = container.resolve(RemoverAlocacoesEmLoteUseCase);
      const response = await useCase.execute(token, {
        idsPeriodoAlocacao: idsArray,
      });

      if (response.sucesso) {
        // Remover os registros da tabela
        setLocalAlocados(prev => prev.filter(alocado => !selectedIds.has(alocado.periodoAlocadoId)));
        // Limpar seleção
        setSelectedIds(new Set());
        // Fechar modal
        setModalExcluirOpen(false);
        // Exibir toast de sucesso
        sonnerToast.success("Alocações excluídas com sucesso!");
      } else {
        // Exibir modal de erro
        setErroMensagem(response.mensagem || "Erro ao excluir alocações");
        setErroDialogOpen(true);
        setModalExcluirOpen(false);
      }
    } catch (error: unknown) {
      const errorMessage = error instanceof Error ? error.message : "Erro desconhecido ao excluir alocações";
      setErroMensagem(errorMessage);
      setErroDialogOpen(true);
      setModalExcluirOpen(false);
    } finally {
      setExcluindo(false);
    }
  };

  const salvarEdicao = async () => {
    if (!editingCell) return;
    
    const { rowId, columnId } = editingCell;
    const alocadoIndex = localAlocados.findIndex(a => a.periodoAlocadoId === rowId);
    
    if (alocadoIndex === -1) {
      setEditingCell(null);
      setColaboradorSelecionado(null);
      setColaboradorComboOpen(null);
      setProjetoSelecionado(null);
      setProjetoComboOpen(null);
      return;
    }

    // Salvar o estado original antes de fazer alterações (para reverter em caso de erro)
    const alocadoOriginal = { ...localAlocados[alocadoIndex] };
    const novoAlocados = [...localAlocados];
    const alocado = { ...novoAlocados[alocadoIndex] };

    // Atualizar o valor baseado na coluna
    switch (columnId) {
      case "colaborador":
        const colaboradorAtual = colaboradorSelecionado?.rowId === rowId ? colaboradorSelecionado.colaborador : null;
        if (colaboradorAtual) {
          // Se for colaborador, chamar o endpoint de substituição
          if (token) {
            setSubstituindo(true);
            try {
              const useCase = container.resolve(SubstituirDadosAlocacoesPorPeriodoUseCase);
              const response = await useCase.execute(token, {
                codigoColaboradorNovo: colaboradorAtual.codProfissional,
                ehTbd: colaboradorAtual.ehTbd,
                periodosIdSubstituidos: [rowId],
              });

              if (response.sucesso && response.retorno && response.retorno.length > 0) {
                // Atualizar apenas a linha editada com os dados retornados
                const alocadoAtualizado = response.retorno[0];
                
                setLocalAlocados((alocadosAnteriores) => {
                  const indexAtualizado = alocadosAnteriores.findIndex(
                    a => a.periodoAlocadoId === alocadoAtualizado.periodoAlocadoId
                  );
                  
                  if (indexAtualizado !== -1) {
                    const novosAlocados = [...alocadosAnteriores];
                    // Garantir que prioridade seja sempre booleano
                    const alocadoComPrioridadeCorrigida = {
                      ...alocadoAtualizado,
                      prioridade: Boolean(alocadoAtualizado.prioridade)
                    };
                    novosAlocados[indexAtualizado] = alocadoComPrioridadeCorrigida;
                    return novosAlocados;
                  }
                  
                  return alocadosAnteriores;
                });
                
                // Exibir toast de sucesso
                toast({
                  title: "Sucesso",
                  description: "Colaborador/TBD substituído com sucesso.",
                  variant: "success",
                });
                
                // Limpar estados de edição
                setEditingCell(null);
                setEditingValue("");
                setColaboradorSelecionado(null);
                setColaboradorComboOpen(null);
              } else {
                // Exibir erro em modal
                setErroMensagem(response.mensagem || "Erro ao substituir colaborador");
                setErroDialogOpen(true);
              }
            } catch (error) {
              const mensagemErro = error instanceof Error ? error.message : "Erro ao substituir colaborador";
              setErroMensagem(mensagemErro);
              setErroDialogOpen(true);
            } finally {
              setSubstituindo(false);
            }
          }
          return; // Retornar aqui pois a atualização já foi feita pela API
        } else {
          // Se não selecionou, não salva
          return;
        }
      case "clienteProjeto":
        const projetoAtual = projetoSelecionado?.rowId === rowId ? projetoSelecionado.projeto : null;
        if (projetoAtual) {
          // Se for cliente/projeto, chamar o endpoint de substituição
          if (token) {
            setSubstituindo(true);
            try {
              const useCase = container.resolve(SubstituirDadosAlocacoesPorPeriodoUseCase);
              const response = await useCase.execute(token, {
                codigoProjetoNovo: projetoAtual.codigoProjeto,
                codigoColaboradorNovo: "",
                ehTbd: false,
                periodosIdSubstituidos: [rowId],
              });

              if (response.sucesso && response.retorno && response.retorno.length > 0) {
                // Atualizar apenas a linha editada com os dados retornados
                const alocadoAtualizado = response.retorno[0];
                
                setLocalAlocados((alocadosAnteriores) => {
                  const indexAtualizado = alocadosAnteriores.findIndex(
                    a => a.periodoAlocadoId === alocadoAtualizado.periodoAlocadoId
                  );
                  
                  if (indexAtualizado !== -1) {
                    const novosAlocados = [...alocadosAnteriores];
                    // Garantir que prioridade seja sempre booleano
                    const alocadoComPrioridadeCorrigida = {
                      ...alocadoAtualizado,
                      prioridade: Boolean(alocadoAtualizado.prioridade)
                    };
                    novosAlocados[indexAtualizado] = alocadoComPrioridadeCorrigida;
                    return novosAlocados;
                  }
                  
                  return alocadosAnteriores;
                });
                
                // Exibir toast de sucesso
                toast({
                  title: "Sucesso",
                  description: "Cliente/Projeto substituído com sucesso.",
                  variant: "success",
                });
                
                // Limpar estados de edição
                setEditingCell(null);
                setEditingValue("");
                setProjetoSelecionado(null);
                setProjetoComboOpen(null);
              } else {
                // Exibir erro em modal
                setErroMensagem(response.mensagem || "Erro ao substituir projeto");
                setErroDialogOpen(true);
              }
            } catch (error) {
              const mensagemErro = error instanceof Error ? error.message : "Erro ao substituir projeto";
              setErroMensagem(mensagemErro);
              setErroDialogOpen(true);
            } finally {
              setSubstituindo(false);
            }
          }
          return; // Retornar aqui pois a atualização já foi feita pela API
        } else {
          // Se não selecionou, não salva
          return;
        }
      case "inicio":
        alocado.dataInicio = parsearData(editingValue);
        break;
      case "fim":
        alocado.dataFim = parsearData(editingValue);
        break;
      case "horas":
        alocado.quantidadeDeHoras = Number(editingValue) || 0;
        break;
      case "percentual":
        const percentualValue = Number(editingValue.replace("%", "").trim()) || 0;
        alocado.percentual = percentualValue;
        // Calcular quantidadeDeHoras baseado no percentual: 100% = 8 horas
        // Fórmula: quantidadeDeHoras = (percentual / 100) * 8
        alocado.quantidadeDeHoras = (percentualValue / 100) * 8;
        break;
      case "prioridade":
        alocado.prioridade = editingValue === "true" || editingValue === "1";
        break;
      case "oportunidade":
        alocado.oportunidade = editingValue;
        break;
      case "observacao":
        alocado.observacao = editingValue;
        break;
      case "perfil":
        // Perfil é editado via modal, não via campo de texto
        return;
    }

    // Atualizar localmente primeiro
    novoAlocados[alocadoIndex] = alocado;
    setLocalAlocados(novoAlocados);

    // Se for um campo editável verde, chamar o endpoint de edição
    // Nota: "perfil" não entra aqui pois tem seu próprio fluxo via modal
    const camposEditaveisVerdes = ["inicio", "fim", "horas", "percentual", "prioridade", "oportunidade", "observacao"];
    if (camposEditaveisVerdes.includes(columnId) && token) {
      setSubstituindo(true);
      try {
        // Converter dataInicio para formato YYYY-MM-DD (sem horário) para o payload
        const dataInicioFormatada = alocado.dataInicio 
          ? alocado.dataInicio.split('T')[0] 
          : '';
        
        // Converter dataFim - se existir, manter formato completo (YYYY-MM-DDTHH:mm:ss), senão usar dataInicio
        let dataFimFormatada = '';
        if (alocado.dataFim) {
          dataFimFormatada = alocado.dataFim;
        } else if (alocado.dataInicio) {
          // Se não tem dataFim, usar dataInicio como fallback
          dataFimFormatada = alocado.dataInicio;
        }
        
        // Converter prioridade para número binário (0 ou 1)
        const prioritarioBinario = alocado.prioridade === true ? 1 : (alocado.prioridade === false ? 0 : 0);
        
        const useCase = container.resolve(EditarAlocacaoUseCase);
        const response = await useCase.execute(token, {
          periodoAlocacaoId: rowId,
          dataInicio: dataInicioFormatada,
          dataFim: dataFimFormatada,
          quantidadeDeHoras: alocado.quantidadeDeHoras || 0,
          percentual: alocado.percentual || 0,
          oportunidade: alocado.oportunidade || "",
          observacao: alocado.observacao || "",
          prioritario: prioritarioBinario,
          incluiFimDeSemana: alocado.incluiFimDeSemana || null,
          flagRetroalimentaCV: alocado.retroalimentaCv || null,
          idPerfilAlocacao: alocado.perfil?.id || "",
          perfilSkills: [],
        });

        if (response.sucesso && response.AlocacaoEditada) {
          // Atualizar apenas a linha editada com os dados retornados
          setLocalAlocados((alocadosAnteriores) => {
            const indexAtualizado = alocadosAnteriores.findIndex(
              a => a.periodoAlocadoId === response.AlocacaoEditada.periodoAlocadoId
            );
            
            if (indexAtualizado !== -1) {
              const novosAlocados = [...alocadosAnteriores];
              // Garantir que prioridade seja sempre booleano
              const alocadoAtualizado = {
                ...response.AlocacaoEditada,
                prioridade: Boolean(response.AlocacaoEditada.prioridade)
              };
              novosAlocados[indexAtualizado] = alocadoAtualizado;
              return novosAlocados;
            }
            
            return alocadosAnteriores;
          });
          
          // Exibir toast de sucesso com mensagem específica para cada campo
          const mensagensSucesso: Record<string, string> = {
            inicio: "Data de início atualizada com sucesso.",
            fim: "Data final atualizada com sucesso.",
            horas: "Horas atualizadas com sucesso.",
            percentual: "Percentual atualizado com sucesso.",
            prioridade: "Prioridade atualizada com sucesso.",
            oportunidade: "Oportunidade atualizada com sucesso.",
            observacao: "Observação atualizada com sucesso.",
          };
          
          toast({
            title: "Sucesso",
            description: mensagensSucesso[columnId] || "Alocação atualizada com sucesso.",
            variant: "success",
          });
        } else {
          // Exibir erro em modal
          setErroMensagem(response.mensagem || "Erro ao editar alocação");
          setErroDialogOpen(true);
          // Reverter a mudança local em caso de erro
          setLocalAlocados((alocadosAnteriores) => {
            const novosAlocados = [...alocadosAnteriores];
            const indexOriginal = novosAlocados.findIndex(a => a.periodoAlocadoId === rowId);
            if (indexOriginal !== -1) {
              novosAlocados[indexOriginal] = { ...alocadoOriginal };
            }
            return novosAlocados;
          });
        }
      } catch (error) {
        const mensagemErro = error instanceof Error ? error.message : "Erro ao editar alocação";
        setErroMensagem(mensagemErro);
        setErroDialogOpen(true);
        // Reverter a mudança local em caso de erro
        setLocalAlocados((alocadosAnteriores) => {
          const novosAlocados = [...alocadosAnteriores];
          const indexOriginal = novosAlocados.findIndex(a => a.periodoAlocadoId === rowId);
          if (indexOriginal !== -1) {
            novosAlocados[indexOriginal] = { ...alocadoOriginal };
          }
          return novosAlocados;
        });
      } finally {
        setSubstituindo(false);
      }
    }

    // Limpar estados de edição
    setEditingCell(null);
    setEditingValue("");
    setColaboradorSelecionado(null);
    setColaboradorComboOpen(null);
    setProjetoSelecionado(null);
    setProjetoComboOpen(null);
  };

  const cancelarEdicao = () => {
    setEditingCell(null);
    setEditingValue("");
    setColaboradorSelecionado(null);
    setColaboradorComboOpen(null);
    setProjetoSelecionado(null);
    setProjetoComboOpen(null);
  };

  const getCellClassName = (columnId: string): string => {
    // Colunas roxas (Pode substituir)
    if (columnId === "colaborador" || columnId === "clienteProjeto") {
      return "bg-purple-50 dark:bg-purple-950/30 hover:bg-purple-100 dark:hover:bg-purple-900/50 cursor-pointer transition-colors relative";
    }
    
    // Colunas verdes (Pode editar)
    if (["perfil", "inicio", "fim", "horas", "percentual", "prioridade", "oportunidade", "observacao"].includes(columnId)) {
      return "bg-green-50 dark:bg-green-950/30 hover:bg-green-100 dark:hover:bg-green-900/50 cursor-pointer transition-colors relative";
    }
    
    return "";
  };

  const renderCell = (alocado: AlocacaoColaboradorTbd & Record<string, unknown>, columnId: string) => {
    const isEditing = editingCell?.rowId === alocado.periodoAlocadoId && editingCell?.columnId === columnId;
    const rowId = alocado.periodoAlocadoId;

    switch (columnId) {
      case "checkbox":
        const isSelected = selectedIds.has(rowId);
        return (
          <div onClick={(e) => e.stopPropagation()}>
            <Checkbox
              checked={isSelected}
              onCheckedChange={(checked) => {
                setSelectedIds(prev => {
                  const newSet = new Set(prev);
                  if (checked) {
                    newSet.add(rowId);
                  } else {
                    newSet.delete(rowId);
                  }
                  return newSet;
                });
              }}
            />
          </div>
        );
      case "cv":
        return (
          <div 
            className="cursor-pointer hover:opacity-80 transition-opacity"
            onClick={(e) => {
              e.stopPropagation();
              navigate(`/profile360?cpf=${encodeURIComponent(alocado.cpf)}`, {
                state: { 
                  from: '/mapa-alocacao',
                  busca: busca,
                  filtros: filtros
                }
              });
            }}
          >
            <Avatar className="h-8 w-8">
              <AvatarFallback className="bg-secondary text-secondary-foreground">
                <User className="h-4 w-4" />
              </AvatarFallback>
            </Avatar>
          </div>
        );
      case "colaborador":
        if (isEditing) {
          const isComboOpen = colaboradorComboOpen?.rowId === rowId;
          const colaboradorAtual = colaboradorSelecionado?.rowId === rowId ? colaboradorSelecionado.colaborador : null;
          
          return (
            <div 
              className="flex items-center gap-2 w-full min-w-[250px]"
              onKeyDown={(e) => {
                if (e.key === "Tab" && !e.shiftKey && !isComboOpen) {
                  e.preventDefault();
                  navegarParaProximoCampo(rowId, columnId);
                }
              }}
            >
              <Popover 
                open={isComboOpen} 
                onOpenChange={(open) => {
                  if (open) {
                    setColaboradorComboOpen({ rowId });
                    if (colaboradores.length === 0) {
                      carregarColaboradores();
                    }
                  } else {
                    setColaboradorComboOpen(null);
                  }
                }}
              >
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={isComboOpen}
                    className={cn(
                      "h-9 text-sm flex-1 justify-between min-w-[200px] border-2 border-primary",
                      !colaboradorAtual && "text-muted-foreground"
                    )}
                    onKeyDown={(e) => {
                      if (e.key === "Tab" && !e.shiftKey && !isComboOpen) {
                        e.preventDefault();
                        navegarParaProximoCampo(rowId, columnId);
                      }
                    }}
                  >
                    <span className="truncate text-left flex-1 mr-2">
                      {colaboradorAtual
                        ? colaboradorAtual.labelCodigoNome
                        : alocado.nomeColaborador || "Selecione colaborador..."}
                    </span>
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                  <Command>
                    <CommandInput placeholder="Buscar colaborador/TBD..." />
                    <CommandList>
                      <CommandEmpty>
                        {loadingColaboradores ? "Carregando..." : "Nenhum colaborador encontrado."}
                      </CommandEmpty>
                      <CommandGroup>
                        {colaboradores.map((colab, index) => (
                          <CommandItem
                            key={`colab-${colab.codProfissional}-${colab.cpf || index}`}
                            value={colab.labelCodigoNome}
                            onSelect={() => {
                              setColaboradorSelecionado({ rowId, colaborador: colab });
                              setEditingValue(colab.labelCodigoNome);
                              setColaboradorComboOpen(null);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                colaboradorAtual?.codProfissional === colab.codProfissional
                                  ? "opacity-100"
                                  : "opacity-0"
                              )}
                            />
                            <span className="flex-1">{colab.labelCodigoNome}</span>
                            {colab.ehTbd && (
                              <span className="ml-2 text-xs text-muted-foreground">(TBD)</span>
                            )}
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
              <div className="flex items-center gap-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    if (colaboradorAtual && !substituindo) {
                      salvarEdicao();
                    }
                  }}
                  title="Salvar"
                  disabled={!colaboradorAtual || substituindo}
                >
                  {substituindo ? (
                    <div className="h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />
                  ) : (
                    <Check className="h-4 w-4" />
                  )}
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    setColaboradorSelecionado(null);
                    cancelarEdicao();
                  }}
                  title="Cancelar"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        return (
          <div 
            className="flex items-center gap-2 font-medium w-full group"
            onClick={() => iniciarEdicao(rowId, columnId, alocado.nomeColaborador)}
          >
            <span className="flex-1">{alocado.nomeColaborador}</span>
            {alocado.tbd && <X className="h-4 w-4 text-muted-foreground" />}
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity" />
          </div>
        );
      case "departamento":
        return alocado.nomeDepartamento;
      case "gestorAdm":
        return alocado.nomeGestorAdm;
      case "clienteProjeto":
        if (isEditing) {
          const isComboOpen = projetoComboOpen?.rowId === rowId;
          const projetoAtual = projetoSelecionado?.rowId === rowId ? projetoSelecionado.projeto : null;
          
          return (
            <div 
              className="flex items-center gap-2 w-full min-w-[300px]"
              onKeyDown={(e) => {
                if (e.key === "Tab" && !e.shiftKey && !isComboOpen) {
                  e.preventDefault();
                  navegarParaProximoCampo(rowId, columnId);
                }
              }}
            >
              <Popover 
                open={isComboOpen} 
                onOpenChange={(open) => {
                  if (open) {
                    setProjetoComboOpen({ rowId });
                    if (projetos.length === 0) {
                      carregarProjetos();
                    }
                  } else {
                    setProjetoComboOpen(null);
                  }
                }}
              >
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={isComboOpen}
                    className={cn(
                      "h-9 text-sm flex-1 justify-between min-w-[250px] border-2 border-primary",
                      !projetoAtual && "text-muted-foreground"
                    )}
                    onKeyDown={(e) => {
                      if (e.key === "Tab" && !e.shiftKey && !isComboOpen) {
                        e.preventDefault();
                        navegarParaProximoCampo(rowId, columnId);
                      }
                    }}
                  >
                    <span className="truncate text-left flex-1 mr-2">
                      {projetoAtual
                        ? projetoAtual.projetos
                        : alocado.projeto?.labelClienteProjeto || "Selecione cliente/projeto..."}
                    </span>
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                  <Command>
                    <CommandInput placeholder="Buscar cliente/projeto..." />
                    <CommandList>
                      <CommandEmpty>
                        {loadingProjetos ? "Carregando..." : "Nenhum projeto encontrado."}
                      </CommandEmpty>
                      <CommandGroup>
                        {projetos.map((proj, index) => (
                          <CommandItem
                            key={`proj-${proj.codigoProjeto}-${proj.codigoCliente}-${index}`}
                            value={proj.projetos}
                            onSelect={() => {
                              setProjetoSelecionado({ rowId, projeto: proj });
                              setEditingValue(proj.projetos);
                              setProjetoComboOpen(null);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                projetoAtual?.codigoProjeto === proj.codigoProjeto && 
                                projetoAtual?.codigoCliente === proj.codigoCliente
                                  ? "opacity-100"
                                  : "opacity-0"
                              )}
                            />
                            <span className="flex-1">{proj.projetos}</span>
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
              <div className="flex items-center gap-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    if (projetoAtual && !substituindo) {
                      salvarEdicao();
                    }
                  }}
                  title="Salvar"
                  disabled={!projetoAtual || substituindo}
                >
                  {substituindo ? (
                    <div className="h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />
                  ) : (
                    <Check className="h-4 w-4" />
                  )}
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    setProjetoSelecionado(null);
                    cancelarEdicao();
                  }}
                  title="Cancelar"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        return (
          <div 
            className="max-w-xs truncate w-full flex items-center gap-2 group"
            title={alocado.projeto?.labelClienteProjeto || ""}
            onClick={() => iniciarEdicao(rowId, columnId, alocado.projeto?.labelClienteProjeto || "")}
          >
            <span className="flex-1 truncate">{alocado.projeto?.labelClienteProjeto || ""}</span>
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0" />
          </div>
        );
      case "gestorProjeto":
        const gestores = alocado.projeto?.gestoresProjeto?.map(g => g.nomeGestor).join(", ") || "";
        return (
          <div className="max-w-xs truncate" title={gestores}>
            {gestores}
          </div>
        );
      case "inicio":
        if (isEditing) {
          return (
            <div className="flex items-center gap-2 w-full min-w-[200px]">
              <Input
                ref={inputRef}
                value={editingValue || formatarData(alocado.dataInicio)}
                onChange={(e) => {
                  const valorFormatado = formatarDataInput(e.target.value);
                  setEditingValue(valorFormatado);
                }}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    salvarEdicao();
                  }
                  if (e.key === "Escape") {
                    e.preventDefault();
                    cancelarEdicao();
                  }
                  if (e.key === "Tab" && !e.shiftKey) {
                    e.preventDefault();
                    navegarParaProximoCampo(rowId, columnId);
                  }
                }}
                className="h-9 text-sm flex-1 min-w-[120px] border-2 border-primary focus:border-primary"
                placeholder="DD/MM/YYYY"
                maxLength={10}
                autoFocus
              />
              <div className="flex items-center gap-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    salvarEdicao();
                  }}
                  title="Salvar"
                >
                  <Check className="h-4 w-4" />
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    cancelarEdicao();
                  }}
                  title="Cancelar"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        return (
          <div 
            className="w-full flex items-center gap-2 group"
            onClick={() => iniciarEdicao(rowId, columnId, formatarData(alocado.dataInicio))}
          >
            <span className="flex-1">{formatarData(alocado.dataInicio)}</span>
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity" />
          </div>
        );
      case "fim":
        if (isEditing) {
          return (
            <div className="flex items-center gap-2 w-full min-w-[200px]">
              <Input
                ref={inputRef}
                value={editingValue || formatarData(alocado.dataFim)}
                onChange={(e) => {
                  const valorFormatado = formatarDataInput(e.target.value);
                  setEditingValue(valorFormatado);
                }}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    salvarEdicao();
                  }
                  if (e.key === "Escape") {
                    e.preventDefault();
                    cancelarEdicao();
                  }
                  if (e.key === "Tab" && !e.shiftKey) {
                    e.preventDefault();
                    navegarParaProximoCampo(rowId, columnId);
                  }
                }}
                className="h-9 text-sm flex-1 min-w-[120px] border-2 border-primary focus:border-primary"
                placeholder="DD/MM/YYYY"
                maxLength={10}
                autoFocus
              />
              <div className="flex items-center gap-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    salvarEdicao();
                  }}
                  title="Salvar"
                >
                  <Check className="h-4 w-4" />
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    cancelarEdicao();
                  }}
                  title="Cancelar"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        return (
          <div 
            className="w-full flex items-center gap-2 group"
            onClick={() => iniciarEdicao(rowId, columnId, formatarData(alocado.dataFim))}
          >
            <span className="flex-1">{formatarData(alocado.dataFim)}</span>
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity" />
          </div>
        );
      case "horas":
        if (isEditing) {
          return (
            <div className="flex items-center gap-2 w-full min-w-[150px]">
              <Input
                ref={inputRef}
                type="number"
                value={editingValue || alocado.quantidadeDeHoras?.toString() || "0"}
                onChange={(e) => setEditingValue(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    salvarEdicao();
                  }
                  if (e.key === "Escape") {
                    e.preventDefault();
                    cancelarEdicao();
                  }
                  if (e.key === "Tab" && !e.shiftKey) {
                    e.preventDefault();
                    navegarParaProximoCampo(rowId, columnId);
                  }
                }}
                className="h-9 text-sm flex-1 min-w-[80px] border-2 border-primary focus:border-primary"
                min="0"
                step="0.5"
                autoFocus
              />
              <div className="flex items-center gap-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    salvarEdicao();
                  }}
                  title="Salvar"
                >
                  <Check className="h-4 w-4" />
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    cancelarEdicao();
                  }}
                  title="Cancelar"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        return (
          <div 
            className="w-full flex items-center gap-2 group"
            onClick={() => iniciarEdicao(rowId, columnId, alocado.quantidadeDeHoras?.toString() || "0")}
          >
            <span className="flex-1">{alocado.quantidadeDeHoras?.toString() || "0"}</span>
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity" />
          </div>
        );
      case "percentual":
        if (isEditing) {
          const percentValue = editingValue ? editingValue.replace("%", "").trim() : (alocado.percentual || 0).toString();
          return (
            <div className="flex items-center gap-2 w-full min-w-[150px]">
              <div className="relative flex-1 min-w-[100px]">
                <Input
                  ref={inputRef}
                  type="number"
                  value={percentValue}
                  onChange={(e) => {
                    const val = e.target.value;
                    if (val === "" || (Number(val) >= 0 && Number(val) <= 100)) {
                      setEditingValue(val);
                    }
                  }}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      e.preventDefault();
                      salvarEdicao();
                    }
                    if (e.key === "Escape") {
                      e.preventDefault();
                      cancelarEdicao();
                    }
                    if (e.key === "Tab" && !e.shiftKey) {
                      e.preventDefault();
                      navegarParaProximoCampo(rowId, columnId);
                    }
                  }}
                  className="h-9 text-sm pr-8 border-2 border-primary focus:border-primary"
                  min="0"
                  max="100"
                  autoFocus
                />
                <span className="absolute right-3 top-1/2 -translate-y-1/2 text-sm text-muted-foreground pointer-events-none">%</span>
              </div>
              <div className="flex items-center gap-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    salvarEdicao();
                  }}
                  title="Salvar"
                >
                  <Check className="h-4 w-4" />
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    cancelarEdicao();
                  }}
                  title="Cancelar"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        return (
          <div 
            className="w-full flex items-center gap-2 group"
            onClick={() => iniciarEdicao(rowId, columnId, `${alocado.percentual || 0}`)}
          >
            <span className="flex-1">{`${alocado.percentual || 0}%`}</span>
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity" />
          </div>
        );
      case "statusProjeto":
        return (
          <span className="text-sm">
            {alocado.projeto?.statusProjeto || ""}
          </span>
        );
      case "tbd":
        return alocado.tbd ? (
          <span className="text-sm font-medium text-primary">Sim</span>
        ) : (
          <span className="text-sm text-muted-foreground">Não</span>
        );
      case "prioridade":
        if (isEditing) {
          return (
            <div className="flex items-center gap-2 w-full min-w-[150px]">
              <Select
                value={editingValue || (alocado.prioridade ? "true" : "false")}
                onValueChange={setEditingValue}
                onOpenChange={(open) => {
                  if (!open) {
                    // Quando fechar o select, verificar se Tab foi pressionado
                    // Mas isso é difícil de detectar, então vamos usar onKeyDown no container
                  }
                }}
              >
                <SelectTrigger 
                  className="h-9 text-sm flex-1 min-w-[100px] border-2 border-primary"
                  onKeyDown={(e) => {
                    if (e.key === "Tab" && !e.shiftKey) {
                      e.preventDefault();
                      navegarParaProximoCampo(rowId, columnId);
                    }
                    if (e.key === "Escape") {
                      e.preventDefault();
                      cancelarEdicao();
                    }
                  }}
                >
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="true">Sim</SelectItem>
                  <SelectItem value="false">Não</SelectItem>
                </SelectContent>
              </Select>
              <div className="flex items-center gap-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    salvarEdicao();
                  }}
                  title="Salvar"
                >
                  <Check className="h-4 w-4" />
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    cancelarEdicao();
                  }}
                  title="Cancelar"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        // Garantir que prioridade seja sempre um booleano válido
        // Se já for booleano, usar diretamente; caso contrário, converter
        const prioridadeChecked = typeof alocado.prioridade === 'boolean' 
          ? alocado.prioridade 
          : Boolean(alocado.prioridade === true || alocado.prioridade === 1 || alocado.prioridade === "1");
        
        return (
          <div 
            className="w-full flex items-center gap-2 group cursor-pointer"
            onClick={() => iniciarEdicao(rowId, columnId, prioridadeChecked ? "true" : "false")}
          >
            <Checkbox 
              checked={prioridadeChecked} 
              disabled 
              className={prioridadeChecked ? "data-[state=checked]:text-white" : ""}
            />
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity" />
          </div>
        );
      case "retroalimentaCv":
        return alocado.retroalimentaCv ? (
          <span className="text-sm font-medium text-green-600">Sim</span>
        ) : (
          <span className="text-sm text-muted-foreground">Não</span>
        );
      case "perfil":
        // Garantir que temos o objeto perfil original (não a string de ordenação)
        const perfilObj = (alocado as AlocacaoColaboradorTbd).perfil;
        const perfilNome = perfilObj?.perfil || "";
        return (
          <div 
            className="max-w-xs truncate w-full flex items-center gap-2 group bg-green-50 dark:bg-green-950/30 hover:bg-green-100 dark:hover:bg-green-900/50 cursor-pointer transition-colors px-2 py-1 rounded"
            title={perfilNome}
            onClick={() => {
              setPerfilModalRowId(rowId);
              setPerfilModalOpen(true);
              setPerfilSelecionado(null);
              setPerfilSearch("");
              // Carregar perfis do projeto
              if (token && alocado.projeto?.codProjeto) {
                carregarPerfis(alocado.projeto.codProjeto);
              }
            }}
          >
            <span className="flex-1 truncate">{perfilNome || "-"}</span>
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0" />
          </div>
        );
      case "oportunidade":
        if (isEditing) {
          return (
            <div className="flex items-center gap-2 w-full min-w-[200px]">
              <Input
                ref={inputRef}
                value={editingValue || alocado.oportunidade || ""}
                onChange={(e) => setEditingValue(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    salvarEdicao();
                  }
                  if (e.key === "Escape") {
                    e.preventDefault();
                    cancelarEdicao();
                  }
                  if (e.key === "Tab" && !e.shiftKey) {
                    e.preventDefault();
                    navegarParaProximoCampo(rowId, columnId);
                  }
                }}
                className="h-9 text-sm flex-1 min-w-[150px] border-2 border-primary focus:border-primary"
                autoFocus
              />
              <div className="flex items-center gap-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    salvarEdicao();
                  }}
                  title="Salvar"
                >
                  <Check className="h-4 w-4" />
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    cancelarEdicao();
                  }}
                  title="Cancelar"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        return (
          <div 
            className="max-w-xs truncate w-full flex items-center gap-2 group"
            title={alocado.oportunidade || ""}
            onClick={() => iniciarEdicao(rowId, columnId, alocado.oportunidade || "")}
          >
            <span className="flex-1 truncate">{alocado.oportunidade || "-"}</span>
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0" />
          </div>
        );
      case "observacao":
        if (isEditing) {
          return (
            <div className="flex items-start gap-2 w-full min-w-[250px]">
              <Textarea
                ref={textareaRef}
                value={editingValue || alocado.observacao || ""}
                onChange={(e) => setEditingValue(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Escape") {
                    e.preventDefault();
                    cancelarEdicao();
                  }
                  if (e.key === "Enter" && (e.ctrlKey || e.metaKey)) {
                    e.preventDefault();
                    salvarEdicao();
                  }
                  if (e.key === "Tab" && !e.shiftKey) {
                    e.preventDefault();
                    navegarParaProximoCampo(rowId, columnId);
                  }
                }}
                className="text-sm flex-1 min-w-[200px] border-2 border-primary focus:border-primary resize-none"
                rows={3}
                autoFocus
              />
              <div className="flex flex-col gap-1 pt-1 flex-shrink-0">
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-green-100 dark:hover:bg-green-900/50 hover:text-green-700 dark:hover:text-green-300" 
                  onClick={(e) => {
                    e.stopPropagation();
                    salvarEdicao();
                  }}
                  title="Salvar (Ctrl+Enter)"
                >
                  <Check className="h-4 w-4" />
                </Button>
                <Button 
                  size="icon" 
                  variant="ghost" 
                  className="h-8 w-8 hover:bg-red-100 hover:text-red-700" 
                  onClick={(e) => {
                    e.stopPropagation();
                    cancelarEdicao();
                  }}
                  title="Cancelar (Esc)"
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>
          );
        }
        return (
          <div 
            className="max-w-xs truncate w-full flex items-center gap-2 group"
            title={alocado.observacao || ""}
            onClick={() => iniciarEdicao(rowId, columnId, alocado.observacao || "")}
          >
            <span className="flex-1 truncate">{alocado.observacao || "-"}</span>
            <Edit2 className="h-3.5 w-3.5 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0" />
          </div>
        );
      case "habilidades":
        const habilidades = [
          ...(alocado.habilidadesAlocacao || []),
          ...(alocado.habilidadesTecnicas || [])
        ].join(", ");
        const perfilId = alocado.perfil?.id;
        const temPerfilId = perfilId && perfilId.trim() !== "";
        
        return (
          <div className="flex items-center gap-2">
            <div className="max-w-xs truncate" title={habilidades}>
              {habilidades || "-"}
            </div>
            {temPerfilId && (
              <Button
                variant="ghost"
                size="sm"
                className="h-7 px-2"
                onClick={(e) => {
                  e.stopPropagation();
                  setHabilidadesModalPerfilId(perfilId);
                  setHabilidadesModalOpen(true);
                  carregarSkillsPerfil(perfilId);
                }}
                title="Visualizar habilidades"
              >
                <Eye className="h-4 w-4" />
              </Button>
            )}
          </div>
        );
      case "habilidadesPerfil360":
        const codigoInternoColaborador = alocado.cpf;
        const temCodigoInterno = codigoInternoColaborador && codigoInternoColaborador.trim() !== "";
        
        return (
          <div className="flex items-center gap-2">
            <div className="max-w-xs truncate">
              {temCodigoInterno ? "Disponível" : "-"}
            </div>
            {temCodigoInterno && (
              <Button
                variant="ghost"
                size="sm"
                className="h-7 px-2"
                onClick={(e) => {
                  e.stopPropagation();
                  setHabilidadesPerfil360ModalCodigoInterno(codigoInternoColaborador);
                  setHabilidadesPerfil360ModalOpen(true);
                  setBuscaSkillsPerfil360("");
                  carregarSkillsPerfil360(codigoInternoColaborador);
                }}
                title="Visualizar habilidades Perfil 360"
              >
                <Eye className="h-4 w-4" />
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
      {/* Tabela */}
      <Card>
        <CardContent className="p-4 md:p-6">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
            <div className="flex items-center gap-4 flex-wrap">
              <div className="flex items-center gap-3">
                <h2 className="text-xl font-semibold">Alocações</h2>
                <span className="text-muted-foreground/80 text-xs font-normal flex items-center gap-2">
                  <span className="flex items-center gap-1.5">
                    <span className="w-3 h-3 rounded-sm bg-purple-100 dark:bg-purple-900/50 border border-purple-200 dark:border-purple-800" />
                    Pode substituir
                  </span>
                  <span className="flex items-center gap-1.5">
                    <span className="w-3 h-3 rounded-sm bg-green-100 dark:bg-green-900/50 border border-green-200 dark:border-green-800" />
                    Pode editar
                  </span>
                </span>
              </div>
              {selectedIds.size > 0 && (
                <div className="flex items-center gap-2 px-3 py-1.5 bg-destructive/10 border border-destructive/20 rounded-md">
                  <span className="text-sm font-medium text-destructive">
                    {selectedIds.size} {selectedIds.size === 1 ? 'item selecionado' : 'itens selecionados'}
                  </span>
                </div>
              )}
            </div>
            <div className="flex items-center gap-2 flex-wrap">
              <div className="relative flex-1 sm:w-80 min-w-[200px]">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Busca"
                  value={busca}
                  onChange={(e) => setBusca(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      handleBuscar();
                    }
                  }}
                  className="pl-10"
                />
              </div>
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
              {(busca.trim() || temFiltrosAplicados || localAlocados.length > 0) && (
                <Button 
                  variant="outline" 
                  className="gap-2"
                  onClick={handleLimparTabela}
                >
                  <X className="h-4 w-4" />
                  <span>Limpar</span>
                </Button>
              )}
              <Popover open={colunasVisibilidadeOpen} onOpenChange={setColunasVisibilidadeOpen}>
                <PopoverTrigger asChild>
                  <Button variant="outline" size="icon" className="gap-2">
                    <Table className="h-4 w-4" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-64 p-0" align="end">
                  <div className="p-4 border-b">
                    <div className="flex items-center gap-2">
                      <Table className="h-4 w-4 text-muted-foreground" />
                      <h4 className="font-medium text-sm">Mostrar colunas</h4>
                    </div>
                  </div>
                  <div className="p-2 overflow-y-auto" style={{ maxHeight: '300px' }}>
                    {allColumns
                      .filter(col => col.id !== "checkbox") // Não mostrar checkbox na lista
                      .map((column) => (
                        <div
                          key={column.id}
                          className="flex items-center gap-2 px-2 py-1.5 hover:bg-muted/50 rounded-sm cursor-pointer"
                        >
                          <Checkbox
                            checked={colunasVisiveis[column.id] === true}
                            onCheckedChange={(checked) => {
                              setColunasVisiveis(prev => {
                                const newValue = checked === true;
                                // Criar novo objeto para garantir re-render
                                const updated = {
                                  ...prev,
                                  [column.id]: newValue
                                };
                                // Forçar re-render retornando um novo objeto
                                return { ...updated };
                              });
                            }}
                            onClick={(e) => e.stopPropagation()}
                          />
                          <label className="text-sm cursor-pointer flex-1">
                            {column.label}
                          </label>
                        </div>
                      ))}
                  </div>
                </PopoverContent>
              </Popover>
              {selectedIds.size > 0 && (
                <Button
                  variant="destructive"
                  size="sm"
                  className="gap-2"
                  onClick={() => setModalExcluirOpen(true)}
                >
                  <Trash2 className="h-4 w-4" />
                  Excluir
                </Button>
              )}
            </div>
          </div>
          {loading ? (
            <div className="space-y-4">
              {/* Skeleton do cabeçalho da tabela */}
              <div className="flex items-center gap-4 border-b pb-4">
                <Skeleton className="h-10 w-32" />
                <Skeleton className="h-10 w-40" />
                <Skeleton className="h-10 w-48" />
                <Skeleton className="h-10 w-36" />
                <Skeleton className="h-10 w-44" />
                <Skeleton className="h-10 w-40" />
              </div>
              {/* Skeleton das linhas */}
              {[...Array(8)].map((_, index) => (
                <div key={index} className="flex items-center gap-4 py-3 border-b">
                  <Skeleton className="h-8 w-8" />
                  <Skeleton className="h-8 w-8" />
                  <Skeleton className="h-10 w-48" />
                  <Skeleton className="h-10 w-40" />
                  <Skeleton className="h-10 w-48" />
                  <Skeleton className="h-10 w-36" />
                  <Skeleton className="h-10 w-44" />
                  <Skeleton className="h-10 w-40" />
                </div>
              ))}
            </div>
          ) : error ? (
            <div className="text-center py-8 text-destructive">{error}</div>
          ) : (
            <>
              <DataTable
                columns={columns}
                data={paginatedAlocados}
                keyExtractor={(item) => {
                  // Usar uma combinação de IDs para garantir unicidade
                  const periodoId = item.periodoAlocadoId || 0;
                  const colaboradorId = item.codColaborador || '';
                  const colaboradorAlocadoId = item.colaboradorAlocadoId || '';
                  const dataInicio = item.dataInicio || '';
                  return `aloc-${periodoId}-${colaboradorId}-${colaboradorAlocadoId}-${dataInicio}`;
                }}
                renderCell={renderCell}
                getCellClassName={getCellClassName}
                emptyMessage="Nenhuma alocação encontrada. Use o campo de busca ou os filtros para encontrar alocações."
                stickyLeftColumnIds={["checkbox", "cv", "colaborador"]}
                dense
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

      {/* Filters Sidebar */}
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
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, unidade: open }))}
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
                    value={filtros.codigoUnidade?.toString() || "todos"} 
                    onValueChange={(value) => {
                      const newFiltros = { ...filtros, codigoUnidade: value === "todos" ? null : value };
                      setFiltros(newFiltros);
                      // Recarregar departamentos e gestores quando unidade mudar (de forma não-bloqueante)
                      if (token) {
                        // Usar setTimeout para não bloquear a UI
                        setTimeout(() => {
                          (async () => {
                            try {
                              // Recarregar departamentos filtrados por unidade
                              const listarDepartamentosUseCase = container.resolve(ListarDepartamentosUseCase);
                              const departamentosResponse = await listarDepartamentosUseCase.execute(token, {
                                codigoDiretoria: newFiltros.codigoUnidade || undefined,
                              });
                              const departamentosData = departamentosResponse.retorno || [];
                              // Remover duplicatas de forma eficiente usando Set
                              const seenDept = new Set<string>();
                              const departamentosUnicos = departamentosData.filter((dept) => {
                                if (seenDept.has(dept.cod)) return false;
                                seenDept.add(dept.cod);
                                return true;
                              });
                              setDepartamentos(departamentosUnicos);

                              // Recarregar gestores filtrados por unidade
                              const listarNomesGestoresUseCase = container.resolve(ListarNomesGestoresUseCase);
                              const gestoresResponse = await listarNomesGestoresUseCase.execute(token, {
                                codDiretoria: newFiltros.codigoUnidade ? Number(newFiltros.codigoUnidade) : undefined,
                                codDepartamento: newFiltros.codigoDepartamento || undefined,
                              });
                              const gestoresData = gestoresResponse.Gestor || [];
                              const gestoresUnicos = gestoresData.filter((gestor, index, self) => 
                                index === self.findIndex((g) => g.codigoProfissional === gestor.codigoProfissional)
                              );
                              setGestoresAdm(gestoresUnicos);
                              setGestoresProjeto(gestoresUnicos);

                              // Recarregar colaboradores com novos filtros
                              await carregarColaboradoresFiltro(newFiltros);
                            } catch (error) {
                              console.error("Erro ao recarregar dados:", error);
                            }
                          })();
                        }, 0);
                      }
                    }}
                    disabled={loadingFiltros}
                  >
                    <SelectTrigger className="flex-1">
                      <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione.."} />
                    </SelectTrigger>
                    <SelectContent className="max-h-[300px] z-[100]">
                      <SelectItem value="todos">Todos</SelectItem>
                      {unidades.map((unidade) => (
                        <SelectItem key={`unidade-${unidade.id}`} value={unidade.id}>
                          {unidade.descricao}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {filtros.codigoUnidade && (
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      className="h-10 w-10 shrink-0"
                      onClick={() => {
                        const newFiltros = { ...filtros, codigoUnidade: null };
                        setFiltros(newFiltros);
                        // Recarregar dados quando limpar unidade
                        if (token) {
                          // Recarregar departamentos sem filtro
                          const listarDepartamentosUseCase = container.resolve(ListarDepartamentosUseCase);
                          listarDepartamentosUseCase.execute(token, {}).then(response => {
                            const departamentosData = response.retorno || [];
                            const departamentosUnicos = departamentosData.filter((dept, index, self) => 
                              index === self.findIndex((d) => d.cod === dept.cod)
                            );
                            setDepartamentos(departamentosUnicos);
                          }).catch(console.error);

                          // Recarregar gestores sem filtro
                          const listarNomesGestoresUseCase = container.resolve(ListarNomesGestoresUseCase);
                          listarNomesGestoresUseCase.execute(token, {}).then(response => {
                            const gestoresData = response.Gestor || [];
                            const gestoresUnicos = gestoresData.filter((gestor, index, self) => 
                              index === self.findIndex((g) => g.codigoProfissional === gestor.codigoProfissional)
                            );
                            setGestoresAdm(gestoresUnicos);
                            setGestoresProjeto(gestoresUnicos);
                          }).catch(console.error);

                          // Recarregar colaboradores
                          carregarColaboradoresFiltro(newFiltros).catch(console.error);
                        }
                      }}
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              </CollapsibleContent>
            </Collapsible>

            {/* Departamento */}
            <Collapsible 
              open={filtrosExpandidos.departamento}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, departamento: open }))}
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="business" size={20} className="text-muted-foreground" />
                  <span>Departamento</span>
                </div>
                {filtrosExpandidos.departamento ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <div className="flex items-center gap-2">
                  <Select 
                    value={filtros.codigoDepartamento || "todos"} 
                    onValueChange={(value) => {
                      const newFiltros = { ...filtros, codigoDepartamento: value === "todos" ? null : value };
                      setFiltros(newFiltros);
                      // Recarregar gestores e colaboradores quando departamento mudar (de forma não-bloqueante)
                      if (token) {
                        setTimeout(() => {
                          (async () => {
                            try {
                              // Recarregar gestores filtrados por departamento
                              const listarNomesGestoresUseCase = container.resolve(ListarNomesGestoresUseCase);
                              const gestoresResponse = await listarNomesGestoresUseCase.execute(token, {
                                codDiretoria: newFiltros.codigoUnidade ? Number(newFiltros.codigoUnidade) : undefined,
                                codDepartamento: newFiltros.codigoDepartamento || undefined,
                              });
                              const gestoresData = gestoresResponse.Gestor || [];
                              // Remover duplicatas de forma eficiente usando Set
                              const seenGestor = new Set<string>();
                              const gestoresUnicos = gestoresData.filter((gestor) => {
                                if (seenGestor.has(gestor.codigoProfissional)) return false;
                                seenGestor.add(gestor.codigoProfissional);
                                return true;
                              });
                              setGestoresAdm(gestoresUnicos);
                              setGestoresProjeto(gestoresUnicos);

                              // Recarregar colaboradores filtrados por departamento
                              await carregarColaboradoresFiltro(newFiltros);
                            } catch (error) {
                              console.error("Erro ao recarregar dados:", error);
                            }
                          })();
                        }, 0);
                      }
                    }}
                    disabled={loadingFiltros}
                  >
                    <SelectTrigger className="flex-1">
                      <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione.."} />
                    </SelectTrigger>
                    <SelectContent className="max-h-[300px] z-[100]">
                      <SelectItem value="todos">Todos</SelectItem>
                      {departamentos.map((dept) => (
                        <SelectItem key={`departamento-${dept.cod}`} value={dept.cod}>
                          {dept.departamento}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {filtros.codigoDepartamento && (
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      className="h-10 w-10 shrink-0"
                      onClick={() => {
                        const newFiltros = { ...filtros, codigoDepartamento: null };
                        setFiltros(newFiltros);
                        // Recarregar gestores e colaboradores quando limpar departamento
                        if (token) {
                          const listarNomesGestoresUseCase = container.resolve(ListarNomesGestoresUseCase);
                          listarNomesGestoresUseCase.execute(token, {
                            codDiretoria: newFiltros.codigoUnidade ? Number(newFiltros.codigoUnidade) : undefined,
                          }).then(response => {
                            const gestoresData = response.Gestor || [];
                            const gestoresUnicos = gestoresData.filter((gestor, index, self) => 
                              index === self.findIndex((g) => g.codigoProfissional === gestor.codigoProfissional)
                            );
                            setGestoresAdm(gestoresUnicos);
                            setGestoresProjeto(gestoresUnicos);
                          }).catch(console.error);

                          carregarColaboradoresFiltro(newFiltros).catch(console.error);
                        }
                      }}
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              </CollapsibleContent>
            </Collapsible>

            {/* Gestor Adm */}
            <Collapsible 
              open={filtrosExpandidos.gestorAdm}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, gestorAdm: open }))}
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="person" size={20} className="text-muted-foreground" />
                  <span>Gestor Adm</span>
                </div>
                {filtrosExpandidos.gestorAdm ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select 
                  value={filtros.codigoGestorAdm || "todos"} 
                    onValueChange={(value) => {
                      const newFiltros = { ...filtros, codigoGestorAdm: value === "todos" ? null : value };
                      setFiltros(newFiltros);
                      // Recarregar colaboradores quando gestor adm mudar (de forma não-bloqueante)
                      if (token) {
                        setTimeout(() => {
                          carregarColaboradoresFiltro(newFiltros).catch(console.error);
                        }, 0);
                      }
                    }}
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione.."} />
                  </SelectTrigger>
                  <SelectContent className="max-h-[300px] z-[100]">
                    <SelectItem value="todos">Todos</SelectItem>
                    {gestoresAdm.map((gestor) => (
                      <SelectItem key={`gestor-adm-${gestor.codigoProfissional}`} value={gestor.codigoProfissional}>
                        {gestor.nome}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>

            {/* Tipo Colaborador */}
            <Collapsible 
              open={filtrosExpandidos.tipoColaborador}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, tipoColaborador: open }))}
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="groups" size={20} className="text-muted-foreground" />
                  <span>Tipo Colaborador</span>
                </div>
                {filtrosExpandidos.tipoColaborador ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <div className="flex items-center gap-2">
                  <Select 
                    value={filtros.filtroTipoProfissional?.toString() || "0"} 
                    onValueChange={(value) => {
                      const newFiltros = { ...filtros, filtroTipoProfissional: Number(value) };
                      setFiltros(newFiltros);
                      // Recarregar colaboradores quando tipo colaborador mudar (de forma não-bloqueante)
                      if (token) {
                        setTimeout(() => {
                          carregarColaboradoresFiltro(newFiltros).catch(console.error);
                        }, 0);
                      }
                    }}
                    disabled={loadingFiltros}
                  >
                    <SelectTrigger className="flex-1">
                      <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione.."} />
                    </SelectTrigger>
                    <SelectContent className="max-h-[300px] z-[100]">
                      <SelectItem value="0">Todos</SelectItem>
                      <SelectItem value="1">Colaborador</SelectItem>
                      <SelectItem value="2">TBD</SelectItem>
                    </SelectContent>
                  </Select>
                  {filtros.filtroTipoProfissional && filtros.filtroTipoProfissional !== 0 && (
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      className="h-10 w-10 shrink-0"
                      onClick={() => {
                        const newFiltros = { ...filtros, filtroTipoProfissional: 0 };
                        setFiltros(newFiltros);
                        if (token) {
                          carregarColaboradoresFiltro(newFiltros).catch(console.error);
                        }
                      }}
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              </CollapsibleContent>
            </Collapsible>

            {/* Colaborador/TBD */}
            <Collapsible 
              open={filtrosExpandidos.colaboradorTbd}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, colaboradorTbd: open }))}
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
              <CollapsibleContent className="pt-2 px-3 space-y-2">
                <Select 
                  value="" 
                  onValueChange={(value) => {
                    if (value && value !== "todos") {
                      const colaborador = colaboradoresFiltro.find(c => c.codProfissional === value);
                      if (colaborador && !colaboradoresSelecionados.find(c => c.codProfissional === colaborador.codProfissional)) {
                        setColaboradoresSelecionados([...colaboradoresSelecionados, colaborador]);
                        setFiltros({ 
                          ...filtros, 
                          listaCodigoColabOuTbd: [...(filtros.listaCodigoColabOuTbd || []), colaborador.codProfissional]
                        });
                      }
                    }
                  }}
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione um colaborador..."} />
                  </SelectTrigger>
                  <SelectContent className="max-h-[300px] z-[100]">
                    {colaboradoresDisponiveis.map((colab) => (
                      <SelectItem key={`colaborador-${colab.codProfissional}`} value={colab.codProfissional}>
                        {colab.labelCodigoNome}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {colaboradoresSelecionados.length > 0 && (
                  <div className="flex flex-wrap gap-2 pt-2">
                    {colaboradoresSelecionados.map((colab) => (
                      <Badge 
                        key={`tag-colaborador-${colab.codProfissional}`} 
                        variant="outline" 
                        className="px-3 py-1.5 text-sm bg-white border-border"
                      >
                        {colab.labelCodigoNome}
                        <button
                          type="button"
                          onClick={() => {
                            const novosColaboradores = colaboradoresSelecionados.filter(c => c.codProfissional !== colab.codProfissional);
                            setColaboradoresSelecionados(novosColaboradores);
                            setFiltros({ 
                              ...filtros, 
                              listaCodigoColabOuTbd: novosColaboradores.map(c => c.codProfissional)
                            });
                          }}
                          className="ml-2 hover:text-destructive"
                        >
                          <X className="h-3 w-3" />
                        </button>
                      </Badge>
                    ))}
                  </div>
                )}
              </CollapsibleContent>
            </Collapsible>

            {/* Gestor de Projeto */}
            <Collapsible 
              open={filtrosExpandidos.gestorProjeto}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, gestorProjeto: open }))}
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="person" size={20} className="text-muted-foreground" />
                  <span>Gestor de Projeto</span>
                </div>
                {filtrosExpandidos.gestorProjeto ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select
                  value={filtros.codigoGestorProjeto || "todos"}
                  onValueChange={(value) => {
                    const newFiltros = { ...filtros, codigoGestorProjeto: value === "todos" ? null : value };
                    setFiltros(newFiltros);
                    // Recarregar clientes quando gestor de projeto mudar (de forma não-bloqueante)
                    if (token) {
                      setTimeout(() => {
                        (async () => {
                          try {
                            const projetosApi = container.resolve<ProjetosApi>(DiTokens.projetosApi);
                            const clientesData = await projetosApi.listarClientesOrg(token, {
                              codigoClienteFiltro: '',
                              codigoGerenteProjeto: newFiltros.codigoGestorProjeto || undefined,
                            });
                            const clientesArray = Array.isArray(clientesData) ? clientesData : [];
                            // Remover duplicatas de forma eficiente usando Set
                            const seenClientes = new Set<string>();
                            const clientesUnicos = clientesArray.filter((cliente) => {
                              if (seenClientes.has(cliente.codigoCliente)) return false;
                              seenClientes.add(cliente.codigoCliente);
                              return true;
                            });
                            setClientes(clientesUnicos);
                          } catch (error) {
                            console.error("Erro ao recarregar clientes:", error);
                          }
                        })();
                      }, 0);
                    }
                  }}
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione.."} />
                  </SelectTrigger>
                  <SelectContent className="max-h-[300px] z-[100]">
                    <SelectItem value="todos">Todos</SelectItem>
                    {gestoresProjeto.map((gestor) => (
                      <SelectItem key={`gestor-projeto-${gestor.codigoProfissional}`} value={gestor.codigoProfissional}>
                        {gestor.nome}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>

            {/* Clientes */}
            <Collapsible 
              open={filtrosExpandidos.clientes}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, clientes: open }))}
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="groups" size={20} className="text-muted-foreground" />
                  <span>Clientes</span>
                </div>
                {filtrosExpandidos.clientes ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 space-y-2">
                <Select 
                  value="" 
                  onValueChange={(value) => {
                    if (value && value !== "todos") {
                      const cliente = clientes.find(c => c.codigoCliente === value);
                      if (cliente && !clientesSelecionados.find(c => c.codigoCliente === cliente.codigoCliente)) {
                        setClientesSelecionados([...clientesSelecionados, cliente]);
                        setFiltros({ 
                          ...filtros, 
                          listaCodigoClientes: [...(filtros.listaCodigoClientes || []), cliente.codigoCliente]
                        });
                      }
                    }
                  }}
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione um cliente..."} />
                  </SelectTrigger>
                  <SelectContent className="max-h-[300px] z-[100]">
                    {clientesDisponiveis.map((cliente) => (
                      <SelectItem key={`cliente-${cliente.codigoCliente}`} value={cliente.codigoCliente}>
                        {cliente.labelCodigoCliente}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {clientesSelecionados.length > 0 && (
                  <div className="flex flex-wrap gap-2 pt-2">
                    {clientesSelecionados.map((cliente) => (
                      <Badge 
                        key={`tag-cliente-${cliente.codigoCliente}`} 
                        variant="outline" 
                        className="px-3 py-1.5 text-sm bg-white border-border"
                      >
                        {cliente.labelCodigoCliente}
                        <button
                          type="button"
                          onClick={() => {
                            const novosClientes = clientesSelecionados.filter(c => c.codigoCliente !== cliente.codigoCliente);
                            setClientesSelecionados(novosClientes);
                            setFiltros({ 
                              ...filtros, 
                              listaCodigoClientes: novosClientes.map(c => c.codigoCliente)
                            });
                          }}
                          className="ml-2 hover:text-destructive"
                        >
                          <X className="h-3 w-3" />
                        </button>
                      </Badge>
                    ))}
                  </div>
                )}
              </CollapsibleContent>
            </Collapsible>

            {/* Status Projeto */}
            <Collapsible 
              open={filtrosExpandidos.statusProjeto}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, statusProjeto: open }))}
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="checklist" size={20} className="text-muted-foreground" />
                  <span>Status Projeto</span>
                </div>
                {filtrosExpandidos.statusProjeto ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select 
                  value={filtros.statusProjeto || "todos"} 
                  onValueChange={(value) => setFiltros({ ...filtros, statusProjeto: value === "todos" ? null : value })}
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione.."} />
                  </SelectTrigger>
                  <SelectContent className="max-h-[300px] z-[100]">
                    <SelectItem value="todos">Todos</SelectItem>
                    {statusProjeto.map((status) => (
                      <SelectItem key={`status-projeto-${status.codigoStatusProjeto}`} value={status.nomeStatusProjeto}>
                        {status.nomeStatusProjeto}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>

            {/* Prioridade */}
            <Collapsible 
              open={filtrosExpandidos.prioridade}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, prioridade: open }))}
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="flag" size={20} className="text-muted-foreground" />
                  <span>Prioridade</span>
                </div>
                {filtrosExpandidos.prioridade ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <div className="flex items-center gap-2">
                  <Select 
                    value={filtros.filtroPrioridade?.toString() || "0"} 
                    onValueChange={(value) => {
                      const prioridadeValue = Number(value);
                      setFiltros({ 
                        ...filtros, 
                        filtroPrioridade: prioridadeValue,
                        // Manter compatibilidade com apenasProjetosPrioritarios
                        apenasProjetosPrioritarios: prioridadeValue === 1 ? true : (prioridadeValue === 2 ? false : undefined)
                      });
                    }}
                    disabled={loadingFiltros}
                  >
                    <SelectTrigger className="flex-1">
                      <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione.."} />
                    </SelectTrigger>
                    <SelectContent className="max-h-[300px] z-[100]">
                      <SelectItem value="0">Todos</SelectItem>
                      <SelectItem value="1">Prioritários</SelectItem>
                      <SelectItem value="2">Não prioritários</SelectItem>
                    </SelectContent>
                  </Select>
                  {filtros.filtroPrioridade && filtros.filtroPrioridade !== 0 && (
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      className="h-10 w-10 shrink-0"
                      onClick={() => {
                        setFiltros({ 
                          ...filtros, 
                          filtroPrioridade: 0,
                          apenasProjetosPrioritarios: false
                        });
                      }}
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              </CollapsibleContent>
            </Collapsible>

            {/* Projeto */}
            <Collapsible 
              open={filtrosExpandidos.projeto}
              onOpenChange={(open) => setFiltrosExpandidos(prev => ({ ...prev, projeto: open }))}
            >
              <CollapsibleTrigger className="flex items-center justify-between w-full py-3 px-3 text-sm font-medium hover:bg-muted/50 rounded-lg">
                <div className="flex items-center gap-3">
                  <Icon name="folder" size={20} className="text-muted-foreground" />
                  <span>Projeto</span>
                </div>
                {filtrosExpandidos.projeto ? (
                  <ChevronUp className="h-4 w-4 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-4 w-4 text-muted-foreground" />
                )}
              </CollapsibleTrigger>
              <CollapsibleContent className="pt-2 px-3 overflow-hidden">
                <Select 
                  value={filtros.listaCodigoProjetos && filtros.listaCodigoProjetos.length > 0 
                    ? filtros.listaCodigoProjetos[0] 
                    : "todos"} 
                  onValueChange={(value) => setFiltros({ 
                    ...filtros, 
                    listaCodigoProjetos: value === "todos" ? [] : [value] 
                  })}
                  disabled={loadingFiltros}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={loadingFiltros ? "Carregando.." : "Selecione.."} />
                  </SelectTrigger>
                  <SelectContent className="max-h-[300px] z-[100]">
                    <SelectItem value="todos">Todos</SelectItem>
                    {projetosFiltro.map((projeto) => (
                      <SelectItem key={`projeto-${projeto.codigoProjeto}`} value={projeto.codigoProjeto}>
                        {projeto.projetos}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </CollapsibleContent>
            </Collapsible>
          </div>

          <SheetFooter className="px-6 py-4 border-t border-border gap-2 flex-row">
            <Button 
              variant="outline" 
              onClick={() => {
                handleLimpar();
                setFiltersOpen(false);
              }}
              className="flex-1"
            >
              Limpar
            </Button>
            <Button 
              onClick={() => {
                setFiltersOpen(false);
                handleAplicar();
              }}
              className="flex-1"
            >
              Aplicar
            </Button>
          </SheetFooter>
        </SheetContent>
      </Sheet>

      {/* Dialog de Erro */}
      <Dialog open={erroDialogOpen} onOpenChange={setErroDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Erro ao substituir</DialogTitle>
            <DialogDescription>
              {erroMensagem}
            </DialogDescription>
          </DialogHeader>
          <div className="flex justify-end mt-4">
            <Button onClick={() => setErroDialogOpen(false)}>
              Fechar
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal de Confirmação de Exclusão */}
      <AlertDialog open={modalExcluirOpen} onOpenChange={(open) => {
        if (!excluindo) {
          setModalExcluirOpen(open);
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar exclusão</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja excluir {selectedIds.size} {selectedIds.size === 1 ? 'alocação selecionada' : 'alocações selecionadas'}? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={excluindo}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={async () => {
                await handleExcluirAlocacoes();
              }}
              disabled={excluindo}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {excluindo ? "Excluindo..." : "Confirmar exclusão"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal de Seleção de Perfil */}
      <Dialog open={perfilModalOpen} onOpenChange={setPerfilModalOpen}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Selecionar Perfil</DialogTitle>
            <DialogDescription>
              Selecione um perfil para esta alocação
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar perfil..."
                value={perfilSearch}
                onChange={(e) => setPerfilSearch(e.target.value)}
                className="pl-9"
              />
            </div>
            <div className="max-h-[400px] overflow-y-auto border rounded-md">
              {loadingPerfis ? (
                <div className="p-4 text-center text-muted-foreground">
                  Carregando perfis...
                </div>
              ) : perfisFiltrados.length === 0 ? (
                <div className="p-4 text-center text-muted-foreground">
                  {perfilSearch.trim() ? "Nenhum perfil encontrado" : "Nenhum perfil disponível"}
                </div>
              ) : (
                <div className="divide-y">
                  {perfisFiltrados.map((perfil) => (
                    <div
                      key={perfil.id}
                      className={cn(
                        "p-3 cursor-pointer hover:bg-muted transition-colors",
                        perfilSelecionado?.id === perfil.id && "bg-primary/10"
                      )}
                      onClick={() => setPerfilSelecionado(perfil)}
                    >
                      <div className="flex items-center gap-2">
                        <Check
                          className={cn(
                            "h-4 w-4",
                            perfilSelecionado?.id === perfil.id ? "opacity-100 text-primary" : "opacity-0"
                          )}
                        />
                        <span className="text-sm">{perfil.perfil}</span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
          <div className="flex justify-end gap-2 mt-4">
            <Button
              variant="outline"
              onClick={() => {
                setPerfilModalOpen(false);
                setPerfilModalRowId(null);
                setPerfilSelecionado(null);
                setPerfilSearch("");
                setPerfisDisponiveis([]);
              }}
            >
              Cancelar
            </Button>
            <Button
              onClick={salvarPerfil}
              disabled={!perfilSelecionado || substituindo}
            >
              {substituindo ? "Salvando..." : "Confirmar"}
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal de Habilidades */}
      <Dialog open={habilidadesModalOpen} onOpenChange={setHabilidadesModalOpen}>
        <DialogContent className="max-w-3xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <div className="flex items-center gap-2">
              <Icon name="lightbulb" size={24} className="text-muted-foreground" />
              <DialogTitle>Habilidades Alocação</DialogTitle>
            </div>
            <DialogDescription>
              Visualize as habilidades associadas ao perfil desta alocação
            </DialogDescription>
          </DialogHeader>
          
          {loadingSkills ? (
            <div className="flex items-center justify-center py-8">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
            </div>
          ) : skillsPerfil.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              Nenhuma habilidade encontrada para este perfil.
            </div>
          ) : (
            <div className="flex flex-wrap gap-2 py-4">
              {skillsPerfil.map((skillItem, index) => (
                <Badge
                  key={`skill-${skillItem.skill.id}-${index}`}
                  variant="outline"
                  className="px-3 py-1.5 text-sm bg-white border-border flex items-center gap-1.5"
                >
                  {skillItem.relevante && (
                    <Star className="h-3.5 w-3.5 text-yellow-500 fill-yellow-500" />
                  )}
                  <span>
                    {skillItem.skill.descricao} ({skillItem.nivel.descricao})
                  </span>
                </Badge>
              ))}
            </div>
          )}
        </DialogContent>
      </Dialog>

      {/* Modal de Habilidades Perfil 360 */}
      <Dialog open={habilidadesPerfil360ModalOpen} onOpenChange={setHabilidadesPerfil360ModalOpen}>
        <DialogContent className="max-w-3xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <div className="flex items-center gap-2">
              <Icon name="lightbulb" size={24} className="text-muted-foreground" />
              <DialogTitle>Habilidades Perfil 360</DialogTitle>
            </div>
            <DialogDescription>
              Visualize as habilidades do colaborador no Perfil 360
            </DialogDescription>
          </DialogHeader>
          
          <div className="space-y-4">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar habilidades..."
                value={buscaSkillsPerfil360}
                onChange={(e) => setBuscaSkillsPerfil360(e.target.value)}
                className="pl-9"
              />
            </div>
            
            {loadingSkillsPerfil360 ? (
              <div className="flex items-center justify-center py-8">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
              </div>
            ) : skillsPerfil360Filtradas.length === 0 ? (
              <div className="text-center py-8 text-muted-foreground">
                {buscaSkillsPerfil360.trim() 
                  ? "Nenhuma habilidade encontrada com o termo buscado."
                  : "Nenhuma habilidade encontrada para este colaborador."}
              </div>
            ) : (
              <div className="flex flex-wrap gap-2 py-4">
                {skillsPerfil360Filtradas.map((skill) => (
                  <Badge
                    key={`skill-perfil360-${skill.id}`}
                    variant="outline"
                    className="px-3 py-1.5 text-sm bg-white border-border"
                  >
                    <span>
                      {skill.descricao} ({skill.nivel})
                    </span>
                  </Badge>
                ))}
              </div>
            )}
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
};
