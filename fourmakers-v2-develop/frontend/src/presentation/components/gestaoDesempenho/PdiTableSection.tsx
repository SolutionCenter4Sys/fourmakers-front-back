import type { ReactNode } from 'react';
import { Card, CardContent } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';

export interface PdiTableColumn {
  id: string;
  label: string;
  align?: 'left' | 'right';
}

interface PdiTableSectionProps<T> {
  /** Título da seção (ex: "Ativos", "Histórico") */
  title?: string;
  /** Subtítulo (ex: "Acompanhe o status dos seus PDI's") */
  subtitle?: string;
  /** Botão opcional no canto superior direito (ex: "Criar PDI") */
  topButton?: ReactNode;
  /** Definição das colunas da tabela */
  columns: PdiTableColumn[];
  /** Dados para as linhas */
  data: T[];
  /** Chave única por linha */
  keyExtractor: (row: T) => string;
  /** Renderiza o conteúdo da célula por coluna */
  renderCell: (row: T, columnId: string) => ReactNode;
  /** Estado de carregamento: exibe skeleton */
  loading?: boolean;
  /** Mensagem quando não há dados */
  emptyMessage?: string;
  /** Conteúdo do empty state (ex: botão Criar PDI); se não passar, usa só emptyMessage */
  emptyContent?: ReactNode;
  /** data-testid para a seção (automação QA) */
  'data-testid'?: string;
}

/**
 * Seção de tabela PDI no padrão da gestão de desempenho (colaborador e gestor).
 * Mesmo formato: Card rounded-xl, cabeçalho bg-muted/60, linhas com hover.
 */
export function PdiTableSection<T>({
  title,
  subtitle,
  topButton,
  columns,
  data,
  keyExtractor,
  renderCell,
  loading = false,
  emptyMessage = 'Nenhum PDI encontrado.',
  emptyContent,
  'data-testid': dataTestId,
}: PdiTableSectionProps<T>) {
  return (
    <div data-testid={dataTestId}>
      {(title || topButton) && (
        <div
          className="flex items-center justify-between mb-4"
          data-testid={dataTestId ? `${dataTestId}-header` : undefined}
        >
          <div>
            {title && <h2 className="text-lg font-semibold">{title}</h2>}
            {subtitle && <p className="text-sm text-muted-foreground">{subtitle}</p>}
          </div>
          {topButton}
        </div>
      )}

      {loading ? (
        <Card className="rounded-xl border border-border bg-card shadow-sm">
          <CardContent className="p-0">
            <div className="border-b bg-muted/60 p-4 flex flex-wrap gap-4 items-center">
              {columns.map((col) => (
                <Skeleton key={col.id} className="h-4 w-24" />
              ))}
              {topButton && <Skeleton className="h-8 w-32 rounded-md ml-auto" />}
            </div>
            <div className="divide-y">
              {[1, 2, 3, 4].map((i) => (
                <div key={i} className="p-4 flex flex-wrap gap-4 items-center">
                  <Skeleton className="h-6 w-24 rounded-full shrink-0" />
                  <Skeleton className="h-4 w-48 max-w-full shrink-0" />
                  <Skeleton className="h-4 w-24 shrink-0" />
                  <div className="flex items-center gap-2 shrink-0">
                    <Skeleton className="h-2 w-20 rounded-full" />
                    <Skeleton className="h-4 w-8" />
                  </div>
                  <div className="ml-auto shrink-0">
                    <Skeleton className="h-8 w-36 rounded-md" />
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      ) : data.length === 0 ? (
        <Card className="rounded-xl border border-border bg-card shadow-sm">
          <CardContent className="py-12 text-center text-muted-foreground" data-testid={dataTestId ? `${dataTestId}-empty` : undefined}>
            {emptyContent ?? <p>{emptyMessage}</p>}
          </CardContent>
        </Card>
      ) : (
        <Card className="rounded-xl border border-border bg-card shadow-sm">
          <CardContent className="p-0">
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-muted/60">
                    {columns.map((col) => (
                      <th
                        key={col.id}
                        className={`p-4 font-semibold text-foreground ${col.align === 'right' ? 'text-right' : 'text-left'}`}
                      >
                        {col.label}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {data.map((row) => (
                    <tr
                      key={keyExtractor(row)}
                      className="border-b last:border-0 hover:bg-muted/30 transition-colors"
                    >
                      {columns.map((col) => (
                        <td
                          key={col.id}
                          className={`p-4 ${col.align === 'right' ? 'text-right' : 'text-left'}`}
                        >
                          {renderCell(row, col.id)}
                        </td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
