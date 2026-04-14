/**
 * Match de Talentos (Beta) – Comparação Motor Atual (API) x Motor Beta (Labs).
 * Avaliação por coluna: "Prefiro essa resposta". Ver Perfil abre currículo na sidebar.
 */

import { useState, useMemo } from 'react';
import { useAppSelector } from '@app/store/hooks';
import { PageBreadcrumb } from '@presentation/components/common';
import { AdherenceDetailsModal } from '@presentation/components/common/AdherenceDetailsModal';
import { useMatchTalentos } from '@presentation/hooks/useMatchTalentos';
import type { MatchCardItem, MatchEngine } from '@shared/types/matchTalentos';
import { retornoMatchToMinhaJornadaAdherence } from '@domain/adapters/retornoMatchToAdherence';
import type { RetornoMatchRaw } from '@domain/entities/GestaoVagasCandidatos';
import { Search, ThumbsUp, BarChart3, Info, Briefcase, Eye, Check, Settings2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Textarea } from '@/components/ui/textarea';
import { Spinner } from '@/components/ui/spinner';
import { Badge } from '@/components/ui/badge';
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip';
import { CurriculoSidebar } from '@presentation/components/profile/CurriculoSidebar';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';

const ENGINE_LABEL: Record<MatchEngine, string> = {
  actual: 'Motor Atual',
  beta: 'Motor Beta (IA)',
};

const PREFERENCIA_CANDIDATE_ID = 'preferencia-coluna';

function buildAdherence(item: MatchCardItem) {
  const raw = item.retornoMatch as RetornoMatchRaw | null | undefined;
  if (!raw) return null;
  const withCamel = {
    ...raw,
    codigoInternoColaborador: (raw as { codigo_interno_colaborador?: string }).codigo_interno_colaborador ?? item.codigoInternoColaborador,
  };
  return retornoMatchToMinhaJornadaAdherence(withCamel, item.nome);
}

