import type { ReembolsoMock, ReembolsoStatMock } from '../mocks/reembolsosMock'
import { reembolsosMock, reembolsosStatsMock } from '../mocks/reembolsosMock'
import type { ListarSolicitacoesApiResponse, ListarSolicitacoesVisaoAdmApiResponse, ListarSolicitacoesGerenteProjetoApiResponse, BuscarSolicitacaoBigNumbersApiResponse, ListarSolicitacoesAprovacaoPorColaboradorApiResponse } from '@domain/entities/SolicitacaoReembolso'
import { httpClient } from './httpClient'

export interface StatusReembolso {
  id: number
  descricao: string
}

export interface ListarStatusApiResponse {
  retorno: StatusReembolso[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface ListarSolicitacoesParams {
  dataInicial?: string
  dataFinal?: string
}

export interface ListarSolicitacoesVisaoAdmParams {
  dataInicio?: string
  dataFim?: string
  codigoProjeto?: string
  codigoCliente?: string
  statusId?: number
  aprovadorId?: string
}

export interface ListarSolicitacoesGerenteProjetoParams {
  filtro?: string
  clienteId?: string
  projetoId?: string
  dataInicio?: string
  dataFim?: string
  statusId?: number
}

export class ReembolsosApi {
  async listarSolicitacoesPorColab(
    token: string,
    params?: ListarSolicitacoesParams
  ): Promise<ListarSolicitacoesApiResponse> {
    const queryParams = new URLSearchParams()
    
    if (params?.dataInicial) {
      queryParams.append('dataInicial', params.dataInicial)
    }
    
    if (params?.dataFinal) {
      queryParams.append('dataFinal', params.dataFinal)
    }

    return httpClient.get<ListarSolicitacoesApiResponse>(
      `/api/Financeiro/Reembolso/Solicitacao/ListarPorColab${queryParams.toString() ? `?${queryParams.toString()}` : ''}`,
      { token }
    )
  }

  async getReembolsos(): Promise<ReembolsoMock[]> {
    // Simula delay de API
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...reembolsosMock]
  }

  async getReembolsosStats(): Promise<ReembolsoStatMock[]> {
    // Simula delay de API
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...reembolsosStatsMock]
  }

  async listarSolicitacoesVisaoAdm(
    token: string,
    params?: ListarSolicitacoesVisaoAdmParams
  ): Promise<ListarSolicitacoesVisaoAdmApiResponse> {
    const payload = {
      dataInicio: params?.dataInicio || '',
      dataFim: params?.dataFim || '',
      codigoProjeto: params?.codigoProjeto || '',
      codigoCliente: params?.codigoCliente || '',
      statusId: params?.statusId || 0,
      aprovadorId: params?.aprovadorId || '',
    }

    return httpClient.post<ListarSolicitacoesVisaoAdmApiResponse>(
      '/api/Financeiro/Reembolso/Solicitacao/ListarSolicitacoesVisaoAdm',
      payload,
      { token }
    )
  }

  async listarStatus(
    token: string
  ): Promise<ListarStatusApiResponse> {
    return httpClient.get<ListarStatusApiResponse>(
      '/api/Financeiro/Reembolso/Solicitacao/ListarStatus',
      { token }
    )
  }

  async gerarRelatorioSolicitacoesAguardandoPagamento(
    token: string
  ): Promise<{ arrayBuffer: ArrayBuffer; contentType: string; fileName: string }> {
    const response = await httpClient.postBlob(
      '/api/Financeiro/Reembolso/Solicitacao/GerarReleatorioSolicitacoesAguardandoPagamento',
      undefined,
      {
        token,
        headers: {
          'Accept': '*/*',
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        },
      }
    )

    // Usar Content-Type específico para arquivos Excel
    const contentType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    
    // Obter nome do arquivo do header Content-Disposition
    // Formato: attachment; filename=Exportacao_Relatorio_Pagamento_2025-12-18_14-03-05.xlsx; filename*=UTF-8''Exportacao_Relatorio_Pagamento_2025-12-18_14-03-05.xlsx
    const contentDisposition = response.headers.get('Content-Disposition') || ''
    let fileName = 'reembolso_pagamento.xlsx' // Nome padrão
    
    if (contentDisposition) {
      // Tentar primeiro o formato filename*=UTF-8'' (RFC 5987) - mais confiável
      const filenameStarMatch = contentDisposition.match(/filename\*=UTF-8''([^;]+?)(?:;|$)/i)
      if (filenameStarMatch && filenameStarMatch[1]) {
        try {
          fileName = decodeURIComponent(filenameStarMatch[1].trim())
        } catch {
          // Se falhar o decode, usar o valor direto
          fileName = filenameStarMatch[1].trim()
        }
      } else {
        // Fallback para o formato filename= simples
        const filenameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]+?)(?:;|$)/i)
        if (filenameMatch && filenameMatch[1]) {
          fileName = filenameMatch[1].replace(/['"]/g, '').trim()
        }
      }
    }

