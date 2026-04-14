import { useState, useEffect } from "react";
import { Plus, Building2, Calendar, ChevronDown, ChevronUp } from "@/components/ui/system-icons";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import type { ExperienciaEmpresaCompletaProfile360 } from "@domain/entities/Profile360";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";

interface ExperienceSectionProps {
  experiencias?: ExperienciaEmpresaCompletaProfile360[];
  readOnly?: boolean;
}

export const ExperienceSection = ({ experiencias = [], readOnly = false }: ExperienceSectionProps) => {
  const [expandedEmpresas, setExpandedEmpresas] = useState<Set<string>>(new Set());

  // Expandir a primeira empresa por padrão
  useEffect(() => {
    if (experiencias.length > 0 && expandedEmpresas.size === 0) {
      setExpandedEmpresas(new Set([experiencias[0].empresa]));
    }
  }, [experiencias]);

  const toggleEmpresa = (empresa: string) => {
    const newExpanded = new Set(expandedEmpresas);
    if (newExpanded.has(empresa)) {
      newExpanded.delete(empresa);
    } else {
      newExpanded.add(empresa);
    }
    setExpandedEmpresas(newExpanded);
  };

  const formatDate = (dateString: string) => {
    try {
      const date = new Date(dateString);
      return format(date, "MM/yyyy", { locale: ptBR });
    } catch {
      return dateString;
    }
  };

  const formatDateRange = (inicio: string, fim: string | null, atual: boolean) => {
    const inicioFormatado = formatDate(inicio);
    if (atual) {
      return `${inicioFormatado} - Experiência atual`;
    }
    const fimFormatado = fim ? formatDate(fim) : "";
    return `${inicioFormatado} - ${fimFormatado}`;
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Building2 className="h-5 w-5 text-muted-foreground" />
          <h2 className="text-xl font-semibold text-foreground">Experiências</h2>
        </div>
        {!readOnly && (
          <Button size="sm" variant="outline" className="h-8">
            <Plus className="h-3.5 w-3.5 mr-1.5" />
            Adicionar
          </Button>
        )}
      </div>

      <div className="space-y-3">
        {experiencias.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground">Nenhuma experiência cadastrada</div>
        ) : (
          experiencias.map((empresaExp, index) => {
            const isExpanded = expandedEmpresas.has(empresaExp.empresa);
            const dataRange = formatDateRange(empresaExp.dataInicio, empresaExp.dataSaida, empresaExp.atual);

            return (
              <div
                key={`${empresaExp.empresa}-${index}`}
                className="rounded-xl border bg-card transition-all hover:shadow-md"
              >
                {/* Header da Empresa - Sempre visível */}
                <div
                  className="flex items-center justify-between p-4 cursor-pointer"
                  onClick={() => toggleEmpresa(empresaExp.empresa)}
                >
                  <div className="flex items-center gap-3 flex-1">
                    <div className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-950">
                      <Building2 className="h-5 w-5 text-violet-600 dark:text-violet-400" />
                    </div>
                    <div className="flex-1">
                      <h3 className="font-semibold text-foreground text-base">{empresaExp.empresa}</h3>
                      <div className="flex items-center gap-1.5 text-sm text-muted-foreground mt-0.5">
                        <Calendar className="h-3.5 w-3.5" />
                        <span>{dataRange}</span>
                      </div>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    {isExpanded ? (
                      <ChevronUp className="h-5 w-5 text-muted-foreground" />
                    ) : (
                      <ChevronDown className="h-5 w-5 text-muted-foreground" />
                    )}
                  </div>
                </div>

                {/* Conteúdo Expandido */}
                {isExpanded && (
                  <div className="px-4 pb-4 space-y-4 border-t">
                    {/* Experiências (Funções) */}
                    {empresaExp.experiencias.map((exp, expIdx) => (
                      <div key={exp.id || expIdx} className="pt-4 space-y-3">
                        <div className="flex items-center gap-2">
                          <h4 className="font-medium text-foreground">{exp.funcao}</h4>
                          {exp.atual && (
                            <Badge variant="secondary" className="bg-primary/10 text-primary text-xs">
                              Atual
                            </Badge>
                          )}
                        </div>
                        <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                          <Calendar className="h-3.5 w-3.5" />
                          <span>
                            {formatDateRange(exp.dataInicio, exp.dataSaida, exp.atual)}
                          </span>
                        </div>
                        {exp.atividades && exp.atividades.trim() && (
                          <ul className="mt-2 space-y-1.5 ml-4">
                            {exp.atividades.split("•").filter(a => a.trim()).map((atividade, idx) => (
                              <li
                                key={idx}
                                className="flex items-start gap-2 text-sm text-muted-foreground"
                              >
                                <span className="mt-1.5 h-1 w-1 rounded-full bg-primary flex-shrink-0" />
                                <span>{atividade.trim()}</span>
                              </li>
                            ))}
                          </ul>
                        )}
                        {/* Projetos */}
                        {exp.projetos && exp.projetos.length > 0 && (
                          <div className="mt-3">
                            <h5 className="text-sm font-medium text-foreground mb-2">Projetos</h5>
                            <div className="flex flex-wrap gap-2">
                              {exp.projetos.map((projeto, projIdx) => {
                                // Cores alternadas: azul escuro e azul claro
                                const colors = [
                                  "bg-blue-600 text-white hover:bg-blue-700",
                                  "bg-blue-400 text-white hover:bg-blue-500"
                                ];
                                const colorClass = colors[projIdx % colors.length];
                                
                                return (
                                  <Badge
                                    key={projIdx}
                                    className={`${colorClass} px-3 py-1 text-sm font-medium`}
                                  >
                                    {projeto}
                                  </Badge>
                                );
                              })}
                            </div>
                          </div>
                        )}
                      </div>
                    ))}

                    {/* Realizações */}
                    {empresaExp.realizacoes && empresaExp.realizacoes.length > 0 && (
                      <div className="pt-4 border-t">
                        <h4 className="font-medium text-foreground mb-3">Realizações</h4>
                        <div className="space-y-3">
                          {empresaExp.realizacoes.map((realizacao, realIdx) => {
                            const projetoOuCliente = realizacao.projeto && realizacao.projeto.trim() !== "-" 
                              ? realizacao.projeto 
                              : realizacao.cliente && realizacao.cliente.trim() !== "-"
                              ? realizacao.cliente
                              : null;
                            
                            const perfil = realizacao.perfil && realizacao.perfil.trim() !== "-" 
                              ? realizacao.perfil 
                              : null;

                            return (
                              <div key={realIdx} className="bg-gray-50 rounded-lg p-3 space-y-1">
                                {projetoOuCliente && (
                                  <div className="text-sm font-medium text-foreground">
                                    {projetoOuCliente}
                                  </div>
                                )}
                                {perfil && (
                                  <div className="text-xs text-muted-foreground">
                                    Perfil de atuação: {perfil}
                                  </div>
                                )}
                                <div className="text-xs text-muted-foreground">
                                  {realizacao.horas} horas
                                </div>
                                <div className="text-xs text-muted-foreground">
                                  {formatDateRange(realizacao.dataInicio, realizacao.dataFim, realizacao.atual)}
                                </div>
                              </div>
                            );
                          })}
                        </div>
                      </div>
                    )}
                  </div>
                )}
              </div>
            );
          })
        )}
      </div>
    </div>
  );
};
