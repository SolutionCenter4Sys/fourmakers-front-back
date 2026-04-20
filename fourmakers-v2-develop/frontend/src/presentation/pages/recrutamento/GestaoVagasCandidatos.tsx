import { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { TooltipProvider, Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Badge } from '@/components/ui/badge';
import { Spinner } from '@/components/ui/spinner';
import { Slider } from '@/components/ui/slider';
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from '@/components/ui/sheet';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import { toast } from 'sonner';
import { useAppSelector } from '@app/store/hooks';
import {
  useGestaoVagasCandidatos,
  DEFAULT_FILTROS_ADERENTES,
  formatDatePtBr,
  formatCurrencyBRL,
  formatSlaTooltip,
  formatFrequenciaPresencial,
  firstNonEmpty,
  getDescricaoById,
  parseSlaToHours,
} from '@presentation/hooks/recrutamento'
import type {
  CandidatoInscrito,
  CandidatoAderenteRaw,
  StatusCandidatura,
  MudarStatusCandidaturaRetorno,
} from '@domain/entities/GestaoVagasCandidatos';
import type { ListarCandidatosAderentesParams } from '@domain/repositories/VagaRepository';
import {
  ArrowLeft,
  ChevronDown,
  ChevronUp,
  Edit,
  GripVertical,
  Info,
  Search,
  Sparkles,
  AssignmentInd,
  Calculator,
  Clock,
  FileText,
  MessageCircle,
  WorkspacePremium,
  CheckCircle2,
  SlidersHorizontal,
  Plus,
  Minus,
  PersonAdd,
} from '@/components/ui/system-icons';
import { AdherenceDetailsModal } from '@presentation/components/common/AdherenceDetailsModal';
import {
  InscreverCandidatoModal,
  InscricaoPreferenciasModal,
  InscreverTalentoModal,
  AlterarStatusCandidatoModal,
  AtribuirRecrutadorCandidatoModal,
  EditarDadosPessoaisModal,
  ComentarioJornadaModal,
} from '@presentation/components/gestao-vagas';
import { retornoMatchToMinhaJornadaAdherence } from '@domain/adapters/retornoMatchToAdherence';
import { useViaCep } from '@presentation/hooks/useViaCep';
import { cn } from '@/lib/utils';
const LIMITE_OPCOES = [
  { value: 10, label: '10' },
  { value: 20, label: '20' },
  { value: 30, label: '30' },
  { value: 50, label: '50' },
  { value: 10000, label: 'Todos' },
];

const SLA_LIMITE_HORAS = 2;

/** Normaliza descrição da vaga: converte \n literais em quebras reais e trim. Emojis permanecem. */
function formatDescricaoVaga(descricao: string | null | undefined): string {
  if (descricao == null || String(descricao).trim() === '') return 'Não informado';
  return String(descricao)
    .replace(/\\n/g, '\n')
    .replace(/\r\n/g, '\n')
    .replace(/\r/g, '\n')
    .replace(/\n{3,}/g, '\n\n')
    .trim();
}

interface CandidatoCardProps {
  candidato: CandidatoInscrito;
  statusCandidatura: StatusCandidatura[];
  vagaId: string;
  vagaTitle?: string;
  vagaRaw?: unknown;
  /** Id do status da coluna (ex.: "1", "2"). A partir de 2 exibe Responsável Atual + botão editar. */
  statusId?: string;
  /** Ao selecionar outro status no dropdown, abre o modal de alteração (comentário obrigatório). */
  onStatusSelect?: (candidaturaId: string, codigoStatus: string) => void;
  /** Ao clicar em editar do responsável, abre modal para atribuir recrutador (status >= 2). */
  onAtribuirRecrutador?: (candidato: CandidatoInscrito) => void;
  /** Chamado ao iniciar arraste do card (para DnD entre colunas). */
  onDragStartCard?: (idCandidatura: string, statusFrom: string) => void;
  /** Abre modal de edição dos dados pessoais do candidato. */
  onEditarDadosPessoais?: (params: { codigoColaborador: string; nome?: string }) => void;
  /** Abre modal de comentário da jornada para a candidatura. */
  onComentarJornada?: (params: { candidaturaId: string; nome?: string }) => void;
}

function CandidatoCard({
  candidato,
  statusCandidatura,
  vagaId,
  vagaTitle,
  vagaRaw,
  statusId,
  onStatusSelect,
  onAtribuirRecrutador,
  onDragStartCard,
  onEditarDadosPessoais,
  onComentarJornada,
}: CandidatoCardProps) {
  const navigate = useNavigate();
  const [adherenceModalOpen, setAdherenceModalOpen] = useState(false);

  const nome = (candidato.nome ?? candidato.nomeColaborador ?? 'Não informado') as string;
  const dataCandidatura = candidato.dataCandidatura ? formatDatePtBr(candidato.dataCandidatura) : '';
  const criadoPor = (candidato.criadoPor ?? 'Sem informação') as string;
  const modificadoEm = candidato.modificadoEm ? formatDatePtBr(String(candidato.modificadoEm)) : '';
  const percentual = candidato.percentualMatch ?? 0;
  const totalOutrasVagas = candidato.totalInscritoOutrasVagas ?? 0;
  const qualificado = candidato.qualificado === true;
  const dataQualificacao = candidato.dataQualificacao ? formatDatePtBr(candidato.dataQualificacao) : '';
  const nomeQualificou = candidato.nomeDeQuemQualificou ?? '';

  const mostraResponsavel = statusId != null && Number(statusId) >= 2;
  const responsavelAtual = candidato.recrutadorResponsavel?.trim() || 'Não informado';

  const { hours: slaHours, label: slaLabel } = parseSlaToHours(candidato.tempoDecorridoTexto);
  const slaExcedeLimite = slaHours > SLA_LIMITE_HORAS;
  const slaTooltip = formatSlaTooltip(
    candidato.slaDecorridoDaEtapaAtual,
    candidato.tempoDecorridoTexto,
    SLA_LIMITE_HORAS
  );

  const organizacoesList = (candidato.organizacoes ?? [])
    .map((o) => o.orgDescricao)
    .filter(Boolean)
    .join(', ') || '—';

  const adherence = retornoMatchToMinhaJornadaAdherence(candidato.retornoMatch ?? null, nome);
  const simuladorUrl = `/simulador?codigoInternoColaborador=${encodeURIComponent(candidato.codigo ?? '')}&idCandidatura=${encodeURIComponent(candidato.idCandidatura ?? '')}&idVaga=${encodeURIComponent(vagaId)}`;
  const hasOrigemExterna = candidato.origem != null && String(candidato.origem).trim() !== '';
  const origemLabel = hasOrigemExterna
    ? String(candidato.origem).trim()
    : (organizacoesList !== '—' ? organizacoesList : '—');
  const isOrigemInterna = !hasOrigemExterna && organizacoesList !== '—';

  return (
    <>
      <Card className="shadow-none border-borderSoft cursor-default relative" data-kanban-card>
        <CardContent className="p-4 space-y-3">
          {onDragStartCard && (
            <div
              role="button"
              tabIndex={0}
              draggable
              onDragStart={(e) => {
                e.dataTransfer.setData('text/plain', 'candidato');
                e.dataTransfer.effectAllowed = 'move';
                onDragStartCard(candidato.idCandidatura ?? '', String(candidato.statusCandidaturaId ?? statusId ?? ''));
                const cardEl = (e.currentTarget as HTMLElement).closest('[data-kanban-card]') as HTMLElement | null;
                if (cardEl) {
                  const rect = cardEl.getBoundingClientRect();
                  const clone = cardEl.cloneNode(true) as HTMLElement;
                  clone.style.opacity = '0.95';
                  clone.style.pointerEvents = 'none';
                  clone.style.position = 'absolute';
                  clone.style.left = '-9999px';
                  clone.style.top = '0';
                  clone.style.width = `${rect.width}px`;
                  document.body.appendChild(clone);
                  const offsetX = rect.width - 36;
                  const offsetY = 20;
                  e.dataTransfer.setDragImage(clone, offsetX, offsetY);
                  setTimeout(() => clone.remove(), 0);
                }
              }}
              className="absolute top-2 right-2 p-1 rounded cursor-grab active:cursor-grabbing text-muted-foreground hover:bg-muted/50 touch-none z-10"
              aria-label="Arrastar para alterar coluna"
            >
              <GripVertical className="h-4 w-4" />
            </div>
          )}
          {/* Nome + ícone edição */}
          <div className="flex items-start justify-between gap-2">
            <span className="text-sm font-medium leading-snug">{nome}</span>
            <Tooltip>
              <TooltipTrigger asChild>
                <button
                  type="button"
                  className="min-h-[44px] min-w-[44px] flex items-center justify-center rounded hover:bg-muted/50 p-2 -m-2"
                  aria-label="Editar dados pessoais"
                  onClick={() => {
                    const codigoColaborador = candidato.codigo ?? '';
                    if (!codigoColaborador) {
                      toast.info('Código do colaborador não disponível para edição.');
                      return;
                    }
                    onEditarDadosPessoais?.({
                      codigoColaborador,
                      nome,
                    });
                  }}
                >
                  <Edit className="h-5 w-5 text-muted-foreground" />
                </button>
              </TooltipTrigger>
              <TooltipContent side="left">Editar dados pessoais</TooltipContent>
            </Tooltip>
          </div>

          {/* Mesma linha: tag match + ícone cálculo + ícone avaliação + ícone criado por */}
          <div className="flex flex-wrap items-center gap-2">
            <Tooltip>
              <TooltipTrigger asChild>
                <button
                  type="button"
                  className={cn(
                    'inline-flex items-center gap-1.5 rounded-md border px-2 py-1 text-xs font-semibold transition-colors hover:opacity-90 min-h-[44px]',
                    percentual < 20 && 'border-destructive bg-destructive/10 text-destructive',
                    percentual >= 20 && percentual < 80 && 'border-warning bg-warning/10 text-warning',
                    percentual >= 80 && 'border-success bg-success/10 text-success'
                  )}
                  aria-label="Detalhes do cálculo de match"
                  onClick={() => setAdherenceModalOpen(true)}
                >
                  <span>{Math.round(percentual)}%</span>
                  <Info className="h-5 w-5 shrink-0" />
                </button>
              </TooltipTrigger>
              <TooltipContent>Cálculo de aderência e match</TooltipContent>
            </Tooltip>

            <Tooltip>
              <TooltipTrigger asChild>
                <span
                  className={cn(
                    'inline-flex items-center',
                    qualificado ? 'text-success' : 'text-muted-foreground/50 cursor-not-allowed'
                  )}
                  aria-hidden
                >
                  <WorkspacePremium className="h-5 w-5" />
                </span>
              </TooltipTrigger>
              <TooltipContent>
                {qualificado
                  ? `Qualificado em ${dataQualificacao}, por ${nomeQualificou}`
                  : 'Sem avaliação'}
              </TooltipContent>
            </Tooltip>

            <Tooltip>
              <TooltipTrigger asChild>
                <span className="inline-flex text-muted-foreground" aria-hidden>
                  <CheckCircle2 className="h-5 w-5" />
                </span>
              </TooltipTrigger>
              <TooltipContent>
                Criado por {criadoPor}, em {candidato.dataCandidatura ? formatDatePtBr(candidato.dataCandidatura) : '—'}
              </TooltipContent>
            </Tooltip>
          </div>

          {/* Linha: tag origem (valor do dado ou orgDescricao se interno) + tooltip com orgs */}
          <Tooltip>
            <TooltipTrigger asChild>
              <span
                className={cn(
                  'inline-flex items-center rounded-md border px-2 py-0.5 text-xs font-medium',
                  isOrigemInterna
                    ? 'border-info bg-info/10 text-info'
                    : 'border-borderSoft bg-surfaceSubtle text-primaryText'
                )}
              >
                {origemLabel}
              </span>
            </TooltipTrigger>
            <TooltipContent>Organizações: {organizacoesList}</TooltipContent>
          </Tooltip>

          {/* Inscrição em vagas */}
          <p className="text-xs text-muted-foreground">
            {totalOutrasVagas > 0
              ? `Inscrito nesta e em outras ${totalOutrasVagas} vagas`
              : 'Inscrito somente nesta vaga'}
          </p>

          {mostraResponsavel && (
            <div className="flex items-center justify-between gap-2">
              <span className="text-xs text-muted-foreground">
                Responsável Atual: <span className="text-foreground">{responsavelAtual}</span>
              </span>
              <Tooltip>
                <TooltipTrigger asChild>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 shrink-0"
                    aria-label="Atribuir recrutador responsável"
                    onClick={() => onAtribuirRecrutador?.(candidato)}
                  >
                    <Edit className="h-4 w-4 text-muted-foreground" />
                  </Button>
                </TooltipTrigger>
                <TooltipContent>Atribuir recrutador responsável</TooltipContent>
              </Tooltip>
            </div>
          )}

          <hr className="border-borderSoft" />

          {/* Ícones de ação: área de toque mínima 44px, ícones 20px */}
          <div className="flex items-center gap-1">
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-11 w-11 shrink-0"
                  aria-label="Ver Perfil"
                  onClick={() => {
                    const codigoColaborador = candidato.codigo?.trim();
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
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-11 w-11 shrink-0"
                  aria-label="Ver histórico do candidato"
                  onClick={() => {
                    const codigo = candidato.codigo ?? '';
                    if (codigo) {
                      const nomeParaHistorico = (candidato.nome ?? candidato.nomeColaborador)?.trim() || undefined;
                      navigate(`/recrutamento/historico-candidato/${encodeURIComponent(codigo)}`, {
                        state: {
                          nome: nomeParaHistorico,
                          vagaId,
                          vagaTitle,
                          vagaRaw,
                        },
                      });
                    }
                  }}
                >
                  <Clock className="h-5 w-5" />
                </Button>
              </TooltipTrigger>
              <TooltipContent>Ver histórico do candidato</TooltipContent>
            </Tooltip>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button variant="ghost" size="icon" className="h-11 w-11 shrink-0" aria-label="Simulador de remuneração" asChild>
                  <Link to={simuladorUrl} state={{ from: 'kanban-candidatos' }}>
                    <Calculator className="h-5 w-5" />
                  </Link>
                </Button>
              </TooltipTrigger>
              <TooltipContent>Simulador de remuneração</TooltipContent>
            </Tooltip>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-11 w-11 shrink-0"
                  aria-label="Comentários da jornada"
                  onClick={() => {
                    const candidaturaId = candidato.idCandidatura ?? '';
                    if (!candidaturaId) {
                      toast.info('Candidatura não disponível para comentário.');
                      return;
                    }
                    onComentarJornada?.({ candidaturaId, nome });
                  }}
                >
                  <MessageCircle className="h-5 w-5" />
                </Button>
              </TooltipTrigger>
              <TooltipContent>Comentários da jornada</TooltipContent>
            </Tooltip>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button variant="ghost" size="icon" className="h-11 w-11 shrink-0" aria-label="Template de contratação" asChild>
                  <Link
                  to={`/recrutamento/template-contratacao/${candidato.idCandidatura ?? ''}`}
                  state={{ vagaId, vagaTitle, vagaRaw, exibirRemuneracao: candidato.exibirRemuneracao === true }}
                >
                    <FileText className="h-5 w-5" />
                  </Link>
                </Button>
              </TooltipTrigger>
              <TooltipContent>Template de contratação</TooltipContent>
            </Tooltip>
          </div>

          {/* Dropdown status */}
          <Select
            value={candidato.statusCandidaturaId ?? ''}
            onValueChange={(value) => {
              if (value && value !== candidato.statusCandidaturaId) {
                onStatusSelect?.(candidato.idCandidatura ?? '', value);
              }
            }}
          >
            <SelectTrigger className="h-9 text-xs">
              <SelectValue placeholder="Status" />
            </SelectTrigger>
            <SelectContent>
              {statusCandidatura.map((s) => (
                <SelectItem key={s.id} value={s.id}>
                  {s.nome}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          <hr className="border-borderSoft" />

          {/* Tempo decorrido + tag */}
          <div className="flex items-center gap-2 flex-wrap">
            <span className="text-xs text-muted-foreground">Tempo decorrido</span>
            <Tooltip>
              <TooltipTrigger asChild>
                <Badge
                  variant={slaExcedeLimite ? 'destructive' : 'default'}
                  className={cn(
                    'text-xs',
                    !slaExcedeLimite && 'border-success/50 bg-success/10 text-success'
                  )}
                >
                  {slaLabel}
                </Badge>
              </TooltipTrigger>
              <TooltipContent className="whitespace-pre-wrap">{slaTooltip}</TooltipContent>
            </Tooltip>
          </div>

          <hr className="border-borderSoft" />

          {/* Rodapé: Criado por, Candidatura em, Modificado em */}
          <div className="space-y-1 text-xs text-muted-foreground">
            <div>Criado por: {criadoPor}</div>
            {dataCandidatura && <div>Candidatura em: {dataCandidatura}</div>}
            {modificadoEm && <div>Modificado em {modificadoEm}</div>}
          </div>
        </CardContent>
      </Card>

      <AdherenceDetailsModal
        open={adherenceModalOpen}
        onOpenChange={setAdherenceModalOpen}
        adherence={adherence}
      />
    </>
  );
}

interface AderenteCardProps {
  item: CandidatoAderenteRaw;
  vagaId: string;
  vagaTitle?: string;
  vagaRaw?: unknown;
  onInscrever?: (codigoColaborador: string) => void;
  onEditarDadosPessoais?: (params: { codigoColaborador: string; nome?: string }) => void;
}

function AderenteCard({ item, vagaId, vagaTitle, vagaRaw, onInscrever, onEditarDadosPessoais }: AderenteCardProps) {
  const navigate = useNavigate();
  const [adherenceModalOpen, setAdherenceModalOpen] = useState(false);
  const nome = (item.nome ?? 'Não informado') as string;
  const percentual = item.percentualAderencia ?? 0;
  const origem = (item.origem ?? item.retornoMatch?.origem ?? '—') as string;
  const adherence = retornoMatchToMinhaJornadaAdherence(item.retornoMatch ?? null, nome);
  const codigoColaborador = item.codigo ?? '';

  return (
    <>
      <Card className="shadow-none border-borderSoft cursor-default" data-kanban-card>
        <CardContent className="p-4 space-y-3">
          <div className="flex items-start justify-between gap-2">
            <span className="text-sm font-medium leading-snug">{nome}</span>
            <Tooltip>
              <TooltipTrigger asChild>
                <button
                  type="button"
                  className="min-h-[44px] min-w-[44px] flex items-center justify-center rounded hover:bg-muted/50 p-2 -m-2"
                  aria-label="Editar dados pessoais"
                  onClick={() => {
                    if (!codigoColaborador) {
                      toast.info('Código do colaborador não disponível para edição.');
                      return;
                    }
                    onEditarDadosPessoais?.({
                      codigoColaborador,
                      nome,
                    });
                  }}
                >
                  <Edit className="h-5 w-5 text-muted-foreground" />
                </button>
              </TooltipTrigger>
              <TooltipContent side="left">Editar dados pessoais</TooltipContent>
            </Tooltip>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <Tooltip>
              <TooltipTrigger asChild>
                <button
                  type="button"
                  className={cn(
                    'inline-flex items-center gap-1.5 rounded-md border px-2 py-1 text-xs font-semibold transition-colors hover:opacity-90 min-h-[44px]',
                    percentual < 20 && 'border-destructive bg-destructive/10 text-destructive',
                    percentual >= 20 && percentual < 80 && 'border-warning bg-warning/10 text-warning',
                    percentual >= 80 && 'border-success bg-success/10 text-success'
                  )}
                  aria-label="Detalhes do cálculo de match"
                  onClick={() => setAdherenceModalOpen(true)}
                >
                  <span>{percentual.toFixed(2)} %</span>
                  <Info className="h-5 w-5 shrink-0" />
                </button>
              </TooltipTrigger>
              <TooltipContent>Cálculo de aderência e match</TooltipContent>
            </Tooltip>
          </div>
          <span className="inline-flex items-center rounded-md border border-borderSoft bg-surfaceSubtle px-2 py-0.5 text-xs font-medium text-muted-foreground">
            {origem}
          </span>
          <hr className="border-borderSoft" />
          <div className="flex items-center gap-1">
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-11 w-11 shrink-0"
                  aria-label="Ver Perfil"
                  onClick={() => {
                    const codigo = codigoColaborador?.trim();
                    if (!codigo) {
                      toast.info('Código do colaborador não disponível para abrir o perfil.');
                      return;
                    }
                    navigate(`/curriculoProfissional?cpf=${encodeURIComponent(codigo)}`, {
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
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-11 w-11 shrink-0"
                  aria-label="Ver histórico do candidato"
                  onClick={() => {
                    if (codigoColaborador) {
                      const nomeParaHistorico = item.nome?.trim() || undefined;
                      navigate(`/recrutamento/historico-candidato/${encodeURIComponent(codigoColaborador)}`, {
                        state: {
                          nome: nomeParaHistorico,
                          vagaId,
                          vagaTitle,
                          vagaRaw,
                        },
                      });
                    }
                  }}
                >
                  <Clock className="h-5 w-5" />
                </Button>
              </TooltipTrigger>
              <TooltipContent>Ver histórico do candidato</TooltipContent>
            </Tooltip>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-11 w-11 shrink-0"
                  aria-label="Inscrever candidato"
                  disabled={!codigoColaborador || !onInscrever}
                  onClick={() => onInscrever?.(codigoColaborador)}
                >
                  <PersonAdd className="h-5 w-5" />
                </Button>
              </TooltipTrigger>
              <TooltipContent>Inscrever candidato</TooltipContent>
            </Tooltip>
            <Tooltip>
              <TooltipTrigger asChild>
                <span
                  className={cn(
                    'inline-flex items-center',
                    item.qualificado === true ? 'text-success' : 'text-muted-foreground/50 cursor-not-allowed'
                  )}
                  aria-hidden
                >
                  <WorkspacePremium className="h-5 w-5" />
                </span>
              </TooltipTrigger>
              <TooltipContent>{item.qualificado === true ? 'Qualificado' : 'Não qualificado'}</TooltipContent>
            </Tooltip>
          </div>
        </CardContent>
      </Card>
      <AdherenceDetailsModal
        open={adherenceModalOpen}
        onOpenChange={setAdherenceModalOpen}
        adherence={adherence}
      />
    </>
  );
}

export default function GestaoVagasCandidatos() {
  const navigate = useNavigate();
  const {
    vagaId,
    vagaTitle,
    vagaRaw,
    vagaDetalhes,
    unidadesList,
    tiposVagaList,
    tiposContratacaoList,
    loading,
    busca,
    setBusca,
    statusCandidatura,
    totais,
    candidatos,
    candidatosPorStatus,
    infoVagaExpanded,
    setInfoVagaExpanded,
    aderentes,
    loadingAderentes,
    filtrosAderentes,
    setFiltrosAderentes,
    filtrosAderentesOpen,
    setFiltrosAderentesOpen,
    limparFiltrosAderentes,
    origensColaborador,
    inscreverCandidato,
    refetch,
  } = useGestaoVagasCandidatos();

  const [filtrosDraft, setFiltrosDraft] = useState<Omit<ListarCandidatosAderentesParams, 'vagaId'>>(DEFAULT_FILTROS_ADERENTES);
  const [cepFiltro, setCepFiltro] = useState('');
  const { buscarCep, endereco: enderecoCep, loading: loadingCep } = useViaCep();

  useEffect(() => {
    if (filtrosAderentesOpen) {
      setFiltrosDraft({ ...filtrosAderentes });
      setCepFiltro(filtrosAderentes.localizacaoCidade || filtrosAderentes.localizacaoEstado ? '' : '');
    }
  }, [filtrosAderentesOpen, filtrosAderentes]);

  useEffect(() => {
    if (enderecoCep) {
      setFiltrosDraft((prev) => ({
        ...prev,
        localizacaoCidade: enderecoCep.cidade ?? prev.localizacaoCidade,
        localizacaoEstado: enderecoCep.estado ?? prev.localizacaoEstado,
      }));
    }
  }, [enderecoCep]);

  const [alterarStatusModalOpen, setAlterarStatusModalOpen] = useState(false);
  const [alterarStatusId, setAlterarStatusId] = useState<string>('');
  const [alterarStatusCodigo, setAlterarStatusCodigo] = useState<number>(0);
  const [atribuirRecrutadorCandidato, setAtribuirRecrutadorCandidato] = useState<{
    idCandidatura: string;
    recrutadorNome: string | null;
  } | null>(null);
  /** Retorno da última alteração de status (candidaturaId, comentarioId) para próximas ações (ex.: anexar arquivo ao comentário). */
  const [_lastMudarStatusResponse, setLastMudarStatusResponse] =
    useState<MudarStatusCandidaturaRetorno | null>(null);

  const handleStatusSelect = useCallback((candidaturaId: string, codigoStatus: string) => {
    setAlterarStatusId(candidaturaId);
    setAlterarStatusCodigo(Number(codigoStatus));
    setAlterarStatusModalOpen(true);
  }, []);

  const kanbanScrollRef = useRef<HTMLDivElement>(null);
  const kanbanDragStartX = useRef(0);
  const kanbanDragStartScroll = useRef(0);
  const [isKanbanDragging, setIsKanbanDragging] = useState(false);
  const dragPayloadRef = useRef<{ idCandidatura: string; statusFrom: string } | null>(null);
  const [opcoesTelaOpen, setOpcoesTelaOpen] = useState(false);

  const user = useAppSelector((state) => state.auth.user);
  const token = useAppSelector((state) => state.auth.token);
  const orgId = user?.colaboradorOrg?.orgId ?? null;

  const [cadastrarTalentoModalOpen, setCadastrarTalentoModalOpen] = useState(false);
  const [inscreverTalentoModalOpen, setInscreverTalentoModalOpen] = useState(false);
  const [preferenciasModalOpen, setPreferenciasModalOpen] = useState(false);
  const [codigoColaboradorInscrito, setCodigoColaboradorInscrito] = useState<string | null>(null);
  const [editarDadosPessoais, setEditarDadosPessoais] = useState<{ codigoColaborador: string; nome?: string } | null>(null);
  const [comentarioJornada, setComentarioJornada] = useState<{ candidaturaId: string; nome?: string } | null>(null);

  const OPCOES_TELA_BASE = useMemo(
    () => [
      {
        value: 'instrucoes-contratacao',
        label: 'Instruções de Contratação',
        url: 'https://foursys-my.sharepoint.com/personal/marcelo_nogueira_foursys_com_br/_layouts/15/Doc.aspx?sourcedoc={72574272-ca02-4221-ba86-02a128bd704b}&action=edit&wd=target%28Fluxo%20inicial.one%7C96df19a3-9277-4b08-829a-9a74afbd4085%2fTemplate%20%5C%7C%20Publica%C3%A7%C3%A3o%7C8ac0a270-661c-4eb4-9b70-855373ebb6b5%2f%29&wdorigin=',
      },
      {
        value: 'planilha-remuneracao',
        label: 'Planilha de Remuneração',
        url: 'https://foursys-my.sharepoint.com/:x:/g/personal/silvia_taketa_foursys_com_br/Ee9EXP6O5z5KkbGu_Y6mbWUBNkFbQXEg9hNc2c1rBmn1-A?e=BEPZmI',
      },
      { value: 'inscrever-talento', label: 'Inscrever Talento' },
      { value: 'cadastrar-talento', label: 'Cadastrar Talento' },
    ],
    []
  );

  /** Oculta "Instruções de Contratação" e "Planilha de Remuneração" quando orgId do usuário for 9. */
  const OPCOES_TELA = useMemo(() => {
    if (orgId === 9) {
      return OPCOES_TELA_BASE.filter((op) => op.value !== 'instrucoes-contratacao' && op.value !== 'planilha-remuneracao');
    }
    return OPCOES_TELA_BASE;
  }, [orgId, OPCOES_TELA_BASE]);

  const handleOpcaoTelaChange = useCallback((value: string) => {
    setOpcoesTelaOpen(false);
    const op = OPCOES_TELA_BASE.find((o) => o.value === value);
    if (op && 'url' in op && op.url) {
      window.open(op.url, '_blank', 'noopener,noreferrer');
      return;
    }
    if (value === 'cadastrar-talento') {
      setCadastrarTalentoModalOpen(true);
      return;
    }
    if (value === 'inscrever-talento') {
      setInscreverTalentoModalOpen(true);
      return;
    }
    toast.info('Funcionalidade em breve.');
  }, [OPCOES_TELA_BASE]);

  const handleCadastrarTalentoSuccess = useCallback((codigoColaborador: string) => {
    setCadastrarTalentoModalOpen(false);
    setCodigoColaboradorInscrito(codigoColaborador);
    setPreferenciasModalOpen(true);
  }, []);

  const handlePreferenciasClose = useCallback((open: boolean) => {
    setPreferenciasModalOpen(open);
    if (!open) {
      setCodigoColaboradorInscrito(null);
    }
  }, []);

  const handleEditarDadosPessoais = useCallback((params: { codigoColaborador: string; nome?: string }) => {
    if (!params.codigoColaborador) return;
    setEditarDadosPessoais(params);
  }, []);

  const handleComentarJornada = useCallback((params: { candidaturaId: string; nome?: string }) => {
    if (!params.candidaturaId) return;
    setComentarioJornada(params);
  }, []);

  /** Set de identificadores dos candidatos já inscritos nesta vaga (para o modal Inscrever Talento). */
  const codigosInscritosNaVaga = useMemo(() => {
    const set = new Set<string>();
    for (const c of candidatos ?? []) {
      const cod = c.codigo;
      if (cod != null && String(cod).trim() !== '') set.add(String(cod).trim());
      const interno = (c as { retornoMatch?: { codigoInternoColaborador?: string } }).retornoMatch?.codigoInternoColaborador;
      if (interno != null && String(interno).trim() !== '') set.add(String(interno).trim());
    }
    return set;
  }, [candidatos]);

  const handleKanbanHeaderMouseDown = useCallback(
    (e: React.MouseEvent) => {
      if (e.button !== 0 || !kanbanScrollRef.current) return;
      if ((e.target as HTMLElement).closest('button')) return;
      e.preventDefault();
      kanbanDragStartX.current = e.clientX;
      kanbanDragStartScroll.current = kanbanScrollRef.current.scrollLeft;
      setIsKanbanDragging(true);
    },
    []
  );

  const handleKanbanDragMove = useCallback((e: MouseEvent) => {
    if (!kanbanScrollRef.current) return;
    const dx = kanbanDragStartX.current - e.clientX;
    kanbanScrollRef.current.scrollLeft = kanbanDragStartScroll.current + dx;
  }, []);

  const handleKanbanDragEnd = useCallback(() => {
    setIsKanbanDragging(false);
    document.removeEventListener('mousemove', handleKanbanDragMove);
    document.removeEventListener('mouseup', handleKanbanDragEnd);
    document.body.style.cursor = '';
    document.body.style.userSelect = '';
  }, [handleKanbanDragMove]);

  const attachKanbanDragListeners = useCallback(() => {
    document.addEventListener('mousemove', handleKanbanDragMove);
    document.addEventListener('mouseup', handleKanbanDragEnd);
    document.body.style.cursor = 'grabbing';
    document.body.style.userSelect = 'none';
  }, [handleKanbanDragMove, handleKanbanDragEnd]);

  const handleKanbanHeaderDragStart = useCallback(
    (e: React.MouseEvent) => {
      handleKanbanHeaderMouseDown(e);
      if (e.button === 0 && !(e.target as HTMLElement).closest('button')) attachKanbanDragListeners();
    },
    [handleKanbanHeaderMouseDown, attachKanbanDragListeners]
  );

  /** Arrastar no body/background das colunas (não inicia se o clique for em um card). */
  const handleKanbanBodyDragStart = useCallback(
    (e: React.MouseEvent) => {
      if (e.button !== 0 || !kanbanScrollRef.current) return;
      if ((e.target as HTMLElement).closest('[data-kanban-card]')) return;
      e.preventDefault();
      kanbanDragStartX.current = e.clientX;
      kanbanDragStartScroll.current = kanbanScrollRef.current.scrollLeft;
      setIsKanbanDragging(true);
      attachKanbanDragListeners();
    },
    [attachKanbanDragListeners]
  );

  /** Mescla detalhes da API com dados do card (vagaRaw) para alinhar ao modal "Dados sobre a vaga" do kanban de vagas. */
  const vagaParaInfo = useMemo(() => {
    const detalhes = (vagaDetalhes ?? {}) as Record<string, unknown>;
    const raw = (vagaRaw ?? {}) as Record<string, unknown>;
    return { ...detalhes, ...raw };
  }, [vagaDetalhes, vagaRaw]);

  const vagaInfoLeft = useMemo(
    () => [
      { label: 'Código', value: firstNonEmpty(vagaParaInfo, ['codigo', 'numeroVagaCliente', 'codigoVaga']) || (vagaId ?? 'Não informado') },
      {
        label: 'Cliente',
        value:
          [firstNonEmpty(vagaParaInfo, ['nomeCliente']), firstNonEmpty(vagaParaInfo, ['unidadeCliente', 'nomeUnidade', 'unidade'])].filter(Boolean).join(' - ') ||
          'Não informado',
      },
      { label: 'Gestor', value: firstNonEmpty(vagaParaInfo, ['nomeGestor', 'nomeGestorExterno', 'gestorNome']) || 'Não informado' },
      { label: 'Abertura em', value: formatDatePtBr(firstNonEmpty(vagaParaInfo, ['dataCriacao', 'dataAbertura']), 'Não informado') },
      { label: 'Contratação em', value: formatDatePtBr(firstNonEmpty(vagaParaInfo, ['dataEnd', 'dataContratacao', 'dataUltimaAlteracao']), 'Não informado') },
      { label: 'Proposta', value: firstNonEmpty(vagaParaInfo, ['propostaCrm', 'proposta']) || 'Não informado' },
      {
        label: 'Tipo de vaga',
        value: getDescricaoById(tiposVagaList, vagaParaInfo.tipoVagaId as string | undefined) || firstNonEmpty(vagaParaInfo, ['tipoVagaDescricao', 'tipoVaga']) || 'Não informado',
      },
      { label: 'Observações internas', value: firstNonEmpty(vagaParaInfo, ['observacoesInternas']) || 'Não informado' },
    ],
    [vagaParaInfo, vagaId, tiposVagaList]
  );

  const vagaInfoRight = useMemo(
    () => [
      {
        label: 'Tipo de contratação',
        value: getDescricaoById(tiposContratacaoList, vagaParaInfo.tipoContratacaoId as number | string | undefined) || firstNonEmpty(vagaParaInfo, ['tipoContratacaoDescricao', 'tipoContratacao']) || 'Não informado',
      },
      { label: 'Custo', value: formatCurrencyBRL(vagaParaInfo.custoProfissional ?? firstNonEmpty(vagaParaInfo, ['custoProfissional'])) },
      { label: 'Modelo de trabalho', value: firstNonEmpty(vagaParaInfo, ['modeloTrabalhoDescricao', 'modeloTrabalho']) || 'Não informado' },
      {
        label: 'Localização',
        value:
          [firstNonEmpty(vagaParaInfo, ['cidade']), firstNonEmpty(vagaParaInfo, ['estado'])].filter(Boolean).join(' / ') || 'Não informado',
      },
      {
        label: 'Frequência',
        value: (() => {
          const v = formatFrequenciaPresencial(vagaParaInfo.frequencia as string | number | undefined);
          return v === '—' ? 'Não informado' : v;
        })(),
      },
      {
        label: 'Unidade',
        value: getDescricaoById(unidadesList, vagaParaInfo.unidadeId as string | undefined) || firstNonEmpty(vagaParaInfo, ['unidadeDescricao', 'unidade']) || 'Não informado',
      },
    ],
    [vagaParaInfo, tiposContratacaoList, unidadesList]
  );

  const totalInscritos = totais?.totalInscritos ?? 0;
  const totalAprovados = totais?.totalAprovados ?? 0;
  const totalReprovados = totais?.totalReprovados ?? 0;
  const totalDeclinados = totais?.totalDeclinados ?? 0;

  if (!vagaId) {
    return (
      <div className="w-full max-w-none mx-auto p-4 space-y-4">
        <PageBreadcrumb
          items={[
            { label: 'Recrutamento', href: '/recrutamento' },
            { label: 'Candidatos' },
          ]}
        />
        <Card>
          <CardContent className="p-8 text-center">
            <p className="text-muted-foreground">
              Nenhuma vaga selecionada. Acesse o Recrutamento e clique no ícone de visualizar (olho) em um card de
              vaga para abrir o painel de candidatos.
            </p>
            <Button className="mt-4" variant="outline" onClick={() => navigate('/recrutamento')}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Voltar para Recrutamento
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <TooltipProvider>
      <div className="w-full max-w-none mx-auto p-4 space-y-4">
        <PageBreadcrumb
          items={[
            { label: 'Recrutamento', href: '/recrutamento' },
            { label: `Candidatos - ${vagaTitle}` },
          ]}
        />

        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={() => navigate('/recrutamento')} aria-label="Voltar">
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <PageHeader
            title={`Candidatos - ${vagaTitle}`}
            description="Lista dos candidatos das vagas criadas neste cliente no período."
            actions={
              <div className="flex items-center gap-2">
                <Tooltip>
                <TooltipTrigger asChild>
                  <Button variant="ghost" size="icon" aria-label="Informações">
                    <Info className="h-5 w-5 text-muted-foreground" />
                  </Button>
                </TooltipTrigger>
                <TooltipContent side="bottom">Informações sobre a vaga e candidatos</TooltipContent>
              </Tooltip>
            </div>
          }
          />
        </div>

        {/* Informações sobre a vaga - colapsável */}
        <Card className="border-borderSoft">
          <button
            type="button"
            className="w-full flex items-center justify-between p-4 text-left hover:bg-muted/30 rounded-t-lg transition-colors"
            onClick={() => setInfoVagaExpanded((e) => !e)}
            aria-expanded={infoVagaExpanded}
          >
            <span className="font-semibold text-foreground">Informações sobre a vaga</span>
            {infoVagaExpanded ? (
              <ChevronUp className="h-4 w-4 text-muted-foreground" />
            ) : (
              <ChevronDown className="h-4 w-4 text-muted-foreground" />
            )}
          </button>
          {infoVagaExpanded && (
            <CardContent className="pt-0 pb-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-x-10 gap-y-3 mb-6">
                <div className="space-y-2">
                  {vagaInfoLeft.map((item) => (
                    <div key={item.label} className="flex flex-col">
                      <span className="text-xs text-muted-foreground">{item.label}</span>
                      <span className="text-sm font-medium">{item.value}</span>
                    </div>
                  ))}
                </div>
                <div className="space-y-2">
                  {vagaInfoRight.map((item) => (
                    <div key={item.label} className="flex flex-col">
                      <span className="text-xs text-muted-foreground">{item.label}</span>
                      <span className="text-sm font-medium">{item.value}</span>
                    </div>
                  ))}
                </div>
              </div>
              {/* Descrição da vaga: mesma informação exibida no modal "Dados sobre a vaga" do kanban de vagas */}
              <div className="w-full mt-4 pt-4 border-t border-borderSoft">
                <span className="text-xs text-muted-foreground block mb-2">Descrição</span>
                <div className="text-xs font-medium text-foreground rounded-md bg-surfaceSubtle/50 border border-borderSoft p-4 w-full max-h-[300px] overflow-auto whitespace-pre-wrap break-words">
                  {formatDescricaoVaga((vagaParaInfo.descricao ?? vagaDetalhes?.descricao) as string | null | undefined)}
                </div>
              </div>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4 items-stretch mt-6">
                <Card className="border-borderSoft bg-surfaceSubtle h-full flex flex-col">
                  <CardContent className="p-4 flex flex-col flex-1 text-center">
                    <p className="text-xs text-muted-foreground">Total de Inscritos</p>
                    <div className="flex-1 min-h-2" />
                    <p className="text-2xl font-semibold text-foreground">{totalInscritos}</p>
                  </CardContent>
                </Card>
                <Card className="border-borderSoft bg-surfaceSubtle h-full flex flex-col">
                  <CardContent className="p-4 flex flex-col flex-1 text-center">
                    <p className="text-xs text-muted-foreground">Candidatos Aprovados</p>
                    <div className="flex-1 min-h-2" />
                    <p className="text-2xl font-semibold text-foreground">{totalAprovados}</p>
                  </CardContent>
                </Card>
                <Card className="border-borderSoft bg-surfaceSubtle h-full flex flex-col">
                  <CardContent className="p-4 flex flex-col flex-1 text-center">
                    <p className="text-xs text-muted-foreground">Candidatos Reprovados</p>
                    <div className="flex-1 min-h-2" />
                    <p className="text-2xl font-semibold text-foreground">{totalReprovados}</p>
                  </CardContent>
                </Card>
                <Card className="border-borderSoft bg-surfaceSubtle h-full flex flex-col">
                  <CardContent className="p-4 flex flex-col flex-1 text-center">
                    <p className="text-xs text-muted-foreground">Declinados</p>
                    <div className="flex-1 min-h-2" />
                    <p className="text-2xl font-semibold text-foreground">{totalDeclinados}</p>
                  </CardContent>
                </Card>
              </div>
            </CardContent>
          )}
        </Card>

        {/* Barra de busca e ações */}
        <Card className="border-borderSoft">
          <CardContent className="p-4">
            <div className="flex flex-col sm:flex-row gap-4 items-stretch sm:items-center">
              <div className="flex-1 flex gap-2">
                <div className="relative flex-1">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    placeholder="Buscar Nome ou Desc.Status"
                    value={busca}
                    onChange={(e) => setBusca(e.target.value)}
                    className="pl-10"
                  />
                </div>
                <Button variant="outline" size="icon" aria-label="Buscar">
                  <Search className="h-4 w-4" />
                </Button>
              </div>
              <Button
                className="bg-primary text-primary-foreground"
                onClick={() => navigate('/recrutamento/gerar-match', { state: { vagaId, vagaTitle, vagaRaw } })}
              >
                <Sparkles className="mr-2 h-4 w-4" />
                Gerar match
              </Button>
            </div>
          </CardContent>
        </Card>

        {/* Card Candidatos: primeira coluna = Aderentes e Qualificados, demais = status do Kanban */}
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between gap-4 mb-4">
              <h2 className="text-lg font-semibold">Candidatos</h2>
              <Popover open={opcoesTelaOpen} onOpenChange={setOpcoesTelaOpen}>
                <PopoverTrigger asChild>
                  <Button variant="outline" className="w-[200px] justify-between">
                    <span>Opções de tela</span>
                    <ChevronDown className="h-4 w-4 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent align="end" className="w-[240px] p-0" alignOffset={4}>
                  <div className="py-1">
                    {OPCOES_TELA.map((op) => (
                      <div
                        key={op.value}
                        className="flex items-center gap-2 px-3 py-2 hover:bg-muted/50 cursor-pointer rounded-sm mx-1 text-sm"
                        onClick={() => handleOpcaoTelaChange(op.value)}
                        onKeyDown={(e) => {
                          if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault();
                            handleOpcaoTelaChange(op.value);
                          }
                        }}
                        role="menuitem"
                        tabIndex={0}
                      >
                        <span className="text-sm flex-1">{op.label}</span>
                      </div>
                    ))}
                  </div>
                </PopoverContent>
              </Popover>
            </div>
            {loading ? (
              <div className="flex flex-col items-center justify-center gap-2 py-12 text-muted-foreground">
                <Spinner size={24} aria-hidden />
                <span>Carregando candidatos...</span>
              </div>
            ) : (
              <div
                ref={kanbanScrollRef}
                className={cn('flex gap-4 overflow-x-auto pb-2 select-none', isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab')}
                onMouseDown={handleKanbanBodyDragStart}
              >
                {/* Primeira coluna: Aderentes e Qualificados (mesmo nível das colunas de status) */}
                <div className={cn('min-w-[320px] w-[320px] flex-shrink-0')}>
                  <div className="rounded-md border border-borderSoft bg-background">
                    <div
                      role="button"
                      tabIndex={0}
                      className={cn(
                        'px-4 py-3 border-b border-primary/30 bg-primary text-primary-foreground flex items-center justify-between rounded-t-md select-none',
                        isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab'
                      )}
                      onMouseDown={handleKanbanHeaderDragStart}
                    >
                      <span className="font-semibold">Aderentes e Qualificados</span>
                      <div className="flex items-center gap-2">
                        <span className="text-sm text-primary-foreground/90">{aderentes.length}</span>
                        <Sheet open={filtrosAderentesOpen} onOpenChange={setFiltrosAderentesOpen}>
                          <Tooltip>
                            <TooltipTrigger asChild>
                              <Button
                                variant="ghost"
                                size="icon"
                                className="h-11 w-11 shrink-0 text-primary-foreground hover:bg-primary-foreground/20"
                                aria-label="Filtros avançados"
                                onClick={() => setFiltrosAderentesOpen(true)}
                              >
                                <SlidersHorizontal className="h-5 w-5" />
                              </Button>
                            </TooltipTrigger>
                            <TooltipContent side="left">Filtros avançados</TooltipContent>
                          </Tooltip>
                          <SheetContent side="right" className="w-full sm:max-w-md overflow-y-auto">
                            <SheetHeader>
                              <SheetTitle className="flex items-center gap-2">
                                <SlidersHorizontal className="h-5 w-5" />
                                Filtros avançados
                              </SheetTitle>
                            </SheetHeader>
                            <div className="space-y-4 py-4">
                              <div className="space-y-2">
                                <Label>Ver resultados</Label>
                                <Select
                                  value={String(filtrosDraft.limite ?? 20)}
                                  onValueChange={(v) => setFiltrosDraft((p) => ({ ...p, limite: Number(v) }))}
                                >
                                  <SelectTrigger>
                                    <SelectValue />
                                  </SelectTrigger>
                                  <SelectContent>
                                    {LIMITE_OPCOES.map((o) => (
                                      <SelectItem key={o.value} value={String(o.value)}>
                                        {o.label}
                                      </SelectItem>
                                    ))}
                                  </SelectContent>
                                </Select>
                              </div>
                              <hr className="border-borderSoft" />
                              <div className="space-y-2">
                                <Label>Selecione a origem</Label>
                                <div className="space-y-2">
                                  {origensColaborador.filter((o) => o.ativo).map((origem) => (
                                    <label key={origem.id} className="flex items-center gap-2 text-sm">
                                      <Checkbox
                                        checked={(filtrosDraft.origens ?? []).includes(origem.id)}
                                        onCheckedChange={(checked) => {
                                          setFiltrosDraft((p) => ({
                                            ...p,
                                            origens: checked
                                              ? [...(p.origens ?? []), origem.id]
                                              : (p.origens ?? []).filter((x) => x !== origem.id),
                                          }));
                                        }}
                                      />
                                      {origem.descricao}
                                    </label>
                                  ))}
                                </div>
                              </div>
                              <hr className="border-borderSoft" />
                              <div className="space-y-2">
                                <Label>Filtrar por CEP</Label>
                                <Input
                                  placeholder="00000-000"
                                  value={cepFiltro}
                                  onChange={(e) => setCepFiltro(e.target.value.replace(/\D/g, '').slice(0, 8).replace(/(\d{5})(\d)/, '$1-$2'))}
                                  maxLength={9}
                                />
                                <Button
                                  type="button"
                                  variant="outline"
                                  size="sm"
                                  disabled={loadingCep || cepFiltro.replace(/\D/g, '').length !== 8}
                                  onClick={() => buscarCep(cepFiltro.replace(/\D/g, ''))}
                                >
                                  {loadingCep ? 'Buscando...' : 'Buscar CEP'}
                                </Button>
                                {(filtrosDraft.localizacaoCidade || filtrosDraft.localizacaoEstado) && (
                                  <p className="text-xs text-muted-foreground">
                                    {filtrosDraft.localizacaoCidade}, {filtrosDraft.localizacaoEstado}
                                  </p>
                                )}
                              </div>
                              <hr className="border-borderSoft" />
                              <label className="flex items-center gap-2 text-sm">
                                <Checkbox
                                  checked={filtrosDraft.qualificados ?? false}
                                  onCheckedChange={(checked) => setFiltrosDraft((p) => ({ ...p, qualificados: !!checked }))}
                                />
                                Mostrar apenas Qualificados
                              </label>
                              <hr className="border-borderSoft" />
                              <div className="space-y-2">
                                <Label>Última alteração em (dias)</Label>
                                <div className="flex items-center gap-2">
                                  <Button
                                    type="button"
                                    variant="outline"
                                    size="icon"
                                    className="h-9 w-9 shrink-0"
                                    onClick={() =>
                                      setFiltrosDraft((p) => ({
                                        ...p,
                                        diasUltimaAlteracao: Math.max(0, (p.diasUltimaAlteracao ?? 30) - 1),
                                      }))
                                    }
                                  >
                                    <Minus className="h-4 w-4" />
                                  </Button>
                                  <span className="min-w-[2.5rem] text-center font-medium">
                                    {filtrosDraft.diasUltimaAlteracao ?? 30}
                                  </span>
                                  <Button
                                    type="button"
                                    variant="outline"
                                    size="icon"
                                    className="h-9 w-9 shrink-0"
                                    onClick={() =>
                                      setFiltrosDraft((p) => ({
                                        ...p,
                                        diasUltimaAlteracao: (p.diasUltimaAlteracao ?? 30) + 1,
                                      }))
                                    }
                                  >
                                    <Plus className="h-4 w-4" />
                                  </Button>
                                  <span className="text-sm text-muted-foreground">dias</span>
                                </div>
                              </div>
                              <hr className="border-borderSoft" />
                              {[
                                { key: 'pesoHardSkills' as const, label: 'Hardskills' },
                                { key: 'pesoSoftSkills' as const, label: 'Softskills' },
                                { key: 'pesoMetodologias' as const, label: 'Metodologias' },
                                { key: 'pesoDominiosNegocio' as const, label: 'Domínios de negócio' },
                                { key: 'pesoIdiomas' as const, label: 'Idiomas' },
                                { key: 'pesoDisponibilidades' as const, label: 'Disponibilidades' },
                              ].map(({ key, label }) => (
                                <div key={key} className="space-y-2">
                                  <div className="flex justify-between text-xs">
                                    <span>{label}</span>
                                    <span>{(filtrosDraft[key] ?? 1).toFixed(2)}</span>
                                  </div>
                                  <Slider
                                    min={0}
                                    max={1}
                                    step={0.1}
                                    value={[filtrosDraft[key] ?? 1]}
                                    onValueChange={([v]) => setFiltrosDraft((p) => ({ ...p, [key]: v }))}
                                  />
                                </div>
                              ))}
                              <hr className="border-borderSoft" />
                              <div className="flex gap-2">
                                <Button
                                  className="flex-1"
                                  onClick={() => {
                                    setFiltrosAderentes(filtrosDraft);
                                    setFiltrosAderentesOpen(false);
                                  }}
                                >
                                  Filtrar
                                </Button>
                                <Button
                                  variant="outline"
                                  onClick={() => {
                                    limparFiltrosAderentes();
                                    setFiltrosDraft(DEFAULT_FILTROS_ADERENTES);
                                    setCepFiltro('');
                                  }}
                                >
                                  Limpar
                                </Button>
                              </div>
                            </div>
                          </SheetContent>
                        </Sheet>
                      </div>
                    </div>
                    <div
                      className={cn('p-3 space-y-3 max-h-[70vh] overflow-auto min-h-[120px]', isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab')}
                      onMouseDown={handleKanbanBodyDragStart}
                    >
                      {loadingAderentes ? (
                        <div className="flex flex-col items-center justify-center gap-2 py-8 text-muted-foreground text-sm">
                          <Spinner size={20} aria-hidden />
                          <span>Carregando aderentes...</span>
                        </div>
                      ) : aderentes.length > 0 ? (
                        aderentes.map((item) => (
                          <AderenteCard
                            key={item.codigo ?? item.nome ?? String(Math.random())}
                            item={item}
                            vagaId={vagaId ?? ''}
                            vagaTitle={vagaTitle}
                            vagaRaw={vagaRaw}
                            onInscrever={inscreverCandidato}
                            onEditarDadosPessoais={handleEditarDadosPessoais}
                          />
                        ))
                      ) : (
                        <div className="text-center py-8 text-muted-foreground text-sm">Sem candidatos aderentes</div>
                      )}
                    </div>
                  </div>
                </div>

                {/* Colunas de status (inscritos por fase) */}
                {statusCandidatura.map((status) => {
                  const lista = candidatosPorStatus[status.id] ?? [];
                  return (
                    <div key={status.id} className={cn('min-w-[320px] w-[320px] flex-shrink-0')}>
                      <div className="rounded-md border border-borderSoft bg-background">
                        <div
                          role="button"
                          tabIndex={0}
                          className={cn(
                            'px-4 py-3 border-b border-primary/30 bg-primary text-primary-foreground flex items-center justify-between rounded-t-md select-none',
                            isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab'
                          )}
                          onMouseDown={handleKanbanHeaderDragStart}
                        >
                          <div className="font-semibold">{status.nome}</div>
                          <div className="text-sm text-primary-foreground/90">{lista.length}</div>
                        </div>
                        <div
                          className={cn('p-3 space-y-3 max-h-[70vh] overflow-auto min-h-[120px]', isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab')}
                          onMouseDown={handleKanbanBodyDragStart}
                          data-status-id={status.id}
                          onDragOver={(e) => {
                            e.preventDefault();
                            e.dataTransfer.dropEffect = 'move';
                          }}
                          onDrop={(e) => {
                            e.preventDefault();
                            const statusTo = e.currentTarget.getAttribute('data-status-id');
                            const payload = dragPayloadRef.current;
                            if (statusTo && payload && payload.statusFrom !== statusTo) {
                              handleStatusSelect(payload.idCandidatura, statusTo);
                              dragPayloadRef.current = null;
                            }
                          }}
                        >
                          {lista.length > 0 ? (
                            lista.map((c) => (
                              <CandidatoCard
                                key={c.id ?? c.idCandidatura ?? String(Math.random())}
                                candidato={c}
                                statusCandidatura={statusCandidatura}
                                vagaId={vagaId}
                                vagaTitle={vagaTitle}
                                vagaRaw={vagaRaw}
                                statusId={status.id}
                                onStatusSelect={handleStatusSelect}
                                onDragStartCard={(idCandidatura, statusFrom) => {
                                  dragPayloadRef.current = { idCandidatura, statusFrom };
                                }}
                                onAtribuirRecrutador={(cand) =>
                                  setAtribuirRecrutadorCandidato({
                                    idCandidatura: cand.idCandidatura ?? '',
                                    recrutadorNome: cand.recrutadorResponsavel ?? null,
                                  })
                                }
                                onEditarDadosPessoais={handleEditarDadosPessoais}
                                onComentarJornada={handleComentarJornada}
                              />
                            ))
                          ) : (
                            <div className="flex flex-col items-center justify-center py-8 text-center text-sm text-muted-foreground">
                              <span className="text-4xl text-muted-foreground/50">0</span>
                              <span>Candidatos nesta fase</span>
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
      <InscreverCandidatoModal
        open={cadastrarTalentoModalOpen}
        onOpenChange={setCadastrarTalentoModalOpen}
        token={token}
        onSuccess={handleCadastrarTalentoSuccess}
      />
      <InscreverTalentoModal
        open={inscreverTalentoModalOpen}
        onOpenChange={setInscreverTalentoModalOpen}
        token={token}
        onInscrever={inscreverCandidato}
        codigosInscritosNaVaga={codigosInscritosNaVaga}
        vagaId={vagaId}
        vagaTitle={vagaTitle}
        vagaRaw={vagaRaw}
      />
      <AlterarStatusCandidatoModal
        open={alterarStatusModalOpen}
        onOpenChange={(open) => {
          setAlterarStatusModalOpen(open);
          if (!open) {
            setAlterarStatusId('');
            setAlterarStatusCodigo(0);
          }
        }}
        token={token}
        idCandidatura={alterarStatusId}
        codigoStatus={alterarStatusCodigo}
        onSuccess={(retorno) => {
          setLastMudarStatusResponse(retorno ?? null);
          refetch();
          setAlterarStatusModalOpen(false);
          setAlterarStatusId('');
          setAlterarStatusCodigo(0);
        }}
      />
      <AtribuirRecrutadorCandidatoModal
        open={atribuirRecrutadorCandidato != null}
        onOpenChange={(open) => {
          if (!open) setAtribuirRecrutadorCandidato(null);
        }}
        idCandidatura={atribuirRecrutadorCandidato?.idCandidatura ?? ''}
        currentRecrutadorNome={atribuirRecrutadorCandidato?.recrutadorNome ?? null}
        token={token}
        onSuccess={() => {
          setAtribuirRecrutadorCandidato(null);
          refetch();
        }}
      />
      <EditarDadosPessoaisModal
        open={editarDadosPessoais != null}
        onOpenChange={(open) => {
          if (!open) setEditarDadosPessoais(null);
        }}
        token={token}
        codigoInternoColaborador={editarDadosPessoais?.codigoColaborador ?? null}
        nomeCandidato={editarDadosPessoais?.nome}
      />
      <ComentarioJornadaModal
        open={comentarioJornada != null}
        onOpenChange={(open) => {
          if (!open) setComentarioJornada(null);
        }}
        token={token}
        candidaturaId={comentarioJornada?.candidaturaId ?? ''}
        nomeCandidato={comentarioJornada?.nome}
      />
      {vagaId && codigoColaboradorInscrito && (
        <InscricaoPreferenciasModal
          open={preferenciasModalOpen}
          onOpenChange={handlePreferenciasClose}
          token={token}
          codigoVaga={
            typeof vagaDetalhes?.codigo === 'number'
              ? vagaDetalhes.codigo
              : Number((vagaRaw as Record<string, unknown>)?.codigo ?? vagaId) || 0
          }
          vagaId={vagaId}
          vagaTitle={vagaTitle ?? ''}
          vagaRaw={vagaRaw as Record<string, unknown>}
          codigoColaborador={codigoColaboradorInscrito}
          onVerCandidaturas={() => handlePreferenciasClose(false)}
          onInscricaoConcluida={refetch}
        />
      )}
    </TooltipProvider>
  );
}
