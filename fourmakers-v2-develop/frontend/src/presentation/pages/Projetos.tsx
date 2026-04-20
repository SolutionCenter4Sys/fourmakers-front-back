import { useState } from "react";
import { Plus, Edit } from "@/components/ui/system-icons";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { SearchCard } from "@presentation/components/common/SearchCard";
import { StatusBadge } from "@presentation/components/common/StatusBadge";
import { DataTable, PageBreadcrumb, PageHeader } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { useProjetos } from "@/hooks/useProjetos";

const Projetos = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const { projetos, loading } = useProjetos();

  const columns: Column[] = [
    { id: "projeto", label: "Projeto", sortable: true },
    { id: "cliente", label: "Cliente", sortable: true },
    { id: "aprovadores", label: "Aprovadores", sortable: false },
    { id: "oportunidade", label: "Oportunidade", sortable: false },
    { id: "dataInicio", label: "Data Início", sortable: true },
    { id: "dataFim", label: "Data Fim", sortable: true },
    { id: "status", label: "Status do Projeto", sortable: true },
    { id: "acoes", label: "Ações", sortable: false, width: "text-right" },
  ];


  const filteredProjetos = projetos.filter((projeto) =>
    projeto.projeto.toLowerCase().includes(searchTerm.toLowerCase()) ||
    projeto.cliente.toLowerCase().includes(searchTerm.toLowerCase()) ||
    projeto.status.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const renderCell = (projeto: typeof projetos[0], columnId: string) => {
    switch (columnId) {
      case "projeto":
        return <span className="font-medium">{projeto.projeto}</span>;
      case "cliente":
        return projeto.cliente;
      case "aprovadores":
        return (
          <div className="flex gap-2">
            {projeto.aprovadores.map((aprovador, idx) => (
              <Badge
                key={idx}
                variant="outline"
                className="text-primary border-primary"
              >
                {aprovador}
              </Badge>
            ))}
          </div>
        );
      case "oportunidade":
        return projeto.oportunidade;
      case "dataInicio":
        return projeto.dataInicio;
      case "dataFim":
        return projeto.dataFim;
      case "status":
        return <StatusBadge status={projeto.status} variant="projeto" />;
      case "acoes":
        return (
          <div className="text-right">
            <Button variant="ghost" size="icon">
              <Edit className="h-4 w-4" />
            </Button>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <div className="container mx-auto p-4 space-y-4">
      <PageBreadcrumb 
        items={[
          { label: 'Projetos' }
        ]} 
      />
      
      <PageHeader 
        title="Projetos"
        description="Gerencie e visualize todos os projetos"
      />

      <SearchCard 
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        placeholder="Buscar projeto..."
        actionButton={
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Adicionar projeto
          </Button>
        }
      />

      {/* Projetos Table */}
      <Card>
        <CardContent className="p-6">
          {loading ? (
            <div className="text-center py-8 text-muted-foreground">Carregando projetos...</div>
          ) : (
            <DataTable
              columns={columns}
              data={filteredProjetos}
              keyExtractor={(item) => item.id}
              renderCell={renderCell}
              emptyMessage="Nenhum projeto encontrado"
            />
          )}
        </CardContent>
      </Card>
    </div>
  );
};

export default Projetos;
