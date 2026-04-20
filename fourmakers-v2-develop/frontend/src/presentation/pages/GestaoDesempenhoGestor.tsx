import { useState, useMemo } from 'react';
import { GuideTour, TourButton, useTour, gestaoDesempenhoGestorSteps } from '@shared/tour';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { StatCard } from '@presentation/components/common/StatCard';
import { CardMeusColaboradores } from '@/presentation/components/gestaoDesempenho/CardMeusColaboradores';
import { DialogNovoFeedback } from '@/presentation/components/gestaoDesempenho/DialogNovoFeedback';
import { DialogNovoRegistro1on1 } from '@/presentation/components/gestaoDesempenho/DialogNovoRegistro1on1';
import { 
  Users, 
  Calendar, 
  AlertTriangle, 
  MessageCircle, 
  Target,
  TrendingUp,
  ChevronRight,
  Eye,
  Plus,
  User as Person,
  FileText
} from '@/components/ui/system-icons';
import type { Column } from '@/hooks/useColumnReorder';
import type { ColaboradorDesempenho, FiltroDesempenho } from '@shared/types/gestaoDesempenho';
import { 
  useEstatisticasDesempenho, 
  useRegistrosCriticos, 
  useColaboradoresDesempenho 
} from '@/presentation/hooks/useGestaoDesempenho';
import { formatData } from '@shared/utils/bookColaboradorFormatters';

// Função para verificar se o aniversário é hoje
const isAniversarioHoje = (dataAniversario?: string): boolean => {
  if (!dataAniversario) return false;
  
  const hoje = new Date();
  const diaHoje = String(hoje.getDate()).padStart(2, '0');
  const mesHoje = String(hoje.getMonth() + 1).padStart(2, '0');
  const dataHoje = `${diaHoje}/${mesHoje}`;
  
  return dataAniversario === dataHoje;
};

