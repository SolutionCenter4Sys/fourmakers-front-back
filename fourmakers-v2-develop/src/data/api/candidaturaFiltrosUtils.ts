import type { FiltrosCandidaturaParams } from '@domain/entities/DashboardMetricasRecrutamento'

/** Monta um único segmento query: nome=valor1,valor2 (sem repetir o nome). */
function segmento(nome: string, valores: string[]): string {
  if (valores.length === 0) return ''
  return `${encodeURIComponent(nome)}=${encodeURIComponent(valores.join(','))}`
}

type NomeParamRecrutador = 'CodigoRecrutador' | 'CodigoGestor'

/** Base: monta query com DataInicio, DataFim, CodigoCliente e um parâmetro de recrutador/gestor (nome configurável). */
function buildCandidaturaQueryParamsBase(
  filtros: FiltrosCandidaturaParams,
  nomeParamRecrutador: NomeParamRecrutador,
): string {
  const partes: string[] = []
  if (filtros.DataInicio) {
    partes.push(`DataInicio=${encodeURIComponent(filtros.DataInicio)}`)
  }
  if (filtros.DataFim) {
    partes.push(`DataFim=${encodeURIComponent(filtros.DataFim)}`)
  }
  if (filtros.CodigoCliente?.length) {
    partes.push(segmento('CodigoCliente', filtros.CodigoCliente))
  }
  if (filtros.CodigoRecrutador?.length) {
    partes.push(segmento(nomeParamRecrutador, filtros.CodigoRecrutador))
  }
  return partes.join('&')
}

/**
 * Constrói a query string com CodigoCliente e CodigoRecrutador como listas separadas por vírgula.
 * Resultado: CodigoCliente=id1,id2,id3&CodigoRecrutador=id1,id2 (uma única chave por parâmetro).
 */
export function buildCandidaturaQueryParams(filtros: FiltrosCandidaturaParams): string {
  return buildCandidaturaQueryParamsBase(filtros, 'CodigoRecrutador')
}

/**
 * Constrói a query string para dashboardMetricasRecrutamento com CodigoRecrutador.
 */
export function buildCandidaturaQueryParamsRecrutamento(filtros: FiltrosCandidaturaParams): string {
  return buildCandidaturaQueryParamsBase(filtros, 'CodigoRecrutador')
}

/**
 * Constrói a query string para dashboardMetricasFunilDeVagas: CodigoGestor (lista por vírgula) e CodigoCliente (lista por vírgula).
 * Resultado: CodigoGestor=id1,id2&CodigoCliente=id1,id2 (uma única chave por parâmetro).
 */
export function buildCandidaturaQueryParamsFunil(filtros: FiltrosCandidaturaParams): string {
  return buildCandidaturaQueryParamsBase(filtros, 'CodigoGestor')
}