export function MatchTalentosPage() {
  const { token, user } = useAppSelector((state) => state.auth);
  const recruiterName = user?.nomeColaborador ?? user?.colaborador?.nomeCompleto ?? 'Recrutador';

  const {
    query,
    setQuery,
    resultsActual,
    resultsBeta,
    loading,
    loadingActual,
    loadingBeta,
    error,
    runSearch,
    feedbacks,
    addFeedback,
    stats,
    idLogMatchSemantico,
    idLogRankCandidatesIds,
    mensagemUsuarioMotorAtual,
    errorActual,
    errorBeta,
    preferenciaRegistrada,
    registrarFeedbackPreferencia,
    botoesPreferenciaHabilitados,
  } = useMatchTalentos(token);

  const [perfilSheetOpen, setPerfilSheetOpen] = useState(false);
  const [perfilItem, setPerfilItem] = useState<MatchCardItem | null>(null);
  const [adherenceModalOpen, setAdherenceModalOpen] = useState(false);
  const [adherenceItem, setAdherenceItem] = useState<MatchCardItem | null>(null);
  const [gestaoQualidadeModalOpen, setGestaoQualidadeModalOpen] = useState(false);
  const [parecerModalOpen, setParecerModalOpen] = useState(false);
  const [parecerItem, setParecerItem] = useState<MatchCardItem | null>(null);

  const hasResults = resultsActual.length > 0 || resultsBeta.length > 0;
  const showComparison = hasResults || loadingActual || loadingBeta;

  const preferredColumn = useMemo(() => {
    const list = feedbacks.filter(
      (f) => f.query === query && f.candidateId === PREFERENCIA_CANDIDATE_ID
    );
    if (list.length === 0) return null;
    return list[list.length - 1].engine as MatchEngine;
  }, [feedbacks, query]);

  const handlePreferColumn = (engine: MatchEngine) => {
    const idLogBeta = engine === 'beta' ? idLogMatchSemantico : null;
    const idLogActual = engine === 'actual' ? idLogRankCandidatesIds : null;
    addFeedback({
      recruiter: recruiterName,
      query,
      candidateName: ENGINE_LABEL[engine],
      candidateId: PREFERENCIA_CANDIDATE_ID,
      engine,
      rankingPosition: 0,
      isRelevant: true,
      reason: null,
      idLogMatchSemantico: idLogBeta ?? undefined,
      idLogRankCandidatesIds: idLogActual ?? undefined,
    });
    registrarFeedbackPreferencia(engine);
    if (engine === 'beta' && idLogBeta) {
      toast.success(`Você preferiu a resposta do ${ENGINE_LABEL[engine]}. idLog: ${idLogBeta}`);
    } else if (engine === 'actual' && idLogActual) {
      toast.success(`Você preferiu a resposta do ${ENGINE_LABEL[engine]}. idLog: ${idLogActual}`);
    } else {
      toast.success(`Você preferiu a resposta do ${ENGINE_LABEL[engine]}.`);
    }
  };

  const openAdherenceModal = (item: MatchCardItem) => {
    if (!item.retornoMatch) return;
    const raw = item.retornoMatch as { parecer_IA_senioridade?: { motivo_parecer?: string[] } };
    if (raw?.parecer_IA_senioridade?.motivo_parecer?.length) {
      setParecerItem(item);
      setParecerModalOpen(true);
    } else {
      setAdherenceItem(item);
      setAdherenceModalOpen(true);
    }
  };

  const adherence = useMemo(
    () => (adherenceItem ? buildAdherence(adherenceItem) : null),
    [adherenceItem]
  );

  const precisionAt3Percent = useMemo(
    () => (stats.precisionAt3 != null ? `${(stats.precisionAt3 * 100).toFixed(1)}%` : '—'),
    [stats.precisionAt3]
  );
  const precisionAt5Percent = useMemo(
    () => (stats.precisionAt5 != null ? `${(stats.precisionAt5 * 100).toFixed(1)}%` : '—'),
    [stats.precisionAt5]
  );

  return (
    <div className="min-h-screen p-4 font-sans md:p-8">
      <div className="mx-auto max-w-7xl space-y-6">
        <PageBreadcrumb items={[{ label: 'Match de Talentos' }]} />

        <header className="mb-8 flex flex-row flex-wrap items-start justify-between gap-4">
          <div className="flex items-center gap-3">
            <div className="rounded-lg bg-primary p-2 shadow-md">
              <BarChart3 className="h-7 w-7 text-primary-foreground" />
            </div>
            <div>
              <h1 className="flex items-center gap-2 text-3xl font-bold tracking-tight text-foreground">
                Match de Talentos
                <Badge variant="info" className="shrink-0">Beta</Badge>
              </h1>
              <p className="text-sm text-muted-foreground">
                Compare o match atual com o novo motor de match e avalie qual retorno ajuda mais.
              </p>
            </div>
          </div>
          <Button
            variant="outline"
            className="hidden shrink-0"
            onClick={() => setGestaoQualidadeModalOpen(true)}
            aria-hidden
          >
            <Settings2 className="mr-1.5 h-4 w-4" />
            Gestão de Qualidade
          </Button>
        </header>

        {/* Busca */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Search className="h-5 w-5" />
              Busca em linguagem natural
            </CardTitle>
            <CardDescription>
              Descreva o perfil desejado (cargo, skills, localização). A primeira coluna usa a API atual; a segunda simula o motor Beta.
            </CardDescription>
          </CardHeader>
          <CardContent className="flex flex-col gap-4 sm:flex-row sm:items-end">
            <div className="flex-1">
              <Textarea
                placeholder="Ex.: desenvolvedor backend java pleno remoto"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className="min-h-[80px] resize-y"
                disabled={loading}
              />
            </div>
            <Button onClick={() => runSearch()} disabled={!query.trim() || loading}>
              {loading ? (
                <>
                  <Spinner size={18} className="mr-2" />
                  Gerando...
                </>
              ) : (
                'Gerar match'
              )}
            </Button>
          </CardContent>
        </Card>

        {error && (
          <div className="rounded-lg border border-destructive bg-destructive/10 px-4 py-3 text-sm text-destructive">
            {error}
          </div>
        )}

        {/* Comparação lado a lado: exibe assim que a busca inicia; loading por coluna */}
        {showComparison && (
          <div className="grid gap-6 md:grid-cols-2">
            {/* Motor Atual */}
            <Card>
              <CardHeader className="flex flex-row items-start justify-between gap-3">
                <div className="min-w-0 flex-1">
                  <CardTitle>{ENGINE_LABEL.actual}</CardTitle>
                </div>
                {!loadingActual && (resultsActual.length > 0 || idLogRankCandidatesIds) && (
                  <Button
                    size="sm"
                    variant="outline"
                    disabled={!botoesPreferenciaHabilitados || preferenciaRegistrada}
                    className={cn(
                      'shrink-0',
                      preferredColumn === 'actual'
                        ? 'bg-success border-success text-white hover:bg-success/90'
                        : 'border-success text-success hover:bg-success/10',
                      preferenciaRegistrada && preferredColumn === 'actual' && 'disabled:opacity-100 disabled:!text-white'
                    )}
                    onClick={() => handlePreferColumn('actual')}
                  >
                    <ThumbsUp className="mr-1.5 h-4 w-4" />
                    Prefiro essa resposta
                  </Button>
                )}
              </CardHeader>
              <CardContent className="space-y-3">
                {loadingActual && resultsActual.length === 0 ? (
                  <div className="flex flex-col items-center justify-center py-12 text-muted-foreground">
                    <Spinner size={32} className="mb-3" />
                    <p className="text-sm">Obtendo resultado...</p>
                  </div>
                ) : errorActual && resultsActual.length === 0 ? (
                  <div className="flex flex-col items-center justify-center py-12 text-center px-4">
                    <p className="text-sm text-destructive font-medium">{errorActual}</p>
                  </div>
                ) : resultsActual.length === 0 ? (
                  <div className="flex flex-col items-center justify-center py-12 text-center px-4">
                    <p className="text-sm text-muted-foreground">
                      {mensagemUsuarioMotorAtual != null && mensagemUsuarioMotorAtual !== ''
                        ? mensagemUsuarioMotorAtual
                        : 'Nenhum talento encontrado para este prompt.'}
                    </p>
                  </div>
                ) : (
                  resultsActual.map((item) => (
                    <MatchCard
                      key={item.id}
                      item={item}
                      engine="actual"
                      position={0}
                      onVerPerfil={() => {
                        setPerfilItem(item);
                        setPerfilSheetOpen(true);
                      }}
                      onOpenAdherence={openAdherenceModal}
                    />
                  ))
                )}
              </CardContent>
            </Card>

            {/* Motor Beta (IA) */}
            <Card className={cn('ring-2 ring-accent')}>
              <CardHeader className="flex flex-row items-start justify-between gap-3">
                <div className="min-w-0 flex-1">
                  <CardTitle>{ENGINE_LABEL.beta}</CardTitle>
                </div>
                {!loadingBeta && (resultsBeta.length > 0 || idLogMatchSemantico) && (
                  <Button
                    size="sm"
                    variant="outline"
                    disabled={!botoesPreferenciaHabilitados || preferenciaRegistrada}
                    className={cn(
                      'shrink-0',
                      preferredColumn === 'beta'
                        ? 'bg-success border-success text-white hover:bg-success/90'
                        : 'border-success text-success hover:bg-success/10',
                      preferenciaRegistrada && preferredColumn === 'beta' && 'disabled:opacity-100 disabled:!text-white'
                    )}
                    onClick={() => handlePreferColumn('beta')}
                  >
                    <ThumbsUp className="mr-1.5 h-4 w-4" />
                    Prefiro essa resposta
                  </Button>
                )}
              </CardHeader>
              <CardContent className="space-y-3">
                {loadingBeta && resultsBeta.length === 0 ? (
                  <div className="flex flex-col items-center justify-center py-12 text-muted-foreground">
                    <Spinner size={32} className="mb-3" />
                    <p className="text-sm">Obtendo resultado...</p>
                  </div>
                ) : errorBeta && resultsBeta.length === 0 ? (
                  <div className="flex flex-col items-center justify-center py-12 text-center px-4">
                    <p className="text-sm text-destructive font-medium">{errorBeta}</p>
                  </div>
                ) : (
                  resultsBeta.map((item) => (
                    <MatchCard
                      key={item.id}
                      item={item}
                      engine="beta"
                      position={0}
                      onVerPerfil={() => {
                        setPerfilItem(item);
                        setPerfilSheetOpen(true);
                      }}
                      onOpenAdherence={openAdherenceModal}
                    />
                  ))
                )}
              </CardContent>
            </Card>
          </div>
        )}
      </div>

      {/* Modal Gestão de Qualidade */}
      <Dialog open={gestaoQualidadeModalOpen} onOpenChange={setGestaoQualidadeModalOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <BarChart3 className="h-5 w-5" />
              Gestão de Qualidade
            </DialogTitle>
            <DialogDescription>
              Métricas agregadas dos feedbacks (armazenados localmente).
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-4 sm:grid-cols-2 pt-2">
            <div className="rounded-lg border bg-muted/50 p-4">
              <p className="text-sm text-muted-foreground">Total de avaliações</p>
              <p className="text-2xl font-semibold">{stats.metrics.total}</p>
            </div>
            <div className="rounded-lg border bg-muted/50 p-4">
              <p className="text-sm text-muted-foreground">Relevantes (👍)</p>
              <p className="text-2xl font-semibold text-success">{stats.metrics.positive}</p>
            </div>
            <div className="rounded-lg border bg-muted/50 p-4">
              <p className="text-sm text-muted-foreground">Não relevantes (👎)</p>
              <p className="text-2xl font-semibold text-destructive">{stats.metrics.negative}</p>
            </div>
            <div className="rounded-lg border bg-muted/50 p-4">
              <p className="text-sm text-muted-foreground">Precision@3 / Precision@5</p>
              <p className="text-2xl font-semibold">
                {precisionAt3Percent} / {precisionAt5Percent}
              </p>
            </div>
          </div>
          <div className="flex flex-wrap gap-2">
            {stats.engineStats.map((es) => (
              <Badge key={es.engine} variant="secondary">
                {ENGINE_LABEL[es.engine]}: {es.positive}/{es.total} relevantes
              </Badge>
            ))}
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal parecer IA (Motor Beta) */}
      <Dialog open={parecerModalOpen} onOpenChange={setParecerModalOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Parecer IA — Senioridade</DialogTitle>
            <DialogDescription>
              {parecerItem?.nome && (
                <span className="text-foreground font-medium">{parecerItem.nome}</span>
              )}
              {' — motivos do parecer.'}
            </DialogDescription>
          </DialogHeader>
          <ul className="list-disc list-inside space-y-2 text-sm text-muted-foreground pt-2">
            {(
              (parecerItem?.retornoMatch as { parecer_IA_senioridade?: { motivo_parecer?: string[] } })
                ?.parecer_IA_senioridade?.motivo_parecer ?? []
            ).map((motivo, i) => (
              <li key={i} className="text-foreground/90">{motivo}</li>
            ))}
          </ul>
        </DialogContent>
      </Dialog>

      {/* Modal cálculo do match (aderência) - Motor Atual */}
      <AdherenceDetailsModal
        open={adherenceModalOpen}
        onOpenChange={setAdherenceModalOpen}
        adherence={adherence}
      />

      {/* Sidebar com currículo completo (mesma lógica do /curriculoProfissional) */}
      <CurriculoSidebar
        open={perfilSheetOpen}
        onOpenChange={setPerfilSheetOpen}
        identificador={perfilItem?.codigoInternoColaborador ?? null}
        token={token}
        title="Perfil 360"
      />
    </div>
  );
}

interface MatchCardProps {
  item: MatchCardItem;
  engine: MatchEngine;
  position: number;
  onVerPerfil: () => void;
  onOpenAdherence: (item: MatchCardItem) => void;
}

function MatchCard({
  item,
  engine,
  onVerPerfil,
  onOpenAdherence,
}: MatchCardProps) {
  const matchPct = item.match;
  const hasAdherence = !!item.retornoMatch;
  const parecer = (item.retornoMatch as { parecer_IA_senioridade?: { motivo_parecer?: string[] } })?.parecer_IA_senioridade;
  const motivoParecer = engine === 'beta' ? (parecer?.motivo_parecer ?? []) : [];

  const matchBadgeClass = cn(
    'inline-flex items-center gap-1 rounded-full border px-2.5 py-0.5 text-xs font-semibold',
    matchPct >= 80 && 'border-success bg-success/10 text-success',
    matchPct >= 60 && matchPct < 80 && 'border-warning bg-warning/10 text-warning',
    matchPct < 60 && 'border-destructive bg-destructive/10 text-destructive'
  );

  const badgeButton = hasAdherence ? (
    <button
      type="button"
      className={matchBadgeClass}
      onClick={() => onOpenAdherence(item)}
      aria-label="Ver detalhes do cálculo de match"
    >
      <span>{matchPct.toFixed(1)}%</span>
      <Info className="h-3.5 w-3.5 shrink-0" />
    </button>
  ) : (
    <span className={matchBadgeClass}>
      <span>{matchPct.toFixed(1)}%</span>
    </span>
  );

  const badgeWithTooltip =
    engine === 'beta' && motivoParecer.length > 0 ? (
      <TooltipProvider delayDuration={200}>
        <Tooltip>
          <TooltipTrigger asChild>{badgeButton}</TooltipTrigger>
          <TooltipContent side="bottom" className="max-w-sm">
            <p className="font-medium mb-1.5">Parecer IA — Senioridade</p>
            <ul className="list-disc list-inside space-y-1 text-xs">
              {motivoParecer.map((m, i) => (
                <li key={i}>{m}</li>
              ))}
            </ul>
          </TooltipContent>
        </Tooltip>
      </TooltipProvider>
    ) : (
      badgeButton
    );

  return (
    <div className="rounded-lg border bg-card p-3 space-y-2">
      <div className="flex items-start justify-between gap-2">
        <div className="min-w-0 flex-1">
          <p className="font-medium text-foreground truncate">{item.nome}</p>
          <div className="flex flex-wrap items-center gap-2 mt-1">
            {badgeWithTooltip}
            <Badge variant="outline" className="text-xs font-normal">
              {item.origem}
            </Badge>
          </div>
        </div>
      </div>

      {item.skills.length > 0 && (
        <ul className="flex flex-wrap gap-1.5 text-xs text-muted-foreground">
          {item.skills.slice(0, 4).map((s, i) => (
            <li key={i} className="flex items-center gap-1">
              <Check className="h-3.5 w-3.5 text-success shrink-0" />
              <span>{s.skill} — {s.nivel}</span>
            </li>
          ))}
        </ul>
      )}

      {item.resumo && (
        <p className="text-xs text-muted-foreground line-clamp-2">{item.resumo}</p>
      )}

      <div className="flex items-center justify-between gap-2 pt-1 border-t">
        <span className="flex items-center gap-1 text-xs text-muted-foreground">
          <Briefcase className="h-3.5 w-3.5" />
          {item.candidaturasLabel}
        </span>
        <Button size="sm" variant="ghost" onClick={onVerPerfil} className="shrink-0">
          <Eye className="mr-1 h-4 w-4" />
          Ver Perfil 360
        </Button>
      </div>
    </div>
  );
}
