import type {
  RetornoContabilMock,
  ColaboradorRetornoMock,
} from '../mocks/integracaoContabilMock'
import type {
  GerarRemessaContabilMensalResponse,
  LoteHoleriteDTO,
  ListarLotesHoleriteResponse,
  DetalharLoteRetornoDTO,
  DetalharLoteResponse,
  ProcessarHoleriteResponse,
  SumarioHoleriteResponse,
  SumarioHoleriteResultDTO,
} from '@shared/types/integracaoContabilApi'
import {
  retornosContabilMock,
  colaboradoresRetornoMock,
} from '../mocks/integracaoContabilMock'
import { httpClient } from './httpClient'

export interface GerarRemessaResult {
  blob: Blob
  fileName: string
}

export class IntegracaoContabilApi {
  async getRetornos(): Promise<RetornoContabilMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...retornosContabilMock]
  }

  async getColaboradores(): Promise<ColaboradorRetornoMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...colaboradoresRetornoMock]
  }

  /**
   * Gera remessa contábil mensal (arquivo para envio ao sistema contábil).
   * GET api/Financeiro/IntegracaoContabil/GerarRemessaContabilMensal?cnpj=&competencia=
   * Resposta JSON com arquivo em base64 no Retorno (FileContents, FileDownloadName, ContentType).
   */
  async gerarRemessaContabilMensal(
    token: string,
    cnpj: string,
    competencia: string
  ): Promise<GerarRemessaResult> {
    const url = `/api/Financeiro/IntegracaoContabil/GerarRemessaContabilMensal?cnpj=${encodeURIComponent(cnpj)}&competencia=${encodeURIComponent(competencia)}`
    const response = await httpClient.get<GerarRemessaContabilMensalResponse>(url, { token })

    const sucesso = response?.Sucesso ?? response?.sucesso ?? false
    const mensagem = response?.Mensagem ?? response?.mensagem ?? null
    const retorno = response?.Retorno ?? response?.retorno

    if (!sucesso) {
      throw new Error(mensagem ?? 'Falha ao gerar remessa contábil.')
    }

    const base64 = retorno?.FileContents ?? retorno?.fileContents
    const fileName = retorno?.FileDownloadName ?? retorno?.fileDownloadName ?? 'remessa_contabil.xlsx'
    const contentType = retorno?.ContentType ?? retorno?.contentType ?? 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'

    if (!base64) {
      throw new Error('Resposta da API sem conteúdo do arquivo.')
    }

    const binary = atob(base64)
    const bytes = new Uint8Array(binary.length)
    for (let i = 0; i < binary.length; i++) {
      bytes[i] = binary.charCodeAt(i)
    }
    const blob = new Blob([bytes], { type: contentType })

    return { blob, fileName }
  }

  /**
   * Lista lotes de holerite (aba Retorno – Integração Contábil).
   * GET api/Financeiro/Holerite/ListarLotes
   */
  async listarLotesHolerite(token: string): Promise<LoteHoleriteDTO[]> {
    const response = await httpClient.get<ListarLotesHoleriteResponse>(
      '/api/Financeiro/Holerite/ListarLotes',
      { token }
    )
    if (!response?.sucesso || !Array.isArray(response.retorno)) {
      return []
    }
    return response.retorno
  }

  /**
   * Detalha um lote de holerite (itens por colaborador).
   * GET api/Financeiro/Holerite/DetalharLote?loteId=...
   */
  async detalharLote(token: string, loteId: string): Promise<DetalharLoteRetornoDTO> {
    const url = `/api/Financeiro/Holerite/DetalharLote?loteId=${encodeURIComponent(loteId)}`
    const response = await httpClient.get<DetalharLoteResponse>(url, { token })
    if (!response?.sucesso || !response.retorno) {
      throw new Error(response?.mensagem ?? 'Falha ao carregar detalhes do lote.')
    }
    return response.retorno
  }

  /**
   * Envia PDF de holerite e obtém sumário (cria lote no backend).
   * POST api/Financeiro/Holerite/SumarioHolerite (multipart/form-data: arquivoPdf)
   */
  async sumarioHolerite(token: string, arquivoPdf: File): Promise<SumarioHoleriteResultDTO> {
    const url = '/api/Financeiro/Holerite/SumarioHolerite'
    const formData = new FormData()
    formData.append('arquivoPdf', arquivoPdf)
    const response = await httpClient.post<SumarioHoleriteResponse>(url, formData, { token })
    const sucesso = response?.Sucesso ?? response?.sucesso ?? false
    const retorno = response?.Retorno ?? response?.retorno
    const mensagem = response?.Mensagem ?? response?.mensagem ?? null
    if (!sucesso || !retorno) {
      throw new Error(mensagem ?? 'Falha ao processar o PDF do holerite.')
    }
    return retorno
  }

  /**
   * Processa um lote de holerite (tipo: Mensal, Adiantamento, Férias, etc.).
   * POST api/Financeiro/Holerite/ProcessarHolerite?loteId=...&tipoProcessamento=0..5
   */
  async processarHolerite(
    token: string,
    loteId: string,
    tipoProcessamento: number
  ): Promise<void> {
    const url = `/api/Financeiro/Holerite/ProcessarHolerite?loteId=${encodeURIComponent(loteId)}&tipoProcessamento=${tipoProcessamento}`
    const response = await httpClient.post<ProcessarHoleriteResponse>(url, undefined, { token })
    const sucesso = response?.Sucesso ?? response?.sucesso ?? false
    const mensagem = response?.Mensagem ?? response?.mensagem ?? null
    if (!sucesso) {
      throw new Error(mensagem ?? 'Falha ao processar lote.')
    }
  }

  /**
   * Exclui um lote de holerite.
   * POST api/Financeiro/Holerite/DeletarLote?loteId=...
   */
  async deletarLote(token: string, loteId: string): Promise<void> {
    const url = `/api/Financeiro/Holerite/DeletarLote?loteId=${encodeURIComponent(loteId)}`
    const response = await httpClient.post<ProcessarHoleriteResponse>(url, undefined, { token })
    const sucesso = response?.Sucesso ?? response?.sucesso ?? false
    const mensagem = response?.Mensagem ?? response?.mensagem ?? null
    if (!sucesso) {
      throw new Error(mensagem ?? 'Falha ao excluir lote.')
    }
  }
}

