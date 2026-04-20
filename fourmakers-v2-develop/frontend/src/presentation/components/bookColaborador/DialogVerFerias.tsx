import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Calendar } from "@/components/ui/system-icons";
import type { FeriasInfo } from "@shared/types/bookColaborador";
import { formatData } from "@shared/utils/bookColaboradorFormatters";

interface DialogVerFeriasProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  ferias: FeriasInfo | null;
  colaboradorNome: string;
}

export function DialogVerFerias({
  open,
  onOpenChange,
  ferias,
  colaboradorNome,
}: DialogVerFeriasProps) {
  if (!ferias) {
    return (
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>Férias de {colaboradorNome}</DialogTitle>
            <DialogDescription>
              Não há informações de férias disponíveis.
            </DialogDescription>
          </DialogHeader>
          <div className="flex justify-end pt-4">
            <Button onClick={() => onOpenChange(false)}>Fechar</Button>
          </div>
        </DialogContent>
      </Dialog>
    );
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>Férias de {colaboradorNome}</DialogTitle>
          <DialogDescription>
            Informações sobre o saldo e período de férias.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <Card>
            <CardContent className="p-4">
              <div className="flex items-center gap-3 mb-4">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <Calendar className="h-5 w-5 text-blue-600" />
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Saldo de Férias</p>
                  <p className="text-2xl font-bold">{ferias.saldo} dias</p>
                </div>
              </div>
            </CardContent>
          </Card>

          <div className="space-y-2">
            <div className="flex justify-between">
              <span className="text-sm text-muted-foreground">Período:</span>
              <span className="text-sm font-medium">{ferias.periodo}</span>
            </div>
            {ferias.proximasFerias && (
              <div className="flex justify-between">
                <span className="text-sm text-muted-foreground">Próximas Férias:</span>
                <span className="text-sm font-medium">
                  {formatData(ferias.proximasFerias)}
                </span>
              </div>
            )}
          </div>

          <div className="flex justify-end pt-4">
            <Button onClick={() => onOpenChange(false)}>Fechar</Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}

