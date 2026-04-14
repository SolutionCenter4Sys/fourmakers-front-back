import { Plus, GraduationCap, Calendar } from "@/components/ui/system-icons";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { EscolaridadeColaborador } from "@domain/entities/Escolaridade";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";

interface EducationSectionProps {
  escolaridades?: EscolaridadeColaborador[];
  escolaridade?: string;
  readOnly?: boolean;
}

const educationLevels = [
  "Ensino Fundamental",
  "Ensino Médio",
  "Ensino Superior Incompleto",
  "Ensino Superior Completo",
  "Pós-graduação",
  "MBA",
  "Mestrado",
  "Doutorado",
];

export const EducationSection = ({ escolaridades = [], escolaridade, readOnly = false }: EducationSectionProps) => {
  const formatDate = (dateString: string) => {
    try {
      const date = new Date(dateString);
      return format(date, "MM/yyyy", { locale: ptBR });
    } catch {
      return dateString;
    }
  };

  const formatDateRange = (inicio: string, termino: string | null) => {
    const inicioFormatado = formatDate(inicio);
    if (!termino || termino.trim() === "" || termino === "0001-01-01T00:00:00") {
      return `${inicioFormatado} - Em andamento`;
    }
    const terminoFormatado = formatDate(termino);
    return `${inicioFormatado} - ${terminoFormatado}`;
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <GraduationCap className="h-5 w-5 text-muted-foreground" />
          <h2 className="text-xl font-semibold text-foreground">Escolaridade</h2>
        </div>
        {!readOnly && (
          <Select defaultValue={escolaridade || "Ensino Superior Completo"}>
            <SelectTrigger className="w-[220px] h-9 text-sm">
              <SelectValue />
            </SelectTrigger>
            <SelectContent className="bg-popover z-50">
              {educationLevels.map((level) => (
                <SelectItem key={level} value={level} className="text-sm">
                  {level}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        )}
        {readOnly && escolaridade && (
          <Badge variant="outline" className="text-sm">
            {escolaridade}
          </Badge>
        )}
      </div>

      <div className="flex items-center justify-between pt-2">
        <h3 className="text-lg font-semibold text-foreground">Formação Acadêmica</h3>
        {!readOnly && (
          <Button size="sm" variant="outline" className="h-8">
            <Plus className="h-3.5 w-3.5 mr-1.5" />
            Adicionar
          </Button>
        )}
      </div>

      <div className="space-y-3">
        {escolaridades.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground">Nenhuma formação cadastrada</div>
        ) : (
          escolaridades.map((escolaridadeItem) => (
            <div
              key={escolaridadeItem.id}
              className="rounded-xl border bg-card p-4 transition-all hover:shadow-md"
            >
              <div className="flex items-start gap-4">
                <div className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-950">
                  <GraduationCap className="h-5 w-5 text-violet-600 dark:text-violet-400" />
                </div>
                <div className="flex-1 space-y-2">
                  <div>
                    <h4 className="font-semibold text-foreground text-base">
                      {escolaridadeItem.formacaoDescricao || escolaridadeItem.descricao || "Formação"}
                    </h4>
                    {escolaridadeItem.instituicao && (
                      <p className="text-sm text-muted-foreground mt-0.5">{escolaridadeItem.instituicao}</p>
                    )}
                  </div>
                  <div className="flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
                    {escolaridadeItem.descricao && (
                      <Badge variant="outline" className="text-sm font-normal">
                        {escolaridadeItem.descricao}
                      </Badge>
                    )}
                    <div className="flex items-center gap-1">
                      <Calendar className="h-3.5 w-3.5" />
                      <span>
                        {formatDateRange(escolaridadeItem.dataInicio, escolaridadeItem.dataTermino)}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};
