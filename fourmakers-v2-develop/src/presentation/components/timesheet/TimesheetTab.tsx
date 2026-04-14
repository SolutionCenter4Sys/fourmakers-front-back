import { useState, useEffect, useCallback } from "react";
import { useSearchParams } from "react-router-dom";
import { Triangle, CheckCircle2, XCircle, Clock, Timer, AlertCircle, ChevronDown, ChevronUp, FileText } from "lucide-react";
import type { LucideIcon } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { TimesheetForm } from "./TimesheetForm";
import { WeeklyView } from "./WeeklyView";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useAppSelector } from "@app/store/hooks";
import { useParametros } from "@presentation/hooks/useParametros";
import { container } from "@core/di/container";
import { TimesheetComponentesApi, type MesVigencia, type PeriodoFechado, type ApontamentoMensal } from "@data/api/TimesheetComponentesApi";
import { SumarioPorProjetoReadOnlyTable } from "./SumarioPorProjetoReadOnlyTable";

interface TimesheetStat {
  icon: LucideIcon;
  label: string;
  value: string;
  textColor: string;
}

interface TimesheetTabProps {
  cpfColaborador?: string; // Se não houver, será undefined. Se houver mas vazio, será ''
  mes?: number;
  ano?: number;
  onPeriodChange?: (mes: number, ano: number) => void;
}

