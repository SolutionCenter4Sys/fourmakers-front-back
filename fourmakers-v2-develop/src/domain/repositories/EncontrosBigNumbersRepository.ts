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

/** Parâmetros para obter Big Numbers e gráfico por categoria */
export interface ObterBigNumbersParams {
  /** Número de dias; null = "Todos" (parâmetro omitido na requisição) */
  periodo?: number | null
  /** Data inicial (YYYY-MM-DD); null = sem filtro */
  dataInicio?: string | null
  /** Data final (YYYY-MM-DD); null = sem filtro */
  dataFim?: string | null
  codigoCliente?: string | null
  codigoColaboradorAgendou?: string | null
  codigoGestorExterno?: string | null
}

export interface EncontrosBigNumbersRepository {
  obterBigNumbers(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<EncontrosBigNumbersRetorno>

  obterBigNumbersCategoria(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<BigNumbersCategoriaItem[]>

  /** Detalhe de agendas realizadas (drilldown Encontros Realizados) */
  agendaRealizadaDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<AgendaRealizadaDetalhe[]>

  /** Detalhe de clientes impactados (drilldown Clientes Impactados) */
  clientesImpactadosDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ClienteImpactadoDetalhe[]>

  /** Detalhe de agendas sem interação (drilldown Sem Interação) */
  agendaSemInteracaoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<AgendaSemInteracaoDetalhe[]>

  /** Detalhe de ações em atraso (drilldown Ações em Atraso) */
  acoesEmAtrasoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<AcoesEmAtrasoDetalhe[]>

  /** Detalhe de categorias com interação (drilldown Categorias com Interação) */
  categoriaComInteracaoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<CategoriaComInteracaoDetalhe[]>

  /** Detalhe de gestores impactados (drilldown Gestores Impactados) */
  gestoresImpactadosDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<GestoresImpactadosDetalhe[]>
}