    // Retornar ArrayBuffer junto com metadados
    const arrayBuffer = await response.arrayBuffer()
    return {
      arrayBuffer,
      contentType,
      fileName,
    }
  }

  async listarSolicitacoesGerenteProjeto(
    token: string,
    params?: ListarSolicitacoesGerenteProjetoParams
  ): Promise<ListarSolicitacoesGerenteProjetoApiResponse> {
    const payload = {
      filtro: params?.filtro || '',
      clienteId: params?.clienteId || '',
      projetoId: params?.projetoId || '',
      dataInicio: params?.dataInicio || '',
      dataFim: params?.dataFim || '',
      statusId: params?.statusId || 0,
    }

    return httpClient.post<ListarSolicitacoesGerenteProjetoApiResponse>(
      '/api/Financeiro/Reembolso/Solicitacao/ListarSolicitacoesGerenteProjeto',
      payload,
      { token }
    )
  }

  async buscarSolicitacaoBigNumbers(
    token: string
  ): Promise<BuscarSolicitacaoBigNumbersApiResponse> {
    return httpClient.get<BuscarSolicitacaoBigNumbersApiResponse>(
      '/api/Financeiro/Reembolso/Solicitacao/BuscarSolicitacaoBigNumbers',
      { token }
    )
  }

  async listarSolicitacoesAprovacaoPorColaborador(
    token: string,
    codColaborador: string,
    params?: { clienteId?: string; projetoId?: string }
  ): Promise<ListarSolicitacoesAprovacaoPorColaboradorApiResponse> {
    const payload = {
      codigoColaborador: codColaborador,
      clienteId: params?.clienteId || "",
      projetoId: params?.projetoId || ""
    }
    
    return httpClient.post<ListarSolicitacoesAprovacaoPorColaboradorApiResponse>(
      '/api/Financeiro/Reembolso/Solicitacao/ListarSolicitacoesAprovacaoPorColaborador',
      payload,
      { token }
    )
  }
  
  async aprovarSolicitacoes(
    token: string,
    solicitacoesIds: number[],
    observacao: string
  ): Promise<{ sucesso: boolean; mensagem: string | null; erros: string[] | null }> {
    const payload = {
      solicitacoesIds: solicitacoesIds,
      observacao: observacao
    }
    
    return httpClient.post<{ sucesso: boolean; mensagem: string | null; erros: string[] | null }>(
      '/api/Financeiro/Reembolso/Solicitacao/AprovarSolicitacoes',
      payload,
      { token }
    )
  }
  
  async reprovarSolicitacoes(
    token: string,
    solicitacoesIds: number[],
    observacao: string
  ): Promise<{ sucesso: boolean; mensagem: string | null; erros: string[] | null }> {
    const payload = {
      solicitacoesIds: solicitacoesIds,
      observacao: observacao
    }
    
    return httpClient.post<{ sucesso: boolean; mensagem: string | null; erros: string[] | null }>(
      '/api/Financeiro/Reembolso/Solicitacao/ReprovarSolicitacoes',
      payload,
      { token }
    )
  }
  
  async gerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIds(
    token: string,
    ids: number[]
  ): Promise<{ sucesso: boolean; mensagem: string | null; erros: string[] | null }> {
    const payload = ids
    
    console.log('=== API CALL ===');
    console.log('Payload (array de IDs):', payload);
    console.log('Payload JSON:', JSON.stringify(payload));
    
    try {
      const jsonResponse = await httpClient.post<{ sucesso: boolean; mensagem: string | null; erros: string[] | null }>(
        '/api/Financeiro/Reembolso/Solicitacao/GerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIds',
        payload,
        { token }
      )
      console.log('Response JSON:', jsonResponse);
      return jsonResponse
    } catch (error) {
      console.error('Erro na resposta:', error);
      throw error
    }
  }
}

