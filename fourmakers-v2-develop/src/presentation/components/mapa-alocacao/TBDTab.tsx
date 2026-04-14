import { useState, useEffect, useCallback, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Plus, Search } from "@/components/ui/system-icons";
import { DataTable, TablePagination } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { useAppSelector } from "@/app/store/hooks";
import { container } from "@/core/di/container";
import { ListarTbdUseCase } from "@domain/usecases/ListarTbdUseCase";
import type { Tbd } from "@domain/entities/Tbd";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { Edit2 } from "@/components/ui/system-icons";
import { Skeleton } from "@/components/ui/skeleton";

export const TBDTab = () => {
  const navigate = useNavigate();
  const { token } = useAppSelector((state) => state.auth);
  const [tbds, setTbds] = useState<Tbd[]>([]);
  const [loading, setLoading] = useState(false);
  const [busca, setBusca] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10);

  const columns: Column[] = useMemo(() => [
    { id: "codTbdAlocado", label: "Cód", sortable: true },
    { id: "descricao", label: "Nome", sortable: true },
    { id: "descricaoDiretoria", label: "Unidade", sortable: true },
    { id: "descricaoDepartamento", label: "Departamento", sortable: true },
    { id: "codGestor", label: "Gestor", sortable: true },
    { id: "dataCriacao", label: "Data Criação", sortable: true },
    { id: "dataAlteracao", label: "Data Alteração", sortable: true },
    { id: "acoes", label: "Ações", sortable: false },
  ], []);

  const carregarTbds = useCallback(async () => {
    if (!token) return;

    try {
      setLoading(true);
      const useCase = container.resolve(ListarTbdUseCase);
      const data = await useCase.execute(token);
      setTbds(data);
    } catch (error) {
      console.error("Erro ao carregar TBDs:", error);
    } finally {
      setLoading(false);
    }
  }, [token]);

  useEffect(() => {
    carregarTbds();
  }, [carregarTbds]);

  const renderCell = (columnId: string, tbd: Tbd) => {
    switch (columnId) {
      case "codTbdAlocado":
        return <span>{tbd.codTbdAlocado}</span>;
      case "descricao":
        return <span>{tbd.descricao}</span>;
      case "descricaoDiretoria":
        return <span>{tbd.descricaoDiretoria || "-"}</span>;
      case "descricaoDepartamento":
        return <span>{tbd.descricaoDepartamento || "-"}</span>;
      case "codGestor":
        return <span>{tbd.codGestor || "-"}</span>;
      case "dataCriacao":
        try {
          const date = new Date(tbd.dataCriacao.split("-").reverse().join("-"));
          return <span>{format(date, "dd/MM/yyyy", { locale: ptBR })}</span>;
        } catch {
          return <span>{tbd.dataCriacao}</span>;
        }
      case "dataAlteracao":
        try {
          const date = new Date(tbd.dataAlteracao.split("-").reverse().join("-"));
          return <span>{format(date, "dd/MM/yyyy", { locale: ptBR })}</span>;
        } catch {
          return <span>{tbd.dataAlteracao}</span>;
        }
      case "acoes":
        return (
          <Button
            variant="ghost"
            size="icon"
            onClick={() => navigate(`/mapa-alocacao/tbd/${tbd.codTbdAlocado}`)}
            title="Editar"
          >
            <Edit2 className="h-4 w-4" />
          </Button>
        );
      default:
        return null;
    }
  };

  // Filtrar TBDs por busca
  const tbdsFiltrados = useMemo(() => {
    if (!busca.trim()) return tbds;
    
    const buscaLower = busca.toLowerCase().trim();
    return tbds.filter((tbd) => {
      return (
        tbd.descricao?.toLowerCase().includes(buscaLower) ||
        tbd.descricaoDiretoria?.toLowerCase().includes(buscaLower) ||
        tbd.descricaoDepartamento?.toLowerCase().includes(buscaLower) ||
        tbd.codGestor?.toLowerCase().includes(buscaLower) ||
        tbd.codTbdAlocado.toString().includes(buscaLower)
      );
    });
  }, [tbds, busca]);

  const paginatedData = useMemo(() => {
    const start = (currentPage - 1) * itemsPerPage;
    const end = start + itemsPerPage;
    return tbdsFiltrados.slice(start, end);
  }, [tbdsFiltrados, currentPage, itemsPerPage]);

  // Resetar página quando busca mudar
  useEffect(() => {
    setCurrentPage(1);
  }, [busca]);

  return (
    <Card>
      <CardHeader>
        <div className="flex justify-between items-center">
          <CardTitle>TBDs</CardTitle>
          <Button onClick={() => navigate("/mapa-alocacao/tbd/novo")}>
            <Plus className="h-4 w-4 mr-2" />
            Cadastrar TBD
          </Button>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Buscar TBDs..."
            value={busca}
            onChange={(e) => setBusca(e.target.value)}
            className="pl-9"
            disabled={loading}
          />
        </div>

        {loading ? (
          <div className="space-y-4">
            {/* Skeleton do cabeçalho da tabela */}
            <div className="flex items-center gap-4 border-b pb-4">
              <Skeleton className="h-10 w-20" />
              <Skeleton className="h-10 w-48" />
              <Skeleton className="h-10 w-40" />
              <Skeleton className="h-10 w-44" />
              <Skeleton className="h-10 w-36" />
              <Skeleton className="h-10 w-36" />
              <Skeleton className="h-10 w-36" />
              <Skeleton className="h-10 w-24" />
            </div>
            {/* Skeleton das linhas */}
            {[...Array(8)].map((_, index) => (
              <div key={index} className="flex items-center gap-4 py-3 border-b">
                <Skeleton className="h-10 w-20" />
                <Skeleton className="h-10 w-48" />
                <Skeleton className="h-10 w-40" />
                <Skeleton className="h-10 w-44" />
                <Skeleton className="h-10 w-36" />
                <Skeleton className="h-10 w-36" />
                <Skeleton className="h-10 w-36" />
                <Skeleton className="h-10 w-24" />
              </div>
            ))}
          </div>
        ) : (
          <>
            <DataTable
              columns={columns}
              data={paginatedData}
              renderCell={(item, columnId) => renderCell(columnId, item)}
              keyExtractor={(item) => item.codTbdAlocado.toString()}
              emptyMessage="Nenhum TBD encontrado"
            />

            {tbdsFiltrados.length > 0 && (
              <TablePagination
                currentPage={currentPage}
                itemsPerPage={itemsPerPage}
                totalItems={tbdsFiltrados.length}
                onPageChange={setCurrentPage}
                onItemsPerPageChange={(value) => setItemsPerPage(Number(value))}
              />
            )}
          </>
        )}
      </CardContent>
    </Card>
  );
};
