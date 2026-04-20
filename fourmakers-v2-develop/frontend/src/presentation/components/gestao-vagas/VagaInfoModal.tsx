import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Separator } from '@/components/ui/separator';
import { Info } from '@/components/ui/system-icons';
import type { VagaInfoModel } from '@presentation/hooks/recrutamento'

interface VagaInfoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  model: VagaInfoModel | null;
}

export const VagaInfoModal = ({ open, onOpenChange, model }: VagaInfoModalProps) => {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl">
        <DialogHeader className="flex-row items-center justify-between space-y-0">
          <div className="flex items-center gap-4">
            <div className="h-12 w-12 rounded-xl bg-muted flex items-center justify-center">
              <Info className="h-7 w-7" />
            </div>
            <DialogTitle className="text-3xl">Dados sobre a vaga</DialogTitle>
          </div>
        </DialogHeader>

        {model && (
          <div className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-10 gap-y-3 text-lg">
              <div className="space-y-3">
                {model.left.map((item) => (
                  <div key={item.label}>
                    <span className="text-muted-foreground">{item.label}:</span>{' '}
                    <span>{item.value}</span>
                  </div>
                ))}
              </div>

              <div className="space-y-3">
                {model.right.map((item) => (
                  <div key={item.label}>
                    <span className="text-muted-foreground">{item.label}:</span>{' '}
                    <span>{item.value}</span>
                  </div>
                ))}
              </div>
            </div>

            <Separator />

            <ScrollArea className="h-[320px] rounded-xl bg-muted/40 p-6">
              <div className="whitespace-pre-wrap text-base leading-relaxed text-foreground">
                {model.descricao == null ||
                model.descricao === '' ||
                String(model.descricao).trim().toLowerCase() === 'null'
                  ? 'Sem descrição informada.'
                  : model.descricao}
              </div>
            </ScrollArea>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
};

