import { httpClient } from './httpClient'
import type {
  ContarNotificacoesNaoLidasResponse,
  ListarNotificacoesResponse,
  MarcarNotificacoesComoLidasResponse,
} from '@domain/entities/Notificacao'

export class NotificacaoApi {
  async contarNotificacoesNaoLidas(token: string): Promise<ContarNotificacoesNaoLidasResponse> {
    return httpClient.get<ContarNotificacoesNaoLidasResponse>(
      '/api/Notificacao/ContarNotificacoesNaoLidasColaborador',
      { token }
    )
  }

  async listarNotificacoes(token: string): Promise<ListarNotificacoesResponse> {
    return httpClient.get<ListarNotificacoesResponse>(
      '/api/Notificacao/ListarNotificacoesColaborador',
      { token }
    )
  }

  async marcarNotificacoesComoLidas(token: string): Promise<MarcarNotificacoesComoLidasResponse> {
    return httpClient.post<MarcarNotificacoesComoLidasResponse>(
      '/api/Notificacao/MarcarNotificacoesComoLidasColaborador',
      undefined,
      { token }
    )
  }
}

