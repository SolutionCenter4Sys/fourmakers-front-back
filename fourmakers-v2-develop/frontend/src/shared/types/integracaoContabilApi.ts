/**
 * Tipos para a API de Competência/Remessa (backoffice-rf).
 * Usados na aba Remessa da página Integração Contábil.
 */

export interface UnidadeRemessaDTO {
  id: string;
  descricao: string;
}

export interface ListarUnidadesRemessaResponse {
  ListaUnidadesResult: UnidadeRemessaDTO[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

/** Retorno do arquivo quando a API devolve JSON com arquivo em base64 (GerarRemessaContabilMensal). */
export interface GerarRemessaRetornoDTO {
  FileContents?: string;
  ContentType?: string;
  FileDownloadName?: string;
  fileContents?: string;
  contentType?: string;
  fileDownloadName?: string;
}

/** Resposta do GET GerarRemessaContabilMensal (Financeiro.API). */
export interface GerarRemessaContabilMensalResponse {
  Sucesso?: boolean;
  Mensagem?: string | null;
  Retorno?: GerarRemessaRetornoDTO | null;
  sucesso?: boolean;
  mensagem?: string | null;
  retorno?: GerarRemessaRetornoDTO | null;
}

/** Sumário do holerite no lote (ListarLotes). */
export interface SumarioHoleriteLoteDTO {
  cnpj: string;
  empresa: string;
  competencia: string | null;
  adiantamento: boolean;
  ferias: boolean;
  decimoTerceiro: boolean;
  decimoTerceiroAdiantamento: boolean;
}

/** Lote de holerite retornado por ListarLotes (aba Retorno Integração Contábil). */
export interface LoteHoleriteDTO {
  id: string;
  status: string;
  sumarioHolerite: SumarioHoleriteLoteDTO;
  quantidadeDePaginas: number;
  pdf: string;
  dataCriacao: string;
  dataFinalizacao: string | null;
  aprovadoParaProcessamento: boolean;
}

/** Resposta do GET api/Financeiro/Holerite/ListarLotes. */
export interface ListarLotesHoleriteResponse {
  retorno: LoteHoleriteDTO[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

/** Lote retornado por DetalharLote (dados do lote). */
export interface LoteDetalheDTO {
  id: string;
  quantidadeItens: number;
  quantidadeItensProcessados: number;
  quantidadeItensProcessadosComErro: number;
  cnpj: string;
  empresa: string;
  competencia: string;
  adiantamento: boolean;
  ferias: boolean;
  decimoTerceiro: boolean;
  decimoTerceiroAdiantamento: boolean;
}

/** Item (colaborador) do lote retornado por DetalharLote. */
export interface ItemLoteDTO {
  cpf: string;
  nome: string;
  cargo: string;
  matricula: string;
  erro: string | null;
  id: string;
  dataCriacao: string;
  dataFinalizacao: string | null;
  filePath: string;
  sucesso: boolean;
}

/** Retorno do GET DetalharLote (lote + itens). */
export interface DetalharLoteRetornoDTO {
  lote: LoteDetalheDTO;
  itens: ItemLoteDTO[];
}

/** Resposta do GET api/Financeiro/Holerite/DetalharLote. */
export interface DetalharLoteResponse {
  retorno: DetalharLoteRetornoDTO;
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

/** Resposta do POST api/Financeiro/Holerite/ProcessarHolerite. */
export interface ProcessarHoleriteResponse {
  Sucesso?: boolean;
  Mensagem?: string | null;
  sucesso?: boolean;
  mensagem?: string | null;
}

/** Retorno do POST SumarioHolerite (lote criado com sumário extraído do PDF). */
export interface SumarioHoleriteResultDTO {
  id?: string;
  cnpj?: string;
  empresa?: string;
  competencia?: string | null;
  quantidadeItens?: number;
  quantidadeItensProcessados?: number;
  quantidadeItensProcessadosComErro?: number;
}

/** Resposta do POST api/Financeiro/Holerite/SumarioHolerite. */
export interface SumarioHoleriteResponse {
  Sucesso?: boolean;
  Mensagem?: string | null;
  Retorno?: SumarioHoleriteResultDTO | null;
  sucesso?: boolean;
  mensagem?: string | null;
  retorno?: SumarioHoleriteResultDTO | null;
}

/** Opções de tipo de processamento para o modal Processar Lote (enum backend 0–5). */
export const TIPOS_PROCESSAMENTO_HOLERITE = [
  { value: 0, label: "Mensal" },
  { value: 1, label: "Adiantamento" },
  { value: 2, label: "Férias" },
  { value: 3, label: "Quitação do 13º salário" },
  { value: 4, label: "Adiantamento do 13º salário" },
  { value: 5, label: "Informe de Rendimentos" },
] as const;
