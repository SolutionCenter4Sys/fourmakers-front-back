import { useState, useEffect, useCallback } from 'react';
import { GuideTour, TourButton, useTour, pdisMetricasSteps } from '@shared/tour';
import { useAppSelector } from '@app/store/hooks';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Eye,
  Target,
  CalendarIcon,
  FileText,
  CheckCircle,
  Clock,
  AlertCircle,
  Play,
  XCircle,
  SlidersHorizontal,
} from '@/components/ui/system-icons';
import { ColaboradorPdiApi, type PdiMetricasFiltrosParams } from '@data/api/ColaboradorPdiApi';
import type {
  PdiMetricasResultDTO,
  PdiMetricaItemDTO,
  PdiCompletoTimeDTO,
} from '@shared/types/pdiApi';
import { mapPdiStatusToLabel, PdiStatusLabel } from '@shared/types/pdiApi';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { cn } from '@/lib/utils';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { PdiTableSection } from '@/presentation/components/gestaoDesempenho/PdiTableSection';
import { container } from '@core/di/container';
import { DiTokens } from '@core/di/tokens';
import { useColaboradoresDesempenho } from '@/presentation/hooks/useGestaoDesempenho';
import { CompetenciasApi } from '@data/api/CompetenciasApi';

const api = new ColaboradorPdiApi();

const statusBadgeClass: Record<string, string> = {
  [PdiStatusLabel.NAO_INICIADO]: 'bg-amber-50 text-amber-800 border-2 border-amber-200 dark:bg-amber-950/30 dark:text-amber-300 dark:border-amber-700',
  [PdiStatusLabel.EM_ANALISE]: 'bg-blue-50 text-blue-800 border-2 border-blue-200 dark:bg-blue-950/30 dark:text-blue-300 dark:border-blue-700',
  [PdiStatusLabel.IN_PROGRESS]: 'bg-primary/10 text-primary border-2 border-primary/40 dark:bg-primary/20 dark:border-primary/50',
  [PdiStatusLabel.FINALIZADO]: 'bg-green-50 text-green-800 border-2 border-green-200 dark:bg-green-950/30 dark:text-green-300 dark:border-green-700',
  [PdiStatusLabel.CANCELADO]: 'bg-slate-50 text-slate-700 border-2 border-slate-200 dark:bg-slate-800/50 dark:text-slate-300 dark:border-slate-600',
};

interface UnidadeOption {
  id: string;
  descricao: string;
}

