import type {
  ContarNotificacoesNaoLidasResponse,
  ListarNotificacoesResponse,
  MarcarNotificacoesComoLidasResponse,
} from '@domain/entities/Notificacao'

export interface NotificacaoRepository {
  contarNotificacoesNaoLidas(token: string): Promise<ContarNotificacoesNaoLidasResponse>
  listarNotificacoes(token: string): Promise<ListarNotificacoesResponse>
  marcarNotificacoesComoLidas(token: string): Promise<MarcarNotificacoesComoLidasResponse>
}

