import { useState, useMemo, useEffect } from "react";
import { useAppSelector } from "@app/store/hooks";
import { container } from "@core/di/container";
import { DiTokens } from "@core/di/tokens";
import { ConciliacaoFolhaPagamentoApi } from "@data/api/ConciliacaoFolhaPagamentoApi";
import { NotasFiscaisApi } from "@data/api/NotasFiscaisApi";
import type {
  DetalharLoteDivergenciaDTO,
  DetalharLoteItemDTO,
  DetalharLoteRetornoDTO,
} from "@data/api/ConciliacaoFolhaPagamentoApi";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import {
  CheckCircle,
  Eye,
  Calendar,
  Download,
  FileText,
  Building2,
  Hash,
  Users2,
  Check,
  AlertCircle,
  Clock,
  XCircle,
  Layers,
} from "@/components/ui/system-icons";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
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
import { Input } from "@/components/ui/input";
import { StatusBadge } from "@presentation/components/common/StatusBadge";
import { downloadBlob } from "@shared/utils/downloadUtils";
import { useToast } from "@/hooks/use-toast";
import { DataTable } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";

interface LoteConciliacao {
  /** ID numérico (mock) ou UUID (API BuscarLotes) */
  id: number | string;
  competencia: string;
  empresa: string;
  cnpj: string;
  processadoErro: string;
  statusLote: string;
  statusConciliacao: string;
  total: number;
}

interface Divergencia {
  id: number | string;
  descricao: string;
  valorContabilidade: number | string;
  valorEsperado: number | string;
  mensagem: string;
  status: "ok" | "pendente";
  formula?: string;
  passos?: string[];
  variaveis?: Record<string, string | number>;
}

interface Colaborador {
  id: number | string;
  nome: string;
  cargo: string;
  divergencias: Divergencia[];
  /** URL do holerite com placeholder $1 para o token (base64). */
  holeritePath?: string;
  /** URL da folha ponto com placeholder $1 para o token (base64). */
  folhaPontoPath?: string;
}

/** Unidade para filtro (compatível com lista vazia até API de unidades estar disponível). */
interface UnidadeFiltro {
  id: string;
  descricao: string;
}

/**
 * Substitui $1 na URL pelo token codificado (UTF-8 → Base64 → encodeURIComponent), como no FlutterFlow urlBase64.
 */
function buildDocumentUrl(url: string | null | undefined, token: string | null | undefined): string | null {
  if (!url) return null;
  if (!token) return url;
  try {
    const encodedToken = encodeURIComponent(
      btoa(new TextEncoder().encode(token).reduce((data, byte) => data + String.fromCharCode(byte), "")),
    );
    return url.replace("$1", encodedToken);
  } catch {
    return url;
  }
}

/** Formata CNPJ para exibição: XX.XXX.XXX/XXXX-XX (aceita só dígitos ou já formatado). */
function formatCnpj(value: string): string {
  const digits = String(value ?? "").replace(/\D/g, "");
  if (digits.length !== 14) return value ?? "";
  return `${digits.slice(0, 2)}.${digits.slice(2, 5)}.${digits.slice(5, 8)}/${digits.slice(8, 12)}-${digits.slice(12)}`;
}

/** Exibe valor monetário ou texto (API retorna "R$ 1.234,56" ou "Não cadastrado"). */
function formatValor(val: number | string): string {
  if (typeof val === "number") return val.toFixed(2);
  return String(val);
}

function parseVariaveis(arr: string[]): Record<string, string | number> {
  const out: Record<string, string | number> = {};
  for (const s of arr || []) {
    const i = s.indexOf(":");
    if (i < 0) continue;
    const k = s.slice(0, i).trim();
    let v: string | number = s.slice(i + 1).trim();
    if (/^\d+(\.\d+)?$/.test(v)) v = Number(v);
    out[k] = v;
  }
  return out;
}

function mapDetalharLoteToColaboradores(retorno: DetalharLoteRetornoDTO): Colaborador[] {
  return (retorno.itensConciliacao || []).map((item: DetalharLoteItemDTO) => {
    const divs = (item.resultadoConciliacao?.divergencias || []).map((d: DetalharLoteDivergenciaDTO) => ({
      id: d.id,
      descricao: d.campoDivergencia,
      valorContabilidade: d.valorContabilidade,
      valorEsperado: d.valorEsperado,
      mensagem: d.mensagem,
      status: d.status === "VERIFICADO" ? "ok" as const : "pendente" as const,
      formula: d.formula ?? undefined,
      passos: d.passos?.length ? d.passos : undefined,
      variaveis: d.variaveis?.length ? parseVariaveis(d.variaveis) : undefined,
    }));
    return {
      id: item.codigoInternoColaborador,
      nome: item.nomeColaborador ?? "",
      cargo: item.cargo ?? "",
      holeritePath: item.holeritePath ?? undefined,
      folhaPontoPath: item.folhaPontoPath ?? undefined,
      divergencias: divs,
    };
  });
}

import { useConciliacaoFolhaPagamento } from "@/hooks/useConciliacaoFolhaPagamento";

/** Preserva algarismos romanos (II, III, IV) em cargos. */
const ROMAN_NUMERAL = /^[IVXLCDM]+$/i;

