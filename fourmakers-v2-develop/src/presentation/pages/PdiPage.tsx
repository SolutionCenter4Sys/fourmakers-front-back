import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Target, Clock, AlertCircle, Play, CheckCircle, XCircle, Eye } from '@/components/ui/system-icons';
import { ColaboradorPdiApi } from '@data/api/ColaboradorPdiApi';
import type { PdiResumoDTO } from '@shared/types/pdiApi';
import { PdiStatusLabel, mapPdiStatusToLabel, getPdiStatusDisplayLabel } from '@shared/types/pdiApi';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { cn } from '@/lib/utils';
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

export default function PdiPage() {
  const navigate = useNavigate();
  const { token, user, codColaborador: authCodColaborador } = useAppSelector((state) => state.auth);
  const codColaborador = user?.colaboradorOrg?.codColaborador || '';
  const nomeUsuario = user?.nomeColaborador?.trim().split(/\s+/)[0] || 'você';
  const codigoInternoUsuario = authCodColaborador || user?.cpf || codColaborador;
  const [meusPdis, setMeusPdis] = useState<PdiResumoDTO[]>([]);
  const [loadingMeus, setLoadingMeus] = useState(false);

  const carregarMeusPdis = async () => {
    if (!token) return;
    setLoadingMeus(true);
    try {
      const list = await api.getMeusPdis(token);
      setMeusPdis(list);
    } catch {
      setMeusPdis([]);
    } finally {
      setLoadingMeus(false);
    }
  };

  useEffect(() => {
    carregarMeusPdis();
  }, [token]);

  const contadoresMeus = {
    naoIniciados: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.NAO_INICIADO).length,
    emAnalise: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.EM_ANALISE).length,
    emAndamento: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.IN_PROGRESS).length,
    finalizados: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.FINALIZADO).length,
    cancelados: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.CANCELADO).length,
  };

  const ativosMeus = meusPdis.filter((p) => mapPdiStatusToLabel(p.status) !== PdiStatusLabel.FINALIZADO && mapPdiStatusToLabel(p.status) !== PdiStatusLabel.CANCELADO);
  const historicoMeus = meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.FINALIZADO);

  return (
    <div className="container mx-auto p-4 space-y-6">
      <div className="flex items-center gap-2">
        <div className="h-6 w-1 bg-primary rounded" />
        <h1 className="text-2xl font-bold">Plano de desenvolvimento individual</h1>
      </div>

      {/* Banner Ian — Meus PDIs */}
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
              <span className="text-white/95">. Sou o Ian e vou te acompanhar no seu PDI, vamos juntos?</span>
            </p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-2 sm:grid-cols-5 gap-4">
        {[
          { label: PdiStatusLabel.NAO_INICIADO, value: contadoresMeus.naoIniciados, icon: Clock, bg: 'bg-amber-50 dark:bg-amber-950/30', border: 'border-amber-200 dark:border-amber-800', text: 'text-amber-700 dark:text-amber-400', iconBg: 'bg-amber-200/80 dark:bg-amber-800/60', iconRing: 'ring-amber-300 dark:ring-amber-700' },
          { label: PdiStatusLabel.EM_ANALISE, value: contadoresMeus.emAnalise, icon: AlertCircle, bg: 'bg-blue-50 dark:bg-blue-950/30', border: 'border-blue-200 dark:border-blue-800', text: 'text-blue-700 dark:text-blue-400', iconBg: 'bg-blue-200/80 dark:bg-blue-800/60', iconRing: 'ring-blue-300 dark:ring-blue-700' },
          { label: PdiStatusLabel.IN_PROGRESS, value: contadoresMeus.emAndamento, icon: Play, bg: 'bg-primary/10 dark:bg-primary/20', border: 'border-primary/40', text: 'text-primary', iconBg: 'bg-primary/30 dark:bg-primary/40', iconRing: 'ring-primary/50' },
          { label: PdiStatusLabel.FINALIZADO, value: contadoresMeus.finalizados, icon: CheckCircle, bg: 'bg-green-50 dark:bg-green-950/30', border: 'border-green-200 dark:border-green-800', text: 'text-green-700 dark:text-green-400', iconBg: 'bg-green-200/80 dark:bg-green-800/60', iconRing: 'ring-green-300 dark:ring-green-700' },
          { label: PdiStatusLabel.CANCELADO, value: contadoresMeus.cancelados, icon: XCircle, bg: 'bg-slate-50 dark:bg-slate-900/50', border: 'border-slate-200 dark:border-slate-700', text: 'text-slate-600 dark:text-slate-400', iconBg: 'bg-slate-200/80 dark:bg-slate-700/60', iconRing: 'ring-slate-300 dark:ring-slate-600' },
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
        <div className="flex items-center justify-between mb-4">
          <div>
            <h2 className="text-lg font-semibold">Ativos</h2>
            <p className="text-sm text-muted-foreground">Acompanhe o status dos seus PDI&apos;s</p>
          </div>
          <Button onClick={() => navigate('/pdi/novo')}>
            <Target className="h-4 w-4 mr-2" />
            Criar PDI
          </Button>
        </div>
        {loadingMeus ? (
          <p className="text-muted-foreground py-8 text-center">Carregando...</p>
        ) : ativosMeus.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center text-muted-foreground">
              <p className="mb-2">Fale com seu gestor para que construam em conjunto seu Plano de Desenvolvimento.</p>
              <Button variant="outline" className="mt-4" onClick={() => navigate('/pdi/novo')}>
                Criar PDI
              </Button>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-3">
            {ativosMeus.map((pdi) => {
              const statusLabel = getPdiStatusDisplayLabel(pdi.status, pdi.progress);
              const badgeClass = statusBadgeClass[statusLabel] || 'bg-muted text-muted-foreground border-0';
              const criadoPorGestor = !!(pdi.codigoInternoColaboradorCriacao && pdi.codigoInternoColaboradorCriacao !== codigoInternoUsuario);
              return (
                <Card
                  key={pdi.id}
                  className="cursor-pointer hover:bg-muted/50 hover:shadow-md transition-all border border-border/80 rounded-xl"
                  onClick={() => navigate(`/pdi/${pdi.id}`, { state: { isMeuPdi: true, from: '/pdi', criadoPorGestor } })}
                >
                  <CardContent className="p-4 flex items-center justify-between gap-4">
                    <div className="min-w-0 flex-1">
                      <p className="font-semibold text-foreground truncate">{pdi.titulo}</p>
                      <p className="text-sm text-muted-foreground mt-0.5">
                        Previsão: {pdi.dataCriacao ? format(new Date(pdi.dataCriacao), 'dd/MM/yyyy', { locale: ptBR }) : '-'} · {Math.round((pdi.progress ?? 0) * 100)}%
                      </p>
                    </div>
                    <Badge className={cn('shrink-0 font-medium rounded-full px-2.5 py-0.5', badgeClass)}>{statusLabel}</Badge>
                  </CardContent>
                </Card>
              );
            })}
          </div>
        )}
      </div>

      <div>
        <h2 className="text-lg font-semibold mb-2">Histórico</h2>
        <p className="text-sm text-muted-foreground mb-4">Acompanhe a sua evolução de cada meta conquistada</p>
        {loadingMeus ? (
          <p className="text-muted-foreground py-4 text-center">Carregando...</p>
        ) : historicoMeus.length === 0 ? (
          <Card>
            <CardContent className="py-8 text-center text-muted-foreground">
              Fale com seu gestor para que construam em conjunto seu Plano de Desenvolvimento.
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
                    <th className="text-left p-4 font-semibold text-foreground">Previsão conclusão</th>
                    <th className="text-left p-4 font-semibold text-foreground">Concluído em</th>
                    <th className="text-right p-4 font-semibold text-foreground">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {historicoMeus.map((pdi) => (
                    <tr key={pdi.id} className="border-b last:border-0 hover:bg-muted/30 transition-colors">
                      <td className="p-4">
                        <Badge className={cn('font-medium rounded-full px-2.5 py-0.5', statusBadgeClass[PdiStatusLabel.FINALIZADO])}>Finalizado</Badge>
                      </td>
                      <td className="p-4 font-medium text-foreground">{pdi.titulo}</td>
                      <td className="p-4 text-muted-foreground">{pdi.dataCriacao ? format(new Date(pdi.dataCriacao), 'dd/MM/yyyy', { locale: ptBR }) : '-'}</td>
                      <td className="p-4 text-muted-foreground">{pdi.dataAtualizacao ? format(new Date(pdi.dataAtualizacao), 'dd/MM/yyyy', { locale: ptBR }) : '-'}</td>
                      <td className="p-4 text-right">
                        <Button
                          variant="outline"
                          size="sm"
                          className="shrink-0"
                          onClick={(e) => {
                            e.stopPropagation();
                            const criadoPorGestor = !!(pdi.codigoInternoColaboradorCriacao && pdi.codigoInternoColaboradorCriacao !== codigoInternoUsuario);
                            navigate(`/pdi/${pdi.id}`, { state: { isMeuPdi: true, from: '/pdi', apenasVisualizar: true, criadoPorGestor } });
                          }}
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
