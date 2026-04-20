export interface Holerite {
  tbItemLoteId: string;
  codigoInternoColaborador: string;
  competencia: string; // formato "MM/YYYY"
  cnpj: string;
  totalHorasExtras: string | null;
  diasTrabalhados: number;
  assinado: boolean;
  assinadoEm: string | null;
  holeritePdf: string;
  adiantamento: boolean;
  ferias: boolean;
  decimoTerceiro: boolean;
  decimoTerceiroAdiantamento: boolean;
  /** Quando true, é informe de rendimentos: apenas visualização, sem assinatura. */
  informeDeRendimentos?: boolean;
}

export interface ListarHoleritesColaboradorPorAnoParams {
  codigoInternoColaborador: string;
  ano: number;
}

export interface ListarHoleritesColaboradorPorAnoResponse {
  retorno: Holerite[];
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

export interface AssinarHoleritePorLoteIdParams {
  itemLoteId: string;
}

export interface AssinarHoleritePorLoteIdResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
}

