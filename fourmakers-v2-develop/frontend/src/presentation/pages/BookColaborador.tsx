import { useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { Search, FileText, Plus } from "@/components/ui/system-icons";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { DataTable } from "@presentation/components/common";
import { StatCard } from "@presentation/components/common/StatCard";
import { Users, Clock } from "@/components/ui/system-icons";
import type { Column } from "@/hooks/useColumnReorder";
import {
  useEquipe,
  useColaboradoresEquipe,
} from "@/presentation/hooks/useBookColaborador";
import {
  formatSaldoHoras,
  formatSalario,
  formatData,
  formatTelefone,
} from "@shared/utils/bookColaboradorFormatters";
import type { ColaboradorBook } from "@shared/types/bookColaborador";

export default function BookColaborador() {
  const navigate = useNavigate();
  const equipeId = 1; // TODO: Pegar da URL ou contexto
  const { equipe, loading: loadingEquipe } = useEquipe(equipeId);
  const { colaboradores, loading: loadingColaboradores } =
    useColaboradoresEquipe(equipeId);
  const [busca, setBusca] = useState("");

  const filteredColaboradores = useMemo(() => {
    if (!busca) return colaboradores;
    const buscaLower = busca.toLowerCase();
    return colaboradores.filter(
      (col) =>
        col.nome.toLowerCase().includes(buscaLower) ||
        col.email.toLowerCase().includes(buscaLower) ||
        col.cargo.toLowerCase().includes(buscaLower)
    );
  }, [colaboradores, busca]);

  const totalColaboradores = colaboradores.length;
  const saldoTotalHoras = colaboradores.reduce(
    (acc, col) => acc + col.saldoHoras,
    0
  );

  const columns: Column[] = [
    { id: "colaborador", label: "Colaborador", sortable: true },
    { id: "cargo", label: "Cargo", sortable: true },
    { id: "modelo", label: "Modelo", sortable: true },
    { id: "saldoHoras", label: "Saldo de Horas", sortable: true },
    { id: "salario", label: "Salário", sortable: true },
    { id: "status", label: "Status", sortable: true },
    { id: "email", label: "E-mail", sortable: true },
    { id: "telefone", label: "Telefone", sortable: true },
    { id: "nascimento", label: "Nascimento", sortable: true },
    { id: "tempoCasa", label: "Tempo", sortable: true },
  ];

  const renderCell = (colaborador: ColaboradorBook, columnId: string) => {
    switch (columnId) {
      case "colaborador":
        return (
          <div>
            <div className="font-medium">{colaborador.nome}</div>
            <div className="text-sm text-muted-foreground">
              #{colaborador.id}
            </div>
          </div>
        );
      case "cargo":
        return colaborador.cargo;
      case "modelo":
        return colaborador.modelo;
      case "saldoHoras":
        return formatSaldoHoras(colaborador.saldoHoras);
      case "salario":
        return formatSalario(colaborador.salario);
      case "status":
        return (
          <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800">
            {colaborador.status}
          </span>
        );
      case "email":
        return colaborador.email;
      case "telefone":
        return formatTelefone(colaborador.telefone);
      case "nascimento":
        return formatData(colaborador.nascimento);
      case "tempoCasa":
        return colaborador.tempoCasa;
      default:
        return null;
    }
  };

  const handleExportarRelatorio = () => {
    if (filteredColaboradores.length === 0) {
      alert("Não há dados para exportar.");
      return;
    }

    const headers = [
      "Colaborador",
      "ID",
      "Cargo",
      "Modelo",
      "Saldo de Horas",
      "Salário",
      "Status",
      "E-mail",
      "Telefone",
      "Nascimento",
      "Tempo de Casa",
    ];

    const rows = filteredColaboradores.map((col) => [
      col.nome,
      col.id,
      col.cargo,
      col.modelo,
      formatSaldoHoras(col.saldoHoras),
      formatSalario(col.salario),
      col.status,
      col.email,
      formatTelefone(col.telefone),
      formatData(col.nascimento),
      col.tempoCasa,
    ]);

    const csvContent = [
      headers.join(","),
      ...rows.map((row) =>
        row.map((cell) => `"${String(cell).replace(/"/g, '""')}"`).join(",")
      ),
    ].join("\n");

    const BOM = "\uFEFF";
    const blob = new Blob([BOM + csvContent], {
      type: "text/csv;charset=utf-8;",
    });

    const url = window.URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;

    const dataAtual = new Date().toISOString().split("T")[0];
    const nomeEquipe = equipe?.nome.replace(/\s+/g, "_") || "equipe";
    const fileName = `relatorio_colaboradores_${nomeEquipe}_${dataAtual}.csv`;

    link.setAttribute("download", fileName);
    document.body.appendChild(link);
    link.click();

    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  };

  if (loadingEquipe || loadingColaboradores) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-8 text-muted-foreground">
          Carregando...
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-4 space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Visão Geral da Equipe</h1>
          {equipe && (
            <p className="text-muted-foreground mt-1">
              {equipe.nome} • {totalColaboradores} colaboradores
            </p>
          )}
        </div>
        <div className="flex gap-3">
          <Button
            onClick={handleExportarRelatorio}
            variant="outline"
            className="border-foreground shadow-sm"
          >
            <FileText className="h-4 w-4 mr-2" />
            Exportar Relatório
          </Button>
          <Button className="px-6 shadow-sm">
            <Plus className="h-4 w-4 mr-2" />
            Nova Ação
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <StatCard
          title="Total de Colaboradores"
          value={totalColaboradores.toString()}
          icon={Users}
          color="text-blue-600"
          bgColor="bg-blue-100"
          description="Ativos na equipe"
        />
        <StatCard
          title="Saldo de Horas"
          value={formatSaldoHoras(saldoTotalHoras)}
          icon={Clock}
          color="text-green-600"
          bgColor="bg-green-100"
          description="Acumulado da equipe"
        />
      </div>

      <Card className="rounded-xl">
        <CardContent className="p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-xl font-semibold">
              Colaboradores
              <span className="text-muted-foreground font-normal ml-2">
                {filteredColaboradores.length} de {totalColaboradores}{" "}
                colaboradores
              </span>
            </h2>
            <div className="relative max-w-md">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar colaborador..."
                value={busca}
                onChange={(e) => setBusca(e.target.value)}
                className="pl-10"
              />
            </div>
          </div>

          <DataTable
            columns={columns}
            data={filteredColaboradores}
            keyExtractor={(item) => item.codColaborador}
            renderCell={renderCell}
            emptyMessage="Nenhum colaborador encontrado"
            onRowClick={(item) =>
              navigate(`/book-colaborador/${item.codColaborador}`)
            }
          />
        </CardContent>
      </Card>
    </div>
  );
}
