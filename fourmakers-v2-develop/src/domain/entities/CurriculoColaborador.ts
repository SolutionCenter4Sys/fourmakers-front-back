/**
 * DTOs e tipos de domínio para importação de currículo/colaborador (banco de talentos).
 */

export interface ImportarColaboradorResult {
  sucesso?: boolean;
  mensagem?: string | null;
  codigoColaborador?: string;
}

/** Item retornado em ImportarColaboradorLote (ZIP). */
export interface PdfProcessadoItem {
  nomeArquivo: string;
  aProcessar: boolean;
  erro: string | null;
}

/** Retorno de sucesso da importação em lote (ZIP). */
export interface ImportarColaboradorLoteRetorno {
  pdfsProcessados: PdfProcessadoItem[];
  totalArquivos: number;
  arquivosAProcessar: number;
  arquivosComErro: number;
  idLote: string;
}

export interface ImportarColaboradorLoteResult {
  retorno?: ImportarColaboradorLoteRetorno;
  sucesso?: boolean;
  mensagem?: string | null;
}

/** Retorno de sucesso da importação em lote via planilha. */
export interface ImportarColaboradorLotePlanilhaRetorno {
  totalLinhas: number;
  linhasAProcessar: number;
  linhasComErro: number;
  idLote: string;
}

export interface ImportarColaboradorLotePlanilhaResult {
  retorno?: ImportarColaboradorLotePlanilhaRetorno;
  sucesso?: boolean;
  mensagem?: string | null;
}
