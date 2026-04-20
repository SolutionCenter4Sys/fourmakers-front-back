export interface VerbaData {
  id?: string;
  isActive: boolean;
  tipoCusto: "debito" | "credito" | "fixa" | "variavel";
  nomeCategoria: string;
  unidade: string;
  valor: number;
  isCustoCliente: boolean;
}

export interface ReembolsoItem {
  id: number;
  clienteProjeto: string;
  tipo: string;
  valorSolicitado: string;
  valorAprovado: string;
  dataSolicitacao: string;
  dataAprovacao: string;
  status: string;
  comprovantes?: number;
  observacoes?: string;
}

export interface LogEntry {
  regra: string;
  acao: string;
  de: string;
  para: string;
  alteradoEm: string;
  alteradoPor: string;
}
