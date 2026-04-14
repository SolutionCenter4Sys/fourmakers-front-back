import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { CheckCircle, AlertTriangle } from "@/components/ui/system-icons";

interface AprovarNotasFiscaisModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  quantidadeSelecionadas: number;
  quantidadeDivergentes: number;
  onConfirm: () => void;
}

export const AprovarNotasFiscaisModal = ({
  open,
  onOpenChange,
  quantidadeSelecionadas,
  quantidadeDivergentes,
  onConfirm,
}: AprovarNotasFiscaisModalProps) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-3 bg-primary/10 rounded-lg">
              <CheckCircle className="h-8 w-8 text-primary" />
            </div>
            <div>
              <DialogTitle className="text-2xl font-bold">Aprovar</DialogTitle>
              <DialogDescription className="mt-1">
                Você está prestes a aprovar ({quantidadeSelecionadas}) nota(s) selecionada(s).
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        {quantidadeDivergentes > 0 && (
          <div className="space-y-4 py-4">
            <Card className="border-amber-200 bg-amber-50 dark:border-amber-800 dark:bg-amber-950/30">
              <CardContent className="pt-6">
                <div className="flex items-start gap-3">
                  <AlertTriangle className="h-5 w-5 text-amber-600 dark:text-amber-500 flex-shrink-0 mt-0.5" />
                  <div className="space-y-1">
                    <p className="text-sm font-semibold text-amber-800 dark:text-amber-200">
                      {quantidadeDivergentes} NF(s) com divergência
                    </p>
                    <p className="text-sm text-amber-700 dark:text-amber-300">
                      Existe(m) divergência entre valor e valor analisado nas nota(s) selecionada(s). Deseja mesmo aprovar?
                    </p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        )}

        <DialogFooter>
          <Button data-testid="modal-aprovar-cancelar" variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button data-testid="modal-aprovar-confirmar" onClick={onConfirm}>
            Confirmar Aprovação
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

