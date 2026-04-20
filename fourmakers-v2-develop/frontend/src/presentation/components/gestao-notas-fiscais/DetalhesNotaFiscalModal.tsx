import { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Eye, ChevronLeft, ChevronRight } from "@/components/ui/system-icons";
import { cn } from "@/lib/utils";

interface Lancamento {
  rubrica: string;
  valor: number;
}

interface NotaFiscalDetalhes {
  numeroNF: string;
  competencia: string;
  dataEmissao: string;
  valor: number;
  status: string;
  lancamentos: Lancamento[];
}

interface DetalhesNotaFiscalModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  notaFiscal: NotaFiscalDetalhes | null;
  getStatusColor: (status: string) => string;
}

export const DetalhesNotaFiscalModal = ({
  open,
  onOpenChange,
  notaFiscal,
  getStatusColor,
}: DetalhesNotaFiscalModalProps) => {
  const [linhasPorPagina, setLinhasPorPagina] = useState("5");
  const [paginaAtual, setPaginaAtual] = useState(1);

  // Resetar paginação quando o modal abrir
  useEffect(() => {
    if (open) {
      setPaginaAtual(1);
    }
  }, [open]);

  if (!notaFiscal) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl">
        <DialogHeader>
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-primary/10 rounded-lg">
                <Eye className="h-6 w-6 text-primary" />
              </div>
              <div>
                <DialogTitle className="text-2xl font-bold">
                  Detalhes da NF - {notaFiscal.numeroNF}
                </DialogTitle>
                <p className="text-sm text-muted-foreground mt-1">
                  Lançamentos para composição da nota
                </p>
              </div>
            </div>
            <div className="text-right">
              <p className="text-sm text-muted-foreground">Status:</p>
              <Badge className={cn("font-medium", getStatusColor(notaFiscal.status))}>
                {notaFiscal.status}
              </Badge>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-6 py-4">
          {/* Informações principais */}
          <Card className="bg-muted/50">
            <CardContent className="p-6">
              <div className="grid grid-cols-3 gap-8">
                <div>
                  <p className="text-sm text-muted-foreground mb-1">Competência</p>
                  <p className="text-2xl font-bold">{notaFiscal.competencia}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground mb-1">Emissão</p>
                  <p className="text-2xl font-bold">{notaFiscal.dataEmissao}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground mb-1">Total dos Lançamentos</p>
                  <p className="text-2xl font-bold">
                    {(notaFiscal.valor ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Tabela de Lançamentos */}
          <Card>
            <CardContent className="p-6">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Rubrica</TableHead>
                    <TableHead className="text-right">Valor</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {notaFiscal.lancamentos.map((lancamento, index) => (
                    <TableRow key={index}>
                      <TableCell className="font-medium">{lancamento.rubrica}</TableCell>
                      <TableCell className="text-right font-medium">
                        {(lancamento.valor ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>

              {/* Paginação */}
              <div className="flex items-center justify-end gap-6 mt-4 pt-4 border-t">
                <div className="flex items-center gap-2">
                  <span className="text-sm text-muted-foreground">Linhas por página:</span>
                  <Select value={linhasPorPagina} onValueChange={setLinhasPorPagina}>
                    <SelectTrigger className="w-16" data-testid="modal-detalhes-linhas-por-pagina">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="5">5</SelectItem>
                      <SelectItem value="10">10</SelectItem>
                      <SelectItem value="20">20</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="flex items-center gap-4">
                  <span className="text-sm text-muted-foreground">
                    1 - {notaFiscal.lancamentos.length} de {notaFiscal.lancamentos.length}
                  </span>
                  <div className="flex gap-1">
                    <Button
                      data-testid="modal-detalhes-pagina-anterior"
                      variant="ghost"
                      size="icon"
                      disabled={paginaAtual === 1}
                      onClick={() => setPaginaAtual(p => Math.max(1, p - 1))}
                    >
                      <ChevronLeft className="h-4 w-4" />
                    </Button>
                    <Button
                      data-testid="modal-detalhes-pagina-proxima"
                      variant="ghost"
                      size="icon"
                      disabled={true}
                      onClick={() => setPaginaAtual(p => p + 1)}
                    >
                      <ChevronRight className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </DialogContent>
    </Dialog>
  );
};

