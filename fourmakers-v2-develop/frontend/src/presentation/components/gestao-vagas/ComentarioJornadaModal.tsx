import { useEffect, useMemo, useState } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { MessageCircle, Save } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import { cn } from '@/lib/utils';
import { toast } from 'sonner';
import { InserirComentarioCandidaturaUseCase } from '@domain/usecases/InserirComentarioCandidaturaUseCase';

const MAX_COMENTARIO = 300;

interface ComentarioJornadaModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  candidaturaId: string;
  nomeCandidato?: string;
}

export function ComentarioJornadaModal({
  open,
  onOpenChange,
  token,
  candidaturaId,
  nomeCandidato,
}: ComentarioJornadaModalProps) {
  const inserirComentarioCandidaturaUseCase = container.resolve(InserirComentarioCandidaturaUseCase);
  const [comentario, setComentario] = useState('');
  const [comentarioError, setComentarioError] = useState(false);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (!open) {
      setComentario('');
      setComentarioError(false);
      setSaving(false);
    }
  }, [open]);

  const titulo = useMemo(
    () => `Antes de terminar insira alguns dados${nomeCandidato ? ` (${nomeCandidato})` : ''}`,
    [nomeCandidato],
  );

  const handleSalvar = async () => {
    const comentarioTrim = comentario.trim();
    if (!comentarioTrim) {
      setComentarioError(true);
      return;
    }
    if (!token) {
      toast.error('Sessão inválida. Faça login novamente.');
      return;
    }
    if (!candidaturaId?.trim()) {
      toast.error('Candidatura inválida para inserir comentário.');
      return;
    }

    setSaving(true);
    try {
      const res = await inserirComentarioCandidaturaUseCase.execute(token, {
        comentario: comentarioTrim,
        candidaturaId: candidaturaId.trim(),
      });
      if (!res?.sucesso) {
        toast.error(res?.mensagem ?? 'Não foi possível salvar o comentário.');
        return;
      }
      toast.success(res?.mensagem ?? 'Comentário incluído com sucesso.');
      onOpenChange(false);
    } catch (e) {
      console.error('Erro ao inserir comentário da jornada', e);
      toast.error(e instanceof Error ? e.message : 'Erro ao salvar comentário.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md p-0 gap-0" aria-labelledby="comentario-jornada-title" aria-describedby="comentario-jornada-desc">
        <DialogHeader className="flex flex-row items-start gap-4 px-6 py-4 border-b shrink-0">
          <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-lg bg-muted">
            <MessageCircle className="h-7 w-7 text-primary" />
          </div>
          <div className="flex-1 min-w-0">
            <DialogTitle id="comentario-jornada-title" className="text-xl">
              {titulo}
            </DialogTitle>
            <DialogDescription id="comentario-jornada-desc">
              Insira detalhes e informações do candidato abaixo.
            </DialogDescription>
          </div>
        </DialogHeader>

        <div className="px-6 py-4 space-y-4">
          <Textarea
            value={comentario}
            onChange={(e) => {
              const text = e.target.value.slice(0, MAX_COMENTARIO);
              setComentario(text);
              if (comentarioError && text.trim()) setComentarioError(false);
            }}
            placeholder="Escreva o comentário aqui..."
            rows={4}
            className={cn(
              'resize-none bg-muted/30',
              comentarioError && 'border-destructive focus-visible:ring-destructive',
            )}
            aria-invalid={comentarioError}
            aria-describedby={comentarioError ? 'comentario-jornada-erro' : undefined}
            disabled={saving}
          />
          <div className="flex items-center justify-between text-sm">
            {comentarioError ? (
              <p id="comentario-jornada-erro" className="text-destructive">
                Por favor, justifique a movimentação.
              </p>
            ) : (
              <span className="text-muted-foreground"> </span>
            )}
            <span className={cn('text-muted-foreground', comentario.length >= MAX_COMENTARIO && 'text-warning')}>
              {comentario.length}/{MAX_COMENTARIO}
            </span>
          </div>
        </div>

        <div className="px-6 pb-4 flex justify-end">
          <Button onClick={handleSalvar} disabled={saving}>
            {saving ? <Spinner size={16} className="mr-2" /> : <Save className="h-4 w-4 mr-2" />}
            {saving ? 'Salvando...' : 'Salvar'}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
