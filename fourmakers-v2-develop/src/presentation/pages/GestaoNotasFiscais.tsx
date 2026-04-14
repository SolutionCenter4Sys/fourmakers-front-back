import { useState, useMemo, useEffect } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Checkbox } from "@/components/ui/checkbox";
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
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { MonthPicker } from "@/components/ui/month-picker";
import { CalendarIcon, Check, ChevronsUpDown, Eye, ChevronRight, Search, CheckCircle, XCircle, AlertTriangle } from "@/components/ui/system-icons";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";
import { cn } from "@/lib/utils";
import { DataTable, TablePagination } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";

interface NotaFiscalGestao {
  id: number;
  idNotaFiscal: string; // UUID original da API para envio em Aprovar/Reprovar
  prestador: string;
  vigencia: string;
  numeroNF: string;
  valor: number;
  valorAnalisado?: number | null; // Valor analisado da API
  dataEnvio: string;
  status: string; // Usar notaFiscalStatusDescricao diretamente da API
  statusId?: number; // ID do status da API
  competencia: string;
  dataEmissao: string;
  urlNotaFiscalDownload?: string | null; // URL para download da nota fiscal
  lancamentos: Array<{
    rubrica: string;
    valor: number;
  }>;
}

import { useNotasFiscaisGestao, useNotasFiscaisPorVigencia } from "@presentation/hooks/useNotasFiscais";
import { container } from "@core/di/container";
import { GetNotasFiscaisUseCase } from "@domain/usecases/GetNotasFiscaisUseCase";
import { useAppSelector } from "@app/store/hooks";
import {
  AprovarNotasFiscaisModal,
  ReprovarNotasFiscaisModal,
  LiberarEmissaoNotasFiscaisModal,
  DetalhesNotaFiscalModal,
} from "@presentation/components/gestao-notas-fiscais";

// Função para formatar mês e ano como "MMMM/yyyy"
const formatarCompetencia = (mes: number, ano: number): string => {
  const meses = [
    "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
    "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
  ];
  return `${meses[mes - 1]}/${ano}`;
};

