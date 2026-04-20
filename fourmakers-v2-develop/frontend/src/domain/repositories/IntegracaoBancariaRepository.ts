import type { ListarDiretoriasDisponiveisResponse } from '@domain/entities/Diretoria'

export interface IntegracaoBancariaRepository {
  listarDiretoriasDisponiveis(token: string): Promise<ListarDiretoriasDisponiveisResponse>
}

