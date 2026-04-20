import { useState, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Checkbox } from '@/components/ui/checkbox';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Briefcase, ChevronDown } from '@/components/ui/system-icons';
import type { OpcaoContatoItem } from '@domain/entities/GestaoVagasCandidatos';
import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile';
import { ListarOpcoesContatoUseCase } from '@domain/usecases/ListarOpcoesContatoUseCase';
import { AlterarFormularioColaboradorUseCase } from '@domain/usecases/AlterarFormularioColaboradorUseCase';
import { CandidatarSeUseCase } from '@domain/usecases/CandidatarSeUseCase';
import { toast } from 'sonner';
import { logUserAction } from '@shared/utils/firebaseAnalytics';
import { cn } from '@/lib/utils';

export interface InscricaoVagaModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  /** Código numérico da vaga (ex.: 533) para a API CandidatarSe. */
  codigoVaga: number;
  vagaTitle: string;
  user: ShowmeUserProfile | null;
  onSuccess?: () => void;
}

function getEmailInicial(user: ShowmeUserProfile | null): string {
  if (!user) return '';
  return (user.colaborador as { email_alternativo?: string } | undefined)?.email_alternativo?.trim?.()
    || user.email?.trim?.()
    || '';
}

function getCelularInicial(user: ShowmeUserProfile | null): string {
  if (!user) return '';
  return user.contatoPrincipal?.trim?.()
    || (user.colaborador as { contatoPrincipal?: string } | undefined)?.contatoPrincipal?.trim?.()
    || '';
}

export function InscricaoVagaModal({
  open,
  onOpenChange,
  token,
  codigoVaga,
  vagaTitle,
  user,
  onSuccess,
}: InscricaoVagaModalProps) {
  const [opcoes, setOpcoes] = useState<OpcaoContatoItem[]>([]);
  const [loadingOpcoes, setLoadingOpcoes] = useState(false);
  const [email, setEmail] = useState('');
  const [celular, setCelular] = useState('');
  const [selectedIds, setSelectedIds] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);
  const [popoverOpen, setPopoverOpen] = useState(false);

  const listarOpcoesContato = container.resolve(ListarOpcoesContatoUseCase);
  const alterarFormulario = container.resolve(AlterarFormularioColaboradorUseCase);
  const candidatarSe = container.resolve(CandidatarSeUseCase);

  useEffect(() => {
    if (!open) return;
    setEmail(getEmailInicial(user));
    setCelular(getCelularInicial(user));
    setSelectedIds([]);
  }, [open, user]);

  useEffect(() => {
    if (!open || !token) return;
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
  const emailTrim = email.trim();
  const celularTrim = celular.trim();
  const canSubmit = hasSelection && emailTrim.length > 0 && celularTrim.length > 0;

  const toggleOpcao = (id: string) => {
    setSelectedIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  const handleConfirmarInscricao = async () => {
    if (!token || !canSubmit) return;
    setSubmitting(true);
    try {
      await alterarFormulario.execute(token, {
        emailAlternativo: emailTrim,
        celular: celularTrim,
      });
      await candidatarSe.execute(token, {
        codigoVaga,
        opcoesContatoIds: selectedIds.filter((id) => id != null && String(id).trim() !== ''),
        pretencaoSalarial: '',
        modeloTrabalhoId: '',
      });
      logUserAction('VagasDetalhe', 'CandidatarSe', { codigoVaga, opcoesContatoIds: selectedIds.length }, user ?? undefined);
      onOpenChange(false);
      toast.success('Inscrição realizada com sucesso.');
      onSuccess?.();
    } catch (e) {
      const msg = e instanceof Error ? e.message : 'Não foi possível realizar a inscrição. Tente novamente.';
      logUserAction('VagasDetalhe', 'CandidatarSeErro', { codigoVaga, erro: msg }, user ?? undefined);
      onOpenChange(false);
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  };

  const selectedLabels = selectedIds
    .map((id) => opcoes.find((o) => o.id === id)?.descricao)
    .filter(Boolean) as string[];

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader className="flex-row items-center gap-3 space-y-0 pb-2">
          <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10">
            <Briefcase className="h-7 w-7 text-primary" />
          </div>
          <div className="flex-1">
            <DialogTitle className="text-xl">Inscrição - {vagaTitle}</DialogTitle>
            <DialogDescription className="mt-1">
              Confirme os dados abaixo para realizar a sua inscrição na vaga.
            </DialogDescription>
          </div>
        </DialogHeader>

        <div className="space-y-4 pt-2">
          <div className="space-y-2">
            <Label htmlFor="inscricao-email">
              E-mail <span className="text-destructive">*</span>
            </Label>
            <Input
              id="inscricao-email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Seu e-mail"
              className="w-full"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="inscricao-celular">
              Celular <span className="text-destructive">*</span>
            </Label>
            <Input
              id="inscricao-celular"
              type="tel"
              value={celular}
              onChange={(e) => setCelular(e.target.value)}
              placeholder="Seu celular"
              className="w-full"
            />
          </div>

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
                            'flex items-center gap-2 rounded-md px-2 py-2 text-sm cursor-pointer hover:bg-primary hover:text-primary-foreground'
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
              disabled={!canSubmit || loadingOpcoes || submitting}
              className={cn(submitting && 'opacity-70')}
            >
              {submitting ? 'Enviando...' : 'Confirmar e realizar inscrição'}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
