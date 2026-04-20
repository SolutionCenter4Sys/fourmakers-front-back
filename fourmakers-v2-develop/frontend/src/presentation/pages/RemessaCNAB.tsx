import { useState, useEffect, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { Card, CardContent } from "@/components/ui/card";
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
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
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
  ArrowLeft,
  Download,
  Upload,
  Eye,
  FileText,
  Search,
  ChevronRight,
  Maximize2,
  Minimize2,
  Loader2,
  Check,
  ChevronsUpDown,
} from "@/components/ui/system-icons";
import { format } from "date-fns";
import { cn } from "@/lib/utils";
import {
  DataTable,
  PageBreadcrumb,
  PageHeader,
  TablePagination,
} from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { formatCurrency } from "@shared/utils/calculations";
import { StatusBadge } from "@presentation/components/common/StatusBadge";
import { useToast } from "@/hooks/use-toast";
import { useIntegracaoBancaria } from "@/hooks/useIntegracaoBancaria";
import { useAppSelector } from "@app/store/hooks";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { container } from "@core/di/container";
import { ListarDiretoriasDisponiveisUseCase } from "@domain/usecases/ListarDiretoriasDisponiveisUseCase";
import type { Diretoria } from "@domain/entities/Diretoria";
import type {
  RemessaCnab,
  LancamentoRemessa,
  SolicitacaoRemessaCNAB,
  FormaPagamento,
  StatusRemessa,
} from "@data/api/IntegracaoBancariaApi";

