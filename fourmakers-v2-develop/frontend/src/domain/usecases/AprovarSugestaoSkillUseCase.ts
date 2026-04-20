import { inject, injectable } from 'tsyringe'
import type { MinhaEquipeRepository } from '@domain/repositories/MinhaEquipeRepository'
import type { AprovarRejeitarSugestaoPayload } from '@domain/entities/MinhaEquipeSugestao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class AprovarSugestaoSkillUseCase {
  constructor(
    @inject(DiTokens.minhaEquipeRepository)
    private readonly repository: MinhaEquipeRepository
  ) {}

  async execute(
    token: string,
    payload: AprovarRejeitarSugestaoPayload
  ): Promise<void> {
    await this.repository.aprovarRejeitarSugestao(token, payload)
  }
}
