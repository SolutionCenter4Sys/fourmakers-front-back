import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { PageBreadcrumb, DataTable } from '@presentation/components/common';
import type { Column } from '@/hooks/useColumnReorder';
import { Spinner } from '@/components/ui/spinner';
import { ArrowLeft, Info } from '@/components/ui/system-icons';
import { useAppSelector } from '@app/store/hooks';
import { logUserAction } from '@shared/utils/firebaseAnalytics';
import { container } from '@core/di/container';
import type { LoteItemGestao } from '@domain/entities/GestaoVagasCandidatos';
import { BuscarMeusLotesUseCase } from '@domain/usecases/BuscarMeusLotesUseCase';
import { formatDatePtBr } from '@presentation/hooks/recrutamento'
import { toast } from 'sonner';
import { ModalDetalhesLote } from '@presentation/components/gestao-vagas';

const NAO_INFORMADO = 'Não informado';

function textoProcessamento(item: LoteItemGestao): string {
  const total = item.totalItens ?? 0;
  const processado = item.quantidadeProcessada ?? item.quantidadeAProcessar ?? 0;
  if (total === 0) return '0 de 0';
  return `${processado} de ${total}`;
}

function textoDataProcessamento(item: LoteItemGestao): string {
  const data = item.dataInicioProcessamento;
  if (!data) return 'Não iniciado';
  return `A processar em ${formatDatePtBr(data)}`;
}

const COLUNAS: Column[] = [
  { id: 'dataCriacao', label: 'Data de criação', sortable: true },
  { id: 'dataInicioProcessamento', label: 'Data início processamento', sortable: true },
  { id: 'totalItens', label: 'Total de CVs', sortable: true },
  { id: 'progresso', label: 'Progresso', sortable: false },
  { id: 'detalhes', label: 'Detalhes', sortable: false, width: 'w-[80px]', align: 'center' },
];

export default function MinhasImportacoes() {
  const navigate = useNavigate();
  const { token, user } = useAppSelector((s) => s.auth);

  const [loading, setLoading] = useState(false);
  const [lista, setLista] = useState<LoteItemGestao[]>([]);
  const [modalDetalhesOpen, setModalDetalhesOpen] = useState(false);
  const [idLoteSelecionado, setIdLoteSelecionado] = useState<string | null>(null);

  const buscarMeusLotesUseCase = container.resolve(BuscarMeusLotesUseCase);

  const carregar = useCallback(async () => {
    if (!token) {
      toast.error('Sessão inválida. Faça login novamente.');
      return;
    }
    setLoading(true);
    try {
      const dados = await buscarMeusLotesUseCase.execute(token, {
        busca: '',
        cursor: 0,
        limite: 5000,
      });
      setLista(dados);
    } catch {
      toast.error('Erro ao carregar minhas importações.');
      setLista([]);
    } finally {
      setLoading(false);
    }
  }, [buscarMeusLotesUseCase, token]);

  useEffect(() => {
    carregar();
  }, [carregar]);

  const abrirDetalhes = (idLote: string) => {
    logUserAction('MinhasImportacoes', 'VerDetalhesLote', { idLote }, user);
    setIdLoteSelecionado(idLote);
    setModalDetalhesOpen(true);
  };

  const renderCell = (item: LoteItemGestao, columnId: string) => {
    switch (columnId) {
      case 'dataCriacao':
        return item.dataCriacao ? formatDatePtBr(item.dataCriacao) : NAO_INFORMADO;
      case 'dataInicioProcessamento':
        return textoDataProcessamento(item);
      case 'totalItens':
        return item.totalItens ?? 0;
      case 'progresso':
        return textoProcessamento(item);
      case 'detalhes':
        return (
          <Tooltip>
            <TooltipTrigger asChild>
              <Button
                variant="ghost"
                size="icon"
                className="h-9 w-9 shrink-0"
                aria-label="Ver detalhes do lote"
                onClick={(e) => {
                  e.stopPropagation();
                  abrirDetalhes(item.id);
                }}
              >
                <Info className="h-5 w-5 text-primary" />
              </Button>
            </TooltipTrigger>
            <TooltipContent>Ver detalhes do lote</TooltipContent>
          </Tooltip>
        );
      default:
        return null;
    }
  };

  const emptyMessage = 'Nenhum lote encontrado.';
  const dataParaTabela = lista;

  return (
    <div className="container mx-auto p-4 space-y-4">
      <PageBreadcrumb
        items={[
          { label: 'Recrutamento', href: '/recrutamento' },
          { label: 'Minhas importações' },
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
        <h1 className="text-xl font-semibold">Minhas importações</h1>
      </div>

      <Card>
        <CardContent className="p-6 space-y-4">
          <div>
            <h2 className="text-lg font-medium">Histórico de importações em lote</h2>
            <p className="text-sm text-muted-foreground">
              Lista dos lotes de CVs que você importou. Clique no ícone de informações para ver os detalhes.
            </p>
          </div>

          <div className="rounded-md overflow-hidden min-h-[200px]">
            {loading ? (
              <div className="flex flex-col items-center justify-center gap-2 py-12 text-muted-foreground" role="status" aria-live="polite">
                <Spinner size={24} className="text-primary" />
                <span>Carregando...</span>
              </div>
            ) : (
              <DataTable<LoteItemGestao>
                columns={COLUNAS}
                data={dataParaTabela}
                keyExtractor={(item) => item.id}
                renderCell={renderCell}
                emptyMessage={emptyMessage}
                defaultSort={{ columnId: 'dataCriacao', direction: 'desc' }}
                centerEmptyMessage
              />
            )}
          </div>
        </CardContent>
      </Card>

      <ModalDetalhesLote
        open={modalDetalhesOpen}
        onOpenChange={setModalDetalhesOpen}
        token={token}
        idLote={idLoteSelecionado}
      />
    </div>
  );
}
