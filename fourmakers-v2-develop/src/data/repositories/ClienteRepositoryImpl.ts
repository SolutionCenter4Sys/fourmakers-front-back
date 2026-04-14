import { injectable } from 'tsyringe'

import type { ClienteListItem } from '@domain/entities/Cliente'
import type { ClienteRepository } from '@domain/repositories/ClienteRepository'
import { fetchClientes } from '@data/api/PerfilAtuacaoApi'

@injectable()
export class ClienteRepositoryImpl implements ClienteRepository {
  async listar(token: string, busca: string): Promise<ClienteListItem[]> {
    const retorno = await fetchClientes(token, busca)
    return retorno.map((c) => ({
      id: c.codigoCliente,
      name: c.nomeCliente,
    }))
  }
}
