import { useState, useEffect, Fragment } from 'react';
import { useNavigate } from 'react-router-dom';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { ChevronDown, ChevronRight, Users, Clock } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import type { MeuTalentoItem } from '@domain/entities/GestaoVagasCandidatos';
import { ListarMeusTalentosUseCase } from '@domain/usecases/ListarMeusTalentosUseCase';
import { formatDatePtBr } from '@presentation/hooks/recrutamento'
import { cn } from '@/lib/utils';

interface MeusCandidatosModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
}

function formatAtualizacoes(item: MeuTalentoItem): string {
  const status = item.statusMovimentacao?.trim() || item.statusVaga?.trim() || 'Atualização';
  const data = item.ultimaAlteracao ? formatDatePtBr(item.ultimaAlteracao, '') : '';
  if (!data) return status;
  return `${status} em ${data}`;
}

export function MeusCandidatosModal({
  open,
  onOpenChange,
  token,
}: MeusCandidatosModalProps) {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [lista, setLista] = useState<MeuTalentoItem[]>([]);
  const [expandedIds, setExpandedIds] = useState<Set<string>>(new Set());

  const listarMeusTalentosUseCase = container.resolve(ListarMeusTalentosUseCase);

  useEffect(() => {
    if (!open || !token) {
      setLista([]);
      setExpandedIds(new Set());
      return;
    }
    let cancelled = false;
    setLoading(true);
    listarMeusTalentosUseCase
      .execute(token)
      .then((list) => {
        if (cancelled) return;
        setLista(Array.isArray(list) ? list : []);
      })
      .catch(() => {
        if (!cancelled) setLista([]);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [open, token, listarMeusTalentosUseCase]);

  const toggleExpand = (codColaborador: string) => {
    setExpandedIds((prev) => {
      const next = new Set(prev);
      if (next.has(codColaborador)) next.delete(codColaborador);
      else next.add(codColaborador);
      return next;
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent
        className="max-w-4xl max-h-[90vh] flex flex-col p-4 sm:p-6"
        aria-labelledby="meus-candidatos-modal-title"
        aria-describedby="meus-candidatos-modal-desc"
      >
        <DialogHeader className="flex-row items-start justify-between gap-4 space-y-0 pr-8 sm:pr-10">
          <div className="flex flex-col gap-1 min-w-0 flex-1">
            <div className="flex items-center gap-3">
              <div
                className="h-10 w-10 sm:h-12 sm:w-12 rounded-xl bg-muted flex items-center justify-center shrink-0"
                aria-hidden
              >
                <Users className="h-6 w-6 sm:h-7 sm:w-7 text-muted-foreground" />
              </div>
              <DialogTitle
                id="meus-candidatos-modal-title"
                className="text-lg sm:text-2xl leading-tight"
              >
                Talentos das candidaturas sob minha responsabilidade
              </DialogTitle>
            </div>
            <DialogDescription
              id="meus-candidatos-modal-desc"
              className="text-sm text-muted-foreground pl-[52px] sm:pl-14"
            >
              Lista de talentos com detalhes expansíveis por linha.
            </DialogDescription>
          </div>
        </DialogHeader>

        {loading ? (
          <div
            className="flex items-center justify-center gap-2 py-12 text-muted-foreground min-h-[280px]"
            role="status"
            aria-live="polite"
          >
            <Spinner size={32} className="text-muted-foreground" />
            <span>Carregando…</span>
          </div>
        ) : lista.length === 0 ? (
          <div
            className="flex items-center justify-center py-12 text-muted-foreground min-h-[280px]"
            role="status"
          >
            Nenhum talento encontrado.
          </div>
        ) : (
          <div
            className="flex-1 min-h-[280px] rounded-md border overflow-auto -mx-1 px-1"
            role="region"
            aria-label="Tabela de talentos"
          >
            <Table className="min-w-[600px] w-full">
              <TableHeader>
                <TableRow>
                  <TableHead className="w-10 shrink-0" scope="col">
                    <span className="sr-only">Expandir ou recolher detalhes</span>
                  </TableHead>
                  <TableHead scope="col">Candidato</TableHead>
                  <TableHead scope="col">Cod / Nome da Vaga</TableHead>
                  <TableHead scope="col">Status</TableHead>
                  <TableHead scope="col">Cliente</TableHead>
                  <TableHead scope="col">Gestor</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {lista.map((item) => {
                  const isExpanded = expandedIds.has(item.codColaborador);
                  return (
                    <Fragment key={item.codColaborador}>
                      <TableRow
                        className={cn(isExpanded && 'bg-muted/30')}
                      >
                        <TableCell className="w-10 p-2 align-middle shrink-0">
                          <button
                            type="button"
                            onClick={() => toggleExpand(item.codColaborador)}
                            className="p-1.5 rounded-md hover:bg-muted focus:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 inline-flex items-center justify-center touch-manipulation"
                            aria-expanded={isExpanded}
                            aria-controls={`detalhes-${item.codColaborador}`}
                            aria-label={
                              isExpanded
                                ? `Recolher detalhes de ${item.nomeColaborador ?? 'candidato'}`
                                : `Ver detalhes de ${item.nomeColaborador ?? 'candidato'}`
                            }
                          >
                            {isExpanded ? (
                              <ChevronDown className="h-4 w-4" aria-hidden />
                            ) : (
                              <ChevronRight className="h-4 w-4" aria-hidden />
                            )}
                          </button>
                        </TableCell>
                        <TableCell className="font-medium min-w-[120px]">
                          <div className="flex items-center gap-1">
                            <span className="truncate">{item.nomeColaborador ?? '—'}</span>
                            <Tooltip>
                              <TooltipTrigger asChild>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="h-9 w-9 shrink-0"
                                  aria-label={`Ver histórico do candidato ${item.nomeColaborador ?? ''}`}
                                  onClick={() => {
                                    const codigo = item.codColaborador ?? '';
                                    if (codigo) {
                                      onOpenChange(false);
                                      const nomeParaHistorico = item.nomeColaborador?.trim() || undefined;
                                      navigate(`/recrutamento/historico-candidato/${encodeURIComponent(codigo)}`, {
                                        state: { nome: nomeParaHistorico },
                                      });
                                    }
                                  }}
                                >
                                  <Clock className="h-5 w-5" />
                                </Button>
                              </TooltipTrigger>
                              <TooltipContent>Ver histórico do candidato</TooltipContent>
                            </Tooltip>
                          </div>
                        </TableCell>
                        <TableCell className="min-w-[100px]">
                          {[item.codigoVaga, item.nomeVaga].filter(Boolean).join(' ') || '—'}
                        </TableCell>
                        <TableCell className="min-w-[80px]">{item.statusVaga ?? '—'}</TableCell>
                        <TableCell className="min-w-[100px]">{item.nomeCliente ?? '—'}</TableCell>
                        <TableCell className="min-w-[100px]">{item.nomeGestor ?? '—'}</TableCell>
                      </TableRow>
                      {isExpanded && (
                        <TableRow
                          id={`detalhes-${item.codColaborador}`}
                          className="bg-muted/60 hover:bg-muted/60"
                          aria-hidden={!isExpanded}
                        >
                          <TableCell colSpan={6} className="p-4 border-l-4 border-primary/30">
                            <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 text-sm">
                              <div>
                                <span className="text-muted-foreground block mb-0.5 text-xs uppercase tracking-wide">
                                  Atualizações
                                </span>
                                <Tooltip>
                                  <TooltipTrigger asChild>
                                    <span className="block truncate max-w-[200px]">
                                      {formatAtualizacoes(item)}
                                    </span>
                                  </TooltipTrigger>
                                  <TooltipContent side="bottom" className="max-w-xs">
                                    {formatAtualizacoes(item)}
                                  </TooltipContent>
                                </Tooltip>
                              </div>
                              <div>
                                <span className="text-muted-foreground block mb-0.5 text-xs uppercase tracking-wide">
                                  Rec. da Ultima Movimentação
                                </span>
                                <span>{item.recrutadorUltimaMovimentacao ?? '—'}</span>
                              </div>
                              <div>
                                <span className="text-muted-foreground block mb-0.5 text-xs uppercase tracking-wide">
                                  Criado por
                                </span>
                                <span>{item.criadoPor ?? '—'}</span>
                              </div>
                            </div>
                          </TableCell>
                        </TableRow>
                      )}
                    </Fragment>
                  );
                })}
              </TableBody>
            </Table>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}
