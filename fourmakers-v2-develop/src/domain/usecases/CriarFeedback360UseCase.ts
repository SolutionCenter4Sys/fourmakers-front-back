import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  Feedback360CriarPayload,
  Feedback360CreateResponse,
} from '@domain/entities/Feedback360';
import type { Feedback360Repository } from '@domain/repositories/Feedback360Repository';

@injectable()
export class CriarFeedback360UseCase {
  constructor(
    @inject(DiTokens.feedback360Repository)
    private readonly repository: Feedback360Repository,
  ) {}

  async execute(token: string, payload: Feedback360CriarPayload): Promise<Feedback360CreateResponse> {
    return this.repository.criar(token, payload);
  }
}
