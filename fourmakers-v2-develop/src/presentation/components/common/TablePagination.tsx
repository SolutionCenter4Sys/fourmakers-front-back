import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { ChevronLeft, ChevronRight } from "@/components/ui/system-icons";

interface TablePaginationProps {
  currentPage: number;
  totalItems: number;
  itemsPerPage: number;
  hasMore?: boolean;
  onPageChange: (page: number) => void;
  onItemsPerPageChange: (items: string) => void;
}

export const TablePagination = ({
  currentPage,
  totalItems,
  itemsPerPage,
  hasMore = false,
  onPageChange,
  onItemsPerPageChange,
}: TablePaginationProps) => {
  // Se hasMore é true, garantir que podemos avançar para próxima página
  const totalPages = hasMore ? currentPage + 1 : Math.ceil(totalItems / itemsPerPage);
  const startItem = (currentPage - 1) * itemsPerPage + 1;
  const endItem = Math.min(currentPage * itemsPerPage, totalItems);

  return (
    <div className="flex items-center justify-between py-4 px-4 border-t">
      <div className="flex items-center gap-2">
        <span className="text-sm text-muted-foreground">Linhas por página:</span>
        <Select 
          value={itemsPerPage.toString()} 
          onValueChange={onItemsPerPageChange}
        >
          <SelectTrigger className="w-16 h-8" data-testid="pagination-items-per-page">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="5">5</SelectItem>
            <SelectItem value="10">10</SelectItem>
            <SelectItem value="25">25</SelectItem>
            <SelectItem value="50">50</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <div className="flex items-center gap-4">
        <span className="text-sm text-muted-foreground">
          {startItem} – {endItem} de {totalItems}
        </span>
        <div className="flex items-center gap-1">
          <Button 
            data-testid="pagination-prev"
            variant="ghost" 
            size="icon" 
            className="h-8 w-8"
            onClick={() => onPageChange(currentPage - 1)}
            disabled={currentPage === 1}
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <Button 
            data-testid="pagination-next"
            variant="ghost" 
            size="icon" 
            className="h-8 w-8"
            onClick={() => onPageChange(currentPage + 1)}
            disabled={currentPage === totalPages}
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  );
};
