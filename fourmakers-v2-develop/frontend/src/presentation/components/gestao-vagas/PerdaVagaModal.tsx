import { useState, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import type { MotivoPerdaVagaItem } from '@domain/entities/GestaoVagasCandidatos';
import { ListarMotivosPerdaVagaUseCase } from '@domain/usecases/ListarMotivosPerdaVagaUseCase';
import { GravarPerdaVagaUseCase } from '@domain/usecases/GravarPerdaVagaUseCase';
import { MudarStatusVagaUseCase } from '@domain/usecases/MudarStatusVagaUseCase';
import { toast } from 'sonner';
import { AlertTriangle } from '@/components/ui/system-icons';

interface PerdaVagaModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  codigoVaga: string;
  token: string | null;
  onSuccess?: () => void;
}

export function PerdaVagaModal({
  open,
  onOpenChange,
  codigoVaga,
  token,
  onSuccess,
}: PerdaVagaModalProps) {
  const [saving, setSaving] = useState(false);
  const [motivos, setMotivos] = useState<MotivoPerdaVagaItem[]>([]);
  const [idMotivoPerda, setIdMotivoPerda] = useState<string>('');
  const [comentario, setComentario] = useState('');

  const listarMotivos = container.resolve(ListarMotivosPerdaVagaUseCase);
  const gravarPerda = container.resolve(GravarPerdaVagaUseCase);
  const mudarStatusVaga = container.resolve(MudarStatusVagaUseCase);

  useEffect(() => {
    if (!open || !token) return;
    const load = async () => {
      try {
        const list = await listarMotivos.execute(token);
        setMotivos(list);
      } catch {
        setMotivos([]);
        toast.error('Erro ao carregar motivos de perda.');
      }
    };
    load();
  }, [open, token, listarMotivos]);

  useEffect(() => {
    if (!open) {
      setIdMotivoPerda('');
      setComentario('');
    }
  }, [open]);

  const handleRegistrar = async () => {
    if (!token || !idMotivoPerda.trim()) {
      toast.error('Selecione o motivo da perda.');
      return;
    }
    setSaving(true);
    try {
      const codigoVagaNum = parseInt(codigoVaga, 10);
      if (Number.isNaN(codigoVagaNum)) {
        toast.error('Código da vaga inválido.');
        setSaving(false);
        return;
      }
      const resGravar = await gravarPerda.execute(token, {
        codigoVaga: codigoVagaNum,
        idMotivoPerda: idMotivoPerda.trim(),
        comentario: comentario.trim(),
      });
      if (resGravar?.sucesso === false) {
        toast.error(resGravar?.mensagem ?? 'Erro ao registrar perda.');
        setSaving(false);
        return;
      }
      toast.success(resGravar?.mensagem ?? 'Vaga marcada como perdida com sucesso.');
      const resStatus = await mudarStatusVaga.execute(token, {
        codigoVaga,
        codigoStatus: 11,
        comentarioVaga: '',
      });
      if (resStatus?.sucesso === false) {
        toast.error(resStatus?.mensagem ?? 'Erro ao alterar status.');
        setSaving(false);
        return;
      }
      toast.success(resStatus?.mensagem ?? 'Status alterado com sucesso.');
      onOpenChange(false);
      onSuccess?.();
    } catch (e) {
      toast.error('Erro ao registrar perda da vaga.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md" aria-labelledby="perda-vaga-title">
        <DialogHeader>
          <div className="flex items-start gap-3">
            <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-destructive/10 text-destructive" aria-hidden>
              <AlertTriangle className="h-5 w-5" />
            </span>
            <div className="space-y-1.5">
              <DialogTitle id="perda-vaga-title">Informar o motivo da perda da vaga</DialogTitle>
              <DialogDescription>Conta pra gente qual foi o motivo.</DialogDescription>
            </div>
          </div>
        </DialogHeader>
        <div className="grid gap-4 py-2">
          <div className="space-y-2">
            <Label htmlFor="motivo-perda">Qual é o motivo da perda?</Label>
            <Select value={idMotivoPerda} onValueChange={setIdMotivoPerda}>
              <SelectTrigger id="motivo-perda">
                <SelectValue placeholder="Selecione os motivos" />
              </SelectTrigger>
              <SelectContent>
                {motivos.map((m) => (
                  <SelectItem key={m.id} value={m.id}>
                    {m.descricao}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div className="space-y-2">
            <Label htmlFor="comentario-perda">Deseja inserir um comentário?</Label>
            <Textarea
              id="comentario-perda"
              placeholder="Escreva o comentário aqui..."
              value={comentario}
              onChange={(e) => setComentario(e.target.value)}
              rows={4}
              className="resize-none"
            />
          </div>
        </div>
        <DialogFooter>
          <Button onClick={handleRegistrar} disabled={saving}>
            {saving ? 'Registrando…' : 'Registrar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
