import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";
import { Textarea } from "@/components/ui/textarea";
import { Checkbox } from "@/components/ui/checkbox";
import { Download, CheckCircle, XCircle, ArrowLeft, AlertTriangle } from "@/components/ui/system-icons";
import { Badge } from "@/components/ui/badge";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/accordion";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { differenceInDays } from "date-fns";
import { DataTable, PageBreadcrumb, PageHeader } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { formatCurrency } from "@shared/utils/calculations";
import { StatusBadge } from "@presentation/components/common";
import type { SolicitacaoAprovacaoPorColaborador, SolicitacaoDocumentoAprovacao, SolicitacaoAprovacaoAgrupada } from "@domain/entities/SolicitacaoReembolso";
import { container } from "@core/di/container";
import { DiTokens } from "@core/di/tokens";
import { ReembolsosApi } from "@data/api/ReembolsosApi";
import { ProjetosApi } from "@data/api/ProjetosApi";
import { MapaAlocacaoApi } from "@data/api/MapaAlocacaoApi";
import { GetVerbasUseCase } from "@domain/usecases/GetVerbasUseCase";
import { useAppSelector } from "../../app/store/hooks";
import { processarUrlComToken } from "@shared/utils/urlUtils";

const formatDateDisplay = (dateString: string): string => {
  if (!dateString) return '-'
  try {
    const date = new Date(dateString)
    return date.toLocaleDateString('pt-BR')
  } catch {
    return dateString
  }
}

