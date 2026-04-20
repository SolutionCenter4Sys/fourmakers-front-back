import { useEffect, useState } from 'react';
import { Sparkles } from '@/components/ui/system-icons';
import { Dialog, DialogContent } from '@/components/ui/dialog';

const messages = [
  'Analisando o prompt...',
  'Identificando competências técnicas...',
  'Mapeando soft skills...',
  'Definindo modelo de trabalho...',
  'Estruturando responsabilidades...',
  'Finalizando perfil...',
];

interface AILoadingModalProps {
  open: boolean;
}

export const AILoadingModal = ({ open }: AILoadingModalProps) => {
  const [messageIndex, setMessageIndex] = useState(0);

  useEffect(() => {
    if (!open) {
      setMessageIndex(0);
      return;
    }
    const interval = setInterval(() => {
      setMessageIndex(prev => (prev + 1) % messages.length);
    }, 2000);
    return () => clearInterval(interval);
  }, [open]);

  return (
    <Dialog open={open} onOpenChange={() => {}}>
      <DialogContent className="sm:max-w-sm [&>button]:hidden">
        <div className="flex flex-col items-center justify-center py-8 space-y-6 text-center">
          <div className="relative">
            <div className="w-20 h-20 rounded-full bg-brand-gradient flex items-center justify-center animate-pulse">
              <Sparkles className="h-10 w-10 text-primary-foreground" />
            </div>
            <div
              className="absolute inset-0 w-20 h-20 rounded-full border-4 border-primary/30 border-t-primary animate-spin"
              style={{ animationDuration: '1.6s' }}
            />
          </div>

          <div className="space-y-2">
            <h3 className="text-lg font-semibold text-foreground">Gerando com IA</h3>
            <p className="text-sm text-muted-foreground animate-in fade-in-0" key={messageIndex}>
              {messages[messageIndex]}
            </p>
          </div>

          <div className="flex gap-2">
            {messages.map((_, i) => (
              <div
                key={i}
                className={`w-2 h-2 rounded-full transition-all duration-300 ${i <= messageIndex ? 'bg-primary' : 'bg-muted'}`}
              />
            ))}
          </div>

          <p className="text-xs text-muted-foreground">Tempo estimado: ~14 segundos</p>
        </div>
      </DialogContent>
    </Dialog>
  );
};
