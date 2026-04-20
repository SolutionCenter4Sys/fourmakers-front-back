import { useState, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Checkbox } from '@/components/ui/checkbox';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Briefcase, CheckCircle2, ChevronDown } from '@/components/ui/system-icons';
import type { OpcaoContatoItem } from '@domain/entities/GestaoVagasCandidatos';
import { ListarOpcoesContatoUseCase } from '@domain/usecases/ListarOpcoesContatoUseCase';
import { CandidatarOutraPessoaUseCase } from '@domain/usecases/CandidatarOutraPessoaUseCase';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';

export interface InscricaoPreferenciasModalParams {
  vagaId: string | number;
  vagaTitle: string;
  vagaRaw?: Record<string, unknown>;
}

interface InscricaoPreferenciasModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  /** Código numérico da vaga (ex.: 466) para a API CandidatarOutraPessoa. */
  codigoVaga: number;
  vagaId: string | number;
  vagaTitle: string;
  vagaRaw?: Record<string, unknown>;
  codigoColaborador: string;
  /** Chamado ao clicar em "Ver candidaturas" após sucesso. Recebe params para navegação. */
  onVerCandidaturas?: (params: InscricaoPreferenciasModalParams) => void;
  /** Chamado após inscrição concluída com sucesso (para refresh da lista). */
  onInscricaoConcluida?: () => void;
}

