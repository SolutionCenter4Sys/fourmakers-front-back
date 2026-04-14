import { inject, injectable } from 'tsyringe'

import type { ClienteListItem } from '@domain/entities/Cliente'
import type { ClienteRepository } from '@domain/repositories/ClienteRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarClientesUseCase {
  constructor(
    @inject(DiTokens.clienteRepository)
    private readonly repository: ClienteRepository,
  ) {}

  async execute(token: string, busca: string): Promise<ClienteListItem[]> {
    return this.repository.listar(token, busca)
  }
}
