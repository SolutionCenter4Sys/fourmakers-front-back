import { httpClient } from './httpClient'

import type {
  ClientesMapaRelacionamentoResponse,
  GestoresExternosResponse,
  PerfisExternosResponse,
} from '@domain/entities/MapaRelacionamento'
import type { DepartamentoResponse } from '@domain/entities/Organograma'

export class MapaRelacionamentoApi {
  async listarClientes(token: string, limite: number): Promise<ClientesMapaRelacionamentoResponse> {
    const queryParams = new URLSearchParams({
      limite: limite.toString(),
    })

    return httpClient.get<ClientesMapaRelacionamentoResponse>(
      `/api/GestaoDeAlocados/ListarClienteOrgDaGestaoDeAlocados?${queryParams.toString()}`,
      { token },
    )
  }

  async listarGestoresExternos(
    token: string,
    codigoCliente: string,
    limite: number,
  ): Promise<GestoresExternosResponse> {
    const queryParams = new URLSearchParams({
      codigoCliente,
      limite: limite.toString(),
    })

    return httpClient.get<GestoresExternosResponse>(
      `/api/GestaoDeAlocados/GestorExterno/ListarGestoresExterno?${queryParams.toString()}`,
      { token },
    )
  }

  async listarPerfisExternos(
    token: string,
    codigoCliente: string,
    limite: number,
  ): Promise<PerfisExternosResponse> {
    const queryParams = new URLSearchParams({
      codigoCliente,
      limite: limite.toString(),
    })

    return httpClient.get<PerfisExternosResponse>(
      `/api/GestaoDeAlocados/ListarGestoresEPerfisDaGestaoDeAlocados?${queryParams.toString()}`,
      { token },
    )
  }

  async listarDepartamentos(token: string, codigoCliente: string): Promise<DepartamentoResponse[]> {
    const queryParams = new URLSearchParams({
      codCliente: codigoCliente,
    })
    return httpClient.get<DepartamentoResponse[]>(
      `/api/Organograma/DepartamentoListarPorCliente?${queryParams.toString()}`,
      { token },
    )
  }
}
