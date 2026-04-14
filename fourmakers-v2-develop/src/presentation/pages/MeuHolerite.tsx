import { useState } from "react";
import avatarImage from "@/assets/avatar.png";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { DataTable } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { Eye } from "@/components/ui/system-icons";
import { Button } from "@/components/ui/button";
import { useHolerites } from "@/hooks/useHolerites";
import type { Holerite } from "@domain/entities/Holerite";
import { useAppSelector } from "@app/store/hooks";
import { processarUrlComToken } from "@shared/utils/urlUtils";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";

const MeuHolerite = () => {
  const anoAtual = new Date().getFullYear();
  const [anoSelecionado, setAnoSelecionado] = useState<number>(anoAtual);
  const { holerites, loading, error, assinarHolerite } = useHolerites(anoSelecionado);
  const [assinando, setAssinando] = useState<string | null>(null);
  const { token } = useAppSelector((state) => state.auth);
  const columns: Column[] = [
    { id: "mesAno", label: "Mês/Ano", sortable: true },
    { id: "diasTrabalhados", label: "Dias trabalhados", sortable: true },
    { id: "saldoHoras", label: "Saldo de horas", sortable: true },
    { id: "acoes", label: "", sortable: false, width: "w-[50px]" },
  ];

  const getTipoHolerite = (holerite: Holerite): string => {
    if (holerite.informeDeRendimentos) return "Informe de Rendimentos";
    if (holerite.adiantamento) return "Adiantamento";
    if (holerite.ferias) return "Férias";
    if (holerite.decimoTerceiroAdiantamento) return "Adiantamento 13º";
    if (holerite.decimoTerceiro) return "Quitação 13º";
    return "Holerite";
  };

  const isTipoEspecial = (holerite: Holerite): boolean => {
    return !!(
      holerite.informeDeRendimentos ||
      holerite.adiantamento ||
      holerite.ferias ||
      holerite.decimoTerceiro ||
      holerite.decimoTerceiroAdiantamento
    );
  };

  const formatarDataVisualizacao = (dataString: string | null): string => {
    if (!dataString) return "Não visualizado";
    try {
      const data = new Date(dataString);
      if (Number.isNaN(data.getTime())) return "Não visualizado";
      return `Visualizado: ${format(data, "d 'de' MMMM 'de' yyyy", { locale: ptBR })}`;
    } catch {
      return "Não visualizado";
    }
  };

  const renderCell = (holerite: Holerite, columnId: string, index: number) => {
    const tipoEspecial = isTipoEspecial(holerite);
    const tipoHolerite = getTipoHolerite(holerite);

    switch (columnId) {
      case "mesAno":
        return (
          <div className="space-y-1">
            <div className="flex items-center gap-2">
              <span className="font-semibold text-foreground">{holerite.competencia}</span>
              {index === 0 && (
                <Badge variant="secondary" className="bg-muted text-muted-foreground hover:bg-muted">
                  Recente
                </Badge>
              )}
            </div>
            <div className="text-sm text-muted-foreground">
              {holerite.assinado ? formatarDataVisualizacao(holerite.assinadoEm) : "Não visualizado"}
            </div>
            <div className="mt-1">
              <Badge 
                variant="outline" 
                className="bg-blue-50 text-blue-700 border-blue-200 hover:bg-blue-50 dark:bg-blue-900/30 dark:text-blue-300 dark:border-blue-700"
              >
                {tipoHolerite}
              </Badge>
            </div>
          </div>
        );
      case "diasTrabalhados":
        if (tipoEspecial) {
          return (
            <div className="rounded-lg bg-muted px-4 py-2 text-center">
              <div className="text-sm text-muted-foreground">-</div>
            </div>
          );
        }
        return (
          <div className="rounded-lg bg-muted px-3 sm:px-4 py-2 text-center min-w-[4rem]">
            <div className="text-xl sm:text-2xl font-bold text-foreground">
              {holerite.diasTrabalhados.toString().padStart(2, '0')}
            </div>
            <div className="text-xs text-muted-foreground">Dias trabalhados</div>
          </div>
        );
      case "saldoHoras":
        if (tipoEspecial) {
          return (
            <div className="rounded-lg bg-muted px-4 py-2 text-center">
              <div className="text-sm text-muted-foreground">-</div>
            </div>
          );
        }
        return (
          <div className="rounded-lg bg-muted px-3 sm:px-4 py-2 text-center min-w-[4rem]">
            <div className="text-xl sm:text-2xl font-bold text-foreground">
              {holerite.totalHorasExtras || "00:00"}
            </div>
            <div className="text-xs text-muted-foreground">Saldo de horas</div>
          </div>
        );
      case "acoes":
        const isAssinando = assinando === holerite.tbItemLoteId;
        return (
          <Button 
            variant="ghost" 
            size="icon"
            disabled={isAssinando}
            onClick={async () => {
              if (!holerite.holeritePdf) return;

              const urlProcessada = processarUrlComToken(holerite.holeritePdf, token);
              if (!urlProcessada) {
                console.error('Erro ao processar URL do PDF');
                return;
              }

              if (!holerite.assinado) {
                setAssinando(holerite.tbItemLoteId);
                try {
                  const sucesso = await assinarHolerite(holerite.tbItemLoteId);
                  if (sucesso) {
                    window.open(urlProcessada, '_blank');
                  }
                } catch (err) {
                  console.error('Erro ao assinar holerite:', err);
                } finally {
                  setAssinando(null);
                }
              } else {
                window.open(urlProcessada, '_blank');
              }
            }}
          >
            <Eye className="h-5 w-5" />
          </Button>
        );
      default:
        return null;
    }
  };
  return (
    <div className="min-h-screen bg-primaryBackground p-4 sm:p-6">
      <div className="mx-auto max-w-7xl space-y-4 sm:space-y-6">

        {/* Hero Banner — empilha em mobile, lado a lado em sm+ */}
        <div className="relative overflow-hidden rounded-2xl bg-gradient-to-r from-purple-900 via-purple-800 to-teal-800 p-4 sm:p-6 shadow-lg">
          <div className="flex flex-col sm:flex-row items-center sm:items-center gap-4 sm:gap-6">
            {/* Avatar — menor em mobile */}
            <div className="relative flex-shrink-0">
              <img 
                src={avatarImage} 
                alt="Avatar" 
                className="h-20 w-20 sm:h-28 sm:w-28 md:h-32 md:w-32 rounded-full object-cover"
              />
            </div>

            {/* Speech Bubble */}
            <div className="relative w-full sm:max-w-md rounded-2xl bg-white/20 backdrop-blur-sm px-4 sm:px-6 py-3 sm:py-4 shadow-lg text-center sm:text-left">
              <p className="text-base sm:text-lg font-medium text-white">
                Encontre seu holerite abaixo
              </p>
            </div>
          </div>
        </div>

        {/* Header Section — empilha em mobile (título + select em coluna) */}
        <Card className="p-4 sm:p-6">
          <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
            <div className="space-y-1">
              <h2 className="text-lg sm:text-xl font-semibold text-foreground">
                Consulta de recibos de pagamento
              </h2>
              <p className="text-sm text-muted-foreground">
                Encontre abaixo o holerite referente ao mês e ano desejado
              </p>
            </div>
            <div className="flex flex-col gap-1 w-full sm:w-auto min-w-0">
              <label className="text-sm font-medium text-foreground">
                Ano de referência
              </label>
              <Select 
                value={anoSelecionado.toString()} 
                onValueChange={(value) => setAnoSelecionado(parseInt(value, 10))}
              >
                <SelectTrigger className="w-full sm:w-[200px]">
                  <SelectValue placeholder="Selecione..." />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={anoAtual.toString()}>{anoAtual}</SelectItem>
                  <SelectItem value={(anoAtual - 1).toString()}>{anoAtual - 1}</SelectItem>
                  <SelectItem value={(anoAtual - 2).toString()}>{anoAtual - 2}</SelectItem>
                  <SelectItem value={(anoAtual - 3).toString()}>{anoAtual - 3}</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </Card>

        {/* Table Section — scroll horizontal em telas pequenas (DataTable já usa overflow-x-auto) */}
        <Card className="p-4 sm:p-6 overflow-hidden">
          {loading ? (
            <div className="text-center py-8 text-muted-foreground text-sm sm:text-base">Carregando holerites...</div>
          ) : error ? (
            <div className="text-center py-8 text-destructive text-sm sm:text-base">{error}</div>
          ) : (
            <div className="min-w-0 min-h-[200px]" role="region" aria-label="Lista de holerites">
            <DataTable
              columns={columns}
              data={holerites}
              keyExtractor={(item) => item.tbItemLoteId}
              renderCell={(item, columnId) => renderCell(item, columnId, holerites.indexOf(item))}
              emptyMessage="Nenhum holerite encontrado"
            />
            </div>
          )}
        </Card>
      </div>
    </div>
  );
};

export default MeuHolerite;
