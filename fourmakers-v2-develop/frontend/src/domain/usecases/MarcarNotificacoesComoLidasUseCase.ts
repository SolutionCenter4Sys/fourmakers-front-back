import { inject, injectable } from 'tsyringe'
import type { NotificacaoRepository } from '@domain/repositories/NotificacaoRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class MarcarNotificacoesComoLidasUseCase {
  constructor(
    @inject(DiTokens.notificacaoRepository)
    private readonly repository: NotificacaoRepository,
  ) {}

  async execute(token: string): Promise<void> {
    if (!token) {
      throw new Error('Token é obrigatório')
    }

    await this.repository.marcarNotificacoesComoLidas(token)
  }
}

