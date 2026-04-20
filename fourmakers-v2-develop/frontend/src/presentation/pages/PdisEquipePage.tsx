import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { Search, Target, Clock, AlertCircle, Play, CheckCircle, XCircle, Eye } from '@/components/ui/system-icons';
import { ColaboradorPdiApi } from '@data/api/ColaboradorPdiApi';
import type { PdiResumoTimeDTO } from '@shared/types/pdiApi';
import { PdiStatusLabel, mapPdiStatusToLabel, getPdiStatusDisplayLabel } from '@shared/types/pdiApi';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { useColaboradoresDesempenho } from '@/presentation/hooks/useGestaoDesempenho';
import avatarIan from '@/assets/avatar.png';

const api = new ColaboradorPdiApi();

/** Badges: mesmas cores dos big numbers */
const statusBadgeClass: Record<string, string> = {
  [PdiStatusLabel.NAO_INICIADO]: 'bg-amber-50 text-amber-800 border-2 border-amber-200 dark:bg-amber-950/30 dark:text-amber-300 dark:border-amber-700',
  [PdiStatusLabel.EM_ANALISE]: 'bg-blue-50 text-blue-800 border-2 border-blue-200 dark:bg-blue-950/30 dark:text-blue-300 dark:border-blue-700',
  [PdiStatusLabel.IN_PROGRESS]: 'bg-primary/10 text-primary border-2 border-primary/40 dark:bg-primary/20 dark:border-primary/50',
  [PdiStatusLabel.FINALIZADO]: 'bg-green-50 text-green-800 border-2 border-green-200 dark:bg-green-950/30 dark:text-green-300 dark:border-green-700',
  [PdiStatusLabel.CANCELADO]: 'bg-slate-50 text-slate-700 border-2 border-slate-200 dark:bg-slate-800/50 dark:text-slate-300 dark:border-slate-600',
};

