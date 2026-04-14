import type { ReactNode } from 'react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { DataTable } from '@presentation/components/common';
import { TablePagination } from '@presentation/components/common';
import { TooltipProvider } from '@/components/ui/tooltip';
import { Search, SlidersHorizontal } from '@/components/ui/system-icons';
import type { Column } from '@/hooks/useColumnReorder';
import type { ColaboradorDesempenho, FiltroDesempenho } from '@shared/types/gestaoDesempenho';

export interface ContadoresFiltros {
  semPdi: number;
  semFeedback: number;
  semUmAUm: number;
}

export interface CardMeusColaboradoresPagination {
  currentPage: number;
  totalItems: number;
  itemsPerPage: number;
  onPageChange: (page: number) => void;
  onItemsPerPageChange?: (items: number) => void;
}

export interface CardMeusColaboradoresProps {
  /** Título do card (ex.: "Meus Colaboradores" ou "Colaboradores") */
  title?: string;
  /** Subtítulo (default: "Acompanhe as atividades de desempenho da sua equipe") */
  subtitle?: string;
  busca: string;
  onBuscaChange: (value: string) => void;
  filtro: FiltroDesempenho;
  onFiltroChange: (value: FiltroDesempenho) => void;
  contadoresFiltros: ContadoresFiltros;
  columns: Column[];
  data: ColaboradorDesempenho[];
  renderCell: (item: ColaboradorDesempenho, columnId: string) => ReactNode;
  keyExtractor: (item: ColaboradorDesempenho) => string;
  emptyMessage?: string;
  getCellClassName?: (columnId: string) => string;
  onRowClick?: (item: ColaboradorDesempenho) => void;
  /** Conteúdo extra na barra de filtros (ex.: Select de gestor na tela RH) */
  toolbarExtra?: ReactNode;
  /** Paginação (ex.: tela RH); quando não informado, não exibe paginação */
  pagination?: CardMeusColaboradoresPagination;
  /** Envolver tabela em TooltipProvider (gestor usa; RH pode não precisar) */
  wrapWithTooltipProvider?: boolean;
  /** data-testid para o card (automação QA) */
  'data-testid'?: string;
}

const SUBTITLE_DEFAULT = 'Acompanhe as atividades de desempenho da sua equipe';

export function CardMeusColaboradores({
  title = 'Meus Colaboradores',
  subtitle = SUBTITLE_DEFAULT,
  busca,
  onBuscaChange,
  filtro,
  onFiltroChange,
  contadoresFiltros,
  columns,
  data,
  renderCell,
  keyExtractor,
  emptyMessage = 'Nenhum colaborador encontrado',
  getCellClassName,
  onRowClick,
  toolbarExtra,
  pagination,
  wrapWithTooltipProvider = false,
  'data-testid': dataTestId,
}: CardMeusColaboradoresProps) {
  const table = (
    <DataTable
      columns={columns}
      data={data}
      keyExtractor={keyExtractor}
      renderCell={renderCell}
      emptyMessage={emptyMessage}
      getCellClassName={getCellClassName}
      onRowClick={onRowClick}
    />
  );

  return (
    <Card data-testid={dataTestId}>
      <CardContent className="p-6">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-4">
          <div>
            <h2 className="text-lg font-semibold mb-1">{title}</h2>
            <p className="text-sm text-muted-foreground">{subtitle}</p>
          </div>
          <div className="relative w-full md:w-auto max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Buscar..."
              value={busca}
              onChange={(e) => onBuscaChange(e.target.value)}
              className="pl-10 w-full md:w-[300px]"
              data-testid="card-meus-colaboradores-busca-input"
            />
          </div>
        </div>

        {/* Filtros */}
        <div className="flex flex-col md:flex-row md:items-center gap-4 mb-4">
          {toolbarExtra && (
            <div className="flex items-center gap-2">
              {toolbarExtra}
            </div>
          )}
          <div className="flex items-center gap-2 flex-wrap">
            <SlidersHorizontal className="h-4 w-4 text-muted-foreground shrink-0" />
            <span className="text-sm font-medium">Filtrar:</span>
            <Button
              variant={filtro === 'todos' ? 'primary' : 'outline'}
              size="sm"
              onClick={() => onFiltroChange('todos')}
              data-testid="card-meus-colaboradores-filtro-todos"
            >
              Todos
            </Button>
            <Button
              variant={filtro === 'sem-pdi' ? 'primary' : 'outline'}
              size="sm"
              onClick={() => onFiltroChange('sem-pdi')}
              data-testid="card-meus-colaboradores-filtro-sem-pdi"
            >
              Sem PDI ({contadoresFiltros.semPdi})
            </Button>
            <Button
              variant={filtro === 'sem-feedback' ? 'primary' : 'outline'}
              size="sm"
              onClick={() => onFiltroChange('sem-feedback')}
              data-testid="card-meus-colaboradores-filtro-sem-feedback"
            >
              Sem Feedback ({contadoresFiltros.semFeedback})
            </Button>
            <Button
              variant={filtro === 'sem-1-1' ? 'primary' : 'outline'}
              size="sm"
              onClick={() => onFiltroChange('sem-1-1')}
              data-testid="card-meus-colaboradores-filtro-sem-1-1"
            >
              Sem 1:1 ({contadoresFiltros.semUmAUm})
            </Button>
          </div>
        </div>

        {/* Tabela */}
        {wrapWithTooltipProvider ? (
          <TooltipProvider>{table}</TooltipProvider>
        ) : (
          table
        )}

        {/* Paginação */}
        {pagination && pagination.totalItems > 0 && (
          <TablePagination
            currentPage={pagination.currentPage}
            totalItems={pagination.totalItems}
            itemsPerPage={pagination.itemsPerPage}
            onPageChange={pagination.onPageChange}
            onItemsPerPageChange={(items) => pagination.onItemsPerPageChange?.(Number(items))}
          />
        )}
      </CardContent>
    </Card>
  );
}
