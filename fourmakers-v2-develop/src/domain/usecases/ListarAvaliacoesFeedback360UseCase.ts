import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { Feedback360AvaliacaoItem } from '@domain/entities/Feedback360';
import type { Feedback360Repository } from '@domain/repositories/Feedback360Repository';

@injectable()
export class ListarAvaliacoesFeedback360UseCase {
  constructor(
    @inject(DiTokens.feedback360Repository)
    private readonly repository: Feedback360Repository,
  ) {}

  async execute(token: string): Promise<Feedback360AvaliacaoItem[]> {
    return this.repository.listarAvaliacoes(token);
  }
}
