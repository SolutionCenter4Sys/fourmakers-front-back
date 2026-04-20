import { inject, injectable } from 'tsyringe'
import type { NotificacaoRepository } from '@domain/repositories/NotificacaoRepository'
import type { Notificacao } from '@domain/entities/Notificacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarNotificacoesUseCase {
  constructor(
    @inject(DiTokens.notificacaoRepository)
    private readonly repository: NotificacaoRepository,
  ) {}

  async execute(token: string): Promise<Notificacao[]> {
    if (!token) {
      throw new Error('Token é obrigatório')
    }

    const response = await this.repository.listarNotificacoes(token)
    return response.retorno
  }
}

