import { inject, injectable } from 'tsyringe'
import type { NotificacaoRepository } from '@domain/repositories/NotificacaoRepository'
import type {
  ContarNotificacoesNaoLidasResponse,
  ListarNotificacoesResponse,
  MarcarNotificacoesComoLidasResponse,
} from '@domain/entities/Notificacao'
import { NotificacaoApi } from '@data/api/NotificacaoApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class NotificacaoRepositoryImpl implements NotificacaoRepository {
  constructor(
    @inject(DiTokens.notificacaoApi)
    private readonly api: NotificacaoApi,
  ) {}

  async contarNotificacoesNaoLidas(token: string): Promise<ContarNotificacoesNaoLidasResponse> {
    return await this.api.contarNotificacoesNaoLidas(token)
  }

  async listarNotificacoes(token: string): Promise<ListarNotificacoesResponse> {
    return await this.api.listarNotificacoes(token)
  }

  async marcarNotificacoesComoLidas(token: string): Promise<MarcarNotificacoesComoLidasResponse> {
    return await this.api.marcarNotificacoesComoLidas(token)
  }
}

