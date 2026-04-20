import type {
  NotaFiscalMock,
  NotaFiscalGestaoMock,
  ColaboradorOptionMock,
  UnidadeOptionMock,
} from '../mocks/notasFiscaisMock'
import {
  notasFiscaisMock,
  notasFiscaisGestaoMock,
  colaboradoresOptionMock,
  unidadesOptionMock,
} from '../mocks/notasFiscaisMock'
import { httpClient } from './httpClient'
import type {
  ListarNotaFiscalStatusResponse,
  ListarUnidadesResponse,
  ListarColaboradoresETbdsResponse,
  ListarColaboradoresETbdsParams,
  ListarNotasFiscaisPorVigenciaVisaoGestorParams,
  ListarNotasFiscaisPorVigenciaVisaoGestorResponse,
  AprovarNotasFiscaisParams,
  AprovarNotasFiscaisResponse,
  ReprovarNotasFiscaisParams,
  ReprovarNotasFiscaisResponse,
  LiberarEmissaoNotasFiscaisPorVigenciaParams,
  LiberarEmissaoNotasFiscaisPorVigenciaResponse,
  ListarRubricasColaboradorParaLiberacaoDeNfParams,
  ListarRubricasColaboradorParaLiberacaoDeNfResponse,
  InserirNotaFiscalParams,
  InserirNotaFiscalResponse,
} from '@domain/entities/NotaFiscalGestao'

