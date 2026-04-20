import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { CardMeusColaboradores } from '@/presentation/components/gestaoDesempenho/CardMeusColaboradores';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { StatCard } from '@presentation/components/common/StatCard';
import {
  Users,
  AlertTriangle,
  MessageCircle,
  Target,
  TrendingUp,
  Clock,
  Eye,
  Settings,
  FileText,
  AlertCircle,
  Play,
  CheckCircle,
  XCircle
} from '@/components/ui/system-icons';
import type { Column } from '@/hooks/useColumnReorder';
import type { ColaboradorDesempenho, FiltroDesempenho } from '@shared/types/gestaoDesempenho';
import { PdiStatusLabel } from '@shared/types/pdiApi';
import {
  useEstatisticasRH,
  useColaboradoresRH,
  useMetricasPdiRH
} from '@/presentation/hooks/useGestaoDesempenho';
import { formatData } from '@shared/utils/bookColaboradorFormatters';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

export default function GestaoDesempenhoRH() {
  const navigate = useNavigate();
  const { estatisticas, loading: loadingEstatisticas, accessDenied: accessDeniedEstatisticas } = useEstatisticasRH();
  const { metricas: metricasPdi, loading: loadingMetricasPdi } = useMetricasPdiRH();
  const [filtro, setFiltro] = useState<FiltroDesempenho>('todos');
  const [busca, setBusca] = useState('');
  const [gestorFiltro, setGestorFiltro] = useState<string>('todos');
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(25);
  const { colaboradores, loading: loadingColaboradores, contadoresFiltros, accessDenied: accessDeniedColaboradores } = useColaboradoresRH(filtro, busca);
  const accessDenied = accessDeniedEstatisticas || accessDeniedColaboradores;

  // Filtrar por gestor
  const colaboradoresFiltrados = gestorFiltro === 'todos'
    ? colaboradores
    : colaboradores.filter((c) => c.gestor === gestorFiltro);

  // Paginar colaboradores
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const colaboradoresPaginados = colaboradoresFiltrados.slice(startIndex, endIndex);

  // Obter lista única de gestores
  const gestores = Array.from(new Set(colaboradores.map((c) => c.gestor).filter(Boolean)));

  // Resetar página quando filtros mudarem
  useEffect(() => {
    setCurrentPage(1);
  }, [filtro, busca, gestorFiltro]);

  if (loadingEstatisticas || loadingColaboradores || loadingMetricasPdi) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-8 text-muted-foreground">Carregando...</div>
      </div>
    );
  }

  if (accessDenied) {
    return (
      <div className="container mx-auto p-4">
        <Card className="max-w-lg mx-auto mt-8">
          <CardContent className="pt-6">
            <p className="text-center text-muted-foreground mb-4">
              Você não tem permissão para acessar a Gestão de Desempenho RH. Sua sessão foi mantida.
            </p>
            <div className="flex justify-center gap-2">
              <Button variant="outline" onClick={() => navigate('/dashboard')}>
                Ir para o Dashboard
              </Button>
              <Button variant="primary" onClick={() => navigate('/gestao-desempenho-colaborador')}>
                Gestão Desempenho (Colaborador)
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  const columns: Column[] = [
    { id: 'colaborador', label: 'Colaborador', sortable: true },
    { id: 'cargo', label: 'Cargo', sortable: true },
    { id: 'gestor', label: 'Gestor', sortable: true },
    { id: 'status', label: 'Status', sortable: true },
    { id: 'ultimoFeedback', label: 'Último Feedback', sortable: true },
    { id: 'ultimaUmAUm', label: 'Última 1:1', sortable: true },
    { id: 'pdisAtivos', label: 'PDIs Ativos', sortable: true },
  ];

  // RH somente leitura: sem botões de criar feedback/1:1, apenas exibição
  const renderCell = (colaborador: ColaboradorDesempenho, columnId: string) => {
    switch (columnId) {
      case 'colaborador':
        return (
          <div className="flex items-center gap-2">
            <Eye
              title="Perfil do Colaborador"
              className="h-4 w-4 text-muted-foreground cursor-pointer hover:text-foreground"
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/gestao-desempenho-rh/colaborador/${colaborador.codColaborador}`, {
                  state: { from: '/gestao-desempenho-rh' },
                });
              }}
            />
            <FileText
              title="Ver CV"
              className="h-4 w-4 text-muted-foreground cursor-pointer hover:text-foreground"
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/profile360?colaborador=${encodeURIComponent(colaborador.codColaborador)}`, {
                  state: { from: '/gestao-desempenho-rh' },
                });
              }}
            />
            <span className="font-medium">{colaborador.nome}</span>
          </div>
        );
      case 'cargo':
        return <span>{colaborador.cargo}</span>;
      case 'gestor':
        return <span>{colaborador.gestor || '--'}</span>;
      case 'status':
        const statusConfig = {
          ativo: { label: 'Ativo', className: 'bg-green-500 text-white' },
          ferias: { label: 'Férias', className: 'bg-orange-500 text-white' },
          afastado: { label: 'Afastado', className: 'bg-gray-500 text-white' },
        };
        const config = statusConfig[colaborador.status] || statusConfig.ativo;
        return (
          <Badge className={`${config.className} border-0`}>
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
          </div>
        );
      case 'pdisAtivos':
        return <span>{colaborador.pdisAtivos}</span>;
      default:
        return null;
    }
  };

  return (
    <div className="container mx-auto p-4 space-y-6">
      {/* Header — alinhado ao do Gestor */}
      <div className="flex items-start justify-between">
        <div>
          <div className="flex items-center gap-2 mb-2">
            <TrendingUp className="h-6 w-6 text-green-600" />
            <h1 className="text-2xl font-bold">Gestão de Desempenho - RH</h1>
          </div>
          <p className="text-muted-foreground">
            Governança, padronização e visão da empresa (somente leitura)
          </p>
        </div>
        <Button
          variant="outline"
          onClick={() => navigate('/gestao-desempenho-rh/parametrizacao')}
        >
          <Settings className="h-4 w-4 mr-2" />
          Ver parametrização
        </Button>
      </div>

      {/* Painel de Controle — mesmo padrão visual do Gestor, dados da API RH */}
      <Card>
        <CardContent className="p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold">Painel de Controle</h2>
            <p className="text-sm text-muted-foreground">
              Visão geral da empresa • Períodos: 1:1 14d • Feedback 30d
            </p>
          </div>
          <div className="grid grid-cols-3 gap-4">
            <StatCard
              title="Total de colaboradores"
              value={estatisticas?.totalColaboradores.toString() || '0'}
              icon={Users}
              color="text-gray-600"
              bgColor="bg-gray-100"
              vertical
            />
            <StatCard
              title="Gestores com 1:1 em dia"
              value={`${estatisticas?.gestoresCom1a1EmDia || 0}%`}
              icon={TrendingUp}
              color="text-green-600"
              bgColor="bg-green-100"
              vertical
            />
            <StatCard
              title="Feedback em dia"
              value={`${estatisticas?.feedbackEmDia || 0}%`}
              icon={MessageCircle}
              color="text-green-600"
              bgColor="bg-green-100"
              vertical
            />
            <StatCard
              title="Sem 1:1 há +14d"
              value={estatisticas?.sem1a1Mais14d.toString() || '0'}
              icon={Clock}
              color="text-orange-600"
              bgColor="bg-orange-100"
              vertical
            />
            <StatCard
              title="Sem feedback há +30d"
              value={estatisticas?.semFeedbackMais30d.toString() || '0'}
              icon={AlertTriangle}
              color="text-orange-600"
              bgColor="bg-orange-100"
              vertical
            />
            <StatCard
              title="Sem PDI ativo"
              value={contadoresFiltros.semPdi.toString()}
              icon={Target}
              color="text-orange-600"
              bgColor="bg-orange-100"
              vertical
            />
          </div>
        </CardContent>
      </Card>

      {/* Métricas PDI — API GET /api/GestaoPessoa/Pdi/Metricas/gestor */}
      <Card>
        <CardContent className="p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold">Métricas PDI</h2>
            <p className="text-sm text-muted-foreground">
              Resultados por status (visão empresa)
            </p>
          </div>
          <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4">
            <StatCard
              title={PdiStatusLabel.NAO_INICIADO}
              value={String(metricasPdi?.bigNumbers?.naoIniciado ?? 0)}
              icon={Clock}
              color="text-amber-700"
              bgColor="bg-amber-100"
              vertical
            />
            <StatCard
              title={PdiStatusLabel.EM_ANALISE}
              value={String(metricasPdi?.bigNumbers?.emAnalise ?? 0)}
              icon={AlertCircle}
              color="text-blue-700"
              bgColor="bg-blue-100"
              vertical
            />
            <StatCard
              title={PdiStatusLabel.IN_PROGRESS}
              value={String(metricasPdi?.bigNumbers?.emAndamento ?? 0)}
              icon={Play}
              color="text-primary"
              bgColor="bg-primary/10"
              vertical
            />
            <StatCard
              title={PdiStatusLabel.FINALIZADO}
              value={String(metricasPdi?.bigNumbers?.finalizados ?? 0)}
              icon={CheckCircle}
              color="text-green-700"
              bgColor="bg-green-100"
              vertical
            />
            <StatCard
              title={PdiStatusLabel.CANCELADO}
              value={String(metricasPdi?.bigNumbers?.cancelados ?? 0)}
              icon={XCircle}
              color="text-slate-600"
              bgColor="bg-slate-100"
              vertical
            />
          </div>
        </CardContent>
      </Card>

      {/* Colaboradores — mesmo componente do Gestor, somente leitura (sem ações de criar) */}
      <CardMeusColaboradores
        title="Colaboradores"
        subtitle="Visão da empresa para acompanhamento de desempenho"
        busca={busca}
        onBuscaChange={setBusca}
        filtro={filtro}
        onFiltroChange={setFiltro}
        contadoresFiltros={contadoresFiltros}
        columns={columns}
        data={colaboradoresPaginados}
        renderCell={renderCell}
        keyExtractor={(item) => item.id}
        emptyMessage="Nenhum colaborador encontrado"
        onRowClick={(colaborador) =>
          navigate(`/gestao-desempenho-rh/colaborador/${colaborador.codColaborador}`, {
            state: { from: '/gestao-desempenho-rh' },
          })
        }
        getCellClassName={(columnId) => {
          if (columnId === 'ultimoFeedback') return 'bg-blue-50/50';
          if (columnId === 'ultimaUmAUm') return 'bg-purple-50/50';
          return '';
        }}
        wrapWithTooltipProvider
        toolbarExtra={
          <>
            <span className="text-sm font-medium">Gestor:</span>
            <Select value={gestorFiltro} onValueChange={(value) => setGestorFiltro(value || 'todos')}>
              <SelectTrigger className="w-[200px]">
                <SelectValue placeholder="Todos os gestores" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="todos">Todos os gestores</SelectItem>
                {gestores.map((gestor) => (
                  <SelectItem key={gestor || ''} value={gestor || ''}>
                    {gestor}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </>
        }
        pagination={
          colaboradoresFiltrados.length > 0
            ? {
                currentPage,
                totalItems: colaboradoresFiltrados.length,
                itemsPerPage,
                onPageChange: (page) => {
                  setCurrentPage(page);
                  window.scrollTo({ top: 0, behavior: 'smooth' });
                },
                onItemsPerPageChange: (items) => {
                  setItemsPerPage(Number(items));
                  setCurrentPage(1);
                },
              }
            : undefined
        }
      />
    </div>
  );
}
