import { inject, injectable } from 'tsyringe'
import { DiTokens } from '@core/di/tokens'
import type {
  EncontrosBigNumbersRepository,
  ObterBigNumbersParams,
} from '@domain/repositories/EncontrosBigNumbersRepository'
import type { EncontrosBigNumbersApi } from '@data/api/EncontrosBigNumbersApi'
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

@injectable()
export class EncontrosBigNumbersRepositoryImpl implements EncontrosBigNumbersRepository {
  constructor(
    @inject(DiTokens.encontrosBigNumbersApi)
    private readonly api: EncontrosBigNumbersApi,
  ) {}

  async obterBigNumbers(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<EncontrosBigNumbersRetorno> {
    const response = await this.api.obterBigNumbers(token, params)
    if (!response?.sucesso || response.retorno == null) {
      throw new Error(response?.mensagem ?? 'Erro ao obter Big Numbers de encontros.')
    }
    // Normaliza nomes da API (reunioes* vs encontros*) para contrato interno
    const r = response.retorno as unknown as Record<string, unknown>
    return {
      encontrosRealizados: Number(r.reunioesRealizados ?? r.encontrosRealizados ?? 0),
      encontrosSemInteracao: Number(r.reunioesSemInteracao ?? r.encontrosSemInteracao ?? 0),
      acoesEmAtraso: Number(r.acoesEmAtraso ?? 0),
      clientesImpactados: Number(r.clientesImpactados ?? 0),
      gestoresImpactados: Number(r.gestoresImpactados ?? 0),
      categoriasComInteracao: Number(r.categoriasComInteracao ?? 0),
    }
  }

  async obterBigNumbersCategoria(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<BigNumbersCategoriaItem[]> {
    const response = await this.api.obterBigNumbersCategoria(token, params)
    if (!response?.sucesso || !Array.isArray(response.retorno)) {
      return []
    }
    return response.retorno
  }

  async agendaRealizadaDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<AgendaRealizadaDetalhe[]> {
    const response = await this.api.obterAgendaRealizadaDetalhe(token, params)
    if (!response?.sucesso) {
      throw new Error(response?.mensagem ?? 'Erro ao obter detalhe de agendas realizadas.')
    }
    const lista = response.retorno ?? (response as { data?: AgendaRealizadaDetalhe[] }).data
    return Array.isArray(lista) ? lista : []
  }

  async clientesImpactadosDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<ClienteImpactadoDetalhe[]> {
    const response = await this.api.obterClientesImpactadosDetalhe(token, params)
    if (!response?.sucesso) {
      throw new Error(response?.mensagem ?? 'Erro ao obter detalhe de clientes impactados.')
    }
    return Array.isArray(response.retorno) ? response.retorno : []
  }

  async agendaSemInteracaoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<AgendaSemInteracaoDetalhe[]> {
    const response = await this.api.obterAgendaSemInteracaoDetalhe(token, params)
    if (!response?.sucesso) {
      throw new Error(response?.mensagem ?? 'Erro ao obter detalhe de agendas sem interação.')
    }
    return Array.isArray(response.retorno) ? response.retorno : []
  }

  async acoesEmAtrasoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<AcoesEmAtrasoDetalhe[]> {
    const response = await this.api.obterAcoesEmAtrasoDetalhe(token, params)
    if (!response?.sucesso) {
      throw new Error(response?.mensagem ?? 'Erro ao obter detalhe de ações em atraso.')
    }
    return Array.isArray(response.retorno) ? response.retorno : []
  }

  async categoriaComInteracaoDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<CategoriaComInteracaoDetalhe[]> {
    const response = await this.api.obterCategoriaComInteracaoDetalhe(token, params)
    if (!response?.sucesso) {
      throw new Error(response?.mensagem ?? 'Erro ao obter detalhe de categorias com interação.')
    }
    return Array.isArray(response.retorno) ? response.retorno : []
  }

  async gestoresImpactadosDetalhe(
    token: string,
    params: ObterBigNumbersParams,
  ): Promise<GestoresImpactadosDetalhe[]> {
    const response = await this.api.obterGestoresImpactadosDetalhe(token, params)
    if (!response?.sucesso) {
      throw new Error(response?.mensagem ?? 'Erro ao obter detalhe de gestores impactados.')
    }
    return Array.isArray(response.retorno) ? response.retorno : []
  }
}
