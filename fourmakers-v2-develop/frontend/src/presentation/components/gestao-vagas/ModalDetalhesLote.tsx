import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import type { DetalhesLote, ErroLoteItem } from '@domain/entities/GestaoVagasCandidatos';
import { BuscarInformacoesLoteUseCase } from '@domain/usecases/BuscarInformacoesLoteUseCase';
import { formatDatePtBr } from '@presentation/hooks/recrutamento'
import { Button } from '@/components/ui/button';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { Upload, AssignmentInd, AlertCircle, FileText } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';

interface ModalDetalhesLoteProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  idLote: string | null;
}

export function ModalDetalhesLote({
  open,
  onOpenChange,
  token,
  idLote,
}: ModalDetalhesLoteProps) {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [detalhes, setDetalhes] = useState<DetalhesLote | null>(null);

  const buscarInformacoesLoteUseCase = container.resolve(BuscarInformacoesLoteUseCase);

  useEffect(() => {
    if (!open || !token || !idLote) {
      setDetalhes(null);
      return;
    }
    let cancelled = false;
    setLoading(true);
    buscarInformacoesLoteUseCase
      .execute(token, idLote)
      .then((data) => {
        if (!cancelled) setDetalhes(data);
      })
      .catch(() => {
        if (!cancelled) {
          toast.error('Não foi possível carregar os detalhes do lote.');
          setDetalhes(null);
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [open, token, idLote, buscarInformacoesLoteUseCase]);

  const lote = detalhes?.lote;
  const pessoas = (detalhes?.pessoasCadastradasNesteLote ?? []).filter((p): p is NonNullable<typeof p> => p != null);
  const errosRaw = (detalhes?.erros ?? []).filter((e) => e != null);
  const pdfsRaw = (detalhes?.pdfsAProcessar ?? []).filter((item) => item != null);
  const totalItens = lote?.totalItens ?? 0;
  const quantidadeProcessada = lote?.quantidadeProcessada ?? 0;

  const textoErro = (item: string | ErroLoteItem): string => {
    if (typeof item === 'string') return item;
    const msg = item?.mensagemErro ?? '';
    const arquivo = item?.nomeArquivoCv ?? '';
    return arquivo ? `${msg} (${arquivo})` : msg || '—';
  };

  const textoPdf = (item: string | { nomeArquivo?: string; nomeArquivoCv?: string }): string => {
    if (typeof item === 'string') return item;
    return item?.nomeArquivo ?? item?.nomeArquivoCv ?? '—';
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-[750px] max-h-[85vh] flex flex-col p-0 gap-0">
        <DialogHeader className="flex flex-row items-center gap-4 px-6 py-4 border-b shrink-0">
          <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-lg bg-muted">
            <Upload className="h-7 w-7 text-primary" />
          </div>
          <div>
            <DialogTitle className="text-xl">Upload de lote de CVs</DialogTitle>
            <DialogDescription>Confira abaixo os detalhes do lote.</DialogDescription>
          </div>
        </DialogHeader>

        <div className="flex-1 overflow-y-auto px-6 py-4 space-y-4">
          {loading ? (
            <div className="flex items-center justify-center gap-2 py-12 text-muted-foreground">
              <Spinner size={32} />
              <span>Carregando…</span>
            </div>
          ) : detalhes ? (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="rounded-lg border bg-card p-4">
                  <p className="text-sm text-muted-foreground mb-1">Data de upload</p>
                  <p className="text-lg font-medium text-primary">
                    {lote?.dataCriacao ? formatDatePtBr(lote.dataCriacao) : '—'}
                  </p>
                </div>
                <div className="rounded-lg border bg-card p-4">
                  <p className="text-sm text-muted-foreground mb-1">CVs processados</p>
                  <p className="text-lg font-medium text-primary">
                    {quantidadeProcessada} de {totalItens}
                  </p>
                </div>
              </div>

              {pessoas.length > 0 && (
                <div className="space-y-3">
                  <h3 className="text-base font-semibold">Pessoas cadastradas</h3>
                  <div className="space-y-2 max-h-56 overflow-y-auto">
                    {pessoas.map((p, i) => (
                      <div
                        key={p?.codigoInternoColaborador ?? i}
                        className="flex items-center gap-3 rounded-lg border bg-card px-4 py-3"
                      >
                        <span className="text-sm font-medium flex-1 min-w-0 truncate">
                          {p?.nome ?? '—'}
                        </span>
                        <Tooltip>
                          <TooltipTrigger asChild>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-9 w-9 shrink-0 rounded-full"
                              aria-label="Ver Perfil"
                              onClick={() => {
                                const codigoColaborador = p?.codigoInternoColaborador?.trim();
                                if (!codigoColaborador) {
                                  toast.info('Código do colaborador não disponível para abrir o perfil.');
                                  return;
                                }
                                navigate(`/curriculoProfissional?cpf=${encodeURIComponent(codigoColaborador)}`, {
                                  state: { from: '/recrutamento/uploads' },
                                });
                              }}
                            >
                              <AssignmentInd className="h-5 w-5" />
                            </Button>
                          </TooltipTrigger>
                          <TooltipContent>Ver Perfil</TooltipContent>
                        </Tooltip>
                        <span className="text-muted-foreground text-xs shrink-0 border-l border-border pl-3">
                          Cadastro em {p?.dataDoCadastro ? formatDatePtBr(p.dataDoCadastro) : '—'}
                        </span>
                        <span
                          className={cn(
                            'shrink-0 rounded-md px-2 py-1 text-xs font-medium',
                            p?.possuiCandidatura
                              ? 'bg-primary/10 text-primary'
                              : 'bg-muted text-muted-foreground'
                          )}
                        >
                          {p?.possuiCandidatura
                            ? (Array.isArray(p?.candidaturas) && p.candidaturas.length > 0
                                ? `${p.candidaturas.length} candidatura(s)`
                                : 'Com candidaturas')
                            : 'Sem candidaturas'}
                        </span>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {errosRaw.length > 0 && (
                <div className="space-y-2">
                  <h3 className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                    <AlertCircle className="h-4 w-4 text-destructive" />
                    Erros
                  </h3>
                  <ul className="rounded-md border border-destructive/30 divide-y max-h-40 overflow-y-auto">
                    {errosRaw.map((item, i) => (
                      <li key={i} className="px-3 py-2 text-sm text-destructive">
                        {textoErro(item)}
                      </li>
                    ))}
                  </ul>
                </div>
              )}

              {pdfsRaw.length > 0 && (
                <div className="space-y-2">
                  <h3 className="text-sm font-medium text-muted-foreground flex items-center gap-2">
                    <FileText className="h-4 w-4" />
                    PDFs recebidos
                  </h3>
                  <ul className="rounded-md border divide-y max-h-40 overflow-y-auto">
                    {pdfsRaw.map((item, i) => (
                      <li key={i} className="px-3 py-2 text-sm">
                        {textoPdf(item)}
                      </li>
                    ))}
                  </ul>
                </div>
              )}

              {pessoas.length === 0 && errosRaw.length === 0 && pdfsRaw.length === 0 && (
                <p className="text-sm text-muted-foreground py-4">
                  Nenhum detalhe adicional para exibir.
                </p>
              )}
            </>
          ) : null}
        </div>
      </DialogContent>
    </Dialog>
  );
}
