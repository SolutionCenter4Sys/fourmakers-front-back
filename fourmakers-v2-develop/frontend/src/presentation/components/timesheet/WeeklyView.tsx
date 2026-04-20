import { useState, useEffect } from "react";
import { Triangle, Circle, Edit, Trash2, Clock, CheckCircle2, XCircle, Timer } from "@/components/ui/system-icons";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { Check, ChevronsUpDown } from "@/components/ui/system-icons";
import { cn } from "@/lib/utils";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { useAppSelector } from "@app/store/hooks";
import { useToast } from "@/hooks/use-toast";
import { 
  TimesheetComponentesApi, 
  type DiaTemplate, 
  type SemanaTemplate,
  type ColaboradorApontamento,
  type ApontamentoMensal,
  type ProjetoComAtividades,
  type Atividade
} from "@data/api/TimesheetComponentesApi";

interface WeeklyViewProps {
  period: string; // formato: "mes-ano" (ex: "11-2025")
  onRefetch?: () => void;
  cpfColaborador?: string;
  /** Exibe a coluna Aprovadores no grid do dia. Parâmetro MOSTRAR_COLUNA_APROVADORES_TIMESHEET. */
  mostrarColunaAprovadores?: boolean;
}

export const WeeklyView = ({ period, onRefetch, cpfColaborador, mostrarColunaAprovadores = false }: WeeklyViewProps) => {
  const { token, user } = useAppSelector((state) => state.auth);
  
  // Verificar se o usuário tem a funcionalidade APONTAMENTO_HORAS_COLABORADOR
  const temApontamentoHorasColaborador = user?.funcionalidadeSistema?.some(
    (func) => func.descricao === "APONTAMENTO_HORAS_COLABORADOR" && func.ativo === true
  ) ?? false;
  const [selectedDay, setSelectedDay] = useState<string | null>(null);
  const [semanas, setSemanas] = useState<SemanaTemplate[]>([]);
  const [apontamentos, setApontamentos] = useState<ColaboradorApontamento[]>([]);
  const [apontamentosMensais, setApontamentosMensais] = useState<ApontamentoMensal[]>([]);
  const [dataColetaDeDados, setDataColetaDeDados] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const [showDistribuicaoModal, setShowDistribuicaoModal] = useState(false);
  const [semanaSelecionada, setSemanaSelecionada] = useState<SemanaTemplate | null>(null);
  const [showEditModal, setShowEditModal] = useState(false);
  const [apontamentoEditando, setApontamentoEditando] = useState<ColaboradorApontamento | null>(null);
  const [projetos, setProjetos] = useState<ProjetoComAtividades[]>([]);
  const [atividades, setAtividades] = useState<Atividade[]>([]);
  const [selectedProjetoEdit, setSelectedProjetoEdit] = useState<string>("");
  const [selectedAtividadeEdit, setSelectedAtividadeEdit] = useState<string>("");
  const [dataInicioEdit, setDataInicioEdit] = useState<string>("");
  const [horasEdit, setHorasEdit] = useState<string>("");
  const [minutosEdit, setMinutosEdit] = useState<string>("00");
  const [observacaoEdit, setObservacaoEdit] = useState<string>("");
  const [salvando, setSalvando] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [apontamentoDeletando, setApontamentoDeletando] = useState<ColaboradorApontamento | null>(null);
  const [deletando, setDeletando] = useState(false);
  const [projetoEditOpen, setProjetoEditOpen] = useState(false);
  const [atividadeEditOpen, setAtividadeEditOpen] = useState(false);
  const [modalAprovadoresOpen, setModalAprovadoresOpen] = useState(false);
  const [aprovadoresModalList, setAprovadoresModalList] = useState<Array<{ nome: string }>>([]);
  const { toast } = useToast();

  const loadData = async () => {
    if (!token || !period) {
      // Limpar estados quando não houver token ou período
      setApontamentos([]);
      setApontamentosMensais([]);
      setSemanas([]);
      setDataColetaDeDados("");
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      const [mes, ano] = period.split('-').map(Number);
      
      if (!mes || !ano) {
        // Limpar estados quando não houver mês ou ano válido
        setApontamentos([]);
        setApontamentosMensais([]);
        setSemanas([]);
        setDataColetaDeDados("");
        setLoading(false);
        return;
      }

      // IMPORTANTE: Limpar estados antes de carregar novos dados
      // Isso evita mostrar dados de colaboradores anteriores
      setApontamentos([]);
      setApontamentosMensais([]);
      setDataColetaDeDados("");

      const api = new TimesheetComponentesApi();
      
      // IMPORTANTE: Se houver cpfColaborador, sempre usar ele (mesmo que seja string vazia)
      // Se não houver (undefined), usar string vazia
      const cpfParaEnvio = cpfColaborador !== undefined ? cpfColaborador : '';
      
      // Carregar template da semana e apontamentos em paralelo
      const [templateResponse, apontamentosResponse] = await Promise.all([
        api.listarTemplateSemanaVigencia(token, mes, ano),
        api.listarApontamentosPorVigencia(token, mes, ano, cpfParaEnvio)
      ]);
      
      // Sempre definir semanas (mesmo que vazio) para garantir limpeza
      if (templateResponse.semanas && templateResponse.semanas.length > 0) {
        setSemanas(templateResponse.semanas);
      } else {
        setSemanas([]);
      }
      
      if (apontamentosResponse.apontamentos_por_vigencia) {
        // Sempre definir apontamentos (mesmo que vazio) para garantir limpeza
        if (apontamentosResponse.apontamentos_por_vigencia.colaboradorApontamentos) {
          setApontamentos(apontamentosResponse.apontamentos_por_vigencia.colaboradorApontamentos);
        } else {
          setApontamentos([]);
        }
        
        // Sempre definir apontamentos mensais (mesmo que vazio) para garantir limpeza
        if (apontamentosResponse.apontamentos_por_vigencia.apontamentosMensais) {
          setApontamentosMensais(apontamentosResponse.apontamentos_por_vigencia.apontamentosMensais);
        } else {
          setApontamentosMensais([]);
        }
        
        if (apontamentosResponse.apontamentos_por_vigencia.dataColetaDeDados) {
          setDataColetaDeDados(apontamentosResponse.apontamentos_por_vigencia.dataColetaDeDados);
        } else {
          setDataColetaDeDados("");
        }
      } else {
        // Se não houver apontamentos_por_vigencia, limpar tudo
        setApontamentos([]);
        setApontamentosMensais([]);
        setDataColetaDeDados("");
      }
    } catch (err) {
      console.error("Erro ao carregar dados:", err);
      // Em caso de erro, limpar estados
      setApontamentos([]);
      setApontamentosMensais([]);
      setDataColetaDeDados("");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // Limpar dia selecionado quando colaborador ou período mudar
    setSelectedDay(null);
    loadData();
  }, [token, period, cpfColaborador]);

  // Expor função de refetch via callback
  useEffect(() => {
    if (onRefetch) {
      // Criar uma função que pode ser chamada externamente
      (window as any).refetchWeeklyView = loadData;
    }
    return () => {
      if ((window as any).refetchWeeklyView === loadData) {
        delete (window as any).refetchWeeklyView;
      }
    };
  }, [token, period]);

  const formatarData = (dataString: string): string => {
    const data = new Date(dataString);
    return data.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' });
  };

  const extrairDia = (dataString: string): string => {
    const data = new Date(dataString);
    return data.getDate().toString().padStart(2, '0');
  };

  const getStatusIcons = (dia: DiaTemplate) => {
    if (dia.feriado) {
      return [<Circle key="feriado" className="h-4 w-4 text-orange-500" />];
    }
    if (!dia.mesAtivo) {
      return [<Triangle key="inativo" className="h-4 w-4 text-muted-foreground" />];
    }
    
    const apontamentosDia = getApontamentosDoDia(dia.data);
    if (apontamentosDia.length === 0) {
      return [<Triangle key="nao-apontado" className="h-4 w-4 text-muted-foreground" />];
    }
    
    const statusDoDia = getStatusDoDia(dia);
    const icons: React.ReactElement[] = [];
    
    // Ordem de prioridade: Reprovado > Aprovado > Pendente
    if (statusDoDia.includes("3")) {
      icons.push(<XCircle key="reprovado" className="h-4 w-4 text-destructive" />);
    }
    if (statusDoDia.includes("2")) {
      icons.push(<CheckCircle2 key="aprovado" className="h-4 w-4 text-green-600" />);
    }
    if (statusDoDia.includes("1")) {
      icons.push(<Clock key="pendente" className="h-4 w-4 text-blue-600" />);
    }
    
    return icons.length > 0 ? icons : [<Triangle key="sem-status" className="h-4 w-4 text-muted-foreground" />];
  };

  const getStatusColor = (dia: DiaTemplate, isSelected: boolean) => {
    const statusDia = getStatusDoDia(dia);
    const temPendente = statusDia.includes("1");
    const temReprovado = statusDia.includes("3");
    if (isSelected) {
      return "border-blue-600 border-2";
    }
    if (!dia.mesAtivo) {
      return "border-muted/30 bg-muted/10 opacity-50";
    }
    if (dia.feriado) {
      return "border-orange-200 bg-orange-50";
    }
    if (temPendente && !temReprovado) {
      return "border-blue-500 bg-blue-50/50";
    }
    return "border-muted bg-muted/30";
  };

  // Converter minutos para formato HH:MM
  const formatarHoras = (minutos: number): string => {
    const horas = Math.floor(minutos / 60);
    const mins = minutos % 60;
    return `${String(horas).padStart(2, '0')}:${String(mins).padStart(2, '0')}`;
  };

  // Obter apontamentos de um dia específico
  const getApontamentosDoDia = (dataString: string): ColaboradorApontamento[] => {
    const data = new Date(dataString);
    const dataFormatada = data.toISOString().split('T')[0];
    
    return apontamentos.filter(ap => {
      const apData = new Date(ap.data_registro);
      const apDataFormatada = apData.toISOString().split('T')[0];
      return apDataFormatada === dataFormatada;
    });
  };

  // Calcular total de horas de um dia
  const getHoras = (dia: DiaTemplate): string => {
    const apontamentosDia = getApontamentosDoDia(dia.data);
    if (apontamentosDia.length === 0) return "";
    
    const totalMinutos = apontamentosDia.reduce((sum, ap) => sum + ap.horas, 0);
    return formatarHoras(totalMinutos);
  };

  // Obter todos os status presentes em um dia
  const getStatusDoDia = (dia: DiaTemplate): string[] => {
    const apontamentosDia = getApontamentosDoDia(dia.data);
    const statusUnicos = new Set<string>();
    apontamentosDia.forEach(ap => {
      statusUnicos.add(ap.codStatusApontamentoGrupo);
    });
    return Array.from(statusUnicos);
  };

  // Obter todos os status presentes em uma semana
  const getStatusDaSemana = (semana: SemanaTemplate): string[] => {
    const statusUnicos = new Set<string>();
    semana.dias.forEach(dia => {
      const statusDia = getStatusDoDia(dia);
      statusDia.forEach(status => statusUnicos.add(status));
    });
    return Array.from(statusUnicos);
  };

  // Carregar projetos e atividades para edição
  useEffect(() => {
    const loadProjetosParaEdicao = async () => {
      if (!token || !showEditModal) return;

      try {
        const api = new TimesheetComponentesApi();
        const response = await api.listarProjetosComAtividades(token, '');
        
        if (response.projetos_atividades && response.projetos_atividades.length > 0) {
          setProjetos(response.projetos_atividades);
        }
      } catch (err) {
        console.error("Erro ao carregar projetos para edição:", err);
      }
    };

    loadProjetosParaEdicao();
  }, [token, showEditModal]);

  // Atualizar atividades quando projeto mudar
  useEffect(() => {
    if (selectedProjetoEdit) {
      const projetoSelecionado = projetos.find((p) => p.id === selectedProjetoEdit);
      if (projetoSelecionado) {
        setAtividades(projetoSelecionado.atividades || []);
      } else {
        setAtividades([]);
      }
    } else {
      setAtividades([]);
    }
  }, [selectedProjetoEdit, projetos]);

  // Abrir modal de edição
  const handleAbrirEdicao = (apontamento: ColaboradorApontamento) => {
    setApontamentoEditando(apontamento);
    setSelectedProjetoEdit(apontamento.projeto.id);
    setSelectedAtividadeEdit(apontamento.atividade.id);
    
    // Formatar data para o input
    const data = new Date(apontamento.data_registro);
    const ano = data.getFullYear();
    const mes = String(data.getMonth() + 1).padStart(2, '0');
    const dia = String(data.getDate()).padStart(2, '0');
    setDataInicioEdit(`${ano}-${mes}-${dia}`);
    
    // Converter minutos para HH:MM
    const horas = Math.floor(apontamento.horas / 60);
    const minutos = apontamento.horas % 60;
    setHorasEdit(String(horas).padStart(2, '0'));
    setMinutosEdit(String(minutos).padStart(2, '0'));
    
    setObservacaoEdit(apontamento.observacao || "");
    setShowEditModal(true);
  };

  // Converter horas HH:MM para minutos
  const converterHorasParaMinutos = (horas: string, minutos: string): number => {
    const h = parseInt(horas, 10) || 0;
    const m = parseInt(minutos, 10) || 0;
    return h * 60 + m;
  };

  // Salvar edição
  const handleSalvarEdicao = async () => {
    if (!apontamentoEditando || !token) return;

    setSalvando(true);
    try {
      const api = new TimesheetComponentesApi();
      const horasEmMinutos = converterHorasParaMinutos(horasEdit, minutosEdit);
      
      // Formatar data para o formato esperado pela API (YYYY-MM-DDTHH:mm:ss)
      const dataObj = new Date(dataInicioEdit + 'T00:00:00');
      const dataRegistro = dataObj.toISOString();
      
      // Usar dataColetaDeDados do endpoint ListarApontamentosPorVigencia, ou atual se não houver
      const dataColetaDeDadosParaEnvio = dataColetaDeDados || new Date().toISOString();

      const response = await api.editarApontamento(
        token,
        {
          projetoId: selectedProjetoEdit,
          atividadeId: selectedAtividadeEdit,
          horas: horasEmMinutos,
          dataRegistro: dataRegistro,
          apontamentoId: apontamentoEditando.id,
          cpfColaborador: cpfColaborador || "",
          observacao: observacaoEdit,
          dataColetaDeDados: dataColetaDeDadosParaEnvio,
        },
        'pt-BR'
      );

      console.log("Resposta da API editarApontamento:", response);

      // Verificar se a resposta indica sucesso
      if (response && response.sucesso === false) {
        toast({
          title: "Erro",
          description: response.mensagem || "Erro ao editar apontamento.",
          variant: "destructive",
        });
        return;
      }

      // Se sucesso é true ou não existe (assumir sucesso se não houver campo sucesso)
      if (response && response.sucesso === true) {
        toast({
          title: "Sucesso",
          description: response.mensagem || "Apontamento editado com sucesso!",
        });

        setShowEditModal(false);
        setApontamentoEditando(null);

        // Refetch dos dados
        if (onRefetch) {
          onRefetch();
        }
        if ((window as any).refetchWeeklyView) {
          (window as any).refetchWeeklyView();
        }
        return;
      }

      // Fallback: se não houver campo sucesso, assumir sucesso
      toast({
        title: "Sucesso",
        description: "Apontamento editado com sucesso!",
      });

      setShowEditModal(false);
      setApontamentoEditando(null);

      // Refetch dos dados
      if (onRefetch) {
        onRefetch();
      }
      if ((window as any).refetchWeeklyView) {
        (window as any).refetchWeeklyView();
      }

    } catch (err) {
      console.error("Erro ao editar apontamento:", err);
      toast({
        title: "Erro",
        description: "Erro inesperado ao editar apontamento.",
        variant: "destructive",
      });
    } finally {
      setSalvando(false);
    }
  };

  // Abrir modal de exclusão
  const handleAbrirExclusao = (apontamento: ColaboradorApontamento) => {
    setApontamentoDeletando(apontamento);
    setShowDeleteModal(true);
  };

  // Confirmar exclusão
  const handleConfirmarExclusao = async () => {
    if (!apontamentoDeletando || !token) return;

    setDeletando(true);
    try {
      const api = new TimesheetComponentesApi();
      
      // Usar dataColetaDeDados do endpoint ListarApontamentosPorVigencia, ou atual se não houver
      const dataColetaDeDadosParaEnvio = dataColetaDeDados || new Date().toISOString();

      const response = await api.deletarApontamentoColaborador(
        token,
        apontamentoDeletando.id,
        dataColetaDeDadosParaEnvio,
        'pt-BR'
      );

      console.log("Resposta da API deletarApontamento:", response);

      // Verificar se a resposta indica sucesso
      if (response && response.sucesso === false) {
        toast({
          title: "Erro",
          description: response.mensagem || "Erro ao excluir apontamento.",
          variant: "destructive",
        });
        return;
      }

      // Se sucesso é true ou não existe (assumir sucesso se não houver campo sucesso)
      toast({
        title: "Sucesso",
        description: response.mensagem || "Apontamento excluído com sucesso!",
      });

      setShowDeleteModal(false);
      setApontamentoDeletando(null);

      // Refetch dos dados
      if (onRefetch) {
        onRefetch();
      }
      if ((window as any).refetchWeeklyView) {
        (window as any).refetchWeeklyView();
      }
    } catch (err) {
      console.error("Erro ao excluir apontamento:", err);
      toast({
        title: "Erro",
        description: "Erro inesperado ao excluir apontamento.",
        variant: "destructive",
      });
    } finally {
      setDeletando(false);
    }
  };

  // Formatar label do projeto
  const formatarLabelProjeto = (projeto: ProjetoComAtividades): string => {
    return `${projeto.id} - ${projeto.nome} / ${projeto.codigoCliente} - ${projeto.nomeCliente}`;
  };

  // Calcular horas por status de uma semana
  const getHorasPorStatusSemana = (semana: SemanaTemplate) => {
    let totalLancadas = 0;
    let totalAprovadas = 0;
    let totalPendentes = 0;
    let totalReprovadas = 0;

    semana.dias.forEach(dia => {
      const apontamentosDia = getApontamentosDoDia(dia.data);
      apontamentosDia.forEach(ap => {
        totalLancadas += ap.horas;
        switch (ap.codStatusApontamentoGrupo) {
          case "2": // Aprovado
            totalAprovadas += ap.horas;
            break;
          case "1": // Pendente
            totalPendentes += ap.horas;
            break;
          case "3": // Reprovado
            totalReprovadas += ap.horas;
            break;
        }
      });
    });

    return {
      lancadas: totalLancadas,
      aprovadas: totalAprovadas,
      pendentes: totalPendentes,
      reprovadas: totalReprovadas,
    };
  };

  // Formatar título da semana (ex: "Dezembro de 1 a 6")
  const getTituloSemana = (semana: SemanaTemplate): string => {
    if (!period) return "";
    const [mes, ano] = period.split('-').map(Number);
    if (!mes || !ano) return "";
    
    const meses = [
      "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
      "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    ];
    
    // Encontrar primeiro e último dia do mês ativo na semana
    const diasAtivos = semana.dias.filter(d => d.mesAtivo);
    if (diasAtivos.length === 0) return "";
    
    const primeiroDia = Math.min(...diasAtivos.map(d => new Date(d.data).getDate()));
    const ultimoDia = Math.max(...diasAtivos.map(d => new Date(d.data).getDate()));
    
    return `${meses[mes - 1]} de ${primeiroDia} a ${ultimoDia}`;
  };

  // Calcular total de horas da semana
  const getTotalHorasSemana = (semana: SemanaTemplate): string => {
    let totalMinutos = 0;
    
    semana.dias.forEach(dia => {
      const apontamentosDia = getApontamentosDoDia(dia.data);
      totalMinutos += apontamentosDia.reduce((sum, ap) => sum + ap.horas, 0);
    });
    
    return formatarHoras(totalMinutos);
  };

  // Verificar se a semana tem horas apontadas
  const semanaTemHoras = (semana: SemanaTemplate): boolean => {
    return semana.dias.some(dia => {
      const apontamentosDia = getApontamentosDoDia(dia.data);
      return apontamentosDia.length > 0;
    });
  };

  // Obter status badge baseado no código
  const getStatusBadgePorCodigo = (codStatus: string) => {
    switch (codStatus) {
      case "2": // Aprovado
        return <Badge className="bg-green-600 text-white">Aprovado</Badge>;
      case "1": // Pendente
        return <Badge className="bg-blue-600 text-white">Pendente</Badge>;
      case "3": // Reprovado
        return <Badge className="bg-destructive text-destructive-foreground">Reprovado</Badge>;
      default:
        return <Badge variant="secondary">Não apontado</Badge>;
    }
  };

  // Buscar aprovadores de um apontamento pelo projeto
  const getAprovadoresPorProjeto = (projetoId: string) => {
    const apontamentoMensal = apontamentosMensais.find(
      ap => ap.projeto?.id === projetoId
    );
    
    if (!apontamentoMensal || !apontamentoMensal.aprovadores) {
      return [];
    }
    
    return apontamentoMensal.aprovadores;
  };

  // Formatar aprovadores: "Nome + N" quando mais de 1, clicável para abrir modal com a lista
  const renderizarAprovadores = (apontamento: ColaboradorApontamento) => {
    const aprovadores = getAprovadoresPorProjeto(apontamento.projeto.id);

    if (aprovadores.length === 0) {
      return <span className="text-muted-foreground">-</span>;
    }

    const primeiro = aprovadores[0];
    const restantes = aprovadores.length - 1;
    const textoExibicao = restantes > 0 ? `${primeiro.nome} + ${restantes}` : primeiro.nome;
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
    return <span className="text-sm">{primeiro.nome}</span>;
  };

  return (
    <div className="space-y-6">
      {loading ? (
        <div className="space-y-4">
          {[...Array(2)].map((_, weekIndex) => (
            <Card key={weekIndex} className="border border-border">
              <CardContent className="p-4 sm:p-6 space-y-4">
                <div className="flex items-center justify-between">
                  <Skeleton className="h-6 w-48" />
                  <Skeleton className="h-5 w-20" />
                </div>
                <div className="overflow-x-auto -mx-6 px-6">
                  <div className="flex gap-3 sm:gap-4 min-w-max">
                    {[...Array(7)].map((_, dayIndex) => (
                      <Card key={dayIndex} className="flex-shrink-0 w-[100px] sm:w-[120px]">
                        <CardContent className="p-3 space-y-2">
                          <Skeleton className="h-4 w-12" />
                          <Skeleton className="h-8 w-16" />
                          <Skeleton className="h-4 w-20" />
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : (
        semanas.map((semana) => {
          const tituloSemana = getTituloSemana(semana);
          const totalHoras = getTotalHorasSemana(semana);
          const temHoras = semanaTemHoras(semana);
          
          // Verificar se há um dia selecionado nesta semana
          const diaSelecionadoNaSemana = selectedDay 
            ? semana.dias.find(d => d.data === selectedDay)
            : null;
          
          return (
            <Card key={semana.numeroSemana} className="border border-border">
              <CardContent className="p-4 sm:p-6 space-y-4">
                <div className="flex items-center justify-between">
                  <h3 className="text-lg font-semibold">{tituloSemana}</h3>
                  {temHoras && (
                    <div 
                      className="flex flex-col items-center gap-1 cursor-pointer hover:opacity-80 transition-opacity"
                      onClick={() => {
                        setSemanaSelecionada(semana);
                        setShowDistribuicaoModal(true);
                      }}
                    >
                      <div className="flex items-center gap-1">
                        {(() => {
                          const statusDaSemana = getStatusDaSemana(semana);
                          const icons: React.ReactElement[] = [];
                          
                          // Ordem de prioridade: Reprovado > Aprovado > Pendente
                          if (statusDaSemana.includes("3")) {
                            icons.push(<XCircle key="reprovado" className="h-5 w-5 text-destructive" />);
                          }
                          if (statusDaSemana.includes("2")) {
                            icons.push(<CheckCircle2 key="aprovado" className="h-5 w-5 text-green-600" />);
                          }
                          if (statusDaSemana.includes("1")) {
                            icons.push(<Clock key="pendente" className="h-5 w-5 text-blue-600" />);
                          }
                          
                          return icons.length > 0 ? icons : <Clock className="h-5 w-5 text-blue-600" />;
                        })()}
                      </div>
                      <span className="text-sm font-medium">{totalHoras}</span>
                    </div>
                  )}
                </div>
                
                <div className="overflow-x-auto -mx-4 sm:-mx-6 px-4 sm:px-6">
                  <div className="flex gap-3 sm:gap-4 min-w-max">
                    {semana.dias.map((dia) => {
                      const horas = getHoras(dia);
                      const isSelected = selectedDay === dia.data;
                      
                      return (
                        <Card
                          key={dia.data}
                          className={`${getStatusColor(dia, isSelected)} transition-all hover:shadow-md cursor-pointer flex-shrink-0 w-[100px] sm:w-[120px]`}
                          onClick={() => setSelectedDay(isSelected ? null : dia.data)}
                        >
                          <CardContent className="p-4">
                            <div className="flex flex-col gap-3">
                              <div className="flex flex-col items-center">
                                <span className="text-2xl font-bold">{extrairDia(dia.data)}</span>
                                <span className="text-xs text-muted-foreground text-center">{dia.label}</span>
                                {dia.feriado && (
                                  <span className="text-xs text-orange-600 font-medium mt-0.5">(Feriado)</span>
                                )}
                              </div>
                              <div className="flex flex-col items-center justify-center gap-2 pt-2 border-t border-border/50">
                                <div className="flex items-center gap-1 justify-center">
                                  {getStatusIcons(dia)}
                                </div>
                                {horas && <span className="text-sm font-medium">{horas}</span>}
                                {!horas && <span className="text-sm font-medium text-muted-foreground">---</span>}
                              </div>
                            </div>
                          </CardContent>
                        </Card>
                      );
                    })}
                  </div>
                </div>

                {/* Day Details Table - dentro da seção da semana */}
                {diaSelecionadoNaSemana && (
                  <div className="mt-4 pt-4 border-t border-border">
                    <div className="mb-4">
                      <h4 className="text-lg font-semibold">
                        {formatarData(diaSelecionadoNaSemana.data)} - {diaSelecionadoNaSemana.label}
                      </h4>
                    </div>
                    <div className="overflow-x-auto">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Cliente</TableHead>
                            <TableHead>Projeto</TableHead>
                            {mostrarColunaAprovadores && <TableHead>Aprovadores</TableHead>}
                            <TableHead>Atividade</TableHead>
                            <TableHead>Resumo das atividades</TableHead>
                            <TableHead>Status</TableHead>
                            <TableHead>Horas</TableHead>
                            <TableHead className="text-right">Ação</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {(() => {
                            const apontamentosDia = getApontamentosDoDia(diaSelecionadoNaSemana.data);
                            
                            if (apontamentosDia.length === 0) {
                            return (
                                <TableRow>
                                  <TableCell colSpan={mostrarColunaAprovadores ? 8 : 7} className="text-center text-muted-foreground py-8">
                                    Nenhum apontamento encontrado para este dia
                                  </TableCell>
                                </TableRow>
                              );
                            }
                            
                            return apontamentosDia.map((apontamento) => {
                              const horasFormatadas = formatarHoras(apontamento.horas);
                              const clienteLabel = `${apontamento.projeto.codCliente} - ${apontamento.projeto.nomeCliente}`;
                              const projetoLabel = `${apontamento.projeto.id} - ${apontamento.projeto.nomeProjeto}`;
                              
                              return (
                                <TableRow key={apontamento.id}>
                                  <TableCell>{clienteLabel}</TableCell>
                                  <TableCell>{projetoLabel}</TableCell>
                                  {mostrarColunaAprovadores && (
                                    <TableCell>
                                      {renderizarAprovadores(apontamento)}
                                    </TableCell>
                                  )}
                                  <TableCell>{apontamento.atividade.descricao}</TableCell>
                                  <TableCell>{apontamento.observacao || "-"}</TableCell>
                                  <TableCell>
                                    {getStatusBadgePorCodigo(apontamento.codStatusApontamentoGrupo)}
                                  </TableCell>
                                  <TableCell>
                                    <span className="font-medium">{horasFormatadas}</span>
                                  </TableCell>
                                  <TableCell className="text-right">
                                    <div className="flex items-center justify-end gap-2">
                                      {apontamento.codStatusApontamentoGrupo === "1" && (
                                        <Button 
                                          variant="ghost" 
                                          size="icon" 
                                          className="h-8 w-8"
                                          onClick={() => handleAbrirEdicao(apontamento)}
                                        >
                                          <Edit className="h-4 w-4" />
                                        </Button>
                                      )}
                                      {((apontamento.codStatusApontamentoGrupo === "1" || apontamento.codStatusApontamentoGrupo === "3") || temApontamentoHorasColaborador) && (
                                        <Button 
                                          variant="ghost" 
                                          size="icon" 
                                          className="h-8 w-8 text-destructive hover:text-destructive"
                                          onClick={() => handleAbrirExclusao(apontamento)}
                                        >
                                          <Trash2 className="h-4 w-4" />
                                        </Button>
                                      )}
                                    </div>
                                  </TableCell>
                                </TableRow>
                              );
                            });
                          })()}
                        </TableBody>
                      </Table>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          );
        })
      )}

      {/* Modal de Distribuição de Horas */}
      <Dialog open={showDistribuicaoModal} onOpenChange={setShowDistribuicaoModal}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Distribuição de Horas</DialogTitle>
          </DialogHeader>
          {semanaSelecionada && (() => {
            const horasPorStatus = getHorasPorStatusSemana(semanaSelecionada);
            const tituloSemana = getTituloSemana(semanaSelecionada);
            
            return (
              <div className="space-y-4">
                <p className="text-sm text-muted-foreground">{tituloSemana}</p>
                
                <div className="grid grid-cols-2 gap-4">
                  {/* Lançadas */}
                  <Card className="border-2">
                    <CardContent className="p-4 flex flex-col items-center gap-2">
                      <div className="w-12 h-12 rounded-lg bg-blue-100 flex items-center justify-center">
                        <Timer className="h-6 w-6 text-blue-600" />
                      </div>
                      <div className="text-center">
                        <p className="text-2xl font-bold">{formatarHoras(horasPorStatus.lancadas)}</p>
                        <p className="text-sm text-muted-foreground">Lançadas</p>
                      </div>
                    </CardContent>
                  </Card>

                  {/* Aprovadas */}
                  <Card className="border-2">
                    <CardContent className="p-4 flex flex-col items-center gap-2">
                      <div className="w-12 h-12 rounded-lg bg-green-100 flex items-center justify-center">
                        <CheckCircle2 className="h-6 w-6 text-green-600" />
                      </div>
                      <div className="text-center">
                        <p className="text-2xl font-bold">{formatarHoras(horasPorStatus.aprovadas)}</p>
                        <p className="text-sm text-muted-foreground">Aprovado</p>
                      </div>
                    </CardContent>
                  </Card>

                  {/* Pendentes */}
                  <Card className="border-2 border-blue-200">
                    <CardContent className="p-4 flex flex-col items-center gap-2">
                      <div className="w-12 h-12 rounded-lg bg-blue-100 flex items-center justify-center">
                        <Clock className="h-6 w-6 text-blue-600" />
                      </div>
                      <div className="text-center">
                        <p className="text-2xl font-bold text-blue-600">{formatarHoras(horasPorStatus.pendentes)}</p>
                        <p className="text-sm text-blue-700">Pendente</p>
                      </div>
                    </CardContent>
                  </Card>

                  {/* Reprovadas */}
                  <Card className="border-2">
                    <CardContent className="p-4 flex flex-col items-center gap-2">
                      <div className="w-12 h-12 rounded-lg bg-red-100 flex items-center justify-center">
                        <XCircle className="h-6 w-6 text-red-600" />
                      </div>
                      <div className="text-center">
                        <p className="text-2xl font-bold">{formatarHoras(horasPorStatus.reprovadas)}</p>
                        <p className="text-sm text-muted-foreground">Reprovado</p>
                      </div>
                    </CardContent>
                  </Card>
                </div>
              </div>
            );
          })()}
        </DialogContent>
      </Dialog>

      {/* Modal de Edição de Apontamento */}
      <Dialog open={showEditModal} onOpenChange={setShowEditModal}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader>
            <div className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-blue-600" />
              <DialogTitle>Editar Apontamento</DialogTitle>
            </div>
          </DialogHeader>
          
          {apontamentoEditando && (
            <div className="space-y-4">
              {/* Projeto */}
              <div className="space-y-2">
                <label className="text-sm font-medium">
                  Projeto <span className="text-destructive">*</span>
                </label>
                <Popover open={projetoEditOpen} onOpenChange={setProjetoEditOpen}>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      role="combobox"
                      aria-expanded={projetoEditOpen}
                      className="h-9 w-full justify-between font-normal"
                    >
                      {selectedProjetoEdit
                        ? projetos.find((p) => p.id === selectedProjetoEdit)
                          ? formatarLabelProjeto(projetos.find((p) => p.id === selectedProjetoEdit)!)
                          : "Selecione o projeto..."
                        : "Selecione o projeto..."}
                      <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                    <Command>
                      <CommandInput placeholder="Buscar projeto..." />
                      <CommandList>
                        <CommandEmpty>Nenhum projeto encontrado.</CommandEmpty>
                        <CommandGroup>
                          {projetos.map((projeto) => {
                            const label = formatarLabelProjeto(projeto);
                            return (
                              <CommandItem
                                key={projeto.id}
                                value={label}
                                onSelect={() => {
                                  setSelectedProjetoEdit(selectedProjetoEdit === projeto.id ? "" : projeto.id);
                                  setProjetoEditOpen(false);
                                }}
                              >
                                <Check
                                  className={cn(
                                    "mr-2 h-4 w-4",
                                    selectedProjetoEdit === projeto.id ? "opacity-100" : "opacity-0"
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

              {/* Atividade */}
              <div className="space-y-2">
                <label className="text-sm font-medium">
                  Atividade <span className="text-destructive">*</span>
                </label>
                <Popover open={atividadeEditOpen} onOpenChange={setAtividadeEditOpen}>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      role="combobox"
                      aria-expanded={atividadeEditOpen}
                      className="h-9 w-full justify-between font-normal"
                      disabled={!selectedProjetoEdit || atividades.length === 0}
                    >
                      {selectedAtividadeEdit
                        ? atividades.find((a) => a.id === selectedAtividadeEdit)
                          ? atividades.find((a) => a.id === selectedAtividadeEdit)!.descricao
                          : "Selecione a atividade..."
                        : !selectedProjetoEdit
                        ? "Selecione um projeto primeiro..."
                        : "Selecione a atividade..."}
                      <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                    <Command>
                      <CommandInput placeholder="Buscar atividade..." />
                      <CommandList>
                        <CommandEmpty>Nenhuma atividade encontrada.</CommandEmpty>
                        <CommandGroup>
                          {atividades.map((atividade) => (
                            <CommandItem
                              key={atividade.id}
                              value={atividade.descricao}
                              onSelect={() => {
                                setSelectedAtividadeEdit(selectedAtividadeEdit === atividade.id ? "" : atividade.id);
                                setAtividadeEditOpen(false);
                              }}
                            >
                              <Check
                                className={cn(
                                  "mr-2 h-4 w-4",
                                  selectedAtividadeEdit === atividade.id ? "opacity-100" : "opacity-0"
                                )}
                              />
                              {atividade.descricao}
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
              </div>

              {/* Data início */}
              <div className="space-y-2">
                <label className="text-sm font-medium">
                  Data início <span className="text-destructive">*</span>
                </label>
                <Input 
                  type="date" 
                  value={dataInicioEdit} 
                  onChange={(e) => setDataInicioEdit(e.target.value)}
                  className="h-9"
                />
              </div>

              {/* Horas por dia */}
              <div className="space-y-2">
                <label className="text-sm font-medium">
                  Horas por dia <span className="text-destructive">*</span>
                </label>
                <div className="flex items-center gap-2">
                  <Input
                    type="number"
                    min="0"
                    max="23"
                    value={horasEdit}
                    placeholder="00"
                    onChange={(e) => {
                      const value = e.target.value;
                      // Permitir digitação livre, sem padding durante a digitação
                      setHorasEdit(value);
                    }}
                    onBlur={(e) => {
                      const value = e.target.value;
                      // Aplicar padding apenas no blur se necessário
                      if (value === "") {
                        setHorasEdit("");
                      } else {
                        const numValue = parseInt(value, 10);
                        if (!isNaN(numValue) && numValue >= 0 && numValue <= 23) {
                          setHorasEdit(String(numValue).padStart(2, '0'));
                        } else {
                          setHorasEdit("00");
                        }
                      }
                    }}
                    className="w-20 h-9 text-center [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                  />
                  <span className="text-sm font-medium text-muted-foreground">:</span>
                  <Input
                    type="number"
                    min="0"
                    max="59"
                    value={minutosEdit}
                    onChange={(e) => {
                      const value = e.target.value;
                      // Permitir digitação livre, sem padding durante a digitação
                      setMinutosEdit(value);
                    }}
                    onBlur={(e) => {
                      const value = e.target.value;
                      // Aplicar padding apenas no blur se necessário
                      if (value === "") {
                        setMinutosEdit("00");
                      } else {
                        const numValue = parseInt(value, 10);
                        if (!isNaN(numValue) && numValue >= 0 && numValue <= 59) {
                          setMinutosEdit(String(numValue).padStart(2, '0'));
                        } else {
                          setMinutosEdit("00");
                        }
                      }
                    }}
                    className="w-20 h-9 text-center [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                  />
                </div>
              </div>

              {/* Resumo das atividades */}
              <div className="space-y-2">
                <label className="text-sm font-medium">
                  Resumo das atividades <span className="text-destructive">*</span>
                </label>
                <Textarea
                  value={observacaoEdit}
                  onChange={(e) => setObservacaoEdit(e.target.value)}
                  placeholder="Digite aqui..."
                  className="min-h-[80px] resize-none"
                />
              </div>
            </div>
          )}

          <DialogFooter>
            <Button 
              variant="outline" 
              onClick={() => setShowEditModal(false)}
              disabled={salvando}
            >
              Cancelar
            </Button>
            <Button 
              onClick={handleSalvarEdicao}
              disabled={salvando || !selectedProjetoEdit || !selectedAtividadeEdit || !dataInicioEdit || observacaoEdit.trim() === "" || (horasEdit === "" && minutosEdit === "00")}
            >
              {salvando ? "Salvando..." : "Salvar"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal de Confirmação de Exclusão */}
      <Dialog open={showDeleteModal} onOpenChange={setShowDeleteModal}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Confirmar exclusão</DialogTitle>
          </DialogHeader>
          
          {apontamentoDeletando && (
            <div className="space-y-4">
              <p className="text-sm text-muted-foreground">
                Tem certeza que deseja excluir este apontamento? Esta ação não pode ser desfeita.
              </p>
              <div className="bg-muted/50 p-3 rounded-lg space-y-1">
                <p className="text-sm font-medium">
                  <span className="text-muted-foreground">Cliente:</span> {apontamentoDeletando.projeto.codCliente} - {apontamentoDeletando.projeto.nomeCliente}
                </p>
                <p className="text-sm font-medium">
                  <span className="text-muted-foreground">Projeto:</span> {apontamentoDeletando.projeto.nomeProjeto}
                </p>
                <p className="text-sm font-medium">
                  <span className="text-muted-foreground">Atividade:</span> {apontamentoDeletando.atividade.descricao}
                </p>
                <p className="text-sm font-medium">
                  <span className="text-muted-foreground">Data:</span> {formatarData(apontamentoDeletando.data_registro)}
                </p>
                <p className="text-sm font-medium">
                  <span className="text-muted-foreground">Horas:</span> {formatarHoras(apontamentoDeletando.horas)}
                </p>
              </div>
            </div>
          )}

          <DialogFooter>
            <Button 
              variant="outline" 
              onClick={() => setShowDeleteModal(false)}
              disabled={deletando}
            >
              Cancelar
            </Button>
            <Button 
              variant="destructive"
              onClick={handleConfirmarExclusao}
              disabled={deletando}
            >
              {deletando ? "Excluindo..." : "Excluir"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal: lista de aprovadores (coluna Aprovadores do grid do dia) */}
      <Dialog open={modalAprovadoresOpen} onOpenChange={setModalAprovadoresOpen}>
        <DialogContent className="max-w-sm">
          <DialogHeader>
            <DialogTitle>Aprovadores</DialogTitle>
          </DialogHeader>
          <ul className="list-disc list-inside space-y-1 py-2">
            {aprovadoresModalList.map((aprov, idx) => (
              <li key={idx} className="text-sm">
                {aprov.nome}
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
