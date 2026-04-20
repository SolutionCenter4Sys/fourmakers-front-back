import { useState, useCallback } from 'react';
import { container } from 'tsyringe';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { Sheet, SheetContent } from '@/components/ui/sheet';
import { Sparkles, PersonAdd, AssignmentInd, Info, ChevronLeft, ChevronRight, Eye } from '@/components/ui/system-icons';
import type { ColaboradorMatchPromptItem } from '@domain/entities/GestaoVagasCandidatos';
import { BuscarBancoTalentosComPromptMatchUseCase } from '@domain/usecases/BuscarBancoTalentosComPromptMatchUseCase';

import { AdherenceDetailsModal } from '@presentation/components/common/AdherenceDetailsModal';
import { PerfilCandidatoDrawerContent } from './PerfilCandidatoDrawerContent';
import { retornoMatchToMinhaJornadaAdherence } from '@domain/adapters/retornoMatchToAdherence';
import { cn } from '@/lib/utils';
import { toast } from 'sonner';

/** Opções de limite para a API (15 padrão; "todos" = 10000). */
export const LIMITE_OPTIONS = [
  { value: 15, label: '15 perfis' },
  { value: 30, label: '30 perfis' },
  { value: 50, label: '50 perfis' },
  { value: 100, label: '100 perfis' },
  { value: 10000, label: 'Mostrar todos' },
] as const;
export const PAGE_SIZE_OPTIONS = [10, 20, 50] as const;

function formatCandidaturaEmVagas(item: ColaboradorMatchPromptItem): string {
  if (!item.possui_candidatura || !item.candidaturas?.length) return 'Sem candidaturas';
  return `${item.candidaturas.length} vaga(s)`;
}

export interface GerarMatchPromptContentProps {
  token: string | null;
  /** Inscreve o talento na vaga (mesma lógica do card Aderentes / Inscrever Talento). */
  onInscrever: (codigoColaborador: string) => void;
  /** Códigos dos candidatos já inscritos nesta vaga; mostra check em vez de botão. */
  codigosInscritosNaVaga?: Set<string>;
}