export class NotasFiscaisApi {
  // Métodos mockados (mantidos para compatibilidade)
  async getNotasFiscais(): Promise<NotaFiscalMock[]> {
    // Simula delay de API
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...notasFiscaisMock]
  }

  async getNotasFiscaisGestao(): Promise<NotaFiscalGestaoMock[]> {
    // Simula delay de API
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...notasFiscaisGestaoMock]
  }

  // Métodos reais usando httpClient
  /**
   * Lista os status disponíveis para notas fiscais
   */
  async listarNotaFiscalStatus(token: string): Promise<ListarNotaFiscalStatusResponse> {
    return httpClient.get<ListarNotaFiscalStatusResponse>(
      '/api/Financeiro/NotaFiscal/ListarNotaFiscalStatus',
      { token }
    )
  }

  /**
   * Lista as unidades por organização
   */
  async listarUnidadesPorOrgId(token: string): Promise<ListarUnidadesResponse> {
    return httpClient.get<ListarUnidadesResponse>(
      '/api/Competencia/ListarUnidadesPorOrgId',
      { token }
    )
  }

  /**
   * Lista colaboradores e TBDs para filtros
   */
  async listarColaboradoresETbds(
    token: string,
    params?: ListarColaboradoresETbdsParams
  ): Promise<ListarColaboradoresETbdsResponse> {
    const queryParams = new URLSearchParams()
    
    if (params?.codigoDiretoria !== undefined) {
      queryParams.append('codigoDiretoria', params.codigoDiretoria.toString())
    }
    if (params?.codigoGestor !== undefined) {
      queryParams.append('codigoGestor', params.codigoGestor.toString())
    }
    if (params?.filtroTipoProfissional !== undefined) {
      queryParams.append('filtroTipoProfissional', params.filtroTipoProfissional.toString())
    }
    if (params?.codigoDepartamento) {
      queryParams.append('codigoDepartamento', params.codigoDepartamento)
    }

    const queryString = queryParams.toString()
    const url = `/api/MapaDeAlocacao/ListarColaboradoresETbds${queryString ? `?${queryString}` : ''}`

    return httpClient.get<ListarColaboradoresETbdsResponse>(url, { token })
  }

  // Métodos mockados (mantidos para compatibilidade)
  async getColaboradoresOptions(): Promise<ColaboradorOptionMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...colaboradoresOptionMock]
  }

  async getUnidadesOptions(): Promise<UnidadeOptionMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...unidadesOptionMock]
  }

  /**
   * Lista notas fiscais por vigência (visão gestor)
   */
  async listarNotasFiscaisPorVigenciaVisaoGestor(
    token: string,
    params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams
  ): Promise<ListarNotasFiscaisPorVigenciaVisaoGestorResponse> {
    const queryParams = new URLSearchParams()
    
    if (params?.filtro !== undefined && params.filtro !== '') {
      queryParams.append('filtro', params.filtro)
    }
    if (params?.mes !== undefined) {
      queryParams.append('mes', params.mes.toString())
    }
    if (params?.ano !== undefined) {
      queryParams.append('ano', params.ano.toString())
    }
    if (params?.statusId !== undefined && params.statusId !== '') {
      queryParams.append('statusId', params.statusId.toString())
    }
    if (params?.documentoColaborador !== undefined && params.documentoColaborador !== '') {
      queryParams.append('documentoColaborador', params.documentoColaborador)
    }
    if (params?.codDiretoria !== undefined && params.codDiretoria !== '') {
      queryParams.append('codDiretoria', params.codDiretoria)
    }
    if (params?.cursor !== undefined) {
      queryParams.append('cursor', params.cursor.toString())
    }
    if (params?.limite !== undefined) {
      queryParams.append('limite', params.limite.toString())
    }

    const queryString = queryParams.toString()
    const url = `/api/Financeiro/NotaFiscal/ListarNotasFiscaisPorVigenciaVisaoGestor${queryString ? `?${queryString}` : ''}`

    return httpClient.get<ListarNotasFiscaisPorVigenciaVisaoGestorResponse>(url, { token })
  }

  /**
   * Lista notas fiscais por vigência (visão colaborador)
   */
  async listarNotasFiscaisPorVigencia(
    token: string,
    params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams
  ): Promise<ListarNotasFiscaisPorVigenciaVisaoGestorResponse> {
    const queryParams = new URLSearchParams()
    
    if (params?.mes !== undefined) {
      queryParams.append('mes', params.mes.toString())
    }
    if (params?.ano !== undefined) {
      queryParams.append('ano', params.ano.toString())
    }
    if (params?.statusId !== undefined && params.statusId !== '') {
      queryParams.append('statusId', params.statusId.toString())
    }
    if (params?.cursor !== undefined) {
      queryParams.append('cursor', params.cursor.toString())
    }
    if (params?.limite !== undefined) {
      queryParams.append('limite', params.limite.toString())
    }

    const queryString = queryParams.toString()
    const url = `/api/Financeiro/NotaFiscal/ListarNotasFiscaisPorVigencia${queryString ? `?${queryString}` : ''}`

    return httpClient.get<ListarNotasFiscaisPorVigenciaVisaoGestorResponse>(url, { token })
  }

  /**
   * Aprovar notas fiscais (envia lista de ids; motivoReprovacao vazio ao aprovar)
   */
  async aprovarNotasFiscais(
    token: string,
    params: AprovarNotasFiscaisParams
  ): Promise<AprovarNotasFiscaisResponse> {
    return httpClient.post<AprovarNotasFiscaisResponse>(
      '/api/Financeiro/NotaFiscal/AprovarNotasFiscais',
      { ids: params.ids, motivoReprovacao: params.motivoReprovacao ?? '' },
      { token }
    )
  }

  /**
   * Reprovar notas fiscais (ids + motivoReprovacao obrigatório)
   */
  async reprovarNotasFiscais(
    token: string,
    params: ReprovarNotasFiscaisParams
  ): Promise<ReprovarNotasFiscaisResponse> {
    return httpClient.post<ReprovarNotasFiscaisResponse>(
      '/api/Financeiro/NotaFiscal/ReprovarNotasFiscais',
      { ids: params.ids, motivoReprovacao: params.motivoReprovacao },
      { token }
    )
  }

  /**
   * Liberar emissão de notas fiscais por vigência
   */
  async liberarEmissaoNotasFiscaisPorVigencia(
    token: string,
    params: LiberarEmissaoNotasFiscaisPorVigenciaParams
  ): Promise<LiberarEmissaoNotasFiscaisPorVigenciaResponse> {
    return httpClient.post<LiberarEmissaoNotasFiscaisPorVigenciaResponse>(
      '/api/Financeiro/NotaFiscal/LiberarEmissaoDeNotasFiscaisPorVigencia',
      {
        mes: params.mes,
        ano: params.ano,
        enviarEmail: params.enviarEmail,
        codigoDiretoria: params.codigoDiretoria,
      },
      { token }
    )
  }

  /**
   * Lista rubricas do colaborador para liberação de NF
   */
  async listarRubricasColaboradorParaLiberacaoDeNf(
    token: string,
    params: ListarRubricasColaboradorParaLiberacaoDeNfParams
  ): Promise<ListarRubricasColaboradorParaLiberacaoDeNfResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('mes', params.mes.toString())
    queryParams.append('ano', params.ano.toString())

    const queryString = queryParams.toString()
    const url = `/api/Financeiro/NotaFiscal/ListarRubricasColaboradorParaLiberacaoDeNf${queryString ? `?${queryString}` : ''}`

    return httpClient.get<ListarRubricasColaboradorParaLiberacaoDeNfResponse>(url, { token })
  }

  /**
   * Insere nota fiscal com upload de arquivo
   */
  async inserirNotaFiscal(
    token: string,
    params: InserirNotaFiscalParams
  ): Promise<InserirNotaFiscalResponse> {
    return httpClient.post<InserirNotaFiscalResponse>(
      '/api/Financeiro/NotaFiscal/InserirNotaFiscal',
      {
        base64Objeto: params.base64Objeto,
        numeroNf: params.numeroNf,
        vigenciaMes: params.vigenciaMes,
        vigenciaAno: params.vigenciaAno,
        listaDeIdsRubricasLiberacao: params.listaDeIdsRubricasLiberacao,
      },
      { token }
    )
  }
}