export default function PdisEquipePage() {
  const navigate = useNavigate();
  const { token, user } = useAppSelector((state) => state.auth);
  const nomeUsuario = user?.nomeColaborador?.trim().split(/\s+/)[0] || 'você';
  const [pdisTime, setPdisTime] = useState<PdiResumoTimeDTO[]>([]);
  const [loadingTime, setLoadingTime] = useState(false);
  const [buscaEquipe, setBuscaEquipe] = useState('');
  const [filtroStatusEquipe, setFiltroStatusEquipe] = useState('todos');
  const { colaboradores } = useColaboradoresDesempenho('todos', '');

  const getNomeColaborador = (pdiIdStr: string, fallbackNome?: string) => {
    const idStr = pdiIdStr?.trim().toLowerCase();
    if (!idStr) return '—';
    // 1. Verificar se é o próprio gestor logado
    const isSelf = 
      user?.colaborador?.codigoColaboradorInterno?.trim().toLowerCase() === idStr ||
      user?.colaboradorOrg?.codColaborador?.trim().toLowerCase() === idStr ||
      user?.cpf?.trim().toLowerCase() === idStr;
    if (isSelf) return user?.nomeColaborador;
    
    // 2. Tentar encontrar na lista de subordinados
    const colaborador = colaboradores.find(c => 
      c.codColaborador?.trim().toLowerCase() === idStr ||
      c.codigoColaboradorExterno?.trim().toLowerCase() === idStr
    );
    
    return colaborador?.nome || fallbackNome || pdiIdStr;
  };

  const carregarPdisTime = async () => {
    if (!token) return;
    setLoadingTime(true);
    try {
      const result = await api.getPdisTime(token, { pagina: 1, tamanhoPagina: 100 });
      setPdisTime(result.items ?? []);
    } catch {
      setPdisTime([]);
    } finally {
      setLoadingTime(false);
    }
  };

  useEffect(() => {
    carregarPdisTime();
  }, [token]);

  const contadoresEquipe = {
    naoIniciados: pdisTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.NAO_INICIADO).length,
    emAnalise: pdisTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.EM_ANALISE).length,
    emAndamento: pdisTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.IN_PROGRESS).length,
    finalizados: pdisTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.FINALIZADO).length,
    cancelados: pdisTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.CANCELADO).length,
  };

  const pdisTimeFiltrados = pdisTime.filter((p) => {
    if (filtroStatusEquipe !== 'todos') {
      const lbl = mapPdiStatusToLabel(p.status);
      if (filtroStatusEquipe === 'nao_iniciado' && lbl !== PdiStatusLabel.NAO_INICIADO) return false;
      if (filtroStatusEquipe === 'em_andamento' && lbl !== PdiStatusLabel.IN_PROGRESS) return false;
      if (filtroStatusEquipe === 'finalizado' && lbl !== PdiStatusLabel.FINALIZADO) return false;
    }
    if (buscaEquipe.trim()) {
      const termo = buscaEquipe.toLowerCase();
      return (p.titulo || '').toLowerCase().includes(termo);
    }
    return true;
  });

  const historicoEquipe = pdisTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.FINALIZADO);

  return (
    <div className="container mx-auto p-4 space-y-6">
      <div className="flex items-center gap-2">
        <div className="h-6 w-1 bg-primary rounded" />
        <h1 className="text-2xl font-bold">PDIs da minha equipe</h1>
      </div>

      {/* Banner Ian */}
      <div
        className="relative overflow-hidden rounded-2xl h-[200px] flex items-center gap-4 sm:gap-6 p-0 pr-6 text-white"
        style={{
          backgroundImage: 'linear-gradient(135deg, rgba(15, 118, 110, 0.95) 0%, rgba(88, 28, 135, 0.9) 45%, rgba(49, 46, 129, 0.95) 100%), url(/profile-hero-bg.jpg)',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      >
        <div className="self-end">
          <img src={avatarIan} alt="Ian" className="h-32 sm:h-40 w-auto object-contain object-bottom block" />
        </div>
        <div className="flex-1 min-w-0 flex items-center py-4">
          <div className="rounded-xl bg-indigo-900/80 backdrop-blur-sm border border-white/10 shadow-lg px-5 py-4 text-left max-w-xl">
            <p className="text-lg sm:text-xl font-medium leading-snug text-white">
              <span className="font-bold text-white bg-white/20 px-1.5 py-0.5 rounded">Oi, {nomeUsuario}</span>
              <span className="text-white/95">. Sou o Ian e vou te acompanhar na gestão de PDI&apos;s do time, vamos juntos?</span>
            </p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-2 sm:grid-cols-5 gap-4">
        {[
          { label: PdiStatusLabel.NAO_INICIADO, value: contadoresEquipe.naoIniciados, icon: Clock, bg: 'bg-amber-50 dark:bg-amber-950/30', border: 'border-amber-200 dark:border-amber-800', text: 'text-amber-700 dark:text-amber-400', iconBg: 'bg-amber-200/80 dark:bg-amber-800/60', iconRing: 'ring-amber-300 dark:ring-amber-700' },
          { label: PdiStatusLabel.EM_ANALISE, value: contadoresEquipe.emAnalise, icon: AlertCircle, bg: 'bg-blue-50 dark:bg-blue-950/30', border: 'border-blue-200 dark:border-blue-800', text: 'text-blue-700 dark:text-blue-400', iconBg: 'bg-blue-200/80 dark:bg-blue-800/60', iconRing: 'ring-blue-300 dark:ring-blue-700' },
          { label: PdiStatusLabel.IN_PROGRESS, value: contadoresEquipe.emAndamento, icon: Play, bg: 'bg-primary/10 dark:bg-primary/20', border: 'border-primary/40', text: 'text-primary', iconBg: 'bg-primary/30 dark:bg-primary/40', iconRing: 'ring-primary/50' },
          { label: PdiStatusLabel.FINALIZADO, value: contadoresEquipe.finalizados, icon: CheckCircle, bg: 'bg-green-50 dark:bg-green-950/30', border: 'border-green-200 dark:border-green-800', text: 'text-green-700 dark:text-green-400', iconBg: 'bg-green-200/80 dark:bg-green-800/60', iconRing: 'ring-green-300 dark:ring-green-700' },
          { label: PdiStatusLabel.CANCELADO, value: contadoresEquipe.cancelados, icon: XCircle, bg: 'bg-slate-50 dark:bg-slate-900/50', border: 'border-slate-200 dark:border-slate-700', text: 'text-slate-600 dark:text-slate-400', iconBg: 'bg-slate-200/80 dark:bg-slate-700/60', iconRing: 'ring-slate-300 dark:ring-slate-600' },
        ].map((item) => {
          const Icon = item.icon;
          return (
            <Card key={item.label} className={cn('overflow-hidden border-2 transition-all hover:shadow-lg hover:-translate-y-0.5', item.border, item.bg)}>
              <CardContent className="p-5">
                <div className="flex items-start justify-between gap-3">
                  <div className={cn('flex shrink-0 items-center justify-center w-11 h-11 rounded-full ring-2', item.iconBg, item.iconRing)}>
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

      <div>
        <div className="flex flex-wrap items-center justify-between gap-4 mb-4">
          <div>
            <h2 className="text-lg font-semibold">Ativos</h2>
            <p className="text-sm text-muted-foreground">Acompanhe o status de PDI do time</p>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <select
              className="border rounded-md px-3 py-2 text-sm"
              value={filtroStatusEquipe}
              onChange={(e) => setFiltroStatusEquipe(e.target.value)}
            >
              <option value="todos">Todos</option>
              <option value="nao_iniciado">Não iniciado</option>
              <option value="em_andamento">Em andamento</option>
              <option value="finalizado">Finalizado</option>
            </select>
            <div className="relative">
              <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Pesquisar Fourtalent"
                className="pl-8 w-48"
                value={buscaEquipe}
                onChange={(e) => setBuscaEquipe(e.target.value)}
              />
            </div>
            <Button onClick={() => navigate('/gestao-desempenho-gestor')}>
              <Target className="h-4 w-4 mr-2" />
              Criar PDI
            </Button>
          </div>
        </div>
        {loadingTime ? (
          <p className="text-muted-foreground py-8 text-center">Carregando...</p>
        ) : pdisTimeFiltrados.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center text-muted-foreground">
              Nenhum PDI do time encontrado.
            </CardContent>
          </Card>
        ) : (
          <div className="rounded-xl border border-border/80 overflow-hidden shadow-sm">
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-muted/60">
                    <th className="text-left p-4 font-semibold text-foreground">Status</th>
                    <th className="text-left p-4 font-semibold text-foreground">PDI</th>
                    <th className="text-left p-4 font-semibold text-foreground">FourTalent</th>
                    <th className="text-left p-4 font-semibold text-foreground">Gestor</th>
                    <th className="text-left p-4 font-semibold text-foreground">Previsão conclusão</th>
                    <th className="text-left p-4 font-semibold text-foreground">Andamento</th>
                    <th className="text-left p-4 font-semibold text-foreground">Criado por</th>
                    <th className="text-right p-4 font-semibold text-foreground">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {pdisTimeFiltrados.map((p) => {
                    const statusLabel = getPdiStatusDisplayLabel(p.status, p.progress);
                    const badgeClass = statusBadgeClass[statusLabel] || 'bg-muted text-muted-foreground border-0';
                    return (
                      <tr key={p.pdiId} className="border-b last:border-0 hover:bg-muted/30 transition-colors">
                        <td className="p-4">
                          <Badge className={cn('font-medium rounded-full px-2.5 py-0.5', badgeClass)}>{statusLabel}</Badge>
                        </td>
                        <td className="p-4 font-medium text-foreground">{p.titulo}</td>
                        <td className="p-4 text-muted-foreground">
                          {getNomeColaborador(p.colaboradorId, p.nomeColaborador)}
                        </td>
                        <td className="p-4 text-muted-foreground">
                          {user?.nomeColaborador || '—'}
                        </td>
                        <td className="p-4 text-muted-foreground">
                          {p.previsaoConclusao ? (() => {
                            try {
                              const d = new Date(p.previsaoConclusao);
                              return isNaN(d.getTime()) ? p.previsaoConclusao : format(d, 'dd/MM/yyyy', { locale: ptBR });
                            } catch {
                              return p.previsaoConclusao;
                            }
                          })() : '-'}
                        </td>
                        <td className="p-4">
                          <div className="flex items-center gap-2">
                            <div className="w-20 h-2 bg-muted rounded-full overflow-hidden">
                              <div className={cn('h-full bg-primary transition-all')} style={{ width: `${Math.round((p.progress ?? 0) * 100)}%` }} />
                            </div>
                            <span className="tabular-nums text-muted-foreground">{Math.round((p.progress ?? 0) * 100)}%</span>
                          </div>
                        </td>
                        <td className="p-4">
                          <Badge className="bg-blue-50 text-blue-700 dark:bg-blue-900/40 dark:text-blue-300 border-0 font-medium">Gestor</Badge>
                        </td>
                        <td className="p-4 text-right">
                          <Button
                            variant="outline"
                            size="sm"
                            className="shrink-0"
                            onClick={() => navigate(`/gestao-desempenho-gestor/${p.colaboradorId}/pdi/${p.pdiId}`)}
                          >
                            <Eye className="h-4 w-4 mr-1.5" />
                            Visualizar
                          </Button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>

      <div>
        <div className="mb-4">
          <h2 className="text-lg font-semibold">Histórico</h2>
          <p className="text-sm text-muted-foreground">Acompanhe a evolução e metas conquistadas do time</p>
        </div>
        {historicoEquipe.length === 0 ? (
          <Card>
            <CardContent className="py-8 text-center text-muted-foreground">
              Nenhum PDI finalizado do time no momento.
            </CardContent>
          </Card>
        ) : (
          <div className="rounded-xl border border-border/80 overflow-hidden shadow-sm">
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-muted/60">
                    <th className="text-left p-4 font-semibold text-foreground">Status</th>
                    <th className="text-left p-4 font-semibold text-foreground">PDI</th>
                    <th className="text-left p-4 font-semibold text-foreground">FourTalent</th>
                    <th className="text-left p-4 font-semibold text-foreground">Gestor</th>
                    <th className="text-left p-4 font-semibold text-foreground">Previsão conclusão</th>
                    <th className="text-left p-4 font-semibold text-foreground">Concluído em</th>
                    <th className="text-right p-4 font-semibold text-foreground">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {historicoEquipe.map((p) => (
                    <tr key={p.pdiId} className="border-b last:border-0 hover:bg-muted/30 transition-colors">
                      <td className="p-4">
                        <Badge className={cn('font-medium rounded-full px-2.5 py-0.5', statusBadgeClass[PdiStatusLabel.FINALIZADO])}>Finalizado</Badge>
                      </td>
                      <td className="p-4 font-medium text-foreground">{p.titulo}</td>
                      <td className="p-4 text-muted-foreground">
                        {getNomeColaborador(p.colaboradorId, p.nomeColaborador)}
                      </td>
                      <td className="p-4 text-muted-foreground">
                        {user?.nomeColaborador || '—'}
                      </td>
                      <td className="p-4 text-muted-foreground">
                        {p.previsaoConclusao ? (() => {
                          try {
                            const d = new Date(p.previsaoConclusao);
                            return isNaN(d.getTime()) ? p.previsaoConclusao : format(d, 'dd/MM/yyyy', { locale: ptBR });
                          } catch {
                            return p.previsaoConclusao;
                          }
                        })() : '-'}
                      </td>
                      <td className="p-4 text-muted-foreground">-</td>
                      <td className="p-4 text-right">
                        <Button
                          variant="outline"
                          size="sm"
                          className="shrink-0"
                          onClick={() => navigate(`/gestao-desempenho-gestor/${p.colaboradorId}/pdi/${p.pdiId}`, { state: { apenasVisualizar: true } })}
                        >
                          <Eye className="h-4 w-4 mr-1.5" />
                          Visualizar
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
