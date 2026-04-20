import { useState } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { SearchCard } from "@presentation/components/common/SearchCard";
import { TablePagination } from "@presentation/components/common/TablePagination";
import { useReembolsoLogs } from "@/hooks/useReembolsoParametros";

export const LogTab = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [rowsPerPage, setRowsPerPage] = useState("5");
  const [currentPage, setCurrentPage] = useState(1);
  const { logs, loading } = useReembolsoLogs();

  return (
    <div className="space-y-6">
      <SearchCard 
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        placeholder="Buscar logs..."
      />

      {/* Table Card */}
      <Card>
        <CardContent className="p-6">
          <div className="standard-table-wrapper">
            <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Regra</TableHead>
                <TableHead>Ação</TableHead>
                <TableHead>De:</TableHead>
                <TableHead>Para:</TableHead>
                <TableHead>Alterado em:</TableHead>
                <TableHead>Alterado por</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                    Carregando logs...
                  </TableCell>
                </TableRow>
              ) : (
                logs.map((log, index) => (
                  <TableRow key={index}>
                    <TableCell className="font-medium">{log.regra}</TableCell>
                    <TableCell>{log.acao}</TableCell>
                    <TableCell>{log.de}</TableCell>
                    <TableCell>{log.para}</TableCell>
                    <TableCell>{log.alteradoEm}</TableCell>
                    <TableCell>{log.alteradoPor}</TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
          </div>

          <TablePagination
            currentPage={currentPage}
            totalItems={logs.length}
            itemsPerPage={parseInt(rowsPerPage)}
            onPageChange={setCurrentPage}
            onItemsPerPageChange={setRowsPerPage}
          />
        </CardContent>
      </Card>
    </div>
  );
};
