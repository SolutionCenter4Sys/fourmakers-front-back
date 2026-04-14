import { useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Upload, FileText, Eye, Building2, Calendar, Hash } from "@/components/ui/system-icons";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { StatusBadge } from "@presentation/components/common/StatusBadge";
import { useToast } from "@/hooks/use-toast";
import { Separator } from "@/components/ui/separator";
import { DataTable } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";

interface LoteProcessado {
  id: number;
  competencia: string;
  empresa: string;
  cnpj: string;
  arquivo: string;
  qtdPag: number;
  status: "processado" | "erro" | "processando";
  processadoErro: string;
}

interface ColaboradorDetalhe {
  id: number;
  colaborador: string;
  cargo: string;
  cpf: string;
  matricula: string;
  arquivo: string;
  erro?: string;
}

import { useIntegracaoFolhaPonto } from "@/hooks/useIntegracaoFolhaPonto";

export default function IntegracaoFolhaPonto() {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const { lotes, loading: lotesLoading, getColaboradoresPorLote } = useIntegracaoFolhaPonto();
  const [colaboradoresDetalhe, setColaboradoresDetalhe] = useState<Record<number, ColaboradorDetalhe[]>>({});
  const [selectedLote, setSelectedLote] = useState<LoteProcessado | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);
  const { toast } = useToast();

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedFile(file);
    }
  };

  const handleUpload = () => {
    if (!selectedFile) {
      toast({
        title: "Nenhum arquivo selecionado",
        description: "Por favor, selecione um arquivo antes de enviar.",
        variant: "destructive",
      });
      return;
    }

    toast({
      title: "Arquivo enviado",
      description: `${selectedFile.name} foi enviado com sucesso.`,
    });
    setSelectedFile(null);
  };

  const handleViewDetails = async (lote: LoteProcessado) => {
    setSelectedLote(lote);
    setIsDetailModalOpen(true);
    // Carregar colaboradores do lote se ainda não foram carregados
    if (!colaboradoresDetalhe[lote.id]) {
      const colaboradores = await getColaboradoresPorLote(lote.id);
      setColaboradoresDetalhe(prev => ({
        ...prev,
        [lote.id]: colaboradores,
      }));
    }
  };

  const columns: Column[] = [
    { id: "competencia", label: "Competência", sortable: true },
    { id: "empresa", label: "Empresa", sortable: true },
    { id: "cnpj", label: "CNPJ", sortable: true },
    { id: "arquivo", label: "Arquivo", sortable: true },
    { id: "qtdPag", label: "Qtd Pág", sortable: true },
    { id: "status", label: "Status", sortable: true },
    { id: "processadoErro", label: "Processado/Erro", sortable: true },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[120px]" },
  ];

  const renderCell = (lote: LoteProcessado, columnId: string) => {
    switch (columnId) {
      case "competencia":
        return <span className="font-medium">{lote.competencia}</span>;
      case "empresa":
        return lote.empresa;
      case "cnpj":
        return lote.cnpj;
      case "arquivo":
        return (
          <div className="flex items-center gap-2">
            <FileText className="h-4 w-4 text-muted-foreground" />
            {lote.arquivo}
          </div>
        );
      case "qtdPag":
        return lote.qtdPag;
      case "status":
        return <StatusBadge status={lote.status} variant="reembolso" />;
      case "processadoErro":
        return lote.processadoErro;
      case "acoes":
        return (
          <Button variant="outline" size="sm" onClick={() => handleViewDetails(lote)}>
            <Eye className="h-4 w-4 mr-2" />
            Detalhes
          </Button>
        );
      default:
        return null;
    }
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="page-title">Integração Folha Ponto</h1>
          <p className="text-muted-foreground mt-1">
            Centralize o controle de apontamento de horas de forma automatizada
          </p>
        </div>
      </div>

      {/* Upload Section */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Upload className="h-5 w-5" />
            Upload de Arquivos
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="flex items-center gap-4">
              <div className="flex-1">
                <Input
                  type="file"
                  accept=".xlsx,.xls,.csv"
                  onChange={handleFileSelect}
                  className="cursor-pointer"
                />
              </div>
              <Button onClick={handleUpload} disabled={!selectedFile}>
                <Upload className="h-4 w-4 mr-2" />
                Enviar
              </Button>
            </div>
            {selectedFile && (
              <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <FileText className="h-4 w-4" />
                <span>{selectedFile.name}</span>
              </div>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Lista de Lotes Processados */}
      <Card>
        <CardHeader>
          <CardTitle>Lista de Lotes Processados</CardTitle>
        </CardHeader>
        <CardContent>
          {lotesLoading ? (
            <div className="text-center py-8 text-muted-foreground">Carregando lotes...</div>
          ) : (
            <DataTable
              columns={columns}
              data={lotes as LoteProcessado[]}
              keyExtractor={(item) => item.id.toString()}
              renderCell={renderCell}
              emptyMessage="Nenhum lote processado"
            />
          )}
        </CardContent>
      </Card>

      {/* Modal de Detalhes */}
      <Dialog open={isDetailModalOpen} onOpenChange={setIsDetailModalOpen}>
        <DialogContent className="max-w-6xl max-h-[85vh] bg-white p-0 gap-0 overflow-hidden">
          {/* Header */}
          <div className="relative bg-white p-6 pb-4 border-b">
            <DialogHeader>
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                    <FileText className="h-5 w-5 text-primary" />
                  </div>
                  <div>
                    <DialogTitle className="text-2xl font-bold">Detalhes do Lote</DialogTitle>
                    <p className="text-sm text-muted-foreground mt-0.5">
                      Visualize informações completas do processamento
                    </p>
                  </div>
                </div>
              </div>
            </DialogHeader>
          </div>

          {selectedLote && (
            <div className="overflow-y-auto max-h-[calc(85vh-120px)] px-6 pb-6">
              {/* Cards de Informações */}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
                <Card className="border-2 hover:border-primary/50 transition-colors">
                  <CardContent className="p-4">
                    <div className="flex items-start gap-3">
                      <div className="h-9 w-9 rounded-lg bg-blue-500/10 flex items-center justify-center flex-shrink-0">
                        <Building2 className="h-4 w-4 text-blue-600" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
                          Empresa
                        </p>
                        <p className="font-semibold text-sm leading-tight truncate" title={selectedLote.empresa}>
                          {selectedLote.empresa}
                        </p>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card className="border-2 hover:border-primary/50 transition-colors">
                  <CardContent className="p-4">
                    <div className="flex items-start gap-3">
                      <div className="h-9 w-9 rounded-lg bg-purple-500/10 flex items-center justify-center flex-shrink-0">
                        <Hash className="h-4 w-4 text-purple-600" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
                          CNPJ/CPF
                        </p>
                        <p className="font-semibold text-sm leading-tight">
                          {selectedLote.cnpj}
                        </p>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card className="border-2 hover:border-primary/50 transition-colors">
                  <CardContent className="p-4">
                    <div className="flex items-start gap-3">
                      <div className="h-9 w-9 rounded-lg bg-orange-500/10 flex items-center justify-center flex-shrink-0">
                        <Calendar className="h-4 w-4 text-orange-600" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
                          Competência
                        </p>
                        <p className="font-semibold text-sm leading-tight">
                          {selectedLote.competencia}
                        </p>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                <Card className="border-2 hover:border-primary/50 transition-colors">
                  <CardContent className="p-4">
                    <div className="flex items-start gap-3">
                      <div className="h-9 w-9 rounded-lg bg-green-500/10 flex items-center justify-center flex-shrink-0">
                        <FileText className="h-4 w-4 text-green-600" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
                          Status
                        </p>
                        <StatusBadge status={selectedLote.status} variant="reembolso" />
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </div>

              <Separator className="my-6" />

              {/* Cabeçalho da Tabela */}
              <div className="mb-4">
                <h3 className="text-lg font-semibold">Colaboradores Processados</h3>
                <p className="text-sm text-muted-foreground">
                  {colaboradoresDetalhe[selectedLote.id]?.length || 0} registros encontrados
                </p>
              </div>

              {/* Tabela de Colaboradores com design moderno */}
              <Card className="border-2">
                <CardContent className="p-0">
                  <div className="rounded-lg overflow-hidden">
                    <Table>
                      <TableHeader>
                        <TableRow className="bg-muted/50 hover:bg-muted/50">
                          <TableHead className="font-semibold">Colaborador</TableHead>
                          <TableHead className="font-semibold">Cargo</TableHead>
                          <TableHead className="font-semibold">CPF</TableHead>
                          <TableHead className="font-semibold">Matrícula</TableHead>
                          <TableHead className="font-semibold">Arquivo</TableHead>
                          <TableHead className="font-semibold">Erro</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {colaboradoresDetalhe[selectedLote.id]?.map((colaborador) => (
                          <TableRow 
                            key={colaborador.id}
                            className="hover:bg-muted/30 transition-colors"
                          >
                            <TableCell className="font-medium">
                              <div className="flex items-center gap-2">
                                <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center">
                                  <span className="text-xs font-semibold text-primary">
                                    {colaborador.colaborador.split(' ').map(n => n[0]).join('').slice(0, 2)}
                                  </span>
                                </div>
                                {colaborador.colaborador}
                              </div>
                            </TableCell>
                            <TableCell>
                              <span className="text-sm">{colaborador.cargo}</span>
                            </TableCell>
                            <TableCell>
                              <span className="text-sm font-mono">{colaborador.cpf}</span>
                            </TableCell>
                            <TableCell>
                              <span className="text-sm font-mono">{colaborador.matricula}</span>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                <div className="h-7 w-7 rounded bg-muted flex items-center justify-center">
                                  <FileText className="h-3.5 w-3.5 text-muted-foreground" />
                                </div>
                                <span className="text-sm truncate max-w-[200px]" title={colaborador.arquivo}>
                                  {colaborador.arquivo}
                                </span>
                              </div>
                            </TableCell>
                            <TableCell>
                              {colaborador.erro ? (
                                <div className="flex items-center gap-2">
                                  <div className="h-2 w-2 rounded-full bg-destructive animate-pulse" />
                                  <span className="text-destructive text-sm font-medium">
                                    {colaborador.erro}
                                  </span>
                                </div>
                              ) : (
                                <div className="flex items-center gap-2">
                                  <div className="h-2 w-2 rounded-full bg-green-500" />
                                  <span className="text-muted-foreground text-sm">Sucesso</span>
                                </div>
                              )}
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                </CardContent>
              </Card>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}
