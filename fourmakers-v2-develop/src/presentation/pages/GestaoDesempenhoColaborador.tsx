import { useState, useEffect, useMemo, useRef } from 'react';
import { GuideTour, TourButton, useTour, gestaoDesempenhoColaboradorSteps } from '@shared/tour';
import { useNavigate, useLocation, useSearchParams } from 'react-router-dom';
import { useAppSelector } from '@/app/store/hooks';
import { Card, CardContent } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { StatCard } from '@presentation/components/common/StatCard';
import { 
  MessageCircle, 
  Users, 
  Target,
  TrendingUp,
  AlertTriangle,
  Calendar,
  User as Person,
  ChevronRight,
  Eye,
  EyeOff,
  ChevronUp,
  ChevronDown,
  Play,
  Stop,
  CheckCircle,
  RefreshCw,
  Edit,
  FileText,
  Clock,
  AlertCircle,
  XCircle,
} from '@/components/ui/system-icons';
import { 
  useEstatisticasPessoais,
  useRegistrosCriticosPessoais,
  useFeedbacks, 
  useRegistrosUmAUm, 
  usePautasSugeridas 
} from '@/presentation/hooks/useGestaoDesempenho';
import { ColaboradorPdiApi } from '@data/api/ColaboradorPdiApi';
import type { PdiResumoDTO } from '@shared/types/pdiApi';
import { PdiStatusLabel, mapPdiStatusToLabel } from '@shared/types/pdiApi';
import { cn } from '@/lib/utils';
import { container } from '@core/di/container';
import { InserirVisualizacaoFeedbackUseCase } from '@domain/usecases/InserirVisualizacaoFeedbackUseCase';
import { InserirPautaSugeridaColaboradorUseCase } from '@domain/usecases/InserirPautaSugeridaColaboradorUseCase';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from '@/components/ui/dialog';
import { PautaSugeridaEditor, htmlToPlainText } from '@/presentation/components/gestaoDesempenho/PautaSugeridaEditor';
import { toast } from 'sonner';
import { Plus } from '@/components/ui/system-icons';
import type { RegistroCritico } from '@shared/types/gestaoDesempenho';
import { PdiTableSection } from '@/presentation/components/gestaoDesempenho/PdiTableSection';
import { formatData } from '@shared/utils/bookColaboradorFormatters';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';

const colaboradorPdiApi = new ColaboradorPdiApi();

/** Badges da tabela: mesmas cores dos big numbers (amber, blue, primary, green, slate) */
const statusBadgeClassPdi: Record<string, string> = {
  [PdiStatusLabel.NAO_INICIADO]: 'bg-amber-50 text-amber-800 border-2 border-amber-200 dark:bg-amber-950/30 dark:text-amber-300 dark:border-amber-700',
  [PdiStatusLabel.EM_ANALISE]: 'bg-blue-50 text-blue-800 border-2 border-blue-200 dark:bg-blue-950/30 dark:text-blue-300 dark:border-blue-700',
  [PdiStatusLabel.IN_PROGRESS]: 'bg-primary/10 text-primary border-2 border-primary/40 dark:bg-primary/20 dark:border-primary/50',
  [PdiStatusLabel.FINALIZADO]: 'bg-green-50 text-green-800 border-2 border-green-200 dark:bg-green-950/30 dark:text-green-300 dark:border-green-700',
  [PdiStatusLabel.CANCELADO]: 'bg-slate-50 text-slate-700 border-2 border-slate-200 dark:bg-slate-800/50 dark:text-slate-300 dark:border-slate-600',
};

const TAB_VALUES = ['feedbacks', 'registros-1-1', 'pdi'] as const;

