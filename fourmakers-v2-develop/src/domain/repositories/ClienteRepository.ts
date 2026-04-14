import type { ClienteListItem } from '@domain/entities/Cliente'

export interface ClienteRepository {
  listar(token: string, busca: string): Promise<ClienteListItem[]>
}
