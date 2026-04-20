import { httpClient } from './httpClient'
import type {
  LoteConciliacaoMock,
  ColaboradorConciliacaoMock,
} from '../mocks/conciliacaoFolhaPagamentoMock'
import {
  lotesConciliacaoMock,
  colaboradoresConciliacaoMock,
} from '../mocks/conciliacaoFolhaPagamentoMock'

/** Item retornado por GET /api/Financeiro/Conciliacao/BuscarLotes */
export interface BuscarLotesItemDTO {
  id: string
  quantidadeItens: number
  status: string
  statusConciliacao: string
  statusCode: number
  quantidadeItensProcessados: number
  quantidadeItensProcessadosComErro: number
  cnpj: string
  empresa: string
  competencia: string
}

/** Resposta de GET /api/Financeiro/Conciliacao/BuscarLotes */
export interface BuscarLotesResponse {
  retorno: BuscarLotesItemDTO[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

/** Item retornado por GET /api/Financeiro/Conciliacao/StatusVigencia */
export interface StatusVigenciaItemDTO {
  cnpj: string
  vigencia: string
  descricao: string
  status: number
}

/** Resposta de GET /api/Financeiro/Conciliacao/StatusVigencia */
export interface StatusVigenciaResponse {
  retorno: StatusVigenciaItemDTO[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

/** Sumário do lote em GET /api/Financeiro/Conciliacao/DetalharLote */
export interface DetalharLoteSumarioDTO {
  id: string | null
  quantidadeItens: number
  status: string
  statusConciliacao: string
  statusCode: number
  quantidadeItensProcessados: number
  quantidadeItensProcessadosComErro: number
  cnpj: string
  empresa: string
  competencia: string
}

/** Divergência dentro de itensConciliacao[].resultadoConciliacao */
export interface DetalharLoteDivergenciaDTO {
  id: string
  campoDivergencia: string
  mensagem: string
  valorEsperado: string
  valorContabilidade: string
  status: string
  regra: string | null
  formula: string | null
  passos: string[]
  variaveis: string[]
  ehDivergencia: boolean
}

/** Resultado da conciliação por colaborador */
export interface DetalharLoteResultadoConciliacaoDTO {
  id: string
  houveDivergencia: boolean
  numeroDivergencias: number
  divergencias: DetalharLoteDivergenciaDTO[]
}

/** Item de itensConciliacao em DetalharLote */
export interface DetalharLoteItemDTO {
  nomeColaborador: string
  codigoInternoColaborador: string
  cargo: string
  holeritePath: string | null
  folhaPontoPath: string | null
  resultadoConciliacao: DetalharLoteResultadoConciliacaoDTO
}

/** Retorno.retorno de GET /api/Financeiro/Conciliacao/DetalharLote */
export interface DetalharLoteRetornoDTO {
  sumarioConciliacao: DetalharLoteSumarioDTO
  itensConciliacao: DetalharLoteItemDTO[]
}

/** Resposta de GET /api/Financeiro/Conciliacao/DetalharLote */
export interface DetalharLoteResponse {
  retorno: DetalharLoteRetornoDTO
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

/** Resposta de POST /api/Financeiro/Conciliacao/AprovarLote */
export interface AprovarLoteResponse {
  retorno: boolean
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

function mapBuscarLotesItemToLote(dto: BuscarLotesItemDTO): LoteConciliacaoMock {
  const ok = dto.quantidadeItensProcessados - dto.quantidadeItensProcessadosComErro
  const total = dto.quantidadeItensProcessados
  const processadoErro = total > 0 ? `${ok}/${total}` : '0/0'
  return {
    id: dto.id,
    competencia: dto.competencia,
    empresa: dto.empresa,
    cnpj: dto.cnpj,
    processadoErro,
    statusLote: dto.status,
    statusConciliacao: dto.statusConciliacao,
    total: dto.quantidadeItens,
  }
}

export class ConciliacaoFolhaPagamentoApi {
  /**
   * Busca lotes de conciliação na API (GET /api/Financeiro/Conciliacao/BuscarLotes).
   * Requer token de autenticação.
   */
  async buscarLotes(token: string): Promise<LoteConciliacaoMock[]> {
    const response = await httpClient.get<BuscarLotesResponse>(
      '/api/Financeiro/Conciliacao/BuscarLotes',
      { token }
    )
    if (!response?.sucesso || !Array.isArray(response.retorno)) {
      return []
    }
    return response.retorno.map(mapBuscarLotesItemToLote)
  }

  /**
   * Busca status de vigência da conciliação (GET /api/Financeiro/Conciliacao/StatusVigencia).
   * Requer token de autenticação.
   */
  async statusVigencia(token: string): Promise<StatusVigenciaItemDTO[]> {
    const response = await httpClient.get<StatusVigenciaResponse>(
      '/api/Financeiro/Conciliacao/StatusVigencia',
      { token }
    )
    if (!response?.sucesso || !Array.isArray(response.retorno)) {
      return []
    }
    return response.retorno
  }

  /**
   * Detalha um lote (GET /api/Financeiro/Conciliacao/DetalharLote?loteId=...).
   * Requer token de autenticação.
   */
  async detalharLote(
    token: string,
    loteId: string
  ): Promise<DetalharLoteRetornoDTO | null> {
    const response = await httpClient.get<DetalharLoteResponse>(
      `/api/Financeiro/Conciliacao/DetalharLote?loteId=${encodeURIComponent(loteId)}`,
      { token }
    )
    if (!response?.sucesso || !response?.retorno) {
      return null
    }
    return response.retorno
  }

  /**
   * Aprova um lote (POST /api/Financeiro/Conciliacao/AprovarLote).
   * Requer token de autenticação.
   */
  async aprovarLote(token: string, loteId: string): Promise<AprovarLoteResponse> {
    return httpClient.post<AprovarLoteResponse>(
      '/api/Financeiro/Conciliacao/AprovarLote',
      { loteId },
      { token }
    )
  }

  /**
   * Baixa o arquivo de divergências do lote (GET binário).
   * Nome esperado do arquivo: divergencias_{loteId}.xlsx
   */
  async downloadArquivoDivergencias(
    token: string,
    loteId: string
  ): Promise<Response> {
    return httpClient.getBlob(
      `/api/Financeiro/Conciliacao/DownloadDivergencias?loteId=${encodeURIComponent(loteId)}`,
      { token }
    )
  }

  async getLotes(): Promise<LoteConciliacaoMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...lotesConciliacaoMock]
  }

  async getColaboradores(): Promise<ColaboradorConciliacaoMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...colaboradoresConciliacaoMock]
  }
}

