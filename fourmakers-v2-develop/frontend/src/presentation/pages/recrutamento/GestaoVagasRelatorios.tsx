import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { container } from '@core/di/container';
import { GerarRelatorioProdutividadeUseCase } from '@domain/usecases/GerarRelatorioProdutividadeUseCase';
import { GerarRelatorioVagasUseCase } from '@domain/usecases/GerarRelatorioVagasUseCase';
import { GerarRelatorioVagasCandidaturasUseCase } from '@domain/usecases/GerarRelatorioVagasCandidaturasUseCase';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import { ArrowLeft, ChevronDown, Settings, FileText, Users } from '@/components/ui/system-icons';
import { downloadBlob } from '@shared/utils/downloadUtils';
import { logUserAction } from '@shared/utils/firebaseAnalytics';
import { toast } from 'sonner';

const MSG_ERRO_GERAR = 'Tivemos um erro ao gerar o relatório. Por favor, revise as datas ou tente novamente em alguns minutos.';
const MSG_SEM_DADOS = 'Sem dados para extrair o relatório';
const MSG_SUCESSO = 'Download realizado com sucesso';

function formatDateForInput(d: Date): string {
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${y}-${m}-${day}`;
}

/** Período de = hoje - 30 dias corridos. */
function getDefaultDataInicio(): string {
  const d = new Date();
  d.setDate(d.getDate() - 30);
  return formatDateForInput(d);
}

function getDefaultDataFim(): string {
  return formatDateForInput(new Date());
}

/** Formato dd-MM-yyyy_HH-mm para uso no nome do arquivo (ex.: 19-02-2026_15-56). */
function formatDateTimeForFilename(d: Date): string {
  const day = String(d.getDate()).padStart(2, '0');
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const year = d.getFullYear();
  const hours = String(d.getHours()).padStart(2, '0');
  const minutes = String(d.getMinutes()).padStart(2, '0');
  return `${day}-${month}-${year}_${hours}-${minutes}`;
}

type TipoRelatorio = 'produtividade' | 'vagas' | 'candidaturas';

const RELATORIOS_CONFIG: Record<
  TipoRelatorio,
  { titulo: string; descricao: string; nomeArquivo: string; icon: React.ReactNode }
> = {
  produtividade: {
    titulo: 'Produtividade',
    descricao: 'Relatório extraído com o período padrão de 30 dias. Você pode ajustar as datas como preferir abaixo.',
    nomeArquivo: 'relatorio-produtividade',
    icon: <Settings className="h-5 w-5 text-foreground" />,
  },
  vagas: {
    titulo: 'Relatório de Vagas',
    descricao: 'Relatório de vagas com o período padrão de 30 dias. Ajuste as datas para o intervalo desejado.',
    nomeArquivo: 'relatorio-vagas',
    icon: <FileText className="h-5 w-5 text-foreground" />,
  },
  candidaturas: {
    titulo: 'Acompanhamento de Vagas Alocação',
    descricao: 'Relatório de candidaturas e alocação no período. Você pode alterar as datas para um período específico.',
    nomeArquivo: 'relatorio-vagas-candidaturas',
    icon: <Users className="h-5 w-5 text-foreground" />,
  },
};

function RelatorioCard({
  tipo,
  dataInicio,
  dataFim,
  onDataInicioChange,
  onDataFimChange,
  onGerar,
  loading,
}: {
  tipo: TipoRelatorio;
  dataInicio: string;
  dataFim: string;
  onDataInicioChange: (v: string) => void;
  onDataFimChange: (v: string) => void;
  onGerar: () => void;
  loading: boolean;
}) {
  const config = RELATORIOS_CONFIG[tipo];
  return (
    <Card className="rounded-lg border border-border bg-card shadow-sm">
      <CardContent className="p-5 space-y-4">
        <div className="flex items-start gap-3">
          <div className="rounded-lg bg-primary/10 p-2">{config.icon}</div>
          <div>
            <h3 className="font-semibold text-foreground">{config.titulo}</h3>
            <p className="text-sm text-muted-foreground mt-1">{config.descricao}</p>
          </div>
        </div>

        <Collapsible defaultOpen>
          <CollapsibleTrigger className="flex w-full items-center justify-between rounded-lg bg-primary/10 px-3 py-2 text-left text-sm font-medium text-primary hover:bg-primary/15 transition-colors">
            <span>Mais detalhes para extrair os dados</span>
            <ChevronDown className="h-4 w-4 opacity-70" />
          </CollapsibleTrigger>
          <CollapsibleContent>
            <div className="mt-3 grid grid-cols-1 sm:grid-cols-2 gap-4 rounded-lg bg-muted/30 p-4">
              <div className="space-y-2">
                <Label className="text-sm font-medium text-foreground">Período de</Label>
                <Input
                  type="date"
                  value={dataInicio}
                  onChange={(e) => onDataInicioChange(e.target.value)}
                  aria-label="Data início do período"
                />
              </div>
              <div className="space-y-2">
                <Label className="text-sm font-medium text-foreground">até</Label>
                <Input
                  type="date"
                  value={dataFim}
                  onChange={(e) => onDataFimChange(e.target.value)}
                  aria-label="Data fim do período"
                />
              </div>
            </div>
          </CollapsibleContent>
        </Collapsible>

        <div className="flex justify-center pt-2">
          <Button
            onClick={onGerar}
            disabled={loading}
            className="w-full sm:w-auto min-w-[120px]"
          >
            {loading ? (
              <>
                <Spinner className="mr-2" size={16} aria-hidden />
                Gerando...
              </>
            ) : (
              'Iniciar'
            )}
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}

export default function GestaoVagasRelatorios() {
  const navigate = useNavigate();
  const { token, user } = useAppSelector((s) => s.auth);

  const [dataInicioProd, setDataInicioProd] = useState(getDefaultDataInicio);
  const [dataFimProd, setDataFimProd] = useState(getDefaultDataFim);
  const [dataInicioVagas, setDataInicioVagas] = useState(getDefaultDataInicio);
  const [dataFimVagas, setDataFimVagas] = useState(getDefaultDataFim);
  const [dataInicioCand, setDataInicioCand] = useState(getDefaultDataInicio);
  const [dataFimCand, setDataFimCand] = useState(getDefaultDataFim);

  const [loadingProd, setLoadingProd] = useState(false);
  const [loadingVagas, setLoadingVagas] = useState(false);
  const [loadingCand, setLoadingCand] = useState(false);

  const useCaseProd = useMemo(() => container.resolve(GerarRelatorioProdutividadeUseCase), []);
  const useCaseVagas = useMemo(() => container.resolve(GerarRelatorioVagasUseCase), []);
  const useCaseCand = useMemo(() => container.resolve(GerarRelatorioVagasCandidaturasUseCase), []);

  const handleGerar = async (
    tipo: TipoRelatorio,
    dataInicio: string,
    dataFim: string,
    setLoading: (v: boolean) => void,
    nomeArquivo: string
  ) => {
    if (!token) {
      toast.error('Sessão inválida. Faça login novamente.');
      return;
    }
    setLoading(true);
    try {
      let res: import('@domain/repositories/VagaRepository').RelatorioVagaResult;
      if (tipo === 'produtividade') {
        res = await useCaseProd.execute(token, dataInicio, dataFim);
      } else if (tipo === 'vagas') {
        res = await useCaseVagas.execute(token, dataInicio, dataFim);
      } else {
        res = await useCaseCand.execute(token, dataInicio, dataFim);
      }
      if (res.status === 204) {
        toast.info(MSG_SEM_DADOS);
        return;
      }
      const filenameWithTimestamp = `${nomeArquivo}_${formatDateTimeForFilename(new Date())}`;
      downloadBlob(res.blob, filenameWithTimestamp);
      logUserAction(
        'GestaoVagas',
        `GerarRelatorio${tipo.charAt(0).toUpperCase() + tipo.slice(1)}`,
        { dataInicio, dataFim, nomeArquivo: filenameWithTimestamp },
        user
      );
      toast.success(MSG_SUCESSO);
    } catch {
      toast.error(MSG_ERRO_GERAR);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto p-4 space-y-6">
      <PageBreadcrumb
        items={[
          { label: 'Recrutamento', href: '/recrutamento' },
          { label: 'Relatórios' },
        ]}
      />

      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          aria-label="Voltar"
          onClick={() => navigate('/recrutamento')}
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <PageHeader
          title="Página de Relatórios de Vagas"
          description="Você pode gerar abaixo os relatórios que precisa."
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <RelatorioCard
          tipo="produtividade"
          dataInicio={dataInicioProd}
          dataFim={dataFimProd}
          onDataInicioChange={setDataInicioProd}
          onDataFimChange={setDataFimProd}
          onGerar={() =>
            handleGerar(
              'produtividade',
              dataInicioProd,
              dataFimProd,
              setLoadingProd,
              RELATORIOS_CONFIG.produtividade.nomeArquivo
            )
          }
          loading={loadingProd}
        />
        <RelatorioCard
          tipo="vagas"
          dataInicio={dataInicioVagas}
          dataFim={dataFimVagas}
          onDataInicioChange={setDataInicioVagas}
          onDataFimChange={setDataFimVagas}
          onGerar={() =>
            handleGerar(
              'vagas',
              dataInicioVagas,
              dataFimVagas,
              setLoadingVagas,
              RELATORIOS_CONFIG.vagas.nomeArquivo
            )
          }
          loading={loadingVagas}
        />
        <RelatorioCard
          tipo="candidaturas"
          dataInicio={dataInicioCand}
          dataFim={dataFimCand}
          onDataInicioChange={setDataInicioCand}
          onDataFimChange={setDataFimCand}
          onGerar={() =>
            handleGerar(
              'candidaturas',
              dataInicioCand,
              dataFimCand,
              setLoadingCand,
              RELATORIOS_CONFIG.candidaturas.nomeArquivo
            )
          }
          loading={loadingCand}
        />
      </div>
    </div>
  );
}
