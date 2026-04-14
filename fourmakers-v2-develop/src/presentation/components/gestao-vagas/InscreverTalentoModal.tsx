import { useState, useEffect, useCallback } from 'react';
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
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { PersonAdd, AssignmentInd, Search, ChevronLeft, ChevronRight, CheckCircle2, Clock } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import { CountBadge } from '@presentation/components/common';
import type { BancoTalentosItem } from '@domain/entities/GestaoVagasCandidatos';
import { BuscarBancoTalentosUseCase } from '@domain/usecases/BuscarBancoTalentosUseCase';
import { formatDatePtBr } from '@presentation/hooks/recrutamento'
import { toast } from 'sonner';

/** Opções de itens por página na tabela (paginação client-side). */
const PAGE_SIZE_OPTIONS = [5, 10, 25, 50] as const;
/** Opções de limite da busca na API (quantos registros trazer do servidor). */
const LIMITE_BUSCA_OPCOES = [50, 100, 250, 500, 1000] as const;

interface InscreverTalentoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  /** Inscreve o talento na vaga atual (mesma lógica do card Aderentes e Qualificados). */
  onInscrever: (codigoColaborador: string) => void;
  /** Códigos (codigoInternoColaborador ou codigo) dos candidatos já inscritos nesta vaga; talentos presentes mostram check em vez de botão inscrever. */
  codigosInscritosNaVaga?: Set<string>;
  vagaId?: string;
  vagaTitle?: string;
  vagaRaw?: unknown;
}

