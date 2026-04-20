import { GripVertical, ArrowUpDown, ArrowUp, ArrowDown } from "@/components/ui/system-icons";
import { TableHead } from "@/components/ui/table";
import { cn } from "@/lib/utils";
import type { SortDirection } from "@/hooks/useColumnReorder";

interface DraggableTableHeadProps {
  columnId: string;
  children: React.ReactNode;
  className?: string;
  isDragging?: boolean;
  sortable?: boolean;
  sortDirection?: SortDirection;
  onDragStart: (columnId: string) => void;
  onDragOver: (e: React.DragEvent) => void;
  onDrop: (columnId: string) => void;
  onDragEnd: () => void;
  onSort?: (columnId: string) => void;
  sticky?: boolean;
  /** Offset em px para coluna fixa à direita (0 = coluna mais à direita). */
  stickyRightOffset?: number;
  stickyLeft?: boolean;
  stickyLeftOffset?: number;
  align?: "left" | "center" | "right";
}

export const DraggableTableHead = ({
  columnId,
  children,
  className,
  isDragging,
  sortable = true,
  sortDirection,
  onDragStart,
  onDragOver,
  onDrop,
  onDragEnd,
  onSort,
  sticky = false,
  stickyRightOffset,
  stickyLeft = false,
  stickyLeftOffset,
  align = "left",
}: DraggableTableHeadProps) => {
  const SortIcon = sortDirection === "asc" ? ArrowUp : sortDirection === "desc" ? ArrowDown : ArrowUpDown;
  
  const alignClass = align === "center"
    ? "text-center"
    : align === "right"
    ? "text-right"
    : "text-left";

  return (
    <TableHead
      className={cn(
        "relative group select-none min-w-[5.5rem]",
        alignClass,
        isDragging && "opacity-50",
        sticky && "sticky z-30 min-w-[180px] bg-[var(--color-surface-elevated)] sticky-col-shadow-fixed",
        stickyLeft && "sticky bg-surfaceElevated z-10 shadow-[inset_1px_0_0_0_hsl(var(--border))]",
        className
      )}
      style={
        stickyLeft && stickyLeftOffset !== undefined
          ? { left: `${stickyLeftOffset}px`, position: 'sticky' as const, zIndex: 10 }
          : sticky
            ? { right: `${stickyRightOffset ?? 0}px`, position: 'sticky' as const, zIndex: 30 }
            : undefined
      }
      draggable
      onDragStart={() => onDragStart(columnId)}
      onDragOver={onDragOver}
      onDrop={() => onDrop(columnId)}
      onDragEnd={onDragEnd}
    >
      <div className="flex items-center gap-3 min-h-8">
        <span className={cn("flex-1 min-w-0 leading-tight whitespace-nowrap overflow-hidden text-ellipsis", alignClass)}>
          {children}
        </span>
        <div className="flex items-center gap-1.5 shrink-0 min-w-[3rem]">
          <GripVertical className="h-4 w-4 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity cursor-grab active:cursor-grabbing" />
          {sortable && onSort && (
            <button
              onClick={(e) => {
                e.stopPropagation();
                onSort(columnId);
              }}
              className="opacity-0 group-hover:opacity-100 transition-opacity hover:text-foreground p-0.5"
            >
              <SortIcon className="h-4 w-4" />
            </button>
          )}
        </div>
      </div>
    </TableHead>
  );
};
