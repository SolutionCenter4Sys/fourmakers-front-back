import { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { container } from '@core/di/container';
import { InserirPautaSugeridaUseCase } from '@domain/usecases/InserirPautaSugeridaUseCase';
import { useAppSelector } from '@app/store/hooks';
import { toast } from 'sonner';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { 
  ArrowLeft, 
  Briefcase, 
  Mail, 
  Phone, 
  Calendar, 
  Clock, 
  DollarSign,
  Plus,
  ChevronDown,
  MessageCircle,
  Users,
  Target,
  Eye,
  EyeOff,
  ChevronUp,
  ChevronRight,
  Play,
  Stop,
  User,
  FileText,
  AlertTriangle,
  AlertCircle,
  CheckCircle,
  RefreshCw,
  XCircle,
} from '@/components/ui/system-icons';
import { 
  useColaboradorDesempenhoDetalhes, 
  useFeedbacks, 
  useRegistrosUmAUm, 
  usePDIs,
  usePautasSugeridas
} from '@/presentation/hooks/useGestaoDesempenho';
import type { PdiResumoTimeDTO } from '@shared/types/pdiApi';
import { mapPdiStatusToLabel, PdiStatusLabel } from '@shared/types/pdiApi';
import { cn } from '@/lib/utils';
import { PdiTableSection } from '@/presentation/components/gestaoDesempenho/PdiTableSection';
import { formatData, formatTelefone, formatSalario, formatSaldoHoras } from '@shared/utils/bookColaboradorFormatters';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from '@/components/ui/dialog';
import { PautaSugeridaEditor, htmlToPlainText } from '@/presentation/components/gestaoDesempenho/PautaSugeridaEditor';
import { DialogNovoFeedback } from '@/presentation/components/gestaoDesempenho/DialogNovoFeedback';
import { DialogNovoRegistro1on1 } from '@/presentation/components/gestaoDesempenho/DialogNovoRegistro1on1';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';

export default function GestaoDesempenhoColaboradorDetalhes() {
  const { codColaborador } = useParams<{ codColaborador: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const pathname = location.pathname;
  const isRhContext = pathname.startsWith('/gestao-desempenho-rh');
  const { token, user } = useAppSelector((state) => state.auth);
  const orgId = user?.colaboradorOrg?.orgId ?? 0;

  // Obter rota de origem: RH mantém rota dentro de gestao-desempenho-rh
  const rotaOrigem = isRhContext ? '/gestao-desempenho-rh' : ((location.state as { from?: string })?.from || '/gestao-desempenho-gestor');
  const basePathPerfil = isRhContext ? `/gestao-desempenho-rh/colaborador/${codColaborador}` : `/gestao-desempenho-gestor/${codColaborador}`;
  const { detalhes, loading: loadingDetalhes } = useColaboradorDesempenhoDetalhes(codColaborador || '');
  const { feedbacks, feedbacksVistos, feedbacksNaoVistos, refetch: refetchFeedbacks } = useFeedbacks(codColaborador || '');
  const { registros, registrosVistos, registrosNaoVistos, refetch: refetchRegistros } = useRegistrosUmAUm(codColaborador || '');
  const { pdisResumoTime, contadoresPdi, pdisAtivos, pdisHistorico, loading: loadingPdis } = usePDIs(codColaborador || '');
  const { pautas, refetch: refetchPautas } = usePautasSugeridas(codColaborador || '');
  // Ao voltar do fluxo do PDI, state.openTab === 'pdi' para abrir já na aba PDI
  const [activeTab, setActiveTab] = useState(() =>
    (location.state as { openTab?: string })?.openTab === 'pdi' ? 'pdi' : 'feedbacks'
  );

  useEffect(() => {
    const openTab = (location.state as { openTab?: string })?.openTab;
    if (openTab === 'pdi' || openTab === 'feedbacks' || openTab === 'registros-1-1') setActiveTab(openTab);
  }, [location.state]);

  const statusBadgeClassPdi: Record<string, string> = {
    [PdiStatusLabel.NAO_INICIADO]: 'bg-amber-50 text-amber-800 border-2 border-amber-200 dark:bg-amber-950/30 dark:text-amber-300 dark:border-amber-700',
    [PdiStatusLabel.EM_ANALISE]: 'bg-blue-50 text-blue-800 border-2 border-blue-200 dark:bg-blue-950/30 dark:text-blue-300 dark:border-blue-700',
    [PdiStatusLabel.IN_PROGRESS]: 'bg-primary/10 text-primary border-2 border-primary/40 dark:bg-primary/20 dark:border-primary/50',
    [PdiStatusLabel.FINALIZADO]: 'bg-green-50 text-green-800 border-2 border-green-200 dark:bg-green-950/30 dark:text-green-300 dark:border-green-700',
    [PdiStatusLabel.CANCELADO]: 'bg-slate-50 text-slate-700 border-2 border-slate-200 dark:bg-slate-800/50 dark:text-slate-300 dark:border-slate-600',
  };
  const renderPdiCell = (row: PdiResumoTimeDTO, columnId: string) => {
    switch (columnId) {
      case 'status': {
        const label = mapPdiStatusToLabel(row.status);
        const badgeClass = statusBadgeClassPdi[label] ?? 'bg-muted text-muted-foreground';
        return <Badge className={cn(badgeClass, 'rounded-full px-2.5 py-0.5 font-medium')}>{label}</Badge>;
      }
      case 'titulo':
        return <span className="font-medium text-foreground">{row.titulo}</span>;
      case 'fourTalent':
        return <span className="block min-w-0">{detalhes?.nome ?? '-'}</span>;
      case 'previsaoConclusao':
        return <span className="text-muted-foreground" title="Previsão de conclusão">{row.previsaoConclusao ?? '-'}</span>;
      case 'andamento':
        return (
          <div className="flex items-center gap-2">
            <div className="w-20 h-2 bg-muted rounded-full overflow-hidden">
              <div className="h-full bg-primary transition-all" style={{ width: `${Math.round((row.progress ?? 0) * 100)}%` }} />
            </div>
            <span className="tabular-nums text-muted-foreground">{Math.round((row.progress ?? 0) * 100)}%</span>
          </div>
        );
      case 'criadoPor': {
        // PDI do colaborador: criado por Gestor se o criador não é o próprio colaborador (dono do PDI)
        const criadoPeloColaborador =
          row.codigoInternoColaboradorCriacao != null &&
          codColaborador != null &&
          (row.codigoInternoColaboradorCriacao === codColaborador || row.codigoInternoColaboradorCriacao === row.colaboradorId);
        return (
          <Badge variant="secondary" className="font-normal rounded-full">
            {criadoPeloColaborador ? 'Colaborador' : 'Gestor'}
          </Badge>
        );
      }
      case 'acoes': {
        // RH: apenas visualizar PDI (sem criar, concluir ou atualizar andamento)
        if (isRhContext) {
          return (
            <Button
              variant="outline"
              size="sm"
              className="gap-1 shrink-0"
              onClick={(e) => {
                e.stopPropagation();
                navigate(`${basePathPerfil}/pdi/${row.pdiId}`, {
                  state: {
                    from: rotaOrigem,
                    colaboradorNome: detalhes?.nome,
                    apenasVisualizar: true,
                  },
                });
              }}
            >
              <Eye className="h-4 w-4" />
              Visualizar
            </Button>
          );
        }
        const statusLabel = mapPdiStatusToLabel(row.status);
        const isNaoIniciado = statusLabel === PdiStatusLabel.NAO_INICIADO;
        const isEmAndamento = statusLabel === PdiStatusLabel.IN_PROGRESS;
        const actionLabel = isNaoIniciado ? 'Concluir planejamento' : isEmAndamento ? 'Atualizar andamento' : 'Visualizar';
        const openStep = isNaoIniciado ? 2 : isEmAndamento ? 4 : undefined;
        const apenasVisualizar = !isNaoIniciado && !isEmAndamento && (row.status || '').toLowerCase() === 'finalizado';
        const ActionIcon = isNaoIniciado ? CheckCircle : isEmAndamento ? RefreshCw : Eye;
        return (
          <Button
            variant="outline"
            size="sm"
            className="gap-1 shrink-0"
            onClick={(e) => {
              e.stopPropagation();
              navigate(`${basePathPerfil}/pdi/${row.pdiId}`, {
                state: {
                  from: rotaOrigem,
                  colaboradorNome: detalhes?.nome,
                  apenasVisualizar,
                  ...(openStep != null && { openStep }),
                },
              });
            }}
          >
            <ActionIcon className="h-4 w-4" />
            {actionLabel}
          </Button>
        );
      }
      default:
        return null;
    }
  };
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [expandedFeedback, setExpandedFeedback] = useState<string | null>('1');
  const [expandedRegistro, setExpandedRegistro] = useState<string | null>('1');
  const [dialogPautaOpen, setDialogPautaOpen] = useState(false);
  const [pautaTexto, setPautaTexto] = useState('');
  const [salvandoPauta, setSalvandoPauta] = useState(false);
  const [dialogNovoFeedbackOpen, setDialogNovoFeedbackOpen] = useState(false);
  const [dialogNovoRegistro1on1Open, setDialogNovoRegistro1on1Open] = useState(false);

  if (loadingDetalhes) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-8 text-muted-foreground">Carregando...</div>
      </div>
    );
  }

  if (!detalhes) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-8 text-destructive">Colaborador não encontrado</div>
      </div>
    );
  }

  const iniciais = detalhes.nome
    .split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2);

  const statusConfig = {
    ativo: { label: 'Ativo', className: 'bg-green-100 text-green-800' },
    ferias: { label: 'Férias', className: 'bg-yellow-100 text-yellow-800' },
    afastado: { label: 'Afastado', className: 'bg-gray-100 text-gray-800' },
  };
  const statusBadge = statusConfig[detalhes.status] || statusConfig.ativo;

  const handlePerfil360 = () => {
    const rotaOrigem = (location.state as { from?: string })?.from || '/gestao-desempenho-gestor';
    navigate(`/profile360?colaborador=${detalhes.codColaborador}`, {
      state: { from: rotaOrigem }
    });
  };

  return (
    <div className="container mx-auto p-4 space-y-6">
      {/* Botão Voltar */}
      <Button 
        variant="ghost" 
        onClick={() => navigate(rotaOrigem)}
        className="mb-2"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Voltar
      </Button>

      {/* Card Principal do Perfil */}
      <Card>
        <CardContent className="p-6">
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-2">
              <div className="h-6 w-1 bg-gray-700 rounded"></div>
              <h1 className="text-2xl font-bold">Perfil do Colaborador</h1>
            </div>
            <Popover open={dropdownOpen} onOpenChange={setDropdownOpen}>
              <PopoverTrigger asChild>
                <Button variant="primary" size="sm">
                  <Plus className="h-4 w-4 mr-2" />
                  Adicionar
                  <ChevronDown className="h-4 w-4 ml-2" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-56 p-2" align="end">
                <div className="space-y-1">
                  <Button
                    variant="ghost"
                    className="w-full justify-start"
                    onClick={() => {
                      setDropdownOpen(false);
                      setDialogNovoFeedbackOpen(true);
                    }}
                  >
                    <MessageCircle className="h-4 w-4 mr-2" />
                    Feedback
                  </Button>
                  <Button
                    variant="ghost"
                    className="w-full justify-start"
                    onClick={() => {
                      setDropdownOpen(false);
                      setDialogNovoRegistro1on1Open(true);
                    }}
                  >
                    <Users className="h-4 w-4 mr-2" />
                    Registro 1:1
                  </Button>
                  {!isRhContext && (
                    <Button
                      variant="ghost"
                      className="w-full justify-start"
                      onClick={() => {
                        setDropdownOpen(false);
                        navigate(`${basePathPerfil}/novo-pdi`, {
                          state: { colaboradorNome: detalhes?.nome, from: rotaOrigem },
                        });
                      }}
                    >
                      <Target className="h-4 w-4 mr-2" />
                      Meta PDI
                    </Button>
                  )}
                </div>
              </PopoverContent>
            </Popover>
          </div>

          {/* Primeira linha: duas colunas — Perfil (foto) | Nome e badges */}
          <div className="flex items-start gap-6">
            <div className="flex-shrink-0">
              <Avatar className="h-24 w-24 rounded-lg">
                <AvatarImage src="" alt={detalhes.nome} />
                <AvatarFallback className="text-2xl rounded-lg bg-green-100 text-green-700">
                  {iniciais}
                </AvatarFallback>
              </Avatar>
            </div>
            <div className="flex-1 min-w-0">
              <h2 className="text-2xl font-bold mb-1">{detalhes.nome}</h2>
              <p className="text-muted-foreground mb-4">{detalhes.cargo}</p>
              <div className="flex gap-2 flex-wrap items-center">
                <Button
                  variant="secondary"
                  size="sm"
                  className="h-6 rounded-md px-2.5 font-normal text-sm cursor-pointer hover:bg-secondary/80"
                  onClick={handlePerfil360}
                >
                  <User className="h-3.5 w-3.5 mr-1.5" />
                  Perfil 360
                </Button>
                <Badge variant="secondary" className="font-normal">
                  #{detalhes.codigoColaboradorExterno?.trim() || detalhes.codColaborador}
                </Badge>
                {detalhes.modalidadeContratacao?.trim() && (
                  <Badge variant="secondary" className="font-normal">
                    {detalhes.modalidadeContratacao}
                  </Badge>
                )}
                <Badge className={`${statusBadge.className} border-0`}>
                  {statusBadge.label}
                </Badge>
                {detalhes.regimeTrabalho?.trim() &&
                  detalhes.regimeTrabalho.trim().toLowerCase() !== detalhes.modalidadeContratacao?.trim().toLowerCase() && (
                    <Badge variant="secondary" className="font-normal">
                      {detalhes.regimeTrabalho}
                    </Badge>
                  )}
                {detalhes.modelo?.trim() && (
                  <Badge variant="secondary" className="font-normal">
                    {detalhes.modelo}
                  </Badge>
                )}
              </div>
            </div>
          </div>

          {/* Segunda linha: informações (Cargo, E-mail, Telefone, etc.) alinhadas à esquerda com a foto */}
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mt-6 w-full">
            <div className="flex items-center gap-2">
              <Briefcase className="h-5 w-5 text-muted-foreground flex-shrink-0" />
              <div>
                <p className="text-sm text-muted-foreground">Cargo</p>
                <p className="font-medium text-sm">{detalhes.cargo}</p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Mail className="h-5 w-5 text-muted-foreground flex-shrink-0" />
              <div>
                <p className="text-sm text-muted-foreground">E-mail</p>
                <p className="font-medium text-sm">{detalhes.email}</p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Phone className="h-5 w-5 text-muted-foreground flex-shrink-0" />
              <div>
                <p className="text-sm text-muted-foreground">Telefone</p>
                <p className="font-medium text-sm">{formatTelefone(detalhes.telefone)}</p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Calendar className="h-5 w-5 text-muted-foreground flex-shrink-0" />
              <div>
                <p className="text-sm text-muted-foreground">Data de Nascimento</p>
                <p className="font-medium text-sm">{formatData(detalhes.dataNascimento)}</p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Calendar className="h-5 w-5 text-muted-foreground flex-shrink-0" />
              <div>
                <p className="text-sm text-muted-foreground">Data de Admissão</p>
                <p className="font-medium text-sm">{formatData(detalhes.dataAdmissao)}</p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Clock className="h-5 w-5 text-muted-foreground flex-shrink-0" />
              <div>
                <p className="text-sm text-muted-foreground">Tempo de Casa</p>
                <p className="font-medium text-sm">{detalhes.tempoCasa}</p>
              </div>
            </div>
            {orgId !== 0 && (
              <>
                <div className="flex items-center gap-2">
                  <DollarSign className="h-5 w-5 text-muted-foreground flex-shrink-0" />
                  <div>
                    <p className="text-sm text-muted-foreground">Salário</p>
                    <p className="font-medium text-sm">{formatSalario(detalhes.salario)}</p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <Clock className="h-5 w-5 text-muted-foreground flex-shrink-0" />
                  <div>
                    <p className="text-sm text-muted-foreground">Saldo de Horas</p>
                    <p className={`font-medium text-sm ${detalhes.saldoHoras >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                      {formatSaldoHoras(detalhes.saldoHoras)}
                    </p>
                  </div>
                </div>
              </>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Tabs */}
      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList>
          <TabsTrigger value="feedbacks">
            <MessageCircle className="h-4 w-4 mr-2" />
            Feedbacks {feedbacks.length > 0 && `(${feedbacks.length})`}
          </TabsTrigger>
          <TabsTrigger value="registros-1-1">
            <Users className="h-4 w-4 mr-2" />
            Registros 1:1 {registros.length > 0 && `(${registros.length})`}
          </TabsTrigger>
          <TabsTrigger value="pdi">
            <Target className="h-4 w-4 mr-2" />
            PDI {pdisResumoTime.length > 0 && `(${pdisResumoTime.length})`}
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
              const temObservacoes = !!feedback.observacoesGerais;
              
              return (
                <Card key={feedback.id}>
                  <CardContent className="p-6">
                    <div className="flex items-start justify-between mb-4">
                      <div className="flex items-center gap-2">
                        <Calendar className="h-5 w-5 text-green-600" />
                        <h3 className="text-lg font-semibold">
                          Feedback - {formatData(feedback.data)}
                        </h3>
                      </div>
                      {feedback.visto && feedback.vistoEm && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <Eye className="h-4 w-4" />
                          <span>Visto em {format(new Date(feedback.vistoEm), "dd/MM/yyyy, HH:mm", { locale: ptBR })}</span>
                          {(temAcoes || temObservacoes) && (
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-6 w-6"
                              onClick={() => setExpandedFeedback(isExpanded ? null : feedback.id)}
                            >
                              {isExpanded ? <ChevronUp className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
                            </Button>
                          )}
                        </div>
                      )}
                    </div>
                    <p className="text-muted-foreground mb-4">{feedback.resumo}</p>
                    <div className="flex items-center gap-2 mb-4">
                      <User className="h-4 w-4 text-muted-foreground" />
                      <span className="text-sm text-muted-foreground">
                        Realizado por: {feedback.realizadoPor}
                      </span>
                    </div>

                    {/* Cards de Ação - 3 colunas na primeira linha */}
                    {((feedback.continuar && feedback.continuar.length > 0) || 
                      (feedback.comecar && feedback.comecar.length > 0) || 
                      (feedback.parar && feedback.parar.length > 0)) && (
                      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
                        {feedback.continuar && feedback.continuar.length > 0 && (
                          <div className="bg-green-50 rounded-lg p-4">
                            <div className="flex items-center gap-2 mb-2">
                              <Play className="h-5 w-5 text-green-600" />
                              <h4 className="font-semibold text-green-800">Continuar</h4>
                            </div>
                            {feedback.continuar.map((item, idx) => (
                              <p key={idx} className="text-sm text-green-900">{item}</p>
                            ))}
                          </div>
                        )}
                        {feedback.comecar && feedback.comecar.length > 0 && (
                          <div className="bg-blue-50 rounded-lg p-4">
                            <div className="flex items-center gap-2 mb-2">
                              <ChevronRight className="h-5 w-5 text-blue-600" />
                              <h4 className="font-semibold text-blue-800">Começar</h4>
                            </div>
                            {feedback.comecar.map((item, idx) => (
                              <p key={idx} className="text-sm text-blue-900">{item}</p>
                            ))}
                          </div>
                        )}
                        {feedback.parar && feedback.parar.length > 0 && (
                          <div className="bg-red-50 rounded-lg p-4">
                            <div className="flex items-center gap-2 mb-2">
                              <Stop className="h-5 w-5 text-red-600" />
                              <h4 className="font-semibold text-red-800">Parar</h4>
                            </div>
                            {feedback.parar.map((item, idx) => (
                              <p key={idx} className="text-sm text-red-900">{item}</p>
                            ))}
                          </div>
                        )}
                      </div>
                    )}

                    {/* Observações gerais - uma coluna na segunda linha */}
                    {feedback.observacoesGerais && (
                      <div className="bg-gray-50 rounded-lg p-4">
                        <h4 className="font-semibold mb-2">Observações gerais</h4>
                        <p className="text-sm text-muted-foreground">{feedback.observacoesGerais}</p>
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
                  <FileText className="h-5 w-5 text-muted-foreground" />
                  <h3 className="text-lg font-semibold">
                    Pautas Sugeridas {pautas.length > 0 && `(${pautas.length})`}
                  </h3>
                </div>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => {
                    if (pautas.length > 0) {
                      setPautaTexto(pautas[pautas.length - 1].descricao);
                    } else {
                      setPautaTexto('');
                    }
                    setDialogPautaOpen(true);
                  }}
                >
                  <Plus className="h-4 w-4 mr-2" />
                  Adicionar Pauta
                </Button>
              </div>
              <ul className="list-disc list-inside space-y-2 text-sm text-muted-foreground">
                {pautas.map((pauta) => {
                  const mostrarTag = pauta.origem === "COLABORADOR" || pauta.origem === "SUBORDINADO";
                  return (
                    <li key={pauta.id} className="flex items-start gap-2">
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
            </CardContent>
          </Card>

          {/* Lista de Registros */}
          <div className="space-y-4">
            {registros.map((registro) => {
              const isExpanded = expandedRegistro === registro.id;
              const temDetalhes = (registro.pautaSugerida && registro.pautaSugerida.length > 0) || !!registro.anotacoes;
              
              return (
                <Card key={registro.id}>
                  <CardContent className="p-6">
                    <div className="flex items-start justify-between mb-4">
                      <div className="flex items-center gap-2 flex-1">
                        {registro.critico ? (
                          <AlertTriangle className="h-5 w-5 text-orange-600" />
                        ) : (
                          <Calendar className="h-5 w-5 text-green-600" />
                        )}
                        <h3 className="text-lg font-semibold">
                          1:1 - {formatData(registro.data)}
                        </h3>
                        {registro.critico && (
                          <Badge className="bg-red-100 text-red-800 border-0">
                            Crítico
                          </Badge>
                        )}
                      </div>
                      {registro.visto && registro.vistoEm && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <Eye className="h-4 w-4" />
                          <span>Visto em {format(new Date(registro.vistoEm), "dd/MM/yyyy, HH:mm", { locale: ptBR })}</span>
                          {temDetalhes && (
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-6 w-6"
                              onClick={() => setExpandedRegistro(isExpanded ? null : registro.id)}
                            >
                              {isExpanded ? <ChevronUp className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
                            </Button>
                          )}
                        </div>
                      )}
                    </div>
                    <p className="text-muted-foreground mb-4">{registro.resumo}</p>

                    {/* Detalhes expandidos */}
                    {isExpanded && temDetalhes && (
                      <div className="space-y-4 mt-4 pt-4 border-t">
                        {/* Realizado por - só aparece quando expandido */}
                        <div className="flex items-center gap-2 mb-4">
                          <User className="h-4 w-4 text-muted-foreground" />
                          <span className="text-sm text-muted-foreground">
                            Realizado por: <span className="text-blue-600 font-medium">{registro.realizadoPor}</span>
                          </span>
                        </div>

                        {/* Pauta Sugerida */}
                        {registro.pautaSugerida && registro.pautaSugerida.length > 0 && (
                          <div>
                            <div className="flex items-center gap-2 mb-2">
                              <FileText className="h-5 w-5 text-muted-foreground" />
                              <h4 className="font-semibold">Pauta Sugerida</h4>
                            </div>
                            <ul className="list-disc list-inside space-y-1 text-sm text-muted-foreground ml-7">
                              {registro.pautaSugerida.map((pauta, idx) => (
                                <li key={idx}>{pauta}</li>
                              ))}
                            </ul>
                          </div>
                        )}

                        {/* Anotações da Reunião */}
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

        {/* Tab PDI — 5 cards de status + tabelas Ativos e Histórico (mesmo formato da tela colaborador) */}
        <TabsContent value="pdi" className="space-y-6">
          {/* Cards big numbers: Não iniciados, Em análise, Em andamento, Finalizados, Cancelados */}
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

          {/* Tabela Ativos */}
          <PdiTableSection<PdiResumoTimeDTO>
            title="Ativos"
            subtitle="Acompanhe o status dos PDI's deste colaborador"
            topButton={!isRhContext ? (
              <Button onClick={() => navigate(`${basePathPerfil}/novo-pdi`, { state: { colaboradorNome: detalhes?.nome, from: rotaOrigem } })}>
                <Target className="h-4 w-4 mr-2" />
                Criar PDI
              </Button>
            ) : undefined}
            columns={[
              { id: 'status', label: 'Status' },
              { id: 'titulo', label: 'Título' },
              { id: 'fourTalent', label: 'Four Talent' },
              { id: 'previsaoConclusao', label: 'Previsão' },
              { id: 'andamento', label: 'Andamento' },
              { id: 'criadoPor', label: 'Criado por' },
              { id: 'acoes', label: 'Ações', align: 'right' },
            ]}
            data={pdisAtivos}
            keyExtractor={(row) => row.pdiId}
            loading={loadingPdis}
            emptyMessage="Nenhum PDI ativo para este colaborador."
            renderCell={(row, columnId) => renderPdiCell(row, columnId)}
          />

          {/* Tabela Histórico */}
          <PdiTableSection<PdiResumoTimeDTO>
            title="Histórico"
            subtitle="Acompanhe a evolução de cada meta conquistada"
            columns={[
              { id: 'status', label: 'Status' },
              { id: 'titulo', label: 'Título' },
              { id: 'fourTalent', label: 'Four Talent' },
              { id: 'previsaoConclusao', label: 'Previsão' },
              { id: 'andamento', label: 'Andamento' },
              { id: 'criadoPor', label: 'Criado por' },
              { id: 'acoes', label: 'Ações', align: 'right' },
            ]}
            data={pdisHistorico}
            keyExtractor={(row) => row.pdiId}
            loading={loadingPdis}
            emptyMessage="Nenhum PDI no histórico."
            renderCell={(row, columnId) => renderPdiCell(row, columnId)}
          />
        </TabsContent>
      </Tabs>

      {/* Dialog Adicionar Pauta */}
      <Dialog open={dialogPautaOpen} onOpenChange={setDialogPautaOpen}>
        <DialogContent className="sm:max-w-[680px] !max-h-[90vh] flex flex-col overflow-hidden p-6 gap-4">
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
              />
            </div>
          </div>
          <div className="flex justify-end gap-2 flex-shrink-0 border-t border-border pt-4">
              <Button
                variant="outline"
                onClick={() => {
                  setDialogPautaOpen(false);
                  setPautaTexto('');
                }}
              >
                Cancelar
              </Button>
              <Button
                onClick={async () => {
                  const texto = htmlToPlainText(pautaTexto);
                  if (!texto || !codColaborador || !token) {
                    return;
                  }

                  setSalvandoPauta(true);
                  try {
                    const useCase = container.resolve(InserirPautaSugeridaUseCase);
                    const payload = {
                      codigoInternoColaboradorAvaliado: codColaborador,
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

      {/* Dialog Novo Feedback */}
      <DialogNovoFeedback
        open={dialogNovoFeedbackOpen}
        onOpenChange={setDialogNovoFeedbackOpen}
        colaboradorNome={detalhes.nome}
        colaboradorCargo={detalhes.cargo}
        colaboradorCod={detalhes.codColaborador}
        onSuccess={async () => {
          // Refetch dos dados do colaborador
          await refetchFeedbacks();
        }}
      />

      {/* Dialog Novo Registro 1:1 */}
      <DialogNovoRegistro1on1
        open={dialogNovoRegistro1on1Open}
        onOpenChange={setDialogNovoRegistro1on1Open}
        colaboradorNome={detalhes.nome}
        colaboradorCargo={detalhes.cargo}
        colaboradorCod={detalhes.codColaborador}
        pautas={pautas}
        onSuccess={async () => {
          // Refetch dos dados do colaborador
          await refetchRegistros();
        }}
      />
    </div>
  );
}

