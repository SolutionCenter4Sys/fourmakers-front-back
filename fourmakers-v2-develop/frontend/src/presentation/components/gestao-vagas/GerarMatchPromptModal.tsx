import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Sparkles } from '@/components/ui/system-icons';
import { GerarMatchPromptContent } from './GerarMatchPromptContent';

interface GerarMatchPromptModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  onInscrever: (codigoColaborador: string) => void;
  codigosInscritosNaVaga?: Set<string>;
  vagaId?: string;
  vagaTitle?: string;
  vagaRaw?: unknown;
}

export function GerarMatchPromptModal({
  open,
  onOpenChange,
  token,
  onInscrever,
  codigosInscritosNaVaga

}: GerarMatchPromptModalProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent
        className="max-w-4xl max-h-[90vh] flex flex-col p-4 sm:p-6"
        aria-labelledby="gerar-match-modal-title"
        aria-describedby="gerar-match-modal-desc"
      >
        <DialogHeader className="flex-row items-center justify-between space-y-0 gap-4">
          <div className="flex items-center gap-3">
            <div
              className="h-10 w-10 sm:h-12 sm:w-12 rounded-xl bg-primary/10 flex items-center justify-center shrink-0"
              aria-hidden
            >
              <Sparkles className="h-6 w-6 sm:h-7 sm:w-7 text-primary" />
            </div>
            <div>
              <DialogTitle id="gerar-match-modal-title" className="text-lg sm:text-2xl">
                Gerar Match a partir de Prompt
              </DialogTitle>
              <DialogDescription id="gerar-match-modal-desc">
                Descreva a vaga ou o perfil desejado e a IA buscará profissionais com cálculo de match.
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <GerarMatchPromptContent
          token={token}
          onInscrever={onInscrever}
          codigosInscritosNaVaga={codigosInscritosNaVaga}
        />
      </DialogContent>
    </Dialog>
  );
}
