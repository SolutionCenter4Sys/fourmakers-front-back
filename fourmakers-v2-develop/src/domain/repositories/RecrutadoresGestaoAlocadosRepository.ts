import type { RecrutadoresGestaoAlocadosResponse } from '@domain/entities/RecrutadorGestaoAlocados'

export interface RecrutadoresGestaoAlocadosRepository {
  listarRecrutadores(
    token: string,
    busca: string,
  ): Promise<RecrutadoresGestaoAlocadosResponse>
}
