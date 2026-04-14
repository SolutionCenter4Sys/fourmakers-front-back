import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { XCircle } from "@/components/ui/system-icons";

interface ReprovarNotasFiscaisModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  quantidadeSelecionadas: number;
  onConfirm: (justificativa: string) => Promise<void>;
  processando?: boolean;
  erro?: string | null;
}

export const ReprovarNotasFiscaisModal = ({
  open,
  onOpenChange,
  quantidadeSelecionadas,
  onConfirm,
  processando = false,
  erro = null,
}: ReprovarNotasFiscaisModalProps) => {
  const [justificativa, setJustificativa] = useState("");

  // Resetar justificativa quando o modal fechar
  useEffect(() => {
    if (!open) {
      setJustificativa("");
    }
  }, [open]);

  const handleConfirm = async () => {
    if (!justificativa.trim()) return;
    await onConfirm(justificativa.trim());
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-3 bg-destructive/10 rounded-lg">
              <XCircle className="h-8 w-8 text-destructive" />
            </div>
            <div>
              <DialogTitle className="text-2xl font-bold">
                Reprovar ({quantidadeSelecionadas}) nota(s) selecionada(s)
              </DialogTitle>
              <DialogDescription className="mt-1">
                Ao reprovar, esta ação não poderá ser desfeita.
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="space-y-2">
            <Label htmlFor="justificativa-reprovar" className="text-base font-medium">
              Justificativa da Reprovação<span className="text-destructive">*</span>
            </Label>
            <Textarea
              id="justificativa-reprovar"
              data-testid="modal-reprovar-justificativa"
              value={justificativa}
              onChange={(e) => setJustificativa(e.target.value)}
              placeholder="Descreva o motivo da reprovação"
              className="min-h-[120px] resize-none"
            />
          </div>
          {erro && (
            <p className="text-sm text-destructive">{erro}</p>
          )}
        </div>

        <DialogFooter>
          <Button data-testid="modal-reprovar-cancelar" variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button
            data-testid="modal-reprovar-confirmar"
            onClick={handleConfirm}
            disabled={!justificativa.trim() || processando}
          >
            {processando ? "Reprovando..." : "Confirmar Reprovação"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

