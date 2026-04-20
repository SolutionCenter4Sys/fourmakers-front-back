import type {
  ClientesMapaRelacionamentoResponse,
  GestoresExternosResponse,
  PerfisExternosResponse,
} from '@domain/entities/MapaRelacionamento'
import type { DepartamentoResponse } from '@domain/entities/Organograma'

export interface ListarGestoresParams {
  codigoCliente: string
  limite: number
}

export interface ListarPerfisParams {
  codigoCliente: string
  limite: number
}

export interface ListarClientesParams {
  orgId: number
  limite: number
  cursor?: number
  nomeCliente?: string
}

export interface MapaRelacionamentoRepository {
  listarClientes(token: string, params: ListarClientesParams): Promise<ClientesMapaRelacionamentoResponse>
  listarClientesAlternativo(token: string, limite: number): Promise<ClientesMapaRelacionamentoResponse>
  listarGestoresExternos(token: string, params: ListarGestoresParams): Promise<GestoresExternosResponse>
  listarPerfisExternos(token: string, params: ListarPerfisParams): Promise<PerfisExternosResponse>
  listarDepartamentos(token: string, codigoCliente: string): Promise<DepartamentoResponse[]>
}
