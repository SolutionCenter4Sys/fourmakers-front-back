import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  InserirInteracaoComentarioPayload,
  InserirInteracaoComentarioResponse,
} from '@domain/entities/comunicacao';
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';

@injectable()
export class InserirInteracaoComentarioUseCase {
  constructor(
    @inject(DiTokens.comunicacaoFeedRepository)
    private readonly repository: ComunicacaoFeedRepository,
  ) {}

  async execute(
    token: string,
    payload: InserirInteracaoComentarioPayload,
  ): Promise<InserirInteracaoComentarioResponse> {
    return this.repository.inserirInteracaoComentario(token, payload);
  }
}
