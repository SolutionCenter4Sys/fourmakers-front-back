import { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Download, Loader2 } from '@/components/ui/system-icons';
import { useAppSelector } from '@app/store/hooks';
import { useToast } from '@/hooks/use-toast';
import { container } from '@core/di/container';
import { ExportarRelatorioAlocacoesUseCase } from '@domain/usecases/ExportarRelatorioAlocacoesUseCase';
import type { ListarAlocacoesPayload, ExportarRelatorioAlocacoesPayload } from '@domain/entities/MapaAlocacao';
import { toast as sonnerToast } from 'sonner';

interface ExportarRelatorioModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  filtrosAtuais?: ListarAlocacoesPayload;
}

export const ExportarRelatorioModal = ({
  open,
  onOpenChange,
  filtrosAtuais,
}: ExportarRelatorioModalProps) => {
  const { token } = useAppSelector((state) => state.auth);
  const { toast } = useToast();
  const [isExporting, setIsExporting] = useState(false);
  const [tipoExportacao, setTipoExportacao] = useState<'filtrados' | 'todos'>('filtrados');

  const handleExportar = async () => {
    if (!token) {
      toast({
        title: 'Erro',
        description: 'Token de autenticação não encontrado',
        variant: 'destructive',
      });
      return;
    }

    setIsExporting(true);

    try {
      const useCase = container.resolve(ExportarRelatorioAlocacoesUseCase);
      
      // Se for "filtrados", usar os filtros atuais; se for "todos", enviar payload vazio
      const payload: ListarAlocacoesPayload = tipoExportacao === 'filtrados' && filtrosAtuais
        ? filtrosAtuais
        : {} as ListarAlocacoesPayload; // Payload vazio para exportar todos os dados

      // Cast para ExportarRelatorioAlocacoesPayload (as interfaces são idênticas)
      const response = await useCase.execute(token, payload as ExportarRelatorioAlocacoesPayload);

      // Obter nome do arquivo do header Content-Disposition
      const contentDisposition = response.headers.get('Content-Disposition') || '';
      let fileName = 'alocacoes_filtrado.xlsx';
      
      if (tipoExportacao === 'todos') {
        fileName = 'alocacoes_todas.xlsx';
      }

      if (contentDisposition) {
        // Tentar primeiro o formato filename*=UTF-8'' (RFC 5987)
        const filenameStarMatch = contentDisposition.match(/filename\*=UTF-8''([^;]+?)(?:;|$)/i);
        if (filenameStarMatch && filenameStarMatch[1]) {
          try {
            fileName = decodeURIComponent(filenameStarMatch[1].trim());
          } catch {
            fileName = filenameStarMatch[1].trim();
          }
        } else {
          // Fallback para o formato filename= simples
          const filenameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]+?)(?:;|$)/i);
          if (filenameMatch && filenameMatch[1]) {
            fileName = filenameMatch[1].replace(/['"]/g, '').trim();
          }
        }
      }

      // Converter resposta para blob e fazer download
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', fileName);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);

      sonnerToast.success('Relatório exportado com sucesso!');
      onOpenChange(false);
    } catch (error) {
      console.error('Erro ao exportar relatório:', error);
      toast({
        title: 'Erro',
        description: error instanceof Error ? error.message : 'Falha ao exportar relatório',
        variant: 'destructive',
      });
    } finally {
      setIsExporting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-3 bg-primary/10 rounded-lg">
              <Download className="h-6 w-6 text-primary" />
            </div>
            <div>
              <DialogTitle className="text-xl font-semibold">
                Exportar Relatório
              </DialogTitle>
              <DialogDescription className="mt-1">
                Como gostaria de exportar este relatório?
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="space-y-3">
            <label 
              className="flex items-start gap-3 p-3 rounded-lg border cursor-pointer hover:bg-muted/50 transition-colors"
              onClick={() => setTipoExportacao('filtrados')}
            >
              <Checkbox
                checked={tipoExportacao === 'filtrados'}
                onCheckedChange={(checked) => {
                  if (checked) {
                    setTipoExportacao('filtrados');
                  }
                }}
                onClick={(e) => e.stopPropagation()}
                className="mt-1"
              />
              <div className="flex-1" onClick={(e) => e.stopPropagation()}>
                <div className="font-medium text-sm">Apenas os campos filtrados</div>
                <div className="text-xs text-muted-foreground mt-0.5">
                  Exporta apenas os dados que correspondem aos filtros aplicados
                </div>
              </div>
            </label>

            <label 
              className="flex items-start gap-3 p-3 rounded-lg border cursor-pointer hover:bg-muted/50 transition-colors"
              onClick={() => setTipoExportacao('todos')}
            >
              <Checkbox
                checked={tipoExportacao === 'todos'}
                onCheckedChange={(checked) => {
                  if (checked) {
                    setTipoExportacao('todos');
                  }
                }}
                onClick={(e) => e.stopPropagation()}
                className="mt-1"
              />
              <div className="flex-1" onClick={(e) => e.stopPropagation()}>
                <div className="font-medium text-sm">Todas os dados</div>
                <div className="text-xs text-muted-foreground mt-0.5">
                  Exporta todos os dados disponíveis, ignorando os filtros
                </div>
              </div>
            </label>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={isExporting}
          >
            Cancelar
          </Button>
          <Button
            onClick={handleExportar}
            className="gap-2"
            disabled={isExporting}
          >
            {isExporting ? (
              <>
                <Loader2 className="h-4 w-4 animate-spin" />
                Exportando...
              </>
            ) : (
              <>
                <Download className="h-4 w-4" />
                Exportar
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};