export default function GestaoDesempenhoColaborador() {
  const navigate = useNavigate();
  const location = useLocation();
  const [searchParams, setSearchParams] = useSearchParams();
  const user = useAppSelector((state) => state.auth.user);
  const { token, codColaborador: authCodColaborador } = useAppSelector((state) => state.auth);
  const codColaborador = user?.colaboradorOrg?.codColaborador || '';
  /** Backend usa CPF como codigo_interno_colaborador; comparar criador do PDI com o CPF do usuário logado para exibir "Criado por: Colaborador" vs "Gestor". */
  const codigoInternoUsuario = authCodColaborador || user?.cpf || codColaborador;

  const { estatisticas, loading: loadingEstatisticas } = useEstatisticasPessoais(codColaborador);
  const { registros, loading: loadingRegistros } = useRegistrosCriticosPessoais(codColaborador);
  const { feedbacks, feedbacksVistos, feedbacksNaoVistos, refetch: refetchFeedbacks } = useFeedbacks(codColaborador);
  const { registros: registros1a1, registrosVistos, registrosNaoVistos } = useRegistrosUmAUm(codColaborador);
  const { pautas, refetch: refetchPautas } = usePautasSugeridas(codColaborador);

  const [meusPdis, setMeusPdis] = useState<PdiResumoDTO[]>([]);
  const [loadingMeusPdis, setLoadingMeusPdis] = useState(false);
  const { isRunning: tourRunning, startTour, handleJoyrideCallback } = useTour();

  // Aba ativa: prioridade (1) ?tab= na URL, (2) location.state.openTab, (3) 'feedbacks'
  const tabFromUrl = searchParams.get('tab');
  const tabFromState = (location.state as { openTab?: string })?.openTab;
  const [activeTab, setActiveTab] = useState<string>(() => {
    if (tabFromUrl && TAB_VALUES.includes(tabFromUrl as (typeof TAB_VALUES)[number])) return tabFromUrl;
    if (tabFromState === 'pdi') return 'pdi';
    return 'feedbacks';
  });

  useEffect(() => {
    const fromUrl = searchParams.get('tab');
    if (fromUrl && TAB_VALUES.includes(fromUrl as (typeof TAB_VALUES)[number])) {
      setActiveTab(fromUrl);
      return;
    }
    const fromState = (location.state as { openTab?: string })?.openTab;
    if (fromState === 'pdi') setActiveTab('pdi');
  }, [searchParams, location.state]);

  const handleTabChange = (value: string) => {
    setActiveTab(value);
    setSearchParams(
      value === 'feedbacks' ? {} : { tab: value },
      { replace: true }
    );
  };

  const [expandedFeedback, setExpandedFeedback] = useState<string | null>(null);
  const [expandedRegistro, setExpandedRegistro] = useState<string | null>(null);
  const initialRegistroExpandDone = useRef(false);
  const [dialogPautaOpen, setDialogPautaOpen] = useState(false);
  const [pautaTexto, setPautaTexto] = useState('');
  const [salvandoPauta, setSalvandoPauta] = useState(false);

  const carregarMeusPdis = async () => {
    if (!token) return;
    setLoadingMeusPdis(true);
    try {
      const list = await colaboradorPdiApi.getMeusPdis(token);
      setMeusPdis(list);
    } catch {
      setMeusPdis([]);
    } finally {
      setLoadingMeusPdis(false);
    }
  };

  useEffect(() => {
    if (token) carregarMeusPdis();
  }, [token]);

  const contadoresPdi = useMemo(() => ({
    naoIniciados: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.NAO_INICIADO).length,
    emAnalise: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.EM_ANALISE).length,
    emAndamento: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.IN_PROGRESS).length,
    finalizados: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.FINALIZADO).length,
    cancelados: meusPdis.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.CANCELADO).length,
  }), [meusPdis]);

  const ativosPdi = useMemo(
    () => meusPdis.filter((p) => mapPdiStatusToLabel(p.status) !== PdiStatusLabel.FINALIZADO && mapPdiStatusToLabel(p.status) !== PdiStatusLabel.CANCELADO),
    [meusPdis]
  );
  const historicoPdi = useMemo(
    () =>
      meusPdis.filter((p) => {
        const label = mapPdiStatusToLabel(p.status);
        return label === PdiStatusLabel.FINALIZADO || label === PdiStatusLabel.CANCELADO;
      }),
    [meusPdis]
  );

  // Expandir o primeiro registro 1:1 apenas na primeira carga dos dados (não reabrir ao fechar)
  useEffect(() => {
    if (registros1a1.length > 0 && !initialRegistroExpandDone.current) {
      initialRegistroExpandDone.current = true;
      setExpandedRegistro(registros1a1[0].id);
    }
    if (registros1a1.length === 0) {
      initialRegistroExpandDone.current = false;
    }
  }, [registros1a1]);

  if (loadingEstatisticas || loadingRegistros) {
    return (
      <div className="container mx-auto p-4" data-testid="gestao-desempenho-colaborador-page-loading">
        <div className="text-center py-8 text-muted-foreground">Carregando...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-4 space-y-6">
      <GuideTour
        steps={gestaoDesempenhoColaboradorSteps}
        run={tourRunning}
        onCallback={handleJoyrideCallback}
      />

      {/* Header */}
      <div className="flex items-start justify-between gap-4 flex-wrap">
        <div>
          <div className="flex items-center gap-2 mb-2">
            <TrendingUp className="h-6 w-6 text-green-600" />
            <h1 className="text-3xl font-bold">Gestão de Desempenho</h1>
          </div>
          <p className="text-muted-foreground">
            Visualize seus feedbacks, conversas e desenvolva seu PDI
          </p>
        </div>
        <TourButton onStart={startTour} data-testid="gestao-desempenho-colaborador-tour-button" />
      </div>

      {/* Painel e Registros Críticos */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        {/* Meu Painel */}
        <Card className="lg:col-span-8">
          <CardContent className="p-6">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-lg font-semibold">Meu Painel</h2>
              <p className="text-sm text-muted-foreground">Visão geral do seu desempenho</p>
            </div>
            <div className="grid grid-cols-3 gap-4">
              <StatCard
                title="Feedbacks"
                value={estatisticas?.totalFeedbacks.toString() || '0'}
                icon={MessageCircle}
                color="text-green-600"
                bgColor="bg-green-100"
                vertical
              />
              <StatCard
                title="Registros 1:1"
                value={estatisticas?.totalRegistros1a1.toString() || '0'}
                icon={Users}
                color="text-green-600"
                bgColor="bg-green-100"
                vertical
              />
              <StatCard
                title="Metas PDI"
                value={meusPdis.length.toString()}
                icon={Target}
                color="text-green-600"
                bgColor="bg-green-100"
                vertical
              />
            </div>
          </CardContent>
        </Card>

        {/* Registros Críticos */}
        <Card className="lg:col-span-4">
          <CardContent className="p-6">
            <div className="flex items-center gap-2 mb-4">
              <AlertTriangle className="h-5 w-5 text-orange-600" />
              <h2 className="text-lg font-semibold">Registros Críticos</h2>
              {registros.length > 0 && (
                <Badge className="bg-red-500 text-white border-0 rounded-full h-5 w-5 p-0 flex items-center justify-center text-xs ml-auto">
                  {registros.length}
                </Badge>
              )}
            </div>
            <div className="space-y-3">
              {registros.length === 0 ? (
                <div className="bg-orange-50 rounded-lg p-4" data-testid="gestao-desempenho-colaborador-page-registros-criticos-empty">
                  <p className="text-muted-foreground text-sm">Nenhum registro crítico</p>
                </div>
              ) : (
                registros.map((registro: RegistroCritico) => (
                  <div
                    key={registro.id}
                    className="bg-orange-50 rounded-lg p-4"
                    data-testid={`gestao-desempenho-colaborador-page-registro-critico-${registro.id}`}
                  >
                    <div className="space-y-2">
                      <div className="flex items-center gap-2">
                        <Calendar className="h-4 w-4 text-muted-foreground flex-shrink-0" />
                        <span className="text-sm text-muted-foreground">
                          {formatData(registro.data)}
                        </span>
                      </div>
                      <p className="text-sm text-muted-foreground line-clamp-2 leading-relaxed">
                        {registro.descricao}
                      </p>
                    </div>
                  </div>
                ))
              )}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Tabs */}
      <Tabs value={activeTab} onValueChange={handleTabChange} data-testid="gestao-desempenho-colaborador-page-tabs">
        <TabsList>
          <TabsTrigger value="feedbacks" data-testid="gestao-desempenho-colaborador-page-feedbacks-tab">
            <MessageCircle className="h-4 w-4 mr-2" />
            Feedbacks {feedbacks.length > 0 && `(${feedbacks.length})`}
          </TabsTrigger>
          <TabsTrigger value="registros-1-1" data-testid="gestao-desempenho-colaborador-page-registros-1-1-tab">
            <Users className="h-4 w-4 mr-2" />
            Registros 1:1 {registros1a1.length > 0 && `(${registros1a1.length})`}
          </TabsTrigger>
          <TabsTrigger value="pdi" data-testid="gestao-desempenho-colaborador-page-pdi-tab">
            <Target className="h-4 w-4 mr-2" />
            PDI {meusPdis.length > 0 && `(${meusPdis.length})`}
          </TabsTrigger>
        </TabsList>

        {/* Tab Feedbacks */}
        <TabsContent value="feedbacks" className="space-y-4">
          {/* Cards de Resumo */}
          <div className="grid grid-cols-2 gap-4">
            <Card>
              <CardContent className="p-6">
                <div className="flex items-center gap-4">
                  <div className="bg-green-100 rounded-full p-3">
                    <Eye className="h-6 w-6 text-green-600" />
                  </div>
                  <div className="flex flex-col">
                    <p className="text-3xl font-bold text-green-600">{feedbacksVistos}</p>
                    <p className="text-sm text-muted-foreground">Vistos</p>
                  </div>
                </div>
              </CardContent>
            </Card>
            <Card>
              <CardContent className="p-6">
                <div className="flex items-center gap-4">
                  <div className="bg-gray-100 rounded-full p-3">
                    <EyeOff className="h-6 w-6 text-gray-600" />
                  </div>
                  <div className="flex flex-col">
                    <p className="text-3xl font-bold text-gray-600">{feedbacksNaoVistos}</p>
                    <p className="text-sm text-muted-foreground">Não vistos</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Lista de Feedbacks */}
          <div className="space-y-4">
            {feedbacks.map((feedback) => {
              const isExpanded = expandedFeedback === feedback.id;
              const temAcoes = (feedback.continuar && feedback.continuar.length > 0) || 
                              (feedback.comecar && feedback.comecar.length > 0) || 
                              (feedback.parar && feedback.parar.length > 0);
              
              return (
                <Card key={feedback.id} data-testid={`gestao-desempenho-colaborador-page-feedback-card-${feedback.id}`}>
                  <CardContent className="p-6">
                    <div 
                      className="flex items-start justify-between mb-4 cursor-pointer"
                      onClick={async () => {
                        // Se clicar no mesmo, fecha. Se clicar em outro, abre ele e fecha os demais
                        const novoEstado = isExpanded ? null : feedback.id;
                        setExpandedFeedback(novoEstado);
                        
                        // Se está expandindo e o feedback não foi visualizado, marcar como visualizado
                        if (novoEstado && !feedback.visto && token) {
                          try {
                            const useCase = container.resolve(InserirVisualizacaoFeedbackUseCase);
                            await useCase.execute(token, feedback.id);
                            // Refetch para atualizar os contadores
                            await refetchFeedbacks();
                          } catch (error) {
                            console.error('Erro ao marcar feedback como visualizado:', error);
                          }
                        }
                      }}
                    >
                      <div className="flex items-center gap-2 flex-1">
                        <Calendar className="h-5 w-5 text-green-600" />
                        <div className="flex-1">
                          <h3 className="text-lg font-semibold">
                            Feedback - {formatData(feedback.data)}
                          </h3>
                          <p className="text-muted-foreground mt-1">{feedback.resumo}</p>
                        </div>
                      </div>
                      {feedback.visto && feedback.vistoEm && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <Eye className="h-4 w-4" />
                          <span>Visto em {format(new Date(feedback.vistoEm), "dd/MM/yyyy, HH:mm", { locale: ptBR })}</span>
                          <div className="ml-2">
                            {isExpanded ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
                          </div>
                        </div>
                      )}
                      {!feedback.visto && (
                        <div className="ml-2">
                          {isExpanded ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
                        </div>
                      )}
                    </div>

                    {/* Conteúdo expandido - aparece apenas quando expandido */}
                    {isExpanded && (
                      <div className="space-y-4 mt-4 pt-4 border-t">
                        <div className="flex items-center gap-2 mb-4">
                          <Person className="h-4 w-4 text-muted-foreground" />
                          <span className="text-sm text-muted-foreground">
                            Realizado por: {feedback.realizadoPor}
                          </span>
                        </div>

                        {/* Cards de ações */}
                        {temAcoes && (
                          <div className="space-y-4">
                            <div className="grid grid-cols-3 gap-4">
                              {feedback.continuar && feedback.continuar.length > 0 && (
                                <Card className="border-green-200 bg-green-50">
                                  <CardContent className="p-4">
                                    <div className="flex items-center gap-2 mb-2">
                                      <Play className="h-5 w-5 text-green-600" />
                                      <h4 className="font-semibold text-green-600">Continuar</h4>
                                    </div>
                                    <ul className="space-y-1">
                                      {feedback.continuar.map((item, idx) => (
                                        <li key={idx} className="text-sm text-muted-foreground">
                                          {item}
                                        </li>
                                      ))}
                                    </ul>
                                  </CardContent>
                                </Card>
                              )}
                              {feedback.comecar && feedback.comecar.length > 0 && (
                                <Card className="border-blue-200 bg-blue-50">
                                  <CardContent className="p-4">
                                    <div className="flex items-center gap-2 mb-2">
                                      <ChevronRight className="h-5 w-5 text-blue-600" />
                                      <h4 className="font-semibold text-blue-600">Começar</h4>
                                    </div>
                                    <ul className="space-y-1">
                                      {feedback.comecar.map((item, idx) => (
                                        <li key={idx} className="text-sm text-muted-foreground">
                                          {item}
                                        </li>
                                      ))}
                                    </ul>
                                  </CardContent>
                                </Card>
                              )}
                              {feedback.parar && feedback.parar.length > 0 && (
                                <Card className="border-red-200 bg-red-50">
                                  <CardContent className="p-4">
                                    <div className="flex items-center gap-2 mb-2">
                                      <Stop className="h-5 w-5 text-red-600" />
                                      <h4 className="font-semibold text-red-600">Parar</h4>
                                    </div>
                                    <ul className="space-y-1">
                                      {feedback.parar.map((item, idx) => (
                                        <li key={idx} className="text-sm text-muted-foreground">
                                          {item}
                                        </li>
                                      ))}
                                    </ul>
                                  </CardContent>
                                </Card>
                              )}
                            </div>
                            {feedback.observacoesGerais && (
                              <Card className="border-gray-200 bg-gray-50">
                                <CardContent className="p-4">
                                  <h4 className="font-semibold mb-2">Observações gerais</h4>
                                  <p className="text-sm text-muted-foreground">{feedback.observacoesGerais}</p>
                                </CardContent>
                              </Card>
                            )}
                          </div>
                        )}
                      </div>
                    )}
                  </CardContent>
                </Card>
              );
            })}
          </div>
        </TabsContent>

        {/* Tab Registros 1:1 */}
        <TabsContent value="registros-1-1" className="space-y-4">
          {/* Cards de Resumo */}
          <div className="grid grid-cols-2 gap-4">
            <Card>
              <CardContent className="p-6">
                <div className="flex items-center gap-4">
                  <div className="bg-green-100 rounded-full p-3">
                    <Eye className="h-6 w-6 text-green-600" />
                  </div>
                  <div className="flex flex-col">
                    <p className="text-3xl font-bold text-green-600">{registrosVistos}</p>
                    <p className="text-sm text-muted-foreground">Vistos</p>
                  </div>
                </div>
              </CardContent>
            </Card>
            <Card>
              <CardContent className="p-6">
                <div className="flex items-center gap-4">
                  <div className="bg-orange-100 rounded-full p-3">
                    <EyeOff className="h-6 w-6 text-orange-600" />
                  </div>
                  <div className="flex flex-col">
                    <p className="text-3xl font-bold text-orange-600">{registrosNaoVistos}</p>
                    <p className="text-sm text-muted-foreground">Não vistos</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Pautas Sugeridas */}
          <Card>
            <CardContent className="p-6">
              <div className="flex items-center justify-between mb-4">
                <div className="flex items-center gap-2">
                  <FileText className="h-5 w-5 text-green-600" />
                  <h3 className="text-lg font-semibold">Pautas Sugeridas</h3>
                  {pautas.length > 0 && <Badge variant="outline">{pautas.length}</Badge>}
                </div>
                <Button variant="outline" size="sm" onClick={() => setDialogPautaOpen(true)} data-testid="gestao-desempenho-colaborador-page-adicionar-pauta-button">
                  <Plus className="h-4 w-4 mr-2" />
                  Adicionar Pauta
                </Button>
              </div>
              {pautas.length > 0 ? (
                <ul className="space-y-2">
                  {pautas.map((pauta) => {
                    const mostrarTag = pauta.origem === "COLABORADOR" || pauta.origem === "SUBORDINADO";
                    return (
                      <li key={pauta.id} className="flex items-start gap-2 text-sm text-muted-foreground">
                        <span className="w-2 h-2 bg-green-600 rounded-full mt-1.5 shrink-0"></span>
                        <span className="whitespace-pre-wrap flex-1">{pauta.descricao}</span>
                        {mostrarTag && (
                          <Badge variant="secondary" className="text-xs">
                            {pauta.origem}
                          </Badge>
                        )}
                      </li>
                    );
                  })}
                </ul>
              ) : (
                <p className="text-sm text-muted-foreground" data-testid="gestao-desempenho-colaborador-page-pautas-empty">Nenhuma pauta sugerida</p>
              )}
            </CardContent>
          </Card>

          {/* Lista de Registros 1:1 */}
          <div className="space-y-4">
            {registros1a1.map((registro) => {
              const isExpanded = expandedRegistro === registro.id;
              
              return (
                <Card key={registro.id} data-testid={`gestao-desempenho-colaborador-page-registro-1-1-card-${registro.id}`}>
                  <CardContent className="p-6">
                    <div 
                      className="flex items-start justify-between mb-4 cursor-pointer"
                      onClick={() => {
                        setExpandedRegistro(isExpanded ? null : registro.id);
                      }}
                    >
                      <div className="flex items-center gap-2 flex-1 min-w-0">
                        {registro.critico ? (
                          <AlertTriangle className="h-5 w-5 shrink-0 text-orange-600" />
                        ) : (
                          <Calendar className="h-5 w-5 shrink-0 text-green-600" />
                        )}
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2">
                            <h3 className="text-lg font-semibold">
                              1:1 - {formatData(registro.data)}
                            </h3>
                            {registro.critico && (
                              <Badge className="bg-red-500 text-white">Crítico</Badge>
                            )}
                          </div>
                          {/* Resumo não é exibido aqui: mesma fonte que Anotações (descricaoAnotacoes), exibida só na seção expandida */}
                        </div>
                      </div>
                      {registro.visto && registro.vistoEm ? (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground shrink-0">
                          <Eye className="h-4 w-4" />
                          <span>Visto em {format(new Date(registro.vistoEm), "dd/MM/yyyy, HH:mm", { locale: ptBR })}</span>
                          <button
                            type="button"
                            className="ml-2 p-1 rounded hover:bg-muted focus:outline-none focus:ring-2 focus:ring-primary"
                            onClick={(e) => {
                              e.stopPropagation();
                              setExpandedRegistro(isExpanded ? null : registro.id);
                            }}
                            aria-expanded={isExpanded}
                            aria-label={isExpanded ? 'Fechar registro 1:1' : 'Abrir registro 1:1'}
                          >
                            {isExpanded ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
                          </button>
                        </div>
                      ) : (
                        <button
                          type="button"
                          className="ml-2 p-1 rounded hover:bg-muted focus:outline-none focus:ring-2 focus:ring-primary shrink-0"
                          onClick={(e) => {
                            e.stopPropagation();
                            setExpandedRegistro(isExpanded ? null : registro.id);
                          }}
                          aria-expanded={isExpanded}
                          aria-label={isExpanded ? 'Fechar registro 1:1' : 'Abrir registro 1:1'}
                        >
                          {isExpanded ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
                        </button>
                      )}
                    </div>
                    
                    {/* Conteúdo expandido */}
                    {isExpanded && (
                      <div className="space-y-4 mt-4 pt-4 border-t">
                        <div className="flex items-center gap-2">
                          <Person className="h-4 w-4 text-muted-foreground" />
                          <span className="text-sm text-muted-foreground">
                            Realizado por: {registro.realizadoPor}
                          </span>
                        </div>
                        {registro.pautaSugerida && registro.pautaSugerida.length > 0 && (
                          <div>
                            <div className="flex items-center gap-2 mb-2">
                              <FileText className="h-4 w-4 text-green-600" />
                              <h4 className="font-semibold">Pauta Sugerida</h4>
                            </div>
                            <ul className="space-y-1 ml-6">
                              {registro.pautaSugerida.map((pauta, idx) => (
                                <li key={idx} className="text-sm text-muted-foreground">
                                  {pauta}
                                </li>
                              ))}
                            </ul>
                          </div>
                        )}
                        {registro.anotacoes && (
                          <div>
                            <h4 className="font-semibold mb-2">Anotações da Reunião</h4>
                            <p className="text-sm text-muted-foreground">{registro.anotacoes}</p>
                          </div>
                        )}
                      </div>
                    )}
                  </CardContent>
                </Card>
              );
            })}
          </div>
        </TabsContent>

        {/* Tab PDI — conforme segundo print: 5 cards de status, Ativos, Histórico */}
        <TabsContent value="pdi" className="space-y-6">
          <div className="grid grid-cols-2 sm:grid-cols-5 gap-4">
            {[
              { label: PdiStatusLabel.NAO_INICIADO, value: contadoresPdi.naoIniciados, icon: Clock, bg: 'bg-amber-50 dark:bg-amber-950/30', border: 'border-amber-200 dark:border-amber-800', text: 'text-amber-700 dark:text-amber-400', iconBg: 'bg-amber-200/80 dark:bg-amber-800/60', iconRing: 'ring-amber-300 dark:ring-amber-700' },
              { label: PdiStatusLabel.EM_ANALISE, value: contadoresPdi.emAnalise, icon: AlertCircle, bg: 'bg-blue-50 dark:bg-blue-950/30', border: 'border-blue-200 dark:border-blue-800', text: 'text-blue-700 dark:text-blue-400', iconBg: 'bg-blue-200/80 dark:bg-blue-800/60', iconRing: 'ring-blue-300 dark:ring-blue-700' },
              { label: PdiStatusLabel.IN_PROGRESS, value: contadoresPdi.emAndamento, icon: Play, bg: 'bg-primary/10 dark:bg-primary/20', border: 'border-primary/40', text: 'text-primary', iconBg: 'bg-primary/30 dark:bg-primary/40', iconRing: 'ring-primary/50' },
              { label: PdiStatusLabel.FINALIZADO, value: contadoresPdi.finalizados, icon: CheckCircle, bg: 'bg-green-50 dark:bg-green-950/30', border: 'border-green-200 dark:border-green-800', text: 'text-green-700 dark:text-green-400', iconBg: 'bg-green-200/80 dark:bg-green-800/60', iconRing: 'ring-green-300 dark:ring-green-700' },
              { label: PdiStatusLabel.CANCELADO, value: contadoresPdi.cancelados, icon: XCircle, bg: 'bg-slate-50 dark:bg-slate-900/50', border: 'border-slate-200 dark:border-slate-700', text: 'text-slate-600 dark:text-slate-400', iconBg: 'bg-slate-200/80 dark:bg-slate-700/60', iconRing: 'ring-slate-300 dark:ring-slate-600' },
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

          <PdiTableSection<PdiResumoDTO>
            title="Ativos"
            subtitle="Acompanhe o status dos seus PDI's"
            data-testid="gestao-desempenho-colaborador-page-pdi-ativos"
            topButton={
              <Button onClick={() => navigate('/pdi/novo')} data-testid="gestao-desempenho-colaborador-page-criar-pdi-button">
                <Target className="h-4 w-4 mr-2" />
                Criar PDI
              </Button>
            }
            columns={[
              { id: 'status', label: 'Status' },
              { id: 'titulo', label: 'Título' },
              { id: 'previsao', label: 'Previsão de conclusão' },
              { id: 'andamento', label: 'Andamento' },
              { id: 'criadoPor', label: 'Criado por' },
              { id: 'acoes', label: 'Ações', align: 'right' },
            ]}
            data={ativosPdi}
            keyExtractor={(row) => row.id}
            loading={loadingMeusPdis}
            emptyMessage="Fale com seu gestor para que construam em conjunto seu Plano de Desenvolvimento."
            emptyContent={
              <>
                <p className="mb-2">Fale com seu gestor para que construam em conjunto seu Plano de Desenvolvimento.</p>
                <Button variant="outline" className="mt-4" onClick={() => navigate('/pdi/novo')} data-testid="gestao-desempenho-colaborador-page-empty-criar-pdi-button">
                  Criar PDI
                </Button>
              </>
            }
            renderCell={(pdi, columnId) => {
              const statusLabel = mapPdiStatusToLabel(pdi.status);
              const badgeClass = statusBadgeClassPdi[statusLabel] ?? 'bg-muted text-muted-foreground border border-border';
              const isNaoIniciado = statusLabel === PdiStatusLabel.NAO_INICIADO;
              const isEmAndamento = statusLabel === PdiStatusLabel.IN_PROGRESS;
              const actionLabel = isNaoIniciado ? 'Concluir planejamento' : isEmAndamento ? 'Atualizar andamento' : 'Visualizar';
              const ActionIcon = isNaoIniciado ? CheckCircle : isEmAndamento ? RefreshCw : Eye;
              const openStep = isNaoIniciado ? 2 : isEmAndamento ? 4 : undefined;
              const criadoPorGestor = !!(pdi.codigoInternoColaboradorCriacao && pdi.codigoInternoColaboradorCriacao !== codigoInternoUsuario);
              switch (columnId) {
                case 'status':
                  return <Badge className={cn('font-medium rounded-full px-2.5 py-0.5', badgeClass)}>{statusLabel}</Badge>;
                case 'titulo':
                  return <span className="font-medium text-foreground">{pdi.titulo}</span>;
                case 'previsao': {
                  // Usar deadLine do PDI (previsão definida na criação) ou o maior prazo dos planos de ação
                  const fromPdi = pdi.deadLine?.trim().slice(0, 10);
                  const prazos = (pdi.actionPlans ?? []).filter((ap) => ap.deadline).map((ap) => ap.deadline!);
                  const dateOnly = fromPdi ?? (prazos.length > 0
                    ? prazos.sort((a, b) => new Date(b).getTime() - new Date(a).getTime())[0].split('T')[0]
                    : null);
                  if (!dateOnly) return <span className="text-muted-foreground">—</span>;
                  const [y, m, d] = dateOnly.split('-').map(Number);
                  if (!y || !m || !d) return <span className="text-muted-foreground">—</span>;
                  const dLocal = new Date(y, m - 1, d);
                  if (Number.isNaN(dLocal.getTime())) return <span className="text-muted-foreground">—</span>;
                  return <span className="text-muted-foreground">{format(dLocal, 'dd/MM/yyyy', { locale: ptBR })}</span>;
                }
                case 'andamento':
                  return (
                    <div className="flex items-center gap-2">
                      <div className="w-20 h-2 bg-muted rounded-full overflow-hidden">
                        <div className="h-full bg-primary transition-all" style={{ width: `${Math.round((pdi.progress ?? 0) * 100)}%` }} />
                      </div>
                      <span className="tabular-nums text-muted-foreground">{Math.round((pdi.progress ?? 0) * 100)}%</span>
                    </div>
                  );
                case 'criadoPor':
                  return (
                    <Badge variant="secondary" className="font-normal rounded-md px-2 py-0.5 bg-muted text-muted-foreground border border-border">
                      {criadoPorGestor ? 'Gestor' : 'Colaborador'}
                    </Badge>
                  );
                case 'acoes':
                  return (
                    <div className="flex items-center justify-end gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        className="shrink-0"
                        data-testid={`gestao-desempenho-colaborador-page-pdi-acao-button-${pdi.id}`}
                        onClick={(e) => {
                          e.stopPropagation();
                          navigate(`/pdi/${pdi.id}`, {
                            state: {
                              isMeuPdi: true,
                              from: '/gestao-desempenho-colaborador',
                              criadoPorGestor,
                              ...(openStep != null && { openStep }),
                            },
                          });
                        }}
                      >
                        <ActionIcon className="h-4 w-4 mr-1.5" />
                        {actionLabel}
                      </Button>
                      {isEmAndamento && (
                        <Button variant="ghost" size="sm" className="shrink-0" data-testid={`gestao-desempenho-colaborador-page-pdi-editar-button-${pdi.id}`} onClick={(e) => { e.stopPropagation(); navigate(`/pdi/${pdi.id}`, { state: { isMeuPdi: true, from: '/gestao-desempenho-colaborador', openStep: 2, criadoPorGestor } }); }}>
                          <Edit className="h-4 w-4 mr-1.5" />
                          Editar
                        </Button>
                      )}
                    </div>
                  );
                default:
                  return null;
              }
            }}
          />

          <PdiTableSection<PdiResumoDTO>
            title="Histórico"
            subtitle="Acompanhe a sua evolução de cada meta conquistada"
            data-testid="gestao-desempenho-colaborador-page-pdi-historico"
            columns={[
              { id: 'status', label: 'Status' },
              { id: 'titulo', label: 'Título' },
              { id: 'previsao', label: 'Previsão conclusão' },
              { id: 'concluidoEm', label: 'Concluído em' },
              { id: 'criadoPor', label: 'Criado por' },
              { id: 'acoes', label: 'Ações', align: 'right' },
            ]}
            data={historicoPdi}
            keyExtractor={(row) => row.id}
            loading={loadingMeusPdis}
            emptyMessage="Fale com seu gestor para que construam em conjunto seu Plano de Desenvolvimento."
            renderCell={(pdi, columnId) => {
              const statusLabel = mapPdiStatusToLabel(pdi.status);
              const badgeClass = statusBadgeClassPdi[statusLabel] ?? 'bg-muted text-muted-foreground border border-border';
              const criadoPorGestor = !!(pdi.codigoInternoColaboradorCriacao && pdi.codigoInternoColaboradorCriacao !== codigoInternoUsuario);
              switch (columnId) {
                case 'status':
                  return <Badge className={cn('font-medium rounded-full px-2.5 py-0.5', badgeClass)}>{statusLabel}</Badge>;
                case 'titulo':
                  return <span className="font-medium text-foreground">{pdi.titulo}</span>;
                case 'previsao':
                  return <span className="text-muted-foreground">{pdi.dataCriacao ? format(new Date(pdi.dataCriacao), 'dd/MM/yyyy', { locale: ptBR }) : '-'}</span>;
                case 'concluidoEm':
                  return <span className="text-muted-foreground">{pdi.dataAtualizacao ? format(new Date(pdi.dataAtualizacao), 'dd/MM/yyyy', { locale: ptBR }) : '-'}</span>;
                case 'criadoPor':
                  return (
                    <Badge variant="secondary" className="font-normal rounded-md px-2 py-0.5 bg-muted text-muted-foreground border border-border">
                      {criadoPorGestor ? 'Gestor' : 'Colaborador'}
                    </Badge>
                  );
                case 'acoes':
                  return (
                    <Button variant="outline" size="sm" className="shrink-0" data-testid={`gestao-desempenho-colaborador-page-pdi-visualizar-button-${pdi.id}`} onClick={(e) => { e.stopPropagation(); navigate(`/pdi/${pdi.id}`, { state: { isMeuPdi: true, from: '/gestao-desempenho-colaborador', apenasVisualizar: true, criadoPorGestor } }); }}>
                      <Eye className="h-4 w-4 mr-1.5" />
                      Visualizar
                    </Button>
                  );
                default:
                  return null;
              }
            }}
          />
        </TabsContent>
      </Tabs>

      {/* Dialog Adicionar Pauta */}
      <Dialog open={dialogPautaOpen} onOpenChange={setDialogPautaOpen}>
        <DialogContent
          className="sm:max-w-[680px] !max-h-[90vh] flex flex-col overflow-hidden p-6 gap-4"
          data-testid="gestao-desempenho-colaborador-page-pauta-modal"
        >
          <DialogHeader className="flex-shrink-0">
            <DialogTitle>Adicionar Pauta Sugerida</DialogTitle>
            <DialogDescription>
              Lembretes de tópicos para a reunião (visível para gestor e colaborador).
            </DialogDescription>
          </DialogHeader>
          <div className="min-h-0 flex-1 overflow-y-auto">
            <div className="max-h-[50vh] overflow-y-auto rounded-lg">
              <PautaSugeridaEditor
                key={dialogPautaOpen ? 'open' : 'closed'}
                content={pautaTexto}
                onChange={setPautaTexto}
                minHeight="min-h-[180px]"
                data-testid="gestao-desempenho-colaborador-page-pauta-textarea"
              />
            </div>
          </div>
          <div className="flex justify-end gap-2 flex-shrink-0 border-t border-border pt-4">
              <Button
                variant="outline"
                data-testid="gestao-desempenho-colaborador-page-pauta-modal-cancelar-button"
                onClick={() => {
                  setDialogPautaOpen(false);
                  setPautaTexto('');
                }}
              >
                Cancelar
              </Button>
              <Button
                data-testid="gestao-desempenho-colaborador-page-pauta-modal-salvar-button"
                onClick={async () => {
                  const texto = htmlToPlainText(pautaTexto);
                  if (!texto || !token) {
                    return;
                  }

                  const codigoGestor = user?.orgHierarquia?.codigoInternoProfissionalSuperior;
                  if (!codigoGestor) {
                    toast.error('Erro: código do gestor não encontrado.');
                    return;
                  }

                  setSalvandoPauta(true);
                  try {
                    const useCase = container.resolve(InserirPautaSugeridaColaboradorUseCase);
                    const payload = {
                      codigoInternoColaboradorSuperior: codigoGestor,
                      descricaoPautaSugerida: texto,
                    };

                    const response = await useCase.execute(token, payload);

                    if (response.sucesso) {
                      toast.success('Pauta sugerida salva com sucesso!');
                      setDialogPautaOpen(false);
                      setPautaTexto('');
                      // Refetch para atualizar a lista de pautas
                      await refetchPautas();
                    } else {
                      toast.error(response.mensagem || 'Erro ao salvar pauta sugerida');
                    }
                  } catch (error: any) {
                    console.error('Erro ao salvar pauta:', error);
                    toast.error(error.message || 'Erro ao salvar pauta sugerida. Tente novamente.');
                  } finally {
                    setSalvandoPauta(false);
                  }
                }}
                disabled={!pautaTexto.trim() || salvandoPauta}
              >
                {salvandoPauta ? 'Salvando...' : 'Salvar Pauta'}
              </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}