export default function GestaoDesempenhoGestor() {
  const navigate = useNavigate();
  const { estatisticas, loading: loadingEstatisticas } = useEstatisticasDesempenho();
  const { registros: registrosCriticos, loading: loadingRegistros } = useRegistrosCriticos();
  const [filtro, setFiltro] = useState<FiltroDesempenho>('todos');
  const [busca, setBusca] = useState('');
  const { isRunning: tourRunning, startTour, handleJoyrideCallback } = useTour();
  const { colaboradores, loading: loadingColaboradores, contadoresFiltros, totalPdisAtivos, refetch: refetchColaboradores } = useColaboradoresDesempenho(filtro, busca);
  
  // Enriquecer registros críticos com código do colaborador baseado na lista de colaboradores
  const registros = useMemo(() => {
    return registrosCriticos.map((registro) => {
      // Buscar colaborador pelo nome
      const colaborador = colaboradores.find(
        (c) => c.nome.toLowerCase() === registro.colaboradorNome.toLowerCase()
      );
      return {
        ...registro,
        colaboradorCod: colaborador?.codColaborador || registro.colaboradorCod,
      };
    });
  }, [registrosCriticos, colaboradores]);
  
  // Estados para modais
  const [modalNovoFeedbackOpen, setModalNovoFeedbackOpen] = useState(false);
  const [modalNovoRegistro1on1Open, setModalNovoRegistro1on1Open] = useState(false);
  const [colaboradorSelecionado, setColaboradorSelecionado] = useState<ColaboradorDesempenho | null>(null);

  if (loadingEstatisticas || loadingRegistros || loadingColaboradores) {
    return (
      <div className="container mx-auto p-4" data-testid="gestao-desempenho-gestor-page-loading">
        <div className="text-center py-8 text-muted-foreground">Carregando...</div>
      </div>
    );
  }

  const columns: Column[] = [
    { id: 'colaborador', label: 'Colaborador', sortable: true },
    { id: 'cargo', label: 'Cargo', sortable: true },
    { id: 'status', label: 'Status', sortable: true },
    { id: 'ultimoFeedback', label: 'Último Feedback', sortable: true },
    { id: 'ultimaUmAUm', label: 'Última 1:1', sortable: true },
    { id: 'pdisAtivos', label: 'PDIs Ativos', sortable: true },
  ];

  const renderCell = (colaborador: ColaboradorDesempenho, columnId: string) => {
    switch (columnId) {
      case 'colaborador':
        return (
          <div className="flex items-center gap-2">
            <Eye
              title="Perfil do Colaborador"
              className="h-4 w-4 text-muted-foreground cursor-pointer hover:text-foreground"
              data-testid={`gestao-desempenho-gestor-colaborador-perfil-link-${colaborador.codColaborador}`}
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/gestao-desempenho-gestor/${colaborador.codColaborador}`, {
                  state: { from: '/gestao-desempenho-gestor' }
                });
              }}
            />
            <FileText
              title="Ver CV"
              className="h-4 w-4 text-muted-foreground cursor-pointer hover:text-foreground"
              data-testid={`gestao-desempenho-gestor-colaborador-cv-link-${colaborador.codColaborador}`}
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/profile360?colaborador=${encodeURIComponent(colaborador.codColaborador)}`, {
                  state: { from: '/gestao-desempenho-gestor' },
                });
              }}
            />
            <span className="font-medium">{colaborador.nome}</span>
            {isAniversarioHoje(colaborador.dataAniversario) && (
              <Tooltip>
                <TooltipTrigger asChild>
                  <span className="cursor-help text-lg">🎉</span>
                </TooltipTrigger>
                <TooltipContent>
                  <p>Aniversário: {colaborador.dataAniversario}</p>
                </TooltipContent>
              </Tooltip>
            )}
          </div>
        );
      case 'cargo':
        return <span>{colaborador.cargo}</span>;
      case 'status':
        const statusConfig = {
          ativo: { label: 'Ativo', className: 'bg-green-500 text-white' },
          ferias: { label: 'Férias', className: 'bg-orange-500 text-white' },
          afastado: { label: 'Afastado', className: 'bg-gray-500 text-white' },
        };
        const config = statusConfig[colaborador.status] || statusConfig.ativo;
        return (
          <Badge className={`${config.className} border-0`} data-testid={`gestao-desempenho-gestor-colaborador-status-badge-${colaborador.codColaborador}`}>
            {config.label}
          </Badge>
        );
      case 'ultimoFeedback':
        return (
          <div className="flex items-center gap-2">
            <MessageCircle className="h-4 w-4 text-muted-foreground" />
            {colaborador.ultimoFeedback ? (
              <span>{formatData(colaborador.ultimoFeedback)}</span>
            ) : (
              <span className="text-muted-foreground">--</span>
            )}
            <Plus 
              className="h-4 w-4 text-muted-foreground cursor-pointer hover:text-foreground"
              data-testid={`gestao-desempenho-gestor-novo-feedback-button-${colaborador.codColaborador}`}
              onClick={(e) => {
                e.stopPropagation();
                setColaboradorSelecionado(colaborador);
                setModalNovoFeedbackOpen(true);
              }}
            />
          </div>
        );
      case 'ultimaUmAUm':
        return (
          <div className="flex items-center gap-2">
            <Users className="h-4 w-4 text-muted-foreground" />
            {colaborador.ultimaUmAUm ? (
              <span>{formatData(colaborador.ultimaUmAUm)}</span>
            ) : (
              <span className="text-muted-foreground">--</span>
            )}
            <Plus 
              className="h-4 w-4 text-muted-foreground cursor-pointer hover:text-foreground"
              data-testid={`gestao-desempenho-gestor-novo-1-1-button-${colaborador.codColaborador}`}
              onClick={(e) => {
                e.stopPropagation();
                setColaboradorSelecionado(colaborador);
                setModalNovoRegistro1on1Open(true);
              }}
            />
          </div>
        );
      case 'pdisAtivos':
        return (
          <div className="flex items-center gap-2">
            <Eye
              title="Perfil do Colaborador"
              className="h-4 w-4 text-muted-foreground cursor-pointer hover:text-foreground"
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/gestao-desempenho-gestor/${colaborador.codColaborador}`, {
                  state: { from: '/gestao-desempenho-gestor' }
                });
              }}
            />
            <span>{colaborador.pdisAtivos}</span>
            <Plus 
              className="h-4 w-4 text-muted-foreground opacity-50 cursor-not-allowed" 
            />
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="container mx-auto p-4 space-y-6">
      <GuideTour
        steps={gestaoDesempenhoGestorSteps}
        run={tourRunning}
        onCallback={handleJoyrideCallback}
      />

      {/* Header */}
      <div className="flex items-start justify-between gap-4 flex-wrap">
        <div>
          <div className="flex items-center gap-2 mb-2">
            <TrendingUp className="h-6 w-6 text-green-600" />
            <h1 className="text-2xl font-bold">Gestão de Desempenho</h1>
          </div>
          <p className="text-muted-foreground">
            Acompanhe feedbacks, conversas e desenvolvimento individual
          </p>
        </div>
        <TourButton onStart={startTour} data-testid="gestao-desempenho-gestor-tour-button" />
      </div>

      {/* Painel de Controle e Registros Críticos */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        {/* Painel de Controle */}
        <Card className="lg:col-span-8">
          <CardContent className="p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold">Painel de Controle</h2>
              <p className="text-sm text-muted-foreground">Visão geral do time</p>
            </div>
            <div className="grid grid-cols-3 gap-4">
            <StatCard
              title="Total de Colaboradores"
              value={estatisticas?.totalColaboradores.toString() || '0'}
              icon={Users}
              color="text-gray-600"
              bgColor="bg-gray-100"
              vertical
            />
            <StatCard
              title="1:1 em dia"
              value={estatisticas?.umAUmEmDia.toString() || '0'}
              icon={Calendar}
              color="text-green-600"
              bgColor="bg-green-100"
              vertical
            />
            <StatCard
              title="1:1 atrasados"
              value={estatisticas?.umAUmAtrasados.toString() || '0'}
              icon={AlertTriangle}
              color="text-orange-600"
              bgColor="bg-orange-100"
              vertical
            />
            <StatCard
              title="Feedback em dia"
              value={estatisticas?.feedbackEmDia.toString() || '0'}
              icon={MessageCircle}
              color="text-green-600"
              bgColor="bg-green-100"
              vertical
            />
            <StatCard
              title="Feedback atrasados"
              value={estatisticas?.feedbackAtrasados.toString() || '0'}
              icon={AlertTriangle}
              color="text-orange-600"
              bgColor="bg-orange-100"
              vertical
            />
            <StatCard
              title="PDIs ativos"
              value={totalPdisAtivos.toString()}
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
            <div className="space-y-3 max-h-[280px] overflow-y-auto">
              {registros.length === 0 ? (
                <div className="bg-orange-50 rounded-lg p-4" data-testid="gestao-desempenho-gestor-page-registros-criticos-empty">
                  <p className="text-muted-foreground text-sm">Nenhum registro crítico</p>
                </div>
              ) : (
                registros.map((registro) => (
                  <div
                    key={registro.id}
                    className="bg-orange-50 rounded-lg p-4 cursor-pointer hover:bg-orange-100 transition-colors"
                    data-testid={`gestao-desempenho-gestor-page-registro-critico-${registro.id}`}
                    onClick={() => navigate(`/gestao-desempenho-gestor/${registro.colaboradorCod}`, {
                      state: { from: '/gestao-desempenho-gestor' }
                    })}
                  >
                    <div className="flex items-start justify-between gap-4">
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-2">
                          <Person className="h-4 w-4 text-muted-foreground flex-shrink-0" />
                          <span className="font-medium text-foreground">{registro.colaboradorNome}</span>
                        </div>
                        <div className="flex items-center gap-2 mb-2">
                          <Calendar className="h-4 w-4 text-muted-foreground flex-shrink-0" />
                          <span className="text-sm text-muted-foreground">
                            {formatData(registro.data)}
                          </span>
                        </div>
                        <p className="text-sm text-muted-foreground line-clamp-2 leading-relaxed">
                          {registro.descricao}
                        </p>
                      </div>
                      <ChevronRight className="h-5 w-5 text-muted-foreground flex-shrink-0 mt-1" />
                    </div>
                  </div>
                ))
              )}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Meus Colaboradores — componente compartilhado com gestao-desempenho-rh */}
      <CardMeusColaboradores
        data-testid="gestao-desempenho-gestor-card-meus-colaboradores"
        title="Meus Colaboradores"
        busca={busca}
        onBuscaChange={setBusca}
        filtro={filtro}
        onFiltroChange={setFiltro}
        contadoresFiltros={contadoresFiltros}
        columns={columns}
        data={colaboradores}
        renderCell={renderCell}
        keyExtractor={(item) => item.id}
        emptyMessage="Nenhum colaborador encontrado"
        getCellClassName={(columnId) => {
          if (columnId === 'ultimoFeedback') return 'bg-blue-50/50';
          if (columnId === 'ultimaUmAUm') return 'bg-purple-50/50';
          return '';
        }}
        wrapWithTooltipProvider
      />

      {/* Modais */}
      {colaboradorSelecionado && (
        <>
          <DialogNovoFeedback
            open={modalNovoFeedbackOpen}
            onOpenChange={setModalNovoFeedbackOpen}
            colaboradorNome={colaboradorSelecionado.nome}
            colaboradorCargo={colaboradorSelecionado.cargo}
            colaboradorCod={colaboradorSelecionado.codColaborador}
            onSuccess={async () => {
              // Refetch da lista de colaboradores para atualizar os dados
              await refetchColaboradores();
            }}
          />
          <DialogNovoRegistro1on1
            open={modalNovoRegistro1on1Open}
            onOpenChange={setModalNovoRegistro1on1Open}
            colaboradorNome={colaboradorSelecionado.nome}
            colaboradorCargo={colaboradorSelecionado.cargo}
            colaboradorCod={colaboradorSelecionado.codColaborador}
            pautas={[]}
            onSuccess={async () => {
              // Refetch da lista de colaboradores para atualizar os dados
              await refetchColaboradores();
            }}
          />
        </>
      )}
    </div>
  );
}

