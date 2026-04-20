import { useState, useCallback, useEffect } from "react";
import type { ReactNode } from "react";

export interface Column {
  id: string;
  label: ReactNode;
  /** Texto completo para tooltip no thead (ex.: "Processado / Erro" quando label é "Proc. / Erro"). */
  title?: string;
  width?: string;
  sortable?: boolean;
  align?: "left" | "center" | "right";
}

export type SortDirection = "asc" | "desc" | null;

export interface SortState {
  columnId: string | null;
  direction: SortDirection;
}

export interface InitialSortState {
  columnId: string;
  direction: "asc" | "desc";
}

export function useColumnReorder(
  initialColumns: Column[],
  initialSortState?: InitialSortState | null
) {
  const [columns, setColumns] = useState<Column[]>(initialColumns);
  const [draggedColumn, setDraggedColumn] = useState<string | null>(null);
  const [sortState, setSortState] = useState<SortState>(() =>
    initialSortState?.columnId && initialSortState?.direction
      ? { columnId: initialSortState.columnId, direction: initialSortState.direction }
      : { columnId: null, direction: null }
  );

  // Sincronizar colunas quando initialColumns mudar
  useEffect(() => {
    setColumns(initialColumns);
    
    // Se a coluna ordenada não existir mais, limpar o estado de ordenação
    setSortState(prev => {
      if (prev.columnId) {
        const columnExists = initialColumns.some(col => col.id === prev.columnId);
        if (!columnExists) {
          return { columnId: null, direction: null };
        }
      }
      return prev;
    });
  }, [initialColumns]);

  const handleDragStart = useCallback((columnId: string) => {
    setDraggedColumn(columnId);
  }, []);

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
  }, []);

  const handleDrop = useCallback(
    (targetColumnId: string) => {
      if (!draggedColumn || draggedColumn === targetColumnId) {
        setDraggedColumn(null);
        return;
      }

      setColumns((prevColumns) => {
        const newColumns = [...prevColumns];
        const draggedIndex = newColumns.findIndex((col) => col.id === draggedColumn);
        const targetIndex = newColumns.findIndex((col) => col.id === targetColumnId);

        if (draggedIndex === -1 || targetIndex === -1) return prevColumns;

        const [removed] = newColumns.splice(draggedIndex, 1);
        newColumns.splice(targetIndex, 0, removed);

        return newColumns;
      });

      setDraggedColumn(null);
    },
    [draggedColumn]
  );

  const handleDragEnd = useCallback(() => {
    setDraggedColumn(null);
  }, []);

  const handleSort = useCallback((columnId: string) => {
    setSortState((prev) => {
      if (prev.columnId !== columnId) {
        return { columnId, direction: "asc" };
      }
      if (prev.direction === "asc") {
        return { columnId, direction: "desc" };
      }
      if (prev.direction === "desc") {
        return { columnId: null, direction: null };
      }
      return { columnId, direction: "asc" };
    });
  }, []);

  const resetColumns = useCallback(() => {
    setColumns(initialColumns);
    setSortState({ columnId: null, direction: null });
  }, [initialColumns]);

  const sortData = useCallback(
    <T = unknown>(data: T[]): T[] => {
      if (!sortState.columnId || !sortState.direction) return data;

      return [...data].sort((a, b) => {
        const aValue = (a as Record<string, unknown>)[sortState.columnId!];
        const bValue = (b as Record<string, unknown>)[sortState.columnId!];

        if (aValue === bValue) return 0;

        const comparison =
          typeof aValue === "string" && typeof bValue === "string"
            ? aValue.localeCompare(bValue)
            : (aValue as number) < (bValue as number)
            ? -1
            : 1;

        return sortState.direction === "asc" ? comparison : -comparison;
      });
    },
    [sortState]
  );

  return {
    columns,
    draggedColumn,
    sortState,
    handleDragStart,
    handleDragOver,
    handleDrop,
    handleDragEnd,
    handleSort,
    resetColumns,
    sortData,
  };
}
