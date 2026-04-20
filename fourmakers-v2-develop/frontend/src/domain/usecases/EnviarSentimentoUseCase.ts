import { inject, injectable } from 'tsyringe'
import type { FelizometroRepository } from '@domain/repositories/FelizometroRepository'
import type { FelizometroPayload } from '@domain/entities/FelizometroSentimento'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class EnviarSentimentoUseCase {
  constructor(
    @inject(DiTokens.felizometroRepository)
    private readonly repository: FelizometroRepository,
  ) {}

  async execute(payload: FelizometroPayload): Promise<void> {
    // Validações
    if (!payload.collaborator_id) {
      throw new Error('ID do colaborador é obrigatório')
    }

    if (!payload.emotion) {
      throw new Error('Sentimento é obrigatório')
    }

    if (!payload.date) {
      throw new Error('Data é obrigatória')
    }

    await this.repository.enviarSentimento(payload)
  }
}

