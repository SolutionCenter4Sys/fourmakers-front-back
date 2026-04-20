import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Briefcase, FileText, Lock } from 'lucide-react';
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip';

export type TipoPerfil = 'completo' | 'reduzido';

interface SelecionarTipoPerfilModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSelect: (tipo: TipoPerfil) => void;
  modo: 'criar' | 'editar';
}

export function SelecionarTipoPerfilModal({
  open,
  onOpenChange,
  onSelect,
  modo,
}: SelecionarTipoPerfilModalProps) {
  const handleSelectCompleto = () => {
    onSelect('completo');
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>
            {modo === 'criar' ? 'Criar Perfil Corporativo' : 'Editar Perfil Corporativo'}
          </DialogTitle>
          <DialogDescription>
            Selecione o tipo de perfil que deseja {modo === 'criar' ? 'criar' : 'editar'}.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-3 py-4">
          {/* Opção: Perfil Completo */}
          <Button
            type="button"
            variant="outline"
            className="w-full h-auto p-4 flex flex-col items-start gap-2 hover:bg-accent/10 hover:border-accent transition-all"
            onClick={handleSelectCompleto}
          >
            <div className="flex items-center gap-3 w-full">
              <div className="p-2 rounded-lg bg-primary/10 text-primary">
                <Briefcase className="w-5 h-5" />
              </div>
              <div className="flex-1 text-left">
                <div className="font-semibold text-sm">Perfil Completo</div>
                <div className="text-xs text-muted-foreground mt-0.5">
                  Perfil com todas as informações detalhadas
                </div>
              </div>
            </div>
          </Button>

          {/* Opção: Perfil Reduzido */}
          <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <div className="w-full">
                  <Button
                    type="button"
                    variant="outline"
                    disabled
                    className="w-full h-auto p-4 flex flex-col items-start gap-2 opacity-60 cursor-not-allowed"
                  >
                    <div className="flex items-center gap-3 w-full">
                      <div className="p-2 rounded-lg bg-muted text-muted-foreground">
                        <FileText className="w-5 h-5" />
                      </div>
                      <div className="flex-1 text-left">
                        <div className="font-semibold text-sm flex items-center gap-2">
                          Perfil Reduzido
                          <Lock className="w-3.5 h-3.5" />
                        </div>
                        <div className="text-xs text-muted-foreground mt-0.5">
                          Perfil simplificado para referência rápida
                        </div>
                      </div>
                    </div>
                  </Button>
                </div>
              </TooltipTrigger>
              <TooltipContent>
                <p>Em Breve</p>
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        </div>
      </DialogContent>
    </Dialog>
  );
}