export default function RemessaCNAB() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const token =
    useAppSelector((state) => state.auth.token) ||
    localStorage.getItem("authToken");
  const {
    solicitacoes,
    remessas,
    loading,
    error,
    modulosRemessa,
    tipoRemessa,
    setTipoRemessa,
    processarRemessa,
    processarRetorno,
    refresh,
    loadRemessas,
    loadSolicitacoes,
  } = useIntegracaoBancaria();

  const [buscaSolicitacoes, setBuscaSolicitacoes] = useState("");
  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
  const [gridExpandido, setGridExpandido] = useState<
    "esquerdo" | "direito" | "ambos"
  >("ambos");
  const [currentPageSolicitacoes, setCurrentPageSolicitacoes] = useState(1);
  const [itemsPerPageSolicitacoes, setItemsPerPageSolicitacoes] = useState(10);
  const [currentPageRemessas, setCurrentPageRemessas] = useState(1);
  const [itemsPerPageRemessas, setItemsPerPageRemessas] = useState(10);
  
  // Filtros para Solicitações (frontend)
  const [competenciaSolicitacoes, setCompetenciaSolicitacoes] = useState<string>("");
  const [mesSolicitacoes, setMesSolicitacoes] = useState<string>("");
  const [anoSolicitacoes, setAnoSolicitacoes] = useState<string>("");
  const [formaPagamentoFiltro, setFormaPagamentoFiltro] = useState<FormaPagamento | "">("");

  // Filtros para Remessas Geradas
  const [competenciaRemessas, setCompetenciaRemessas] = useState<string>("");
  const [statusRemessa, setStatusRemessa] = useState<StatusRemessa | "">("");


  // Estados para seletores de mês/ano (Remessas)
  const [mesRemessas, setMesRemessas] = useState<string>("");
  const [anoRemessas, setAnoRemessas] = useState<string>("");

  // Handler para mudança de diretoria
  const handleDiretoriaChange = (diretoria: Diretoria | null) => {
    setDiretoriaSelecionada(diretoria);
    // Recarregar solicitações com a nova diretoria
    if (diretoria) {
      const codDiretoria = diretoria.configuracaoParaTodaOrg 
        ? undefined 
        : diretoria.codDiretoria;
      setTimeout(() => loadSolicitacoes(codDiretoria), 100);
    }
  };

  // Handler para mudança de tipo de remessa
  const handleTipoRemessaChange = (tipo: string) => {
    setTipoRemessa(tipo);
    
    // Recarregar remessas com o novo tipo
    setTimeout(() => loadRemessas(undefined, undefined, tipo), 100);
    
    // Recarregar solicitações com o novo tipo
    if (diretoriaSelecionada) {
      const codDiretoria = diretoriaSelecionada.configuracaoParaTodaOrg 
        ? undefined 
        : diretoriaSelecionada.codDiretoria;
      setTimeout(() => loadSolicitacoes(codDiretoria, tipo), 100);
    }
  };

  // Handler para mudança de mês/ano (Remessas)
  const handleMesAnoRemessasChange = (mes: string, ano: string) => {
    if (mes && ano) {
      const mesFormatado = mes.padStart(2, "0");
      const novaCompetencia = `${mesFormatado}/${ano}`;
      setCompetenciaRemessas(novaCompetencia);
      setMesRemessas(mesFormatado);
      setAnoRemessas(ano);
    } else {
      setCompetenciaRemessas("");
      setMesRemessas("");
      setAnoRemessas("");
    }
  };

  // Gerar lista de anos (2020 a 2030)
  const anos = Array.from({ length: 11 }, (_, i) => 2020 + i).map(String);

  // Meses em português
  const meses = [
    { value: "01", label: "Janeiro" },
    { value: "02", label: "Fevereiro" },
    { value: "03", label: "Março" },
    { value: "04", label: "Abril" },
    { value: "05", label: "Maio" },
    { value: "06", label: "Junho" },
    { value: "07", label: "Julho" },
    { value: "08", label: "Agosto" },
    { value: "09", label: "Setembro" },
    { value: "10", label: "Outubro" },
    { value: "11", label: "Novembro" },
    { value: "12", label: "Dezembro" },
  ];

  // Manter competencia apenas para o filtro de remessas

  // Handler para mudança de competência nas solicitações (filtro frontend)
  const handleCompetenciaSolicitacoesChange = (mes: string, ano: string) => {
    if (mes && ano) {
      const mesFormatado = mes.padStart(2, "0");
      const novaCompetencia = `${mesFormatado}/${ano}`;
      setCompetenciaSolicitacoes(novaCompetencia);
      setMesSolicitacoes(mesFormatado);
      setAnoSolicitacoes(ano);
    } else {
      setCompetenciaSolicitacoes("");
      setMesSolicitacoes("");
      setAnoSolicitacoes("");
    }
    setCurrentPageSolicitacoes(1);
  };

  // Sincronizar estados de mês/ano com competência (Remessas)
  useEffect(() => {
    if (competenciaRemessas && /^\d{2}\/\d{4}$/.test(competenciaRemessas)) {
      const [mes, ano] = competenciaRemessas.split("/");
      setMesRemessas(mes);
      setAnoRemessas(ano);
    } else if (!competenciaRemessas) {
      setMesRemessas("");
      setAnoRemessas("");
    }
  }, [competenciaRemessas]);

  // Handler para aplicar filtros de remessas
  const handleAplicarFiltrosRemessas = () => {
    let comp: string | undefined = undefined;
    if (mesRemessas && anoRemessas) {
      comp = `${mesRemessas}/${anoRemessas}`;
    }
    const status = statusRemessa || undefined;
    loadRemessas(comp, status, tipoRemessa);
    setCurrentPageRemessas(1);
  };

  // Handler para limpar filtros de remessas
  const handleLimparFiltrosRemessas = () => {
    setCompetenciaRemessas("");
    setStatusRemessa("");
    setMesRemessas("");
    setAnoRemessas("");
    loadRemessas(undefined, undefined, tipoRemessa);
    setCurrentPageRemessas(1);
  };
  const [isLargeScreen, setIsLargeScreen] = useState(window.innerWidth >= 1024);
  const [modalGerarRemessaOpen, setModalGerarRemessaOpen] = useState(false);
  const [modalLancamentosOpen, setModalLancamentosOpen] = useState(false);
  const [modalUploadRetornoOpen, setModalUploadRetornoOpen] = useState(false);
  const [modalRemessaGeradaOpen, setModalRemessaGeradaOpen] = useState(false);
  const [remessasGeradas, setRemessasGeradas] = useState<RemessaCnab[]>([]);
  const [remessaSelecionada, setRemessaSelecionada] =
    useState<RemessaCnab | null>(null);
  const [arquivoRetorno, setArquivoRetorno] = useState<File | null>(null);
  const [processando, setProcessando] = useState(false);
  const [diretorias, setDiretorias] = useState<Diretoria[]>([]);
  const [diretoriaSelecionada, setDiretoriaSelecionada] = useState<Diretoria | null>(null);
  const [diretoriasOpen, setDiretoriasOpen] = useState(false);
  const [diretoriasLoading, setDiretoriasLoading] = useState(false);
  const [buscaDiretoria, setBuscaDiretoria] = useState("");

  // Função helper para refresh completo (remessas + solicitações)
  const handleRefreshData = () => {
    refresh(); // Recarrega remessas
    
    // Recarrega solicitações apenas se houver diretoria selecionada
    if (diretoriaSelecionada) {
      const codDiretoria = diretoriaSelecionada.configuracaoParaTodaOrg 
        ? undefined 
        : diretoriaSelecionada.codDiretoria;
      loadSolicitacoes(codDiretoria);
    }
  };

  // Detectar mudanças no tamanho da tela
  useEffect(() => {
    const handleResize = () => {
      setIsLargeScreen(window.innerWidth >= 1024);
    };
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  // Carregar diretorias quando a página carregar
  useEffect(() => {
    const loadDiretorias = async () => {
      if (!token) return;
      
      setDiretoriasLoading(true);
      try {
        const useCase = container.resolve(ListarDiretoriasDisponiveisUseCase);
        const response = await useCase.execute(token);
        if (response.sucesso && response.retorno) {
          setDiretorias(response.retorno);
        }
      } catch (error) {
        console.error("Erro ao carregar diretorias:", error);
        toast({
          title: "Erro ao carregar diretorias",
          description: "Não foi possível carregar as diretorias disponíveis",
          variant: "destructive",
        });
      } finally {
        setDiretoriasLoading(false);
      }
    };

    loadDiretorias();
  }, [token, toast]);

  // Transformar dados da API para formato flat (uma linha por solicitação)
  const solicitacoesFlat = useMemo(() => {
    console.log("[RemessaCNAB] Processando solicitacoes:", solicitacoes);
    const flat: SolicitacaoRemessaCNAB[] = [];
    solicitacoes.forEach((colab) => {
      console.log(
        "[RemessaCNAB] Colaborador:",
        colab.nomeColaborador,
        "Solicitações:",
        colab.solicitacoes?.length || 0
      );
      colab.solicitacoes?.forEach((sol) => {
        flat.push(sol);
      });
    });
    console.log("[RemessaCNAB] Total de solicitações flat:", flat.length);
    return flat;
  }, [solicitacoes]);

  // Filtrar solicitações por busca, competência e forma de pagamento
  const solicitacoesFiltradas = useMemo(() => {
    let filtradas = [...solicitacoesFlat];

    // Filtro por competência (usando dataSolicitacao)
    if (competenciaSolicitacoes && /^\d{2}\/\d{4}$/.test(competenciaSolicitacoes)) {
      const [mes, ano] = competenciaSolicitacoes.split("/");
      filtradas = filtradas.filter((sol) => {
        if (!sol.dataSolicitacao) return false;
        const dataSol = new Date(sol.dataSolicitacao);
        const mesSol = String(dataSol.getMonth() + 1).padStart(2, "0");
        const anoSol = String(dataSol.getFullYear());
        return mesSol === mes && anoSol === ano;
      });
    }

    // Filtro por forma de pagamento (do colaborador pai)
    if (formaPagamentoFiltro) {
      filtradas = filtradas.filter((sol) => {
        // Encontrar o colaborador pai para pegar a forma de pagamento
        const colaborador = solicitacoes.find(
          (colab) => colab.codigoColaborador === sol.codigoColaborador
        );
        return colaborador?.formaPagamento === formaPagamentoFiltro;
      });
    }

    // Filtro por busca de texto
    if (buscaSolicitacoes) {
      const buscaLower = buscaSolicitacoes.toLowerCase();
      filtradas = filtradas.filter((sol) => {
        return (
          sol.nome.toLowerCase().includes(buscaLower) ||
          sol.cliente.toLowerCase().includes(buscaLower) ||
          sol.projeto.toLowerCase().includes(buscaLower) ||
          formatCurrency(sol.valorParaPagamento)
            .toLowerCase()
            .includes(buscaLower)
        );
      });
    }

    return filtradas;
  }, [solicitacoesFlat, buscaSolicitacoes, competenciaSolicitacoes, formaPagamentoFiltro, solicitacoes]);

  // Remessas filtradas
  const remessasFiltradas = remessas;

  // Resetar página quando filtros mudarem
  useEffect(() => {
    setCurrentPageSolicitacoes(1);
  }, [buscaSolicitacoes, competenciaSolicitacoes, formaPagamentoFiltro]);

  // Paginação - Solicitações
  const startIndexSolicitacoes =
    (currentPageSolicitacoes - 1) * itemsPerPageSolicitacoes;
  const endIndexSolicitacoes =
    startIndexSolicitacoes + itemsPerPageSolicitacoes;
  const solicitacoesPaginadas = solicitacoesFiltradas.slice(
    startIndexSolicitacoes,
    endIndexSolicitacoes
  );

  // Paginação - Remessas
  const startIndexRemessas = (currentPageRemessas - 1) * itemsPerPageRemessas;
  const endIndexRemessas = startIndexRemessas + itemsPerPageRemessas;
  const remessasPaginadas = remessasFiltradas.slice(
    startIndexRemessas,
    endIndexRemessas
  );

  // Filtrar apenas solicitações que podem ser selecionadas (com forma de pagamento)
  const solicitacoesSelecionaveis = solicitacoesFiltradas.filter((s) => {
    const colaborador = solicitacoes.find(
      (colab) => colab.codigoColaborador === s.codigoColaborador
    );
    return !!colaborador?.formaPagamento;
  });

  const todasSelecionadas =
    solicitacoesSelecionaveis.length > 0 &&
    solicitacoesSelecionaveis.every((s) => selectedIds.has(s.id));

  const handleSelectAll = (checked: boolean) => {
    if (checked) {
      const novosIds = new Set(selectedIds);
      // Apenas selecionar solicitações que têm forma de pagamento
      solicitacoesFiltradas.forEach((s) => {
        const colaborador = solicitacoes.find(
          (colab) => colab.codigoColaborador === s.codigoColaborador
        );
        const temFormaPagamento = !!colaborador?.formaPagamento;
        if (temFormaPagamento) {
          novosIds.add(s.id);
        }
      });
      setSelectedIds(novosIds);
    } else {
      const novosIds = new Set(selectedIds);
      solicitacoesFiltradas.forEach((s) => novosIds.delete(s.id));
      setSelectedIds(novosIds);
    }
  };

  const handleSelectItem = (id: string, checked: boolean) => {
    const novosIds = new Set(selectedIds);
    if (checked) {
      novosIds.add(id);
    } else {
      novosIds.delete(id);
    }
    setSelectedIds(novosIds);
  };

  // Handler para expandir/recolher grids
  const toggleGridExpandido = (grid: "esquerdo" | "direito" | "ambos") => {
    if (grid === "ambos") {
      setGridExpandido("ambos");
    } else if (gridExpandido === grid) {
      setGridExpandido("ambos");
    } else {
      setGridExpandido(grid);
    }
  };

  const handleGerarRemessa = () => {
    if (selectedIds.size === 0) return;
    if (!diretoriaSelecionada) {
      toast({
        title: "Diretoria obrigatória",
        description: "Selecione uma diretoria antes de gerar a remessa",
        variant: "destructive",
      });
      return;
    }
    setModalGerarRemessaOpen(true);
  };

  const handleConfirmarGerarRemessa = async () => {
    if (!diretoriaSelecionada) {
      toast({
        title: "Diretoria obrigatória",
        description: "Selecione uma diretoria",
        variant: "destructive",
      });
      return;
    }

    if (selectedIds.size === 0) {
      toast({
        title: "Nenhuma solicitação selecionada",
        description:
          "Selecione pelo menos uma solicitação para gerar a remessa",
        variant: "destructive",
      });
      return;
    }

    try {
      setProcessando(true);
      // Converter Set para Array de IDs (mantendo como strings)
      const solicitacoesPagamentoIds = Array.from(selectedIds);
      // Se configuracaoParaTodaOrg for true, enviar null, caso contrário enviar o codDiretoria
      const codDiretoria = diretoriaSelecionada?.configuracaoParaTodaOrg 
        ? null 
        : diretoriaSelecionada?.codDiretoria || null;
      
      const remessasCriadas = await processarRemessa(
        codDiretoria,
        solicitacoesPagamentoIds,
        tipoRemessa
      );

      // Fechar modal de confirmação
      setModalGerarRemessaOpen(false);
      setSelectedIds(new Set());

      // Abrir modal de sucesso com as remessas geradas
      if (remessasCriadas && remessasCriadas.length > 0) {
        setRemessasGeradas(remessasCriadas);
        setModalRemessaGeradaOpen(true);
      } else {
        toast({
          title: "Remessa gerada",
          description: "A remessa foi processada com sucesso",
        });
      }

      handleRefreshData();
    } catch (err) {
      toast({
        title: "Erro ao gerar remessa",
        description: err instanceof Error ? err.message : "Erro desconhecido",
        variant: "destructive",
      });
    } finally {
      setProcessando(false);
    }
  };

  const handleVerLancamentos = (remessa: RemessaCnab) => {
    setRemessaSelecionada(remessa);
    setModalLancamentosOpen(true);
  };

  const handleDownloadRemessa = async (remessa: RemessaCnab) => {
    if (!remessa.nomeArquivoRemessa) {
      toast({
        title: "Arquivo não disponível",
        description: "O arquivo de remessa ainda não foi gerado",
        variant: "destructive",
      });
      return;
    }

    if (!token) {
      toast({
        title: "Token não encontrado",
        description: "Faça login novamente",
        variant: "destructive",
      });
      return;
    }

    try {
      // Codificar o token em Base64
      const tokenEncoded = btoa(token);

      // Substituir /$1/ pelo token codificado na URL
      const url = remessa.nomeArquivoRemessa.replace(
        "/$1/",
        `/${tokenEncoded}/`
      );

      // Fazer o download usando fetch com o token no header
      const response = await fetch(url, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error(`Erro ao baixar arquivo: ${response.status}`);
      }

      // Obter o blob do arquivo
      const blob = await response.blob();

      // Criar URL temporária e fazer download
      const blobUrl = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = blobUrl;

      // Extrair nome do arquivo da URL ou usar nome padrão
      const fileName =
        remessa.nomeArquivoRemessa.split("/").pop() || "remessa.txt";
      link.download = fileName;

      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      // Limpar URL temporária
      window.URL.revokeObjectURL(blobUrl);

      toast({
        title: "Download iniciado",
        description: "O arquivo está sendo baixado",
      });
    } catch (err) {
      toast({
        title: "Erro ao baixar arquivo",
        description: err instanceof Error ? err.message : "Erro desconhecido",
        variant: "destructive",
      });
    }
  };

  const handleUploadRetorno = (remessa: RemessaCnab) => {
    setRemessaSelecionada(remessa);
    setModalUploadRetornoOpen(true);
  };

  const handleConfirmarUploadRetorno = async () => {
    if (!arquivoRetorno || !remessaSelecionada) {
      toast({
        title: "Arquivo obrigatório",
        description: "Selecione um arquivo de retorno",
        variant: "destructive",
      });
      return;
    }

    try {
      setProcessando(true);
      const resultado = await processarRetorno(
        arquivoRetorno,
        remessaSelecionada.hashRemessa,
        tipoRemessa
      );

      toast({
        title: "Retorno processado com sucesso",
        description: resultado.mensagem || "Baixas realizadas automaticamente",
      });

      setModalUploadRetornoOpen(false);
      setArquivoRetorno(null);
      setRemessaSelecionada(null);
      handleRefreshData();
    } catch (err) {
      toast({
        title: "Erro ao processar retorno",
        description: err instanceof Error ? err.message : "Erro desconhecido",
        variant: "destructive",
      });
    } finally {
      setProcessando(false);
    }
  };

  // Colunas do Grid de Solicitações Aprovadas
  const columnsSolicitacoes: Column[] = [
    { id: "selecao", label: "Seleção", sortable: false, width: "w-[80px]" },
    { id: "colaborador", label: "Colaborador", sortable: true },
    { id: "cliente", label: "Cliente", sortable: true },
    { id: "projeto", label: "Projeto", sortable: true },
    { id: "valor", label: "Valor", sortable: true },
    { id: "dataAprovacao", label: "Data Solicitação", sortable: true },
    { id: "formaPagamento", label: "Forma Pagamento", sortable: true },
  ];

  const renderCellSolicitacoes = (
    solicitacao: SolicitacaoRemessaCNAB,
    columnId: string
  ) => {
    switch (columnId) {
      case "selecao": {
        // Buscar forma de pagamento do colaborador para verificar se pode selecionar
        const colaborador = solicitacoes.find(
          (colab) => colab.codigoColaborador === solicitacao.codigoColaborador
        );
        const temFormaPagamento = !!colaborador?.formaPagamento;
        
        // Se não tem forma de pagamento, não mostra o checkbox
        if (!temFormaPagamento) {
          return <span className="text-xs text-muted-foreground">-</span>;
        }
        
        return (
          <Checkbox
            checked={selectedIds.has(solicitacao.id)}
            onCheckedChange={(checked) =>
              handleSelectItem(solicitacao.id, checked as boolean)
            }
          />
        );
      }
      case "colaborador":
        return (
          <span className="font-medium whitespace-nowrap truncate block">
            {solicitacao.nome}
          </span>
        );
      case "cliente":
        return (
          <span className="whitespace-nowrap truncate block">
            {solicitacao.cliente}
          </span>
        );
      case "projeto":
        return (
          <span
            className="whitespace-nowrap truncate block"
            title={solicitacao.projeto}
          >
            {solicitacao.projeto}
          </span>
        );
      case "valor":
        return (
          <span className="font-semibold whitespace-nowrap">
            {formatCurrency(solicitacao.valorParaPagamento)}
          </span>
        );
      case "dataAprovacao":
        return (
          <span className="whitespace-nowrap">
            {solicitacao.dataSolicitacao 
              ? format(new Date(solicitacao.dataSolicitacao), "dd/MM/yyyy")
              : "N/A"}
          </span>
        );
      case "formaPagamento": {
        // Buscar forma de pagamento do colaborador pai
        const colaborador = solicitacoes.find(
          (colab) => colab.codigoColaborador === solicitacao.codigoColaborador
        );
        return (
          <span className="whitespace-nowrap">
            {colaborador?.formaPagamento || "N/A"}
          </span>
        );
      }
      default:
        return null;
    }
  };

  // Colunas do Grid de Remessas Geradas
  const columnsRemessas: Column[] = [
    {
      id: "dataGeracao",
      label: "Data Geração",
      sortable: true,
      align: "center",
    },
    { id: "tipo", label: "Tipo", sortable: true, align: "center" },
    {
      id: "quantidadeRegistros",
      label: "Quantidade Registros",
      sortable: true,
      align: "center",
    },
    { id: "valorTotal", label: "Valor Total", sortable: true, align: "right" },
    {
      id: "status",
      label: "Status",
      sortable: true,
      width: "w-[120px]",
      align: "center",
    },
    {
      id: "acoes",
      label: "Ações",
      sortable: false,
      width: "w-[250px]",
      align: "center",
    },
  ];

  const renderCellRemessas = (remessa: RemessaCnab, columnId: string) => {
    switch (columnId) {
      case "tipo":
        return (
          <span className="whitespace-nowrap block text-center">
            {remessa.tipo}
          </span>
        );
      case "quantidadeRegistros":
        return (
          <span className="whitespace-nowrap block text-center">
            {remessa.lancamentos.length}
          </span>
        );
      case "valorTotal":
        return (
          <span className="font-semibold whitespace-nowrap block text-right">
            {formatCurrency(remessa.valorTotal)}
          </span>
        );
      case "dataGeracao":
        return (
          <span className="whitespace-nowrap block text-center">
            {remessa.dataCriacao 
              ? format(new Date(remessa.dataCriacao), "dd/MM/yyyy HH:mm")
              : "N/A"}
          </span>
        );
      case "status": {
        const statusMap: Record<string, string> = {
          "GERANDO ARQUIVO": "Pendente",
          "ARQUIVO GERADO": "Aguardando Retorno",
          FINALIZADO: "Processada",
          CANCELADO: "Cancelada",
        };
        return (
          <div className="flex justify-center">
            <StatusBadge
              status={statusMap[remessa.status] || remessa.status}
              variant="reembolso"
            />
          </div>
        );
      }
      case "acoes":
        return (
          <div className="flex items-center gap-2 justify-center">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleVerLancamentos(remessa)}
            >
              <Eye className="h-4 w-4" />
              Ver Lançamentos
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleDownloadRemessa(remessa)}
              disabled={!remessa.nomeArquivoRemessa}
            >
              <Download className="h-4 w-4" />
              Download
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => handleUploadRetorno(remessa)}
              disabled={remessa.status === "FINALIZADO"}
            >
              <Upload className="h-4 w-4" />
              Retorno
            </Button>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="min-h-screen bg-primaryBackground">
      <div className="container mx-auto p-4 md:p-6 h-[calc(100vh-2rem)] flex flex-col gap-3 overflow-hidden">
        <div className="flex-shrink-0">
          <PageBreadcrumb
            items={[
              { label: "Reembolso", href: "/reembolso" },
              { label: "Remessa CNAB" },
            ]}
          />

          <div className="flex flex-col sm:flex-row sm:items-center gap-4 mt-2">
            <div className="flex items-center gap-4">
              <Button
                variant="ghost"
                size="icon"
                onClick={() => navigate("/reembolso?tab=gestaoadm")}
                className="h-10 w-10"
                title="Voltar para Gestão Administrativa"
              >
                <ArrowLeft className="h-5 w-5" />
              </Button>
              <PageHeader
                title="Remessa CNAB"
                description="Gerencie as remessas CNAB para pagamento de reembolsos"
              />
            </div>

            {/* Seletor de Tipo de Remessa */}
            {modulosRemessa.length > 0 && (
              <div className="flex-shrink-0 space-y-2 w-full sm:w-auto sm:min-w-[200px] sm:ml-auto">
                <Label htmlFor="tipoRemessa" className="text-sm font-medium">
                  Tipo de Remessa
                </Label>
                <Select
                  value={tipoRemessa}
                  onValueChange={handleTipoRemessaChange}
                >
                  <SelectTrigger id="tipoRemessa">
                    <SelectValue placeholder="Selecione o tipo de remessa" />
                  </SelectTrigger>
                  <SelectContent>
                    {modulosRemessa.map((modulo) => (
                      <SelectItem key={modulo} value={modulo}>
                        {modulo}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            )}
          </div>
        </div>

        {/* Mensagem quando não há tipo de remessa selecionado */}
        {!tipoRemessa && (
          <Alert className="flex-shrink-0 border-blue-200 bg-blue-50 dark:bg-blue-950/20">
            <AlertDescription className="text-blue-800 dark:text-blue-200">
              Selecione um tipo de remessa para visualizar as solicitações e remessas geradas.
            </AlertDescription>
          </Alert>
        )}

        {/* Grid Layout: Dois grids lado a lado com botão de expandir/recolher */}
        <div
          id="grids-container"
          className={cn(
            "flex flex-col lg:flex-row flex-1 min-h-0",
            isLargeScreen && gridExpandido === "ambos" ? "gap-4" : "gap-0"
          )}
        >
          {/* Grid 1: Solicitações Aprovadas */}
          <Card
            className={cn(
              "flex flex-col min-h-0 transition-all duration-300 border rounded-lg overflow-hidden hover:shadow-none",
              isLargeScreen && gridExpandido === "esquerdo"
                ? "lg:flex-1 lg:min-w-0"
                : isLargeScreen && gridExpandido === "direito"
                ? "lg:w-0 lg:overflow-hidden lg:opacity-0 lg:min-w-0 lg:border-0 lg:flex-none"
                : "lg:w-1/2 lg:flex-none"
            )}
          >
            <CardContent className="p-3 md:p-4 flex flex-col flex-1 min-h-0">
              <div className="flex-shrink-0 mb-2">
                <div className="flex items-center justify-between mb-2">
                  <h2 className="text-xl font-semibold">
                    Solicitações Aprovadas
                  </h2>
                  {isLargeScreen && (
                    <Button
                      variant="ghost"
                      size="icon"
                      className="h-8 w-8"
                      onClick={() => toggleGridExpandido("esquerdo")}
                      title={
                        gridExpandido === "esquerdo"
                          ? "Mostrar ambos os grids"
                          : "Expandir apenas solicitações"
                      }
                    >
                      {gridExpandido === "esquerdo" ? (
                        <Minimize2 className="h-4 w-4" />
                      ) : (
                        <Maximize2 className="h-4 w-4" />
                      )}
                    </Button>
                  )}
                </div>

                {/* Filtros e Ações */}
                <div className="space-y-3 mb-2">
                  {/* Primeira linha: Competência e Forma de Pagamento */}
                  <div className="flex flex-wrap items-end gap-4">
                    <div className="space-y-2 flex-1 min-w-[200px]">
                      <Label>Competência (Data Solicitação)</Label>
                      <div className="flex gap-2">
                        <Select
                          value={mesSolicitacoes || undefined}
                          onValueChange={(value) =>
                            handleCompetenciaSolicitacoesChange(value, anoSolicitacoes)
                          }
                        >
                          <SelectTrigger className="flex-1">
                            <SelectValue placeholder="Mês" />
                          </SelectTrigger>
                          <SelectContent>
                            {meses.map((mes) => (
                              <SelectItem key={mes.value} value={mes.value}>
                                {mes.label}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <Select
                          value={anoSolicitacoes || undefined}
                          onValueChange={(value) =>
                            handleCompetenciaSolicitacoesChange(mesSolicitacoes, value)
                          }
                        >
                          <SelectTrigger className="flex-1">
                            <SelectValue placeholder="Ano" />
                          </SelectTrigger>
                          <SelectContent>
                            {anos.map((ano) => (
                              <SelectItem key={ano} value={ano}>
                                {ano}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                    </div>

                    <div className="space-y-2 flex-1 min-w-[200px]">
                      <Label>Forma de Pagamento</Label>
                      <Select
                        value={formaPagamentoFiltro || "TODAS"}
                        onValueChange={(value) =>
                          setFormaPagamentoFiltro(value === "TODAS" ? "" : (value as FormaPagamento))
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Todas" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="TODAS">Todas</SelectItem>
                          <SelectItem value="PIX">PIX</SelectItem>
                          <SelectItem value="TED">TED</SelectItem>
                          <SelectItem value="DOC">DOC</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>

                    {(competenciaSolicitacoes || formaPagamentoFiltro) && (
                      <Button
                        variant="ghost"
                        onClick={() => {
                          setCompetenciaSolicitacoes("");
                          setMesSolicitacoes("");
                          setAnoSolicitacoes("");
                          setFormaPagamentoFiltro("");
                          setCurrentPageSolicitacoes(1);
                        }}
                        className="h-10"
                      >
                        Limpar Filtros
                      </Button>
                    )}
                  </div>

                  {/* Segunda linha: Diretoria e Botão Gerar Remessa */}
                  <div className="flex flex-wrap items-end gap-4">
                    <div className="space-y-2 flex-1 min-w-[200px]">
                      <Label>
                        Diretoria <span className="text-destructive">*</span>
                      </Label>
                      <Popover open={diretoriasOpen} onOpenChange={setDiretoriasOpen}>
                        <PopoverTrigger asChild>
                          <Button
                            variant="outline"
                            role="combobox"
                            aria-expanded={diretoriasOpen}
                            className="w-full justify-between"
                            disabled={diretoriasLoading}
                          >
                            {diretoriaSelecionada
                              ? diretoriaSelecionada.diretoria || diretoriaSelecionada.codDiretoria
                              : diretoriasLoading
                              ? "Carregando..."
                              : "Selecione a diretoria"}
                            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                          </Button>
                        </PopoverTrigger>
                        <PopoverContent className="w-[400px] p-0">
                          <Command shouldFilter={false}>
                            <CommandInput
                              placeholder="Buscar diretoria..."
                              value={buscaDiretoria}
                              onValueChange={setBuscaDiretoria}
                            />
                            <CommandList>
                              {diretoriasLoading ? (
                                <CommandEmpty>Carregando...</CommandEmpty>
                              ) : diretorias.length === 0 ? (
                                <CommandEmpty>
                                  Não existe configuração do CNAB para essa organização
                                </CommandEmpty>
                              ) : (
                                <CommandGroup>
                                  {diretorias
                                    .filter((d) =>
                                      buscaDiretoria
                                        ? d.diretoria.toLowerCase().includes(buscaDiretoria.toLowerCase()) ||
                                          d.codDiretoria.toLowerCase().includes(buscaDiretoria.toLowerCase())
                                        : true
                                    )
                                    .map((diretoria) => (
                                      <CommandItem
                                        key={diretoria.codDiretoria}
                                        value={diretoria.diretoria}
                                        onSelect={() => {
                                          handleDiretoriaChange(diretoria);
                                          setDiretoriasOpen(false);
                                          setBuscaDiretoria("");
                                        }}
                                      >
                                        <Check
                                          className={cn(
                                            "mr-2 h-4 w-4",
                                            diretoriaSelecionada?.codDiretoria === diretoria.codDiretoria
                                              ? "opacity-100"
                                              : "opacity-0"
                                          )}
                                        />
                                        {diretoria.diretoria} ({diretoria.codDiretoria})
                                      </CommandItem>
                                    ))}
                                </CommandGroup>
                              )}
                            </CommandList>
                          </Command>
                        </PopoverContent>
                      </Popover>
                    </div>

                    <div className="flex items-end gap-3">
                      {selectedIds.size > 0 && (
                        <div className="flex flex-col items-end gap-1">
                          <span className="text-xs text-muted-foreground">
                            Valor Total Selecionado
                          </span>
                          <span className="text-lg font-semibold text-primary">
                            {formatCurrency(
                              solicitacoesFiltradas
                                .filter((s) => selectedIds.has(s.id))
                                .reduce(
                                  (sum, s) => sum + s.valorParaPagamento,
                                  0
                                )
                            )}
                          </span>
                        </div>
                      )}
                      <Button
                        onClick={handleGerarRemessa}
                        disabled={selectedIds.size === 0 || !diretoriaSelecionada}
                        className="bg-primary hover:bg-primary/90 h-10"
                      >
                        <FileText className="h-4 w-4 mr-2" />
                        Gerar Remessa ({selectedIds.size})
                      </Button>
                    </div>
                  </div>

                  {/* Terceira linha: Selecionar Todos e Busca */}
                  <div className="flex items-center justify-between gap-4 flex-wrap">
                    {solicitacoesSelecionaveis.length > 0 ? (
                      <div className="flex items-center gap-2 flex-shrink-0">
                        <Checkbox
                          checked={todasSelecionadas}
                          onCheckedChange={handleSelectAll}
                        />
                        <Label className="text-sm font-medium whitespace-nowrap">
                          Selecionar todos ({solicitacoesSelecionaveis.length}{" "}
                          {solicitacoesSelecionaveis.length !== solicitacoesFiltradas.length 
                            ? `de ${solicitacoesFiltradas.length} registros` 
                            : "registros"})
                        </Label>
                      </div>
                    ) : (
                      <div className="flex items-center gap-2 flex-shrink-0">
                        <Checkbox disabled />
                        <Label className="text-sm font-medium text-muted-foreground whitespace-nowrap">
                          Selecionar todos ({solicitacoesFiltradas.length > 0 
                            ? `0 de ${solicitacoesFiltradas.length} registros`
                            : "0 registros"})
                        </Label>
                      </div>
                    )}
                    <div className="relative w-full sm:w-80">
                      <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                      <Input
                        placeholder="Buscar solicitações..."
                        value={buscaSolicitacoes}
                        onChange={(e) => setBuscaSolicitacoes(e.target.value)}
                        className="pl-10"
                      />
                    </div>
                  </div>
                </div>
              </div>

              {/* Tabela com scroll interno */}
              <div className="flex-1 min-h-0 flex flex-col border rounded-lg overflow-hidden bg-background">
                {loading ? (
                  <div className="flex-1 flex items-center justify-center p-8">
                    <p className="text-muted-foreground">Carregando...</p>
                  </div>
                ) : solicitacoesFiltradas.length === 0 ? (
                  <div className="flex-1 flex items-center justify-center p-8">
                    <Alert className="w-full max-w-md border-0 bg-muted/50">
                      <AlertDescription className="text-center py-2">
                        {error || "Nenhuma solicitação aprovada encontrada"}
                      </AlertDescription>
                    </Alert>
                  </div>
                ) : (
                  <>
                    <div className="flex-1 min-h-0 overflow-x-auto overflow-y-auto">
                      <DataTable
                        columns={columnsSolicitacoes}
                        data={solicitacoesPaginadas}
                        keyExtractor={(item) => item.id.toString()}
                        renderCell={renderCellSolicitacoes}
                        emptyMessage=""
                      />
                    </div>
                    <div className="flex-shrink-0 border-t bg-background">
                      <TablePagination
                        currentPage={currentPageSolicitacoes}
                        totalItems={solicitacoesFiltradas.length}
                        itemsPerPage={itemsPerPageSolicitacoes}
                        onPageChange={setCurrentPageSolicitacoes}
                        onItemsPerPageChange={(items) => {
                          setItemsPerPageSolicitacoes(Number(items));
                          setCurrentPageSolicitacoes(1);
                        }}
                      />
                    </div>
                  </>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Indicador de grid recolhido - Solicitações */}
          {isLargeScreen && gridExpandido === "direito" && (
            <div
              className="hidden lg:flex items-center justify-center w-16 mx-4 bg-background rounded-lg border-2 border-dashed border-primary/40 hover:border-primary/60 hover:bg-muted transition-colors flex-shrink-0 shadow-sm cursor-pointer"
              onClick={() => toggleGridExpandido("ambos")}
              title="Mostrar solicitações aprovadas"
              role="button"
              tabIndex={0}
              onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") {
                  e.preventDefault();
                  toggleGridExpandido("ambos");
                }
              }}
            >
              <ChevronRight className="h-5 w-5 text-primary" />
            </div>
          )}

          {/* Grid 2: Remessas Geradas */}
          <Card
            className={cn(
              "flex flex-col min-h-0 transition-all duration-300 border rounded-lg overflow-hidden hover:shadow-none",
              isLargeScreen && gridExpandido === "direito"
                ? "lg:flex-1 lg:min-w-0"
                : isLargeScreen && gridExpandido === "esquerdo"
                ? "lg:w-0 lg:overflow-hidden lg:opacity-0 lg:min-w-0 lg:border-0 lg:flex-none"
                : "lg:w-1/2 lg:flex-none"
            )}
          >
            <CardContent className="p-3 md:p-4 flex flex-col flex-1 min-h-0">
              <div className="flex-shrink-0 mb-2">
                <div className="flex items-center justify-between mb-2">
                  <h2 className="text-xl font-semibold">Remessas Geradas</h2>
                  {isLargeScreen && (
                    <Button
                      variant="ghost"
                      size="icon"
                      className="h-8 w-8"
                      onClick={() => toggleGridExpandido("direito")}
                      title={
                        gridExpandido === "direito"
                          ? "Mostrar ambos os grids"
                          : "Expandir apenas remessas"
                      }
                    >
                      {gridExpandido === "direito" ? (
                        <Minimize2 className="h-4 w-4" />
                      ) : (
                        <Maximize2 className="h-4 w-4" />
                      )}
                    </Button>
                  )}
                </div>

                {/* Filtros para Remessas Geradas */}
                <div className="space-y-3 mb-2">
                  <div className="flex flex-wrap items-end gap-4">
                    <div className="space-y-2 flex-1 min-w-[200px]">
                      <Label>Data da geração do Processamento</Label>
                      <div className="flex gap-2">
                        <Select
                          value={mesRemessas || undefined}
                          onValueChange={(value) =>
                            handleMesAnoRemessasChange(value, anoRemessas)
                          }
                        >
                          <SelectTrigger className="flex-1">
                            <SelectValue placeholder="Mês" />
                          </SelectTrigger>
                          <SelectContent>
                            {meses.map((mes) => (
                              <SelectItem key={mes.value} value={mes.value}>
                                {mes.label}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <Select
                          value={anoRemessas || undefined}
                          onValueChange={(value) =>
                            handleMesAnoRemessasChange(mesRemessas, value)
                          }
                        >
                          <SelectTrigger className="flex-1">
                            <SelectValue placeholder="Ano" />
                          </SelectTrigger>
                          <SelectContent>
                            {anos.map((ano) => (
                              <SelectItem key={ano} value={ano}>
                                {ano}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                      </div>
                    </div>

                    <div className="space-y-2 flex-1 min-w-[200px]">
                      <Label>Status</Label>
                      <Select
                        value={statusRemessa || undefined}
                        onValueChange={(value) =>
                          setStatusRemessa(value as StatusRemessa | "")
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Todos os status" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="GERANDO ARQUIVO">
                            Gerando Arquivo
                          </SelectItem>
                          <SelectItem value="ARQUIVO GERADO">
                            Arquivo Gerado
                          </SelectItem>
                          <SelectItem value="FINALIZADO">Finalizado</SelectItem>
                          <SelectItem value="CANCELADO">Cancelado</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>

                    <div className="flex items-end gap-2">
                      <Button
                        variant="outline"
                        onClick={handleAplicarFiltrosRemessas}
                        className="h-10"
                      >
                        Aplicar Filtros
                      </Button>
                      {(competenciaRemessas || statusRemessa) && (
                        <Button
                          variant="ghost"
                          onClick={handleLimparFiltrosRemessas}
                          className="h-10"
                        >
                          Limpar
                        </Button>
                      )}
                    </div>
                  </div>
                </div>
              </div>

              {/* Tabela com scroll interno */}
              <div className="flex-1 min-h-0 flex flex-col border rounded-lg overflow-hidden bg-background">
                {loading ? (
                  <div className="flex-1 flex items-center justify-center p-8">
                    <p className="text-muted-foreground">Carregando...</p>
                  </div>
                ) : remessasFiltradas.length === 0 ? (
                  <div className="flex-1 flex items-center justify-center p-8">
                    <Alert className="w-full max-w-md border-0 bg-muted/50">
                      <AlertDescription className="text-center py-2">
                        {error || "Nenhuma remessa gerada"}
                      </AlertDescription>
                    </Alert>
                  </div>
                ) : (
                  <>
                    <div className="flex-1 min-h-0 overflow-x-auto overflow-y-auto">
                      <DataTable
                        columns={columnsRemessas}
                        data={remessasPaginadas}
                        keyExtractor={(item) => item.id.toString()}
                        renderCell={renderCellRemessas}
                        emptyMessage=""
                      />
                    </div>
                    <div className="flex-shrink-0 border-t bg-background">
                      <TablePagination
                        currentPage={currentPageRemessas}
                        totalItems={remessasFiltradas.length}
                        itemsPerPage={itemsPerPageRemessas}
                        onPageChange={setCurrentPageRemessas}
                        onItemsPerPageChange={(items) => {
                          setItemsPerPageRemessas(Number(items));
                          setCurrentPageRemessas(1);
                        }}
                      />
                    </div>
                  </>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Indicador de grid recolhido - Remessas */}
          {isLargeScreen && gridExpandido === "esquerdo" && (
            <div
              className="hidden lg:flex items-center justify-center w-16 ml-4 bg-background rounded-lg border-2 border-dashed border-primary/40 hover:border-primary/60 hover:bg-muted transition-colors flex-shrink-0 shadow-sm cursor-pointer"
              onClick={() => toggleGridExpandido("ambos")}
              title="Mostrar remessas geradas"
              role="button"
              tabIndex={0}
              onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") {
                  e.preventDefault();
                  toggleGridExpandido("ambos");
                }
              }}
            >
              <ChevronRight className="h-5 w-5 text-primary" />
            </div>
          )}
        </div>

        {/* Modal: Gerar Remessa */}
        <AlertDialog
          open={modalGerarRemessaOpen}
          onOpenChange={setModalGerarRemessaOpen}
        >
          <AlertDialogContent className="max-w-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle className="text-2xl font-bold">
                Gerar Remessa CNAB
              </AlertDialogTitle>
              <AlertDialogDescription>
                Confirme as informações antes de gerar a remessa
              </AlertDialogDescription>
            </AlertDialogHeader>

            <div className="space-y-4 py-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label className="text-sm font-medium text-muted-foreground">
                    Quantidade de Registros
                  </Label>
                  <p className="text-lg font-semibold">{selectedIds.size}</p>
                </div>
                <div>
                  <Label className="text-sm font-medium text-muted-foreground">
                    Valor Total
                  </Label>
                  <p className="text-lg font-semibold text-primary">
                    {formatCurrency(
                      solicitacoesFiltradas
                        .filter((s) => selectedIds.has(s.id))
                        .reduce((sum, s) => sum + s.valorParaPagamento, 0)
                    )}
                  </p>
                </div>
              </div>

              <div className="bg-muted/50 rounded-lg p-4">
                <p className="text-sm text-muted-foreground">
                  Ao confirmar, um arquivo .txt será gerado e baixado
                  automaticamente. A remessa será registrada no sistema.
                </p>
              </div>
            </div>

            <AlertDialogFooter>
              <AlertDialogCancel>Cancelar</AlertDialogCancel>
              <AlertDialogAction
                onClick={handleConfirmarGerarRemessa}
                disabled={processando}
              >
                {processando ? (
                  <>
                    <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                    Processando...
                  </>
                ) : (
                  "Gerar Remessa"
                )}
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>

        {/* Modal: Ver Lançamentos */}
        <Dialog
          open={modalLancamentosOpen}
          onOpenChange={setModalLancamentosOpen}
        >
          <DialogContent className="max-w-4xl max-h-[80vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle className="text-2xl font-bold">
                Lançamentos da Remessa
              </DialogTitle>
              <DialogDescription>
                Visualize todos os lançamentos que originaram esta remessa
              </DialogDescription>
            </DialogHeader>

            <div className="py-4">
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
                <div>
                  <Label className="text-sm font-medium text-muted-foreground">
                    Data Processamento
                  </Label>
                  <p className="font-semibold">
                    {remessaSelecionada?.mesAnoProcessamento || remessaSelecionada?.competencia}
                  </p>
                </div>
                <div>
                  <Label className="text-sm font-medium text-muted-foreground">
                    Quantidade
                  </Label>
                  <p className="font-semibold">
                    {remessaSelecionada?.lancamentos.length}
                  </p>
                </div>
                <div>
                  <Label className="text-sm font-medium text-muted-foreground">
                    Valor Total
                  </Label>
                  <p className="font-semibold text-primary">
                    {remessaSelecionada &&
                      formatCurrency(remessaSelecionada.valorTotal)}
                  </p>
                </div>
              </div>

              <DataTable
                columns={[
                  { id: "colaborador", label: "Colaborador", sortable: true },
                  { id: "valor", label: "Valor", sortable: true },
                  {
                    id: "formaPagamento",
                    label: "Forma Pagamento",
                    sortable: true,
                  },
                  { id: "status", label: "Status", sortable: true },
                  { id: "erro", label: "Observação", sortable: false },
                ]}
                data={remessaSelecionada?.lancamentos || []}
                keyExtractor={(item) => item.id}
                renderCell={(item: LancamentoRemessa, columnId: string) => {
                  switch (columnId) {
                    case "colaborador":
                      return (
                        <span className="font-medium">
                          {item.nomeColaborador}
                        </span>
                      );
                    case "valor":
                      return (
                        <span className="font-semibold">
                          {formatCurrency(item.valorPagamentoTotal)}
                        </span>
                      );
                    case "formaPagamento":
                      return <span>{item.formaPagamento}</span>;
                    case "status": {
                      const statusMap: Record<string, string> = {
                        AGUARDANDO: "Aguardando",
                        PAGO: "Pago",
                        ERRO: "Erro",
                      };
                      return (
                        <StatusBadge
                          status={statusMap[item.statusCnab] || item.statusCnab}
                          variant="reembolso"
                        />
                      );
                    }
                    case "erro":
                      return item.descricaoErro ? (
                        <div className="flex items-center gap-2">
                          <Alert className="border-red-200 bg-red-50 p-2">
                            <AlertDescription className="text-xs text-red-800">
                              {item.descricaoErro}
                            </AlertDescription>
                          </Alert>
                        </div>
                      ) : (
                        <span className="text-muted-foreground text-xs">-</span>
                      );
                    default:
                      return null;
                  }
                }}
                emptyMessage="Nenhum lançamento encontrado"
              />
            </div>
          </DialogContent>
        </Dialog>

        {/* Modal: Remessa Gerada com Sucesso */}
        <Dialog
          open={modalRemessaGeradaOpen}
          onOpenChange={setModalRemessaGeradaOpen}
        >
          <DialogContent className="max-w-4xl max-h-[80vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle className="text-2xl font-bold text-green-600">
                Remessa Gerada com Sucesso!
              </DialogTitle>
              <DialogDescription>
                {remessasGeradas.length === 1
                  ? "A remessa foi processada e o arquivo está disponível para download"
                  : `${remessasGeradas.length} remessas foram processadas e os arquivos estão disponíveis para download`}
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-4">
              {/* Lista de Remessas Geradas */}
              {remessasGeradas.map((remessa, index) => (
                <div
                  key={remessa.id}
                  className="bg-muted/50 rounded-lg p-4 space-y-3 border border-border"
                >
                  <div className="flex items-center justify-between">
                    <p className="text-sm font-semibold">
                      Remessa {index + 1} - {remessa.tipo}
                    </p>
                    <StatusBadge
                      status={
                        remessa.status === "ARQUIVO GERADO"
                          ? "Aguardando Retorno"
                          : remessa.status === "FINALIZADO"
                          ? "Processada"
                          : remessa.status || "Processando"
                      }
                      variant="reembolso"
                    />
                  </div>

                  <div className="grid grid-cols-2 md:grid-cols-3 gap-3 text-sm">
                    <div>
                      <span className="text-muted-foreground">Data Processamento:</span>{" "}
                      <span className="font-medium block">
                        {remessa.mesAnoProcessamento || remessa.competencia || "N/A"}
                      </span>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Registros:</span>{" "}
                      <span className="font-medium block">
                        {remessa.lancamentos.length}
                      </span>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Valor Total:</span>{" "}
                      <span className="font-medium text-primary block">
                        {formatCurrency(remessa.valorTotal)}
                      </span>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Data de Criação:</span>{" "}
                      <span className="font-medium block">
                        {remessa.dataCriacao
                          ? format(new Date(remessa.dataCriacao), "dd/MM/yyyy HH:mm")
                          : "N/A"}
                      </span>
                    </div>
                  </div>

                  {/* Botão de Download */}
                  <Button
                    onClick={() => handleDownloadRemessa(remessa)}
                    disabled={!remessa.nomeArquivoRemessa}
                    className="w-full bg-primary hover:bg-primary/90"
                    size="lg"
                  >
                    <Download className="h-5 w-5 mr-2" />
                    {remessa.nomeArquivoRemessa
                      ? "Baixar Arquivo da Remessa"
                      : "Arquivo em processamento..."}
                  </Button>

                  {!remessa.nomeArquivoRemessa && (
                    <p className="text-xs text-muted-foreground text-center">
                      O arquivo está sendo gerado. Aguarde alguns instantes e tente novamente.
                    </p>
                  )}
                </div>
              ))}
            </div>

            <div className="flex justify-end gap-3">
              <Button
                variant="outline"
                onClick={() => {
                  setModalRemessaGeradaOpen(false);
                  setRemessasGeradas([]);
                }}
              >
                Fechar
              </Button>
            </div>
          </DialogContent>
        </Dialog>

        {/* Modal: Upload Retorno */}
        <Dialog
          open={modalUploadRetornoOpen}
          onOpenChange={setModalUploadRetornoOpen}
        >
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle className="text-2xl font-bold">
                Upload Retorno CNAB
              </DialogTitle>
              <DialogDescription>
                Faça upload do arquivo de retorno para processar as baixas
                automaticamente
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-4">
              <div className="bg-muted/50 rounded-lg p-4 space-y-2">
                <p className="text-sm font-medium">Informações da Remessa</p>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <div>
                    <span className="text-muted-foreground">Data Processamento:</span>{" "}
                    {remessaSelecionada?.mesAnoProcessamento || remessaSelecionada?.competencia}
                  </div>
                  <div>
                    <span className="text-muted-foreground">Tipo:</span>{" "}
                    {remessaSelecionada?.tipo}
                  </div>
                  <div>
                    <span className="text-muted-foreground">Registros:</span>{" "}
                    {remessaSelecionada?.lancamentos.length}
                  </div>
                  <div>
                    <span className="text-muted-foreground">Valor Total:</span>{" "}
                    {remessaSelecionada &&
                      formatCurrency(remessaSelecionada.valorTotal)}
                  </div>
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="arquivoRetorno">
                  Arquivo de Retorno <span className="text-destructive">*</span>
                </Label>
                <Input
                  id="arquivoRetorno"
                  type="file"
                  accept=".txt,.ret"
                  onChange={(e) => {
                    const file = e.target.files?.[0];
                    if (file) {
                      setArquivoRetorno(file);
                    }
                  }}
                />
                {arquivoRetorno && (
                  <p className="text-sm text-muted-foreground">
                    Arquivo selecionado: {arquivoRetorno.name}
                  </p>
                )}
              </div>

              <div className="bg-amber-50 dark:bg-amber-950/20 border border-amber-200 dark:border-amber-800 rounded-lg p-4">
                <p className="text-sm text-amber-800 dark:text-amber-200">
                  <strong>Atenção:</strong> Ao fazer upload do retorno, todas as
                  baixas serão processadas automaticamente nos lançamentos desta
                  remessa.
                </p>
              </div>
            </div>

            <div className="flex justify-end gap-3">
              <Button
                variant="outline"
                onClick={() => {
                  setModalUploadRetornoOpen(false);
                  setArquivoRetorno(null);
                }}
              >
                Cancelar
              </Button>
              <Button
                onClick={handleConfirmarUploadRetorno}
                disabled={!arquivoRetorno || processando}
                className="bg-primary hover:bg-primary/90"
              >
                {processando ? (
                  <>
                    <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                    Processando...
                  </>
                ) : (
                  <>
                    <Upload className="h-4 w-4 mr-2" />
                    Processar Retorno
                  </>
                )}
              </Button>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}
