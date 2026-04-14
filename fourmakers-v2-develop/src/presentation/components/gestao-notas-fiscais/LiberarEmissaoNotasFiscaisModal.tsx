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
import { Switch } from "@/components/ui/switch";
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
import { MonthPicker } from "@/components/ui/month-picker";
import { CalendarIcon } from "@/components/ui/system-icons";
import { cn } from "@/lib/utils";
import type { Unidade } from "@domain/entities/NotaFiscalGestao";

interface LiberarEmissaoNotasFiscaisModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  unidades: Unidade[];
  loadingUnidades?: boolean;
  onConfirm: (params: {
    unidadeId: string;
    mes: number;
    ano: number;
    enviarEmail: boolean;
  }) => Promise<void>;
  processando?: boolean;
  erro?: string | null;
}

// Função para formatar mês e ano como "MMMM/yyyy"
const formatarCompetencia = (mes: number, ano: number): string => {
  const meses = [
    "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
    "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
  ];
  return `${meses[mes - 1]}/${ano}`;
};

export const LiberarEmissaoNotasFiscaisModal = ({
  open,
  onOpenChange,
  unidades,
  loadingUnidades = false,
  onConfirm,
  processando = false,
  erro = null,
}: LiberarEmissaoNotasFiscaisModalProps) => {
  const now = new Date();
  const [unidadeId, setUnidadeId] = useState("");
  const [mes, setMes] = useState<number>(now.getMonth() + 1);
  const [ano, setAno] = useState<number>(now.getFullYear());
  const [enviarEmail, setEnviarEmail] = useState(false);

  // Resetar valores quando o modal abrir/fechar
  useEffect(() => {
    if (open) {
      // Inicializar com valores padrão ao abrir
      const currentDate = new Date();
      setMes(currentDate.getMonth() + 1);
      setAno(currentDate.getFullYear());
      setEnviarEmail(false);
    } else {
      // Resetar todos os valores ao fechar
      const currentDate = new Date();
      setUnidadeId("");
      setMes(currentDate.getMonth() + 1);
      setAno(currentDate.getFullYear());
      setEnviarEmail(false);
    }
  }, [open]);

  const handleConfirm = async () => {
    if (!unidadeId || !mes || !ano) return;
    await onConfirm({
      unidadeId,
      mes,
      ano,
      enviarEmail,
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold">
            Liberação para Emissão de Notas Fiscais
          </DialogTitle>
          <DialogDescription className="mt-1">
            Deseja liberar a emissão das Notas Fiscais pelos prestadores para a Unidade selecionada?
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-4">
          {/* Unidades */}
          <div className="space-y-2">
            <Label>Unidades</Label>
            <Select value={unidadeId} onValueChange={setUnidadeId} disabled={loadingUnidades}>
              <SelectTrigger data-testid="modal-liberar-unidade-trigger">
                <SelectValue placeholder={loadingUnidades ? "Carregando..." : "Selecione"} />
              </SelectTrigger>
              <SelectContent>
                {unidades.length > 0 ? (
                  unidades.map((u) => (
                    <SelectItem key={u.id} value={u.id} data-testid={`modal-liberar-unidade-option-${u.id}`}>
                      {u.descricao}
                    </SelectItem>
                  ))
                ) : (
                  !loadingUnidades && (
                    <div className="px-2 py-1.5 text-sm text-muted-foreground">
                      Nenhuma unidade encontrada
                    </div>
                  )
                )}
              </SelectContent>
            </Select>
          </div>

          {/* Vigência */}
          <div className="space-y-2">
            <Label>Vigência</Label>
            <Popover>
              <PopoverTrigger asChild>
                <Button
                  data-testid="modal-liberar-vigencia-trigger"
                  variant="outline"
                  className={cn(
                    "w-full justify-start text-left font-normal",
                    !mes && !ano && "text-muted-foreground"
                  )}
                >
                  <CalendarIcon className="mr-2 h-4 w-4" />
                  {mes && ano ? formatarCompetencia(mes, ano) : "Selecione"}
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

          {/* Switch Notificação por Email */}
          <div className="flex items-center justify-between space-x-2 py-2">
            <Label htmlFor="notificacao-email" className="text-sm font-normal cursor-pointer">
              Deseja enviar notificação por e-mail aos prestadores solicitando a emissão das Notas Fiscais?
            </Label>
            <Switch
              id="notificacao-email"
              data-testid="modal-liberar-enviar-email"
              checked={enviarEmail}
              onCheckedChange={setEnviarEmail}
            />
          </div>
        </div>

        {erro && (
          <div className="px-4 py-2 bg-destructive/10 text-destructive text-sm rounded-md">
            {erro}
          </div>
        )}
        <DialogFooter>
          <Button
            data-testid="modal-liberar-cancelar"
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={processando}
          >
            Cancelar
          </Button>
          <Button
            data-testid="modal-liberar-confirmar"
            onClick={handleConfirm}
            disabled={!unidadeId || !mes || !ano || processando}
          >
            {processando ? "Confirmando..." : "Confirmar"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