/** Exibe nome/cargo em formato legível (evita caixa alta, mantém ex.: III). Aceita null/undefined. */
function formatDisplayName(value: string | null | undefined): string {
  if (value == null || typeof value !== "string") return "";
  return value
    .split(/\s+/)
    .map((w) =>
      w.length <= 4 && ROMAN_NUMERAL.test(w)
        ? w.toUpperCase()
        : w.charAt(0).toUpperCase() + w.slice(1).toLowerCase(),
    )
    .join(" ");
}

const ConciliacaoFolhaPagamento = () => {
  const { token } = useAppSelector((state) => state.auth);
  const [selectedVigencia, setSelectedVigencia] = useState("");
  const {
    lotes,
    colaboradores: mockColaboradores,
    statusVigencias,
    loading,
    recarregarLotes,
  } = useConciliacaoFolhaPagamento(token);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);
  const [selectedLote, setSelectedLote] = useState<LoteConciliacao | null>(
    null,
  );
  const [selectedColaborador, setSelectedColaborador] =
    useState<Colaborador | null>(null);
  const [buscaColaborador, setBuscaColaborador] = useState("");
  const [detalheDivergencia, setDetalheDivergencia] =
    useState<Divergencia | null>(null);
  const [unidades, setUnidades] = useState<UnidadeFiltro[]>([]);
  const [selectedUnidade, setSelectedUnidade] = useState<string>("");
  const [loadingUnidades, setLoadingUnidades] = useState(false);
  const [detalheColaboradores, setDetalheColaboradores] = useState<Colaborador[]>([]);
  const [loadingDetalhe, setLoadingDetalhe] = useState(false);
  const { toast } = useToast();

  /** Colaboradores exibidos no modal: da API (detalhe) ou mock. */
  const colaboradoresDoModal = useMemo(() => {
    if (typeof selectedLote?.id === "string" && loadingDetalhe) return [];
    return detalheColaboradores.length > 0 ? detalheColaboradores : mockColaboradores;
  }, [selectedLote?.id, loadingDetalhe, detalheColaboradores, mockColaboradores]);

  const colaboradoresFiltrados = useMemo(() => {
    if (!buscaColaborador.trim()) return colaboradoresDoModal;
    const q = buscaColaborador.trim().toLowerCase();
    return colaboradoresDoModal.filter(
      (c) =>
        formatDisplayName(c.nome).toLowerCase().includes(q) ||
        formatDisplayName(c.cargo).toLowerCase().includes(q),
    );
  }, [colaboradoresDoModal, buscaColaborador]);

  /** Carrega unidades ao selecionar vigência (GET /api/Competencia/ListarUnidadesPorOrgId). */
  useEffect(() => {
    if (!token || !selectedVigencia) {
      setUnidades([]);
      return;
    }
    let cancelled = false;
    setLoadingUnidades(true);
    const api = container.resolve(
      DiTokens.notasFiscaisApi
    ) as NotasFiscaisApi;
    api
      .listarUnidadesPorOrgId(token)
      .then((res) => {
        if (cancelled) return;
        const raw = res as { retorno?: { id: string; descricao: string }[]; ListaUnidadesResult?: { id: string; descricao: string }[] } | { id: string; descricao: string }[];
        const lista = Array.isArray(raw)
          ? raw
          : (raw?.retorno ?? raw?.ListaUnidadesResult ?? []);
        setUnidades(
          lista.map((u) => ({ id: u.id, descricao: u.descricao ?? u.id }))
        );
      })
      .catch(() => {
        if (!cancelled) setUnidades([]);
      })
      .finally(() => {
        if (!cancelled) setLoadingUnidades(false);
      });
    return () => {
      cancelled = true;
    };
  }, [token, selectedVigencia]);

  const lotesFiltradosPorVigencia = useMemo(() => {
    if (!selectedVigencia) return lotes;
    return lotes.filter((l) => l.competencia === selectedVigencia);
  }, [lotes, selectedVigencia]);

  const bigNumbers = useMemo(() => {
    const list = lotesFiltradosPorVigencia;
    return {
      total: list.length,
      processado: list.filter((l) => l.statusLote === "Processado").length,
      erro: list.filter((l) => l.statusLote === "Erro").length,
      aprovado: list.filter((l) => l.statusConciliacao === "Aprovado").length,
      pendente: list.filter((l) => l.statusConciliacao === "Pendente").length,
    };
  }, [lotesFiltradosPorVigencia]);

  /** Vigências únicas da API StatusVigencia (ordenadas MM/AAAA), ou vazio para usar lista fixa */
  const vigenciaOpcoes = useMemo(() => {
    if (!statusVigencias?.length) return [];
    const unicas = [...new Set(statusVigencias.map((v) => v.vigencia).filter(Boolean))] as string[];
    return unicas.sort((a, b) => {
      const [ma, aa] = (a ?? "").split("/").map(Number);
      const [mb, ab] = (b ?? "").split("/").map(Number);
      return aa !== ab ? ab - aa : mb - ma;
    });
  }, [statusVigencias]);

  const handleProcessarCompetencia = () => {
    if (!selectedVigencia) {
      toast({
        title: "Selecione uma vigência",
        description: "Por favor, selecione uma vigência antes de processar.",
        variant: "destructive",
      });
      return;
    }

    toast({
      title: "Processando competência",
      description: `Competência ${selectedVigencia} está sendo processada.`,
    });
  };

  const handleAprovar = (loteId: number | string) => {
    if (!token || typeof loteId !== "string") {
      toast({
        title: "Lote aprovado",
        description: "O lote foi aprovado com sucesso.",
      });
      return;
    }
    const api = container.resolve(DiTokens.conciliacaoFolhaPagamentoApi) as ConciliacaoFolhaPagamentoApi;
    api
      .aprovarLote(token, loteId)
      .then((res) => {
        toast({
          title: res.sucesso ? "Lote aprovado" : "Falha ao aprovar",
          description: res.mensagem ?? (res.sucesso ? "Lote aprovado com sucesso." : "Tente novamente."),
          variant: res.sucesso ? "default" : "destructive",
        });
        if (res.sucesso) recarregarLotes();
      })
      .catch(() => {
        toast({
          title: "Erro ao aprovar lote",
          variant: "destructive",
        });
      });
  };

  const handleVerDetalhes = (lote: LoteConciliacao) => {
    setSelectedLote(lote);
    setDetalheColaboradores([]);
    setIsDetailModalOpen(true);
    if (token && typeof lote.id === "string") {
      setLoadingDetalhe(true);
      const api = container.resolve(DiTokens.conciliacaoFolhaPagamentoApi) as ConciliacaoFolhaPagamentoApi;
      api
        .detalharLote(token, lote.id)
        .then((retorno) => {
          if (retorno) {
            const cols = mapDetalharLoteToColaboradores(retorno);
            setDetalheColaboradores(cols);
            setSelectedColaborador(cols[0] ?? null);
          } else {
            setSelectedColaborador(mockColaboradores[0] ?? null);
          }
        })
        .catch(() => {
          toast({ title: "Erro ao carregar detalhes do lote", variant: "destructive" });
          setSelectedColaborador(mockColaboradores[0] ?? null);
        })
        .finally(() => setLoadingDetalhe(false));
    } else {
      setSelectedColaborador(mockColaboradores[0] ?? null);
    }
  };

  const handleDownloadDivergencias = async () => {
    if (!selectedLote) return;
    const loteId =
      typeof selectedLote.id === "string"
        ? selectedLote.id
        : String(selectedLote.id);
    const filename = `divergencias_${loteId}.xlsx`;
    if (!token) {
      toast({
        title: "Download indisponível",
        description: "É necessário estar autenticado para baixar o arquivo.",
        variant: "destructive",
      });
      return;
    }
    try {
      const api = container.resolve(
        DiTokens.conciliacaoFolhaPagamentoApi
      ) as ConciliacaoFolhaPagamentoApi;
      const response = await api.downloadArquivoDivergencias(token, loteId);
      if (!response.ok) {
        throw new Error(response.statusText || "Falha no download");
      }
      const blob = await response.blob();
      downloadBlob(blob, filename);
      toast({
        title: "Download concluído",
        description: `Arquivo ${filename} baixado com sucesso.`,
      });
    } catch (err) {
      toast({
        title: "Erro ao baixar",
        description:
          err instanceof Error ? err.message : "Falha ao baixar o arquivo de divergências.",
        variant: "destructive",
      });
    }
  };

  const handleVerHolerite = () => {
    const path = selectedColaborador?.holeritePath;
    const finalUrl = buildDocumentUrl(path, token ?? undefined);
    if (finalUrl && (path?.includes("$1") ? !!token : true)) {
      if (finalUrl.startsWith("http")) {
        window.open(finalUrl as string, "_blank", "noopener,noreferrer");
      } else {
        toast({
          title: "Documento não disponível",
          description: "URL do holerite não configurada para este colaborador.",
          variant: "destructive",
        });
      }
    } else {
      toast({
        title: "Documento não disponível",
        description: !path
          ? "Holerite não disponível para este colaborador."
          : "É necessário estar autenticado para abrir o documento.",
        variant: "destructive",
      });
    }
  };

  const handleVerFolhaPonto = () => {
    const path = selectedColaborador?.folhaPontoPath;
    const finalUrl = buildDocumentUrl(path, token ?? undefined);
    if (finalUrl && (path?.includes("$1") ? !!token : true)) {
      if (finalUrl.startsWith("http")) {
        window.open(finalUrl as string, "_blank", "noopener,noreferrer");
      } else {
        toast({
          title: "Documento não disponível",
          description: "URL da folha ponto não configurada para este colaborador.",
          variant: "destructive",
        });
      }
    } else {
      toast({
        title: "Documento não disponível",
        description: !path
          ? "Folha ponto não disponível para este colaborador."
          : "É necessário estar autenticado para abrir o documento.",
        variant: "destructive",
      });
    }
  };

  const columns: Column[] = [
    {
      id: "competencia",
      label: "Competência",
      sortable: true,
      width: "w-[16%]",
      align: "left",
    },
    {
      id: "empresa",
      label: "Empresa",
      sortable: true,
      width: "w-[17%]",
      align: "left",
    },
    {
      id: "cnpj",
      label: "CNPJ",
      sortable: false,
      width: "w-[13%]",
      align: "left",
    },
    {
      id: "processadoErro",
      label: "Proc. / Erro",
      title: "Processado / Erro",
      sortable: false,
      width: "w-[15%]",
      align: "left",
    },
    {
      id: "statusLote",
      label: "Status do Lote",
      sortable: false,
      width: "w-[17%]",
      align: "left",
    },
    {
      id: "statusConciliacao",
      label: "Status",
      title: "Status da Conciliação",
      sortable: false,
      width: "w-[12%]",
      align: "left",
    },
    {
      id: "acoes",
      label: "Ações",
      sortable: false,
      width: "w-[12%]",
      align: "left",
    },
  ];

  const renderCell = (lote: LoteConciliacao, columnId: string) => {
    switch (columnId) {
      case "competencia":
        return (
          <span className="font-medium whitespace-nowrap">
            {lote.competencia}
          </span>
        );
      case "empresa":
        return (
          <span className="block truncate max-w-full" title={lote.empresa}>
            {lote.empresa}
          </span>
        );
      case "cnpj":
        return (
          <span
            className="block truncate max-w-full font-mono text-sm"
            title={formatCnpj(lote.cnpj)}
          >
            {formatCnpj(lote.cnpj)}
          </span>
        );
      case "processadoErro": {
        const pe = lote.processadoErro ?? "";
        return (
          <span
            className={
              "whitespace-nowrap " +
              (pe.includes("/") &&
              pe.split("/")[0] === pe.split("/")[1]
                ? "text-success font-medium"
                : "text-destructive font-medium")
            }
          >
            {pe}
          </span>
        );
      }
      case "statusLote":
        return (
          <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <span className="inline-block max-w-full min-w-0">
                  <StatusBadge status={lote.statusLote} variant="projeto" truncate />
                </span>
              </TooltipTrigger>
              <TooltipContent>{lote.statusLote}</TooltipContent>
            </Tooltip>
          </TooltipProvider>
        );
      case "statusConciliacao":
        return (
          <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <span className="inline-block max-w-full min-w-0">
                  <StatusBadge status={lote.statusConciliacao} variant="reembolso" truncate />
                </span>
              </TooltipTrigger>
              <TooltipContent>{lote.statusConciliacao}</TooltipContent>
            </Tooltip>
          </TooltipProvider>
        );
      case "acoes":
        return (
          <TooltipProvider>
            <div className="flex justify-start gap-1 flex-nowrap">
              <Tooltip>
                <TooltipTrigger asChild>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 shrink-0"
                    onClick={() => handleAprovar(lote.id)}
                    disabled={lote.statusConciliacao === "Aprovado"}
                  >
                    <CheckCircle className="h-4 w-4" />
                  </Button>
                </TooltipTrigger>
                <TooltipContent>Aprovar</TooltipContent>
              </Tooltip>
              <Tooltip>
                <TooltipTrigger asChild>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 shrink-0"
                    onClick={() => handleVerDetalhes(lote)}
                  >
                    <Eye className="h-4 w-4" />
                  </Button>
                </TooltipTrigger>
                <TooltipContent>Detalhes</TooltipContent>
              </Tooltip>
            </div>
          </TooltipProvider>
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
          <h1 className="page-title">Conciliação da Folha de Pagamento</h1>
          <p className="text-muted-foreground mt-1">
            Concilie e aprove os lotes processados da folha de pagamento
          </p>
        </div>
      </div>

      {/* Filtros de Conciliação */}
      <Card className="border border-borderSoft">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Calendar className="h-5 w-5" />
            Filtros de Conciliação
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap items-end gap-4">
            <div className="flex-1 min-w-[10rem] space-y-2">
              <label className="text-sm font-medium">Vigência</label>
              <Select
                value={selectedVigencia}
                onValueChange={(v) => {
                  setSelectedVigencia(v);
                  if (!v) setSelectedUnidade("");
                }}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione a competência" />
                </SelectTrigger>
                <SelectContent>
                  {vigenciaOpcoes.length > 0
                    ? vigenciaOpcoes.map((vig) => (
                        <SelectItem key={vig} value={vig}>
                          {vig}
                        </SelectItem>
                      ))
                    : (
                      <>
                        <SelectItem value="01/2024">Janeiro/2024</SelectItem>
                        <SelectItem value="02/2024">Fevereiro/2024</SelectItem>
                        <SelectItem value="03/2024">Março/2024</SelectItem>
                        <SelectItem value="04/2024">Abril/2024</SelectItem>
                        <SelectItem value="05/2024">Maio/2024</SelectItem>
                        <SelectItem value="06/2024">Junho/2024</SelectItem>
                        <SelectItem value="07/2024">Julho/2024</SelectItem>
                        <SelectItem value="08/2024">Agosto/2024</SelectItem>
                        <SelectItem value="09/2024">Setembro/2024</SelectItem>
                        <SelectItem value="10/2024">Outubro/2024</SelectItem>
                        <SelectItem value="11/2024">Novembro/2024</SelectItem>
                        <SelectItem value="12/2024">Dezembro/2024</SelectItem>
                      </>
                    )}
                </SelectContent>
              </Select>
            </div>
            {selectedVigencia && (
              <div className="w-48 space-y-2">
                <label className="text-sm font-medium">Unidade</label>
                <Select
                  value={selectedUnidade || "todos"}
                  onValueChange={(v) => setSelectedUnidade(v === "todos" ? "" : v)}
                  disabled={loadingUnidades}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={loadingUnidades ? "Carregando..." : "Todas"} />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="todos">Todas</SelectItem>
                    {unidades.map((u) => (
                      <SelectItem key={u.id} value={u.id}>
                        {u.descricao}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            )}
            <Button
              onClick={handleProcessarCompetencia}
              size="lg"
              disabled={!selectedVigencia}
            >
              <CheckCircle className="mr-2 h-5 w-5" />
              Processar Competência
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Big numbers — totais por status com destaque de cores (atualizam ao mudar vigência) */}
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-3">
        <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-muted-foreground/60 bg-white">
          <CardContent className="p-3 flex items-center gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-muted">
              <Layers className="h-5 w-5 text-muted-foreground" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
                Total
              </p>
              <p className="text-lg font-semibold tabular-nums text-foreground">
                {bigNumbers.total}
              </p>
            </div>
          </CardContent>
        </Card>
        <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-success bg-white">
          <CardContent className="p-3 flex items-center gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-success/15">
              <CheckCircle className="h-5 w-5 text-success" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
                Processado
              </p>
              <p className="text-lg font-semibold tabular-nums text-success">
                {bigNumbers.processado}
              </p>
            </div>
          </CardContent>
        </Card>
        <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-destructive bg-white">
          <CardContent className="p-3 flex items-center gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-destructive/15">
              <XCircle className="h-5 w-5 text-destructive" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
                Erro
              </p>
              <p className="text-lg font-semibold tabular-nums text-destructive">
                {bigNumbers.erro}
              </p>
            </div>
          </CardContent>
        </Card>
        <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-success bg-white">
          <CardContent className="p-3 flex items-center gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-success/15">
              <Check className="h-5 w-5 text-success" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
                Aprovado
              </p>
              <p className="text-lg font-semibold tabular-nums text-success">
                {bigNumbers.aprovado}
              </p>
            </div>
          </CardContent>
        </Card>
        <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-warning bg-white">
          <CardContent className="p-3 flex items-center gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-warning/15">
              <Clock className="h-5 w-5 text-warning" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
                Pendente
              </p>
              <p className="text-lg font-semibold tabular-nums text-warning">
                {bigNumbers.pendente}
              </p>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Lotes Processados */}
      <Card className="border border-borderSoft">
        <CardHeader>
          <CardTitle>Lotes Processados</CardTitle>
          {selectedVigencia && (
            <p className="text-sm text-muted-foreground font-normal mt-1">
              Vigência: {selectedVigencia} — {lotesFiltradosPorVigencia.length}{" "}
              lote(s)
            </p>
          )}
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="text-center py-8 text-muted-foreground">
              Carregando lotes...
            </div>
          ) : (
            <div className="w-full overflow-hidden [&_table]:table-fixed [&_table]:w-full [&_th]:px-2 [&_td]:px-2">
              <DataTable
                columns={columns}
                data={lotesFiltradosPorVigencia}
                keyExtractor={(item) => item.id.toString()}
                renderCell={renderCell}
                getCellClassName={(id) =>
                  ["empresa", "cnpj", "statusLote", "statusConciliacao"].includes(id) ? "min-w-0" : ""
                }
                emptyMessage={
                  selectedVigencia
                    ? "Nenhum lote para esta vigência"
                    : "Selecione uma vigência ou veja todos os lotes"
                }
              />
            </div>
          )}
        </CardContent>
      </Card>

      {/* Modal de Detalhes do Lote */}
      <Dialog
        open={isDetailModalOpen}
        onOpenChange={(open) => {
          if (!open) setDetalheColaboradores([]);
          setIsDetailModalOpen(open);
        }}
      >
        <DialogContent className="max-w-[98vw] max-h-[98vh] w-[98vw] h-[98vh] p-0 gap-0 overflow-hidden">
          {/* Header */}
          <div className="p-6 pb-4 border-b border-borderSoft">
            <DialogHeader>
              <div className="flex items-center justify-between mb-6">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-primary/10 rounded-lg">
                    <FileText className="h-6 w-6 text-primary" />
                  </div>
                  <div>
                    <DialogTitle className="text-2xl font-bold">
                      Detalhes do Lote
                    </DialogTitle>
                    <DialogDescription className="mt-1">
                      Selecione um colaborador para visualizar as divergências
                    </DialogDescription>
                  </div>
                </div>
              </div>

              {/* Resumo do lote — estilo PDI: cada card com detalhe em cor (borda + fundo + ícone) */}
              {selectedLote && (
                <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3">
                  <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-info bg-white">
                    <CardContent className="p-3 flex items-center gap-3">
                      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-muted">
                        <Building2 className="h-5 w-5 text-muted-foreground" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
                          Empresa
                        </p>
                        <p className="truncate text-base font-bold text-foreground mt-0.5">
                          {selectedLote.empresa}
                        </p>
                      </div>
                    </CardContent>
                  </Card>
                  <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-accent bg-white">
                    <CardContent className="p-3 flex items-center gap-3">
                      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-muted">
                        <Hash className="h-5 w-5 text-muted-foreground" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
                          CNPJ
                        </p>
                        <TooltipProvider>
                          <Tooltip>
                            <TooltipTrigger asChild>
                              <p className="truncate font-mono text-base font-bold text-foreground cursor-default mt-0.5">
                                {formatCnpj(selectedLote.cnpj)}
                              </p>
                            </TooltipTrigger>
                            <TooltipContent>{formatCnpj(selectedLote.cnpj)}</TooltipContent>
                          </Tooltip>
                        </TooltipProvider>
                      </div>
                    </CardContent>
                  </Card>
                  <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-success bg-white">
                    <CardContent className="p-3 flex items-center gap-3">
                      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-muted">
                        <Calendar className="h-5 w-5 text-muted-foreground" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
                          Competência
                        </p>
                        <p className="text-base font-bold text-foreground mt-0.5">
                          {selectedLote.competencia}
                        </p>
                      </div>
                    </CardContent>
                  </Card>
                  <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-muted-foreground/60 bg-white">
                    <CardContent className="p-3 flex items-center gap-3">
                      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-muted">
                        <Users2 className="h-5 w-5 text-muted-foreground" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
                          # Total
                        </p>
                        <p className="text-lg font-bold tabular-nums text-foreground mt-0.5">
                          {selectedLote.total}
                        </p>
                      </div>
                    </CardContent>
                  </Card>
                  <Card
                    className={`overflow-hidden rounded-lg border border-borderSoft border-l-4 bg-white ${
                      selectedLote.statusLote === "Processado" || selectedLote.statusLote === "Aprovado"
                        ? "border-l-success"
                        : "border-l-warning"
                    }`}
                  >
                    <CardContent className="p-3 flex items-center gap-3">
                      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-muted">
                        {selectedLote.statusLote === "Processado" || selectedLote.statusLote === "Aprovado" ? (
                          <Check className="h-5 w-5 text-success" />
                        ) : (
                          <AlertCircle className="h-5 w-5 text-muted-foreground" />
                        )}
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
                          Status
                        </p>
                        <div className="mt-0.5">
                        <StatusBadge
                          status={selectedLote.statusLote}
                          variant="projeto"
                        />
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                  <Card className="overflow-hidden rounded-lg border border-borderSoft border-l-4 border-l-destructive bg-white">
                    <CardContent className="p-3 flex items-center gap-3">
                      <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-muted">
                        <Download className="h-5 w-5 text-muted-foreground" />
                      </div>
                      <div className="min-w-0 flex-1">
                        <p className="text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
                          Arquivo
                        </p>
                        <Button
                          variant="link"
                          size="sm"
                          className="h-auto p-0 font-semibold text-destructive hover:text-destructive/90"
                          onClick={handleDownloadDivergencias}
                        >
                          Download
                        </Button>
                      </div>
                    </CardContent>
                  </Card>
                </div>
              )}
            </DialogHeader>
          </div>

          {/* Content - Two Columns Layout */}
          <div className="flex flex-1 overflow-hidden">
            {/* Left Column - Colaboradores List */}
            <div className="w-80 border-r border-borderSoft bg-white">
              <div className="p-4 border-b border-borderSoft bg-muted/20">
                <div className="flex items-center gap-2 mb-3">
                  <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-primary/10">
                    <Users2 className="h-5 w-5 text-primary" />
                  </div>
                  <h3 className="font-bold text-foreground tracking-tight">Colaboradores</h3>
                </div>
                <Input
                  placeholder="Colaborador / Cargo"
                  value={buscaColaborador}
                  onChange={(e) => setBuscaColaborador(e.target.value)}
                  className="rounded-lg"
                />
              </div>
              <ScrollArea className="h-[calc(98vh-380px)]">
                <div className="space-y-1 p-2">
                  {loadingDetalhe ? (
                    <div className="flex items-center justify-center py-8 text-muted-foreground text-sm">
                      Carregando colaboradores...
                    </div>
                  ) : (
                  colaboradoresFiltrados.map((colaborador) => {
                    const isActive = selectedColaborador?.id === colaborador.id;
                    return (
                      <button
                        key={colaborador.id}
                        type="button"
                        onClick={() => setSelectedColaborador(colaborador)}
                        className={`flex w-full items-start gap-2 rounded-lg border-l-4 px-3 py-3 text-left transition-[background-color,border-color,box-shadow] duration-200 ${
                          isActive
                            ? "border-l-primary bg-primary text-primary-foreground shadow-sm"
                            : "border-l-transparent bg-card hover:bg-muted/70"
                        }`}
                      >
                        <div className="flex-1 min-w-0">
                          <TooltipProvider>
                            <Tooltip>
                              <TooltipTrigger asChild>
                                <p
                                  className={`truncate text-sm font-medium ${isActive ? "text-primary-foreground" : "text-foreground"}`}
                                >
                                  {formatDisplayName(colaborador.nome)}
                                </p>
                              </TooltipTrigger>
                              <TooltipContent>{formatDisplayName(colaborador.nome)}</TooltipContent>
                            </Tooltip>
                          </TooltipProvider>
                          <TooltipProvider>
                            <Tooltip>
                              <TooltipTrigger asChild>
                                <p
                                  className={`truncate text-xs ${isActive ? "text-primary-foreground/85" : "text-muted-foreground"}`}
                                >
                                  {formatDisplayName(colaborador.cargo)}
                                </p>
                              </TooltipTrigger>
                              <TooltipContent className="max-w-xs">{formatDisplayName(colaborador.cargo)}</TooltipContent>
                            </Tooltip>
                          </TooltipProvider>
                        </div>
                        {colaborador.divergencias.length > 0 ? (
                          <Badge
                            variant={isActive ? "secondary" : "destructive"}
                            className={`shrink-0 ${isActive ? "border border-primary-foreground/30 bg-primary-foreground/15 text-primary-foreground" : ""}`}
                          >
                            {colaborador.divergencias.length}
                          </Badge>
                        ) : (
                          <div
                            className={`shrink-0 rounded-full p-1.5 ${isActive ? "bg-primary-foreground/20" : "bg-success/10"}`}
                          >
                            <Check
                              className={`h-3.5 w-3.5 ${isActive ? "text-primary-foreground" : "text-success"}`}
                            />
                          </div>
                        )}
                      </button>
                    );
                  })
                  )}
                </div>
              </ScrollArea>
            </div>

            {/* Right Column - Divergências Details */}
            <div className="flex-1 flex flex-col overflow-hidden">
              {selectedColaborador && (
                <>
                  {/* Cabeçalho: título + colunas Colaborador | Competência + botões */}
                  <div className="border-b border-borderSoft bg-muted/25 px-6 py-3 space-y-2">
                    <div className="flex items-center justify-between gap-4">
                      <div className="flex items-center gap-2">
                        <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-destructive/10">
                          <AlertCircle className="h-5 w-5 text-destructive" />
                        </div>
                        <h3 className="text-lg font-bold text-foreground tracking-tight">
                          Divergências
                        </h3>
                      </div>
                      <div className="flex gap-2 shrink-0">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={handleVerHolerite}
                        >
                          <FileText className="h-4 w-4 mr-2" />
                          Holerite
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={handleVerFolhaPonto}
                        >
                          <Calendar className="h-4 w-4 mr-2" />
                          Folha Ponto
                        </Button>
                      </div>
                    </div>
                    <div className="rounded-lg border border-borderSoft bg-muted/30 px-3 py-2">
                      <dl className="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-1 text-sm">
                        <div className="flex items-baseline gap-1.5 min-w-0">
                          <dt className="text-muted-foreground shrink-0 text-xs font-medium uppercase tracking-wider">
                            Colaborador / Cargo
                          </dt>
                          <dd className="font-semibold text-foreground truncate">
                            {formatDisplayName(selectedColaborador.nome)} ·{" "}
                            {formatDisplayName(selectedColaborador.cargo)}
                          </dd>
                        </div>
                        <div className="flex items-baseline gap-1.5">
                          <dt className="text-muted-foreground shrink-0 text-xs font-medium uppercase tracking-wider">
                            Competência
                          </dt>
                          <dd className="font-semibold text-foreground tabular-nums">
                            {selectedLote?.competencia}
                          </dd>
                        </div>
                      </dl>
                    </div>
                  </div>

                  {/* Tabela de Divergências */}
                  <ScrollArea className="flex-1 p-6">
                    {selectedColaborador.divergencias.length > 0 ? (
                      <div className="overflow-hidden rounded-lg border border-borderSoft bg-surfaceElevated shadow-sm">
                        <Table>
                          <TableHeader>
                            <TableRow className="border-borderSoft border-b-2 border-border bg-muted hover:bg-muted">
                              <TableHead className="h-11 px-2 font-semibold text-foreground">
                                Descrição
                              </TableHead>
                              <TableHead className="h-11 px-2 text-right font-semibold text-foreground">
                                Valor Contabilidade
                              </TableHead>
                              <TableHead className="h-11 px-2 text-right font-semibold text-foreground">
                                Valor Esperado
                              </TableHead>
                              <TableHead className="h-11 px-2 font-semibold text-foreground">
                                Mensagem
                              </TableHead>
                              <TableHead className="h-11 px-2 text-center font-semibold text-foreground">
                                Ação
                              </TableHead>
                              <TableHead className="h-11 w-14 px-2 text-center font-semibold text-foreground">
                                Status
                              </TableHead>
                            </TableRow>
                          </TableHeader>
                          <TableBody>
                            {selectedColaborador.divergencias.map(
                              (divergencia) => (
                                <TableRow
                                  key={divergencia.id}
                                  className={`border-borderSoft transition-colors hover:bg-muted/50 ${
                                    divergencia.status === "ok"
                                      ? "border-l-4 border-l-success bg-success/5 even:bg-success/[0.07]"
                                      : "border-l-4 border-l-destructive/70 bg-destructive/5 even:bg-destructive/[0.07]"
                                  }`}
                                >
                                  <TableCell className="px-2 py-3 font-medium text-foreground max-w-[200px]">
                                    <TooltipProvider>
                                      <Tooltip>
                                        <TooltipTrigger asChild>
                                          <span className="block truncate cursor-default" title={divergencia.descricao}>
                                            {divergencia.descricao}
                                          </span>
                                        </TooltipTrigger>
                                        <TooltipContent side="top" className="max-w-sm">
                                          {divergencia.descricao}
                                        </TooltipContent>
                                      </Tooltip>
                                    </TooltipProvider>
                                  </TableCell>
                                  <TableCell className="px-2 py-3 text-right font-mono text-sm tabular-nums">
                                    {formatValor(divergencia.valorContabilidade)}
                                  </TableCell>
                                  <TableCell className="px-2 py-3 text-right font-mono text-sm tabular-nums">
                                    {formatValor(divergencia.valorEsperado)}
                                  </TableCell>
                                  <TableCell className="px-2 py-3 text-sm text-muted-foreground max-w-[240px]">
                                    <TooltipProvider>
                                      <Tooltip>
                                        <TooltipTrigger asChild>
                                          <span className="block truncate cursor-default" title={divergencia.mensagem}>
                                            {divergencia.mensagem}
                                          </span>
                                        </TooltipTrigger>
                                        <TooltipContent side="top" className="max-w-md">
                                          {divergencia.mensagem}
                                        </TooltipContent>
                                      </Tooltip>
                                    </TooltipProvider>
                                  </TableCell>
                                  <TableCell className="px-2 py-3 text-center">
                                    <Button
                                      variant="link"
                                      size="sm"
                                      className="text-primary hover:text-primary/90"
                                      onClick={() =>
                                        setDetalheDivergencia(divergencia)
                                      }
                                    >
                                      + info
                                    </Button>
                                  </TableCell>
                                  <TableCell className="px-2 py-3 text-center">
                                    {divergencia.status === "ok" && (
                                      <span className="inline-flex items-center justify-center rounded-md bg-success/15 px-1.5 py-0.5">
                                        <Check className="h-5 w-5 text-success" />
                                      </span>
                                    )}
                                    {divergencia.status === "pendente" && (
                                      <span className="inline-flex items-center justify-center rounded-md bg-destructive/15 px-1.5 py-0.5">
                                        <AlertCircle className="h-5 w-5 text-destructive" />
                                      </span>
                                    )}
                                  </TableCell>
                                </TableRow>
                              ),
                            )}
                          </TableBody>
                        </Table>
                      </div>
                    ) : (
                      <Card className="p-12 bg-surfaceElevated border-borderSoft">
                        <div className="flex flex-col items-center justify-center text-center">
                          <div className="p-4 bg-success/10 rounded-full mb-4">
                            <Check className="h-12 w-12 text-success" />
                          </div>
                          <h3 className="text-xl font-semibold mb-2">
                            Nenhuma Divergência
                          </h3>
                          <p className="text-muted-foreground max-w-md">
                            Este colaborador não possui divergências registradas
                            para esta competência.
                          </p>
                        </div>
                      </Card>
                    )}
                  </ScrollArea>
                </>
              )}
            </div>
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal Mais detalhes da divergência (+ info) */}
      <Dialog
        open={!!detalheDivergencia}
        onOpenChange={(open) => !open && setDetalheDivergencia(null)}
      >
        <DialogContent className="max-w-lg max-h-[90vh] overflow-hidden flex flex-col">
          <DialogHeader>
            <DialogTitle className="text-lg">
              Mais detalhes: Rubrica: {detalheDivergencia?.descricao}
            </DialogTitle>
          </DialogHeader>
          {detalheDivergencia && (
            <ScrollArea className="flex-1 pr-4 -mr-4">
              <div className="space-y-5 text-sm">
                <div>
                  <h4 className="font-semibold text-foreground mb-1">
                    Mensagem
                  </h4>
                  <p className="text-muted-foreground">
                    {detalheDivergencia.mensagem}
                  </p>
                </div>
                <div>
                  <h4 className="font-semibold text-foreground mb-1">
                    Valor Contabilidade
                  </h4>
                  <p className="text-muted-foreground">
                    {typeof detalheDivergencia.valorContabilidade === "number"
                      ? `R$ ${detalheDivergencia.valorContabilidade.toFixed(2)}`
                      : detalheDivergencia.valorContabilidade}{" "}
                    <span className="text-foreground">
                      Valor Esperado:{" "}
                      {typeof detalheDivergencia.valorEsperado === "number"
                        ? `R$ ${detalheDivergencia.valorEsperado.toFixed(2)}`
                        : detalheDivergencia.valorEsperado}
                    </span>
                  </p>
                </div>
                {detalheDivergencia.formula && (
                  <div>
                    <h4 className="font-semibold text-foreground mb-1">
                      Fórmula
                    </h4>
                    <p className="text-muted-foreground">
                      {detalheDivergencia.formula}
                    </p>
                  </div>
                )}
                {detalheDivergencia.passos &&
                  detalheDivergencia.passos.length > 0 && (
                    <div>
                      <h4 className="font-semibold text-foreground mb-1">
                        Passos
                      </h4>
                      <ul className="list-disc list-inside space-y-1 text-muted-foreground">
                        {detalheDivergencia.passos.map((passo, i) => (
                          <li key={i}>{passo}</li>
                        ))}
                      </ul>
                    </div>
                  )}
                {detalheDivergencia.variaveis &&
                  Object.keys(detalheDivergencia.variaveis).length > 0 && (
                    <div>
                      <h4 className="font-semibold text-foreground mb-1">
                        Variáveis
                      </h4>
                      <ul className="space-y-1 text-muted-foreground font-mono text-xs">
                        {Object.entries(detalheDivergencia.variaveis).map(
                          ([key, value]) => (
                            <li key={key}>
                              {key}: {String(value)}
                            </li>
                          ),
                        )}
                      </ul>
                    </div>
                  )}
              </div>
            </ScrollArea>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default ConciliacaoFolhaPagamento;
