import { useState } from "react";
import type { JSX } from "react";
import { Triangle, Clock, CheckCircle2, XCircle, Circle } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import type { ColaboradorApontamento, ApontamentoMensal, DiaTemplate, SemanaTemplate } from "@data/api/TimesheetComponentesApi";

interface WeeklyViewApprovalProps {
  semanas: SemanaTemplate[];
  apontamentos: ColaboradorApontamento[];
  apontamentosMensais: ApontamentoMensal[];
  mes: number;
  ano: number;
  selectedApontamentos: Set<string>;
  onToggleApontamento: (id: string) => void;
  onSelectSemana: (semana: SemanaTemplate) => void;
}

export const WeeklyViewApproval = ({ 
  semanas, 
  apontamentos, 
  apontamentosMensais, 
  mes, 
  ano: _ano,
  selectedApontamentos,
  onToggleApontamento,
  onSelectSemana
}: WeeklyViewApprovalProps) => {
  const [selectedDay, setSelectedDay] = useState<string | null>(null);

  const formatarHoras = (minutos: number): string => {
    const horas = Math.floor(minutos / 60);
    const mins = minutos % 60;
    return `${String(horas).padStart(2, '0')}:${String(mins).padStart(2, '0')}`;
  };

  const formatarData = (dataString: string): string => {
    const data = new Date(dataString);
    return data.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' });
  };

  const extrairDia = (dataString: string): string => {
    const data = new Date(dataString);
    return data.getDate().toString().padStart(2, '0');
  };

  // Obter apontamentos de um dia específico
  const getApontamentosDoDia = (data: string): ColaboradorApontamento[] => {
    const dataFormatada = new Date(data).toISOString().split('T')[0];
    return apontamentos.filter(ap => {
      const apData = new Date(ap.data_registro).toISOString().split('T')[0];
      return apData === dataFormatada;
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

  // Formatar título da semana (ex: "Dezembro de 1 a 6")
  const getTituloSemana = (semana: SemanaTemplate): string => {
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

  // Obter ícones de status para um dia
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
    
    const statusDia = getStatusDoDia(dia);
    const icons: JSX.Element[] = [];
    
    // Ordem de prioridade: Reprovado > Aprovado > Pendente
    if (statusDia.includes("3")) {
      icons.push(<XCircle key="reprovado" className="h-4 w-4 text-destructive" />);
    }
    if (statusDia.includes("2")) {
      icons.push(<CheckCircle2 key="aprovado" className="h-4 w-4 text-green-600" />);
    }
    if (statusDia.includes("1")) {
      icons.push(<Clock key="pendente" className="h-4 w-4 text-blue-600" />);
    }
    
    return icons.length > 0 ? icons : [<Triangle key="sem-status" className="h-4 w-4 text-muted-foreground" />];
  };

  // Obter cor do card baseado no status
  const getStatusColor = (dia: DiaTemplate, isSelected: boolean): string => {
    if (isSelected) {
      return "border-primary border-2";
    }
    if (!dia.mesAtivo) {
      return "border-muted/30 bg-muted/10 opacity-50";
    }
    if (dia.feriado) {
      return "border-orange-200 bg-orange-50";
    }
    
    const apontamentosDia = getApontamentosDoDia(dia.data);
    const temApontamentos = apontamentosDia.length > 0;
    
    if (!temApontamentos) {
      return "border-muted bg-muted/30";
    }
    
    // Verificar se há algum reprovado
    const temReprovado = apontamentosDia.some(ap => ap.codStatusApontamentoGrupo === "3");
    if (temReprovado) {
      return "border-destructive/50 bg-destructive/5";
    }
    
    // Verificar se há algum pendente
    const temPendente = apontamentosDia.some(ap => ap.codStatusApontamentoGrupo === "1");
    if (temPendente) {
      return "border-primary/50 bg-primary/5";
    }
    
    // Verificar se TODOS estão aprovados
    const todosAprovados = apontamentosDia.every(ap => ap.codStatusApontamentoGrupo === "2");
    if (todosAprovados) {
      return "border-green-600/50 bg-green-50";
    }
    
    return "border-muted bg-muted/30";
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

  // Formatar aprovadores como tags
  const renderizarAprovadores = (apontamento: ColaboradorApontamento) => {
    const aprovadores = getAprovadoresPorProjeto(apontamento.projeto.id);
    
    if (aprovadores.length === 0) {
      return <span className="text-muted-foreground">-</span>;
    }
    
    const primeiro = aprovadores[0];
    const restantes = aprovadores.length - 1;
    
    return (
      <div className="flex items-center gap-2 flex-wrap">
        <Badge variant="outline">{primeiro.nome}</Badge>
        {restantes > 0 && (
          <Badge variant="outline" className="text-muted-foreground">
            +{restantes}
          </Badge>
        )}
      </div>
    );
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

  return (
    <div className="space-y-6">
      {semanas && semanas.length > 0 ? (
        semanas.map((semana) => {
        const tituloSemana = getTituloSemana(semana);
        const totalHoras = getTotalHorasSemana(semana);
        const temHoras = semanaTemHoras(semana);
        
        // Verificar se há um dia selecionado nesta semana
        const diaSelecionadoNaSemana = selectedDay 
          ? semana.dias.find(d => d.data === selectedDay)
          : null;
        
        // Apontamentos pendentes (codStatusApontamentoGrupo = "1") nesta semana
        const apontamentosPendentesSemana = apontamentos.filter(ap => {
          const apData = new Date(ap.data_registro).toISOString().split('T')[0];
          return semana.dias.some(dia => {
            const diaData = new Date(dia.data).toISOString().split('T')[0];
            return apData === diaData && ap.codStatusApontamentoGrupo === "1";
          });
        });
        const semanaTemPendente = apontamentosPendentesSemana.length > 0;
        const todosPendentesSelecionados = semanaTemPendente && 
          apontamentosPendentesSemana.every(ap => selectedApontamentos.has(ap.id));
        
        return (
          <Card key={`semana-${semana.numeroSemana}-${semana.primeiroDiaSemana}`} className="border border-border">
            <CardContent className="p-4 sm:p-6 space-y-4">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  {semanaTemPendente && (
                    <Checkbox
                      checked={todosPendentesSelecionados}
                      onCheckedChange={(checked) => {
                        if (checked) {
                          onSelectSemana(semana);
                        } else {
                          apontamentosPendentesSemana.forEach(ap => {
                            onToggleApontamento(ap.id);
                          });
                        }
                      }}
                    />
                  )}
                  <h3 className="text-lg font-semibold">{tituloSemana}</h3>
                </div>
                {temHoras && (
                  <div className="flex flex-col items-center gap-1">
                    <div className="flex items-center gap-1">
                      {(() => {
                        const statusDaSemana = getStatusDaSemana(semana);
                        const icons: JSX.Element[] = [];
                        
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
                              {!horas && dia.mesAtivo && <span className="text-sm font-medium text-muted-foreground">---</span>}
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
                          <TableHead className="w-[50px]"></TableHead>
                          <TableHead>Cliente</TableHead>
                          <TableHead>Projeto</TableHead>
                          <TableHead>Aprovadores</TableHead>
                          <TableHead>Atividade</TableHead>
                          <TableHead>Resumo das atividades</TableHead>
                          <TableHead>Status</TableHead>
                          <TableHead>Horas</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {(() => {
                          const apontamentosDia = getApontamentosDoDia(diaSelecionadoNaSemana.data);
                          
                          if (apontamentosDia.length === 0) {
                            return (
                              <TableRow>
                                <TableCell colSpan={8} className="text-center text-muted-foreground py-8">
                                  Nenhum apontamento encontrado para este dia
                                </TableCell>
                              </TableRow>
                            );
                          }
                          
                          return apontamentosDia.map((apontamento) => {
                            const horasFormatadas = formatarHoras(apontamento.horas);
                            const clienteLabel = `${apontamento.projeto.codCliente} - ${apontamento.projeto.nomeCliente}`;
                            const projetoLabel = `${apontamento.projeto.id} - ${apontamento.projeto.nomeProjeto}`;
                            const isPendente = apontamento.codStatusApontamentoGrupo === "1";
                            const isSelected = selectedApontamentos.has(apontamento.id);
                            
                            return (
                              <TableRow key={apontamento.id}>
                                <TableCell>
                                  {isPendente ? (
                                    <Checkbox
                                      checked={isSelected}
                                      onCheckedChange={() => onToggleApontamento(apontamento.id)}
                                    />
                                  ) : null}
                                </TableCell>
                                <TableCell>{clienteLabel}</TableCell>
                                <TableCell>{projetoLabel}</TableCell>
                                <TableCell>
                                  {renderizarAprovadores(apontamento)}
                                </TableCell>
                                <TableCell>{apontamento.atividade.descricao}</TableCell>
                                <TableCell>{apontamento.observacao || "-"}</TableCell>
                                <TableCell>
                                  {getStatusBadgePorCodigo(apontamento.codStatusApontamentoGrupo)}
                                </TableCell>
                                <TableCell>
                                  <span className="font-medium">{horasFormatadas}</span>
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
      })) : (
        <Card>
          <CardContent className="p-6">
            <p className="text-center text-muted-foreground">
              {semanas === undefined || semanas === null 
                ? "Carregando semanas..." 
                : "Nenhuma semana encontrada para este período."}
            </p>
          </CardContent>
        </Card>
      )}
    </div>
  );
};
