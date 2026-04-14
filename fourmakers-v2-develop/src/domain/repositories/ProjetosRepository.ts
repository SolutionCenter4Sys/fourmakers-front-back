import type { ProjetoMock } from '@data/mocks/projetosMock'
import type { ListarStatusProjetoResponse } from '@data/api/ProjetosApi'

export interface ProjetosRepository {
  getProjetos(): Promise<ProjetoMock[]>
  listarStatusProjeto(token: string): Promise<ListarStatusProjetoResponse>
}

