import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  Feedback360AtualizarPayload,
  Feedback360CreateResponse,
} from '@domain/entities/Feedback360';
import type { Feedback360Repository } from '@domain/repositories/Feedback360Repository';

@injectable()
export class AtualizarFeedback360UseCase {
  constructor(
    @inject(DiTokens.feedback360Repository)
    private readonly repository: Feedback360Repository,
  ) {}

  async execute(token: string, id: string, payload: Feedback360AtualizarPayload): Promise<Feedback360CreateResponse> {
    return this.repository.atualizar(token, id, payload);
  }
}
