import { inject, injectable } from 'tsyringe'

import type { MapaRelacionamentoRepository, ListarGestoresParams, ListarPerfisParams, ListarClientesParams } from '@domain/repositories/MapaRelacionamentoRepository'
import type {
  ClienteMapaRelacionamento,
  ClientesMapaRelacionamentoResponse,
  GestoresExternosResponse,
  PerfisExternosResponse,
} from '@domain/entities/MapaRelacionamento'
import type { DepartamentoResponse } from '@domain/entities/Organograma'
import type { ClientesGestaoAlocadosResponse } from '@domain/entities/ClienteGestaoAlocados'
import type { ClienteOrganogramaResponse } from '@shared/types/mapaRelacionamentoTypes'
import { MapaRelacionamentoApi } from '@data/api/MapaRelacionamentoApi'
import { OrganogramaApi } from '@data/api/OrganogramaApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class MapaRelacionamentoRepositoryImpl implements MapaRelacionamentoRepository {
  constructor(
    @inject(DiTokens.mapaRelacionamentoApi)
    private readonly api: MapaRelacionamentoApi,
    @inject(DiTokens.organogramaApi)
    private readonly organogramaApi: OrganogramaApi,
  ) {}

  async listarClientes(token: string, params: ListarClientesParams): Promise<ClientesMapaRelacionamentoResponse> {
    const response = await this.organogramaApi.retornarClientesPorOrgId(
      token,
      params.orgId,
      params.limite,
      params.cursor || 0,
      params.nomeCliente || '',
    )
    // Map API response to ensure all fields are present with defaults
    // O backend pode retornar campos opcionais, então fazemos type assertion para ClienteOrganogramaResponse
    return Array.isArray(response) 
      ? response.map((cliente: ClienteOrganogramaResponse): ClienteMapaRelacionamento => ({
          id: cliente.id,
          codigoCliente: cliente.codigoCliente,
          nomeCliente: cliente.nomeCliente,
          qtdAlocados: cliente.qtdAlocados ?? 0,
          qtdGestoresSemPerfil: cliente.qtdGestoresSemPerfil ?? 0,
          qtdGestores: cliente.qtdGestores ?? 0,
        }))
      : []
  }

  async listarClientesAlternativo(token: string, limite: number): Promise<ClientesMapaRelacionamentoResponse> {
    const response = await this.api.listarClientes(token, limite)
    
    // O endpoint alternativo retorna { retorno: ClienteGestaoAlocados[], sucesso: boolean }
    // Precisamos extrair o array de retorno e mapear para ClienteMapaRelacionamento
    const responseTyped = response as unknown as ClientesGestaoAlocadosResponse
    const clientesArray = responseTyped?.retorno || []
    
    return Array.isArray(clientesArray)
      ? clientesArray.map((cliente): ClienteMapaRelacionamento => ({
          id: cliente.codigoCliente, // Usar codigoCliente como id
          codigoCliente: cliente.codigoCliente,
          nomeCliente: cliente.nomeCliente,
          qtdAlocados: 0, // Endpoint alternativo não retorna esses campos
          qtdGestoresSemPerfil: 0,
          qtdGestores: 0,
        }))
      : []
  }

  async listarGestoresExternos(
    token: string,
    params: ListarGestoresParams,
  ): Promise<GestoresExternosResponse> {
    return this.api.listarGestoresExternos(token, params.codigoCliente, params.limite)
  }

  async listarPerfisExternos(
    token: string,
    params: ListarPerfisParams,
  ): Promise<PerfisExternosResponse> {
    return this.api.listarPerfisExternos(token, params.codigoCliente, params.limite)
  }

  async listarDepartamentos(token: string, codigoCliente: string): Promise<DepartamentoResponse[]> {
    return this.api.listarDepartamentos(token, codigoCliente)
  }
}
