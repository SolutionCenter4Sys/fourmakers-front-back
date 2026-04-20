import { httpClient } from './httpClient'
import type { ApiGenericResult } from '@domain/entities/VcxDores'
import type {
  EncontrosBigNumbersRetorno,
  BigNumbersCategoriaItem,
  AgendaRealizadaDetalhe,
  ClienteImpactadoDetalhe,
  AgendaSemInteracaoDetalhe,
  AcoesEmAtrasoDetalhe,
  CategoriaComInteracaoDetalhe,
  GestoresImpactadosDetalhe,
} from '@shared/types/dashboardComercialTypes'
import type { ObterBigNumbersParams } from '@domain/repositories/EncontrosBigNumbersRepository'

/** Monta query params. DataInicio/DataFim: tratar como data (YYYY-MM-DD), sem normalizar com timezone. */
function buildQueryParams(params: ObterBigNumbersParams): URLSearchParams {
  const searchParams = new URLSearchParams()
  if (params.periodo != null && typeof params.periodo === 'number' && !Number.isNaN(params.periodo)) {
    searchParams.set('Periodo', String(params.periodo))
  }
  if (params.dataInicio != null && params.dataInicio.trim() !== '') {
    searchParams.set('DataInicio', params.dataInicio.trim())
  }
  if (params.dataFim != null && params.dataFim.trim() !== '') {
    searchParams.set('DataFim', params.dataFim.trim())
  }
  const codigoCliente = params.codigoCliente
  if (codigoCliente != null && typeof codigoCliente === 'string' && codigoCliente.trim() !== '') {
    searchParams.set('CodigoCliente', codigoCliente.trim())
  }
  const codigoColaborador = params.codigoColaboradorAgendou
  if (codigoColaborador != null && typeof codigoColaborador === 'string' && codigoColaborador.trim() !== '') {
    searchParams.set('CodigoColaboradorAgendou', codigoColaborador.trim())
  }
  const codigoGestor = params.codigoGestorExterno
  if (codigoGestor != null && typeof codigoGestor === 'string' && codigoGestor.trim() !== '') {
    searchParams.set('CodigoGestorExterno', codigoGestor.trim())
  }
  return searchParams
}

export class EncontrosBigNumbersApi {
  async obterBigNumbers(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ApiGenericResult<EncontrosBigNumbersRetorno>> {
    const queryParams = buildQueryParams(params)
    return httpClient.get<ApiGenericResult<EncontrosBigNumbersRetorno>>(
      `/api/Social/EncontrosBigNumbers/ObterBigNumbers?${queryParams.toString()}`,
      { token },
    )
  }

  async obterBigNumbersCategoria(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ApiGenericResult<BigNumbersCategoriaItem[]>> {
    const queryParams = buildQueryParams(params)
    return httpClient.get<ApiGenericResult<BigNumbersCategoriaItem[]>>(
      `/api/Social/EncontrosBigNumbers/ObterBigNumbersCategoria?${queryParams.toString()}`,
      { token },
    )
  }

  async obterAgendaRealizadaDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ApiGenericResult<AgendaRealizadaDetalhe[]>> {
    const queryParams = buildQueryParams(params)
    return httpClient.get<ApiGenericResult<AgendaRealizadaDetalhe[]>>(
      `/api/Social/EncontrosBigNumbers/AgendaRealizadaDetalhe?${queryParams.toString()}`,
      { token },
    )
  }

  async obterClientesImpactadosDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ApiGenericResult<ClienteImpactadoDetalhe[]>> {
    const queryParams = buildQueryParams(params)
    return httpClient.get<ApiGenericResult<ClienteImpactadoDetalhe[]>>(
      `/api/Social/EncontrosBigNumbers/ClientesImpactadosDetalhe?${queryParams.toString()}`,
      { token },
    )
  }

  async obterAgendaSemInteracaoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ApiGenericResult<AgendaSemInteracaoDetalhe[]>> {
    const queryParams = buildQueryParams(params)
    return httpClient.get<ApiGenericResult<AgendaSemInteracaoDetalhe[]>>(
      `/api/Social/EncontrosBigNumbers/AgendaSemInteracaoDetalhe?${queryParams.toString()}`,
      { token },
    )
  }

  async obterAcoesEmAtrasoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ApiGenericResult<AcoesEmAtrasoDetalhe[]>> {
    const queryParams = buildQueryParams(params)
    return httpClient.get<ApiGenericResult<AcoesEmAtrasoDetalhe[]>>(
      `/api/Social/EncontrosBigNumbers/AcoesEmAtrasoDetalhe?${queryParams.toString()}`,
      { token },
    )
  }

  async obterCategoriaComInteracaoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ApiGenericResult<CategoriaComInteracaoDetalhe[]>> {
    const queryParams = buildQueryParams(params)
    return httpClient.get<ApiGenericResult<CategoriaComInteracaoDetalhe[]>>(
      `/api/Social/EncontrosBigNumbers/CategoriaComInteracaoDetalhe?${queryParams.toString()}`,
      { token },
    )
  }

  async obterGestoresImpactadosDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ApiGenericResult<GestoresImpactadosDetalhe[]>> {
    const queryParams = buildQueryParams(params)
    return httpClient.get<ApiGenericResult<GestoresImpactadosDetalhe[]>>(
      `/api/Social/EncontrosBigNumbers/GestoresImpactadosDetalhe?${queryParams.toString()}`,
      { token },
    )
  }
}
