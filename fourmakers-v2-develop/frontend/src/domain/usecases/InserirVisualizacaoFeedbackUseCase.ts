import { inject, injectable } from 'tsyringe'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { InserirVisualizacaoFeedbackResponse } from '@domain/entities/GestaoDesempenhoGestor'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirVisualizacaoFeedbackUseCase {
  constructor(
    @inject(DiTokens.gestaoDesempenhoRepository)
    private readonly repository: GestaoDesempenhoRepository,
  ) {}

  async execute(token: string, feedbackId: string): Promise<InserirVisualizacaoFeedbackResponse> {
    return this.repository.inserirVisualizacaoFeedback(token, feedbackId)
  }
}