export default function AprovarReembolso() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { token, user } = useAppSelector((state) => state.auth);
  
  const codColab = searchParams.get('codColab') || '';
  const nomeColaborador = decodeURIComponent(searchParams.get('nome') || '');
  
  const [cliente, setCliente] = useState<string | undefined>(undefined);
  const [projeto, setProjeto] = useState<string | undefined>(undefined);
  const [clientesList, setClientesList] = useState<any[]>([]);
  const [projetosList, setProjetosList] = useState<any[]>([]);
  const [_solicitacoes, setSolicitacoes] = useState<SolicitacaoAprovacaoPorColaborador[]>([]);
  const [solicitacoesFiltradas, setSolicitacoesFiltradas] = useState<SolicitacaoAprovacaoPorColaborador[]>([]);
  const [solicitacoesAgrupadas, setSolicitacoesAgrupadas] = useState<SolicitacaoAprovacaoAgrupada[]>([]);
  const [isAgrupado, setIsAgrupado] = useState(false);
  const [selectedIds, setSelectedIds] = useState<Set<number>>(new Set());
  const [loading, setLoading] = useState(true);
  const [modalDocumentosOpen, setModalDocumentosOpen] = useState(false);
  const [documentosSelecionados, setDocumentosSelecionados] = useState<SolicitacaoDocumentoAprovacao[]>([]);
  const [modalAprovarOpen, setModalAprovarOpen] = useState(false);
  const [modalReprovarOpen, setModalReprovarOpen] = useState(false);
  const [justificativa, setJustificativa] = useState("");
  const [processando, setProcessando] = useState(false);
  const [validadeComprovanteDias, setValidadeComprovanteDias] = useState<number>(30);
  const [verbasMap, setVerbasMap] = useState<Map<number, { valor: number }>>(new Map());
  
  // Verificar se o usuário tem a funcionalidade PARAMETRIZACAO_REEMBOLSO ativa
  const temParametrizacaoReembolso = user?.funcionalidadeSistema?.some(
    (func) => func.descricao === "PARAMETRIZACAO_REEMBOLSO" && func.ativo === true
  ) ?? false;

  // Função auxiliar para verificar se pode mostrar checkbox para uma solicitação
  const podeMostrarCheckbox = (statusId: number): boolean => {
    // statusId === 1: sempre mostrar
    if (statusId === 1) {
      return true;
    }
    // statusId === 5: mostrar apenas se tiver a funcionalidade
    if (statusId === 5) {
      return temParametrizacaoReembolso;
    }
    return false;
  };
  
  
  
  // Função para validar se a data excedeu o limite
  const validarDataComprovante = (dataDespesa: string): { excedido: boolean; diasExcedidos: number } => {
    if (!dataDespesa) {
      return { excedido: false, diasExcedidos: 0 };
    }
    
    try {
      const dataDate = new Date(dataDespesa);
      if (isNaN(dataDate.getTime())) {
        return { excedido: false, diasExcedidos: 0 };
      }
      
      const hoje = new Date();
      const diasDiferenca = differenceInDays(hoje, dataDate);
      const excedeu = diasDiferenca > validadeComprovanteDias;
      
      if (excedeu) {
        return { excedido: true, diasExcedidos: diasDiferenca - validadeComprovanteDias };
      }
      
      return { excedido: false, diasExcedidos: 0 };
    } catch {
      return { excedido: false, diasExcedidos: 0 };
    }
  };
  
  // Função para validar se o valor excedeu o teto
  const validarValorTeto = (valor: number, categoriaId: number): boolean => {
    const verba = verbasMap.get(categoriaId);
    if (!verba) {
      return false; // Se não encontrar a verba, não mostrar alerta
    }
    return valor > verba.valor;
  };
  
  // Carregar clientes
  useEffect(() => {
    const loadClientes = async () => {
      if (!token) return;
      
      try {
        const projetosApi = container.resolve<ProjetosApi>(DiTokens.projetosApi);
        const response = await projetosApi.listarClientesOrg(token);
        const clientes = Array.isArray(response) ? response : ((response as any)?.retorno || []);
        setClientesList(clientes);
      } catch (err) {
        console.error('Erro ao carregar clientes:', err);
      }
    };
    
    loadClientes();
  }, [token]);
  
  // Carregar parâmetro validadeComprovanteDias
  useEffect(() => {
    const loadParametros = async () => {
      if (!token) return;
      
      try {
        const getVerbasUseCase = container.resolve(GetVerbasUseCase);
        const response = await getVerbasUseCase.obterVerbasEParametroValidacao(token);
        if (response.sucesso && response.retorno?.parametroReembolso) {
          setValidadeComprovanteDias(response.retorno.parametroReembolso.validadeComprovanteDias || 30);
        }
      } catch (err) {
        console.error('Erro ao carregar parâmetros:', err);
      }
    };
    
    loadParametros();
  }, [token]);
  
  // Carregar projetos quando cliente mudar
  useEffect(() => {
    const loadProjetos = async () => {
      if (!token || !cliente || cliente === 'todos') {
        setProjetosList([]);
        return;
      }
      
      try {
        const mapaAlocacaoApi = container.resolve<MapaAlocacaoApi>(DiTokens.mapaAlocacaoApi);
        const response = await mapaAlocacaoApi.listarProjetosColaborador(token, {
          codigoProfissional: '',
          ehTbd: null,
          codigoGerenteProjeto: '',
          listaCodigoCliente: [cliente],
          status: '',
          prioritarioFiltro: 0,
        });
        
        if (response.sucesso && response.Projetos) {
          const projetos = response.Projetos.map(p => ({
            ...p,
            label: `${p.codigoProjeto} - ${p.projetos}`,
          }));
          setProjetosList(projetos);
        } else {
          setProjetosList([]);
        }
      } catch (err) {
        console.error('Erro ao carregar projetos:', err);
        setProjetosList([]);
      }
    };
    
    loadProjetos();
  }, [token, cliente]);
  
  // Carregar solicitações
  useEffect(() => {
    const loadSolicitacoes = async () => {
      if (!token || !codColab) {
        setLoading(false);
        return;
      }
      
      try {
        setLoading(true);
        const reembolsosApi = container.resolve<ReembolsosApi>(DiTokens.reembolsosApi);
        const response = await reembolsosApi.listarSolicitacoesAprovacaoPorColaborador(
          token,
          codColab,
          {
            clienteId: cliente && cliente !== 'todos' ? cliente : "",
            projetoId: projeto && projeto !== 'todos' ? projeto : ""
          }
        );
        
        if (response.sucesso && response.retorno) {
          // Verificar se o retorno é um array e não está vazio
          if (!Array.isArray(response.retorno) || response.retorno.length === 0) {
            setSolicitacoes([]);
            setSolicitacoesFiltradas([]);
            setSolicitacoesAgrupadas([]);
            setIsAgrupado(false);
            setLoading(false);
            return;
          }
          
          // A resposta sempre vem agrupada agora
          const grupos = response.retorno as SolicitacaoAprovacaoAgrupada[];
          setSolicitacoesAgrupadas(grupos);
          setIsAgrupado(true);
          
          // Flatten para manter compatibilidade com código existente
          const todasSolicitacoes = grupos.flatMap(g => (g.objeto || [])).filter(s => s && s.id);
          setSolicitacoes(todasSolicitacoes);
          setSolicitacoesFiltradas(todasSolicitacoes);
          
          // Buscar verbas para todas as categorias únicas
          const categoriasUnicas = new Set(todasSolicitacoes.map(s => s.categoriaId));
          loadVerbasForCategorias(Array.from(categoriasUnicas), todasSolicitacoes);
        }
      } catch (err) {
        console.error('Erro ao carregar solicitações:', err);
        setSolicitacoes([]);
        setSolicitacoesFiltradas([]);
      } finally {
        setLoading(false);
      }
    };
    
    loadSolicitacoes();
  }, [token, codColab, cliente, projeto]);
  
  // Função para carregar verbas para categorias específicas
  const loadVerbasForCategorias = async (categoriaIds: number[], solicitacoesData: SolicitacaoAprovacaoPorColaborador[]) => {
    if (!token || categoriaIds.length === 0) return;
    
    try {
      const getVerbasUseCase = container.resolve(GetVerbasUseCase);
      // Buscar verbas para cada projeto/cliente único nas solicitações
      const projetosClientesUnicos = new Set<string>();
      solicitacoesData.forEach(s => {
        // Extrair código do cliente e projeto
        // Pode vir do campo clienteProjeto (formato antigo) ou dos campos separados
        if (s.clienteProjeto) {
          // Formato antigo: "0001 - Royal / 16845496000134 - R-TRADE..."
          const match = s.clienteProjeto.match(/^(\d+)\s*-\s*[^/]+\s*\/\s*(\d+)/);
          if (match) {
            const codigoCliente = match[1];
            const codigoProjeto = match[2];
            projetosClientesUnicos.add(`${codigoCliente}|${codigoProjeto}`);
          }
        }
        // Se não tiver clienteProjeto, não podemos extrair os códigos, mas isso não é crítico
      });
      
      // Buscar verbas para cada projeto/cliente único
      const verbasMapTemp = new Map<number, { valor: number }>();
      
      for (const projetoCliente of projetosClientesUnicos) {
        const [codigoCliente, codigoProjeto] = projetoCliente.split('|');
        if (!codigoCliente || !codigoProjeto) continue;
        try {
          const response = await getVerbasUseCase.listarVerbasComExcecao(
            token,
            codigoProjeto,
            codigoCliente
          );
          
          if (response.sucesso && response.retorno) {
            response.retorno.forEach(verba => {
              if (verba.id && categoriaIds.includes(verba.id)) {
                verbasMapTemp.set(verba.id, { valor: verba.valor || 0 });
              }
            });
          }
        } catch (err) {
          console.error(`Erro ao carregar verbas para ${projetoCliente}:`, err);
        }
      }
      
      setVerbasMap(verbasMapTemp);
    } catch (err) {
      console.error('Erro ao carregar verbas:', err);
    }
  };
  
  // Filtrar solicitações pendentes para seleção
  // statusId 1: sempre pode selecionar
  // statusId 5: apenas se tiver a funcionalidade PARAMETRIZACAO_REEMBOLSO
  const solicitacoesPendentes = solicitacoesFiltradas.filter(s => podeMostrarCheckbox(s.statusId));
  const todasSelecionadas = solicitacoesPendentes.length > 0 && 
    solicitacoesPendentes.every(s => selectedIds.has(s.id));
  
  const handleSelectAll = (checked: boolean) => {
    if (checked) {
      const novosIds = new Set(selectedIds);
      solicitacoesPendentes.forEach(s => novosIds.add(s.id));
      setSelectedIds(novosIds);
    } else {
      const novosIds = new Set(selectedIds);
      solicitacoesPendentes.forEach(s => novosIds.delete(s.id));
      setSelectedIds(novosIds);
    }
  };

  // Função para selecionar todos de um grupo
  const handleSelectAllGroup = (grupoIndex: number, checked: boolean) => {
    const grupo = solicitacoesAgrupadas[grupoIndex];
    if (!grupo || !grupo.objeto) return;
    
    const solicitacoesGrupo = grupo.objeto.filter(s => s && s.id);
    const solicitacoesPendentesGrupo = solicitacoesGrupo.filter(s => podeMostrarCheckbox(s.statusId));
    const novosIds = new Set(selectedIds);
    
    if (checked) {
      solicitacoesPendentesGrupo.forEach(s => novosIds.add(s.id));
    } else {
      solicitacoesPendentesGrupo.forEach(s => novosIds.delete(s.id));
    }
    
    setSelectedIds(novosIds);
  };

  // Função para verificar se todos de um grupo estão selecionados
  const todosSelecionadosGrupo = (grupoIndex: number): boolean => {
    const grupo = solicitacoesAgrupadas[grupoIndex];
    if (!grupo || !grupo.objeto) return false;
    
    const solicitacoesGrupo = grupo.objeto.filter(s => s && s.id);
    const solicitacoesPendentesGrupo = solicitacoesGrupo.filter(s => podeMostrarCheckbox(s.statusId));
    if (solicitacoesPendentesGrupo.length === 0) return false;
    
    return solicitacoesPendentesGrupo.every(s => selectedIds.has(s.id));
  };
  
  const handleSelectItem = (id: number, checked: boolean) => {
    const novosIds = new Set(selectedIds);
    if (checked) {
      novosIds.add(id);
    } else {
      novosIds.delete(id);
    }
    setSelectedIds(novosIds);
  };
  
  const handleLimpar = () => {
    setCliente(undefined);
    setProjeto(undefined);
  };
  
  const handleAprovar = () => {
    setModalAprovarOpen(true);
  };
  
  const handleReprovar = () => {
    setJustificativa("");
    setModalReprovarOpen(true);
  };
  
  const confirmarAprovar = async () => {
    if (!token || selectedIds.size === 0) {
      setModalAprovarOpen(false);
      return;
    }
    
    try {
      setProcessando(true);
      console.log('Chamando API para aprovar:', Array.from(selectedIds));
      const reembolsosApi = container.resolve<ReembolsosApi>(DiTokens.reembolsosApi);
      const response = await reembolsosApi.aprovarSolicitacoes(
        token,
        Array.from(selectedIds),
        ""
      );
      
      if (response.sucesso) {
        // Recarregar solicitações
        const loadResponse = await reembolsosApi.listarSolicitacoesAprovacaoPorColaborador(
          token,
          codColab,
          {
            clienteId: cliente && cliente !== 'todos' ? cliente : "",
            projetoId: projeto && projeto !== 'todos' ? projeto : ""
          }
        );
        
        if (loadResponse.sucesso && loadResponse.retorno) {
          // Verificar se o retorno é um array e não está vazio
          if (!Array.isArray(loadResponse.retorno) || loadResponse.retorno.length === 0) {
            setSolicitacoes([]);
            setSolicitacoesFiltradas([]);
            setSolicitacoesAgrupadas([]);
            setIsAgrupado(false);
            return;
          }
          
          // A resposta sempre vem agrupada agora
          const grupos = loadResponse.retorno as SolicitacaoAprovacaoAgrupada[];
          setSolicitacoesAgrupadas(grupos);
          setIsAgrupado(true);
          const todasSolicitacoes = grupos.flatMap(g => (g.objeto || [])).filter(s => s && s.id);
          setSolicitacoes(todasSolicitacoes);
          setSolicitacoesFiltradas(todasSolicitacoes);
        }
        
        setSelectedIds(new Set());
        setModalAprovarOpen(false);
      }
    } catch (err) {
      console.error('Erro ao aprovar solicitações:', err);
    } finally {
      setProcessando(false);
    }
  };
  
  const confirmarReprovar = async () => {
    if (!token || selectedIds.size === 0 || !justificativa.trim()) {
      setModalReprovarOpen(false);
      return;
    }
    
    try {
      setProcessando(true);
      console.log('Chamando API para reprovar:', Array.from(selectedIds), justificativa);
      const reembolsosApi = container.resolve<ReembolsosApi>(DiTokens.reembolsosApi);
      const response = await reembolsosApi.reprovarSolicitacoes(
        token,
        Array.from(selectedIds),
        justificativa.trim()
      );
      
      if (response.sucesso) {
        // Recarregar solicitações
        const loadResponse = await reembolsosApi.listarSolicitacoesAprovacaoPorColaborador(
          token,
          codColab,
          {
            clienteId: cliente && cliente !== 'todos' ? cliente : "",
            projetoId: projeto && projeto !== 'todos' ? projeto : ""
          }
        );
        
        if (loadResponse.sucesso && loadResponse.retorno) {
          // Verificar se o retorno é um array e não está vazio
          if (!Array.isArray(loadResponse.retorno) || loadResponse.retorno.length === 0) {
            setSolicitacoes([]);
            setSolicitacoesFiltradas([]);
            setSolicitacoesAgrupadas([]);
            setIsAgrupado(false);
            return;
          }
          
          // A resposta sempre vem agrupada agora
          const grupos = loadResponse.retorno as SolicitacaoAprovacaoAgrupada[];
          setSolicitacoesAgrupadas(grupos);
          setIsAgrupado(true);
          const todasSolicitacoes = grupos.flatMap(g => (g.objeto || [])).filter(s => s && s.id);
          setSolicitacoes(todasSolicitacoes);
          setSolicitacoesFiltradas(todasSolicitacoes);
        }
        
        setSelectedIds(new Set());
        setJustificativa("");
        setModalReprovarOpen(false);
      }
    } catch (err) {
      console.error('Erro ao reprovar solicitações:', err);
    } finally {
      setProcessando(false);
    }
  };
  
  const columns: Column[] = [
    { id: "selecao", label: "Seleção", sortable: false, width: "w-[80px]" },
    { id: "clienteProjeto", label: "Cliente/Projeto", sortable: true },
    { id: "categoria", label: "Categoria", sortable: true },
    { id: "comprovante", label: "Comprovante", sortable: false, width: "w-[120px]" },
    { id: "dataDespesa", label: "Data despesa", sortable: true },
    { id: "valor", label: "Valor", sortable: true },
    { id: "descricao", label: "Descrição", sortable: true },
    { id: "status", label: "Status", sortable: true, width: "w-[120px]" },
  ];
  
  const renderCell = (solicitacao: SolicitacaoAprovacaoPorColaborador, columnId: string) => {
    switch (columnId) {
      case "selecao":
        // statusId === 1: sempre mostrar checkbox
        // statusId === 5: mostrar apenas se tiver a funcionalidade PARAMETRIZACAO_REEMBOLSO
        if (podeMostrarCheckbox(solicitacao.statusId)) {
          return (
            <Checkbox
              checked={selectedIds.has(solicitacao.id)}
              onCheckedChange={(checked) => handleSelectItem(solicitacao.id, checked as boolean)}
            />
          );
        }
        return null;
      case "clienteProjeto":
        const clienteProjetoText = solicitacao.clienteProjeto || 
          `${solicitacao.clienteDescricao || ''} / ${solicitacao.projetoDescricao || ''}`.trim();
        return (
          <div className="max-w-[200px] truncate" title={clienteProjetoText}>
            <span className="font-medium">{clienteProjetoText || '-'}</span>
          </div>
        );
      case "categoria":
        return <span>{solicitacao.categoriaDescricao}</span>;
      case "comprovante":
        if (solicitacao.solicitacaoDocumentos && solicitacao.solicitacaoDocumentos.length > 0) {
          return (
            <Button
              variant="ghost"
              size="sm"
              onClick={() => {
                setDocumentosSelecionados(solicitacao.solicitacaoDocumentos);
                setModalDocumentosOpen(true);
              }}
            >
              <Download className="h-4 w-4 mr-2" />
              {solicitacao.solicitacaoDocumentos.length}
            </Button>
          );
        }
        return <span className="text-muted-foreground">-</span>;
      case "dataDespesa": {
        const validacaoData = validarDataComprovante(solicitacao.dataDespesa);
        return (
          <div className="flex items-center gap-2">
            <span>{formatDateDisplay(solicitacao.dataDespesa)}</span>
            {validacaoData.excedido && (
              <Tooltip>
                <TooltipTrigger asChild>
                  <AlertTriangle className="h-4 w-4 text-amber-500 flex-shrink-0" />
                </TooltipTrigger>
                <TooltipContent>
                  <p>A data do comprovante excedeu o limite permitido de {validadeComprovanteDias} dia(s). Foram excedidos {validacaoData.diasExcedidos} dia(s).</p>
                </TooltipContent>
              </Tooltip>
            )}
          </div>
        );
      }
      case "valor": {
        const valorExcedido = validarValorTeto(solicitacao.valor, solicitacao.categoriaId);
        const verba = verbasMap.get(solicitacao.categoriaId);
        return (
          <div className="flex items-center gap-2">
            <span className="font-semibold">{formatCurrency(solicitacao.valor)}</span>
            {valorExcedido && verba && (
              <Tooltip>
                <TooltipTrigger asChild>
                  <AlertTriangle className="h-4 w-4 text-amber-500 flex-shrink-0" />
                </TooltipTrigger>
                <TooltipContent>
                  <p>O valor máximo permitido para este reembolso é de {formatCurrency(verba.valor)}.</p>
                </TooltipContent>
              </Tooltip>
            )}
          </div>
        );
      }
      case "descricao":
        return <span>{solicitacao.descricao || '-'}</span>;
      case "status":
        return <StatusBadge status={solicitacao.statusDescricao} variant="reembolso" />;
      default:
        return null;
    }
  };
  
  return (
    <TooltipProvider>
    <div className="container mx-auto p-4 md:p-6 space-y-6 pb-32">
      <PageBreadcrumb 
        items={[
          { label: 'Reembolsos', href: '/reembolso' },
          { label: 'Aprovar Reembolso' }
        ]} 
      />
      
      {/* Header com botões */}
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => navigate('/reembolso?tab=aprovacoes')}
          className="h-10 w-10"
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <PageHeader 
          title="Aprovar Reembolso"
          description="Gerencie as aprovações de reembolso"
        />
      </div>
      
      {/* Filtros */}
      <Card>
        <CardContent className="p-4 md:p-6">
          <div className="flex flex-wrap gap-4 items-end">
            <div className="space-y-2 w-full sm:flex-1 sm:min-w-[200px]">
              <Label>Cliente</Label>
              <Select value={cliente || "todos"} onValueChange={(value) => {
                setCliente(value === "todos" ? undefined : value);
                setProjeto(undefined);
              }}>
                <SelectTrigger className="bg-background">
                  <SelectValue placeholder="Selecione o cliente" />
                </SelectTrigger>
                <SelectContent className="bg-white dark:bg-gray-800 z-50">
                  <SelectItem value="todos">Todos</SelectItem>
                  {clientesList.map((clienteItem) => (
                    <SelectItem key={clienteItem.codigoCliente} value={clienteItem.codigoCliente}>
                      {clienteItem.labelCodigoCliente}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div className="space-y-2 w-full sm:flex-1 sm:min-w-[200px]">
              <Label>Projeto</Label>
              <Select
                value={projeto || "todos"}
                onValueChange={(value) => setProjeto(value === "todos" ? undefined : value)}
                disabled={!cliente || cliente === "todos"}
              >
                <SelectTrigger className="bg-background">
                  <SelectValue placeholder={cliente ? "Selecione o projeto" : "Selecione um cliente primeiro"} />
                </SelectTrigger>
                <SelectContent className="bg-white dark:bg-gray-800 z-50">
                  <SelectItem value="todos">Todos</SelectItem>
                  {projetosList.map((projetoItem) => (
                    <SelectItem key={projetoItem.codigoProjeto} value={projetoItem.codigoProjeto}>
                      {projetoItem.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div className="flex gap-3 w-full sm:w-auto">
              <Button variant="outline" onClick={handleLimpar} className="flex-1 sm:flex-initial">Limpar</Button>
            </div>
          </div>
        </CardContent>
      </Card>
      
      {/* Informações do colaborador */}
      <Card>
        <CardContent className="p-4 md:p-6">
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <div>
              <h2 className="text-xl font-bold">{nomeColaborador}</h2>
              <p className="text-muted-foreground mt-1">Colaborador</p>
            </div>
          </div>
        </CardContent>
      </Card>
      
      {/* Tabela ou Agrupador */}
      <Card>
        <CardContent className="p-4 md:p-6">
          {loading ? (
            <div className="text-center py-8 text-muted-foreground">Carregando solicitações...</div>
          ) : isAgrupado && solicitacoesAgrupadas.length > 0 ? (
            // Renderização agrupada com Accordion
            <div className="space-y-4">
              {solicitacoesPendentes.length > 0 && (
                <div className="mb-4 flex items-center gap-2">
                  <Checkbox
                    checked={todasSelecionadas}
                    onCheckedChange={handleSelectAll}
                  />
                  <Label className="text-sm font-medium">
                    Selecionar todos ({solicitacoesPendentes.length} pendentes)
                  </Label>
                </div>
              )}
              <Accordion type="multiple" className="w-full">
                {solicitacoesAgrupadas
                  .map((grupo, originalIndex) => ({ grupo, originalIndex }))
                  .sort((a, b) => {
                    // Primeiro ordenar por data da solicitação (mais recente primeiro)
                    const dataA = new Date(a.grupo.dataSolicitacao || 0).getTime();
                    const dataB = new Date(b.grupo.dataSolicitacao || 0).getTime();
                    if (dataB !== dataA) {
                      return dataB - dataA; // Mais recente primeiro
                    }
                    
                    // Se a data for igual, ordenar por pendentes (pendentes primeiro)
                    const pendentesA = (a.grupo.objeto || []).filter(s => s && s.id && podeMostrarCheckbox(s.statusId)).length;
                    const pendentesB = (b.grupo.objeto || []).filter(s => s && s.id && podeMostrarCheckbox(s.statusId)).length;
                    return pendentesB - pendentesA; // Mais pendentes primeiro
                  })
                  .map(({ grupo, originalIndex }) => {
                    const grupoIndex = originalIndex;
                  const solicitacoesGrupo = (grupo.objeto || []).filter(s => s && s.id);
                  const solicitacoesPendentesGrupo = solicitacoesGrupo.filter(s => podeMostrarCheckbox(s.statusId));
                  const todosSelecionadosNoGrupo = todosSelecionadosGrupo(grupoIndex);
                  const grupoId = `grupo-${grupoIndex}-${grupo.dataSolicitacao}`;
                  const grupoNome = grupo.objetivo || grupo.periodo || `${grupo.cliente} / ${grupo.projeto}`;
                  
                  // Verificar se há divergências no grupo (data excedida ou valor excedido)
                  const temDivergencias = solicitacoesGrupo.some(s => {
                    const validacaoData = validarDataComprovante(s.dataDespesa);
                    const valorExcedido = validarValorTeto(s.valor, s.categoriaId);
                    return validacaoData.excedido || valorExcedido;
                  });
                  
                  return (
                    <AccordionItem key={grupoId} value={grupoId} className="border rounded-lg mb-2 px-4">
                      <AccordionTrigger className="hover:no-underline">
                        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between w-full pr-4 gap-3">
                          <div className="flex items-start gap-3 flex-1 min-w-0">
                            {solicitacoesPendentesGrupo.length > 0 && (
                              <Checkbox
                                checked={todosSelecionadosNoGrupo}
                                onCheckedChange={(checked) => handleSelectAllGroup(grupoIndex, checked as boolean)}
                                onClick={(e) => e.stopPropagation()}
                                className="mt-1 shrink-0"
                              />
                            )}
                            <div className="text-left flex-1 min-w-0">
                              <div className="flex items-center gap-2 flex-wrap">
                                <div className="font-semibold text-base break-words">{grupoNome}</div>
                                {temDivergencias && (
                                  <Tooltip>
                                    <TooltipTrigger asChild>
                                      <AlertTriangle className="h-4 w-4 text-amber-500 flex-shrink-0" />
                                    </TooltipTrigger>
                                    <TooltipContent>
                                      <p>Este grupo contém solicitações com divergências (data ou valor excedido).</p>
                                    </TooltipContent>
                                  </Tooltip>
                                )}
                              </div>
                              <div className="text-sm text-muted-foreground break-words">
                                {solicitacoesGrupo.length} solicitação(ões) • {formatCurrency(grupo.somaValores || 0)}
                              </div>
                              {grupo.periodo && (
                                <div className="text-xs text-muted-foreground mt-1 break-words">
                                  Período: {grupo.periodo}
                                </div>
                              )}
                              {grupo.destino && (
                                <div className="text-xs text-muted-foreground mt-1 break-words">
                                  Destino: {grupo.destino}
                                </div>
                              )}
                            </div>
                          </div>
                          {solicitacoesPendentesGrupo.length > 0 && (
                            <div className="flex items-center gap-3 shrink-0 w-full sm:w-auto justify-between sm:justify-end">
                              <div className="text-right sm:text-right">
                                <p className="text-lg sm:text-xl font-bold text-amber-600">
                                  {formatCurrency(solicitacoesPendentesGrupo.reduce((sum, s) => sum + s.valor, 0))}
                                </p>
                                <p className="text-xs font-semibold text-amber-600">Total Pendente</p>
                              </div>
                              <Badge variant="secondary" className="shrink-0">
                                {solicitacoesPendentesGrupo.length} pendente{solicitacoesPendentesGrupo.length > 1 ? 's' : ''}
                              </Badge>
                            </div>
                          )}
                        </div>
                      </AccordionTrigger>
                      <AccordionContent>
                        <div className="pt-2">
                          <DataTable
                            columns={columns}
                            data={solicitacoesGrupo}
                            keyExtractor={(item) => item?.id?.toString() || Math.random().toString()}
                            renderCell={renderCell}
                            emptyMessage="Nenhuma solicitação encontrada"
                          />
                        </div>
                      </AccordionContent>
                    </AccordionItem>
                  );
                })}
              </Accordion>
            </div>
          ) : (
            // Renderização não agrupada (formato antigo)
            <>
              {solicitacoesPendentes.length > 0 && (
                <div className="mb-4 flex items-center gap-2">
                  <Checkbox
                    checked={todasSelecionadas}
                    onCheckedChange={handleSelectAll}
                  />
                  <Label className="text-sm font-medium">
                    Selecionar todos ({solicitacoesPendentes.length} pendentes)
                  </Label>
                </div>
              )}
              <DataTable
                columns={columns}
                data={solicitacoesFiltradas || []}
                keyExtractor={(item) => item?.id?.toString() || Math.random().toString()}
                renderCell={renderCell}
                emptyMessage="Nenhuma solicitação encontrada"
              />
            </>
          )}
        </CardContent>
      </Card>
      
      {/* Modal de Documentos */}
      <Dialog open={modalDocumentosOpen} onOpenChange={setModalDocumentosOpen}>
        <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                  <Download className="h-5 w-5 text-primary" />
                </div>
                <DialogTitle className="text-2xl font-bold">Documentos</DialogTitle>
              </div>
            </div>
            <DialogDescription>
              Visualize e baixe os documentos anexados
            </DialogDescription>
          </DialogHeader>
          
          <div className="space-y-4 py-4">
            {documentosSelecionados.length > 0 ? (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {documentosSelecionados.map((documento) => (
                  <Card key={documento.id} className="hover:shadow-md transition-shadow">
                    <CardContent className="p-4">
                      <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3 flex-1 min-w-0">
                          <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center shrink-0">
                            <Download className="h-5 w-5 text-primary" />
                          </div>
                          <div className="flex-1 min-w-0">
                            <p className="font-medium truncate">Documento {documento.id}</p>
                            <p className="text-sm text-muted-foreground uppercase">{documento.tipo}</p>
                          </div>
                        </div>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => {
                            const urlProcessada = processarUrlComToken(documento.url, token);
                            if (urlProcessada) {
                              window.open(urlProcessada, "_blank");
                            } else {
                              console.error('Erro ao processar URL do documento');
                            }
                          }}
                        >
                          <Download className="h-4 w-4 mr-2" />
                          Baixar
                        </Button>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            ) : (
              <div className="text-center py-8 text-muted-foreground">
                Nenhum documento disponível
              </div>
            )}
          </div>
        </DialogContent>
      </Dialog>
    </div>
    
    {/* FAB Buttons - Fixed at bottom right */}
    <div className="fixed bottom-6 right-6 z-50 flex flex-row gap-3">
      <Button
        onClick={handleAprovar}
        disabled={selectedIds.size === 0}
        size="lg"
        className="bg-green-600 hover:bg-green-700 text-white shadow-lg hover:shadow-xl transition-all duration-200 rounded-full h-12 px-6 min-w-[140px]"
      >
        <CheckCircle className="h-5 w-5 mr-2" />
        Aprovar
      </Button>
      <Button
        onClick={handleReprovar}
        disabled={selectedIds.size === 0}
        variant="destructive"
        size="lg"
        className="shadow-lg hover:shadow-xl transition-all duration-200 rounded-full h-12 px-6 min-w-[140px]"
      >
        <XCircle className="h-5 w-5 mr-2" />
        Reprovar
      </Button>
    </div>
    
    {/* Modal de Confirmação - Aprovar */}
    <AlertDialog open={modalAprovarOpen} onOpenChange={setModalAprovarOpen}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Confirmar Aprovação</AlertDialogTitle>
          <AlertDialogDescription>
            Tem certeza que deseja aprovar {selectedIds.size} solicitação(ões) selecionada(s)?
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={processando}>Cancelar</AlertDialogCancel>
          <AlertDialogAction 
            onClick={async (e) => {
              e.preventDefault();
              await confirmarAprovar();
            }} 
            disabled={processando}
          >
            {processando ? "Processando..." : "Confirmar"}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
    
    {/* Modal de Confirmação - Reprovar */}
    <AlertDialog open={modalReprovarOpen} onOpenChange={(open) => {
      if (!processando) {
        setModalReprovarOpen(open);
      }
    }}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Confirmar Reprovação</AlertDialogTitle>
          <AlertDialogDescription>
            Tem certeza que deseja reprovar {selectedIds.size} solicitação(ões) selecionada(s)? 
            É obrigatório informar uma justificativa.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <div className="space-y-2 py-4">
          <Label htmlFor="justificativa">Justificativa <span className="text-destructive">*</span></Label>
          <Textarea
            id="justificativa"
            placeholder="Informe o motivo da reprovação..."
            value={justificativa}
            onChange={(e) => setJustificativa(e.target.value)}
            className="min-h-[100px]"
            disabled={processando}
          />
        </div>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={processando}>Cancelar</AlertDialogCancel>
          <AlertDialogAction 
            onClick={async (e) => {
              e.preventDefault();
              await confirmarReprovar();
            }} 
            disabled={processando || !justificativa.trim()}
            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
          >
            {processando ? "Processando..." : "Confirmar Reprovação"}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
    </TooltipProvider>
  );
}

