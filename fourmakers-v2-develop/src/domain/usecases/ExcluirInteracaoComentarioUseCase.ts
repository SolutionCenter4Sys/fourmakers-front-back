import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  ExcluirInteracaoComentarioPayload,
  ExcluirInteracaoComentarioResponse,
} from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class ExcluirInteracaoComentarioUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    payload: ExcluirInteracaoComentarioPayload,
  ): Promise<ExcluirInteracaoComentarioResponse> {
    return this.repository.excluirInteracaoComentario(token, payload);
  }
}