export default function GestaoNotasFiscais() {
  const {
    status: statusList,
    unidades,
    colaboradores,
    loading: loadingFiltros,
  } = useNotasFiscaisGestao();
  const [busca, setBusca] = useState("");
  const [unidade, setUnidade] = useState("");
  const [colaboradorOpen, setColaboradorOpen] = useState(false);
  const [colaborador, setColaborador] = useState("");
  const [status, setStatus] = useState("");
  // Inicializar mês e ano com valores atuais
  const now = new Date();
  const [mes, setMes] = useState<number>(now.getMonth() + 1);
  const [ano, setAno] = useState<number>(now.getFullYear());
  const [selectedNotas, setSelectedNotas] = useState<number[]>([]);
  
  // Estados de paginação
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10);
  
  // Estado para os parâmetros aplicados (usados na busca)
  // Inicializar com mês e ano atual por padrão
  const [apiParamsAplicados, setApiParamsAplicados] = useState<any>(() => {
    const now = new Date();
    return {
      mes: now.getMonth() + 1,
      ano: now.getFullYear(),
      cursor: 0,
      limite: 2000,
    };
  });
  
  // Modal de detalhes
  const [detalhesOpen, setDetalhesOpen] = useState(false);
  const [notaSelecionada, setNotaSelecionada] = useState<NotaFiscalGestao | null>(null);

  const handleLimpar = () => {
    setBusca("");
    setUnidade("");
    setColaborador("");
    setStatus("");
    // Resetar para mês e ano atual ao limpar
    const now = new Date();
    setMes(now.getMonth() + 1);
    setAno(now.getFullYear());
    // Resetar parâmetros aplicados para mês e ano atual
    setApiParamsAplicados({
      mes: now.getMonth() + 1,
      ano: now.getFullYear(),
      cursor: 0,
      limite: 2000,
    });
  };

  const handleAplicar = () => {
    // Preparar parâmetros para a API baseado nos filtros
    const params: any = {
      filtro: busca || undefined,
      documentoColaborador: colaborador || undefined,
      codDiretoria: unidade || undefined,
      cursor: 0,
      limite: 2000,
    };

    // Converter statusId para número se existir
    if (status) {
      const statusIdNumber = parseInt(status, 10);
      if (!isNaN(statusIdNumber)) {
        params.statusId = statusIdNumber;
      }
    }

    // Sempre usar mês e ano selecionados
    params.mes = mes;
    params.ano = ano;

    // Remover undefined values
    Object.keys(params).forEach(key => {
      if (params[key] === undefined || params[key] === '') {
        delete params[key];
      }
    });

    setApiParamsAplicados(params);
  };

  // Carregar notas fiscais da API apenas quando houver parâmetros aplicados
  const { notasFiscais: notasFiscaisApi, loading: loadingNotas, error: errorNotas, refetch: refetchNotas } = useNotasFiscaisPorVigencia(apiParamsAplicados);
  const { token } = useAppSelector((state) => state.auth);
  const [processandoAprovar, setProcessandoAprovar] = useState(false);
  const [erroAprovar, setErroAprovar] = useState<string | null>(null);
  const [modalAprovarOpen, setModalAprovarOpen] = useState(false);
  const [modalReprovarOpen, setModalReprovarOpen] = useState(false);
  const [processandoReprovar, setProcessandoReprovar] = useState(false);
  const [erroReprovar, setErroReprovar] = useState<string | null>(null);
  const [modalLiberacaoOpen, setModalLiberacaoOpen] = useState(false);
  const [processandoLiberacao, setProcessandoLiberacao] = useState(false);
  const [erroLiberacao, setErroLiberacao] = useState<string | null>(null);

  // Função auxiliar para converter UUID em número único
  const uuidToNumber = (uuid: string): number => {
    let hash = 0;
    for (let i = 0; i < uuid.length; i++) {
      const char = uuid.charCodeAt(i);
      hash = ((hash << 5) - hash) + char;
      hash = hash & hash; // Convert to 32bit integer
    }
    return Math.abs(hash);
  };

  // Mapear dados da API para o formato esperado pelo componente
  const notasFiscaisCompletas = useMemo(() => {
    return notasFiscaisApi.map((nf, index) => {
      // Formatar vigência como MM/AAAA
      const vigencia = nf.vigenciaMes && nf.vigenciaAno 
        ? `${String(nf.vigenciaMes).padStart(2, '0')}/${nf.vigenciaAno}`
        : '';
      
      // Usar sumarioValorTotalDeRubricas se valor for null
      // Se ambos forem null, calcular a soma das rubricas
      let valorTotal = nf.valor ?? nf.sumarioValorTotalDeRubricas ?? 0;
      if (valorTotal === 0 && nf.rubricas && nf.rubricas.length > 0) {
        valorTotal = nf.rubricas.reduce((sum, r) => sum + (r.valor ?? 0), 0);
      }
      
      // Usar notaFiscalStatusDescricao diretamente da API
      const status = nf.notaFiscalStatusDescricao || '';
      
      // Formatar data de emissão
      const dataEmissao = nf.dataEmissaoNotaFiscal 
        ? new Date(nf.dataEmissaoNotaFiscal).toLocaleDateString('pt-BR')
        : '';
      
      return {
        id: nf.id ? uuidToNumber(nf.id) : index + 1, // Converter UUID para número único
        idNotaFiscal: nf.id || '', // UUID original para Aprovar/Reprovar
        prestador: nf.nomeColaborador || '',
        vigencia: vigencia,
        numeroNF: nf.numeroNf || '',
        valor: valorTotal,
        valorAnalisado: nf.valorAnalisado ?? null,
        dataEnvio: dataEmissao, // Usar data de emissão como data de envio
        status: status,
        statusId: nf.notaFiscalStatusId || undefined,
        competencia: vigencia, // Usar vigência como competência
        dataEmissao: dataEmissao,
        urlNotaFiscalDownload: nf.urlNotaFiscalDownload || null,
        lancamentos: (nf.rubricas || []).map(r => ({
          rubrica: r.rubricaDescricao || '',
          valor: r.valor ?? 0,
        })),
      };
    });
  }, [notasFiscaisApi]);

  // Paginar dados no frontend
  const notasFiscais = useMemo(() => {
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return notasFiscaisCompletas.slice(startIndex, endIndex);
  }, [notasFiscaisCompletas, currentPage, itemsPerPage]);

  // Resetar para primeira página quando os dados mudarem
  useEffect(() => {
    setCurrentPage(1);
  }, [notasFiscaisCompletas.length]);

  // Quantidade de NFs selecionadas com divergência (valor analisado ≠ valor)
  const qtdDivergentes = useMemo(() =>
    notasFiscaisCompletas.filter(n => {
      if (!selectedNotas.includes(n.id)) return false;
      const va = n.valorAnalisado, v = n.valor;
      return va != null && v != null && Math.abs((va ?? 0) - (v ?? 0)) > 0.01;
    }).length
  , [notasFiscaisCompletas, selectedNotas]);

  const handleSelectNota = (id: number, statusId?: number) => {
    // Só permite selecionar notas com statusId = 3
    if (statusId !== 3) {
      return;
    }
    
    if (selectedNotas.includes(id)) {
      setSelectedNotas(selectedNotas.filter(nfId => nfId !== id));
    } else {
      setSelectedNotas([...selectedNotas, id]);
    }
  };

  const handleSelectAll = () => {
    // Selecionar todas as notas com statusId = 3
    const notasComStatus3 = notasFiscaisCompletas
      .filter(nf => nf.statusId === 3)
      .map(nf => nf.id);
    
    // Se todas já estão selecionadas, desmarcar todas
    const todasSelecionadas = notasComStatus3.length > 0 && notasComStatus3.every(id => selectedNotas.includes(id));
    
    if (todasSelecionadas) {
      // Remover apenas as que têm statusId = 3
      setSelectedNotas(selectedNotas.filter(id => {
        const nota = notasFiscaisCompletas.find(nf => nf.id === id);
        return nota?.statusId !== 3;
      }));
    } else {
      // Adicionar todas as que têm statusId = 3 e ainda não estão selecionadas
      const novasSelecionadas = notasComStatus3.filter(id => !selectedNotas.includes(id));
      setSelectedNotas([...selectedNotas, ...novasSelecionadas]);
    }
  };

  const handleAprovar = async () => {
    const ids = notasFiscaisCompletas
      .filter((n) => selectedNotas.includes(n.id) && n.idNotaFiscal)
      .map((n) => n.idNotaFiscal);
    if (ids.length === 0 || !token) return;
    setModalAprovarOpen(false);
    setErroAprovar(null);
    setErroReprovar(null);
    setProcessandoAprovar(true);
    try {
      const useCase = container.resolve(GetNotasFiscaisUseCase);
      const res = await useCase.executeAprovarNotasFiscais(token, { ids, motivoReprovacao: '' });
      if (res.sucesso) {
        setSelectedNotas([]);
        refetchNotas();
      } else {
        setErroAprovar(res.mensagem || res.erros?.join(', ') || 'Erro ao aprovar');
      }
    } catch (e) {
      setErroAprovar(e instanceof Error ? e.message : 'Erro ao aprovar');
    } finally {
      setProcessandoAprovar(false);
    }
  };

  const handleReprovar = async (justificativa: string) => {
    const motivo = justificativa.trim();
    if (!motivo) return;
    const ids = notasFiscaisCompletas
      .filter((n) => selectedNotas.includes(n.id) && n.idNotaFiscal)
      .map((n) => n.idNotaFiscal);
    if (ids.length === 0 || !token) return;
    setErroReprovar(null);
    setErroAprovar(null);
    setProcessandoReprovar(true);
    try {
      const useCase = container.resolve(GetNotasFiscaisUseCase);
      const res = await useCase.executeReprovarNotasFiscais(token, { ids, motivoReprovacao: motivo });
      if (res.sucesso) {
        setSelectedNotas([]);
        setModalReprovarOpen(false);
        refetchNotas();
      } else {
        setErroReprovar(res.mensagem || res.erros?.join(", ") || "Erro ao reprovar");
      }
    } catch (e) {
      setErroReprovar(e instanceof Error ? e.message : "Erro ao reprovar");
    } finally {
      setProcessandoReprovar(false);
    }
  };

  const handleLiberarEmissao = async (params: {
    unidadeId: string;
    mes: number;
    ano: number;
    enviarEmail: boolean;
  }) => {
    if (!params.unidadeId || !params.mes || !params.ano || !token) return;
    
    setErroLiberacao(null);
    setProcessandoLiberacao(true);
    
    try {
      const useCase = container.resolve(GetNotasFiscaisUseCase);
      const res = await useCase.executeLiberarEmissaoNotasFiscaisPorVigencia(token, {
        mes: params.mes,
        ano: params.ano,
        enviarEmail: params.enviarEmail,
        codigoDiretoria: params.unidadeId, // O id da unidade é o codigoDiretoria
      });
      
      if (res.sucesso) {
        setModalLiberacaoOpen(false);
        // Recarregar notas fiscais
        refetchNotas();
      } else {
        setErroLiberacao(res.mensagem || res.erros?.join(", ") || "Erro ao liberar emissão");
      }
    } catch (e) {
      setErroLiberacao(e instanceof Error ? e.message : "Erro ao liberar emissão");
    } finally {
      setProcessandoLiberacao(false);
    }
  };

  const handleVerDetalhes = (nf: NotaFiscalGestao) => {
    setNotaSelecionada(nf);
    setDetalhesOpen(true);
  };

  const getStatusColor = (status: string) => {
    const statusLower = status.toLowerCase();
    if (statusLower.includes('aprovad')) {
      return "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200";
    } else if (statusLower.includes('rejeitad') || statusLower.includes('reprovad')) {
      return "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200";
    } else if (statusLower.includes('análise') || statusLower.includes('pendent')) {
      return "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200";
    } else {
      return "bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200";
    }
  };

  const columns: Column[] = [
    { id: "select", label: "", sortable: false, width: "w-[50px]" },
    { id: "prestador", label: "Prestador", sortable: true },
    { id: "vigencia", label: "Vigência", sortable: true },
    { id: "numeroNF", label: "N° NF", sortable: true },
    { id: "valor", label: "Valor", sortable: true },
    { id: "valorAnalisado", label: "Valor Analisado", sortable: true },
    { id: "dataEnvio", label: "Data Envio", sortable: true },
    { id: "status", label: "Status", sortable: true },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[250px]" },
  ];

  const renderCell = (nf: NotaFiscalGestao, columnId: string) => {
    switch (columnId) {
      case "select":
        const isStatus3 = nf.statusId === 3;
        // Ocultar checkbox se não for statusId = 3
        if (!isStatus3) {
          return null;
        }
        return (
          <Checkbox
            data-testid={`checkbox-nf-${nf.id}`}
            checked={selectedNotas.includes(nf.id)}
            onCheckedChange={() => handleSelectNota(nf.id, nf.statusId)}
          />
        );
      case "prestador":
        return <span className="font-medium">{nf.prestador}</span>;
      case "vigencia":
        return nf.vigencia;
      case "numeroNF":
        return nf.numeroNF;
      case "valor":
        return (nf.valor ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
      case "valorAnalisado":
        const valorAnalisado = nf.valorAnalisado;
        const valorOriginal = nf.valor;
        const temDivergencia = valorAnalisado !== null && valorAnalisado !== undefined && 
                                valorOriginal !== null && valorOriginal !== undefined &&
                                Math.abs(valorAnalisado - valorOriginal) > 0.01;
        
        if (valorAnalisado === null || valorAnalisado === undefined) {
          return <span className="text-muted-foreground">-</span>;
        }
        
        return (
          <div className="flex items-center gap-2">
            <span>{valorAnalisado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</span>
            {temDivergencia && (
              <Tooltip>
                <TooltipTrigger asChild>
                  <AlertTriangle className="h-4 w-4 text-amber-500 flex-shrink-0" />
                </TooltipTrigger>
                <TooltipContent>
                  <p>A análise encontrou divergência no valor analisado para o valor inserido</p>
                </TooltipContent>
              </Tooltip>
            )}
          </div>
        );
      case "dataEnvio":
        return nf.dataEnvio;
      case "status":
        return (
          <Badge className={cn("font-medium", getStatusColor(nf.status))}>
            {nf.status}
          </Badge>
        );
      case "acoes":
        return (
          <div className="flex justify-end gap-2">
            {nf.urlNotaFiscalDownload && (
              <Button 
                data-testid={`btn-visualizar-nf-${nf.id}`}
                variant="outline" 
                size="sm" 
                className="gap-2"
                onClick={() => {
                  window.open(nf.urlNotaFiscalDownload!, '_blank');
                }}
              >
                <Eye className="h-4 w-4" />
                Visualizar NF
              </Button>
            )}
            <Button data-testid={`btn-detalhes-nf-${nf.id}`} variant="ghost" size="sm" className="gap-1" onClick={() => handleVerDetalhes(nf)}>
              <ChevronRight className="h-4 w-4" />
              Detalhes
            </Button>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <TooltipProvider>
      <div className="container mx-auto p-6 space-y-6" data-testid="page-gestao-notas-fiscais">
      <h1 className="page-title">Gestão de Notas Fiscais</h1>

      {/* Filtros */}
      <Card data-testid="filtros-card">
        <CardContent className="pt-6">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-4">
            <h2 className="text-lg font-semibold">Filtros</h2>
            <Button 
              data-testid="btn-liberar-emissao"
              onClick={() => setModalLiberacaoOpen(true)}
              className="w-full sm:w-auto text-sm sm:text-base"
            >
              Liberação para Emissão de Notas Fiscais
            </Button>
          </div>
          <div className="flex flex-col lg:flex-row gap-4 lg:items-end">
            {/* Busca */}
            <div className="space-y-2 flex-1 w-full">
              <Label>Busca</Label>
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  data-testid="filter-busca"
                  placeholder="Busca por prestador, vigência, ..."
                  value={busca}
                  onChange={(e) => setBusca(e.target.value)}
                  className="pl-10 w-full"
                />
              </div>
            </div>

            {/* Unidades */}
            <div className="space-y-2 flex-1 w-full">
              <Label>Unidades</Label>
              <Select value={unidade} onValueChange={setUnidade} disabled={loadingFiltros}>
                <SelectTrigger className="w-full" data-testid="filter-unidade-trigger">
                  <SelectValue placeholder={loadingFiltros ? "Carregando..." : "Selecione"} />
                </SelectTrigger>
                <SelectContent>
                  {unidades.length > 0 ? (
                    unidades.map((u) => (
                      <SelectItem key={u.id} value={u.id} data-testid={`filter-unidade-option-${u.id}`}>
                        {u.descricao}
                      </SelectItem>
                    ))
                  ) : (
                    !loadingFiltros && (
                      <div className="px-2 py-1.5 text-sm text-muted-foreground">
                        Nenhuma unidade encontrada
                      </div>
                    )
                  )}
                </SelectContent>
              </Select>
            </div>

            {/* Colaborador */}
            <div className="space-y-2 flex-1 w-full">
              <Label>Colaborador</Label>
              <Popover open={colaboradorOpen} onOpenChange={setColaboradorOpen}>
                <PopoverTrigger asChild>
                  <Button
                    data-testid="filter-colaborador-trigger"
                    variant="outline"
                    role="combobox"
                    aria-expanded={colaboradorOpen}
                    className="w-full justify-between"
                    disabled={loadingFiltros}
                  >
                    <span className="truncate flex-1 text-left">
                      {loadingFiltros
                        ? "Carregando..."
                        : colaborador
                        ? colaboradores.find((c) => c.cpf === colaborador)?.nomeProfissional || "Colaborador não encontrado"
                        : "Selecione o colaborador"}
                    </span>
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                  <Command>
                    <CommandInput data-testid="filter-colaborador-search" placeholder="Buscar colaborador..." />
                    <CommandList>
                      {loadingFiltros ? (
                        <div className="px-2 py-6 text-center text-sm text-muted-foreground">
                          Carregando colaboradores...
                        </div>
                      ) : colaboradores.length > 0 ? (
                        <CommandGroup>
                          {colaboradores.map((c) => (
                            <CommandItem
                              key={c.cpf}
                              data-testid={`filter-colaborador-option-${c.cpf}`}
                              value={`${c.nomeProfissional} ${c.labelCodigoNome}`}
                              onSelect={() => {
                                setColaborador(c.cpf);
                                setColaboradorOpen(false);
                              }}
                            >
                              <Check
                                className={cn(
                                  "mr-2 h-4 w-4",
                                  colaborador === c.cpf ? "opacity-100" : "opacity-0"
                                )}
                              />
                              {c.nomeProfissional}
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      ) : (
                        <CommandEmpty>Nenhum colaborador encontrado.</CommandEmpty>
                      )}
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            {/* Status */}
            <div className="space-y-2 flex-1 w-full">
              <Label>Status</Label>
              <Select value={status} onValueChange={setStatus} disabled={loadingFiltros}>
                <SelectTrigger className="w-full" data-testid="filter-status-trigger">
                  <SelectValue placeholder={loadingFiltros ? "Carregando..." : "Selecione"}>
                    {status ? statusList.find(s => s.id.toString() === status)?.nome : null}
                  </SelectValue>
                </SelectTrigger>
                <SelectContent>
                  {statusList.length > 0 ? (
                    statusList.map((s) => (
                      <SelectItem key={s.id} value={s.id.toString()} data-testid={`filter-status-option-${s.id}`}>
                        {s.nome}
                      </SelectItem>
                    ))
                  ) : (
                    !loadingFiltros && (
                      <div className="px-2 py-1.5 text-sm text-muted-foreground">
                        Nenhum status encontrado
                      </div>
                    )
                  )}
                </SelectContent>
              </Select>
            </div>

            {/* Competência */}
            <div className="space-y-2 flex-1 w-full">
              <Label>Competência</Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    data-testid="filter-competencia-trigger"
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
            <div className="flex flex-col sm:flex-row gap-3 w-full lg:w-auto">
              <Button 
                data-testid="btn-limpar-filtros"
                variant="ghost" 
                onClick={handleLimpar} 
                className="text-destructive hover:text-destructive w-full sm:w-auto"
              >
                Limpar
              </Button>
              <Button data-testid="btn-aplicar-filtros" onClick={handleAplicar} className="w-full sm:w-auto">
                Aplicar
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Tabela */}
      <div className="space-y-4">
        <div>
          <h2 className="text-xl font-semibold">Aprove ou reprove notas fiscais dos prestadores</h2>
        </div>

        <Card>
          <CardContent className="p-6">
            {loadingNotas ? (
              <div className="flex items-center justify-center py-8">
                <p className="text-muted-foreground">Carregando notas fiscais...</p>
              </div>
            ) : errorNotas ? (
              <div className="flex items-center justify-center py-8">
                <p className="text-destructive">{errorNotas}</p>
              </div>
            ) : (
              <>
                {/* Checkbox Selecionar Todas e Botões Aprovar/Reprovar */}
                {notasFiscaisCompletas.length > 0 && (
                  <div className="flex items-center justify-between mb-4">
                    <div className="flex items-center gap-2">
                      <Checkbox
                        data-testid="checkbox-selecionar-todas"
                        checked={
                          notasFiscaisCompletas.filter(nf => nf.statusId === 3).length > 0 &&
                          notasFiscaisCompletas
                            .filter(nf => nf.statusId === 3)
                            .every(nf => selectedNotas.includes(nf.id))
                        }
                        onCheckedChange={handleSelectAll}
                      />
                      <Label className="text-sm font-medium cursor-pointer">
                        Selecionar todas com status "NF em Análise"
                      </Label>
                    </div>
                    {selectedNotas.length > 0 && (
                      <div className="flex flex-col items-end gap-2">
                        {(erroAprovar || erroReprovar) && (
                          <p className="text-sm text-destructive">{erroAprovar || erroReprovar}</p>
                        )}
                        <div className="flex items-center gap-3">
                          <Button
                            data-testid="btn-aprovar"
                            onClick={() => setModalAprovarOpen(true)}
                            disabled={processandoAprovar || processandoReprovar}
                            className="bg-green-600 hover:bg-green-700 text-white"
                          >
                            <CheckCircle className="h-4 w-4 mr-2" />
                            {processandoAprovar ? 'Aprovando...' : `Aprovar (${selectedNotas.length})`}
                          </Button>
                          <Button
                            data-testid="btn-reprovar"
                            onClick={() => { setErroReprovar(null); setModalReprovarOpen(true); }}
                            variant="destructive"
                            disabled={processandoAprovar || processandoReprovar}
                            className="text-white"
                          >
                            <XCircle className="h-4 w-4 mr-2" />
                            {processandoReprovar ? 'Reprovando...' : `Reprovar (${selectedNotas.length})`}
                          </Button>
                        </div>
                      </div>
                    )}
                  </div>
                )}
                <div data-testid="table-notas-fiscais">
                <DataTable
                  columns={columns}
                  data={notasFiscais}
                  keyExtractor={(item) => item.id.toString()}
                  renderCell={renderCell}
                  emptyMessage="Nenhuma nota fiscal encontrada"
                />
                </div>
                {notasFiscaisCompletas.length > 0 && (
                  <div data-testid="notas-fiscais-pagination">
                  <TablePagination
                    currentPage={currentPage}
                    totalItems={notasFiscaisCompletas.length}
                    itemsPerPage={itemsPerPage}
                    onPageChange={setCurrentPage}
                    onItemsPerPageChange={(items) => {
                      setItemsPerPage(Number(items));
                      setCurrentPage(1);
                    }}
                  />
                  </div>
                )}
              </>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Modal de confirmação antes de aprovar */}
      <AprovarNotasFiscaisModal
        open={modalAprovarOpen}
        onOpenChange={setModalAprovarOpen}
        quantidadeSelecionadas={selectedNotas.length}
        quantidadeDivergentes={qtdDivergentes}
        onConfirm={handleAprovar}
      />

      {/* Modal de reprovação */}
      <ReprovarNotasFiscaisModal
        open={modalReprovarOpen}
        onOpenChange={setModalReprovarOpen}
        quantidadeSelecionadas={selectedNotas.length}
        onConfirm={handleReprovar}
        processando={processandoReprovar}
        erro={erroReprovar}
      />

      {/* Modal de Liberação para Emissão de Notas Fiscais */}
      <LiberarEmissaoNotasFiscaisModal
        open={modalLiberacaoOpen}
        onOpenChange={setModalLiberacaoOpen}
        unidades={unidades}
        loadingUnidades={loadingFiltros}
        onConfirm={handleLiberarEmissao}
        processando={processandoLiberacao}
        erro={erroLiberacao}
      />

      {/* Modal Detalhes da NF */}
      <DetalhesNotaFiscalModal
        open={detalhesOpen}
        onOpenChange={setDetalhesOpen}
        notaFiscal={notaSelecionada ? {
          numeroNF: notaSelecionada.numeroNF,
          competencia: notaSelecionada.competencia,
          dataEmissao: notaSelecionada.dataEmissao,
          valor: notaSelecionada.valor,
          status: notaSelecionada.status,
          lancamentos: notaSelecionada.lancamentos,
        } : null}
        getStatusColor={getStatusColor}
      />
    </div>
    </TooltipProvider>
  );
}
