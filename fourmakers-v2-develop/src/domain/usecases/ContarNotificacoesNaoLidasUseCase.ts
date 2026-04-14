import { inject, injectable } from 'tsyringe'
import type { NotificacaoRepository } from '@domain/repositories/NotificacaoRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ContarNotificacoesNaoLidasUseCase {
  constructor(
    @inject(DiTokens.notificacaoRepository)
    private readonly repository: NotificacaoRepository,
  ) {}

  async execute(token: string): Promise<number> {
    if (!token) {
      throw new Error('Token é obrigatório')
    }

    const response = await this.repository.contarNotificacoesNaoLidas(token)
    return response.retorno
  }
}

