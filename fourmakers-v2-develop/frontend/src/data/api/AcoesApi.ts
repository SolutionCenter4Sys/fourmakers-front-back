import { httpClient } from './httpClient'

import type { KanbanResponse, AcaoResponse } from '@domain/entities/AgendaGestor'

export class AcoesApi {
  async buscarKanbanEncontrosAcoesComerciais(token: string): Promise<KanbanResponse> {
    return httpClient.get<KanbanResponse>(
      '/api/Social/JornadaComercialApp/BuscarKanbanEncontrosAcoesComerciais',
      { token },
    )
  }

  async atualizarStatusAcoes(
    token: string,
    payload: import('@domain/entities/AgendaGestor').AtualizarStatusAcoesPayload,
  ): Promise<AcaoResponse> {
    return httpClient.put<AcaoResponse>(
      '/api/Social/JornadaComercialApp/AtualizarStatusAcoes',
      payload,
      { token },
    )
  }

  async inserirComentarioAcao(
    token: string,
    payload: import('@domain/entities/AgendaGestor').InserirComentarioAcaoPayload,
  ): Promise<AcaoResponse> {
    return httpClient.post<AcaoResponse>(
      '/api/Social/JornadaComercialApp/InsercaoComentariosAcoes',
      payload,
      { token },
    )
  }
}
