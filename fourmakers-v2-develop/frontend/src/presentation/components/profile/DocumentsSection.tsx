import { Plus, FileText, Calendar } from "@/components/ui/system-icons";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import type { VistoProfile360, PassaporteProfile360 } from "@domain/entities/Profile360";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";

interface DocumentsSectionProps {
  vistos?: VistoProfile360[];
  passaportes?: PassaporteProfile360[];
  readOnly?: boolean;
}

export const DocumentsSection = ({ 
  vistos = [], 
  passaportes = [],
  readOnly = false 
}: DocumentsSectionProps) => {
  const formatDate = (dateString: string) => {
    try {
      const date = new Date(dateString);
      return format(date, "dd/MM/yyyy", { locale: ptBR });
    } catch {
      return dateString;
    }
  };

  const isExpiringSoon = (dateString: string) => {
    try {
      const expiryDate = new Date(dateString);
      const sixMonthsFromNow = new Date();
      sixMonthsFromNow.setMonth(sixMonthsFromNow.getMonth() + 6);
      return expiryDate < sixMonthsFromNow;
    } catch {
      return false;
    }
  };

  return (
    <div className="space-y-5">
      <h2 className="text-xl font-semibold text-foreground">Documentos</h2>

      {/* Vistos */}
      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <h4 className="text-sm font-medium text-foreground">Vistos</h4>
          {!readOnly && (
            <Button size="sm" variant="outline" className="h-8">
              <Plus className="h-3.5 w-3.5 mr-1.5" />
              Adicionar
            </Button>
          )}
        </div>
        
        <div className="space-y-2">
          {vistos.length === 0 ? (
            <div className="text-center py-4 text-muted-foreground">Nenhum visto cadastrado</div>
          ) : (
            vistos.map((visto) => (
              <div
                key={visto.id}
                className="flex items-center justify-between rounded-lg border bg-card p-3"
              >
                <div className="flex items-center gap-3">
                  <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-950">
                    <FileText className="h-5 w-5 text-violet-600 dark:text-violet-400" />
                  </div>
                  <div>
                    <p className="font-medium text-base text-foreground">{visto.descricaoPais}</p>
                    <div className="flex items-center gap-2 text-sm text-muted-foreground mt-0.5">
                      <Calendar className="h-3 w-3" />
                      <span>Validade: {formatDate(visto.validade)}</span>
                      {isExpiringSoon(visto.validade) && (
                        <Badge variant="destructive" className="text-xs">
                          Expirando
                        </Badge>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      </div>

      {/* Passaportes */}
      <div className="space-y-3">
        <div className="flex items-center justify-between">
          <h4 className="text-sm font-medium text-foreground">Passaportes</h4>
          {!readOnly && (
            <Button size="sm" variant="outline" className="h-8">
              <Plus className="h-3.5 w-3.5 mr-1.5" />
              Adicionar
            </Button>
          )}
        </div>
        
        <div className="space-y-2">
          {passaportes.length === 0 ? (
            <div className="text-center py-4 text-muted-foreground">Nenhum passaporte cadastrado</div>
          ) : (
            passaportes.map((passaporte) => (
              <div
                key={passaporte.id}
                className="flex items-center justify-between rounded-lg border bg-card p-3"
              >
                <div className="flex items-center gap-3">
                  <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-950">
                    <FileText className="h-5 w-5 text-violet-600 dark:text-violet-400" />
                  </div>
                  <div>
                    <p className="font-medium text-base text-foreground">{passaporte.descricaoNacionalidade}</p>
                    <div className="flex items-center gap-2 text-sm text-muted-foreground mt-0.5">
                      <Calendar className="h-3 w-3" />
                      <span>Validade: {formatDate(passaporte.validade)}</span>
                      {isExpiringSoon(passaporte.validade) && (
                        <Badge variant="destructive" className="text-xs">
                          Expirando
                        </Badge>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  );
};
