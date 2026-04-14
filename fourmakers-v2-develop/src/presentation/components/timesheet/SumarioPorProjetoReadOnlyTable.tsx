import { Badge } from "@/components/ui/badge";
import { DataTable } from "@presentation/components/common/DataTable";
import type { Column } from "@/hooks/useColumnReorder";
import type { ApontamentoMensal } from "@data/api/TimesheetComponentesApi";

interface SumarioPorProjetoReadOnlyTableProps {
  apontamentosMensais: ApontamentoMensal[];
  formatarHoras: (minutos: number) => string;
  /** Exibe a coluna Aprovador(es). Controlado por parâmetro MOSTRAR_COLUNA_APROVADORES_TIMESHEET no Redux. */
  mostrarColunaAprovadores?: boolean;
}

export function SumarioPorProjetoReadOnlyTable({
  apontamentosMensais,
  formatarHoras,
  mostrarColunaAprovadores = false,
}: SumarioPorProjetoReadOnlyTableProps) {
  const getStatusLabel = (codStatus: number) => {
    switch (codStatus) {
      case 1:
        return "Pendente";
      case 2:
        return "Aprovado";
      case 3:
        return "Reprovado";
      default:
        return "—";
    }
  };

  const getStatusBadgeClass = (codStatus: number) => {
    switch (codStatus) {
      case 1:
        return "bg-blue-600 text-white";
      case 2:
        return "bg-green-600 text-white";
      case 3:
        return "bg-red-600 text-white";
      default:
        return "bg-muted text-muted-foreground";
    }
  };

  const columns: Column[] = [
    { id: "clienteProjeto", label: "Cliente/Projeto", sortable: true },
    { id: "atividade", label: "Atividade", sortable: true },
    ...(mostrarColunaAprovadores ? [{ id: "aprovadores", label: "Aprovador(es)", sortable: false }] : []),
    { id: "status", label: "Status", sortable: true },
    { id: "horas", label: "Horas", sortable: true },
  ];

  const renderCell = (item: ApontamentoMensal, columnId: string) => {
    const { projeto, atividade, aprovadores, codStatusGrupoMensal, horas } = item;
    const clienteProjetoLabel = `${projeto.codCliente} - ${projeto.nomeCliente} / ${projeto.id} - ${projeto.nomeProjeto}`;

    switch (columnId) {
      case "clienteProjeto":
        return <span className="text-sm">{clienteProjetoLabel}</span>;
      case "atividade":
        return <span className="text-sm">{atividade}</span>;
      case "aprovadores":
        if (!mostrarColunaAprovadores) return null;
        if (!aprovadores?.length) return <span className="text-muted-foreground">—</span>;
        const primeiro = aprovadores[0];
        const restantes = aprovadores.length - 1;
        return (
          <div className="flex items-center gap-1 flex-wrap">
            <Badge variant="outline" className="text-xs font-normal">
              {primeiro.nome}
            </Badge>
            {restantes > 0 && (
              <Badge variant="outline" className="text-xs font-normal text-muted-foreground">
                +{restantes}
              </Badge>
            )}
          </div>
        );
      case "status":
        return (
          <Badge className={getStatusBadgeClass(codStatusGrupoMensal)}>
            {getStatusLabel(codStatusGrupoMensal)}
          </Badge>
        );
      case "horas":
        return <span className="text-sm font-medium">{formatarHoras(horas)}</span>;
      default:
        return null;
    }
  };

  const dataWithKey = apontamentosMensais.map((item, index) => ({
    ...item,
    _rowKey: `${item.projeto.id}-${item.atividade}-${index}`,
  }));

  const quantidadeProjetos = new Set(apontamentosMensais.map((m) => m.projeto.id)).size;
  const somaHorasTotal = apontamentosMensais.reduce((acc, m) => acc + m.horas, 0);

  return (
    <div>
      <DataTable
        columns={columns}
        data={dataWithKey}
        renderCell={renderCell}
        keyExtractor={(item) => (item as ApontamentoMensal & { _rowKey: string })._rowKey}
        emptyMessage="Nenhum projeto encontrado."
      />
      <div className="flex flex-wrap items-center gap-6 py-4 px-4 border-t bg-muted/30 text-sm">
        <span className="font-medium">
          Contagem de projetos: <span className="text-foreground">{quantidadeProjetos}</span>
        </span>
        <span className="font-medium">
          Soma de horas:{" "}
          <span className="text-foreground">{formatarHoras(somaHorasTotal)}</span>
        </span>
      </div>
    </div>
  );
}