export function InscricaoPreferenciasModal({
  open,
  onOpenChange,
  token,
  codigoVaga,
  vagaId,
  vagaTitle,
  vagaRaw,
  codigoColaborador,
  onVerCandidaturas,
  onInscricaoConcluida,
}: InscricaoPreferenciasModalProps) {
  const [opcoes, setOpcoes] = useState<OpcaoContatoItem[]>([]);
  const [loadingOpcoes, setLoadingOpcoes] = useState(false);
  const [selectedIds, setSelectedIds] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);
  const [step, setStep] = useState<'preferencias' | 'sucesso'>('preferencias');
  const [pendingClose, setPendingClose] = useState(false);
  const [popoverOpen, setPopoverOpen] = useState(false);

  const listarOpcoesContato = container.resolve(ListarOpcoesContatoUseCase);
  const candidatarOutraPessoa = container.resolve(CandidatarOutraPessoaUseCase);

  useEffect(() => {
    if (!open || !token) return;
    setStep('preferencias');
    setSelectedIds([]);
    setLoadingOpcoes(true);
    listarOpcoesContato
      .execute(token)
      .then((list) => setOpcoes(Array.isArray(list) ? list : []))
      .catch((e) => {
        console.error('Erro ao listar opções de contato', e);
        toast.error('Não foi possível carregar as opções de contato.');
        setOpcoes([]);
      })
      .finally(() => setLoadingOpcoes(false));
  }, [open, token, listarOpcoesContato]);

  const hasSelection = selectedIds.length > 0;

  const handleClose = (value: boolean) => {
    if (!value) {
      if (step === 'preferencias') {
        setPendingClose(true);
        return;
      }
      onOpenChange(false);
    } else {
      onOpenChange(value);
    }
  };

  const handleConfirmClose = () => {
    setPendingClose(false);
    onOpenChange(false);
  };

  const handleCancelClose = () => {
    setPendingClose(false);
  };

  const toggleOpcao = (id: string) => {
    setSelectedIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  const handleConfirmarInscricao = async () => {
    if (!token || !hasSelection) return;
    setSubmitting(true);
    try {
      await candidatarOutraPessoa.execute(token, {
        codigoVaga,
        codigoColaborador,
        opcoesContatoIds: selectedIds,
      });
      setStep('sucesso');
      onInscricaoConcluida?.();
    } catch (e) {
      console.error('Erro ao realizar inscrição', e);
      toast.error(
        e instanceof Error ? e.message : 'Não foi possível realizar a inscrição. Tente novamente.'
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleVerCandidaturas = () => {
    onVerCandidaturas?.({ vagaId, vagaTitle, vagaRaw });
    onOpenChange(false);
  };

  const selectedLabels = selectedIds
    .map((id) => opcoes.find((o) => o.id === id)?.descricao)
    .filter(Boolean) as string[];

  return (
    <>
      <AlertDialog open={pendingClose} onOpenChange={(o) => !o && handleCancelClose()}>
        <AlertDialogContent>
          <AlertDialogTitle>Fechar sem inscrever?</AlertDialogTitle>
          <AlertDialogDescription>
            Deseja fechar sem inscrever o talento na vaga? As preferências selecionadas serão descartadas.
          </AlertDialogDescription>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={handleCancelClose}>Não</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleConfirmClose}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              Sim, fechar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <Dialog open={open} onOpenChange={handleClose}>
        <DialogContent className="max-w-lg">
          {step === 'sucesso' ? (
            <>
              <div className="flex flex-col items-center justify-center py-6 space-y-4">
                <div className="flex h-16 w-16 items-center justify-center rounded-full bg-success/15">
                  <CheckCircle2 className="h-10 w-10 text-success animate-in zoom-in duration-300" />
                </div>
                <p className="text-lg font-medium text-center">Inscrição realizada com sucesso</p>
                <Button onClick={handleVerCandidaturas} className="w-full sm:w-auto">
                  Ver candidaturas
                </Button>
              </div>
            </>
          ) : (
            <>
              <DialogHeader className="flex-row items-center gap-3 space-y-0 pb-2">
                <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10">
                  <Briefcase className="h-7 w-7 text-primary" />
                </div>
                <div className="flex-1">
                  <DialogTitle className="text-xl">Inscrição - {vagaTitle}</DialogTitle>
                  <p className="mt-1 text-sm text-muted-foreground">
                    Confirme os dados abaixo para realizar a inscrição na vaga.
                  </p>
                </div>
              </DialogHeader>

              <div className="space-y-4 pt-2">
                <div className="space-y-2">
                  <Label>
                    Preferência de contato <span className="text-destructive">*</span>
                  </Label>
                  <Popover open={popoverOpen} onOpenChange={setPopoverOpen}>
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        role="combobox"
                        className={cn(
                          'w-full justify-between font-normal h-auto min-h-10 py-2',
                          !selectedLabels.length && 'text-muted-foreground'
                        )}
                      >
                        <span className="truncate">
                          {selectedLabels.length > 0
                            ? selectedLabels.join(', ')
                            : 'Selecione uma ou mais opções'}
                        </span>
                        <ChevronDown className="h-4 w-4 shrink-0 opacity-50" />
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-[var(--radix-popover-trigger-width)] p-0" align="start">
                      {loadingOpcoes ? (
                        <div className="p-4 text-sm text-muted-foreground text-center">
                          Carregando opções...
                        </div>
                      ) : opcoes.length === 0 ? (
                        <div className="p-4 text-sm text-muted-foreground text-center">
                          Nenhuma opção disponível.
                        </div>
                      ) : (
                        <ScrollArea className="max-h-60">
                          <div className="p-1">
                            {opcoes.map((item) => (
                              <label
                                key={item.id}
                                className={cn(
                                  'flex items-center gap-2 rounded-md px-2 py-2 text-sm cursor-pointer hover:bg-accent'
                                )}
                              >
                                <Checkbox
                                  checked={selectedIds.includes(item.id)}
                                  onCheckedChange={() => toggleOpcao(item.id)}
                                />
                                <span>{item.descricao}</span>
                              </label>
                            ))}
                          </div>
                        </ScrollArea>
                      )}
                    </PopoverContent>
                  </Popover>
                </div>

                <div className="flex justify-end pt-2">
                  <Button
                    onClick={handleConfirmarInscricao}
                    disabled={!hasSelection || loadingOpcoes || submitting}
                    className={cn(submitting && 'opacity-70')}
                  >
                    {submitting ? 'Enviando...' : 'Confirmar e realizar inscrição'}
                  </Button>
                </div>
              </div>
            </>
          )}
        </DialogContent>
      </Dialog>
    </>
  );
}