export const TimesheetTab = ({ cpfColaborador, mes, ano, onPeriodChange }: TimesheetTabProps = {}) => {
  const { token, user } = useAppSelector((state) => state.auth);
  const { isEnabled } = useParametros();
  const mostrarColunaAprovadores = isEnabled("MOSTRAR_COLUNA_APROVADORES_TIMESHEET");
  const ocultaTimeSheet = user?.ocultaTimeSheet === true;
  const mostrarEspelhoPonto = isEnabled("TIMESHEET_APRESENTAR_ESPELHO_PONTO");
  const [searchParams, setSearchParams] = useSearchParams();
  
  // Se mes e ano vierem da URL, usar como período inicial
  const initialPeriod = mes && ano ? `${mes}-${ano}` : "";
  const [selectedPeriod, setSelectedPeriod] = useState<string>(initialPeriod);
  
  // Flag para indicar se cpfColaborador foi definido (mesmo que seja string vazia)
  // Isso ajuda a evitar chamadas antes do cpfColaborador estar disponível
  const cpfColaboradorDefinido = cpfColaborador !== undefined;
  
  // Função para atualizar o período selecionado e a URL quando houver id
  const handlePeriodChange = (newPeriod: string) => {
    setSelectedPeriod(newPeriod);
    
    const [newMes, newAno] = newPeriod.split('-').map(Number);
    if (newMes && newAno) {
      // Notificar o componente pai sobre a mudança de período
      if (onPeriodChange) {
        onPeriodChange(newMes, newAno);
      }
      
      // Se houver cpfColaborador (id na URL), atualizar os parâmetros mes e ano na URL
      if (cpfColaborador) {
        const colaboradorNome = searchParams.get('nome');
        const newParams = new URLSearchParams(searchParams);
        newParams.set('mes', newMes.toString());
        newParams.set('ano', newAno.toString());
        // Manter o id e nome se existirem
        if (colaboradorNome) {
          newParams.set('nome', colaboradorNome);
        }
        setSearchParams(newParams);
      }
    }
  };
  const [vigencias, setVigencias] = useState<MesVigencia[]>([]);
  const [loadingVigencias, setLoadingVigencias] = useState(true);
  const [refreshKey, setRefreshKey] = useState(0);
  const [stats, setStats] = useState<TimesheetStat[]>([]);
  const [loadingStats, setLoadingStats] = useState(true);
  const [periodoFechado, setPeriodoFechado] = useState<PeriodoFechado | null>(null);
  const [apontamentosMensais, setApontamentosMensais] = useState<ApontamentoMensal[]>([]);
  const [pdfFolhaPontoUrl, setPdfFolhaPontoUrl] = useState<string | null>(null);
  const [sumarioExpandido, setSumarioExpandido] = useState(false);

  // Converter minutos para formato HH:MM
  const formatarHoras = (minutos: number): string => {
    const horas = Math.floor(minutos / 60);
    const mins = minutos % 60;
    return `${String(horas).padStart(2, '0')}:${String(mins).padStart(2, '0')}`;
  };

  // Função para carregar big numbers
  const loadBigNumbers = useCallback(async () => {
      if (!token || !selectedPeriod) {
        setLoadingStats(false);
        return;
      }

      try {
        setLoadingStats(true);
        const [mes, ano] = selectedPeriod.split('-').map(Number);
        
        if (!mes || !ano) {
          setLoadingStats(false);
          return;
        }

        const api = container.resolve(TimesheetComponentesApi);
        // IMPORTANTE: Se houver cpfColaborador, sempre usar ele (mesmo que seja string vazia)
        // Se não houver, usar string vazia
        const cpfParaEnvio = cpfColaborador !== undefined ? cpfColaborador : '';
        const response = await api.listarApontamentosPorVigencia(token, mes, ano, cpfParaEnvio);

        setApontamentosMensais(response.apontamentos_por_vigencia?.apontamentosMensais ?? []);
        setPdfFolhaPontoUrl(response.apontamentos_por_vigencia?.pdfFolhaPontoUrl ?? null);
        if (response.apontamentos_por_vigencia?.totalizadorApontamentosBigNumbers) {
          const bigNumbers = response.apontamentos_por_vigencia.totalizadorApontamentosBigNumbers;
          
          setStats([
            {
              icon: Timer,
              label: "Horas lançadas",
              value: formatarHoras(bigNumbers.somaHorasLancadas),
              textColor: "text-foreground",
            },
            {
              icon: CheckCircle2,
              label: "Aprovadas",
              value: formatarHoras(bigNumbers.somaHorasAprovadas),
              textColor: "text-green-600",
            },
            {
              icon: Clock,
              label: "Pendentes",
              value: formatarHoras(bigNumbers.somaHorasPendentes),
              textColor: "text-blue-600",
            },
            {
              icon: XCircle,
              label: "Reprovados",
              value: formatarHoras(bigNumbers.somaHorasReprovdas),
              textColor: "text-destructive",
            },
          ]);
        } else {
          // Se não houver dados, inicializar com zeros
          setStats([
            {
              icon: Timer,
              label: "Horas lançadas",
              value: "00:00",
              textColor: "text-foreground",
            },
            {
              icon: CheckCircle2,
              label: "Aprovadas",
              value: "00:00",
              textColor: "text-green-600",
            },
            {
              icon: Clock,
              label: "Pendentes",
              value: "00:00",
              textColor: "text-blue-600",
            },
            {
              icon: XCircle,
              label: "Reprovados",
              value: "00:00",
              textColor: "text-destructive",
            },
          ]);
        }
      } catch (err) {
        console.error("Erro ao carregar big numbers:", err);
        setApontamentosMensais([]);
        setPdfFolhaPontoUrl(null);
        // Em caso de erro, inicializar com zeros
        setStats([
          {
            icon: Timer,
            label: "Horas lançadas",
            value: "00:00",
            textColor: "text-foreground",
          },
          {
            icon: CheckCircle2,
            label: "Aprovadas",
            value: "00:00",
            textColor: "text-green-600",
          },
            {
              icon: Clock,
              label: "Pendentes",
              value: "00:00",
              textColor: "text-blue-600",
            },
            {
              icon: XCircle,
              label: "Reprovados",
            value: "00:00",
            textColor: "text-destructive",
          },
        ]);
      } finally {
        setLoadingStats(false);
      }
  }, [token, selectedPeriod, cpfColaborador]);

  // Verificar se a vigência selecionada está dentro do período fechado
  const vigenciaEstaFechada = (): boolean => {
    if (!periodoFechado || !selectedPeriod) return false;

    const [mes, ano] = selectedPeriod.split('-').map(Number);
    if (!mes || !ano) return false;

    const dataFimPeriodoFechado = new Date(periodoFechado.dataFim);
    const ultimoDiaMesVigencia = new Date(ano, mes, 0); // Último dia do mês

    // Verificar se a vigência selecionada é anterior ou igual ao período fechado
    return ultimoDiaMesVigencia <= dataFimPeriodoFechado;
  };

  // Carregar big numbers quando token, período ou cpfColaborador mudar
  // Mas só se tiver período selecionado
  // IMPORTANTE: Se houver cpfColaborador na URL, aguardar ele estar disponível antes de fazer chamadas
  // Isso evita chamadas duplicadas (uma sem cpfColaborador e outra com)
  useEffect(() => {
    if (selectedPeriod) {
      // Se cpfColaborador foi definido (mesmo que seja string vazia), pode fazer a chamada
      // Se for undefined, pode ser que ainda não tenha sido passado, então aguardar
      // Mas como o cpfColaborador vem da URL e está disponível desde o início, podemos fazer a chamada
      loadBigNumbers();
    }
  }, [selectedPeriod, loadBigNumbers, cpfColaboradorDefinido]);

  const handleEnvioSucesso = () => {
    // Forçar refresh do WeeklyView
    setRefreshKey(prev => prev + 1);
    // Chamar função de refetch se existir
    if ((window as any).refetchWeeklyView) {
      (window as any).refetchWeeklyView();
    }
    // Recarregar big numbers
    loadBigNumbers();
  };

  // Vigência = mês do dia seguinte à data de fechamento (ex.: dataFim 20/01 -> 21/01 -> Jan; dataFim 31/01 -> 01/02 -> Fev). Sem período fechado = mês/ano atual.
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

  useEffect(() => {
    const loadVigencias = async () => {
      if (!token) {
        setLoadingVigencias(false);
        return;
      }

      try {
        setLoadingVigencias(true);
        const api = container.resolve(TimesheetComponentesApi);
        const cpfParaEnvio = cpfColaborador !== undefined ? cpfColaborador : "";
        const [response, periodo] = await Promise.all([
          api.listarVigenciasColaborador(token, cpfParaEnvio),
          api.buscarPeriodoFechado(token, "pt-BR"),
        ]);
        if (periodo) setPeriodoFechado(periodo);

        let meses = response.meses || [];

        // Quando não há mes/ano na URL: exibir mês subsequente ao período fechado (quando houver), senão vigência do mês/ano atual. Se não existir na lista, injeta e seleciona.
        if (!mes || !ano) {
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
          const defaultKey = `${mesDefault}-${anoDefault}`;
          setSelectedPeriod((prev) => prev || defaultKey);
        } else {
          // mes/ano vindos da URL (ex.: clique no colaborador na Gestão Adm): garantir que a vigência exista na lista para o Select exibir
          const mesNum = Number(mes);
          const anoNum = Number(ano);
          if (mesNum && anoNum) {
            const existeNaLista = meses.some((v) => v.mes === mesNum && v.ano === anoNum);
            if (!existeNaLista) {
              meses = [...meses, { mes: mesNum, ano: anoNum, label: formatarLabelVigencia(mesNum, anoNum) }];
            }
          }
          setVigencias(meses);
        }
      } catch (err) {
        console.error("Erro ao carregar vigencias:", err);
      } finally {
        setLoadingVigencias(false);
      }
    };

    loadVigencias();
  }, [token, cpfColaborador, mes, ano]);

  return (
    <div className="space-y-6">
      {/* Aviso de Período Fechado */}
      {vigenciaEstaFechada() && (
        <div className="flex items-center justify-end gap-2 text-sm text-amber-600 bg-amber-50 border border-amber-200 rounded-lg px-4 py-2">
          <AlertCircle className="h-4 w-4" />
          <span>Período até {new Date(periodoFechado!.dataFim).toLocaleDateString('pt-BR')} está fechado.</span>
        </div>
      )}

      {/* Vigência e Big Numbers na mesma linha */}
      <div className="flex flex-col lg:flex-row gap-4 items-start lg:items-end">
        <div className="w-full lg:w-64">
          <label className="text-sm font-medium mb-2 block">Vigência</label>
          {loadingVigencias ? (
            <Skeleton className="h-10 w-full" />
          ) : (
            <Select value={selectedPeriod} onValueChange={handlePeriodChange} disabled={loadingVigencias}>
              <SelectTrigger className="border-muted-foreground/30">
                <SelectValue placeholder="Selecione uma vigência" />
              </SelectTrigger>
              <SelectContent>
                {vigencias.map((vigencia) => {
                  const value = `${vigencia.mes}-${vigencia.ano}`;
                  return (
                    <SelectItem key={value} value={value}>
                      {vigencia.label}
                    </SelectItem>
                  );
                })}
              </SelectContent>
            </Select>
          )}
        </div>

        <div className="flex-1 grid grid-cols-2 lg:grid-cols-4 gap-4">
          {loadingStats ? (
            <>
              {[...Array(4)].map((_, index) => (
                <Card key={index} className="bg-card">
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
            </>
          ) : (
            stats.map((stat, index) => (
              <Card key={index} className="bg-card">
                <CardContent className="p-4">
                  <div className="flex items-start gap-3">
                    <div className={`mt-1 ${stat.textColor}`}>
                      <stat.icon className="h-5 w-5" />
                    </div>
                    <div className="flex-1">
                      <p className="text-xs text-muted-foreground mb-1">{stat.label}</p>
                      <p className={`text-2xl font-bold ${stat.textColor}`}>{stat.value}</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))
          )}
        </div>
      </div>

      {/* Sumário por projeto (colapsável) */}
      <Card>
        <CardContent className="p-0">
          <button
            type="button"
            onClick={() => setSumarioExpandido((prev) => !prev)}
            className="flex items-center justify-between w-full px-4 py-3 border-b text-left hover:bg-muted/50 transition-colors rounded-t-lg"
            aria-expanded={sumarioExpandido}
          >
            <h2 className="text-xl font-semibold">Sumário por projeto</h2>
            {sumarioExpandido ? (
              <ChevronUp className="h-5 w-5 text-muted-foreground shrink-0" />
            ) : (
              <ChevronDown className="h-5 w-5 text-muted-foreground shrink-0" />
            )}
          </button>
          {sumarioExpandido && (
            <div className="overflow-x-auto">
              {loadingStats ? (
                <div className="p-4 space-y-2">
                  <Skeleton className="h-10 w-full" />
                  <Skeleton className="h-10 w-full" />
                  <Skeleton className="h-10 w-full" />
                </div>
              ) : (
                <SumarioPorProjetoReadOnlyTable
                  apontamentosMensais={apontamentosMensais}
                  formatarHoras={formatarHoras}
                  mostrarColunaAprovadores={mostrarColunaAprovadores}
                />
              )}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Timesheet Form (oculto quando user.ocultaTimeSheet) */}
      {!ocultaTimeSheet && (
        <div>
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-4">
            <h2 className="text-2xl font-semibold">Timesheet semanal</h2>
            <div className="flex flex-wrap items-center gap-3 sm:gap-4 text-sm">
              <div className="flex items-center gap-2 whitespace-nowrap">
                <Triangle className="h-4 w-4 text-muted-foreground flex-shrink-0" />
                <span>Não apontado</span>
              </div>
              <div className="flex items-center gap-2 whitespace-nowrap">
                <CheckCircle2 className="h-4 w-4 text-green-600 flex-shrink-0" />
                <span>Aprovado</span>
              </div>
              <div className="flex items-center gap-2 whitespace-nowrap">
                <Clock className="h-4 w-4 text-blue-600 flex-shrink-0" />
                <span>Pendente</span>
              </div>
              <div className="flex items-center gap-2 whitespace-nowrap">
                <XCircle className="h-4 w-4 text-destructive flex-shrink-0" />
                <span>Reprovado</span>
              </div>
            </div>
          </div>
          <TimesheetForm onEnvioSucesso={handleEnvioSucesso} cpfColaborador={cpfColaborador} />
        </div>
      )}

      {/* Botão Espelho de Ponto: apenas com id na URL e parâmetro TIMESHEET_APRESENTAR_ESPELHO_PONTO */}
      {cpfColaborador && mostrarEspelhoPonto && pdfFolhaPontoUrl && token && (
        <div className="flex justify-start">
          <Button
            type="button"
            variant="outline"
            onClick={() => {
              const url = pdfFolhaPontoUrl.replace(/\$1/g, btoa(token));
              window.open(url, "_blank", "noopener,noreferrer");
            }}
          >
            <FileText className="h-4 w-4 mr-2" />
            Espelho de Ponto
          </Button>
        </div>
      )}

      {/* Weekly View */}
      <div>
        <WeeklyView key={refreshKey} period={selectedPeriod} onRefetch={handleEnvioSucesso} cpfColaborador={cpfColaborador} mostrarColunaAprovadores={mostrarColunaAprovadores} />
      </div>
    </div>
  );
};