export default function PdisMetricasPage() {
  const { token, user } = useAppSelector((state) => state.auth);
  const orgId = user?.colaboradorOrg?.orgId ?? (typeof localStorage !== 'undefined' ? Number(localStorage.getItem('lastOrgId')) : null);

  const [metricas, setMetricas] = useState<PdiMetricasResultDTO | null>(null);
  const [loading, setLoading] = useState(false);
  const { isRunning: tourRunning, startTour, handleJoyrideCallback } = useTour();
  const [modalPdiId, setModalPdiId] = useState<string | null>(null);
  const [modalDetalhe, setModalDetalhe] = useState<PdiCompletoTimeDTO | null>(null);
  const [loadingDetalhe, setLoadingDetalhe] = useState(false);

  // Filtros (como antes)
  const [dataInicio, setDataInicio] = useState<string>('');
  const [dataFim, setDataFim] = useState<string>('');
  const [unidade, setUnidade] = useState<string>('');
  const [colaboradorFiltro, setColaboradorFiltro] = useState<string>('');
  const [filtroTodos, setFiltroTodos] = useState(true);
  const [filtroEmAndamento, setFiltroEmAndamento] = useState(true);
  const [filtroNaoIniciado, setFiltroNaoIniciado] = useState(true);
  const [filtroFinalizados, setFiltroFinalizados] = useState(true);

  const [unidades, setUnidades] = useState<UnidadeOption[]>([]);
  // Usar hook de colaboradores para listar a equipe
  const { colaboradores } = useColaboradoresDesempenho('todos', '');

  const buildFiltrosParams = useCallback((): PdiMetricasFiltrosParams | undefined => {
    const hasFilters =
      dataInicio ||
      dataFim ||
      unidade ||
      !filtroTodos ||
      !filtroEmAndamento ||
      !filtroNaoIniciado ||
      !filtroFinalizados;
    if (!hasFilters) return undefined;
    return {
      data_inicio: dataInicio || undefined,
      data_fim: dataFim || undefined,
      unidade: unidade || undefined,
      todos: filtroTodos,
      em_andamento: filtroEmAndamento,
      nao_iniciado: filtroNaoIniciado,
      finalizados: filtroFinalizados,
    };
  }, [dataInicio, dataFim, unidade, filtroTodos, filtroEmAndamento, filtroNaoIniciado, filtroFinalizados]);

  const carregarMetricas = useCallback(
    async (params?: PdiMetricasFiltrosParams) => {
      if (!token) return;
      setLoading(true);
      try {
        let data: PdiMetricasResultDTO;
        if (colaboradorFiltro) {
          data = await api.getMetricasGestorColaborador(token, colaboradorFiltro);
        } else {
          data = await api.getMetricasGestor(token, params);
        }
        setMetricas(data);
      } catch {
        setMetricas(null);
      } finally {
        setLoading(false);
      }
    },
    [token, colaboradorFiltro]
  );

  const buscarComFiltros = useCallback(() => {
    carregarMetricas(buildFiltrosParams());
  }, [carregarMetricas, buildFiltrosParams]);

  const limparFiltros = useCallback(() => {
    setDataInicio('');
    setDataFim('');
    setUnidade('');
    setColaboradorFiltro('');
    setFiltroTodos(true);
    setFiltroEmAndamento(true);
    setFiltroNaoIniciado(true);
    setFiltroFinalizados(true);
    // Como depende do state, melhor limpar e disparar um carregarMetricas manual aqui ou confiar no useEffect
  }, []);

  // Quando os filtros mudarem, carregar métricas (incluindo limparFiltros)
  useEffect(() => {
    carregarMetricas(buildFiltrosParams());
  }, [carregarMetricas, buildFiltrosParams]);

  // Carregar unidades (por orgId)
  useEffect(() => {
    if (!token || orgId == null || !Number.isInteger(orgId)) return;
    const competenciasApi = container.resolve<CompetenciasApi>(DiTokens.competenciasApi);
    competenciasApi
      .listarUnidadesComDefaultPorOrgId(token, orgId)
      .then((res) => {
        const list = res?.ListaUnidadesResult ?? [];
        setUnidades(Array.isArray(list) ? list.map((u) => ({ id: String(u.id), descricao: u.descricao ?? '' })) : []);
      })
      .catch(() => setUnidades([]));
  }, [token, orgId]);


  const abrirModalDetalhe = async (pdiId: string) => {
    if (!token) return;
    setModalPdiId(pdiId);
    setModalDetalhe(null);
    setLoadingDetalhe(true);
    try {
      const detalhe = await api.getPdiTimeById(token, pdiId);
      setModalDetalhe(detalhe);
    } catch {
      setModalDetalhe(null);
    } finally {
      setLoadingDetalhe(false);
    }
  };

  const bigNumbersConfig = [
    { label: PdiStatusLabel.NAO_INICIADO, value: metricas?.bigNumbers?.naoIniciado ?? 0, icon: Clock, bg: 'bg-amber-50 dark:bg-amber-950/30', border: 'border-amber-200 dark:border-amber-800', text: 'text-amber-700 dark:text-amber-400' },
    { label: PdiStatusLabel.EM_ANALISE, value: metricas?.bigNumbers?.emAnalise ?? 0, icon: AlertCircle, bg: 'bg-blue-50 dark:bg-blue-950/30', border: 'border-blue-200 dark:border-blue-800', text: 'text-blue-700 dark:text-blue-400' },
    { label: PdiStatusLabel.IN_PROGRESS, value: metricas?.bigNumbers?.emAndamento ?? 0, icon: Play, bg: 'bg-primary/10 dark:bg-primary/20', border: 'border-primary/40', text: 'text-primary' },
    { label: PdiStatusLabel.FINALIZADO, value: metricas?.bigNumbers?.finalizados ?? 0, icon: CheckCircle, bg: 'bg-green-50 dark:bg-green-950/30', border: 'border-green-200 dark:border-green-800', text: 'text-green-700 dark:text-green-400' },
    { label: PdiStatusLabel.CANCELADO, value: metricas?.bigNumbers?.cancelados ?? 0, icon: XCircle, bg: 'bg-slate-50 dark:bg-slate-900/50', border: 'border-slate-200 dark:border-slate-700', text: 'text-slate-600 dark:text-slate-400' },
  ];

  const renderRow = (row: PdiMetricaItemDTO, columnId: string) => {
    const label = mapPdiStatusToLabel(row.status);
    const badgeClass = statusBadgeClass[label] ?? 'bg-muted text-muted-foreground';
    switch (columnId) {
      case 'status':
        return <Badge className={cn('rounded-full px-2.5 py-0.5 font-medium', badgeClass)}>{label}</Badge>;
      case 'colaboradorId': {
        const pdiId = row.colaboradorId?.trim().toLowerCase();
        // 1. Tentar encontrar na lista de subordinados
        const colaborador = colaboradores.find(c => 
          c.codColaborador?.trim().toLowerCase() === pdiId || 
          c.codigoColaboradorExterno?.trim().toLowerCase() === pdiId
        );
        // 2. Se for o próprio gestor logado (os endpoints agregam PDI do gestor + time)
        const isSelf = 
          user?.colaborador?.codigoColaboradorInterno?.trim().toLowerCase() === pdiId ||
          user?.colaboradorOrg?.codColaborador?.trim().toLowerCase() === pdiId ||
          user?.cpf?.trim().toLowerCase() === pdiId;
        
        const nome = isSelf ? user?.nomeColaborador : (colaborador?.nome || row.nomeColaborador || row.colaboradorId);
        return <span className="text-muted-foreground">{nome || '—'}</span>;
      }
      case 'gestor':
        return <span className="text-muted-foreground">{user?.nomeColaborador || '—'}</span>;
      case 'titulo':
        return (
          <button
            type="button"
            onClick={() => abrirModalDetalhe(row.pdiId)}
            className="flex items-center gap-2 text-left font-medium text-primary hover:text-primary/80 transition-colors"
            data-testid={`pdis-metricas-page-ativos-row-detalhe-button-${row.pdiId}`}
          >
            <Eye className="h-4 w-4 shrink-0" />
            {row.titulo || '—'}
          </button>
        );
      case 'previsao': {
        const dataStr = row.previsao;
        if (!dataStr) return <span className="text-muted-foreground">—</span>;
        try {
          const d = new Date(dataStr);
          return <span className="text-muted-foreground">{isNaN(d.getTime()) ? dataStr : format(d, 'dd/MM/yyyy', { locale: ptBR })}</span>;
        } catch {
          return <span className="text-muted-foreground">{dataStr}</span>;
        }
      }
      case 'andamento':
        return (
          <div className="flex items-center gap-2">
            <div className="w-20 h-2 bg-muted rounded-full overflow-hidden">
              <div className="h-full bg-primary transition-all" style={{ width: `${Math.round((row.progress ?? 0) * 100)}%` }} />
            </div>
            <span className="tabular-nums text-muted-foreground">{Math.round((row.progress ?? 0) * 100)}%</span>
          </div>
        );
      default:
        return null;
    }
  };

  const colunasRelatorio = [
    { id: 'status', label: 'Status' },
    { id: 'colaboradorId', label: 'FourTalent' },
    { id: 'gestor', label: 'Gestor' },
    { id: 'titulo', label: 'Meta' },
    { id: 'previsao', label: 'Previsão' },
    { id: 'andamento', label: 'Andamento' },
  ];

  const previsaoFromActionPlans = (pdi: PdiCompletoTimeDTO): string => {
    const prazos = (pdi.actionPlans ?? []).filter((ap) => ap.deadline).map((ap) => ap.deadline!);
    if (prazos.length === 0) return '—';
    const ultimo = prazos.sort((a, b) => new Date(b).getTime() - new Date(a).getTime())[0];
    const d = new Date(ultimo);
    return format(d, 'dd/MM/yyyy', { locale: ptBR });
  };

  return (
    <div className="container mx-auto p-4 space-y-6">
      <GuideTour
        steps={pdisMetricasSteps}
        run={tourRunning}
        onCallback={handleJoyrideCallback}
      />

      <div className="flex items-center justify-between gap-4 flex-wrap">
        <div className="flex items-center gap-2">
          <div className="h-6 w-1 bg-primary rounded" />
          <h1 className="text-2xl font-bold">Métricas de PDI</h1>
        </div>
        <div className="flex items-center gap-2">
          <TourButton onStart={startTour} data-testid="pdis-metricas-page-tour-button" />
          <Button
            variant="outline"
            onClick={() => carregarMetricas(buildFiltrosParams())}
            disabled={loading}
            data-testid="pdis-metricas-page-atualizar-button"
          >
            {loading ? 'Atualizando...' : 'Atualizar'}
          </Button>
        </div>
      </div>

      {/* Card de filtros (como antes) */}
      <Card className="border border-borderSoft bg-card" data-testid="pdis-metricas-page-filtros-card">
        <CardContent className="p-4">
          <div className="flex items-center gap-2 mb-4">
            <SlidersHorizontal className="h-5 w-5 text-muted-foreground" />
            <h2 className="text-lg font-semibold">Filtros</h2>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
            <div className="space-y-2">
              <Label htmlFor="pdi-metricas-inicio">Início em</Label>
              <Input
                id="pdi-metricas-inicio"
                type="date"
                value={dataInicio}
                onChange={(e) => setDataInicio(e.target.value)}
                className="w-full"
                data-testid="pdis-metricas-page-filtro-data-inicio"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="pdi-metricas-fim">Conclusão até</Label>
              <Input
                id="pdi-metricas-fim"
                type="date"
                value={dataFim}
                onChange={(e) => setDataFim(e.target.value)}
                className="w-full"
                data-testid="pdis-metricas-page-filtro-data-fim"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="pdi-metricas-unidade">Unidade</Label>
              <Select value={unidade || 'todos'} onValueChange={(v) => setUnidade(v === 'todos' ? '' : v)}>
                <SelectTrigger id="pdi-metricas-unidade" data-testid="pdis-metricas-page-filtro-unidade-trigger">
                  <SelectValue placeholder="Todas" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todos">Todas</SelectItem>
                  {unidades.map((u) => (
                    <SelectItem key={u.id} value={u.id} data-testid={`pdis-metricas-page-filtro-unidade-item-${u.id}`}>
                      {u.descricao || u.id}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="pdi-metricas-colaborador">Colaborador da Equipe</Label>
              <Select value={colaboradorFiltro || 'todos'} onValueChange={(v) => setColaboradorFiltro(v === 'todos' ? '' : v)}>
                <SelectTrigger id="pdi-metricas-colaborador" data-testid="pdis-metricas-page-filtro-colaborador-trigger">
                  <SelectValue placeholder="Todos" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todos">Todos</SelectItem>
                  {colaboradores.map((c) => (
                    <SelectItem key={c.codColaborador} value={c.codColaborador} data-testid={`pdis-metricas-page-filtro-colaborador-item-${c.codColaborador}`}>
                      {c.nome}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
          <div className="flex flex-wrap items-center gap-4 mb-4">
            <div className="flex items-center gap-2">
              <Checkbox
                id="filtro-todos"
                checked={filtroTodos}
                onCheckedChange={(c) => setFiltroTodos(c === true)}
                data-testid="pdis-metricas-page-filtro-checkbox-todos"
              />
              <Label htmlFor="filtro-todos" className="font-normal cursor-pointer">Todos</Label>
            </div>
            <div className="flex items-center gap-2">
              <Checkbox
                id="filtro-em-andamento"
                checked={filtroEmAndamento}
                onCheckedChange={(c) => setFiltroEmAndamento(c === true)}
                data-testid="pdis-metricas-page-filtro-checkbox-em-andamento"
              />
              <Label htmlFor="filtro-em-andamento" className="font-normal cursor-pointer">Em andamento</Label>
            </div>
            <div className="flex items-center gap-2">
              <Checkbox
                id="filtro-nao-iniciado"
                checked={filtroNaoIniciado}
                onCheckedChange={(c) => setFiltroNaoIniciado(c === true)}
                data-testid="pdis-metricas-page-filtro-checkbox-nao-iniciado"
              />
              <Label htmlFor="filtro-nao-iniciado" className="font-normal cursor-pointer">Não iniciado</Label>
            </div>
            <div className="flex items-center gap-2">
              <Checkbox
                id="filtro-finalizados"
                checked={filtroFinalizados}
                onCheckedChange={(c) => setFiltroFinalizados(c === true)}
                data-testid="pdis-metricas-page-filtro-checkbox-finalizados"
              />
              <Label htmlFor="filtro-finalizados" className="font-normal cursor-pointer">Finalizados</Label>
            </div>
          </div>
          <div className="flex gap-2">
            <Button variant="outline" onClick={limparFiltros} disabled={loading} data-testid="pdis-metricas-page-filtros-limpar-button">
              Limpar filtros
            </Button>
            <Button onClick={buscarComFiltros} disabled={loading} data-testid="pdis-metricas-page-filtros-buscar-button">
              Buscar
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Big numbers */}
      <div className="grid grid-cols-2 sm:grid-cols-5 gap-4">
        {bigNumbersConfig.map((item) => {
          const Icon = item.icon;
          return (
            <Card key={item.label} className={cn('overflow-hidden border-2 transition-all', item.border, item.bg)}>
              <CardContent className="p-5">
                <div className="flex items-start justify-between gap-3">
                  <div className={cn('flex shrink-0 items-center justify-center w-11 h-11 rounded-full ring-2 bg-background/80', item.text)}>
                    <Icon className={cn('h-5 w-5', item.text)} />
                  </div>
                  <p className={cn('text-3xl font-bold tabular-nums', item.text)}>{item.value}</p>
                </div>
                <p className="text-sm font-medium text-foreground mt-4">{item.label}</p>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Ativos */}
      <PdiTableSection<PdiMetricaItemDTO>
        title="Ativos"
        subtitle="PDI's em análise ou em andamento"
        data-testid="pdis-metricas-page-ativos"
        columns={colunasRelatorio}
        data={metricas?.ativos ?? []}
        keyExtractor={(row) => row.pdiId}
        loading={loading}
        emptyMessage="Nenhum PDI ativo."
        renderCell={renderRow}
      />

      {/* Histórico */}
      <PdiTableSection<PdiMetricaItemDTO>
        title="Histórico"
        subtitle="PDI's finalizados ou cancelados"
        data-testid="pdis-metricas-page-historicos"
        columns={colunasRelatorio}
        data={metricas?.historicos ?? []}
        keyExtractor={(row) => row.pdiId}
        loading={loading}
        emptyMessage="Nenhum PDI no histórico."
        renderCell={renderRow}
      />

      {/* Modal detalhe PDI */}
      <Dialog open={!!modalPdiId} onOpenChange={(open) => !open && setModalPdiId(null)}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto" data-testid="pdis-metricas-page-detalhe-pdi-modal">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Target className="h-5 w-5" />
              Detalhes do PDI
            </DialogTitle>
          </DialogHeader>
          {loadingDetalhe ? (
            <p className="py-8 text-center text-muted-foreground" data-testid="pdis-metricas-page-detalhe-pdi-modal-loading">Carregando...</p>
          ) : modalDetalhe ? (
            <div className="space-y-6">
              <div className="flex flex-wrap items-center gap-4">
                <p className="text-sm">
                  <span className="font-medium">Status:</span>{' '}
                  <Badge className={cn('rounded-full', statusBadgeClass[mapPdiStatusToLabel(modalDetalhe.status)] ?? 'bg-muted text-muted-foreground')}>
                    {mapPdiStatusToLabel(modalDetalhe.status)}
                  </Badge>
                </p>
              </div>
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <Card className="border border-borderSoft bg-card shadow-softToken">
                  <CardContent className="p-4">
                    <div className="flex items-center gap-2 text-muted-foreground mb-1">
                      <Target className="h-4 w-4" />
                      <span className="text-xs font-medium uppercase">Meta</span>
                    </div>
                    <p className="font-medium text-foreground">{modalDetalhe.titulo || '—'}</p>
                  </CardContent>
                </Card>
                <Card className="border border-borderSoft bg-card shadow-softToken">
                  <CardContent className="p-4">
                    <div className="flex items-center gap-2 text-muted-foreground mb-1">
                      <CalendarIcon className="h-4 w-4" />
                      <span className="text-xs font-medium uppercase">Previsão conclusão</span>
                    </div>
                    <p className="font-medium text-foreground">{previsaoFromActionPlans(modalDetalhe)}</p>
                  </CardContent>
                </Card>
                <Card className="border border-borderSoft bg-card shadow-softToken">
                  <CardContent className="p-4">
                    <div className="flex items-center gap-2 text-muted-foreground mb-1">
                      <span className="text-xs font-medium uppercase">Andamento</span>
                    </div>
                    <p className="font-medium text-foreground">{Math.round((modalDetalhe.progress ?? 0) * 100)}%</p>
                  </CardContent>
                </Card>
              </div>
              {(modalDetalhe.skills?.length ?? 0) > 0 && (
                <div>
                  <p className="text-sm font-medium mb-2 flex items-center gap-2">
                    <FileText className="h-4 w-4" />
                    Aprimoramento de competências
                  </p>
                  <div className="flex flex-wrap gap-2">
                    {modalDetalhe.skills.map((s) => (
                      <Badge key={s.id} variant="secondary">
                        {s.nomeSkill ?? '—'}
                      </Badge>
                    ))}
                  </div>
                </div>
              )}
              {(modalDetalhe.actionPlans?.length ?? 0) > 0 && (
                <div>
                  <h4 className="font-semibold mb-3">Planos de ação</h4>
                  <div className="space-y-3">
                    {modalDetalhe.actionPlans.map((ap, i) => (
                      <Card key={ap.id} className="border border-borderSoft bg-card">
                        <CardContent className="p-4">
                          <div className="flex items-start justify-between gap-2">
                            <div>
                              <p className="font-medium">Plano {i + 1}</p>
                              <p className="text-sm text-muted-foreground mt-1">
                                Prazo previsto: {ap.deadline ? format(new Date(ap.deadline), 'dd/MM/yyyy', { locale: ptBR }) : '—'}
                                {ap.concluidoEm ? ` · Concluído em: ${format(new Date(ap.concluidoEm), 'dd/MM/yyyy', { locale: ptBR })}` : ''}
                              </p>
                              {ap.description && <p className="text-sm mt-2">{ap.description}</p>}
                            </div>
                            {ap.concluidoEm ? <CheckCircle className="h-5 w-5 text-green-600 shrink-0" /> : null}
                          </div>
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                </div>
              )}
              <div className="flex justify-end pt-4">
                <Button variant="secondary" onClick={() => setModalPdiId(null)} data-testid="pdis-metricas-page-detalhe-pdi-modal-fechar-button">
                  Fechar
                </Button>
              </div>
            </div>
          ) : modalPdiId && !loadingDetalhe ? (
            <p className="py-8 text-center text-muted-foreground" data-testid="pdis-metricas-page-detalhe-pdi-modal-error">Não foi possível carregar os detalhes.</p>
          ) : null}
        </DialogContent>
      </Dialog>
    </div>
  );
}