export function GerarMatchPromptContent({
  token,
  onInscrever,
  codigosInscritosNaVaga,
}: GerarMatchPromptContentProps) {
  const [prompt, setPrompt] = useState('');
  const [limite, setLimite] = useState<number>(15);
  const [generating, setGenerating] = useState(false);
  const [lista, setLista] = useState<ColaboradorMatchPromptItem[]>([]);
  const [pageSize, setPageSize] = useState<number>(10);
  const [page, setPage] = useState(0);
  const [inscrevendoId, setInscrevendoId] = useState<string | null>(null);
  const [adherenceModalOpen, setAdherenceModalOpen] = useState(false);
  const [adherenceItem, setAdherenceItem] = useState<ColaboradorMatchPromptItem | null>(null);
  /** Side drawer do perfil do candidato: ao clicar no ícone Olho, abre com o código do colaborador. */
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [drawerCodigoColaborador, setDrawerCodigoColaborador] = useState<string | null>(null);

  const buscarBancoTalentosComPromptMatchUseCase = container.resolve(BuscarBancoTalentosComPromptMatchUseCase);

  const fetchMatch = useCallback(
    async (texto: string, limiteParam: number) => {
      if (!texto || !token) return;
      setGenerating(true);
      setLista([]);
      setPage(0);
      try {
        const res = await buscarBancoTalentosComPromptMatchUseCase.execute(token, {
          texto_vaga: texto,
          limite: limiteParam,
        });
        const colaboradores = res?.colaboradores ?? [];
        setLista(Array.isArray(colaboradores) ? colaboradores : []);
        if (colaboradores.length === 0) {
          toast.info('Nenhum perfil encontrado para o prompt informado.');
        }
      } catch (e) {
        console.error('Erro ao gerar match com prompt', e);
        toast.error('Erro ao gerar match. Tente novamente.');
        setLista([]);
      } finally {
        setGenerating(false);
      }
    },
    [token, buscarBancoTalentosComPromptMatchUseCase]
  );

  const handleGerarMatch = useCallback(() => {
    const texto = prompt.trim();
    if (!texto) {
      toast.error('Digite um prompt para gerar o match.');
      return;
    }
    fetchMatch(texto, limite);
  }, [prompt, limite, fetchMatch]);

  const handleLimiteChange = useCallback(
    (newLimite: number) => {
      setLimite(newLimite);
      const texto = prompt.trim();
      if (texto && token) fetchMatch(texto, newLimite);
    },
    [prompt, token, fetchMatch]
  );

  const listToShow = lista;
  const total = listToShow.length;
  const totalPages = Math.max(1, Math.ceil(total / pageSize));
  const currentPage = Math.min(page, totalPages - 1);
  const start = currentPage * pageSize;
  const pageLista = listToShow.slice(start, start + pageSize);

  const adherence = adherenceItem
    ? retornoMatchToMinhaJornadaAdherence(
        (adherenceItem.retornoMatch ?? null) as Parameters<typeof retornoMatchToMinhaJornadaAdherence>[0],
        adherenceItem.nome ?? 'Candidato'
      )
    : null;

  const handleInscrever = (item: ColaboradorMatchPromptItem) => {
    const cod = item.codigo_interno_colaborador;
    if (!cod || inscrevendoId) return;
    setInscrevendoId(cod);
    onInscrever(cod);
    setInscrevendoId(null);
  };

  return (
    <>
      <div className="flex flex-col gap-4 flex-1 min-h-0">
        <div className="space-y-4 w-full">
          <div className="space-y-1.5 w-full">
            <label htmlFor="gerar-match-prompt" className="text-sm font-medium text-foreground">
              Prompt
            </label>
            <Textarea
              id="gerar-match-prompt"
              placeholder="Ex: desenvolvedor javascript, angular, react, flutter..."
              value={prompt}
              onChange={(e) => setPrompt(e.target.value)}
              rows={3}
              className="resize-none w-full"
              disabled={generating}
            />
          </div>
          <Button
            type="button"
            variant="primary"
            onClick={handleGerarMatch}
            disabled={generating || !prompt.trim()}
            className="w-full sm:w-auto"
          >
            {generating ? (
              <>
                <Sparkles className="h-4 w-4 mr-2 animate-pulse" />
                Gerando…
              </>
            ) : (
              'Gerar match'
            )}
          </Button>
        </div>

        {generating && (
          <div className="flex flex-col items-center justify-center py-12 text-center min-h-[200px]" role="status" aria-live="polite">
            <div className="relative mb-4">
              <div className="w-16 h-16 rounded-full bg-primary/10 flex items-center justify-center animate-pulse">
                <Sparkles className="h-8 w-8 text-primary" />
              </div>
              <div
                className="absolute inset-0 w-16 h-16 rounded-full border-4 border-primary/30 border-t-primary animate-spin"
                style={{ animationDuration: '1.6s' }}
              />
            </div>
            <p className="text-base font-medium text-primary">Gerando match...</p>
            <p className="text-sm text-muted-foreground mt-1">A IA está buscando e calculando o match dos perfis.</p>
          </div>
        )}

        {!generating && listToShow.length > 0 && (
          <>
            <div className="flex-1 min-h-[280px] rounded-md border overflow-auto -mx-1 px-1" role="region" aria-label="Resultados do match">
              <Table className="min-w-[640px] w-full">
                <TableHeader>
                  <TableRow>
                    <TableHead scope="col">Match</TableHead>
                    <TableHead scope="col">Nome</TableHead>
                    <TableHead scope="col">Candidatura em vagas</TableHead>
                    <TableHead scope="col">Candidatar talento</TableHead>
                    <TableHead scope="col">Origem</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {pageLista.map((item, idx) => {
                    const cod = item.codigo_interno_colaborador ?? '';
                    const jaInscrito = cod ? codigosInscritosNaVaga?.has(cod) ?? false : false;
                    const matchPct = typeof item.match === 'number' ? item.match : 0;
                    const nome = item.nome ?? '—';
                    return (
                      <TableRow key={cod || `match-row-${start + idx}`}>
                        <TableCell colSpan={5} className="p-0 align-top">
                          <div className="border border-border rounded-md overflow-hidden my-1.5">
                            <table className="w-full min-w-[640px] border-collapse">
                              <tbody>
                                <tr>
                                  <td className="p-3 align-middle border-b border-border">
                                    <Tooltip>
                                      <TooltipTrigger asChild>
                                        <button
                                          type="button"
                                          className={cn(
                                            'inline-flex items-center gap-1.5 rounded-full border px-2.5 py-1 text-xs font-semibold transition-colors hover:opacity-90 min-h-[32px]',
                                            matchPct < 20 && 'border-destructive bg-destructive/10 text-destructive',
                                            matchPct >= 20 && matchPct < 80 && 'border-warning bg-warning/10 text-warning',
                                            matchPct >= 80 && 'border-success bg-success/10 text-success'
                                          )}
                                          aria-label="Detalhes do cálculo de match"
                                          onClick={() => {
                                            setAdherenceItem(item);
                                            setAdherenceModalOpen(true);
                                          }}
                                        >
                                          <span>{matchPct.toFixed(1)}%</span>
                                          <Info className="h-3.5 w-3.5 shrink-0" />
                                        </button>
                                      </TooltipTrigger>
                                      <TooltipContent>Cálculo de aderência e match</TooltipContent>
                                    </Tooltip>
                                  </td>
                                  <td className="p-3 align-middle border-b border-border">
                                    <div className="flex items-center gap-2">
                                      <Tooltip>
                                        <TooltipTrigger asChild>
                                          <Button
                                            type="button"
                                            variant="ghost"
                                            size="icon"
                                            className="h-8 w-8 shrink-0"
                                            aria-label="Abrir perfil do candidato"
                                            onClick={() => {
                                              setDrawerCodigoColaborador(cod || null);
                                              setDrawerOpen(true);
                                            }}
                                          >
                                            <Eye className="h-4 w-4 text-muted-foreground" />
                                          </Button>
                                        </TooltipTrigger>
                                        <TooltipContent>Ver perfil do candidato</TooltipContent>
                                      </Tooltip>
                                      <span className="font-medium">{nome}</span>
                                      <Tooltip>
                                        <TooltipTrigger asChild>
                                          <Button type="button" variant="ghost" size="icon" className="h-8 w-8 shrink-0" aria-label="Ver Perfil">
                                            <AssignmentInd className="h-4 w-4 text-muted-foreground" />
                                          </Button>
                                        </TooltipTrigger>
                                        <TooltipContent>Ver Perfil</TooltipContent>
                                      </Tooltip>
                                    </div>
                                  </td>
                                  <td className="p-3 align-middle border-b border-border">
                                    <span className="inline-flex items-center rounded-md border border-borderSoft bg-surfaceSubtle px-2 py-0.5 text-xs font-medium text-muted-foreground">
                                      {formatCandidaturaEmVagas(item)}
                                    </span>
                                  </td>
                                  <td className="p-3 align-middle border-b border-border">
                                    <Tooltip>
                                      <TooltipTrigger asChild>
                                        {jaInscrito ? (
                                          <span
                                            className="inline-flex h-8 w-8 shrink-0 items-center justify-center rounded-md text-primary"
                                            aria-label="Inscrito nesta vaga"
                                          >
                                            <PersonAdd className="h-4 w-4" />
                                          </span>
                                        ) : (
                                          <Button
                                            type="button"
                                            variant="ghost"
                                            size="icon"
                                            className="h-8 w-8 shrink-0"
                                            aria-label="Candidatar talento"
                                            disabled={!!inscrevendoId}
                                            onClick={() => handleInscrever(item)}
                                          >
                                            <PersonAdd className="h-4 w-4 text-muted-foreground" />
                                          </Button>
                                        )}
                                      </TooltipTrigger>
                                      <TooltipContent>
                                        {jaInscrito ? 'Inscrito nesta vaga' : 'Candidatar talento'}
                                      </TooltipContent>
                                    </Tooltip>
                                  </td>
                                  <td className="p-3 align-middle border-b border-border">
                                    <span className="inline-flex items-center rounded-md border border-borderSoft bg-surfaceSubtle px-2 py-0.5 text-xs font-medium text-muted-foreground">
                                      {item.origem ?? '—'}
                                    </span>
                                  </td>
                                </tr>
                                <tr className="bg-muted/20 hover:bg-muted/30">
                                  <td colSpan={5} className="py-3 px-3 align-top">
                                    <div className="text-sm text-muted-foreground">
                                      <span className="font-medium">Última Experiência em Breve</span>
                                    </div>
                                  </td>
                                </tr>
                              </tbody>
                            </table>
                          </div>
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>

            <div className="flex flex-wrap items-center justify-between gap-2 pt-2 border-t">
              <div className="flex flex-wrap items-center gap-4">
                <div className="flex items-center gap-2">
                  <span className="text-sm text-muted-foreground">Mostrar</span>
                  <Select
                    value={String(limite)}
                    onValueChange={(v) => handleLimiteChange(Number(v))}
                    disabled={generating}
                  >
                    <SelectTrigger className="w-[140px] h-8">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {LIMITE_OPTIONS.map((opt) => (
                        <SelectItem key={opt.value} value={String(opt.value)}>
                          {opt.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
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

        {!generating && listToShow.length === 0 && (
          <div className="flex flex-col items-center justify-center py-12 text-muted-foreground min-h-[120px]" role="status">
            <p className="text-sm">Digite um prompt e clique em &quot;Gerar match&quot; para buscar perfis.</p>
          </div>
        )}
      </div>

      <AdherenceDetailsModal
        open={adherenceModalOpen}
        onOpenChange={setAdherenceModalOpen}
        adherence={adherence}
      />

      <Sheet open={drawerOpen} onOpenChange={setDrawerOpen} modal={false}>
        <SheetContent
          side="right"
          className="w-full sm:max-w-2xl overflow-hidden flex flex-col p-0"
          aria-describedby={undefined}
        >
          <div className="flex-1 overflow-auto">
            <PerfilCandidatoDrawerContent
              codigoInternoColaborador={drawerCodigoColaborador}
              token={token}
              onClose={() => {
                setDrawerOpen(false);
                setDrawerCodigoColaborador(null);
              }}
            />
          </div>
        </SheetContent>
      </Sheet>
    </>
  );
}
