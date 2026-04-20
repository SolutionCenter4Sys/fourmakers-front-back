import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository';
import type {
  InserirFeedbackPayload,
  InserirFeedbackResponse
} from '@domain/entities/GestaoDesempenhoGestor';

@injectable()
export class InserirFeedbackGestaoDesempenhoUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository
  ) {}

  async execute(
    token: string,
    payload: InserirFeedbackPayload
  ): Promise<InserirFeedbackResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    if (!payload.codigoInternoColaboradorAvaliado) {
      throw new Error('codigoInternoColaboradorAvaliado é obrigatório');
    }

    return this.repository.inserirFeedback(token, payload);
  }
}