export function InscreverTalentoModal({
  open,
  onOpenChange,
  token,
  onInscrever,
  codigosInscritosNaVaga,
  vagaId,
  vagaTitle,
  vagaRaw,
}: InscreverTalentoModalProps) {
  const navigate = useNavigate();
  const [busca, setBusca] = useState('');
  const [buscaSubmit, setBuscaSubmit] = useState('');
  const [limiteBusca, setLimiteBusca] = useState<number>(100);
  const [loading, setLoading] = useState(false);
  const [lista, setLista] = useState<BancoTalentosItem[]>([]);
  const [pageSize, setPageSize] = useState<number>(5);
  const [page, setPage] = useState(0);
  const [inscrevendoId, setInscrevendoId] = useState<string | null>(null);

  const buscarBancoTalentosUseCase = container.resolve(BuscarBancoTalentosUseCase);

  const fetchLista = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    try {
      const data = await buscarBancoTalentosUseCase.execute(token, {
        busca: buscaSubmit,
        cursor: 0,
        limite: limiteBusca,
      });
      setLista(Array.isArray(data) ? data : []);
      setPage(0);
    } catch (e) {
      console.error('Erro ao buscar banco de talentos', e);
      setLista([]);
    } finally {
      setLoading(false);
    }
  }, [token, buscaSubmit, limiteBusca, buscarBancoTalentosUseCase]);

  useEffect(() => {
    if (!open) return;
    fetchLista();
  }, [open, fetchLista]);

  const total = lista.length;
  const totalPages = Math.max(1, Math.ceil(total / pageSize));
  const currentPage = Math.min(page, totalPages - 1);
  const start = currentPage * pageSize;
  const pageLista = lista.slice(start, start + pageSize);

  const handleSearch = () => {
    setBuscaSubmit(busca.trim());
  };

  const handleInscrever = (item: BancoTalentosItem) => {
    if (inscrevendoId) return;
    setInscrevendoId(item.codigoInternoColaborador);
    onInscrever(item.codigoInternoColaborador);
    setInscrevendoId(null);
  };

  const handleVerHistorico = (item: BancoTalentosItem) => {
    const codigoColaborador = item.codigoInternoColaborador?.trim();
    if (!codigoColaborador) {
      toast.info('Código do candidato não disponível.');
      return;
    }

    navigate(`/recrutamento/historico-candidato/${encodeURIComponent(codigoColaborador)}`, {
      state: {
        nome: item.nome?.trim() || undefined,
        vagaId,
        vagaTitle,
        vagaRaw,
      },
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent
        className="max-w-4xl max-h-[90vh] flex flex-col p-4 sm:p-6"
        aria-labelledby="inscrever-talento-modal-title"
        aria-describedby="inscrever-talento-modal-desc"
      >
        <DialogHeader className="flex-row items-center justify-between space-y-0 gap-4">
          <div className="flex items-center gap-3">
            <div
              className="h-10 w-10 sm:h-12 sm:w-12 rounded-xl bg-muted flex items-center justify-center shrink-0"
              aria-hidden
            >
              <PersonAdd className="h-6 w-6 sm:h-7 sm:w-7 text-primary" />
            </div>
            <div>
              <DialogTitle id="inscrever-talento-modal-title" className="text-lg sm:text-2xl">
                Inscrever candidato
              </DialogTitle>
              <DialogDescription id="inscrever-talento-modal-desc">
                Inscrever candidato cadastrado no Banco de Talentos do Fourmakers.
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="flex flex-col gap-4 flex-1 min-h-0">
          <div className="flex flex-wrap items-center gap-2">
            <div className="relative flex-1 min-w-[200px] max-w-[300px]">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar talento..."
                value={busca}
                onChange={(e) => setBusca(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                className="pl-9"
              />
            </div>
            <Button type="button" variant="primary" size="icon" onClick={handleSearch} aria-label="Buscar">
              <Search className="h-4 w-4" />
            </Button>
          </div>

          {loading ? (
            <div className="flex items-center justify-center gap-2 py-12 text-muted-foreground min-h-[280px]" role="status" aria-live="polite">
              <Spinner size={32} className="text-muted-foreground" />
              <span>Carregando…</span>
            </div>
          ) : lista.length === 0 ? (
            <div className="flex items-center justify-center py-12 text-muted-foreground min-h-[280px]" role="status">
              Nenhum talento encontrado.
            </div>
          ) : (
            <>
              <div className="flex-1 min-h-[280px] rounded-md border overflow-auto -mx-1 px-1" role="region" aria-label="Tabela de talentos">
                <Table className="min-w-[640px] w-full">
                  <TableHeader>
                    <TableRow>
                      <TableHead scope="col">Nome</TableHead>
                      <TableHead scope="col">Data do cadastro</TableHead>
                      <TableHead scope="col">Candidatura em vagas</TableHead>
                      <TableHead scope="col">Criado por</TableHead>
                      <TableHead scope="col">Origem</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {pageLista.map((item) => {
                      const jaInscrito = codigosInscritosNaVaga?.has(item.codigoInternoColaborador) ?? false;
                      return (
                      <TableRow key={item.codigoInternoColaborador}>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <Tooltip>
                              <TooltipTrigger asChild>
                                <Button
                                  type="button"
                                  variant="ghost"
                                  size="icon"
                                  className="h-11 w-11 shrink-0 rounded-full"
                                  aria-label="Ver Perfil"
                                  onClick={() => {
                                    const codigoColaborador = item.codigoInternoColaborador?.trim();
                                    if (!codigoColaborador) {
                                      toast.info('Código do colaborador não disponível para abrir o perfil.');
                                      return;
                                    }
                                    navigate(`/curriculoProfissional?cpf=${encodeURIComponent(codigoColaborador)}`, {
                                      state: {
                                        from: '/recrutamento/candidatos',
                                        vagaId,
                                        vagaTitle,
                                        vagaRaw,
                                      },
                                    });
                                  }}
                                >
                                  <AssignmentInd className="h-5 w-5" />
                                </Button>
                              </TooltipTrigger>
                              <TooltipContent>Ver Perfil</TooltipContent>
                            </Tooltip>
                            <span className="font-medium">{item.nome || '—'}</span>
                            {jaInscrito ? (
                              <Tooltip>
                                <TooltipTrigger asChild>
                                  <span className="inline-flex h-8 w-8 shrink-0 items-center justify-center rounded-md text-success" aria-label="Inscrito nesta vaga">
                                    <CheckCircle2 className="h-4 w-4" />
                                  </span>
                                </TooltipTrigger>
                                <TooltipContent>Inscrito nesta vaga</TooltipContent>
                              </Tooltip>
                            ) : (
                              <Tooltip>
                                <TooltipTrigger asChild>
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8 shrink-0"
                                    aria-label="Inscrever candidato"
                                    disabled={!!inscrevendoId}
                                    onClick={() => handleInscrever(item)}
                                  >
                                    <PersonAdd className="h-4 w-4 text-muted-foreground" />
                                  </Button>
                                </TooltipTrigger>
                                <TooltipContent>Inscrever candidato</TooltipContent>
                              </Tooltip>
                            )}
                          </div>
                        </TableCell>
                        <TableCell>{formatDatePtBr(item.dataDoCadastro, '—')}</TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <span className="text-sm whitespace-nowrap">
                              {item.possuiCandidatura ? 'Ver candidaturas' : 'Não possui candidaturas'}
                            </span>
                            {item.possuiCandidatura && (
                              <div className="relative inline-flex items-start">
                                <Tooltip>
                                  <TooltipTrigger asChild>
                                    <Button
                                      type="button"
                                      variant="ghost"
                                      size="icon"
                                      className="h-9 w-9 shrink-0"
                                      aria-label={`Ver histórico do candidato${item.nome ? ` ${item.nome}` : ''}`}
                                      onClick={() => handleVerHistorico(item)}
                                    >
                                      <Clock className="h-5 w-5" />
                                    </Button>
                                  </TooltipTrigger>
                                  <TooltipContent>Histórico do candidato</TooltipContent>
                                </Tooltip>
                                <span className="pointer-events-none absolute -top-0.5 -right-0.5 z-10">
                                  <CountBadge
                                    count={item.candidaturas?.length ?? 0}
                                    className="!min-w-[1rem] !h-4 !p-0 !text-[10px]"
                                  />
                                </span>
                              </div>
                            )}
                          </div>
                        </TableCell>
                        <TableCell>{item.nomeCadastrante ?? 'Não informado'}</TableCell>
                        <TableCell>Não informado</TableCell>
                      </TableRow>
                    );
                    })}
                  </TableBody>
                </Table>
              </div>

              <div className="flex flex-wrap items-center justify-between gap-2 pt-2 border-t">
                <div className="flex flex-wrap items-center gap-4">
                  <div className="flex items-center gap-2">
                    <span className="text-sm text-muted-foreground">Linhas por página:</span>
                    <Select
                      value={String(pageSize)}
                      onValueChange={(v) => {
                        setPageSize(Number(v) as (typeof PAGE_SIZE_OPTIONS)[number]);
                        setPage(0);
                      }}
                    >
                      <SelectTrigger className="w-[70px] h-8">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        {PAGE_SIZE_OPTIONS.map((n) => (
                          <SelectItem key={n} value={String(n)}>
                            {n}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="text-sm text-muted-foreground">Limite de resultados:</span>
                    <Select
                      value={String(limiteBusca)}
                      onValueChange={(v) => setLimiteBusca(Number(v))}
                    >
                      <SelectTrigger className="w-[80px] h-8" aria-label="Quantidade de registros a carregar">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        {LIMITE_BUSCA_OPCOES.map((n) => (
                          <SelectItem key={n} value={String(n)}>
                            {n}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <span>
                    {total === 0 ? '0' : `${start + 1} - ${Math.min(start + pageSize, total)} de ${total}`}
                  </span>
                  <Button
                    type="button"
                    variant="outline"
                    size="icon"
                    className="h-8 w-8"
                    disabled={currentPage <= 0}
                    onClick={() => setPage((p) => Math.max(0, p - 1))}
                    aria-label="Página anterior"
                  >
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    size="icon"
                    className="h-8 w-8"
                    disabled={currentPage >= totalPages - 1}
                    onClick={() => setPage((p) => Math.min(totalPages - 1, p + 1))}
                    aria-label="Próxima página"
                  >
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
