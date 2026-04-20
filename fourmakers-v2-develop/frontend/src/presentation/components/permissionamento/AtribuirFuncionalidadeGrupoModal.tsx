import { useState, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Spinner } from '@/components/ui/spinner';
import type { FuncionalidadeSistemaItem } from '@domain/entities/FuncionalidadeSistema';
import type { GrupoAcessoComFuncionalidadesItem } from '@domain/entities/GrupoAcesso';
import { ListarFuncionalidadesSistemaUseCase } from '@domain/usecases/ListarFuncionalidadesSistemaUseCase';
import { ListarGruposAcessoUseCase } from '@domain/usecases/ListarGruposAcessoUseCase';
import { AtribuirFuncionalidadeGrupoUseCase } from '@domain/usecases/AtribuirFuncionalidadeGrupoUseCase';
import { toast } from 'sonner';

interface AtribuirFuncionalidadeGrupoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string;
  /** Quando aberto a partir da lista de funcionalidades: funcionalidade já selecionada. */
  initialFuncionalidadeId?: number;
  /** Quando aberto a partir da lista de grupos: grupo já selecionado. */
  initialGrupoId?: number;
  onSuccess?: () => void;
}

/** Ordem: se veio do grupo, grupo em cima; senão funcionalidade em cima. */
export function AtribuirFuncionalidadeGrupoModal({
  open,
  onOpenChange,
  token,
  initialFuncionalidadeId,
  initialGrupoId,
  onSuccess,
}: AtribuirFuncionalidadeGrupoModalProps) {
  const [funcionalidades, setFuncionalidades] = useState<FuncionalidadeSistemaItem[]>([]);
  const [grupos, setGrupos] = useState<GrupoAcessoComFuncionalidadesItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedFuncionalidadeId, setSelectedFuncionalidadeId] = useState<string>('');
  const [selectedGrupoId, setSelectedGrupoId] = useState<string>('');
  const [submitting, setSubmitting] = useState(false);

  const listarFuncionalidadesUseCase = container.resolve(ListarFuncionalidadesSistemaUseCase);
  const listarGruposAcessoUseCase = container.resolve(ListarGruposAcessoUseCase);
  const atribuirFuncionalidadeGrupoUseCase = container.resolve(AtribuirFuncionalidadeGrupoUseCase);

  const fromGrupo = initialGrupoId != null;

  useEffect(() => {
    if (!open || !token) return;
    setLoading(true);
    setSelectedFuncionalidadeId(initialFuncionalidadeId != null ? String(initialFuncionalidadeId) : '');
    setSelectedGrupoId(initialGrupoId != null ? String(initialGrupoId) : '');
    Promise.all([
      listarFuncionalidadesUseCase.execute(token),
      listarGruposAcessoUseCase.execute(token),
    ])
      .then(([resF, resG]) => {
        setFuncionalidades(resF.retorno ?? []);
        setGrupos(resG.retorno ?? []);
        if (initialFuncionalidadeId != null) setSelectedFuncionalidadeId(String(initialFuncionalidadeId));
        if (initialGrupoId != null) setSelectedGrupoId(String(initialGrupoId));
      })
      .catch(() => {
        setFuncionalidades([]);
        setGrupos([]);
      })
      .finally(() => setLoading(false));
  }, [open, token, initialFuncionalidadeId, initialGrupoId, listarFuncionalidadesUseCase, listarGruposAcessoUseCase]);

  const handleAtribuir = async () => {
    const grupoId = selectedGrupoId ? Number(selectedGrupoId) : NaN;
    const funcId = selectedFuncionalidadeId ? Number(selectedFuncionalidadeId) : NaN;
    if (!Number.isFinite(grupoId) || !Number.isFinite(funcId)) {
      toast.error('Selecione a funcionalidade e o grupo.');
      return;
    }
    setSubmitting(true);
    try {
      const res = await atribuirFuncionalidadeGrupoUseCase.execute(token, grupoId, funcId);
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Funcionalidade associada ao grupo de acesso com sucesso.');
        onSuccess?.();
      } else {
        toast.error(res.mensagem ?? 'Erro ao atribuir.');
      }
    } catch (err: unknown) {
      const msg =
        err && typeof err === 'object' && err !== null && 'mensagem' in err
          ? String((err as { mensagem?: string }).mensagem)
          : err instanceof Error
            ? err.message
            : 'Erro ao atribuir funcionalidade ao grupo.';
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  };

  const grupoSelector = (
    <div className="space-y-2">
      <Label>Grupo de acesso</Label>
      <Select value={selectedGrupoId} onValueChange={setSelectedGrupoId} disabled={loading}>
        <SelectTrigger>
          <SelectValue placeholder="Selecione o grupo" />
        </SelectTrigger>
        <SelectContent>
          {grupos.map((g) => (
            <SelectItem key={g.id} value={String(g.id)}>
              {g.id} - {g.descricao}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );

  const funcionalidadeSelector = (
    <div className="space-y-2">
      <Label>Funcionalidade do sistema</Label>
      <Select value={selectedFuncionalidadeId} onValueChange={setSelectedFuncionalidadeId} disabled={loading}>
        <SelectTrigger>
          <SelectValue placeholder="Selecione a funcionalidade" />
        </SelectTrigger>
        <SelectContent>
          {funcionalidades.map((f) => (
            <SelectItem key={f.id} value={String(f.id)}>
              {f.id} - {f.descricao}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[420px]">
        <DialogHeader>
          <DialogTitle>Atribuir funcionalidade a grupo</DialogTitle>
          <DialogDescription>
            Selecione a funcionalidade e o grupo de acesso para vincular.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-4 py-4">
          {loading ? (
            <div className="flex items-center justify-center py-8 text-muted-foreground">
              <Spinner className="mr-2" size={24} />
              Carregando...
            </div>
          ) : (
            <>
              {fromGrupo ? grupoSelector : funcionalidadeSelector}
              {fromGrupo ? funcionalidadeSelector : grupoSelector}
            </>
          )}
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={submitting}>
            Fechar
          </Button>
          <Button onClick={handleAtribuir} disabled={loading || submitting}>
            {submitting ? (
              <>
                <Spinner className="mr-2" size={16} />
                Atribuindo...
              </>
            ) : (
              'Atribuir a Grupo'
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
