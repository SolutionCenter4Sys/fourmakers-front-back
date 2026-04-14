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
import { ScrollArea } from '@/components/ui/scroll-area';
import type { VagaFilhaItem } from '@domain/entities/GestaoVagasCandidatos';
import { ListarVagasRecrutamentoPorParentEmAndamentoUseCase } from '@domain/usecases/ListarVagasRecrutamentoPorParentEmAndamentoUseCase';
import { Edit } from '@/components/ui/system-icons';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';

interface DuplicarVagaModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  vagaIdParent: string;
  tituloPerfil: string;
  token: string | null;
  onProsseguir: () => void;
  onEditarVaga: (item: VagaFilhaItem) => void;
}

const statusLabelByCod: Record<string, string> = {
  '1': 'Rascunho',
  '2': 'Em foco',
  '10': 'Procurando Candidatos',
  '3': 'Contratação',
  '4': 'Fechada',
};

function getStatusLabel(cod: string | undefined): string {
  if (!cod) return '—';
  return statusLabelByCod[cod] ?? cod;
}

export function DuplicarVagaModal({
  open,
  onOpenChange,
  vagaIdParent,
  tituloPerfil,
  token,
  onProsseguir,
  onEditarVaga,
}: DuplicarVagaModalProps) {
  const [loading, setLoading] = useState(false);
  const [vagasFilhas, setVagasFilhas] = useState<VagaFilhaItem[]>([]);

  const listarVagasParent = container.resolve(ListarVagasRecrutamentoPorParentEmAndamentoUseCase);

  useEffect(() => {
    if (!open || !token || !vagaIdParent) {
      setVagasFilhas([]);
      return;
    }
    let cancelled = false;
    setLoading(true);
    listarVagasParent
      .execute(token, vagaIdParent)
      .then((list) => {
        if (cancelled) return;
        setVagasFilhas(list);
      })
      .catch(() => {
        if (!cancelled) setVagasFilhas([]);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [open, token, vagaIdParent, listarVagasParent]);

  const handleProsseguir = () => {
    onProsseguir();
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg" aria-labelledby="duplicar-vaga-title">
        <DialogHeader>
          <DialogTitle id="duplicar-vaga-title">A vaga será duplicada</DialogTitle>
          <DialogDescription>
            Esta vaga será gerada a partir de {tituloPerfil || '—'}. Antes de prosseguir, verifique se é necessário criar uma nova ou se pode editar vagas em andamento.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-3 text-sm">
          <p>
            Esta vaga será gerada a partir de <strong>{tituloPerfil || '—'}</strong>.
          </p>
          <p className="text-muted-foreground">
            Antes de prosseguir, verifique se realmente é necessário criar uma nova.
          </p>
          <p className="text-muted-foreground">
            Para evitar duplicidades de vagas, você também pode editar as informações, como número
            de posições, ou detalhes complementares, nas vagas em andamento:
          </p>
        </div>
        {loading ? (
          <p className="text-sm text-muted-foreground py-4">Carregando vagas em andamento…</p>
        ) : vagasFilhas.length > 0 ? (
          <ScrollArea className="h-[240px] rounded-md border">
            <ul className="p-2 space-y-2">
              {vagasFilhas.map((item) => (
                <li
                  key={item.id}
                  className="flex items-center justify-between gap-2 rounded-lg border bg-card p-3 text-card-foreground shadow-sm"
                >
                  <div className="min-w-0 flex-1">
                    <p className="font-medium truncate">
                      <span className="text-muted-foreground">Titulo da Vaga:</span>{' '}
                      {item.titulo ?? '—'}
                    </p>
                    <p className="text-xs text-muted-foreground">
                      <span className="text-muted-foreground">Código:</span> {item.codigo ?? '—'}
                      {' · '}
                      <span className="text-muted-foreground">Status da Vaga:</span>{' '}
                      {getStatusLabel(item.statusVagaCod)}
                    </p>
                  </div>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-8 w-8 shrink-0"
                        aria-label="Editar vaga/perfil"
                        onClick={() => onEditarVaga(item)}
                      >
                        <Edit className="h-4 w-4" />
                      </Button>
                    </TooltipTrigger>
                    <TooltipContent>Editar vaga/perfil</TooltipContent>
                  </Tooltip>
                </li>
              ))}
            </ul>
          </ScrollArea>
        ) : (
          <p className="text-sm text-muted-foreground py-2">Nenhuma vaga em andamento encontrada.</p>
        )}
        <p className="text-sm font-medium">
          👉 Deseja continuar com a criação de uma nova vaga?
        </p>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button onClick={handleProsseguir}>Prosseguir</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
